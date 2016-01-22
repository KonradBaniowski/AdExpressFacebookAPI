using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.PromoPSA.Web.Core.Exceptions;

namespace KMI.PromoPSA.Web.Core.Sessions
{
    public class WebSessions
    {

        #region Variables
        /// <summary>
        /// WebSession list
        /// </summary>
        private static Dictionary<Int64, WebSession> _webSessionList = new Dictionary<Int64, WebSession>();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        private WebSessions()
        {
            _webSessionList = new Dictionary<Int64, WebSession>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get A specific Session
        /// </summary>
        /// <param name="sessionID">Session ID</param>
        /// <returns>specific Session</returns>
        public static WebSession Get(Int64 sessionID)
        {
            if (Contains(sessionID)) return _webSessionList[sessionID];
            else throw new ExpireSessionException("Impossible to retrieve session id '" + sessionID + "'");
        }

        /// <summary>
        /// Add a session
        /// </summary>
        /// <param name="webSession">session to add</param>
        public static void Add(WebSession webSession)
        {
            if (!Contains(webSession.CustomerLogin.IdLogin))
                _webSessionList.Add(webSession.CustomerLogin.IdLogin, webSession);
        }

        /// <summary>
        /// Remove a session
        /// </summary>
        /// <param name="sessionID">Session ID</param>
        public static void Remove(Int64 sessionID)
        {
            if (Contains(sessionID)) _webSessionList.Remove(sessionID);
        }

        /// <summary>
        /// Modify a session 
        /// </summary>
        /// <param name="sessionID">Session ID to modify</param>
        /// <param name="webSession">webSession</param>
        public static void Modify(Int64 sessionID, WebSession webSession)
        {
            if (Contains(sessionID)) _webSessionList[sessionID] = webSession;
        }

        /// <summary>
        /// Test if list contains sessionID
        /// </summary>
        /// <param name="sessionID">Session ID</param>
        /// <returns>True if contains else false</returns>
        public static bool Contains(Int64 sessionID)
        {
            return _webSessionList.ContainsKey(sessionID);
        }

        /// <summary>
        /// Get list of sessions
        /// </summary>
        /// <returns>list of sessions</returns>
        public static List<WebSession> GetList()
        {
            if (_webSessionList != null) return new List<WebSession>(_webSessionList.Values);
            else return new List<WebSession>();
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get session number
        /// </summary>
        public static int Count
        {
            get
            {
                if (_webSessionList != null) return _webSessionList.Count;
                else return 0;
            }
        }
        #endregion
    }
}
