using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestOrionLag.Unit
{
    public class OrionTestHelper
    {
        private string m_fullTestDir;
        public OrionTestHelper(string testPath)
        {
            string[] sep = new string[]{ @"\" };
            var splits = testPath.Split(sep,StringSplitOptions.None);
            var start = splits[0];
            string path = GetPathOfCurrentlyExecutingAssembly();

            string[] sep2 = new string[] { start };

            var PathSplits = path.Split(sep2, StringSplitOptions.None);
            m_fullTestDir = Path.Combine(PathSplits[0], testPath);
       }

        internal string[] ReadResultFile(string filename)
        {
            var inputContent = File.ReadAllLines(Path.Combine(m_fullTestDir,filename), new UTF7Encoding());
            return inputContent;
        }

        private static string GetPathOfCurrentlyExecutingAssembly()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string currentlyExecutingAssemblyPath =
                Path.GetDirectoryName(executingAssembly.CodeBase);
            var pathUri = new Uri(currentlyExecutingAssemblyPath);

            return pathUri.LocalPath;
        }

        public string GetTestPath()
        {
            return this.m_fullTestDir;
        }
    }
}
