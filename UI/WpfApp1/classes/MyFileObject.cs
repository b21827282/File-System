using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.classes
{
    class MyFileObject
    {

        private string _Name;
        private string _Path;
        private bool _IsFile;
        private string _ImgSrc;

        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }

        public string Path
        {
            get { return this._Path; }
            set { this._Path = value; }
        }

        public bool IsFile
        {
            get { return this._IsFile; }
            set { this._IsFile = value; }
        }

        public string ImgSrc
        {
            get { return this._ImgSrc; }
            set { this._ImgSrc = value; }
        }
    }
}
