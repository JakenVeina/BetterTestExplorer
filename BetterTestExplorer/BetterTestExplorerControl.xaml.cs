using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using System.Windows;
using System.Windows.Controls;

//using BetterTestExplorer.ViewModels;

namespace BetterTestExplorer
{
    /// <summary>
    /// The control for the <see cref="BetterTestExplorerToolWindowPane"/> Tool Window.
    /// </summary>
    public partial class BetterTestExplorerControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="BetterTestExplorerControl"/>.
        /// </summary>
        public BetterTestExplorerControl()
        {
            InitializeComponent();

            //DataContext = new ExplorerVM();
        }
    }
}