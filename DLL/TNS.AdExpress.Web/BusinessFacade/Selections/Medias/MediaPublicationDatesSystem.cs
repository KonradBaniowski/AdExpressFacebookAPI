#region Information
//Authors: K.Shehzad, D.Mussuma
//Date of Creation: 29/08/2005
//Date of modification:
#endregion

using System;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using System.Collections;
using WebExceptions=TNS.AdExpress.Web.Exceptions;

namespace TNS.AdExpress.Web.BusinessFacade.Selections.Medias
{
	
	/// <summary>
	/// This class provides the façade to manipulate the hashtable containing the media along with their publication dates.
	/// The method GetNextPublicationDate returns the next publication date if present else returns 0.
	/// </summary>
	public static class MediaPublicationDatesSystem
	{
		
		#region variables
		/// <summary>
		/// The static hashtable which is accessable the whole application through GetNextPublicationDate
		/// </summary>
		private static Hashtable _publications;
		/// <summary>
		/// The static date responsible for the update of the parution cache system
		/// </summary>
		private static DateTime _nextUpdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 05, 00, 00);

		#endregion

        #region Constructor
        static MediaPublicationDatesSystem() {
            Init();
        }
        #endregion

        #region init
        /// <summary>
        /// This method is used to initialize the hashtable containing media with their publication dates
        /// </summary>			
		private static void Init()
		{
			try{
				if (_publications == null || _nextUpdate.CompareTo(DateTime.Now) < 0)
				{
					_publications=TNS.AdExpress.Web.Rules.Selections.Medias.MediaPublicationDatesRules.GetData();
					_nextUpdate = _nextUpdate.AddDays(1);
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.MediaPublicationDatesDataAccessException("Init():: Error while initializing the list for MediaPlan Publication Dates ",err));
			}
		}

		#endregion

		#region Accessing Data
		/// <summary>
		/// This method is used to get the next publication date 
		/// </summary>
		/// <param name="idMedia">the id of the media whose publication date is to be returned</param>
		/// <param name="publicationDate">The publication date with respect to which the next date is obtained</param>
		/// <returns>return the next date if present else returns 0</returns>
		public static int GetNextPublicationDate(Int64 idMedia,int publicationDate)
		{			
			Init();
			Hashtable temp=(Hashtable)_publications[idMedia];
			return (int)(temp[publicationDate]);
		}
		#endregion
			
	}
	
}
