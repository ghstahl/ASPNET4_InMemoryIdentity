using System.IO;
using System.Reflection;

namespace P5.MSTest.Common
{
    public class UnitTestHelpers
    {
        private static string _baseDir;
        public static string BaseDir
        {
            get
            {
                if (_baseDir == null)
                {
                    var assembly = Assembly.GetAssembly(typeof(UnitTestHelpers));
                    var codebase = assembly.CodeBase.Replace("file:///", "");
                    _baseDir = Path.GetDirectoryName(codebase);
                }
                return _baseDir;
            }
        }
    }
}
