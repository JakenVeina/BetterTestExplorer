using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BetterTestExplorer
{
    /// <summary>
    /// Command to launch the <see cref="BetterTestExplorerToolWindowPane"/> Tool Window.
    /// </summary>
    internal sealed class ShowBetterTestExplorerCommand
    {
        /**********************************************************************/
        #region Static Fields

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("896c01de-b21b-4e41-92a2-b1bfa9cf98ac");

        #endregion Static Fields

        /**********************************************************************/
        #region Constructors

        /// <summary>
        /// Global singleton instance of the command.
        /// </summary>
        public static ShowBetterTestExplorerCommand Default { get; private set; }

        private ShowBetterTestExplorerCommand(Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            var commandService = ((IServiceProvider)_package).GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuCommandHandler, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        #endregion Constructors

        /**********************************************************************/
        #region Public Methods

        /// <summary>
        /// Initializes the command for use by the application, by creating a new instance of the command,
        /// and saving it as <see cref="Default"/>.
        /// </summary>
        /// <param name="package">The package containing this command.</param>
        /// <exception cref="ArgumentNullException">Throws if package is null.</exception>
        /// <exception cref="InvalidOperationException">Throws if <see cref="Initialize"/> has already been invoked.</exception>
        public static void Initialize(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (Default != null)
                throw new InvalidOperationException($"{nameof(Initialize)} has already been invoked.");

            Default = new ShowBetterTestExplorerCommand(package);
        }

        #endregion Public Methods

        /**********************************************************************/
        #region Event Handlers

        private void MenuCommandHandler(object sender, EventArgs e)
        {
            // Find instance 0 of the window, or create it if it doesn't exist.
            var windowPane = _package.FindToolWindow(typeof(BetterTestExplorerToolWindowPane), 0, true);
            var windowFrame = windowPane.Frame as IVsWindowFrame ?? throw new ApplicationException("Visual Studio could not properly create the window.");
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        #endregion Event Handlers

        /**********************************************************************/
        #region Private Fields

        private readonly Package _package;

        #endregion Private Fields
    }
}
