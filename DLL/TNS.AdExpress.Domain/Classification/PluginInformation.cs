using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Classification
{
    public enum PluginType
    { 
        Appm = 1,
        Bastet = 2,
        Sobek = 4,
        Satet = 5,
        Hotep = 6,
        Miysis = 7,
        Mnevis = 8,
        Shou = 9,
        Amset = 10,
        Aton = 11,
        Alert = 20,
    }

    public class PluginInformation
    {
        private string _filePath;
        private int _longevity;
        private bool _deleteExpired;
        private int _resultType;
        private string _name;
        private string _themePath;
        private string _extension;

        public string Extension
        {
            get { return (this._extension); }
        }

        public string ThemePath
        {
            get { return (this._themePath); }
        }

        public string FilePath
        {
            get { return (this._filePath); }
        }

        public int Longevity
        {
            get { return (this._longevity); }
        }

        public bool DeleteExpired
        {
            get { return (this._deleteExpired); }
        }

        public int ResultType
        {
            get { return (this._resultType); }
        }

        public string Name
        {
            get { return (this._name); }
        }


        public PluginInformation(string filePath, int longevity, bool deleteExpired, int resultType, string themePath, string name, string extension)
        {
            this._filePath = filePath;
            this._longevity = longevity;
            this._deleteExpired = deleteExpired;
            this._resultType = resultType;
            this._name = name;
            this._extension = extension;
        }

        public PluginInformation(string filePath, int longevity, int resultType, string themePath) : this(filePath, longevity, false, resultType, themePath, "Unknown", ".pdf")
        { }
    }
}
