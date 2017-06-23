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

using BetterTestExplorer.Common;
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
            var vstest = (IVsTestConsoleWrapper)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestCaseDiscoveryManager(vstest));

            Assert.AreEqual("vstest", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_SetsIsDiscoveryInProcessToFalse()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var result = uut.IsDiscoveryInProgress;

            Assert.IsFalse(result);
        }

        #endregion Constructor Tests

        /**********************************************************************/
        #region DiscoverTestsAsync Tests

        [Test]
        public void DiscoverTestCasesAsync_SourceAssemblyPathsIsNull_ThrowsException()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var result = Assert.Throws<ArgumentNullException>(() => uut.DiscoverTestCasesAsync(null));

            Assert.AreEqual("sourceAssemblyPaths", result.ParamName);
        }

        [Test]
        public void DiscoverTestCasesAsync_IsDiscoveryInProgressIsTrue_DoesNotInvokeVsTestDiscoverTests()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            vstest.ClearReceivedCalls();

            var task = uut.DiscoverTestCasesAsync(Enumerable.Empty<string>());
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);
            task.Wait();

            vstest.DidNotReceive().DiscoverTests(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<ITestDiscoveryEventsHandler>());
        }

        [Test]
        public void DiscoverTestCasesAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var task = uut.DiscoverTestCasesAsync(Enumerable.Empty<string>());

            Assert.IsFalse(task.IsCompleted);

            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);
            task.Wait();

            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public async Task DiscoverTestCasesAsync_Otherwise_InvokesVsTestDiscoverTestsWithSourceAssemblyPaths()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);
            var sourceAssemblyPaths = Enumerable.Repeat("DummyAssembly", 1);

            await uut.DiscoverTestCasesAsync(sourceAssemblyPaths);

            vstest.Received(1).DiscoverTests(sourceAssemblyPaths, Arg.Any<string>(), Arg.Any<ITestDiscoveryEventsHandler>());
        }

        [Test]
        public async Task DiscoverTestCasesAsync_Otherwise_InvokesVsTestDiscoverTestsWithThis()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            await uut.DiscoverTestCasesAsync(Enumerable.Empty<string>());

            vstest.Received(1).DiscoverTests(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), uut);
        }

        [Test]
        public void DiscoverTestCasesAsync_VsTestDiscoverTestsHasNotReturned_DoesNotComplete()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var tcs = new TaskCompletionSource<int>();
            vstest.When(x => x.DiscoverTests(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<ITestDiscoveryEventsHandler>()))
                  .Do(x =>
                  {
                      tcs.Task.Wait();
                  });

            var result = uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).IsCompleted;

            Assert.IsFalse(result);

            tcs.SetResult(0);
        }

        #endregion DiscoverTestsAsync Tests

        /**********************************************************************/
        #region CancelDiscoveryAsync Tests

        [Test]
        public async Task CancelDiscoveryAsync_Always_InvokesVsTestCancelDiscovery()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            await uut.CancelDiscoveryAsync();

            vstest.Received(1).CancelDiscovery();
        }

        [Test]
        public void CancelDiscoveryAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var task = uut.CancelDiscoveryAsync();

            Assert.IsFalse(task.IsCompleted);

            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);
            task.Wait();

            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void CancelDiscoveryAsync_IsDiscoveryInProgressIsFalse_CompletesImmediately()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var result = uut.CancelDiscoveryAsync().IsCompleted;

            Assert.IsTrue(result);
        }

        #endregion CancelDiscoveryAsync Tests

        /**********************************************************************/
        #region WaitForDiscoveryCompleteAsync Tests

        [Test]
        public void WaitForDiscoveryCompleteAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var task = uut.WaitForDiscoveryCompleteAsync();

            Assert.IsFalse(task.IsCompleted);

            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);
            task.Wait();

            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void WaitForDiscoveryCompleteAsync_IsDiscoveryInProgressIsFalse_CompletesImmediately()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var result = uut.WaitForDiscoveryCompleteAsync().IsCompleted;

            Assert.IsTrue(result);
        }

        #endregion WaitForDiscoveryCompleteAsync Tests

        /**********************************************************************/
        #region HandleDiscoveredTests Tests

        [Test]
        public void HandleDiscoveredTests_IsDiscoveryInProgressIsFalse_DoesNotRaiseTestCasesDiscovered()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<DiscoveredTestsEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.HandleDiscoveredTests(Enumerable.Empty<TestCase>());

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<DiscoveredTestsEventArgs>());
        }

        [Test]
        public void HandleDiscoveredTests_DiscoveredTestCasesIsNull_DoesNotRaiseTestCasesDiscovered()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<DiscoveredTestsEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveredTests(null);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<DiscoveredTestsEventArgs>());
        }

        [Test]
        public void HandleDiscoveredTests_Otherwise_RaisesTestCasesDiscoveredWithDiscoveredTestCases()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            DiscoveredTestsEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveredTestsEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveredTestsEventArgs>())).Do(x => args = (DiscoveredTestsEventArgs)x[1]);
            uut.TestCasesDiscovered += handler;
            var testCase = new TestCase();
            uut.HandleDiscoveredTests(Enumerable.Repeat(testCase, 1));

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveredTestsEventArgs>());
            CollectionAssert.Contains(args.DiscoveredTestCases, testCase);
        }

        #endregion HandleDiscoveredTests Tests

        /**********************************************************************/
        #region HandleDiscoveryComplete Tests

        [Test]
        public void HandleDiscoveryComplete_IsDiscoveryInProgressIsFalse_DoesNotRaiseTestCasesDiscovered()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<DiscoveredTestsEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<DiscoveredTestsEventArgs>());
        }

        [Test]
        public void HandleDiscoveryComplete_IsDiscoveryInProgressIsFalse_DoesNotRaiseDiscoveryCompleted()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>());
        }

        [Test]
        public void HandleDiscoveryComplete_LastChunkIsNull_DoesNotRaiseTestCasesDiscovered()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<DiscoveredTestsEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveryComplete(0, null, false);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<DiscoveredTestsEventArgs>());
        }

        [Test]
        public void HandleDiscoveryComplete_LastChunkIsNull_DoesNotRaiseDiscoveryCompleted()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            uut.DiscoveryCompleted += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveryComplete(0, null, false);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>());
        }

        [Test]
        public void HandleDiscoveryComplete_Otherwise_RaisesTestCasesDiscoveredWithLastChunk()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            DiscoveredTestsEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveredTestsEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveredTestsEventArgs>())).Do(x => args = (DiscoveredTestsEventArgs)x[1]);
            uut.TestCasesDiscovered += handler;
            var testCase = new TestCase();
            uut.HandleDiscoveryComplete(0, Enumerable.Repeat(testCase, 1), false);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveredTestsEventArgs>());
            CollectionAssert.Contains(args.DiscoveredTestCases, testCase);
        }

        [Test]
        public void HandleDiscoveryComplete_Otherwise_RaisesDiscoveryCompleteWithDiscoverTestCasesAsyncSourceAssemblyPaths()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var sourceAssemblyPaths = Enumerable.Repeat("DummyAssembly", 1);
            uut.DiscoverTestCasesAsync(sourceAssemblyPaths).Wait();
            DiscoveryCompletedEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>())).Do(x => args = (DiscoveryCompletedEventArgs)x[1]);
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveryCompletedEventArgs>());
            CollectionAssert.AreEquivalent(sourceAssemblyPaths, args.SourceAssemblyPaths);
        }

        [Test]
        public void HandleDiscoveryComplete_TotalTestsIsNotEqualToLastChunkCount_RaisesDiscoveryCompleteWithTrueAsWasDiscoveryAborted()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            DiscoveryCompletedEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>())).Do(x => args = (DiscoveryCompletedEventArgs)x[1]);
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(1, Enumerable.Empty<TestCase>(), false);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveryCompletedEventArgs>());
            Assert.IsTrue(args.WasDiscoveryAborted);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void HandleDiscoveryComplete_TotalTestsIsEqualToLastChunkCount_RaisesDiscoveryCompleteWithIsAbortedAsWasDiscoveryAbortedAs(bool isAborted)
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            DiscoveryCompletedEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>())).Do(x => args = (DiscoveryCompletedEventArgs)x[1]);
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), isAborted);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveryCompletedEventArgs>());
            Assert.AreEqual(isAborted, args.WasDiscoveryAborted);
        }

        [Test]
        public void HandleDiscoveryComplete_Otherwise_SetsIsDiscoveryInProgressToFalse()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<TestCase>(), false);

            var result = uut.IsDiscoveryInProgress;

            Assert.IsFalse(result);
        }

        #endregion HandleDiscoveryComplete Tests

        /**********************************************************************/
        #region HandleLogMessage Tests

        [Test]
        public void HandleLogMessage_IsDiscoveryInProgressIsFalse_DoesNotRaiseMessageReceived()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.HandleLogMessage(TestMessageLevel.Warning, "DummyMessage");

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleLogMessage_MessageIsNull_DoesNotRaiseMessageReceived()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleLogMessage(TestMessageLevel.Warning, null);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleLogMessage_Otherwise_RaisesMessageReceivedWithLevel()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            TestRunMessageEventArgs args = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>())).Do(x => args = (TestRunMessageEventArgs)x[1]);
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var level = TestMessageLevel.Warning;
            uut.HandleLogMessage(level, "DummyMessage");

            handler.Received(1).Invoke(uut, Arg.Any<TestRunMessageEventArgs>());
            Assert.AreEqual(level, args.Level);
        }

        [Test]
        public void HandleLogMessage_Otherwise_RaisesMessageReceivedWithMessage()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            TestRunMessageEventArgs args = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>())).Do(x => args = (TestRunMessageEventArgs)x[1]);
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var message = "DummyMessage";
            uut.HandleLogMessage(TestMessageLevel.Warning, message);

            handler.Received(1).Invoke(uut, Arg.Any<TestRunMessageEventArgs>());
            Assert.AreEqual(message, args.Message);
        }

        #endregion HandleLogMessage Tests

        /**********************************************************************/
        #region HandleRawMessage Tests

        [Test]
        public void HandleRawMessage_IsDiscoveryInProgressIsFalse_DoesNotRaiseMessageReceived()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.HandleRawMessage("DummyMessage");

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleRawMessage_RawMessageIsNull_DoesNotRaiseMessageReceived()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleRawMessage(null);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleRawMessage_Otherwise_RaisesMessageReceivedWithLevelAsInformational()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            TestRunMessageEventArgs args = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>())).Do(x => args = (TestRunMessageEventArgs)x[1]);
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleRawMessage("DummyMessage");

            handler.Received(1).Invoke(uut, Arg.Any<TestRunMessageEventArgs>());
            Assert.AreEqual(TestMessageLevel.Informational, args.Level);
        }

        [Test]
        public void HandleRawMessage_Otherwise_RaisesMessageReceivedWithRawMessageAsMessageAs()
        {
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            TestRunMessageEventArgs args = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>())).Do(x => args = (TestRunMessageEventArgs)x[1]);
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var rawMessage = "DummyMessage";
            uut.HandleRawMessage(rawMessage);

            handler.Received(1).Invoke(uut, Arg.Any<TestRunMessageEventArgs>());
            Assert.AreEqual(rawMessage, args.Message);
        }

        #endregion HandleRawMessage Tests
    }
}
