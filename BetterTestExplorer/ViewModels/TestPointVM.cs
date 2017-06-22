﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using BetterTestExplorer.Common;
using BetterTestExplorer.Managers;

namespace BetterTestExplorer.ViewModels
{
    public interface ITestPointVM : INotifyPropertyChanged
    {
        /**********************************************************************/
        #region Properties

        string Name { get; }

        TestOutcome LastRunOutcome { get; }

        bool IsSelected { get; set; }

        #endregion Properties

        /**********************************************************************/
        #region Commands

        ICommand RunCommand { get; }

        #endregion Commands
    }

    public abstract class TestPointVM : ITestPointVM, IDisposable
    {
        /**********************************************************************/
        #region Constructors

        internal TestPointVM(ITestCaseManager testDiscoveryManager)
        {
            _testDisoveryManager = testDiscoveryManager ?? throw new ArgumentNullException(nameof(testDiscoveryManager));   
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestPointVM

        public virtual string Name
        {
            get => _name;
            protected set
            {
                if (_name == value)
                    return;

                _name = value;
                RaisePropertyChanged();
            }
        }
        private string _name;

        public TestOutcome LastRunOutcome
        {
            get => _lastRunOutcome;
            protected set
            {
                if (_lastRunOutcome == value)
                    return;

                _lastRunOutcome = value;
                RaisePropertyChanged();
            }
        }
        private TestOutcome _lastRunOutcome;

        public virtual bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                RaisePropertyChanged();
            }
        }
        private bool _isSelected;

        public abstract ICommand RunCommand { get; }

        #endregion ITestVM

        /**********************************************************************/
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        /**********************************************************************/
        #region IDisposable

        public void Dispose() => Dispose(true);

        private bool _hasDisposed = false;
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_hasDisposed)
                return;

            if (disposeManagedResources)
            {
                _testDisoveryManager.TestsAdded -= OnTestsDiscovered;
                _testDisoveryManager.TestDiscoveryComplete -= OnTestDiscoveryComplete;
            }

            _hasDisposed = true;
        }

        #endregion IDisposable

        /**********************************************************************/
        #region Event Handlers

        protected virtual void OnTestsDiscovered(object sender, TestsAddedEventArgs args) { }

        protected virtual void OnTestDiscoveryComplete(object sender, EventArgs args) { }

        #endregion Event Handlers

        /**********************************************************************/
        #region Private Fields

        private ITestCaseManager _testDisoveryManager;

        #endregion Private Fields
    }
}
