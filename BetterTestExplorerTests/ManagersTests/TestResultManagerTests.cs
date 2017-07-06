using NUnit.Framework;
using NSubstitute;
using System.IO.Abstractions.TestingHelpers;

using System;
using System.IO.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

using BetterTestExplorer.Common;
using BetterTestExplorer.TestPlatform;
using BetterTestExplorer.Managers;

using BetterTestExplorerTests.TestPlatformTests;

namespace BetterTestExplorerTests.ManagersTests
{
    [TestFixture]
    public class TestResultManagerTests
    {
        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_FileSystemIsNull_ThrowsException()
        {
            var fileSystem = (IFileSystem)null;
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var result = Assert.Throws<ArgumentNullException>(() => new TestResultManager(fileSystem, discoveryManager, testObjectFactory));

            Assert.AreEqual("fileSystem", result.ParamName);
        }

        [Test]
        public void Constructor_DiscoveryManagerIsNull_ThrowsException()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = (ITestCaseDiscoveryManager)null;
            var testObjectFactory = FakeTestObjectFactory.Default;

            var result = Assert.Throws<ArgumentNullException>(() => new TestResultManager(fileSystem, discoveryManager, testObjectFactory));

            Assert.AreEqual("discoveryManager", result.ParamName);
        }

        [Test]
        public void Constructor_TestObjectFactoryIsNull_ThrowsException()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = (ITestObjectFactory)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestResultManager(fileSystem, discoveryManager, testObjectFactory));

            Assert.AreEqual("testObjectFactory", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_SetsSourceAssemblyPathsToEmpty()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            CollectionAssert.IsEmpty(uut.SourceAssemblyPaths);
        }

        [Test]
        public void Constructor_Otherwise_SetsTestResultsToEmpty()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            CollectionAssert.IsEmpty(uut.TestResults);
        }

        [Test]
        public void Constructor_Otherwise_SubscribesToDiscoveryManagerTestCasesDiscovered()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            discoveryManager.Received(1).TestCasesDiscovered += Arg.Any<EventHandler<DiscoveredTestsEventArgs>>();
        }

        [Test]
        public void Constructor_Otherwise_SubscribesToDiscoveryManagerDiscoveryComplete()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            discoveryManager.Received(1).DiscoveryCompleted += Arg.Any<EventHandler<DiscoveryCompletedEventArgs>>();
        }

        #endregion Constructor Tests

        /**********************************************************************/
        #region AddSourceAssemblyPathAsync Tests

        [Test]
        public void AddSourceAssemblyPathAsync_SourceAssemblyPathIsNull_ThrowsException()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var result = Assert.ThrowsAsync<ArgumentNullException>(async () => await uut.AddSourceAssemblyPathAsync(null));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
        }

        [TestCase("DummyAssembly")]
        public async Task AddSourceAssemblyPathAsync_SourceAssemblyPathIsNotNull_InvokesFileSystemPathGetFullPathWithSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var path = Substitute.For<PathBase>();
            path.GetFullPath(Arg.Any<string>()).Returns(sourceAssemblyPath);
            fileSystem.Path.Returns(path);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            path.Received(1).GetFullPath(sourceAssemblyPath);
        }

        [TestCase("DummyAssembly")]
        public void AddSourceAssemblyPathAsync_FileSystemPathGetFullPathThrowsArgumentException_WrapsException(string sourceAssemblyPath)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var path = Substitute.For<PathBase>();
            var exception = new ArgumentException();
            path.GetFullPath(Arg.Any<string>()).Returns(x => throw exception);
            fileSystem.Path.Returns(path);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
            Assert.AreSame(exception, result.InnerException);
        }

        [TestCase("DummyAssembly")]
        public async Task AddSourceAssemblyPathAsync_SourceAssemblyPathsContainsFileSystemPathGetFullPath_ThrowsException(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
            StringAssert.Contains(nameof(uut.SourceAssemblyPaths), result.Message);
        }

        [TestCase("DummyAssembly")]
        public async Task AddSourceAssemblyPathAsync_DiscoveryManagerIsDiscoveryInProgressIsTrue_InvokesDiscoveryManagerWaitForDiscoveryCompleteUntilDiscoveryManagerIsDiscoveryInProgressIsFalse(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            discoveryManager.IsDiscoveryInProgress.Returns(true, true, true, false, true);
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(Task.CompletedTask);
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            await discoveryManager.Received(3).WaitForDiscoveryCompleteAsync();
        }

        [TestCase("DummyAssembly")]
        public void AddSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteAsyncHasNotCompleted_SourceAssemblyPathsDoesNotContainSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var task = uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            CollectionAssert.DoesNotContain(uut.SourceAssemblyPaths, fileSystem.Path.GetFullPath(sourceAssemblyPath));

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public void AddSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteAsyncHasNotCompleted_DoesNotInvokeDiscoveryManagerDiscoverTestsAsync(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var task = uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            discoveryManager.DidNotReceive().DiscoverTestCasesAsync(Arg.Any<IEnumerable<string>>());

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public void AddSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteAsyncHasNotCompleted_DoesNotComplete(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var task = uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            Assert.IsFalse(task.IsCompleted);

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public async Task AddSourceAssemblyPathAsync_Otherwise_SourceAssemblyPathsContainsSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var expected = fileSystem.Path.GetFullPath(sourceAssemblyPath);

            CollectionAssert.Contains(uut.SourceAssemblyPaths, expected);
        }

        [TestCase("DummyAssembly")]
        public async Task AddSourceAssemblyPathAsync_Otherwise_InvokesDiscoveryManagerDiscoverTestsAsyncWithSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            IEnumerable<string> receivedSources = null;
            discoveryManager.When(x => x.DiscoverTestCasesAsync(Arg.Any<IEnumerable<string>>())).Do(x => receivedSources = (IEnumerable<string>)x[0]);
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var expected = fileSystem.Path.GetFullPath(sourceAssemblyPath);

            await discoveryManager.Received(1).DiscoverTestCasesAsync(Arg.Any<IEnumerable<string>>());
            CollectionAssert.AreEquivalent(receivedSources, Enumerable.Repeat(expected, 1));
        }

        #endregion AddSourceAssemblyPathAsync Tests

        /**********************************************************************/
        #region RemoveSourceAssemblyPathAsync Tests

        [Test]
        public void RemoveSourceAssemblyPathAsync_SourceAssemblyPathIsNull_ThrowsException()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var result = Assert.ThrowsAsync<ArgumentNullException>(async () => await uut.RemoveSourceAssemblyPathAsync(null));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
        }

        [TestCase("DummyAssembly")]
        public async Task RemoveSourceAssemblyPathAsync_SourceAssemblyPathIsNotNull_InvokesFileSystemPathGetFullPathWithSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var path = Substitute.For<PathBase>();
            path.GetFullPath(Arg.Any<string>()).Returns(sourceAssemblyPath);
            fileSystem.Path.Returns(path);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            path.ClearReceivedCalls();

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            path.Received(1).GetFullPath(sourceAssemblyPath);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_FileSystemPathGetFullPathThrowsArgumentException_WrapsException(string sourceAssemblyPath)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var path = Substitute.For<PathBase>();
            var exception = new ArgumentException();
            path.GetFullPath(Arg.Any<string>()).Returns(x => throw exception);
            fileSystem.Path.Returns(path);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
            Assert.AreSame(exception, result.InnerException);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_SourceAssemblyPathsDoesNotContainFileSystemPathGetFullPath_ThrowsException(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
            StringAssert.Contains(nameof(uut.SourceAssemblyPaths), result.Message);
        }

        [TestCase("DummyAssembly")]
        public async Task RemoveSourceAssemblyPathAsync_DiscoveryManagerIsDiscoveryInProgressIsTrue_InvokesDiscoveryManagerWaitForDiscoveryCompleteUntilDiscoveryManagerIsDiscoveryInProgressIsFalse(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            discoveryManager.ClearReceivedCalls();
            discoveryManager.IsDiscoveryInProgress.Returns(true, true, true, false, true);
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(Task.CompletedTask);

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            await discoveryManager.Received(3).WaitForDiscoveryCompleteAsync();
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteHasNotCompleted_SourceAssemblyPathsContainsSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            uut.AddSourceAssemblyPathAsync(sourceAssemblyPath).Wait();

            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);

            var task = uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            CollectionAssert.Contains(uut.SourceAssemblyPaths, fileSystem.Path.GetFullPath(sourceAssemblyPath));

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteHasNotCompletedAndTestResultsContainsItemsWhereTestCaseSourceEqualsSourceAssemblyPath_TestResultsIsEquivalentToInitial(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            uut.AddSourceAssemblyPathAsync(sourceAssemblyPath).Wait();

            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), fileSystem.Path.GetFullPath(sourceAssemblyPath));
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));
            var expected = uut.TestResults.ToArray();

            var task = uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            var result = uut.TestResults;

            CollectionAssert.AreEquivalent(expected, result);

            // Cleanup
            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteHasNotCompletedAndTestResultsContainsItemsWhereTestCaseSourceEqualsSourceAssemblyPath_DoesNotRaiseTestResultsRemoved(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            uut.AddSourceAssemblyPathAsync(sourceAssemblyPath).Wait();

            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), fileSystem.Path.GetFullPath(sourceAssemblyPath));
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            var handler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            uut.TestResultsRemoved += handler;

            var task = uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestResultsEventArgs>());

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteHasNotCompleted_DoesNotComplete(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            uut.AddSourceAssemblyPathAsync(sourceAssemblyPath).Wait();

            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), fileSystem.Path.GetFullPath(sourceAssemblyPath));
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            var task = uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            Assert.IsFalse(task.IsCompleted);

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public async Task RemoveSourceAssemblyPathAsync_Otherwise_SourceAssemblyPathsDoesNotContainSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);
            var expected = fileSystem.Path.GetFullPath(sourceAssemblyPath);

            CollectionAssert.DoesNotContain(uut.SourceAssemblyPaths, expected);
        }

        [TestCase("DummyAssembly", 3)]
        public async Task RemoveSourceAssemblyPathAsync_TestResultsEachTestCaseSourceEqualsFileSystemPathGetFullPath_TestResultsIsEmpty(string sourceAssemblyPath, int testResultCount)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var testCases = Enumerable.Repeat(1, testResultCount).Select(x => new TestCase() { Id = Guid.NewGuid(), Source = fileSystem.Path.GetFullPath(sourceAssemblyPath) }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases));

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            var result = uut.TestResults;

            CollectionAssert.IsEmpty(result);
        }

        [TestCase("DummyAssembly", 3)]
        public async Task RemoveSourceAssemblyPathAsync_TestResultsEachTestCaseSourceEqualsFileSystemPathGetFullPath_RaisesTestResultsRemovedWithTestResults(string sourceAssemblyPath, int testResultCount)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var testCases = Enumerable.Repeat(1, testResultCount).Select(x => new TestCase() { Id = Guid.NewGuid(), Source = fileSystem.Path.GetFullPath(sourceAssemblyPath) }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases));
            var expected = uut.TestResults.ToArray();

            var handler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            TestResultsEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestResultsEventArgs>())).Do(x => receivedArgs = (TestResultsEventArgs)x[1]);
            uut.TestResultsRemoved += handler;

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            handler.Received(1).Invoke(uut, Arg.Any<TestResultsEventArgs>());
            CollectionAssert.AreEquivalent(expected, receivedArgs.TestResultsByTestCaseId.Values);
        }

        #endregion RemoveSourceAssemblyPathAsync Tests

        /**********************************************************************/
        #region DiscoveryManager.TestCasesDiscovered Tests

        // Impossible to achieve test condition with NSubstitute
        //[Test]
        public void DiscoveryManagerTestCasesDiscovered_EIsNull_ThrowsException() { }

        [TestCase(3)]
        public void DiscoveryManagerTestCasesDiscovered_TestResultsIsEmpty_InvokesTestObjectFactoryCreateTestResultWithEachEDiscoveredTestCase(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var testCases = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases));

            foreach (var testCase in testCases)
                testObjectFactory.Received(1).CreateTestResult(testCase);
        }

        [TestCase(3)]
        public void DiscoveryManagerTestCasesDiscovered_TestResultsIsEmpty_TestResultsContainsEachTestObjectFactoryCreateTestResult(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var createdTestResults = new List<ITestResult>();
            testObjectFactory.CreateTestResult(Arg.Any<TestCase>()).Returns(x =>
            {
                var testResult = FakeTestObjectFactory.Default.CreateTestResult((TestCase)x[0]);
                createdTestResults.Add(testResult);
                return testResult;
            });

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var testCases = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases));

            var result = uut.TestResults;

            CollectionAssert.AreEquivalent(createdTestResults, result);
        }

        [TestCase(3)]
        public void DiscoveryManagerTestCasesDiscovered_TestResultsIsEmpty_RaisesTestCasesAddedWithEachTestObjectFactoryCreateTestResult(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var createdTestResults = new List<ITestResult>();
            testObjectFactory.CreateTestResult(Arg.Any<TestCase>()).Returns(x =>
            {
                var testResult = FakeTestObjectFactory.Default.CreateTestResult((TestCase)x[0]);
                createdTestResults.Add(testResult);
                return testResult;
            });

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var handler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            TestResultsEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestResultsEventArgs>())).Do(x => receivedArgs = (TestResultsEventArgs)x[1]);
            uut.TestResultsAdded += handler;
            var testCases = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases));

            handler.Received(1).Invoke(uut, Arg.Any<TestResultsEventArgs>());
            CollectionAssert.AreEquivalent(createdTestResults, receivedArgs.TestResultsByTestCaseId.Values);
        }

        [TestCase(3)]
        public void DiscoveryManagerTestCasesDiscovered_EDiscoveredTestCasesIdsContainsEachTestResultsTestCaseId_InvokesTestObjectFactoryCloneTestResultWithEachEDiscoveredTestCase(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var testCases1 = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases1));

            var testCases2 = testCases1.Select(x => new TestCase() { Id = x.Id }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases2));

            foreach (var testCase2 in testCases2)
                testObjectFactory.Received(1).CloneTestResult(Arg.Any<ITestResult>(), testCase2);
        }

        [TestCase(3)]
        public void DiscoveryManagerTestCasesDiscovered_EDiscoveredTestCasesIdsContainsEachTestResultsTestCaseId_InvokesTestObjectFactoryCloneTestResultWithEachTestResult(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var testCases1 = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases1));
            var testResults = uut.TestResults.ToArray();

            var testCases2 = testCases1.Select(x => new TestCase() { Id = x.Id }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases2));

            foreach (var testResult in testResults)
                testObjectFactory.Received(1).CloneTestResult(testResult, Arg.Any<TestCase>());
        }

        [TestCase(3)]
        public void DiscoveryManagerTestCasesDiscovered_EDiscoveredTestCasesIdsContainsEachTestResultsTestCaseId_TestResultsIsEquivalentToEachTestObjectFactoryCloneTestResult(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var clonedTestResults = new List<ITestResult>();
            testObjectFactory.CloneTestResult(Arg.Any<ITestResult>(), Arg.Any<TestCase>()).Returns(x =>
            {
                var testResult = FakeTestObjectFactory.Default.CloneTestResult((ITestResult)x[0], (TestCase)x[1]);
                clonedTestResults.Add(testResult);
                return testResult;
            });

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var testCases1 = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases1));

            var testCases2 = testCases1.Select(x => new TestCase() { Id = x.Id }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases2));

            var result = uut.TestResults;

            CollectionAssert.AreEquivalent(clonedTestResults, result);
        }

        [TestCase(3)]
        public void DiscoveryManagerTestCasesDiscovered_EDiscoveredTestCasesIdsContainsEachTestResultsTestCaseId_RaisesTestResultsModifiedWithEachTestObjectFactoryCloneTestResult(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var clonedTestResults = new List<ITestResult>();
            testObjectFactory.CloneTestResult(Arg.Any<ITestResult>(), Arg.Any<TestCase>()).Returns(x =>
            {
                var testResult = FakeTestObjectFactory.Default.CloneTestResult((ITestResult)x[0], (TestCase)x[1]);
                clonedTestResults.Add(testResult);
                return testResult;
            });

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);

            var testCases1 = Enumerable.Range(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases1));

            var handler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            TestResultsEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestResultsEventArgs>())).Do(x => receivedArgs = (TestResultsEventArgs)x[1]);
            uut.TestResultsModified += handler;

            var testCases2 = testCases1.Select(x => new TestCase() { Id = x.Id }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases2));

            handler.Received(1).Invoke(uut, Arg.Any<TestResultsEventArgs>());
            CollectionAssert.AreEquivalent(clonedTestResults, receivedArgs.TestResultsByTestCaseId.Values);
        }

        #endregion DiscoveryManager.TestCasesDiscovered Tests

        /**********************************************************************/
        #region DiscoveryManager.DiscoveryComplete Tests

        // Impossible to achieve test condition with NSubstitute
        //[Test]
        public void DiscoveryManagerDiscoveryComplete_EIsNull_ThrowsException() { }

        [TestCase(3)]
        public void DiscoveryManagerDiscoveryComplete_TestResultsIsNotEmptyAndDiscoveryCompleteHasNotBeenReceived_TestResultsIsEquivalentToInitial(int testCaseCount)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            var testCases = Enumerable.Repeat(1, testCaseCount).Select(x => new TestCase() { Id = Guid.NewGuid() }).ToArray();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(testCases));
            var expected = uut.TestResults.ToArray();

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), false));

            var result = uut.TestResults;

            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        public void DiscoveryManagerDiscoveryComplete_TestResultsIsNotEmptyAndDiscoveryCompleteHasBeenReceivedAndEIsAbortedIsTrue_TestResultsIsEquivalentToInitial()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            var testCase = new TestCase() { Id = Guid.NewGuid() };
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));
            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), false));
            var expected = uut.TestResults.ToArray();

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), true));

            var result = uut.TestResults;

            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        public void DiscoveryManagerDiscoveryComplete_TestResultsIsNotEmptyAndDiscoveryCompleteHasBeenReceivedAndEIsAbortedIsTrue_DoesNotRaiseTestCasesRemoved()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            var testCase = new TestCase() { Id = Guid.NewGuid() };
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));
            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), false));

            var handler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            uut.TestResultsRemoved += handler;

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), true));

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestResultsEventArgs>());
        }

        [TestCase("DummyAssembly")]
        public void DiscoveryManagerDiscoveryComplete_TestResultsIsNotEmptyAndDiscoveryCompleteHasBeenReceivedAndEIsAbortedIsFalse_TestResultsIsEmpty(string sourceAssemblyPath)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            var testCase = new TestCase() { Id = Guid.NewGuid(), Source = sourceAssemblyPath };
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));
            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), false));

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Repeat(sourceAssemblyPath, 1), false));

            var result = uut.TestResults;

            CollectionAssert.IsEmpty(result);
        }

        [TestCase("DummyAssembly")]
        public void DiscoveryManagerDiscoveryComplete_TestResultsIsNotEmptyAndDiscoveryCompleteHasBeenReceivedAndEIsAbortedIsFalse_RaisesTestResultsRemovedWithTestResults(string sourceAssemblyPath)
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            var testObjectFactory = FakeTestObjectFactory.Default;

            var uut = new TestResultManager(fileSystem, discoveryManager, testObjectFactory);
            var testCase = new TestCase() { Id = Guid.NewGuid(), Source = sourceAssemblyPath };
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));
            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), false));
            var expected = uut.TestResults.ToArray();

            var handler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            TestResultsEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestResultsEventArgs>())).Do(x => receivedArgs = (TestResultsEventArgs)x[1]);
            uut.TestResultsRemoved += handler;

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Repeat(sourceAssemblyPath, 1), false));

            handler.Received(1).Invoke(uut, Arg.Any<TestResultsEventArgs>());
            CollectionAssert.AreEquivalent(expected, receivedArgs.TestResultsByTestCaseId.Values);
        }

        #endregion DiscoveryManager.DiscoveryComplete Tests
    }
}
