using CompasPack.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Support
{
    public class WinInfo
    {
        private string _productName;
        private string _displayVersion;
        private string _editionID;
        private string _currentBuild;
        private WinArchitectureEnum _currentArchitecture;
        private WinVersionEnum _winVersion;

        public WinInfo(string productName, string displayVersion, string editionID, string currentBuild, WinArchitectureEnum currentArchitecture, WinVersionEnum winVersion)
        {
            _productName = productName;
            _displayVersion = displayVersion;
            _editionID = editionID;
            _currentBuild = currentBuild;
            _currentArchitecture = currentArchitecture;
            _winVersion = winVersion;
        }
        public string ProductName { get { return _productName; } private set { _productName = value; } }
        public string DisplayVersion { get { return _displayVersion; } private set { _displayVersion = value; } }
        public string EditionID { get { return _editionID; } private set { _editionID = value; } }
        public string CurrentBuild { get { return _currentBuild; } private set { _currentBuild = value; } }
        public WinArchitectureEnum WinArchitecture { get { return _currentArchitecture; } private set { _currentArchitecture = value; } }
        public WinVersionEnum WinVer { get { return _winVersion; } private set { _winVersion = value; } }

        public override string ToString()
        {
            return $"ProductName: {_productName}\n" +
                   $"EditionID: {_editionID}\n" +
                   $"DisplayVersion: {_displayVersion}\n" +
                   $"CurrentBuild: {_currentBuild}\n" +
                   $"Type: {WinArchitecture}\n";
        }
    }
}
