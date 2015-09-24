using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.Tools
{
    public class FfmpegLauncher
    {
        private string _arguments;




        public FfmpegLauncher(string arguments)
        {
            _arguments = arguments;

        }

        public bool Convert(string pFullSourceFilename, string pFullDestinationFileName)
        {
            bool isConverted = true;

            Process oProc = new Process();

            try
            {

                oProc.StartInfo.FileName = "ffmpeg.exe";

                oProc.StartInfo.Arguments = @"-i """ + pFullSourceFilename + @""" " + _arguments + @" """ + pFullDestinationFileName + @"""";
                //AdvancedTrace.TraceInformation(_cmdExecuted);
                oProc.StartInfo.UseShellExecute = false;
                oProc.StartInfo.CreateNoWindow = true;
                oProc.Start();
                oProc.WaitForExit();
            }
            catch (Exception ex)
            {
                //AdvancedTrace.TraceError("Convert", ex);
                isConverted = false;
                try
                {
                    if (oProc != null)
                        oProc.Kill();
                }
                catch
                {
                }
            }
            finally
            {
                if (oProc != null)
                {
                    isConverted = oProc.ExitCode == 0;
                    oProc.Dispose();
                }
                oProc = null;
            }

            return isConverted;
        }

        public static string GetDuration(string pFullSourceFilename)
        {
            string strDuration = "-1";
            Process oProc = new Process();

            try
            {

                oProc.StartInfo.FileName = "ffmpeg.exe";
                oProc.StartInfo.Arguments = @"-i """ + pFullSourceFilename + @"""";
                oProc.EnableRaisingEvents = false;
                // AdvancedTrace.TraceInformation(oProc.StartInfo.FileName + " " + oProc.StartInfo.Arguments);
                oProc.StartInfo.UseShellExecute = false;
                oProc.StartInfo.CreateNoWindow = true;
                oProc.StartInfo.RedirectStandardError = true;
                oProc.Start();

                string strLine = string.Empty;
                string strLineTemp = string.Empty;

                StreamReader oStreamReaderError = oProc.StandardError;
                do
                {
                    strLine = oStreamReaderError.ReadLine();
                    if (!string.IsNullOrEmpty(strLine) && strLine.Contains("Duration"))
                    {
                        //strDuration = strLine.Replace(" Duration: ", "").TrimStart(' ').Substring(0, 11);
                        strLineTemp = strLine.Substring(strLine.IndexOf(" Duration: ") + 11);
                        if (strLineTemp.Contains(","))
                            strDuration = strLineTemp.Substring(0, strLineTemp.IndexOf(',')).Trim();
                        else
                            strDuration = strLineTemp.Trim();
                        break;
                    }
                }
                while (!oStreamReaderError.EndOfStream);
                oProc.WaitForExit();
            }
            catch (Exception ex)
            {
                // AdvancedTrace.TraceError("GetDuration", ex);
                strDuration = "-1";
                try
                {
                    if (oProc != null)
                        oProc.Kill();
                }
                catch
                {
                }
            }
            finally
            {
                if (oProc != null)
                {
                    oProc.Dispose();
                }
                oProc = null;
            }

            return strDuration;
        }
    }
}
