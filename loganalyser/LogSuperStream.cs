using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loganalyser
{
    class logSuperStream
    {
        public string name;
        public List<string> files;

        public logSuperStream(string[] fileNames) {

            this.name = "SuperStream_" + DateTime.Now.ToString();

            this.files = new List<string>();

            foreach (string fileName in fileNames)
            {
                files.Add(fileName);             
            }
        }
    }
}
