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
using DiscoveryCompleteEventArgs = BetterTestExplorer.Common.DiscoveryCompleteEventArgs;

using BetterTestExplorer.Common;

namespace BetterTestExplorer.Managers
{
    internal interface ITestCaseManager
    {
        /**********************************************************************/
        #region Properties

        IReadOnlyCollection<string> SourceAssemblyPaths { get; }

        IReadOnlyCollection<TestCase> TestCases { get; }

        #endregion Properties

        /**********************************************************************/
        #region Methods

        TestCase GetTestCase(Guid id);

        bool TryGetTestCase(Guid id, out TestCase testCase);

        Task AddSourceAssemblyPathAsync(string sourceAssemblyPath);

        Task RemoveSourceAssemblyPathAsync(string sourceAssemblyPath);

        #endregion Methods

        /**********************************************************************/
        #region Events

        event EventHandler<TestCasesEventArgs> TestCasesAdded;

        event EventHandler<TestCasesEventArgs> TestCasesModified;

        event EventHandler<TestCasesEventArgs> TestCasesRemoved;

        #endregion Events
    }

    internal partial class TestCaseManager : ITestCaseManager
    {
        /**********************************************************************/
        #region Constructors

        // If you think you need to use this, and you're not a Factory or Unit Test, you're wrongc
        internal TestCaseManager(IFileSystem fileSystem, ITestCaseDiscoveryManager discoveryManager)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _discoveryManager = discoveryManager ?? throw new ArgumentNullException(nameof(discoveryManager));

            _discoveryManager.TestCasesDiscovered += OnTestCasesDiscovered;
            _discoveryManager.DiscoveryComplete += OnDiscoveryComplete;
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCaseManager

        public IReadOnlyCollection<string> SourceAssemblyPaths => _sourceAssemblyPaths;
        private readonly HashSet<string> _sourceAssemblyPaths = new HashSet<string>();

        public IReadOnlyCollection<TestCase> TestCases => _testCasesById.Values;

        public TestCase GetTestCase(Guid id)
        {
            if (!_testCasesById.TryGetValue(id, out var testCase))
                throw new ArgumentException($"No {nameof(TestCase)} exists with the given {nameof(TestCase.Id)} value", nameof(id));

            return testCase;
        }

        public bool TryGetTestCase(Guid id, out TestCase testCase)
        {
            return _testCasesById.TryGetValue(id, out testCase);
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

            var removedTestCases = new List<TestCase>(_testCasesById.Count);
            foreach (var testCase in _testCasesById.Values)
                if (testCase.Source == sourceAssemblyPath)
                    removedTestCases.Add(testCase);

            foreach (var test in removedTestCases)
                _testCasesById.Remove(test.Id);

            if(removedTestCases.Count > 0)
                RaiseTestCasesRemoved(removedTestCases);
        }

        public event EventHandler<TestCasesEventArgs> TestCasesAdded;
        private void RaiseTestCasesAdded(IEnumerable<TestCase> testCases)
        {
            TestCasesAdded?.Invoke(this, new TestCasesEventArgs(testCases));
        }

        public event EventHandler<TestCasesEventArgs> TestCasesModified;
        private void RaiseTestCasesModified(IEnumerable<TestCase> testCases)
        {
            TestCasesModified?.Invoke(this, new TestCasesEventArgs(testCases));
        }

        public event EventHandler<TestCasesEventArgs> TestCasesRemoved;
        private void RaiseTestCasesRemoved(IEnumerable<TestCase> testCases)
        {
            TestCasesRemoved?.Invoke(this, new TestCasesEventArgs(testCases));
        }

        #endregion ITestCaseManager

        /**********************************************************************/
        #region Event Handlers

        private void OnTestCasesDiscovered(object sender, DiscoveredTestsEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var discoveredTestCasesCount = e.DiscoveredTestCases.Count();
            var addedTestCases = new List<TestCase>(discoveredTestCasesCount);
            var modifiedTestCases = new List<TestCase>(discoveredTestCasesCount);

            foreach (var testCase in e.DiscoveredTestCases)
            {
                if (_testCasesById.ContainsKey(testCase.Id))
                {
                    _testCasesById[testCase.Id] = testCase;
                    modifiedTestCases.Add(testCase);
                }
                else
                {
                    _testCasesById.Add(testCase.Id, testCase);
                    addedTestCases.Add(testCase);
                }

                _discoverySessionTestCaseIds.Add(testCase.Id);
            }

            if(addedTestCases.Count > 0)
                RaiseTestCasesAdded(addedTestCases);
            if(modifiedTestCases.Count > 0)
                RaiseTestCasesModified(modifiedTestCases);
        }

        private void OnDiscoveryComplete(object sender, DiscoveryCompleteEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!e.WasDiscoveryAborted)
            {
                var removedTestCases = new List<TestCase>(_testCasesById.Count - _discoverySessionTestCaseIds.Count);
                foreach (var testCase in _testCasesById.Where(x => e.SourceAssemblyPaths.Contains(x.Value.Source))
                                                       .Where(x => _discoverySessionTestCaseIds.Contains(x.Value.Id))
                                                       .Select(x => x.Value))
                    removedTestCases.Add(testCase);

                foreach (var testCase in removedTestCases)
                    _testCasesById.Remove(testCase.Id);

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

        private readonly Dictionary<Guid, TestCase> _testCasesById = new Dictionary<Guid, TestCase>();

        private readonly HashSet<Guid> _discoverySessionTestCaseIds = new HashSet<Guid>();

        #endregion Private Fields
    }
}
