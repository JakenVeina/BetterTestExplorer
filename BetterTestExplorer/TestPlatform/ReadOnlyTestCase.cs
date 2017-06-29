using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public sealed class ReadOnlyTestCase : ReadOnlyTestObject, ITestCase
    {
        /**********************************************************************/
        #region Constructors

        internal ReadOnlyTestCase(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            Id = testCase.Id;
            DisplayName = testCase.DisplayName;
            FullyQualifiedName = testCase.FullyQualifiedName;
            Source = testCase.Source;
            CodeFilePath = testCase.CodeFilePath;
            LineNumber = testCase.LineNumber;
            ExecutorUri = testCase.ExecutorUri;
            Traits = new ReadOnlyCollection<Trait>(testCase.Traits.ToArray());
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCase

        public Guid Id { get; }

        public string DisplayName { get; }

        public string FullyQualifiedName { get; }

        public string Source { get; }

        public string CodeFilePath { get; }

        public int LineNumber { get; }

        public Uri ExecutorUri { get; }

        #endregion ITestCase

        /**********************************************************************/
        #region ReadOnlyTestObject Overrides

        public override IReadOnlyCollection<Trait> Traits { get; }

        #endregion ReadOnlyTestObject Overrides
    }
}
