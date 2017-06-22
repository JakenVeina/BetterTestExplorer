//------------------------------------------------------------------------------
// <copyright file="BetterTestExplorerPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace BetterTestExplorer
{
    /// <summary>
    /// Implements the VSIX <see cref="Package"/> abstract class for interfacing between Visual Studio
    /// and the extension modules present in this package.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(BetterTestExplorerToolWindowPane))]
    [Guid(BetterTestExplorerPackage.GuidString)]
    public sealed class BetterTestExplorerPackage : Package
    {
        /**********************************************************************/
        #region Static Fields

        /// <summary>
        /// The GUID for this VSIX package.
        /// </summary>
        public const string GuidString = "82025b4a-5d40-4922-91e2-9c270fc4e595";

        #endregion Static Fields
        
        /**********************************************************************/
        #region Package Overrides

        /// <summary>
        /// See <see cref="Package.Initialize"/>.
        /// </summary>
        protected override void Initialize()
        {
            ShowBetterTestExplorerCommand.Initialize(this);

            base.Initialize();
        }

        #endregion Package Overrides
    }
}
