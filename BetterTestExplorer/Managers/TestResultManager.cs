using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

using BetterTestExplorer.Common;

namespace BetterTestExplorer.Managers
{
    public interface ITestResultManager
    {
        /**********************************************************************/
        #region Properties

        IReadOnlyCollection<string> SourceAssemblyPaths { get; }

        IReadOnlyCollection<TestResult> TestResults { get; }

        #endregion Properties

        /**********************************************************************/
        #region Methods

        TestResult GetTestResult(Guid id);

        bool TryGetTestResult(Guid id, out TestResult testResult);

        Task AddSourceAssemblyPathAsync(string sourceAssemblyPath);

        Task RemoveSourceAssemblyPathAsync(string sourceAssemblyPath);

        #endregion Methods

        /**********************************************************************/
        #region Events

        event EventHandler<TestResultsEventArgs> TestResultsAdded;

        event EventHandler<TestResultsEventArgs> TestResultsModified;

        event EventHandler<TestResultsEventArgs> TestResultsRemoved;

        #endregion Events
    }

    internal class TestResultManager : ITestResultManager
    {
        /**********************************************************************/
        #region Constructors

        // If you think you need to use this, and you're not a Factory or Unit Test, you're wrongc
        internal TestResultManager(IFileSystem fileSystem, ITestCaseDiscoveryManager discoveryManager)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _discoveryManager = discoveryManager ?? throw new ArgumentNullException(nameof(discoveryManager));

            _discoveryManager.TestCasesDiscovered += OnTestCasesDiscovered;
            _discoveryManager.DiscoveryCompleted += OnDiscoveryComplete;
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCaseManager

        public IReadOnlyCollection<string> SourceAssemblyPaths => _sourceAssemblyPaths;
        private readonly HashSet<string> _sourceAssemblyPaths = new HashSet<string>();

        public IReadOnlyCollection<TestCase> TestResults => _testResultsByTestCaseId.Values;

        public TestResult GetTestResult(Guid id)
        {
            if (!_testResultsByTestCaseId.TryGetValue(id, out var testResult))
                throw new ArgumentException($"No {nameof(TestResult)} exists with the given {nameof(TestResult.TestCase)}.{nameof(TestCase.Id)} value", nameof(id));

            return testResult;
        }

        public bool TryGetTestResult(Guid id, out TestResult testResult)
        {
            return _testResultsByTestCaseId.TryGetValue(id, out testResult);
        }

        public async Task AddSourceAssemblyPathAsync(string sourceAssemblyPath)
        {
            if (sourceAssemblyPath == null)
                throw new ArgumentNullException(nameof(sourceAssemblyPath));

            try
            {
                sourceAssemblyPath = _fileSystem.Path.GetFullPath(sourceAssemblyPath);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(sourceAssemblyPath), ex);
            }

            if (_sourceAssemblyPaths.Contains(sourceAssemblyPath))
                throw new ArgumentException($"\"{sourceAssemblyPath}\" already exists in {nameof(SourceAssemblyPaths)}", nameof(sourceAssemblyPath));


            while (_discoveryManager.IsDiscoveryInProgress)
                await _discoveryManager.WaitForDiscoveryCompleteAsync();

            _sourceAssemblyPaths.Add(sourceAssemblyPath);

            await _discoveryManager.DiscoverTestCasesAsync(Enumerable.Repeat(sourceAssemblyPath, 1));
        }

        public async Task RemoveSourceAssemblyPathAsync(string sourceAssemblyPath)
        {
            if (sourceAssemblyPath == null)
                throw new ArgumentNullException(nameof(sourceAssemblyPath));

            try
            {
                sourceAssemblyPath = _fileSystem.Path.GetFullPath(sourceAssemblyPath);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(sourceAssemblyPath), ex);
            }

            if(!_sourceAssemblyPaths.Contains(sourceAssemblyPath))
                throw new ArgumentException($"\"{sourceAssemblyPath}\" does not exist in {nameof(SourceAssemblyPaths)}", nameof(sourceAssemblyPath));

            while (_discoveryManager.IsDiscoveryInProgress)
                await _discoveryManager.WaitForDiscoveryCompleteAsync();

            _sourceAssemblyPaths.Remove(sourceAssemblyPath);

            var removedTestResults = new List<TestResult>(_testResultsByTestCaseId.Count);
            foreach (var testResult in _testResultsByTestCaseId.Values)
                if (testResult.Source == sourceAssemblyPath)
                    removedTestResults.Add(testResult);

            foreach (var test in removedTestResults)
                _testResultsByTestCaseId.Remove(test.Id);

            if(removedTestResults.Count > 0)
                RaiseTestCasesRemoved(removedTestResults);
        }

        public event EventHandler<TestResultsEventArgs> TestResultsAdded;
        private void RaiseTestCasesAdded(IEnumerable<TestResult> testCases)
        {
            TestResultsAdded?.Invoke(this, new TestResultsEventArgs(testCases));
        }

        public event EventHandler<TestResultsEventArgs> TestResultsModified;
        private void RaiseTestCasesModified(IEnumerable<TestResult> testResults)
        {
            TestResultsModified?.Invoke(this, new TestResultsEventArgs(testResults));
        }

        public event EventHandler<TestResultsEventArgs> TestResultsRemoved;
        private void RaiseTestCasesRemoved(IEnumerable<TestResult> testResults)
        {
            TestResultsRemoved?.Invoke(this, new TestResultsEventArgs(testResults));
        }

        #endregion ITestCaseManager

        /**********************************************************************/
        #region Event Handlers

        private void OnTestCasesDiscovered(object sender, DiscoveredTestsEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var discoveredTestCasesCount = e.DiscoveredTestCases.Count();
            var addedTestResults = new List<TestResult>(discoveredTestCasesCount);
            var modifiedTestResults = new List<TestResult>(discoveredTestCasesCount);

            foreach (var testCase in e.DiscoveredTestCases)
            {
                if (_testResultsByTestCaseId.ContainsKey(testCase.Id))
                {
                    var oldTestResult = _testResultsByTestCaseId[testCase.Id];
                    var testResult = new TestResult(testCase);
                    testResult.Duration = oldTestResult.Duration;
                    testResult.Outcome = oldTestResult.Outcome;
                    testResult.
                    _testResultsByTestCaseId[testCase.Id] = testResult;
                    modifiedTestResults.Add(testResult);
                }
                else
                {
                    var testResult = new TestResult(testCase);
                    _testResultsByTestCaseId.Add(testCase.Id, testResult);
                    addedTestResults.Add(testResult);
                }

                _discoverySessionTestCaseIds.Add(testCase.Id);
            }

            if(addedTestResults.Count > 0)
                RaiseTestCasesAdded(addedTestResults);
            if(modifiedTestResults.Count > 0)
                RaiseTestCasesModified(modifiedTestResults);
        }

        private void OnDiscoveryComplete(object sender, DiscoveryCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!e.WasDiscoveryAborted)
            {
                var removedTestCases = new List<TestCase>(_testResultsByTestCaseId.Count - _discoverySessionTestCaseIds.Count);
                foreach (var testCase in _testResultsByTestCaseId.Where(x => e.SourceAssemblyPaths.Contains(x.Value.Source))
                                                       .Where(x => !_discoverySessionTestCaseIds.Contains(x.Value.Id))
                                                       .Select(x => x.Value))
                    removedTestCases.Add(testCase);

                foreach (var testCase in removedTestCases)
                    _testResultsByTestCaseId.Remove(testCase.Id);

                if(removedTestCases.Count > 0)
                    RaiseTestCasesRemoved(removedTestCases);

                _discoverySessionTestCaseIds.Clear();
            }
        }

        #endregion Event Handlers

        /**********************************************************************/
        #region Private Fields

        private readonly IFileSystem _fileSystem;

        private readonly ITestCaseDiscoveryManager _discoveryManager;

        private readonly Dictionary<Guid, TestCase> _testResultsByTestCaseId = new Dictionary<Guid, TestCase>();

        private readonly HashSet<Guid> _discoverySessionTestCaseIds = new HashSet<Guid>();

        #endregion Private Fields
    }
}
