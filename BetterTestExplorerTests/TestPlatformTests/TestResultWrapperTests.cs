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
        #region Constructor(testObjectFactory, testResult) Tests

        [Test]
        public void Constructor_TestObjectFactoryTestResult_TestObjectFactoryIsNull_ThrowsException()
        {
            var testObjectFactory = (ITestObjectFactory)null;
            var testResult = MakeTestResult();

            var result = Assert.Throws<ArgumentNullException>(() => new ReadOnlyTestResult(testObjectFactory, testResult));

            Assert.AreEqual("testObjectFactory", result.ParamName);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_TestResultIsNull_ThrowsException()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = (TestResult)null;

            var result = Assert.Throws<ArgumentNullException>(() => new ReadOnlyTestResult(testObjectFactory, testResult));

            Assert.AreEqual("testResult", result.ParamName);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_InvokesTestObjectFactoryTranslateTestCaseWithTestResultTestCase()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            testObjectFactory.Received(1).TranslateTestCase(testResult.TestCase);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsTestCaseToTestObjectFactoryTranslateTestCase()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var testResult = MakeTestResult();
            var testCase = FakeTestObjectFactory.Default.TranslateTestCase(testResult.TestCase);
            testObjectFactory.TranslateTestCase(Arg.Any<TestCase>()).Returns(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.TestCase;

            Assert.AreSame(testCase, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsDisplayNameToTestResultDisplayName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.DisplayName;

            Assert.AreEqual(testResult.DisplayName, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsComputerNameToTestResultComputerName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.ComputerName;

            Assert.AreEqual(testResult.ComputerName, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsOutcomeToTestResultOutcome()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.Outcome;

            Assert.AreEqual(testResult.Outcome, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsStartTimeToTestResultStartTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.StartTime;

            Assert.AreEqual(testResult.StartTime, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsEndTimeToTestResultEndTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.EndTime;

            Assert.AreEqual(testResult.EndTime, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsDurationToTestResultDuration()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.Duration;

            Assert.AreEqual(testResult.Duration, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsMessagesToTestResultMessages()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.Messages;

            CollectionAssert.AreEquivalent(testResult.Messages, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsErrorMessageToTestResultErrorMessage()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.ErrorMessage;

            Assert.AreEqual(testResult.ErrorMessage, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsErrorStackTraceToTestResultErrorStackTrace()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.ErrorStackTrace;

            Assert.AreEqual(testResult.ErrorStackTrace, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsAttachmentsToTestResultAttachments()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.Attachments;

            CollectionAssert.AreEquivalent(testResult.Attachments, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestResult_Otherwise_SetsTraitsToTestResultTraits()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testResult = MakeTestResult();

            var uut = new ReadOnlyTestResult(testObjectFactory, testResult);

            var result = uut.Traits;

            CollectionAssert.AreEquivalent(testResult.Traits, result);
        }

        #endregion Constructor(testObjectFactory, testResult) Tests

        /**********************************************************************/
        #region Constructor(testObjectFactory, testCase) Tests

        [Test]
        public void Constructor_TestObjectFactoryTestCase_TestObjectFactoryIsNull_ThrowsException()
        {
            var testObjectFactory = (ITestObjectFactory)null;
            var testCase = MakeTestCase();

            var result = Assert.Throws<ArgumentNullException>(() => new ReadOnlyTestResult(testObjectFactory, testCase));

            Assert.AreEqual("testObjectFactory", result.ParamName);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_TestCaseIsNull_ThrowsException()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = (TestCase)null;

            var result = Assert.Throws<ArgumentNullException>(() => new ReadOnlyTestResult(testObjectFactory, testCase));

            Assert.AreEqual("testCase", result.ParamName);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_InvokesTestObjectFactoryTranslateTestCaseWithTestCase()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var testCase = MakeTestCase();

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            testObjectFactory.Received(1).TranslateTestCase(testCase);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsTestCaseToTestObjectFactoryTranslateTestCase()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var testCase = MakeTestCase();
            var translatedTestCase = FakeTestObjectFactory.Default.TranslateTestCase(testCase);
            testObjectFactory.TranslateTestCase(Arg.Any<TestCase>()).Returns(translatedTestCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.TestCase;

            Assert.AreSame(translatedTestCase, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsDisplayNameToNewTestResultDisplayName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.DisplayName;

            Assert.AreEqual(testResult.DisplayName, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsComputerNameToNewTestResultComputerName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.ComputerName;

            Assert.AreEqual(testResult.ComputerName, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsOutcomeToNewTestResultOutcome()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.Outcome;

            Assert.AreEqual(testResult.Outcome, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsStartTimeToNewTestResultStartTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.StartTime;

            Assert.AreEqual(testResult.StartTime, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsEndTimeToNewTestResultEndTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.EndTime;

            Assert.AreEqual(testResult.EndTime, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsDurationToNewTestResultDuration()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.Duration;

            Assert.AreEqual(testResult.Duration, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsMessagesToNewTestResultMessages()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.Messages;

            CollectionAssert.AreEquivalent(testResult.Messages, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsErrorMessageToNewTestResultErrorMessage()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.ErrorMessage;

            Assert.AreEqual(testResult.ErrorMessage, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsErrorStackTraceToNewTestResultErrorStackTrace()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.ErrorStackTrace;

            Assert.AreEqual(testResult.ErrorStackTrace, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsAttachmentsToNewTestResultAttachments()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.Attachments;

            CollectionAssert.AreEquivalent(testResult.Attachments, result);
        }

        [Test]
        public void Constructor_TestObjectFactoryTestCase_Otherwise_SetsTraitsToNewTestResultTraits()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var testCase = MakeTestCase();
            var testResult = new TestResult(testCase);

            var uut = new ReadOnlyTestResult(testObjectFactory, testCase);

            var result = uut.Traits;

            CollectionAssert.AreEquivalent(testResult.Traits, result);
        }

        #endregion Constructor(testObjectFactory, testCase) Tests
    }
}
