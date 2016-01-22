#region Information
//Authors: K.Shehzad, D.Mussuma
//Date of Creation: 29/08/2005
//Date of modification:
#endregion

using System;
using System.Data;
using System.Collections;
using WebExceptions=TNS.AdExpress.Web.Exceptions;

namespace TNS.AdExpress.Web.Rules.Selections.Medias{
	/// <summary>
	/// This class is used to format the data for publication dates of media
	/// </summary>
	internal class MediaPublicationDatesRules{

		/// <summary>
		/// This method is used to construct Hashtable of date of publications with in the specified period 
		/// </summary>
		/// <returns>return the Hashtable of medias along with their publication dates</returns>
		public static Hashtable GetData(){
			#region Variables
			DataTable publicationsData=null;
			Hashtable publications=null;
			Hashtable dates=null;
			int dateBegin=0;
			int dateEnd=0;
			int nextDate=0;
			Int64 oldMediaId=0;
			Int64 currentMediaId=0;
			string month=string.Empty;
			#endregion

			#region Period
			
			DateTime currentDate=DateTime.Now.Date;
			DateTime previousDate=currentDate.AddMonths(-16);

			//Dates de début et de fin 
			TNS.FrameWork.Date.AtomicPeriodWeek currentWeek= new TNS.FrameWork.Date.AtomicPeriodWeek(currentDate);
			TNS.FrameWork.Date.AtomicPeriodWeek previousWeek= new TNS.FrameWork.Date.AtomicPeriodWeek(previousDate);
			dateBegin = int.Parse(previousWeek.FirstDay.ToString("yyyyMMdd"));
			dateEnd = int.Parse(currentWeek.LastDay.ToString("yyyyMMdd"));
			
			
			#region ancienne version
//			if(previousDate.Month<10)
//				 month="0"+previousDate.Month;
//			else
//				 month=previousDate.Month.ToString();
//			
//			dateBegin=int.Parse(previousDate.Year.ToString()+month+"01");
//			
//			if(currentDate.Month<10)
//				month="0"+currentDate.Month;
//			else
//				month=currentDate.Month.ToString();
//
//            dateEnd=int.Parse(currentDate.Year.ToString()+month+"31");
			#endregion
			#endregion
			
			#region Get Data
			publicationsData=TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetAllPublications(dateBegin,dateEnd).Tables[0];
			#endregion

			#region Construction of the HashTable
			try
			{
				if(publicationsData!=null && publicationsData.Rows.Count>0)
				{
					publications=new Hashtable();
					for(int i=0;i<publicationsData.Rows.Count;i++)
					{
						currentMediaId=(Int64)publicationsData.Rows[i]["id_media"]; 
						if(oldMediaId!=currentMediaId)
						{
							dates=new Hashtable();
							oldMediaId=currentMediaId;
							publications.Add(oldMediaId,dates);
						}
						if((i<publicationsData.Rows.Count-1) && (currentMediaId==(Int64)(publicationsData.Rows[i+1]["id_media"])) )
							nextDate=(int)(publicationsData.Rows[i+1]["publication_date"]);
						else
							nextDate=0;					
						dates.Add((int)(publicationsData.Rows[i]["publication_date"]),nextDate);					
					
					}
				}
			}
			catch(System.Exception err)
			{
				throw(new WebExceptions.MediaPublicationDatesRulesException("GetData:: Error while formatting the data for MediaPlanPublicationDates ",err));
			}
			#endregion

			return publications;
		}
	}
}
