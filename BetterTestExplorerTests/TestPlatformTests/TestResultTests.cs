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
    [TestFixture]
    public class TestResultTests
    {
        /**********************************************************************/
        #region Test Procedures

        private static VsTestPlatform.TestCase MakeVsTestCase()
        {
            var vsTestCase = new VsTestPlatform.TestCase();

            vsTestCase.Id = Guid.NewGuid();
            vsTestCase.DisplayName = "DisplayName";
            vsTestCase.FullyQualifiedName = "Namespace.Class.DisplayName";
            vsTestCase.Source = "Source";
            vsTestCase.CodeFilePath = "CodeFilePath";
            vsTestCase.LineNumber = 1;
            vsTestCase.ExecutorUri = new Uri("uri://executor");

            return vsTestCase;
        }

        private static VsTestPlatform.TestResult MakeVsTestResult()
        {
            var vsTestResult = new VsTestPlatform.TestResult(MakeVsTestCase());

            vsTestResult.Outcome = VsTestPlatform.TestOutcome.Passed;
            vsTestResult.ErrorMessage = "ErrorMessage";
            vsTestResult.ErrorStackTrace = "ErrorStackTrace";
            vsTestResult.DisplayName = "DisplayName";
            vsTestResult.ComputerName = "ComputerName";
            vsTestResult.Duration = TimeSpan.FromMilliseconds(1);
            vsTestResult.StartTime = DateTimeOffset.Now;
            vsTestResult.EndTime = DateTimeOffset.Now + vsTestResult.Duration;

            return vsTestResult;
        }

        #endregion Test Procedures

        /**********************************************************************/
        #region Constructor Tests

        [Test]
        public void Constructor_TestObjectFactoryIsNull_ThrowsException()
        {
            var testObjectFactory = (ITestObjectFactory)null;
            var vsTestResult = MakeVsTestResult();

            var result = Assert.Throws<ArgumentNullException>(() => new TestResult(testObjectFactory, vsTestResult));

            Assert.AreEqual("testObjectFactory", result.ParamName);
        }

        [Test]
        public void Constructor_VsTestResultIsNull_ThrowsException()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = (VsTestPlatform.TestResult)null;

            var result = Assert.Throws<ArgumentNullException>(() => new TestResult(testObjectFactory, vsTestResult));

            Assert.AreEqual("vsTestResult", result.ParamName);
        }

        [Test]
        public void Constructor_Otherwise_InvokesTestObjectFactoryTranslateTestCaseWithVsTestResultTestCase()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            testObjectFactory.Received(1).TranslateTestCase(vsTestResult.TestCase);
        }

        [Test]
        public void Constructor_Otherwise_SetsTestCaseToTestObjectFactoryTranslateTestCaseWith()
        {
            var testObjectFactory = Substitute.ForPartsOf<FakeTestObjectFactory>();
            var vsTestResult = MakeVsTestResult();
            var testCase = FakeTestObjectFactory.Default.TranslateTestCase(vsTestResult.TestCase);
            testObjectFactory.TranslateTestCase(Arg.Any<VsTestPlatform.TestCase>()).Returns(testCase);

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.TestCase;

            Assert.AreSame(testCase, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsOutcomeToVsTestResultOutcome()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.Outcome;

            Assert.AreEqual(vsTestResult.Outcome, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsErrorMessageToVsTestResultErrorMessage()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.ErrorMessage;

            Assert.AreEqual(vsTestResult.ErrorMessage, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsErrorStackTraceToVsTestResultErrorStackTrace()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.ErrorStackTrace;

            Assert.AreEqual(vsTestResult.ErrorStackTrace, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsDisplayNameToVsTestResultDisplayName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.DisplayName;

            Assert.AreEqual(vsTestResult.DisplayName, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsComputerNameToVsTestResultComputerName()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.ComputerName;

            Assert.AreEqual(vsTestResult.ComputerName, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsDurationToVsTestResultDuration()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.Duration;

            Assert.AreEqual(vsTestResult.Duration, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsStartTimeToVsTestResultStartTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.StartTime;

            Assert.AreEqual(vsTestResult.StartTime, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsEndTimeToVsTestResultEndTime()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.EndTime;

            Assert.AreEqual(vsTestResult.EndTime, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsAttachmentsToVsTestResultAttachments()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.Attachments;

            Assert.AreEqual(vsTestResult.Attachments, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsMessagesToVsTestResultMessages()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.Messages;

            Assert.AreEqual(vsTestResult.Messages, result);
        }

        [Test]
        public void Constructor_Otherwise_SetsTraitsToVsTestResultTraits()
        {
            var testObjectFactory = FakeTestObjectFactory.Default;
            var vsTestResult = MakeVsTestResult();

            var uut = new TestResult(testObjectFactory, vsTestResult);

            var result = uut.Traits;

            Assert.AreEqual(vsTestResult.Traits, result);
        }

        #endregion Constructor Tests
    }
}
