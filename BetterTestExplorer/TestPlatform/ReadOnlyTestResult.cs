using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestResult : ITestObject
    {
        /**********************************************************************/
        #region Properties

        ITestCase TestCase { get; }

        string DisplayName { get; }

        string ComputerName { get; }

        TestOutcome Outcome { get; }

        DateTimeOffset StartTime { get; }

        DateTimeOffset EndTime { get; }

        TimeSpan Duration { get; }

        IReadOnlyCollection<TestResultMessage> Messages { get; }

        string ErrorMessage { get; }

        string ErrorStackTrace { get; }

        IReadOnlyCollection<AttachmentSet> Attachments { get; }

        #endregion Properties
    }

    public sealed class ReadOnlyTestResult : ReadOnlyTestObject, ITestResult
    {
        /**********************************************************************/
        #region Constructors

        internal ReadOnlyTestResult(ITestObjectFactory testObjectFactory, TestResult testResult)
        {
            if (testObjectFactory == null)
                throw new ArgumentNullException(nameof(testObjectFactory));

            if (testResult == null)
                throw new ArgumentNullException(nameof(testResult));

            if (testResult.TestCase == null)
                throw new ArgumentException($"{nameof(testResult.TestCase)} cannot be null", nameof(testResult));

            TestCase = testObjectFactory.TranslateTestCase(testResult.TestCase);

            DisplayName = testResult.DisplayName;
            ComputerName = testResult.ComputerName;
            Outcome = testResult.Outcome;
            StartTime = testResult.StartTime;
            EndTime = testResult.EndTime;
            Duration = testResult.Duration;
            Messages = new ReadOnlyCollection<TestResultMessage>(testResult.Messages);
            ErrorMessage = testResult.ErrorMessage;
            ErrorStackTrace = testResult.ErrorStackTrace;
            Attachments = new ReadOnlyCollection<AttachmentSet>(testResult.Attachments);
            Traits = new ReadOnlyCollection<Trait>(testResult.Traits.ToArray());
        }

        internal ReadOnlyTestResult(ITestObjectFactory testObjectFactory, TestCase testCase)
        {
            if (testObjectFactory == null)
                throw new ArgumentNullException(nameof(testObjectFactory));

            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            TestCase = testObjectFactory.TranslateTestCase(testCase);

            var testResult = new TestResult(testCase);

            DisplayName = testResult.DisplayName;
            ComputerName = testResult.ComputerName;
            Outcome = testResult.Outcome;
            StartTime = testResult.StartTime;
            EndTime = testResult.EndTime;
            Duration = testResult.Duration;
            Messages = new ReadOnlyCollection<TestResultMessage>(testResult.Messages);
            ErrorMessage = testResult.ErrorMessage;
            ErrorStackTrace = testResult.ErrorStackTrace;
            Attachments = new ReadOnlyCollection<AttachmentSet>(testResult.Attachments);
            Traits = new ReadOnlyCollection<Trait>(testResult.Traits.ToArray());
        }

        internal ReadOnlyTestResult(ITestObjectFactory testObjectFactory, ITestResult testResult, TestCase testCase)
        {
            if (testObjectFactory == null)
                throw new ArgumentNullException(nameof(testObjectFactory));

            if (testResult == null)
                throw new ArgumentNullException(nameof(testResult));

            if (testResult.TestCase == null)
                throw new ArgumentException($"{nameof(testResult.TestCase)} cannot be null", nameof(testResult));

            TestCase = testObjectFactory.TranslateTestCase(testCase);

            DisplayName = testResult.DisplayName;
            ComputerName = testResult.ComputerName;
            Outcome = testResult.Outcome;
            StartTime = testResult.StartTime;
            EndTime = testResult.EndTime;
            Duration = testResult.Duration;
            Messages = testResult.Messages;
            ErrorMessage = testResult.ErrorMessage;
            ErrorStackTrace = testResult.ErrorStackTrace;
            Attachments = testResult.Attachments;
            Traits = testResult.Traits;
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestResult

        public ITestCase TestCase { get; }

        public string DisplayName { get; }

        public string ComputerName { get; }

        public TestOutcome Outcome { get; }

        public DateTimeOffset StartTime { get; }

        public DateTimeOffset EndTime { get; }

        public TimeSpan Duration { get; }

        public IReadOnlyCollection<TestResultMessage> Messages { get; }

        public string ErrorMessage { get; }

        public string ErrorStackTrace { get; }

        public IReadOnlyCollection<AttachmentSet> Attachments { get; }

        #endregion ITestResult

        /**********************************************************************/
        #region TestObjectWrapper Overrides

        public override IReadOnlyCollection<Trait> Traits { get; }

        #endregion TestObjectWrapper Overrides
    }
}
