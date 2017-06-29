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

        TraitCollection Traits { get; }

        #endregion Properties
    }

    public abstract class TestObjectWrapper : ITestObject
    {
        /**********************************************************************/
        #region ITestObject

        public TraitCollection Traits => TestObject.Traits;

        #endregion ITestObject

        /**********************************************************************/
        #region Protected Properties

        protected abstract TestObject TestObject { get; }

        #endregion Protected Properties
    }
}
