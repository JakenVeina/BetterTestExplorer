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

        ITestResult CreateDefaultTestResult(TestCase testCase);

        ITestResult CloneTestResult(ITestResult testResult, TestCase testCase);

        #endregion Methods
    }

    public class TestObjectFactory : ITestObjectFactory
    {
        /**********************************************************************/
        #region ITestPlatformFactory

        public ITestCase TranslateTestCase(TestCase testCase)
            => new ReadOnlyTestCase(testCase);

        public ITestResult TranslateTestResult(TestResult testResult)
            => new ReadOnlyTestResult(this, testResult);

        public ITestResult CreateDefaultTestResult(TestCase testCase)
            => new ReadOnlyTestResult(this, testCase);

        public ITestResult CloneTestResult(ITestResult testResult, TestCase testCase)
            => new ReadOnlyTestResult(this, testResult, testCase);

        #endregion ITestPlatformFactory
    }
}
