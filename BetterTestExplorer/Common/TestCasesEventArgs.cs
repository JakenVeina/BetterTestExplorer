using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorer.Common
{
    public class TestCasesEventArgs
    {
        /**********************************************************************/
        #region Constructors

        public TestCasesEventArgs()
        {
            _testCasesById = new Dictionary<Guid, ITestCase>();

            TestCasesById = new ReadOnlyDictionary<Guid, ITestCase>(_testCasesById);
        }

        public TestCasesEventArgs(IEnumerable<ITestCase> testCases)
        {
            if (testCases == null)
                throw new ArgumentNullException(nameof(testCases));

            var testCasesCount = testCases.Count();
            _testCasesById = new Dictionary<Guid, ITestCase>(testCasesCount);
            foreach (var testCase in testCases)
                _testCasesById.Add(testCase.Id, testCase);

            TestCasesById = new ReadOnlyDictionary<Guid, ITestCase>(_testCasesById);
        }

        #endregion Constructors

        /**********************************************************************/
        #region Properties

        public IReadOnlyDictionary<Guid, ITestCase> TestCasesById { get; }
        private readonly Dictionary<Guid, ITestCase> _testCasesById;

        #endregion Properties
    }
}
