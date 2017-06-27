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

        ReadOnlyObservableCollection<ITestPointVM> TestPoints { get; }

        #endregion Properties
    }

    internal abstract class TestContainerVM : TestPointVM, ITestContainerVM
    {
        /**********************************************************************/
        #region Constructor

        internal TestContainerVM()
        {
            TestPoints = new ReadOnlyObservableCollection<ITestPointVM>(_testPoints);
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

        public ReadOnlyObservableCollection<ITestPointVM> TestPoints { get; }
        protected readonly ObservableCollection<ITestPointVM> _testPoints = new ObservableCollection<ITestPointVM>();

        #endregion ITestContainerVM

        /**********************************************************************/
        #region ITestPointVM

        public override ICommand RunCommand
            => _runCommand ?? (_runCommand = new DelegateCommand(
                   _ => System.Windows.MessageBox.Show($"{typeof(TestContainerVM).FullName}.{nameof(RunCommand)}"),
                   _ => true));
        private ICommand _runCommand;

        #endregion ITestPointVM
    }
}
