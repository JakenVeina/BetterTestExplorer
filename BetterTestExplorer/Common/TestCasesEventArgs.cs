using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.Common
{
    internal class TestCasesEventArgs : EventArgs
    {
        /**********************************************************************/
        #region Constructors

        public TestCasesEventArgs()
        {
            _testCasesById = new Dictionary<Guid, TestCase>();
        }

        public TestCasesEventArgs(IEnumerable<TestCase> testCases)
        {
            if (testCases == null)
                throw new ArgumentNullException(nameof(testCases));

            var testCasesCount = testCases.Count();
            _testCasesById = new Dictionary<Guid, TestCase>(testCasesCount);
            foreach(var testCase in testCases)
                _testCasesById.Add(testCase.Id, testCase);
        }

        #endregion Constructors

        /**********************************************************************/
        #region Properties

        public IReadOnlyDictionary<Guid, TestCase> TestCasesById => _testCasesById;
        private readonly Dictionary<Guid, TestCase> _testCasesById;

        #endregion Properties
    }
}
