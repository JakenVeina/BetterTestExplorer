using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestObjectFactory
    {
        /**********************************************************************/
        #region Methods

        ITestCase TranslateTestCase(TestCase testCase);

        ITestResult TranslateTestResult(TestResult testResult);

        #endregion Methods
    }

    public class TestObjectFactory : ITestObjectFactory
    {
        /**********************************************************************/
        #region ITestPlatformFactory

        public ITestCase TranslateTestCase(TestCase testCase)
            => new TestCaseWrapper(testCase);

        public ITestResult TranslateTestResult(TestResult testResult)
            => new TestResultWrapper(this, testResult);

        #endregion ITestPlatformFactory
    }
}
