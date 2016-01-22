using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace TNS.Ares.Domain
{
    /// <summary>
    /// This static class is used to retrieves the configuration parameters from the configuration file of the web application (web.config).
    /// The list of the parameters are sorting by alphabetic order.
    /// </summary>
    public static class ConfigParamHelper
    {
        #region Private Static Variable Members

        #region PictureTrack
        /// <summary>
        /// The root input xml file paths.
        /// </summary>
        private static string[] _inputDirectories = null;

        /// <summary>
        /// The root output directory.
        /// </summary>
        private static string _outputDirectory = string.Empty;

        /// <summary>
        /// The root output directories.
        /// </summary>
        private static string[] _outputDirectories = null;

        /// <summary>
        /// The default excluded urls.
        /// </summary>
        private static IList<Regex> _excludedUrls = new List<Regex>();

        /// <summary>
        /// The default max crawl depth.
        /// </summary>
        private static int _maxCrawlDepth = 2;

        /// <summary>
        /// The default max crawl duration.
        /// </summary>
        private static int _maxCrawlDuration = 60;

        /// <summary>
        /// The default max crawl frequency.
        /// </summary>
        private static int _maxCrawlFrequency = 2;

        /// <summary>
        /// The default max page refresh.
        /// </summary>
        private static int _maxPageRefresh = 3;

        /// <summary>
        /// The default max timeout navigating.
        /// </summary>
        private static int _maxTimeoutNavigate = 30000;

        /// <summary>
        /// The default max timeout fro taking snapshot.
        /// </summary>
        private static int _maxTimeoutSnapshot = 100000;

        /// <summary>
        /// The default max try resnap page.
        /// </summary>
        private static int _maxTrySnap = 3;

        /// <summary>
        /// The default max try by crawl step.
        /// </summary>
        private static int _maxTryStep = 3;

        /// <summary>
        /// The default max try getting image.
        /// </summary>
        private static int _maxTryGettingImage = 3;
 
        /// <summary>
        /// The default max image size.
        /// </summary>
        private static int _minImgSize = 100;

        /// <summary>
        /// The default wait time resource.
        /// </summary>
        private static int _waitTimeResource = 5000;

        /// <summary>
        /// Flag that indicates if css are aspired from web site.
        /// </summary>
        private static bool _aspireCss = false;

        /// <summary>
        /// The default screenshot max pool size.
        /// </summary>
        private static int _maxSnapPoolSize = 10;

        /// <summary>
        /// The default max snap by instance.
        /// </summary>
        private static int _maxSnapByInstance = 50;

        #endregion

        #region Picture Sender

        /// <summary>
        /// Flag that indicates if ftp is used to transfer, if false user OutputDirectory.
        /// </summary>
        private static bool _useFtp = false;

        /// <summary>
        /// The maximum days history.
        /// </summary>
        private static int _daysHistory = 15;

        /// <summary>
        /// Flag that indicates if file are deleted on local.
        /// </summary>
        private static bool _isDeleteLocal = true;

        /// <summary>
        /// Flag that indicates if file are deleted on remote.
        /// </summary>
        private static bool _isDeleteRemote = true;

        #endregion

        #region PictureHeartBeat

        /// <summary>
        /// The executable path.
        /// </summary>
        private static string _pathExe = string.Empty;

        /// <summary>
        /// The process name.
        /// </summary>
        private static string _processName = string.Empty;

        /// <summary>
        /// The maximum instance of executable launch.
        /// </summary>
        private static int _maxInstance = 3;

        /// <summary>
        /// The timer tick interval between heartbeat check.
        /// </summary>
        private static int _TimerTickInterval = 3;        

        #endregion

        #endregion

        #region Public Static Properties

        #region PictureTrack

        /// <summary>
        /// The root output directory.
        /// </summary>
        public static string OutputDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("OutputDirectory")))
                {
                    try
                    {
                        _outputDirectory = GetConfigParameter("OutputDirectory");
                    }
                    catch
                    {
                        _outputDirectory = string.Empty;
                    }
                }

                return _outputDirectory;
            }
        }

        /// <summary>
        /// The default xml file path.
        /// </summary>
        public static string[] InputDirectories
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("InputDirectories")))
                {
                    try
                    {
                        _inputDirectories = GetConfigParameter("InputDirectories").Split(new char[] {';',','}, StringSplitOptions.RemoveEmptyEntries);
                    }
                    catch
                    {
                        _inputDirectories = null;
                    }
                }

                return _inputDirectories;
            }
        }

        /// <summary>
        /// The root output directories.
        /// </summary>
        public static string[] OutputDirectories
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("OutputDirectories")))
                {
                    try
                    {
                        _outputDirectories = GetConfigParameter("OutputDirectories").Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    catch
                    {
                        _outputDirectories = null;
                    }
                }

                return _outputDirectories;
            }
        }

        /// <summary>
        /// The default max crawl duration.
        /// </summary>
        public static int MaxCrawlDepth
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxCrawlDepth")))
                {
                    try
                    {
                        _maxCrawlDepth = int.Parse(GetConfigParameter("MaxCrawlDepth"));
                    }
                    catch
                    {
                        _maxCrawlDepth = 2;
                    }
                }

                return _maxCrawlDepth;
            }
        }

        /// <summary>
        /// The default max crawl depth.
        /// </summary>
        public static int MaxCrawlDuration
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxCrawlDuration")))
                {
                    try
                    {
                        _maxCrawlDuration = int.Parse(GetConfigParameter("MaxCrawlDuration"));
                    }
                    catch
                    {
                        _maxCrawlDuration = 60;
                    }
                }

                return _maxCrawlDuration;
            }
        }

        /// <summary>
        /// The default max crawl frequency.
        /// </summary>
        public static int MaxCrawlFrequency
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxCrawlFrequency")))
                {
                    try
                    {
                        _maxCrawlFrequency = int.Parse(GetConfigParameter("MaxCrawlFrequency"));
                    }
                    catch
                    {
                        _maxCrawlFrequency = 2;
                    }
                }

                return _maxCrawlFrequency;
            }
        }

        /// <summary>
        /// The default max page refresh.
        /// </summary>
        public static int MaxPageRefresh
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxPageRefresh")))
                {
                    try
                    {
                        _maxPageRefresh = int.Parse(GetConfigParameter("MaxPageRefresh"));
                    }
                    catch
                    {
                        _maxPageRefresh = 3;
                    }
                }

                return _maxPageRefresh;
            }
        }

        /// <summary>
        /// The default max timeout navigating.
        /// </summary>
        public static int MaxTimeoutNavigate
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxTimeoutNavigate")))
                {
                    try
                    {
                        _maxTimeoutNavigate = int.Parse(GetConfigParameter("MaxTimeoutNavigate"));
                    }
                    catch
                    {
                        _maxTimeoutNavigate = 30000;
                    }
                }

                return _maxTimeoutNavigate;
            }
        }

        /// <summary>
        /// The default max timeout snapshot.
        /// </summary>
        public static int MaxTimeoutSnapshot
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxTimeoutSnapshot")))
                {
                    try
                    {
                        _maxTimeoutSnapshot = int.Parse(GetConfigParameter("MaxTimeoutSnapshot"));
                    }
                    catch
                    {
                        _maxTimeoutSnapshot = 100000;
                    }
                }

                return _maxTimeoutSnapshot;
            }
        }

        /// <summary>
        /// The default max try by crawl step.
        /// </summary>
        public static int MaxTryStep
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxTryStep")))
                {
                    try
                    {
                        _maxTryStep = int.Parse(GetConfigParameter("MaxTryStep"));
                    }
                    catch
                    {
                        _maxTryStep = 3;
                    }
                }

                return _maxTryStep;
            }
        }

        /// <summary>
        /// The default max try snap page.
        /// </summary>
        public static int MaxTrySnap
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxTrySnap")))
                {
                    try
                    {
                        _maxTrySnap = int.Parse(GetConfigParameter("MaxTrySnap"));
                    }
                    catch
                    {
                        _maxTrySnap = 3;
                    }
                }

                return _maxTrySnap;
            }
        }

        /// <summary>
        /// The default max try getting image.
        /// </summary>
        public static int MaxTryGettingImage
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxTryGettingImage")))
                {
                    try
                    {
                        _maxTryGettingImage = int.Parse(GetConfigParameter("MaxTryGettingImage"));
                    }
                    catch
                    {
                        _maxTryGettingImage = 3;
                    }
                }

                return _maxTryGettingImage;
            }
        }

        /// <summary>
        /// The default max image size.
        /// </summary>
        public static int MinImgSize
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MinImgSize")))
                {
                    try
                    {
                        _minImgSize = int.Parse(GetConfigParameter("MinImgSize"));
                    }
                    catch
                    {
                        _minImgSize = 100;
                    }
                }

                return _minImgSize;
            }
        }

        /// <summary>
        /// The default wait time resource.
        /// </summary>
        public static int WaitTimeResource
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("WaitTimeResource")))
                {
                    try
                    {
                        _waitTimeResource = int.Parse(GetConfigParameter("WaitTimeResource"));
                    }
                    catch
                    {
                        _waitTimeResource = 5000;
                    }
                }

                return _waitTimeResource;
            }
        }

        /// <summary>
        /// The default wait time resource.
        /// </summary>
        public static IList<Regex> ExcludedUrls
        {
            get
            {
                if (_excludedUrls.Count == 0 && !string.IsNullOrEmpty(GetConfigParameter("ExcludedUrls")))
                {
                    try
                    {
                        _excludedUrls.Clear();
                        string[] excludedPatterns = GetConfigParameter("ExcludedUrls").Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

                        if (excludedPatterns != null)
                            foreach (string pattern in excludedPatterns)
                                _excludedUrls.Add(new Regex(pattern,
                                    RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase));                        
                    }
                    catch
                    {
                        _excludedUrls.Clear();
                    }
                }

                return _excludedUrls;
            }
        }

        /// <summary>
        /// Flag that indicates if css are aspired from web site.
        /// </summary>
        public static bool AspireCss
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("AspireCss")))
                {
                    try
                    {
                        _aspireCss = bool.Parse(GetConfigParameter("AspireCss"));
                    }
                    catch
                    {
                        _aspireCss = false;
                    }
                }

                return _aspireCss;
            }
        }

        /// <summary>
        /// The default screenshot max pool size.
        /// </summary>
        public static int MaxSnapPoolSize
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxSnapPoolSize")))
                {
                    try
                    {
                        _maxSnapPoolSize = int.Parse(GetConfigParameter("MaxSnapPoolSize"));
                    }
                    catch
                    {
                        _maxSnapPoolSize = 10;
                    }
                }

                return _maxSnapPoolSize;
            }
        }

        /// <summary>
        /// The default max snapshot by instance.
        /// </summary>
        public static int MaxSnapByInstance
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxSnapByInstance")))
                {
                    try
                    {
                        _maxSnapByInstance = int.Parse(GetConfigParameter("MaxSnapByInstance"));
                    }
                    catch
                    {
                        _maxSnapByInstance = 50;
                    }
                }

                return _maxSnapByInstance;
            }
        }

        #endregion

        #region PictureSender

        /// <summary>
        /// Flag that indicates if ftp is used to transfer, if false user OutputDirectory.
        /// </summary>
        public static bool UseFtp
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("UseFtp")))
                {
                    try
                    {
                        _useFtp = bool.Parse(GetConfigParameter("UseFtp"));
                    }
                    catch
                    {
                        _useFtp = false;
                    }
                }

                return _useFtp;
            }
        }

        /// <summary>
        /// The maximum days history.
        /// </summary>
        public static int DaysHistory
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("DaysHistory")))
                {
                    try
                    {
                        _daysHistory = int.Parse(GetConfigParameter("DaysHistory"));
                    }
                    catch
                    {
                        _daysHistory = 15;
                    }
                }

                return _daysHistory;
            }
        }

        /// <summary>
        /// Flag that indicates if file are deleted on local.
        /// </summary>
        public static bool IsDeleteLocal
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("IsDeleteLocal")))
                {
                    try
                    {
                        _isDeleteLocal = bool.Parse(GetConfigParameter("IsDeleteLocal"));
                    }
                    catch
                    {
                        _isDeleteLocal = true;
                    }
                }

                return _isDeleteLocal;
            }
        }

        /// <summary>
        /// Flag that indicates if file are deleted on remote.
        /// </summary>
        public static bool IsDeleteRemote
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("IsDeleteRemote")))
                {
                    try
                    {
                        _isDeleteRemote = bool.Parse(GetConfigParameter("IsDeleteRemote"));
                    }
                    catch
                    {
                        _isDeleteRemote = true;
                    }
                }

                return _isDeleteRemote;
            }
        }

        #endregion

        #region PictureHeartBeat

        /// <summary>
        /// Path of current checking executable.
        /// </summary>
        public static string PathExe
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("PathExe")))
                {
                    try
                    {
                        _pathExe = GetConfigParameter("PathExe");
                    }
                    catch
                    {
                        _pathExe = "";
                    }
                }

                return _pathExe;
            }
        }

        /// <summary>
        /// Process name.
        /// </summary>
        public static string ProcessName
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("ProcessName")))
                {
                    try
                    {
                        _processName = GetConfigParameter("ProcessName");
                    }
                    catch
                    {
                        _processName = "";
                    }
                }

                return _processName;
            }
        }

        /// <summary>
        /// The maximum instance of executable launch.
        /// </summary>
        public static int MaxInstance
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("MaxInstance")))
                {
                    try
                    {
                        _maxInstance = int.Parse(GetConfigParameter("MaxInstance"));
                    }
                    catch
                    {
                        _maxInstance = 3;
                    }
                }

                return _maxInstance;
            }
        }

        /// <summary>
        /// The timer tick interval between heartbeat check.
        /// </summary>
        public static int TimerTickInterval
        {
            get
            {
                if (!string.IsNullOrEmpty(GetConfigParameter("TimerTickInterval")))
                {
                    try
                    {
                        _TimerTickInterval = int.Parse(GetConfigParameter("TimerTickInterval"));
                    }
                    catch
                    {
                        _TimerTickInterval = 3;
                    }
                }

                return _TimerTickInterval;
            }
        }

        #endregion

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Get a value parameter with its key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value parameter, string Empty otherwise.</returns>
        public static string GetConfigParameter(string key)
        {
            string paramValue = string.Empty;

            if (!string.IsNullOrEmpty(key))
                paramValue = ConfigurationManager.AppSettings[key];

            return paramValue;
        }

        /// <summary>
        /// Get the message with the corresponding key and langID.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="langID"></param>
        /// <returns></returns>
        public static string GetDefaultMessage(string key, string langID)
        {
            string paramValue = string.Empty;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(langID))
                paramValue = ConfigurationManager.AppSettings[key + "_" + langID];
            else if (!string.IsNullOrEmpty(key))
                paramValue = ConfigurationManager.AppSettings[key];

            return paramValue;
        }

        /// <summary>
        /// Get a section of current config.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetSection<T>(string name)
        {
            T section = default(T);
            try
            {
                object sectObj = ConfigurationManager.GetSection(name);
                if (sectObj is T)
                    section = (T)sectObj;
            }
            catch
            {
                section = default(T);
            }

            return section;
        }

        /// <summary>
        /// Refresh a section of current config.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void RefreshSection(string name)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(null);
            ConfigurationManager.RefreshSection(name);
            using (FileStream stream = new FileStream(config.FilePath, FileMode.Append))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(' ');
                writer.Close();
            }
        }

        #endregion
    }
}
