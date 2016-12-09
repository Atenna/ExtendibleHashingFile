using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.Services
{
    class HelperWriter
    {
        public string Path;
        public HelperWriter(string pathName)
        {
            Path = pathName;
        }

        public void Write(int offset, byte[] arr)
        {
            FileStream fs =  new FileStream(Path, FileMode.OpenOrCreate);
            fs.Position = offset;
            fs.Write(arr, 0, arr.Length);
            fs.Close();
        }
    }
}
