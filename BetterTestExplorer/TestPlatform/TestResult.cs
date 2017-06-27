using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestResult : ITestObject
    {
        /**********************************************************************/
        #region Properties

        ITestCase TestCase { get; }

        Collection<VsTestPlatform.AttachmentSet> Attachments { get; }

        VsTestPlatform.TestOutcome Outcome { get; }

        string ErrorMessage { get; }

        string ErrorStackTrace { get; }

        string DisplayName { get; }

        Collection<VsTestPlatform.TestResultMessage> Messages { get; }

        string ComputerName { get; }

        TimeSpan Duration { get; }

        DateTimeOffset StartTime { get; }

        DateTimeOffset EndTime { get; }

        #endregion Properties
    }

    public class TestResult : TestObject, ITestResult
    {
        /**********************************************************************/
        #region Constructors

        internal TestResult(VsTestPlatform.TestResult vsTestResult) : base(vsTestResult)
        {
            TestCase = new TestCase(vsTestResult.TestCase);
            Attachments = vsTestResult.Attachments;
            Outcome = vsTestResult.Outcome;
            ErrorMessage = vsTestResult.ErrorMessage;
            ErrorStackTrace = vsTestResult.ErrorStackTrace;
            DisplayName = vsTestResult.DisplayName;
            Messages = vsTestResult.Messages;
            ComputerName = vsTestResult.ComputerName;
            Duration = vsTestResult.Duration;
            StartTime = vsTestResult.StartTime;
            EndTime = vsTestResult.EndTime;
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestResult

        public ITestCase TestCase { get; }

        public Collection<VsTestPlatform.AttachmentSet> Attachments { get; }

        public VsTestPlatform.TestOutcome Outcome { get; }

        public string ErrorMessage { get; }

        public string ErrorStackTrace { get; }

        public string DisplayName { get; }

        public Collection<VsTestPlatform.TestResultMessage> Messages { get; }

        public string ComputerName { get; }

        public TimeSpan Duration { get; }

        public DateTimeOffset StartTime { get; }

        public DateTimeOffset EndTime { get; }

        #endregion ITestResult
    }
}
