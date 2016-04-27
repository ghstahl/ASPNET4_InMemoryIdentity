using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P5.IdentityServer3.BiggyJson.Test
{

    internal class UnitTestHelpers
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
