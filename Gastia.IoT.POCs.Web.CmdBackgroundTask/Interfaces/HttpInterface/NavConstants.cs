using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask.Interfaces.HttpInterface
{
    struct NavConstants
    {
        //public static string ASSETSWEB = @"Gastia.IoT.POCs.Web.CmdBackgroundTask\Assets\Web";
        //public static string ASSETSWEB = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + @"\Assets\Web";
        //public static string ASSETSWEB = Windows.ApplicationModel.Package.Current.InstalledLocation.DisplayName + @"\Assets\Web";
        public static string ASSETSWEB = @"\Assets\Web";
        public static string DEFAULT_PAGE = "default.htm";
        public static string HOME_PAGE = "home.htm";
        public static string SETTINGS_PAGE = "settings.htm";
        public static string ADDSTATION_PAGE = "addstation.htm";
        public static string ADDSTATIONSET_PAGE = "addstationset.htm";
    }
}

