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
using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorer.Managers
{
    public interface ITestResultManager
    {
        /**********************************************************************/
        #region Properties

        IReadOnlyCollection<string> SourceAssemblyPaths { get; }

        IReadOnlyCollection<ITestResult> TestResults { get; }

        #endregion Properties

        /**********************************************************************/
        #region Methods

        Task AddSourceAssemblyPathAsync(string sourceAssemblyPath);

        Task RemoveSourceAssemblyPathAsync(string sourceAssemblyPath);

        bool ContainsTestResult(Guid testCaseId);

        ITestResult GetTestResult(Guid testCaseId);

        bool TryGetTestResult(Guid testCaseId, out ITestResult testResult);

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
        internal TestResultManager(IFileSystem fileSystem, ITestCaseDiscoveryManager discoveryManager, ITestObjectFactory testObjectFactory)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _discoveryManager = discoveryManager ?? throw new ArgumentNullException(nameof(discoveryManager));
            _testObjectFactory = testObjectFactory ?? throw new ArgumentNullException(nameof(testObjectFactory));

            _discoveryManager.TestCasesDiscovered += OnTestCasesDiscovered;
            _discoveryManager.DiscoveryCompleted += OnDiscoveryComplete;
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCaseManager

        public IReadOnlyCollection<string> SourceAssemblyPaths { get; }
        private readonly HashSet<string> _sourceAssemblyPaths = new HashSet<string>();

        public IReadOnlyCollection<ITestResult> TestResults => _testResultsByTestCaseId.Values;

        public bool ContainsTestResult(Guid testCaseId)
        {
            return _testResultsByTestCaseId.ContainsKey(testCaseId);
        }

        public ITestResult GetTestResult(Guid testCaseId)
        {
            if (!_testResultsByTestCaseId.TryGetValue(testCaseId, out var testResult))
                throw new ArgumentException($"No {nameof(ITestResult)} exists with the given {nameof(ITestResult.TestCase)}.{nameof(ITestCase.Id)} value", nameof(testCaseId));

            return testResult;
        }

        public bool TryGetTestResult(Guid testCaseId, out ITestResult testResult)
        {
            return _testResultsByTestCaseId.TryGetValue(testCaseId, out testResult);
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

            var removedTestResults = new List<ITestResult>(_testResultsByTestCaseId.Count);
            foreach (var testResult in _testResultsByTestCaseId.Values)
                if (testResult.TestCase.Source == sourceAssemblyPath)
                    removedTestResults.Add(testResult);

            foreach (var testResult in removedTestResults)
                _testResultsByTestCaseId.Remove(testResult.TestCase.Id);

            if(removedTestResults.Count > 0)
                RaiseTestResultsRemoved(removedTestResults);
        }

        public event EventHandler<TestResultsEventArgs> TestResultsAdded;
        private void RaiseTestResultsAdded(IEnumerable<ITestResult> testResults)
        {
            TestResultsAdded?.Invoke(this, new TestResultsEventArgs(testResults));
        }

        public event EventHandler<TestResultsEventArgs> TestResultsModified;
        private void RaiseTestResultsModified(IEnumerable<ITestResult> testResults)
        {
            TestResultsModified?.Invoke(this, new TestResultsEventArgs(testResults));
        }

        public event EventHandler<TestResultsEventArgs> TestResultsRemoved;
        private void RaiseTestResultsRemoved(IEnumerable<ITestResult> testResults)
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

            var addedTestResults = new List<ITestResult>(e.DiscoveredTestCases.Count());
            var modifiedTestResults = new List<ITestResult>(_testResultsByTestCaseId.Count);

            foreach (var testCase in e.DiscoveredTestCases)
            {
                if (_testResultsByTestCaseId.ContainsKey(testCase.Id))
                {
                    var oldTestResult = _testResultsByTestCaseId[testCase.Id];
                    var newTestResult = _testObjectFactory.CloneTestResult(oldTestResult, testCase);
                    _testResultsByTestCaseId[testCase.Id] = newTestResult;
                    modifiedTestResults.Add(newTestResult);
                }
                else
                {
                    var newTestResult = _testObjectFactory.CreateDefaultTestResult(testCase);
                    _testResultsByTestCaseId.Add(testCase.Id, newTestResult);
                    addedTestResults.Add(newTestResult);
                }

                _discoverySessionTestResultTestCaseIds.Add(testCase.Id);
            }

            if(addedTestResults.Count > 0)
                RaiseTestResultsAdded(addedTestResults);
            if(modifiedTestResults.Count > 0)
                RaiseTestResultsModified(modifiedTestResults);
        }

        private void OnDiscoveryComplete(object sender, DiscoveryCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!e.WasDiscoveryAborted)
            {
                var removedTestResults = new List<ITestResult>(_testResultsByTestCaseId.Count - _discoverySessionTestResultTestCaseIds.Count);
                foreach (var testResult in _testResultsByTestCaseId.Where(x => e.SourceAssemblyPaths.Contains(x.Value.TestCase.Source))
                                                       .Where(x => !_discoverySessionTestResultTestCaseIds.Contains(x.Value.TestCase.Id))
                                                       .Select(x => x.Value))
                    removedTestResults.Add(testResult);

                foreach (var testResult in removedTestResults)
                    _testResultsByTestCaseId.Remove(testResult.TestCase.Id);

                if(removedTestResults.Count > 0)
                    RaiseTestResultsRemoved(removedTestResults);

                _discoverySessionTestResultTestCaseIds.Clear();
            }
        }

        #endregion Event Handlers

        /**********************************************************************/
        #region Private Fields

        private readonly IFileSystem _fileSystem;

        private readonly ITestCaseDiscoveryManager _discoveryManager;

        private readonly ITestObjectFactory _testObjectFactory;

        private readonly Dictionary<Guid, ITestResult> _testResultsByTestCaseId = new Dictionary<Guid, ITestResult>();

        private readonly HashSet<Guid> _discoverySessionTestResultTestCaseIds = new HashSet<Guid>();

        #endregion Private Fields
    }
}
