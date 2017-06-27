using NUnit.Framework;
using NSubstitute;

using System;
using System.IO.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

using BetterTestExplorer.Common;
using BetterTestExplorer.Managers;

namespace BetterTestExplorerTests
{
    [TestFixture]
    public class SandboxTests
    {
        //[Test]
        public async Task Sandbox()
        {
            var fileSystem = new FileSystem();

            var vstest = new VsTestConsoleWrapper(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe");
            vstest.InitializeExtensions(Enumerable.Repeat(@"C:\Users\Jake\Documents\Visual Studio 2017\Projects\nunit3-vs-adapter\src\NUnitTestAdapter\bin\Debug\net35\NUnit3.TestAdapter.dll", 1));

            var testCaseDiscoveryManager = new TestCaseDiscoveryManager(vstest);
            var testCaseManager = new TestResultManager(fileSystem, testCaseDiscoveryManager);

            var testCasesDiscoveredHandler = Substitute.For<EventHandler<DiscoveredTestsEventArgs>>();
            testCaseDiscoveryManager.TestCasesDiscovered += testCasesDiscoveredHandler;
            var discoveryCompleteHandler = Substitute.For<EventHandler<DiscoveryCompletedEventArgs>>();
            testCaseDiscoveryManager.DiscoveryCompleted += discoveryCompleteHandler;
            var messageReceivedHandler = Substitute.For<EventHandler<TestRunMessageEventArgs>>();
            testCaseDiscoveryManager.MessageReceived += messageReceivedHandler;

            var testCasesAddedHandler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            testCaseManager.TestResultsAdded += testCasesAddedHandler;
            var testCasesModifiedHandler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            testCaseManager.TestResultsModified += testCasesModifiedHandler;
            var testCasesRemovedHandler = Substitute.For<EventHandler<TestResultsEventArgs>>();
            testCaseManager.TestResultsRemoved += testCasesRemovedHandler;

            await testCaseManager.AddSourceAssemblyPathAsync(@"C:\Users\Jake\Documents\Visual Studio 2017\Projects\FaultDictionaryDebugger\CommonUtilitiesTests\bin\Debug\CommonUtilitiesTests.dll");

            CollectionAssert.IsNotEmpty(testCaseManager.TestResults);
        }
    }
}
