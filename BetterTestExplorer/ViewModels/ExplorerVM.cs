using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.PlatformUI;

using BetterTestExplorer.Common;
using BetterTestExplorer.Managers;

namespace BetterTestExplorer.ViewModels
{
    public interface IExplorerVM : ITestContainerVM, INotifyPropertyChanged
    {
        /**********************************************************************/
        #region Commands

        ICommand RunAllCommand { get; }

        ICommand RefreshCommand { get; }

        #endregion Commands
    }

    public class ExplorerVM : IExplorerVM
    {
        /**********************************************************************/
        #region Constructors

        public ExplorerVM()
        {
            _testPoints = new ObservableCollection<ITestPointVM>();
            _readOnlyTestPoints = new ReadOnlyObservableCollection<ITestPointVM>(_testPoints);

            RunAllCommand = new DelegateCommand(x => System.Windows.MessageBox.Show("TestVM.RunAllCommand.Execute()"));
            RefreshCommand = new DelegateCommand(x => DiscoverTests());

            _sources = new List<string>()
                {
                    @"C:\Users\Jake\Documents\Visual Studio 2017\Projects\FaultDictionaryDebugger\CommonUtilitiesTests\bin\Debug\CommonUtilitiesTests.dll"
                };

            _adapters = new List<string>()
                {
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\Microsoft\NodejsTools\NodejsTools\Microsoft.NodejsTools.TestAdapter.dll",
                    @"C:\Users\Jake\Documents\Visual Studio 2017\Projects\nunit3-vs-adapter\src\NUnitTestAdapter\bin\Debug\net35\NUnit3.TestAdapter.dll"
                };

            if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");
            if (File.Exists("log\vstest.log"))
                File.Delete("log\vstest.log");

            _vstest = new VsTestConsoleWrapper(_vstestPath, new ConsoleParameters() { LogFilePath = @"log\vstest.log"});
            Task.Factory.StartNew(() =>
            {
                _vstest.StartSession();
                _vstest.InitializeExtensions(_adapters);
            }).ContinueWith(task =>
            {
                _vstestInitialized = (task.Status == TaskStatus.RanToCompletion);
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

            _testDiscoveryManager = new TestDiscoveryManager(_vstest);
            _testDiscoveryManager.Sources.AddRange(_sources);
            _testDiscoveryManager.TestsAdded += OnTestsDiscovered;
            _testDiscoveryManager.TestDiscoveryComplete += OnTestDiscoveryComplete;


            BreakCommand = new DelegateCommand(x => Break());
        }

        #endregion Constructors

        public ICommand BreakCommand { get; private set; }

        private void Break()
        {

        }

        /**********************************************************************/
        #region IExplorerVM

        public ReadOnlyObservableCollection<ITestPointVM> TestPoints => _readOnlyTestPoints;
        private readonly ReadOnlyObservableCollection<ITestPointVM> _readOnlyTestPoints;
        private readonly ObservableCollection<ITestPointVM> _testPoints;

        public ICommand RunAllCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        #endregion IExplorerVM

        /**********************************************************************/
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        /**********************************************************************/
        #region Private Methods

        private void DiscoverTests()
        {
            _testDiscoveryManager.InitiateDiscovery();
        }

        private void OnTestsDiscovered(object sender, TestsAddedEventArgs args)
        {
            _currentDiscoveredTests = args.DiscoveredTests;
        }

        private void OnTestDiscoveryComplete(object sender, EventArgs args)
        {
            foreach(var testCase in _currentDiscoveredTests)
            {
                _testPoints.Add(new TestVM(_testDiscoveryManager, testCase));
            }

            _currentDiscoveredTests = null;
        }

        #endregion Private Methods

        /**********************************************************************/
        #region Private Fields

        private const string _vstestPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe";

        private readonly VsTestConsoleWrapper _vstest;

        private bool _vstestInitialized = false;

        private readonly List<string> _adapters;

        private readonly List<string> _sources;

        private readonly ITestCaseManager _testDiscoveryManager;

        private HashSet<TestCase> _currentDiscoveredTests;

        #endregion Private Fields
    }
}
