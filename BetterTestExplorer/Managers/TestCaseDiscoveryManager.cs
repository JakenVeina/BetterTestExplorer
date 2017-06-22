using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using DiscoveryCompleteEventArgs = BetterTestExplorer.Common.DiscoveryCompleteEventArgs;

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

        event EventHandler<DiscoveredTestsEventArgs> TestCasesDiscovered;

        event EventHandler<TestRunMessageEventArgs> MessageReceived;

        event EventHandler<DiscoveryCompleteEventArgs> DiscoveryComplete;

        #endregion Events
    }

    internal class TestCaseDiscoveryManager : ITestCaseDiscoveryManager, ITestDiscoveryEventsHandler
    {
        /**********************************************************************/
        #region Constructors

        // If you think you need to use this, and you're not a Factory or Unit Test, you're wrongc
        internal TestCaseDiscoveryManager(IVsTestConsoleWrapper vstest)
        {
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

            _eventContext = SynchronizationContext.Current;
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

        public event EventHandler<DiscoveredTestsEventArgs> TestCasesDiscovered;
        private void RaiseTestCasesDiscovered(IEnumerable<TestCase> discoveredTestCases)
        {
            _eventContext.Post(x => TestCasesDiscovered?.Invoke(this, new DiscoveredTestsEventArgs(discoveredTestCases)), null);
        }

        public event EventHandler<TestRunMessageEventArgs> MessageReceived;
        private void RaiseMessageReceived(TestMessageLevel level, string message)
        {
            _eventContext.Post(x => MessageReceived?.Invoke(this, new TestRunMessageEventArgs(level, message)), null);
        }

        public event EventHandler<DiscoveryCompleteEventArgs> DiscoveryComplete;
        private void RaiseDiscoveryComplete(IEnumerable<string> sourceAssemblyPaths, bool wasDiscoveryAborted)
        {
            _eventContext.Post(x => DiscoveryComplete?.Invoke(this, new DiscoveryCompleteEventArgs(sourceAssemblyPaths, wasDiscoveryAborted)), null);
        }

        #endregion ITestCaseDiscoveryManager

        /**********************************************************************/
        #region ITestDiscoveryEventsHandler

        public void HandleDiscoveredTests(IEnumerable<TestCase> discoveredTestCases)
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

            _discoverySessionTotalTestCasesDiscovered += discoveredTestCases.Count();
            RaiseTestCasesDiscovered(discoveredTestCases);
        }

        public void HandleDiscoveryComplete(long totalTests, IEnumerable<TestCase> lastChunk, bool isAborted)
        {
            if (_discoveryCompletionSource.Task.IsCompleted)
            {
                // PENDING: vstest error, log this
                return;
            }

            if (lastChunk == null)            {
                // PENDING: vstest error, log this
                return;
            }

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

        private readonly IVsTestConsoleWrapper _vstest;

        private IEnumerable<string> _discoverySessionSourceAssemblyPaths;

        private long _discoverySessionTotalTestCasesDiscovered;

        private SynchronizationContext _eventContext = SynchronizationContext.Current;

        private TaskCompletionSource<int> _discoveryCompletionSource = new TaskCompletionSource<int>();

        #endregion Private Fields
    }
}
