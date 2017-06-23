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
using DiscoveryCompletedEventArgs = BetterTestExplorer.Common.DiscoveryCompletedEventArgs;

using BetterTestExplorer.Common;
using BetterTestExplorer.Managers;

namespace BetterTestExplorerTests.ManagersTests
{
    [TestFixture]
    public class TestCaseManagerTests
    {
        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_FileSystemIsNull_ThrowsException()
        {
            var fileSystem = (IFileSystem)null;
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var result = Assert.Throws<ArgumentNullException>(() => new TestCaseManager(fileSystem, discoveryManager));

            Assert.AreEqual("fileSystem", result.ParamName);
        }

        [Test]
        public void Constructor_DiscoveryManagerIsNull_ThrowsException()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = (ITestCaseDiscoveryManager)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestCaseManager(fileSystem, discoveryManager));

            Assert.AreEqual("discoveryManager", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_SetsSourceAssemblyPathsToEmpty()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            CollectionAssert.IsEmpty(uut.SourceAssemblyPaths);
        }

        [Test]
        public void Constructor_Otherwise_SetsTestCasesToEmpty()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            CollectionAssert.IsEmpty(uut.TestCases);
        }

        [Test]
        public void Constructor_Otherwise_SubscribesToDiscoveryManagerTestCasesDiscovered()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            discoveryManager.Received(1).TestCasesDiscovered += Arg.Any<EventHandler<DiscoveredTestsEventArgs>>();
        }

        [Test]
        public void Constructor_Otherwise_SubscribesToDiscoveryManagerDiscoveryComplete()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
            Assert.AreSame(exception, result.InnerException);
        }

        [TestCase("DummyAssembly")]
        public async Task AddSourceAssemblyPathAsync_SourceAssemblyPathsContainsFileSystemPathGetFullPathWithSourceAssemblyPath_ThrowsException(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var expected = fileSystem.Path.GetFullPath(sourceAssemblyPath);

            CollectionAssert.Contains(uut.SourceAssemblyPaths, expected);
        }

        [TestCase("DummyAssembly")]
        public async Task AddSourceAssemblyPathAsync_Otherwise_InvokesDiscoverTestsAsyncWithSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();
            IEnumerable<string> receivedSources = null;
            discoveryManager.When(x => x.DiscoverTestCasesAsync(Arg.Any<IEnumerable<string>>())).Do(x => receivedSources = (IEnumerable<string>)x[0]);

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var expected = fileSystem.Path.GetFullPath(sourceAssemblyPath);

            await discoveryManager.Received().DiscoverTestCasesAsync(Arg.Any<IEnumerable<string>>());
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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);
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

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath));

            Assert.AreEqual("sourceAssemblyPath", result.ParamName);
            Assert.AreSame(exception, result.InnerException);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_SourceAssemblyPathsDoesNotContainFileSystemPathGetFullPathWithSourceAssemblyPath_ThrowsException(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

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

            var uut = new TestCaseManager(fileSystem, discoveryManager);
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

            var uut = new TestCaseManager(fileSystem, discoveryManager);
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
        public void RemoveSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteHasNotCompleted_TestCasesContainsTestCasesWhereSourceEqualsSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            uut.AddSourceAssemblyPathAsync(sourceAssemblyPath).Wait();

            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), fileSystem.Path.GetFullPath(sourceAssemblyPath));
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));
            
            var task = uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            CollectionAssert.Contains(uut.TestCases, testCase);

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteHasNotCompleted_DoesNotRaiseTestCasesRemoved(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            uut.AddSourceAssemblyPathAsync(sourceAssemblyPath).Wait();

            discoveryManager.IsDiscoveryInProgress.Returns(true);
            var taskCompletionSource = new TaskCompletionSource<int>();
            discoveryManager.WaitForDiscoveryCompleteAsync().Returns(taskCompletionSource.Task);
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), fileSystem.Path.GetFullPath(sourceAssemblyPath));
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            uut.TestCasesRemoved += handler;

            var task = uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            Task.Delay(100).Wait();

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>());

            discoveryManager.IsDiscoveryInProgress.Returns(false);
            taskCompletionSource.SetResult(0);
        }

        [TestCase("DummyAssembly")]
        public void RemoveSourceAssemblyPathAsync_DiscoveryManagerWaitForDiscoveryCompleteHasNotCompleted_DoesNotComplete(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
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

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);
            var expected = fileSystem.Path.GetFullPath(sourceAssemblyPath);

            CollectionAssert.DoesNotContain(uut.SourceAssemblyPaths, expected);
        }

        [TestCase("DummyAssembly")]
        public async Task RemoveSourceAssemblyPathAsync_Otherwise_TestCasesDoesNotContainTestCasesWhereSourceEqualsSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), fileSystem.Path.GetFullPath(sourceAssemblyPath));
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);
            var expected = uut.TestCases.Where(x => x.Source == fileSystem.Path.GetFullPath(sourceAssemblyPath));

            CollectionAssert.DoesNotContain(expected, testCase);            
        }

        [TestCase("DummyAssembly")]
        public async Task RemoveSourceAssemblyPathAsync_Otherwise_RaisesTestCasesRemovedWithTestCasesWhereSourceEqualsSourceAssemblyPath(string sourceAssemblyPath)
        {
            var fileSystem = new MockFileSystem(null, "DummyDirectory");
            fileSystem.AddFile(sourceAssemblyPath, sourceAssemblyPath);
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            await uut.AddSourceAssemblyPathAsync(sourceAssemblyPath);
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), fileSystem.Path.GetFullPath(sourceAssemblyPath));
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            TestCasesEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>())).Do(x => receivedArgs = (TestCasesEventArgs)x[1]);
            uut.TestCasesRemoved += handler;

            await uut.RemoveSourceAssemblyPathAsync(sourceAssemblyPath);

            handler.Received(1).Invoke(uut, Arg.Any<TestCasesEventArgs>());
            CollectionAssert.Contains(receivedArgs.TestCasesById.Values, testCase);
        }

        #endregion RemoveSourceAssemblyPathAsync Tests

        /**********************************************************************/
        #region DiscoveryManager.TestCasesDiscovered Tests

        // Impossible to test through NUnit
        public void DiscoveryManagerTestCasesDiscovered_EIsNull_ThrowsException() { }

        [Test]
        public void DiscoveryManagerTestCasesDiscovered_TestCasesIdsDoesNotContainAnyEDiscoveredTestCasesIds_TestCasesContainsEDiscoveredTestCases()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            var testCase = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            CollectionAssert.Contains(uut.TestCases, testCase);
        }

        [Test]
        public void DiscoveryManagerTestCasesDiscovered_TestCasesIdsDoesNotContainAnyEDiscoveredTestCasesIds_RaisesTestCasesAddedWithEDiscoveredTestCases()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            TestCasesEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>())).Do(x => receivedArgs = (TestCasesEventArgs)x[1]);
            uut.TestCasesAdded += handler;
            var testCase = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            handler.Received(1).Invoke(uut, Arg.Any<TestCasesEventArgs>());
            CollectionAssert.Contains(receivedArgs.TestCasesById.Values, testCase);
        }

        [Test]
        public void DiscoveryManagerTestCasesDiscovered_TestCasesIdsContainsEachEDiscoveredTestCasesIds_TestCasesContainsEDiscoveredTestCases()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            var testCase1 = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase1, 1)));

            var testCase2 = new TestCase() { Id = testCase1.Id };
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase2, 1)));

            CollectionAssert.Contains(uut.TestCases, testCase2);
        }

        [Test]
        public void DiscoveryManagerTestCasesDiscovered_TestCasesIdsContainsEachEDiscoveredTestCasesIds_TestCasesDoesNotContainTestCases()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            var testCase1 = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase1, 1)));

            var testCase2 = new TestCase() { Id = testCase1.Id };
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase2, 1)));

            CollectionAssert.DoesNotContain(uut.TestCases, testCase1);
        }

        [Test]
        public void DiscoveryManagerTestCasesDiscovered_TestCasesIdsContainsEachEDiscoveredTestCasesIds_RaisesTestCasesModifiedWithEDiscoveredTestCases()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);

            var testCase1 = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase1, 1)));

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            TestCasesEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>())).Do(x => receivedArgs = (TestCasesEventArgs)x[1]);
            uut.TestCasesModified += handler;

            var testCase2 = new TestCase() { Id = testCase1.Id };
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase2, 1)));

            handler.Received(1).Invoke(uut, Arg.Any<TestCasesEventArgs>());
            CollectionAssert.Contains(receivedArgs.TestCasesById.Values, testCase2);
        }

        #endregion DiscoveryManager.TestCasesDiscovered Tests

        /**********************************************************************/
        #region DiscoveryManager.DiscoveryComplete Tests

        // Impossible to test through NUnit
        public void DiscoveryManagerDiscoveryComplete_EIsNull_ThrowsException() { }

        [Test]
        public void DiscoveryManagerDiscoveryComplete_ReceivedTestCasesDiscoveredSinceLastDiscoveryComplete_TestCasesContainsTestsDiscoveredEDiscoveredTestCases()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            var testCase = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), false));

            CollectionAssert.Contains(uut.TestCases, testCase);
        }

        [Test]
        public void DiscoveryManagerDiscoveryComplete_TestCasesIsNotEmptyAndEIsAbortedIsTrue_TestCasesDoesNotChange()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            var testCase = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));
            var expected = uut.TestCases.ToList();

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), true));

            var result = uut.TestCases;

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void DiscoveryManagerDiscoveryComplete_TestCasesIsNotEmptyAndEIsAbortedIsTrue_DoesNotRaiseTestCasesRemoved()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            var testCase = new TestCase();
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            uut.TestCasesRemoved += handler;

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Empty<string>(), true));

            handler.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>());
        }

        [Test]
        public void DiscoveryManagerDiscoveryComplete_TestCasesIsNotEmptyAndEIsAbortedIsFalse_TestCasesIsEmpty()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            var sourceAssemblyPath = "DummySource";
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), sourceAssemblyPath);
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Repeat(sourceAssemblyPath, 1), false));

            var result = uut.TestCases;

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void DiscoveryManagerDiscoveryComplete_TestCasesIsNotEmptyAndEIsAbortedIsFalse_RaisesTestCasesRemovedWithTestCases()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var discoveryManager = Substitute.For<ITestCaseDiscoveryManager>();

            var uut = new TestCaseManager(fileSystem, discoveryManager);
            var sourceAssemblyPath = "DummySource";
            var testCase = new TestCase("DummyTestCase", new Uri("uri://dummy"), sourceAssemblyPath);
            discoveryManager.TestCasesDiscovered += Raise.EventWith(discoveryManager, new DiscoveredTestsEventArgs(Enumerable.Repeat(testCase, 1)));

            var handler = Substitute.For<EventHandler<TestCasesEventArgs>>();
            TestCasesEventArgs receivedArgs = null;
            handler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<TestCasesEventArgs>())).Do(x => receivedArgs = (TestCasesEventArgs)x[1]);
            uut.TestCasesRemoved += handler;

            discoveryManager.DiscoveryCompleted += Raise.EventWith(discoveryManager, new DiscoveryCompletedEventArgs(Enumerable.Repeat(sourceAssemblyPath, 1), false));

            handler.Received(1).Invoke(uut, Arg.Any<TestCasesEventArgs>());
            CollectionAssert.Contains(receivedArgs.TestCasesById.Values, testCase);
        }

        #endregion DiscoveryManager.DiscoveryComplete Tests
    }
}
