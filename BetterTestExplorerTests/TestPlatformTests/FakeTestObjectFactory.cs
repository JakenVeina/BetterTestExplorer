using NUnit.Framework;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorerTests.TestPlatformTests
{
    public class FakeTestObjectFactory : ITestObjectFactory
    {
        public readonly static ITestObjectFactory Default = new FakeTestObjectFactory();

        public virtual ITestCase TranslateTestCase(TestCase testCase)
        {
            var testCaseInterface = Substitute.For<ITestCase>();
            testCaseInterface.Id.Returns(Guid.NewGuid());

            return testCaseInterface;
        }

        public virtual ITestResult TranslateTestResult(TestResult testResult)
        {
            var testResultInterface = Substitute.For<ITestResult>();
            var testCaseInterface = TranslateTestCase(testResult.TestCase);
            testResultInterface.TestCase.Returns(testCaseInterface);

            return testResultInterface;
        }
    }
}
