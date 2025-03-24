using DFLicenseControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK
{
    public class GeneralFunc
    {

        public static void SetLicense()
        {
            try
            {
                string s = LicenseValidator.GenerateProductDescription();

                uint module;
                LicenseState state = LicenseValidator.SetLicense("License.xml", out module);
                if (state != LicenseState.Valid)
                    throw new Exception("License State:" + state);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] SetLicense: {0}", ex.ToString()), "GeneralFunc");
            }
        }

        public static bool DownloadMedia(string path, string file, string url)
        {
            bool success = false;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fullPath = System.IO.Path.Combine(path, file);

                if (!File.Exists(fullPath))
                {
                    WebClient Client = new WebClient();
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("After Download URL = {0}", url), "GeneralFunc");
                    Client.DownloadFile(url, fullPath);
                }
                success = true;
            }
            catch (Exception ex) 
            {
                Trace.WriteLineIf(true, string.Format("[Error] DownloadMedia[{0}] = {1}", url, ex.ToString()), "GeneralFunc");
            }
            return success;
        }

        static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("Download Progress = {0}", e.ProgressPercentage), "GeneralFunc");
        }
    }
}
