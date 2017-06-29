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

        Collection<TestResultMessage> Messages { get; }

        string ErrorMessage { get; }

        string ErrorStackTrace { get; }

        Collection<AttachmentSet> Attachments { get; }

        #endregion Properties
    }

    public sealed class TestResultWrapper : TestObjectWrapper, ITestResult
    {
        /**********************************************************************/
        #region Constructors

        internal TestResultWrapper(ITestObjectFactory testObjectFactory, TestResult testResult)
        {
            if (testObjectFactory == null)
                throw new ArgumentNullException(nameof(testObjectFactory));

            _testResult = testResult ?? throw new ArgumentNullException(nameof(testResult));

            if (testResult.TestCase == null)
                throw new ArgumentException($"{nameof(testResult.TestCase)} cannot be null", nameof(testResult));
            _testCase = testObjectFactory.TranslateTestCase(testResult.TestCase);
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestResult

        public ITestCase TestCase => _testCase;

        public string DisplayName => _testResult.DisplayName;

        public string ComputerName => _testResult.ComputerName;

        public TestOutcome Outcome => _testResult.Outcome;

        public DateTimeOffset StartTime => _testResult.StartTime;

        public DateTimeOffset EndTime => _testResult.EndTime;

        public TimeSpan Duration => _testResult.Duration;

        public Collection<TestResultMessage> Messages => _testResult.Messages;

        public string ErrorMessage => _testResult.ErrorMessage;

        public string ErrorStackTrace => _testResult.ErrorStackTrace;

        public Collection<AttachmentSet> Attachments => _testResult.Attachments;

        #endregion ITestResult

        /**********************************************************************/
        #region TestObjectWrapper Overrides

        protected override TestObject TestObject => _testResult;

        #endregion TestObjectWrapper Overrides

        /**********************************************************************/
        #region Private Fields

        private readonly TestResult _testResult;

        private readonly ITestCase _testCase;

        #endregion Private Fields
    }
}
