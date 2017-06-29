using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BetterTestExplorer.TestPlatform;

namespace BetterTestExplorer.TestPlatform
{
    public class TestCasesEventArgs
    {
        /**********************************************************************/
        #region Constructors

        public TestCasesEventArgs()
        {
            _TestCasesById = new Dictionary<Guid, ITestCase>();
            _TestCasesById_readOnly = new ReadOnlyDictionary<Guid, ITestCase>(_TestCasesById);
        }

        public TestCasesEventArgs(IEnumerable<ITestCase> testCases)
        {
            if (testCases == null)
                throw new ArgumentNullException(nameof(testCases));

            var testCasesCount = testCases.Count();
            _TestCasesById = new Dictionary<Guid, ITestCase>(testCasesCount);
            foreach (var testCase in testCases)
                _TestCasesById.Add(testCase.Id, testCase);
            _TestCasesById_readOnly = new ReadOnlyDictionary<Guid, ITestCase>(_TestCasesById);
        }

        #endregion Constructors

        /**********************************************************************/
        #region Properties

        public IReadOnlyDictionary<Guid, ITestCase> TestCasesById => _TestCasesById_readOnly;
        private readonly ReadOnlyDictionary<Guid, ITestCase> _TestCasesById_readOnly;
        private readonly Dictionary<Guid, ITestCase> _TestCasesById;

        #endregion Properties
    }
}
