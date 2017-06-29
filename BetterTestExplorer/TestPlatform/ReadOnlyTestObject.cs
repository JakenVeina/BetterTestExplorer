using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestObject
    {
        /**********************************************************************/
        #region Properties

        IReadOnlyCollection<Trait> Traits { get; }

        #endregion Properties
    }

    public abstract class ReadOnlyTestObject : ITestObject
    {
        /**********************************************************************/
        #region ITestObject

        public abstract IReadOnlyCollection<Trait> Traits { get; }

        #endregion ITestObject
    }
}
