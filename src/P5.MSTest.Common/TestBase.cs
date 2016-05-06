using System.IO;

namespace P5.MSTest.Common
{
    public class TestBase
    {
        private string _targetFolder;

        protected string TargetFolder
        {
            get { return _targetFolder; }
        }

        protected void Setup()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
        }
    }
}