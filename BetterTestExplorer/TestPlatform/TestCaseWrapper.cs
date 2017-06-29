using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestCase : ITestObject
    {
        Guid Id { get; }

        string DisplayName { get; }

        string FullyQualifiedName { get; }

        string Source { get; }

        string CodeFilePath { get; }

        int LineNumber { get; }

        Uri ExecutorUri { get; }
    }

    public sealed class TestCaseWrapper : TestObjectWrapper, ITestCase
    {
        /**********************************************************************/
        #region Constructors

        internal TestCaseWrapper(TestCase testCase)
        {
            _testCase = testCase ?? throw new ArgumentNullException(nameof(testCase));
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCase

        public Guid Id => _testCase.Id;

        public string DisplayName => _testCase.DisplayName;

        public string FullyQualifiedName => _testCase.FullyQualifiedName;

        public string Source => _testCase.Source;

        public string CodeFilePath => _testCase.CodeFilePath;

        public int LineNumber => _testCase.LineNumber;

        public Uri ExecutorUri => _testCase.ExecutorUri;

        #endregion ITestCase

        /**********************************************************************/
        #region TestObjectWrapper Overrides

        protected override TestObject TestObject => _testCase;

        #endregion TestObjectWrapper Overrides

        /**********************************************************************/
        #region Private Fields

        private readonly TestCase _testCase;

        #endregion Private Fields
    }
}
