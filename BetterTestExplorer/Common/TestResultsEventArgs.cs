using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorer.Common
{
    public class TestResultsEventArgs : EventArgs
    {
        /**********************************************************************/
        #region Constructors

        public TestResultsEventArgs()
        {
            _testResultsByTestCaseId = new Dictionary<Guid, ITestResult>();

            TestResultsByTestCaseId = new ReadOnlyDictionary<Guid, ITestResult>(_testResultsByTestCaseId);
        }

        public TestResultsEventArgs(IEnumerable<ITestResult> testResults)
        {
            if (testResults == null)
                throw new ArgumentNullException(nameof(testResults));

            var testResultsCount = testResults.Count();
            _testResultsByTestCaseId = new Dictionary<Guid, ITestResult>(testResultsCount);
            foreach(var testResult in testResults)
                _testResultsByTestCaseId.Add(testResult.TestCase.Id, testResult);

            TestResultsByTestCaseId = new ReadOnlyDictionary<Guid, ITestResult>(_testResultsByTestCaseId);
        }

        #endregion Constructors

        /**********************************************************************/
        #region Properties

        public IReadOnlyDictionary<Guid, ITestResult> TestResultsByTestCaseId { get; }
        private readonly Dictionary<Guid, ITestResult> _testResultsByTestCaseId;

        #endregion Properties
    }
}
