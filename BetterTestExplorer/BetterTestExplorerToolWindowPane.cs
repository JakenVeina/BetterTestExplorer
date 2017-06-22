using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace BetterTestExplorer
{
    /// <summary>
    /// The VSIX Host for the <see cref="BetterTestExplorerControl"/> user control.
    /// </summary>
    [Guid(BetterTestExplorerToolWindowPane.GuidString)]
    public class BetterTestExplorerToolWindowPane : ToolWindowPane
    {
        /**********************************************************************/
        #region Static Fields

        /// <summary>
        /// The GUID for this Tool Window.
        /// </summary>
        public const string GuidString = "9275903e-2237-4a97-b9cc-3086ea172f8a";

        #endregion Static Fields

        /// <summary>
        /// Creates a new instance of <see cref="BetterTestExplorerToolWindowPane"/>.
        /// </summary>
        public BetterTestExplorerToolWindowPane() : base(null)
        {
            Caption = "Better Test Explorer";
            Content = new BetterTestExplorerControl();
        }
    }
}
