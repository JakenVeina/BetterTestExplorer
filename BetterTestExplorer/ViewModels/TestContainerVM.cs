using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using BetterTestExplorer.Managers;
using BetterTestExplorer.Common;

namespace BetterTestExplorer.ViewModels
{
    public interface ITestContainerVM : ITestPointVM
    {
        /**********************************************************************/
        #region Properties

        bool IsExpanded { get; set; }

        ReadOnlyObservableCollection<ITestPointVM> Children { get; }

        #endregion Properties
    }

    public class TestContainerVM : TestPointVM, ITestContainerVM
    {
        /**********************************************************************/
        #region Constructor

        internal TestContainerVM(ITestCaseManager testDiscoveryManager, string name) : base(testDiscoveryManager)
        {
            Name = name;

            _children = new ObservableCollection<ITestPointVM>();
            _readOnlyChildren = new ReadOnlyObservableCollection<ITestPointVM>(_children);

            _runCommand = new DelegateCommand(x => System.Windows.MessageBox.Show("TestContainerVM.RunCommand.Execute()"));
        }

        #endregion Constructor

        /**********************************************************************/
        #region ITestContainerVM

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;
                RaisePropertyChanged();
            }
        }
        private bool _isExpanded;

        public ReadOnlyObservableCollection<ITestPointVM> Children => _readOnlyChildren;
        private readonly ReadOnlyObservableCollection<ITestPointVM> _readOnlyChildren;
        private readonly ObservableCollection<ITestPointVM> _children;

        #endregion ITestContainerVM

        /**********************************************************************/
        #region ITestPointVM

        public override ICommand RunCommand => _runCommand;
        private readonly ICommand _runCommand;

        #endregion ITestPointVM
    }
}
