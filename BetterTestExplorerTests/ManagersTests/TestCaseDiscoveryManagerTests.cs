using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using DiscoveryCompleteEventArgs = BetterTestExplorer.Common.DiscoveryCompleteEventArgs;

using BetterTestExplorer.Managers;

namespace BetterTestExplorerTests.ManagersTests
{
    [TestFixture]
    public class TestCaseDiscoveryManagerTests
    {
        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_VsTestIsNull_ThrowsException()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void Constructor_Otherwise_SetsIsDiscoveryInProcessToFalse()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion Constructor Tests

        /**********************************************************************/
        #region DiscoverTestsAsync Tests

        [Test]
        public void DiscoverTestCasesAsync_SourceAssemblyPathsIsNull_ThrowsException()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void DiscoverTestCasesAsync_IsDiscoveryInProgressIsTrue_DoesNotInvokeVsTestDiscoverTests()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void DiscoverTestCasesAsync_IsDiscoveryInProgressIsTrue_CompletesImmediately()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void DiscoverTestCasesAsync_Otherwise_InvokesVsTestDiscoverTestsWithSourceAssemblyPaths()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void DiscoverTestCasesAsync_Otherwise_InvokesVsTestDiscoverTestsWithThis()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void DiscoverTestCasesAsync_VsTestDiscoverTestsHasNotReturned_DoesNotComplete()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void DiscoverTestCasesAsync_Otherwise_Completes()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion DiscoverTestsAsync Tests

        /**********************************************************************/
        #region CancelDiscoveryAsync Tests

        [Test]
        public void CancelDiscoveryAsync_Always_InvokesVsTestCancelDiscovery()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void CancelDiscoveryAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void CancelDiscoveryAsync_IsDiscoveryInProgressIsFalse_CompletesImmediately()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion CancelDiscoveryAsync Tests

        /**********************************************************************/
        #region WaitForDiscoveryCompleteAsync Tests

        [Test]
        public void WaitForDiscoveryCompleteAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void WaitForDiscoveryCompleteAsync_IsDiscoveryInProgressIsFalse_CompletesImmediately()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion WaitForDiscoveryCompleteAsync Tests

        /**********************************************************************/
        #region HandleDiscoveredTests Tests

        [Test]
        public void HandleDiscoveredTests_IsDiscoveryInProgressIsFalse_DoesNotRaiseTestCasesDiscovered()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveredTests_DiscoveredTestCasesIsNull_DoesNotRaiseTestCasesDiscovered()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveredTests_Otherwise_RaisesTestCasesDiscoveredWithDiscoveredTestCases()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveredTests_Otherwise_SomethingSomethingCurrentSynchronizationContext()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion HandleDiscoveredTests Tests

        /**********************************************************************/
        #region HandleDiscoveryComplete Tests

        [Test]
        public void HandleDiscoveryComplete_IsDiscoveryInProgressIsFalse_DoesNotRaiseTestCasesDiscovered()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_IsDiscoveryInProgressIsFalse_DoesNotRaiseDiscoveryComplete()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_LastChunkIsNull_DoesNotRaiseTestCasesDiscovered()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_LastChunkIsNull_DoesNotRaiseDiscoveryComplete()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_Otherwise_RaisesTestCasesDiscoveredWithLastChunk()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_TotalTestsIsNotEqualToLastChunkCount_RaisesDiscoveryCompleteWithDiscoverTestCasesAsyncSourceAssemblyPaths()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_TotalTestsIsNotEqualToLastChunkCount_RaisesDiscoveryCompleteWithTrueAsWasDiscoveryAborted()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_TotalTestsIsEqualToLastChunkCount_RaisesDiscoveryCompleteWithIsAbortedAsWasDiscoveryAbortedAs()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleDiscoveryComplete_Otherwise_SetsIsDiscoveryInProgressToFalse()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion HandleDiscoveryComplete Tests

        /**********************************************************************/
        #region HandleLogMessage Tests

        [Test]
        public void HandleLogMessage_IsDiscoveryInProgressIsFalse_DoesNotRaiseMessageReceived()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleLogMessage_MessageIsNull_DoesNotRaiseMessageReceived()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleLogMessage_Otherwise_RaisesMessageReceivedWithLevel()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleLogMessage_Otherwise_RaisesMessageReceivedWithMessage()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion HandleLogMessage Tests

        /**********************************************************************/
        #region HandleRawMessage Tests

        [Test]
        public void HandleRawMessage_IsDiscoveryInProgressIsFalse_DoesNotRaiseMessageReceived()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleRawMessage_RawMessageIsNull_DoesNotRaiseMessageReceived()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleRawMessage_Otherwise_RaisesMessageReceivedWithLevelAsInformational()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        [Test]
        public void HandleRawMessage_Otherwise_RaisesMessageReceivedWithRawMessageAsMessageAs()
        {
            TestingUtilities.AssertExtensions.NotImplemented();
        }

        #endregion HandleRawMessage Tests
    }
}
