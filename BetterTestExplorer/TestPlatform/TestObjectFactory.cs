using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestObjectFactory
    {
        /**********************************************************************/
        #region Methods

        ITestCase TranslateTestCase(VsTestPlatform.TestCase vsTestCase);

        ITestResult TranslateTestResult(VsTestPlatform.TestResult vsTestResult);

        #endregion Methods
    }

    public class TestObjectFactory : ITestObjectFactory
    {
        /**********************************************************************/
        #region ITestPlatformFactory

        public ITestCase TranslateTestCase(VsTestPlatform.TestCase vsTestCase)
            => new TestCase(vsTestCase);

        public ITestResult TranslateTestResult(VsTestPlatform.TestResult vsTestResult)
            => new TestResult(this, vsTestResult);

        #endregion ITestPlatformFactory
    }
}
