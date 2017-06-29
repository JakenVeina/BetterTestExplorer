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
            var newTestCase = Substitute.For<ITestCase>();
            newTestCase.Id.Returns(Guid.NewGuid());

            return newTestCase;
        }

        public virtual ITestResult TranslateTestResult(TestResult testResult)
        {
            var newTestResult = Substitute.For<ITestResult>();
            var testCase = TranslateTestCase(testResult.TestCase);
            newTestResult.TestCase.Returns(testCase);

            return newTestResult;
        }

        public virtual ITestResult CreateDefaultTestResult(TestCase testCase)
        {
            var testResult = Substitute.For<ITestResult>();
            var newTestCase = TranslateTestCase(testCase);
            testResult.TestCase.Returns(newTestCase);

            return testResult;
        }

        public virtual ITestResult CloneTestResult(ITestResult testResult, TestCase testCase)
        {
            var newTestResult = Substitute.For<ITestResult>();
            var newTestCase = TranslateTestCase(testCase);
            newTestResult.TestCase.Returns(newTestCase);

            newTestResult.DisplayName.Returns(testResult.DisplayName);
            newTestResult.ComputerName.Returns(testResult.ComputerName);
            newTestResult.Outcome.Returns(testResult.Outcome);
            newTestResult.StartTime.Returns(testResult.StartTime);
            newTestResult.EndTime.Returns(testResult.EndTime);
            newTestResult.Duration.Returns(testResult.Duration);
            newTestResult.Messages.Returns(testResult.Messages);
            newTestResult.ErrorMessage.Returns(testResult.ErrorMessage);
            newTestResult.ErrorStackTrace.Returns(testResult.ErrorStackTrace);
            newTestResult.Attachments.Returns(testResult.Attachments);
            newTestResult.Traits.Returns(testResult.Traits);

            return newTestResult;
        }
    }
}
