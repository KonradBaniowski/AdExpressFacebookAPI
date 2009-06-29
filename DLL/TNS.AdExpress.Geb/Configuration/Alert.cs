#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Data;
using Oracle.DataAccess.Client;
using AdExClassification=TNS.AdExpress.DataAccess.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Geb.Configuration{
	/// <summary>
	/// Classe de configuration d'une alerte
	/// </summary>
	public class Alert{

		#region Variables
		/// <summary>
		/// Identifiant de l'alerte
		/// </summary>
		private Int64 _alertId = 0;
		/// <summary>
		/// Identifiant du support
		/// </summary>
		private int _mediaId = 0;
		/// <summary>
		/// Identifiant du type d'alerte
		/// </summary>
		private int _typeAlertId = 0;
		/// <summary>
		/// Nom de l'alerte (servant à connaitre l'alerte dans la partie de configuration)
		/// </summary>
		private string _alertName = null;
		/// <summary>
		/// Nom du support (servant à l'objet du mail)
		/// </summary>
		private string _mediaName = null;
		/// <summary>
		/// Liste des emails destinataires
		/// </summary>
		private ArrayList _emailList = new ArrayList();
		/// <summary>
		/// Avec (true) ou hors (false) encart
		/// </summary>
		private bool _inset = true;
		/// <summary>
		/// Avec (true) ou hors (false) autopromo
		/// </summary>
		private bool _autopromo = true;
		/// <summary>
		/// Liste des familles (identifiants)
		/// </summary>
		private string _sectorListId = "";
		/// <summary>
		/// Liste des classes (identifiants)
		/// </summary>
		private string _subSectorListId = "";
		/// <summary>
		/// Liste des groupes (identifiants)
		/// </summary>
		private string _groupListId = "";
		/// <summary>
		/// Liste des variétés (identifiants)
		/// </summary>
		private string _segmentListId = "";
		/// <summary>
		/// Identifiant de la langue
		/// </summary>
		private int _languageId = 33;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public Alert(IDataSource source, Int64 alertId){
			string[] emails = null;
			try{
				// GetData
				DataSet dsUniverse = DataAccess.Configuration.AlertDataAccess.LoadUniverse(source, alertId);
				DataSet dsFlag = DataAccess.Configuration.AlertDataAccess.LoadFlag(source, alertId);

				if(dsUniverse!=null && dsUniverse.Tables.Count>0 && dsUniverse.Tables[0]!=null && dsUniverse.Tables[0].Rows.Count>0){
					_alertId = alertId;
					_typeAlertId = int.Parse(dsUniverse.Tables[0].Rows[0]["id_alert_type"].ToString());
					_alertName = dsUniverse.Tables[0].Rows[0]["alert"].ToString();
					emails = dsUniverse.Tables[0].Rows[0]["email_list"].ToString().Split(',');
					foreach(string currentEmail in emails){
						_emailList.Add(currentEmail);
					}
					foreach(DataRow currentRow in dsUniverse.Tables[0].Rows){
						switch(Int64.Parse(currentRow["id_alert_universe_type"].ToString())){
							case AlertUniverseType.FAMILLE_VALUE: // 1
								_sectorListId = currentRow["universe_list"].ToString();
								break;
							case AlertUniverseType.CLASSE_VALUE: // 2
								_subSectorListId = currentRow["universe_list"].ToString();
								break;
							case AlertUniverseType.GROUPE_VALUE: // 3
								_groupListId = currentRow["universe_list"].ToString();
								break;
							case AlertUniverseType.VARIETE_VALUE: // 4
								_segmentListId = currentRow["universe_list"].ToString();
								break;
							case AlertUniverseType.SUPPORT_VALUE: // 7
								_mediaId = int.Parse(currentRow["universe_list"].ToString());
								break;
						}
					}
					AdExClassification.MediaBranch.PartialMediaListDataAccess mediaName = new AdExClassification.MediaBranch.PartialMediaListDataAccess(_mediaId.ToString(),_languageId,source);
					_mediaName = mediaName[_mediaId].ToString();
				}
				if(dsFlag!=null && dsFlag.Tables.Count>0 && dsFlag.Tables[0]!=null && dsFlag.Tables[0].Rows.Count>0){
					foreach(DataRow currentFlagRow in dsFlag.Tables[0].Rows){
						// Si précisé dans la table alert_flag_assignment, on veut du hors encart et/ou hors autopromo
						switch(Int64.Parse(currentFlagRow["id_alert_flag"].ToString())){
							case 1:
								_inset = false; // False pour hors encart
								break;
							case 2:
								_autopromo = false; // False pour hors autopromo
								break;
						}
					}
				}
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion
		
		#region Constructeur temporaire
//		/// <summary>
//		/// Constructeur temporaire
//		/// </summary>
//		public Alert(){
//			_alertId = 1;
//			_mediaId = 1826;
//			_typeAlertId = 1;
//			_alertName = "Le nom de mon alerte";
//			_mediaName = "BIBA";
//			_emailList.Add("benjamin.masson@tnsmi.fr");
////			_emailList.Add("guillaume.facon@club-internet.fr");
////			_emailList.Add("denis.gaucher@tnsmi.fr");
////			_emailList.Add("sophie.lebarazer@tnsmi.fr");
////			_emailList.Add("eric.trousset@tnsmi.fr");
////			_emailList.Add("cyrille.marie@tnsmi.fr");
//			
//			// Avec encart
//			_inset = true;
//
//			// Avec autopromo
//			_autopromo = true;
//			
//			_sectorListId = "10"; // Toilette beauté
//			_subSectorListId = "37,36,41,39,42,40,38";
//			_groupListId = "352,390,314,330,324,392,335,394,347,332";
//			_segmentListId = "35201,39001,31403,32403,33003,33203,33503,34703,35203,39003,39203,39403,33001,31402,32402,33002,33202,33502,34702,35202,39002,39202,39402,31401,35219,39019,33019,31419,32419,39219,33519,39419,34719,33219,32401,31404,32404,33004,33204,33504,34704,35204,39004,39204,39404,39201,33501,39401,34701,33201,31405,32405,33005,33205,33505,34705,35205,39005,39205,39405";
//			
////			_sectorListId = "";
////			_subSectorListId = "";
////			_groupListId = "";
////			_segmentListId = "";
//
//			_languageId = 33;
//		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou défini l'identifiant de l'alerte
		/// </summary>
		public Int64 AlertId{
			get{return _alertId;}
		}

		/// <summary>
		/// Obtient ou défini l'identifiant du support
		/// </summary>
		public int MediaId{
			get{return _mediaId;}
		}

		/// <summary>
		/// Obtient ou défini l'identifiant du type d'alerte
		/// </summary>
		public int TypeAlertId{
			get{return _typeAlertId;}
		}

		/// <summary>
		/// Obtient ou défini le nom de l'alerte
		/// </summary>
		public string AlertName{
			get{return _alertName;}
		}

		/// <summary>
		/// Obtient ou défini le nom du support
		/// </summary>
		public string MediaName{
			get{return _mediaName;}
		}

		/// <summary>
		/// Obtient ou défini la liste des emails destinataires
		/// </summary>
		public ArrayList EmailList{
			get{return _emailList;}
		}

		/// <summary>
		/// Obtient ou défini le paramètre avec ou hors encart
		/// </summary>
		public bool Inset{
			get{return _inset;}
		}

		/// <summary>
		/// Obtient ou défini le paramètre avec ou hors autopromo
		/// </summary>
		public bool Autopromo{
			get{return _autopromo;}
		}

		/// <summary>
		/// Obtient ou défini la liste des identifiants famille
		/// </summary>
		public string SectorListId{
			get{return _sectorListId;}
		}

		/// <summary>
		/// Obtient ou défini la liste des identifiants classe
		/// </summary>
		public string SubSectorListId{
			get{return _subSectorListId;}
		}

		/// <summary>
		/// Obtient ou défini la liste des identifiants groupe
		/// </summary>
		public string GroupListId{
			get{return _groupListId;}
		}

		/// <summary>
		/// Obtient ou défini la liste des identifiants variété
		/// </summary>
		public string SegmentListId{
			get{return _segmentListId;}
		}

		/// <summary>
		/// Obtient ou défini l'identifiant de la langue
		/// </summary>
		public int LanguageId{
			get{return _languageId;}
		}
		#endregion

	}
}
