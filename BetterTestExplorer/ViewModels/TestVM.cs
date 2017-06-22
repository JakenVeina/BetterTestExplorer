using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.PlatformUI;

using BetterTestExplorer.Managers;
using BetterTestExplorer.Common;

namespace BetterTestExplorer.ViewModels
{
    public interface ITestVM : ITestPointVM
    {
        /**********************************************************************/
        #region Properties

        string SourceFile { get; }

        int SourceFileLineNumber { get; }

        #endregion Properties

        /**********************************************************************/
        #region Commands

        ICommand ViewSourceCommand { get; }

        #endregion Commands
    }

    public class TestVM : TestPointVM, ITestVM
    {
        /**********************************************************************/
        #region Constructors

        internal TestVM(ITestCaseManager testDiscoveryManager, TestCase testCase) : base(testDiscoveryManager)
        {
            TestCase = testCase ?? throw new ArgumentNullException(nameof(testCase));

            _viewSourceCommand = new DelegateCommand(x => System.Windows.MessageBox.Show("TestContainerVM.ViewSourceCommand.Execute()"));
            _runCommand = new DelegateCommand(x => System.Windows.MessageBox.Show("TestContainerVM.RunCommand.Execute()"));
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestVM

        public string SourceAssembly
        {
            get => _sourceAssembly;
            private set
            {
                if (_sourceAssembly == value)
                    return;

                _sourceAssembly = value;
                RaisePropertyChanged();
            }
        }
        private string _sourceAssembly;

        public string SourceFile
        {
            get => _sourceFile;
            private set
            {
                if (_sourceFile == value)
                    return;

                _sourceFile = value;
                RaisePropertyChanged();
            }
        }
        private string _sourceFile;

        public int SourceFileLineNumber
        {
            get => _sourceFileLineNumber;
            private set
            {
                if (_sourceFileLineNumber == value)
                    return;

                _sourceFileLineNumber = value;
                RaisePropertyChanged();
            }
        }
        private int _sourceFileLineNumber;

        public ICommand ViewSourceCommand => _viewSourceCommand;
        private readonly ICommand _viewSourceCommand;

        #endregion ITestVM

        /**********************************************************************/
        #region ITestPointVM

        public override ICommand RunCommand => _runCommand;
        private readonly ICommand _runCommand;

        protected override void OnTestsDiscovered(object sender, TestsAddedEventArgs args)
        {
            base.OnTestsDiscovered(sender, args);

            foreach (var testCase in args.DiscoveredTests)
            {
                if (TestCase.Id == testCase.Id)
                {
                    TestCase = testCase;
                    break;
                }
            }

            args.DiscoveredTests.Remove(TestCase);
        }

        #endregion ITestPointVM

        /**********************************************************************/
        #region Private Properties

        private TestCase TestCase
        {
            get => _testCase;
            set
            {
                if (_testCase == value)
                    return;

                _testCase = value;
                Name = value.DisplayName;
                SourceAssembly = Path.GetFileName(value.Source);
                SourceFile = Path.GetFileName(value.CodeFilePath);
                SourceFileLineNumber = value.LineNumber;
            }

        }
        private TestCase _testCase;

        #endregion Private Properties
    }
}
