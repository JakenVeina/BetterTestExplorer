using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestObject
    {
        /**********************************************************************/
        #region Properties

        VsTestPlatform.TraitCollection Traits { get; }

        #endregion Properties
    }

    public abstract class TestObject : ITestObject
    {
        /**********************************************************************/
        #region Constructors

        protected internal TestObject(VsTestPlatform.TestObject vsTestObject)
        {
            if (vsTestObject == null)
                throw new ArgumentNullException(nameof(vsTestObject));

            Traits = vsTestObject.Traits;
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestObject

        public VsTestPlatform.TraitCollection Traits { get; }

        #endregion ITestObject
    }
}
