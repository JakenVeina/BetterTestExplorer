using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using BetterTestExplorer.Common;
using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorer.Managers
{
    internal interface ITestCaseDiscoveryManager
    {
        /**********************************************************************/
        #region Properties

        bool IsDiscoveryInProgress { get; }

        #endregion Properties

        /**********************************************************************/
        #region Methods

        Task DiscoverTestCasesAsync(IEnumerable<string> sources);

        Task CancelDiscoveryAsync();

        Task WaitForDiscoveryCompleteAsync();

        #endregion Methods

        /**********************************************************************/
        #region Events

        event EventHandler<TestCasesEventArgs> TestCasesDiscovered;

        event EventHandler<TestRunMessageEventArgs> MessageReceived;

        event EventHandler<DiscoveryCompletedEventArgs> DiscoveryCompleted;

        #endregion Events
    }

    internal class TestCaseDiscoveryManager : ITestCaseDiscoveryManager, ITestDiscoveryEventsHandler
    {
        /**********************************************************************/
        #region Constructors

        // If you think you need to use this, and you're not a Factory or Unit Test, you're wrongc
        internal TestCaseDiscoveryManager(ITestObjectFactory testObjectFactory, IVsTestConsoleWrapper vstest)
        {
            _testObjectFactory = testObjectFactory ?? throw new ArgumentNullException(nameof(testObjectFactory));

            _vstest = vstest ?? throw new ArgumentNullException(nameof(vstest));

            _discoveryCompletionSource.SetResult(0);
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCaseDiscoveryManager

        public bool IsDiscoveryInProgress => !_discoveryCompletionSource.Task.IsCompleted;

        public Task DiscoverTestCasesAsync(IEnumerable<string> sourceAssemblyPaths)
        {
            if (!_discoveryCompletionSource.Task.IsCompleted)
                return _discoveryCompletionSource.Task;

            _discoverySessionSourceAssemblyPaths = sourceAssemblyPaths ?? throw new ArgumentNullException(nameof(sourceAssemblyPaths));

            _discoveryCompletionSource = new TaskCompletionSource<int>();
            return Task.Run(() => _vstest.DiscoverTests(sourceAssemblyPaths, null, this));
        }

        public Task CancelDiscoveryAsync()
        {
            _vstest.CancelDiscovery();
            return _discoveryCompletionSource.Task;
        }

        public Task WaitForDiscoveryCompleteAsync()
        {
            return _discoveryCompletionSource.Task;
        }

        public event EventHandler<TestCasesEventArgs> TestCasesDiscovered;
        private void RaiseTestCasesDiscovered(IEnumerable<ITestCase> discoveredTestCases)
        {
            TestCasesDiscovered?.Invoke(this, new TestCasesEventArgs(discoveredTestCases));
        }

        public event EventHandler<TestRunMessageEventArgs> MessageReceived;
        private void RaiseMessageReceived(TestMessageLevel level, string message)
        {
            MessageReceived?.Invoke(this, new TestRunMessageEventArgs(level, message));
        }

        public event EventHandler<DiscoveryCompletedEventArgs> DiscoveryCompleted;
        private void RaiseDiscoveryComplete(IEnumerable<string> sourceAssemblyPaths, bool wasDiscoveryAborted)
        {
            DiscoveryCompleted?.Invoke(this, new DiscoveryCompletedEventArgs(sourceAssemblyPaths, wasDiscoveryAborted));
        }

        #endregion ITestCaseDiscoveryManager

        /**********************************************************************/
        #region ITestDiscoveryEventsHandler

        public void HandleDiscoveredTests(IEnumerable<VsTestPlatform.TestCase> discoveredTestCases)
        {
            if (_discoveryCompletionSource.Task.IsCompleted)
            {
                // PENDING: vstest error, log this
                return;
            }

            if (discoveredTestCases == null)
            {
                // PENDING: vstest error, log this
                return;
            }

            var translatedTests = discoveredTestCases.Select(x => _testObjectFactory.TranslateTestCase(x)).ToArray();
            _discoverySessionTotalTestCasesDiscovered += translatedTests.Length;
            RaiseTestCasesDiscovered(translatedTests);
        }

        public void HandleDiscoveryComplete(long totalTests, IEnumerable<VsTestPlatform.TestCase> lastChunk, bool isAborted)
        {
            if (_discoveryCompletionSource.Task.IsCompleted)
            {
                // PENDING: vstest error, log this
                return;
            }

            if (lastChunk == null)
            { 
                // PENDING: vstest error, log this
                isAborted = true;
            }
            else
                HandleDiscoveredTests(lastChunk);

            if(totalTests != _discoverySessionTotalTestCasesDiscovered)
            {
                // PENDING: vstest error, log this
                isAborted = true;
            }

            RaiseDiscoveryComplete(_discoverySessionSourceAssemblyPaths, isAborted);
            _discoveryCompletionSource.SetResult(0);
        }

        public void HandleLogMessage(TestMessageLevel level, string message)
        {
            if (_discoveryCompletionSource.Task.IsCompleted)
            {
                // PENDING: vstest error, log this
                return;
            }

            if (message == null)
            {
                // PENDING: vstest error, log this
                return;
            }

            RaiseMessageReceived(level, message);
        }

        public void HandleRawMessage(string rawMessage)
        {
            if (_discoveryCompletionSource.Task.IsCompleted)
            {
                // PENDING: vstest error, log this
                return;
            }

            if (rawMessage == null)
            {
                // PENDING: vstest error, log this
                return;
            }

            // PENDING: not sure what raw messages are, might need to do something differen here.
            RaiseMessageReceived(TestMessageLevel.Informational, rawMessage);
        }

        #endregion ITestDiscoveryEventsHandler

        /**********************************************************************/
        #region Private Fields

        private readonly ITestObjectFactory _testObjectFactory;

        private readonly IVsTestConsoleWrapper _vstest;

        private IEnumerable<string> _discoverySessionSourceAssemblyPaths;

        private long _discoverySessionTotalTestCasesDiscovered;

        private TaskCompletionSource<int> _discoveryCompletionSource = new TaskCompletionSource<int>();

        #endregion Private Fields
    }
}
