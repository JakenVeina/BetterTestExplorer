using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsTestPlatform = Microsoft.VisualStudio.TestPlatform;

namespace BetterTestExplorer.TestPlatform
{
    public interface ITestCase : ITestObject
    {
        Guid Id { get; }

        string DisplayName { get; }

        string ClassName { get; }

        string NamespaceName { get; }

        string SourceAssemblyPath { get; }

        string SourceFilePath { get; }

        int SourceFileLine { get; }

        Uri ExecutorUri { get; }
    }

    public class TestCase : TestObject, ITestCase
    {
        /**********************************************************************/
        #region Constructors

        internal TestCase(VsTestPlatform.ObjectModel.TestCase vsTestCase) : base(vsTestCase ?? throw new ArgumentNullException(nameof(vsTestCase)))
        {
            Id = vsTestCase.Id;
            DisplayName = vsTestCase.DisplayName;
            SourceAssemblyPath = vsTestCase.Source;
            SourceFilePath = vsTestCase.CodeFilePath;
            SourceFileLine = vsTestCase.LineNumber;
            ExecutorUri = vsTestCase.ExecutorUri;

            var methodNameIndex = vsTestCase.FullyQualifiedName.LastIndexOf(vsTestCase.DisplayName);
            var namespaceAndClass = vsTestCase.FullyQualifiedName.Substring(0, methodNameIndex - 1);
            var classSeparatorIndex = namespaceAndClass.LastIndexOf('.');

            ClassName = namespaceAndClass.Substring(classSeparatorIndex + 1);
            NamespaceName = namespaceAndClass.Substring(0, classSeparatorIndex);
        }

        #endregion Constructors

        /**********************************************************************/
        #region ITestCase

        public Guid Id { get; }

        public string DisplayName { get; }

        public string ClassName { get; }

        public string NamespaceName { get; }

        public string SourceAssemblyPath { get; }

        public string SourceFilePath { get; }

        public int SourceFileLine { get; }

        public Uri ExecutorUri { get; }

        #endregion ITestCase
    }
}
