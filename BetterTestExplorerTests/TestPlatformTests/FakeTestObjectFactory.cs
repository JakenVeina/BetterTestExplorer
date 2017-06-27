using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorerTests.TestPlatformTests
{
    public class FakeTestObjectFactory : ITestObjectFactory
    {
        public readonly static ITestObjectFactory Default = new FakeTestObjectFactory();

        public virtual ITestCase TranslateTestCase(VsTestPlatform.TestCase vsTestCase)
        {
            var testCase = Substitute.For<ITestCase>();
            testCase.Id.Returns(Guid.NewGuid());

            return testCase;
        }

        public virtual ITestResult TranslateTestResult(VsTestPlatform.TestResult vsTestResult)
        {
            var testResult = Substitute.For<ITestResult>();
            var testCase = TranslateTestCase(vsTestResult.TestCase);
            testResult.TestCase.Returns(testCase);

            return testResult;
        }
    }
}
