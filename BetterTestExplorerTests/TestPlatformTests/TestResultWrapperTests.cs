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
    [TestFixture]
    public class TestResultWrapperTests
    {
        /**********************************************************************/
        #region Test Procedures

        private static TestCase MakeTestCase()
        {
            var testCase = new TestCase();

            testCase.Id = Guid.NewGuid();
            testCase.DisplayName = "DisplayName";
            testCase.FullyQualifiedName = "Namespace.Class.DisplayName";
            testCase.Source = "Source";
            testCase.CodeFilePath = "CodeFilePath";
            testCase.LineNumber = 1;
            testCase.ExecutorUri = new Uri("uri://executor");

            return testCase;
        }

        private static TestResult MakeTestResult()
        {
            var testResult = new TestResult(MakeTestCase());

            testResult.Outcome = TestOutcome.Passed;
            testResult.ErrorMessage = "ErrorMessage";
            testResult.ErrorStackTrace = "ErrorStackTrace";
            testResult.DisplayName = "DisplayName";
            testResult.ComputerName = "ComputerName";
            testResult.Duration = TimeSpan.FromMilliseconds(1);
            testResult.StartTime = DateTimeOffset.Now;
            testResult.EndTime = DateTimeOffset.Now + testResult.Duration;

            return testResult;
        }

        #endregion Test Procedures

        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_TestObjectFactoryIsNull_ThrowsException()
        {
            var testObjectFactory = (ITestObjectFactory)null;
            var testResult = MakeTestResult();

            var result = Assert.Throws<ArgumentNullException>(() => new TestResultWrapper(testObjectFactory, testResult));

            Assert.AreEqual("testObjectFactory", result.ParamName);
        }

        [Test]
        public void Constructor_TestResultIsNull_ThrowsException()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = (TestResult)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestResultWrapper(testObjectFactory, testResult));

            Assert.AreEqual("testResult", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_InvokesTestObjectFactoryTranslateTestCaseWithTestResultTestCase()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            testObjectFactory.Received(1).TranslateTestCase(testResult.TestCase);
        }

        [Test]
        public void Constructor_Otherwise_SetsTestCaseToTestObjectFactoryTranslateTestCase()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var testResult = MakeTestResult();
            var testCase = FakeTestObjectFactory.Default.TranslateTestCase(testResult.TestCase);
            testObjectFactory.TranslateTestCase(Arg.Any<TestCase>()).Returns(testCase);

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.TestCase;

            Assert.AreSame(testCase, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsDisplayNameToTestResultDisplayName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.DisplayName;

            Assert.AreEqual(testResult.DisplayName, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsComputerNameToTestResultComputerName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.ComputerName;

            Assert.AreEqual(testResult.ComputerName, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsOutcomeToTestResultOutcome()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.Outcome;

            Assert.AreEqual(testResult.Outcome, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsStartTimeToTestResultStartTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.StartTime;

            Assert.AreEqual(testResult.StartTime, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsEndTimeToTestResultEndTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.EndTime;

            Assert.AreEqual(testResult.EndTime, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsDurationToTestResultDuration()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.Duration;

            Assert.AreEqual(testResult.Duration, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsMessagesToTestResultMessages()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.Messages;

            Assert.AreEqual(testResult.Messages, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsErrorMessageToTestResultErrorMessage()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.ErrorMessage;

            Assert.AreEqual(testResult.ErrorMessage, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsErrorStackTraceToTestResultErrorStackTrace()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.ErrorStackTrace;

            Assert.AreEqual(testResult.ErrorStackTrace, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsAttachmentsToTestResultAttachments()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.Attachments;

            Assert.AreEqual(testResult.Attachments, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsTraitsToTestResultTraits()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new TestResultWrapper(testObjectFactory, testResult);

            var result = uut.Traits;

            Assert.AreSame(testResult.Traits, result);
        }

        #endregion Constructor Tests
    }
}
