using System;
using System.IO.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;

using BetterTestExplorer.Managers;

namespace BetterTestExplorer.Common
{
    internal interface IMasterFactory
    {
        /**********************************************************************/
        #region Methods

        ITestCaseDiscoveryManager CreateTestCaseDiscoveryManager();

        ITestCaseManager CreateTestCaseManager();

        #endregion Methods
    }

    internal class MasterFactory : IMasterFactory
    {
        /**********************************************************************/
        #region Constructors

        public MasterFactory()
        {
            // PENDING: Discover this path, via ExtensionManager framework (?)
            var vstestPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe";

            _fileSystem = new FileSystem();
            _vstest = new VsTestConsoleWrapper(vstestPath);
        }

        #endregion Constructors

        /**********************************************************************/
        #region IMasterFactory

        public ITestCaseDiscoveryManager CreateTestCaseDiscoveryManager() => new TestCaseDiscoveryManager(_vstest);

        public ITestCaseManager CreateTestCaseManager() => new TestCaseManager(_fileSystem, new TestCaseDiscoveryManager(_vstest));

        #endregion IMasterFactory

        /**********************************************************************/
        #region Private Fields

        private readonly IFileSystem _fileSystem;

        private readonly IVsTestConsoleWrapper _vstest;

        #endregion Private Fields
    }
}
