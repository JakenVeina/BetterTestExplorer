using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using BetterTestExplorer.Common;
using BetterTestExplorer.TestPlatform;
using BetterTestExplorer.Managers;

using BetterTestExplorerTests.TestPlatformTests;

namespace BetterTestExplorerTests.ManagersTests
{
    [TestFixture]
    public class TestCaseDiscoveryManagerTests
    {
        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_TestObjectFactoryIsNull_ThrowsException()
        {
            var testObjectFactory = (ITestObjectFactory)null;
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var result = Assert.Throws<ArgumentNullException>(() => new TestCaseDiscoveryManager(testObjectFactory, vstest));

            Assert.AreEqual("testObjectFactory", result.ParamName);
        }

        [Test]
        public void Constructor_VsTestIsNull_ThrowsException()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = (IVsTestConsoleWrapper)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestCaseDiscoveryManager(testObjectFactory, vstest));

            Assert.AreEqual("vstest", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_SetsIsDiscoveryInProcessToFalse()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var result = uut.IsDiscoveryInProgress;

            Assert.IsFalse(result);
        }

        #endregion Constructor Tests

        /**********************************************************************/
        #region DiscoverTestsAsync Tests

        [Test]
        public void DiscoverTestCasesAsync_SourceAssemblyPathsIsNull_ThrowsException()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var result = Assert.Throws<ArgumentNullException>(() => uut.DiscoverTestCasesAsync(null));

            Assert.AreEqual("sourceAssemblyPaths", result.ParamName);
        }

        [Test]
        public void DiscoverTestCasesAsync_IsDiscoveryInProgressIsTrue_DoesNotInvokeVsTestDiscoverTests()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            vstest.ClearReceivedCalls();

            var task = uut.DiscoverTestCasesAsync(Enumerable.Empty<string>());
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);
            task.Wait();

            vstest.DidNotReceive().DiscoverTests(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<ITestDiscoveryEventsHandler>());
        }

        [Test]
        public void DiscoverTestCasesAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var task = uut.DiscoverTestCasesAsync(Enumerable.Empty<string>());

            Assert.IsFalse(task.IsCompleted);

            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);
            task.Wait();

            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public async Task DiscoverTestCasesAsync_Otherwise_InvokesVsTestDiscoverTestsWithSourceAssemblyPaths()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);
            var sourceAssemblyPaths = Enumerable.Repeat("DummyAssembly", 1);

            await uut.DiscoverTestCasesAsync(sourceAssemblyPaths);

            vstest.Received(1).DiscoverTests(sourceAssemblyPaths, Arg.Any<string>(), Arg.Any<ITestDiscoveryEventsHandler>());
        }

        [Test]
        public async Task DiscoverTestCasesAsync_Otherwise_InvokesVsTestDiscoverTestsWithThis()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            await uut.DiscoverTestCasesAsync(Enumerable.Empty<string>());

            vstest.Received(1).DiscoverTests(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), uut);
        }

        [Test]
        public void DiscoverTestCasesAsync_VsTestDiscoverTestsHasNotReturned_DoesNotComplete()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

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
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            await uut.CancelDiscoveryAsync();

            vstest.Received(1).CancelDiscovery();
        }

        [Test]
        public void CancelDiscoveryAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var task = uut.CancelDiscoveryAsync();

            Assert.IsFalse(task.IsCompleted);

            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);
            task.Wait();

            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void CancelDiscoveryAsync_IsDiscoveryInProgressIsFalse_CompletesImmediately()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var result = uut.CancelDiscoveryAsync().IsCompleted;

            Assert.IsTrue(result);
        }

        #endregion CancelDiscoveryAsync Tests

        /**********************************************************************/
        #region WaitForDiscoveryCompleteAsync Tests

        [Test]
        public void WaitForDiscoveryCompleteAsync_IsDiscoveryInProgressIsTrue_DoesNotCompleteUntilHandleDiscoveryCompleteHasBeenInvoked()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var task = uut.WaitForDiscoveryCompleteAsync();

            Assert.IsFalse(task.IsCompleted);

            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);
            task.Wait();

            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void WaitForDiscoveryCompleteAsync_IsDiscoveryInProgressIsFalse_CompletesImmediately()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var result = uut.WaitForDiscoveryCompleteAsync().IsCompleted;

            Assert.IsTrue(result);
        }

        #endregion WaitForDiscoveryCompleteAsync Tests

        /**********************************************************************/
        #region HandleDiscoveredTests Tests

        [Test]
        public void HandleDiscoveredTests_IsDiscoveryInProgressIsFalse_DoesNotRaiseTestCasesDiscovered()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.HandleDiscoveredTests(Enumerable.Empty<VsTestPlatform.TestCase>());

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>());
        }

        [Test]
        public void HandleDiscoveredTests_DiscoveredTestCasesIsNull_DoesNotRaiseTestCasesDiscovered()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveredTests(null);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>());
        }

        [TestCase(3)]
        public void HandleDiscoveredTests_Otherwise_InvokesTestObjectFactoryTranslateTestCaseForEachDiscoveredTestCase(int testCaseCount)
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var vsTestCases = Enumerable.Repeat(1, testCaseCount).Select(x => new VsTestPlatform.TestCase() { Id = Guid.NewGuid() }).ToArray();
            uut.HandleDiscoveredTests(vsTestCases);

            foreach (var testCase in vsTestCases)
                testObjectFactory.Received(1).TranslateTestCase(testCase);
        }

        [TestCase(3)]
        public void HandleDiscoveredTests_Otherwise_RaisesTestCasesDiscoveredWithEachTestObjectFactoryTranslateTestCaseAsTestCasesByIdValues(int testCaseCount)
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var translatedTestCases = new List<ITestCase>();
            testObjectFactory.TranslateTestCase(Arg.Any<VsTestPlatform.TestCase>()).Returns(x =>
            {
                var translatedTestCase = FakeTestObjectFactory.Default.TranslateTestCase((VsTestPlatform.TestCase)x[0]);
                translatedTestCases.Add(translatedTestCase);
                return translatedTestCase;
            });
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            TestCasesEventArgs args = null;
            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>())).Do(x => args = (TestCasesEventArgs)x[1]);
            uut.TestCasesDiscovered += handler;
            var vsTestCases = Enumerable.Repeat(1, testCaseCount).Select(x => new VsTestPlatform.TestCase() { Id = Guid.NewGuid() }).ToArray();
            uut.HandleDiscoveredTests(vsTestCases);

            handler.Received(1).Invoke(uut, Arg.Any<TestCasesEventArgs>());
            CollectionAssert.AreEquivalent(translatedTestCases, args.TestCasesById.Values);
        }

        #endregion HandleDiscoveredTests Tests

        /**********************************************************************/
        #region HandleDiscoveryComplete Tests

        [Test]
        public void HandleDiscoveryComplete_IsDiscoveryInProgressIsFalse_DoesNotRaiseTestCasesDiscovered()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>());
        }

        [Test]
        public void HandleDiscoveryComplete_IsDiscoveryInProgressIsFalse_DoesNotRaiseDiscoveryCompleted()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>());
        }

        [Test]
        public void HandleDiscoveryComplete_LastChunkIsNull_DoesNotRaiseTestCasesDiscovered()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            uut.TestCasesDiscovered += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveryComplete(0, null, false);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>());
        }

        [Test]
        public void HandleDiscoveryComplete_LastChunkIsNull_RaisesDiscoveryCompletedWithTrueAsWasDiscoveryAborted()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            DiscoveryCompletedEventArgs args = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>())).Do(x => args = (DiscoveryCompletedEventArgs)x[1]);
            uut.DiscoveryCompleted += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveryComplete(0, null, false);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveryCompletedEventArgs>());
            Assert.IsTrue(args.WasDiscoveryAborted);
        }

        [TestCase(3)]
        public void HandleDiscoveryComplete_Otherwise_InvokesTestObjectFactoryTranslateTestCaseForEachTestCaseInLastChunk(int testCaseCount)
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            var lastChunk = Enumerable.Repeat(1, testCaseCount).Select(x => new VsTestPlatform.TestCase() { Id = Guid.NewGuid() }).ToArray();
            uut.HandleDiscoveryComplete(0, lastChunk, false);

            foreach (var testCase in lastChunk)
                testObjectFactory.Received(1).TranslateTestCase(testCase);
        }

        [TestCase(3)]
        public void HandleDiscoveryComplete_Otherwise_RaisesTestCasesDiscoveredWithEachTestObjectFactoryTranslateTestCaseAsTestCasesByIdValue(int testCaseCount)
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var translatedTestCases = new List<ITestCase>();
            testObjectFactory.TranslateTestCase(Arg.Any<VsTestPlatform.TestCase>()).Returns(x =>
            {
                var translatedTestCase = FakeTestObjectFactory.Default.TranslateTestCase((VsTestPlatform.TestCase)x[0]);
                translatedTestCases.Add(translatedTestCase);
                return translatedTestCase;
            });
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            TestCasesEventArgs args = null;
            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>())).Do(x => args = (TestCasesEventArgs)x[1]);
            uut.TestCasesDiscovered += handler;
            var lastChunk = Enumerable.Repeat(1, testCaseCount).Select(x => new VsTestPlatform.TestCase() { Id = Guid.NewGuid() }).ToArray();
            uut.HandleDiscoveryComplete(0, lastChunk, false);

            handler.Received(1).Invoke(uut, Arg.Any<TestCasesEventArgs>());
            CollectionAssert.AreEquivalent(translatedTestCases, args.TestCasesById.Values);
        }

        [Test]
        public void HandleDiscoveryComplete_Otherwise_RaisesDiscoveryCompleteWithDiscoverTestCasesAsyncSourceAssemblyPaths()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var sourceAssemblyPaths = Enumerable.Repeat("DummyAssembly", 1);
            uut.DiscoverTestCasesAsync(sourceAssemblyPaths).Wait();
            DiscoveryCompletedEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>())).Do(x => args = (DiscoveryCompletedEventArgs)x[1]);
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveryCompletedEventArgs>());
            CollectionAssert.AreEquivalent(sourceAssemblyPaths, args.SourceAssemblyPaths);
        }

        [Test]
        public void HandleDiscoveryComplete_TotalTestsIsNotEqualToLastChunkCount_RaisesDiscoveryCompleteWithTrueAsWasDiscoveryAborted()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            DiscoveryCompletedEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>())).Do(x => args = (DiscoveryCompletedEventArgs)x[1]);
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(1, Enumerable.Empty<VsTestPlatform.TestCase>(), false);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveryCompletedEventArgs>());
            Assert.IsTrue(args.WasDiscoveryAborted);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void HandleDiscoveryComplete_TotalTestsIsEqualToLastChunkCount_RaisesDiscoveryCompleteWithIsAbortedAsWasDiscoveryAbortedAs(bool isAborted)
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            DiscoveryCompletedEventArgs args = null;
            var handler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<DiscoveryCompletedEventArgs>())).Do(x => args = (DiscoveryCompletedEventArgs)x[1]);
            uut.DiscoveryCompleted += handler;
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), isAborted);

            handler.Received(1).Invoke(uut, Arg.Any<DiscoveryCompletedEventArgs>());
            Assert.AreEqual(isAborted, args.WasDiscoveryAborted);
        }

        [Test]
        public void HandleDiscoveryComplete_Otherwise_SetsIsDiscoveryInProgressToFalse()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleDiscoveryComplete(0, Enumerable.Empty<VsTestPlatform.TestCase>(), false);

            var result = uut.IsDiscoveryInProgress;

            Assert.IsFalse(result);
        }

        #endregion HandleDiscoveryComplete Tests

        /**********************************************************************/
        #region HandleLogMessage Tests

        [Test]
        public void HandleLogMessage_IsDiscoveryInProgressIsFalse_DoesNotRaiseMessageReceived()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.HandleLogMessage(TestMessageLevel.Warning, "DummyMessage");

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleLogMessage_MessageIsNull_DoesNotRaiseMessageReceived()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleLogMessage(TestMessageLevel.Warning, null);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleLogMessage_Otherwise_RaisesMessageReceivedWithLevel()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

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
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

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
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.HandleRawMessage("DummyMessage");

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleRawMessage_RawMessageIsNull_DoesNotRaiseMessageReceived()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

            var handler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            uut.MessageReceived += handler;
            uut.DiscoverTestCasesAsync(Enumerable.Empty<string>()).Wait();
            uut.HandleRawMessage(null);

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestRunMessageEventArgs>());
        }

        [Test]
        public void HandleRawMessage_Otherwise_RaisesMessageReceivedWithLevelAsInformational()
        {
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

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
            var testObjectFactory = Substitute.For<ITestObjectFactory>();
            var vstest = Substitute.For<IVsTestConsoleWrapper>();

            var uut = new TestCaseDiscoveryManager(testObjectFactory, vstest);

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
