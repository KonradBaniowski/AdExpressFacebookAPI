
#region Informations
// Auteur: D. V. Mussuma 
// Date de cr�ation: 16/08/2005 
// Date de modification:
#endregion

using System;
using System.Data;
using System.Windows.Forms;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using FwkSelectionCst=TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.FrameWork.DB.Common;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.FrameWork.Date;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;

namespace TNS.AdExpress.Web.Rules.Selections.Grp {
	/// <summary>
	/// Classe g�rant la s�lection automatique des vagues.
	/// </summary>
	public class WavesRules{
		
		/// <summary>
		/// Obtient la vague automatiquement en fonction de la p�riode de d�but s�lectionn�e par l'utilisateur.
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de donn�es</param>
		/// <returns>vague s�lectionn�e</returns>
		internal static TreeNode GetWaves(WebSession webSession, IDataSource dataSource){
			
			#region variables
			DataSet ds=null;
			DataTable dt=null;
			System.Windows.Forms.TreeNode node=new TreeNode("wave");
			AtomicPeriodWeek atomicPeriodWeek ;
			DateTime dateBegin=DateTime.Now;			
			DateTime waveDateBegin=DateTime.Now;
			DateTime oldDateCreation=DateTime.Now;;
//			TimeSpan dayDiff;
			int indexRow=-1;			
			int i=0;
			int days=0;
			int oldDays=0;
//			bool start=true;			
			#endregion
			
			try{				
				#region Date de d�but
				if(webSession.DetailPeriod==CstPeriodDetail.monthly){
					dateBegin = new DateTime(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),01);
				}else if(webSession.DetailPeriod==CstPeriodDetail.weekly){
					atomicPeriodWeek = new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)));
					dateBegin = atomicPeriodWeek.FirstDay;
				}
				#endregion

				//Cas ou la date de d�but est inclus dans au moins une vague
				ds=WaveListDataAccess.GetWaveListDataAccess(FwkSelectionCst.Wave.Type.AEPM,dataSource,dateBegin,APPM.TypeDateBegin.inSideWave);	
				
				//Cas ou la date de d�but n'appartient � aucune vague
				if(ds==null || ds.Tables[0].Rows.Count==0)
					ds=WaveListDataAccess.GetWaveListDataAccess(FwkSelectionCst.Wave.Type.AEPM,dataSource,dateBegin,APPM.TypeDateBegin.outSideWave);	
				
				//Chargement des vagues
				if(ds!=null && ds.Tables[0].Rows.Count>0){
					dt=ds.Tables[0];
					#region choix des vagues
					indexRow=0;
					oldDateCreation = DateTime.Parse(dt.Rows[0]["date_creation"].ToString());
					waveDateBegin = DateTime.Parse(dt.Rows[0]["date_beginning"].ToString());
					oldDays = (dateBegin.Subtract(waveDateBegin)).Days;
					for(i=1;i<dt.Rows.Count;i++){
						//Choix de la vague dont la date d�but est la plus proche de celle s�lectionn�e par le client
						waveDateBegin = DateTime.Parse(dt.Rows[i]["date_beginning"].ToString());
						days = (dateBegin.Subtract(waveDateBegin)).Days;
						//S�lection de la vague dont la date de d�but est la  plus proche de la date de d�but s�lectionn�e par le client
						if( (days==oldDays && DateTime.Parse(dt.Rows[i]["date_creation"].ToString()).CompareTo(oldDateCreation)>0 ) // date de d�but identiques on prends le plus r�cent
							|| (days>=0 && days<oldDays) //Dates de d�but vagues apr�s date de d�but client : prendre la plus proche de date client
							|| (days<0 && oldDays<0 && days>oldDays) //Dates de d�but vagues avant date de d�but client : prendre la plus proche de date client
							){
							indexRow=i;
							oldDateCreation = DateTime.Parse(dt.Rows[i]["date_creation"].ToString());
							oldDays=days;	
						}																					
					}
					//Cr�ation du noeud de la vague s�lectionn�e
					node=new System.Windows.Forms.TreeNode(dt.Rows[indexRow]["wave"].ToString());
					node.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.aepmWaveAccess,long.Parse(dt.Rows[indexRow]["id_wave"].ToString()),dt.Rows[indexRow]["wave"].ToString());							
					node.Checked=true;
					#endregion					
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.WaveRulesException("GetWaves(WebSession webSession, IDataSource dataSource) :: Erreur durant la s�lection automatique des vagues. ",err));
			}		
			
			return node;
		}
	}
}
