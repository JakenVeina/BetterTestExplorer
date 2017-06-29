using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.Common
{
    internal class DiscoveryCompletedEventArgs : EventArgs
    {
        /**********************************************************************/
        #region Constructor

        public DiscoveryCompletedEventArgs()
        {
            _sourceAssemblyPaths = new HashSet<string>();
        }

        public DiscoveryCompletedEventArgs(IEnumerable<string> sourceAssemblyPaths, bool wasDiscoveryAborted)
        {
            if (sourceAssemblyPaths == null)
                throw new ArgumentNullException(nameof(sourceAssemblyPaths));
            _sourceAssemblyPaths = new HashSet<string>(sourceAssemblyPaths);

            _wasDiscoveryAborted = wasDiscoveryAborted;
        }

        #endregion Constructor

        /**********************************************************************/
        #region Properties

        public IReadOnlyCollection<string> SourceAssemblyPaths => _sourceAssemblyPaths;
        private readonly HashSet<string> _sourceAssemblyPaths;

        public bool WasDiscoveryAborted => _wasDiscoveryAborted;
        private readonly bool _wasDiscoveryAborted;

        #endregion Properties
    }
}
