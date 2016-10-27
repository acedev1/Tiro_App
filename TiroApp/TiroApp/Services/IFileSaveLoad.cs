using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gis4Mobile
{
    public interface IFileSaveLoad
    {
        void SaveText(string filename, string text);
        string LoadText(string filename);
        Stream OpenFile(string path);
        void DeleteFile(string path);
        void DeleteDirectory(string path, EventHandler callbcak);
    }
}
