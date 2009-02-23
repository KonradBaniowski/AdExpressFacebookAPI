#region Information
/*
Author : D. Mussuma
Creation : 25/01/2007
Modifications : */
#endregion


using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebCommon=TNS.AdExpress.Web.Common;
using WebCore=TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Exceptions;

using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using WebCste = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.Rules.Results
{
	/// <summary>
	/// Obtient les résultats pour les  justificatifs presse
	/// </summary>
	public class ProofRules {

		#region Constantes			
			
		const int FIRST_COL_INDEX = 1; 
		const int PRODUCT_COL_INDEX = 2; 
		const int PROOF_COL_INDEX = 3; 
		const int EUROS_COL_INDEX = 4; 
		const int PAGE_COL_INDEX = 5; 
		const int INSERTION_COL_INDEX = 6; 
		const int FORMAT_COL_INDEX = 7; 
		const int LOCATION_COL_INDEX = 8; 
		
		#endregion

		#region GetResultTable
		/// <summary>
		/// Obtient le tableau contenant l'ensemble des résultats
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>Tableau de résultats</returns>
		public static ResultTable GetResultTable(WebSession webSession,string dateBegin,string dateEnd){
			
			

			#region Variables
			ResultTable tab=null;
			DataSet ds  =null;
			Headers headers;
			int iNbLine=0;
			CellLevel currentCellLevel0 = null;
			AdExpressCellLevel currentCellLevel1=null;
			AdExpressCellLevel currentCellLevel2=null;
			
			long oldL1Id = -1;
			long oldL2Id = -1;
			long idOldAdvertisement = -1;
			long idOldDate = -1;
            int iCurLine = 0;
            int currentL3Index = -1;
			long idL3 = 0;
			#endregion
		
			try{
				//Chargement des données
				ds = TNS.AdExpress.Web.DataAccess.Results.ProofDataAccess.GetData(webSession,dateBegin,dateEnd);

				if(ds!=null  && ds.Tables[0].Rows.Count>0){

					#region Nombre de lignes du tableau du tableau
					iNbLine = 0;				
					iNbLine = GetTableResultSize(webSession,ds.Tables[0]);
					#endregion

					#region Initialisation du tableau de résultats

					headers = new Headers();

					//Colonne libellés
					headers.Root.Add(new Header(false,string.Empty,FIRST_COL_INDEX));
				
					//Colonne produit
					headers.Root.Add(new Header(false, GestionWeb.GetWebWord(1164,webSession.SiteLanguage), PRODUCT_COL_INDEX));
		
					//Colonne justificative
					headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(1731,webSession.SiteLanguage), PROOF_COL_INDEX));
								
					//Colonne euro
					headers.Root.Add(new Header(false, Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].WebTextId,webSession.SiteLanguage)), EUROS_COL_INDEX));
								
					//Colonne Pages
                    headers.Root.Add(new Header(false, Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebCste.CustomerSessions.Unit.pages].WebTextId, webSession.SiteLanguage)), PAGE_COL_INDEX));
				
					//Colonne Insertion
                    headers.Root.Add(new Header(false, Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebCste.CustomerSessions.Unit.insertion].WebTextId, webSession.SiteLanguage)), INSERTION_COL_INDEX));

					//Colonne format
					headers.Root.Add(new Header(false,GestionWeb.GetWebWord(1420,webSession.SiteLanguage), FORMAT_COL_INDEX));

					//Colonne Emplacement
					headers.Root.Add(new Header(false, GestionWeb.GetWebWord(1732,webSession.SiteLanguage), LOCATION_COL_INDEX));

				
				
					tab = new ResultTable(iNbLine, headers);
					#endregion

					#region Traitement du tableau de résultats

					#region Initialisation des totaux

					tab.AddNewLine(LineType.total);
					//Première colonne
					tab[iCurLine, FIRST_COL_INDEX] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805,webSession.SiteLanguage), 0, iCurLine, webSession);
					currentCellLevel0 = (CellLevel)tab[iCurLine,FIRST_COL_INDEX];
					InitProofLine(webSession,tab, iCurLine);

					#endregion
				
					foreach(DataRow dr in ds.Tables[0].Rows){
					
						#region On change de niveau L1 ( Régie )
					
						if( long.Parse(dr["id_media_seller"].ToString())!=oldL1Id ){
							oldL1Id = long.Parse(dr["id_media_seller"].ToString());
							oldL2Id = idOldAdvertisement = idOldDate = -1;	
							iCurLine = tab.AddNewLine(LineType.level1);	
							tab[iCurLine, FIRST_COL_INDEX] = new AdExpressCellLevel(oldL1Id,dr["media_seller"].ToString(),currentCellLevel0,1, iCurLine, webSession);
							currentCellLevel1 = (AdExpressCellLevel)tab[iCurLine,FIRST_COL_INDEX];
							InitProofLine(webSession,tab, iCurLine);						
						}
						#endregion
					
						#region On change de niveau L2 ( Support )
					
						if( long.Parse(dr["id_media"].ToString())!=oldL2Id ){
							oldL2Id = long.Parse(dr["id_media"].ToString());
							idOldAdvertisement = idOldDate = -1;					
							iCurLine = tab.AddNewLine(LineType.level2);	
							tab[iCurLine, FIRST_COL_INDEX] = new AdExpressCellLevel(oldL2Id,dr["media"].ToString(),currentCellLevel1,2, iCurLine, webSession);
							currentCellLevel2 = (AdExpressCellLevel)tab[iCurLine,FIRST_COL_INDEX];
							InitProofLine(webSession,tab, iCurLine);											
						}
						#endregion

						#region On change de niveau L3 ( Nouvelle insertion )
					
						if(int.Parse(dr["id_advertisement"].ToString()) != idOldAdvertisement || int.Parse(dr["date_media_num"].ToString()) != idOldDate){
							idOldAdvertisement = int.Parse(dr["id_advertisement"].ToString());
							idOldDate = int.Parse(dr["date_media_num"].ToString());						
							iCurLine = currentL3Index = tab.AddNewLine(LineType.level3);
							idL3++;
							string date = "";

							if (dr["date_media_num"]!= DBNull.Value){
                                date = dr["date_media_num"].ToString();
								DateTime dTime = new DateTime(int.Parse(date.Substring(0,4)), int.Parse(date.Substring(4,2)), int.Parse(date.Substring(6,2)));
								date = WebFunctions.Dates.DateToString(dTime,webSession.SiteLanguage);
							}
							else if (dr["date_cover_num"]!= DBNull.Value){
                                date = dr["date_cover_num"].ToString();
								DateTime dTime = new DateTime(int.Parse(date.Substring(0,4)), int.Parse(date.Substring(4,2)), int.Parse(date.Substring(6,2)));
								date = WebFunctions.Dates.DateToString(dTime,webSession.SiteLanguage);
							}

							//Première colonne date
							tab[iCurLine, FIRST_COL_INDEX] = new AdExpressCellLevel(idL3,date,currentCellLevel2,3,iCurLine,webSession);						
						
							//Produit
							tab[iCurLine, PRODUCT_COL_INDEX] = new CellLabel(dr["product"].ToString());

							//Lien vers justifs
							tab[iCurLine, PROOF_COL_INDEX] = new CellProofLink(webSession,dr["id_media"].ToString(),dr["id_product"].ToString(),dr["media_paging"].ToString().Trim(),dr["date_cover_num"].ToString(),dr["date_media_num"].ToString());

							// Euro
							tab[iCurLine, EUROS_COL_INDEX] = new CellEuro(0.0);
							tab.AffectValueAndAddToHierarchy(FIRST_COL_INDEX,iCurLine,EUROS_COL_INDEX,double.Parse(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString()));

							// Pages
							tab[iCurLine, PAGE_COL_INDEX] =  new CellPage(0.0);
                            tab.AffectValueAndAddToHierarchy(FIRST_COL_INDEX, iCurLine, PAGE_COL_INDEX, double.Parse(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.pages].Id.ToString()].ToString()));

							// Insertion
							tab[iCurLine, INSERTION_COL_INDEX] = new CellInsertion(0.0);
                            tab.AffectValueAndAddToHierarchy(FIRST_COL_INDEX, iCurLine, INSERTION_COL_INDEX, double.Parse(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.insertion].Id.ToString()].ToString()));

							// Format
							tab[iCurLine, FORMAT_COL_INDEX] = new CellLabel(dr["format"].ToString());

							// Emplacement
							tab[iCurLine, LOCATION_COL_INDEX] = new CellLabel(dr["location"].ToString());
					
						}else{
							// Emplacement
							((CellLabel)tab[currentL3Index, LOCATION_COL_INDEX]).Label = ((CellLabel)tab[currentL3Index, LOCATION_COL_INDEX]).Label+"  "+dr["location"].ToString();

						}

				
						#endregion
					
					}

					#endregion

				}
			}
			catch(System.Exception err){
				throw(new ProofRulesException("Erreur lors du traitement des données de la liste des insertions ",err));
			}

			return tab;
		}
		#endregion

		#region GetProofFileData
		/// <summary>
		/// Méthode pour le traitement des données d'une fiche justificative
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="idProduct">Identifiant du produit</param>
        /// <param name="dateFacial">Date faciale</param>
		/// <param name="page">Numéro de la page</param>
        /// <param name="dateParution">Date de parution</param>
		/// <returns>DataTable contenant les données</returns>
        public static DataTable GetProofFileData(WebSession webSession, string idMedia, string idProduct, string dateCoverNum, string page, string dateParution)
        {

			#region Variables
			DataTable data = null;
			DataTable dataLocation = null;
			DataTable dtResult = null;
			DataRow dr = null;
			string dateResult = "";
			int nbMediaPage = 0;
			#endregion

			try{

				#region Get Data
				data = DataAccess.Results.ProofDataAccess.GetProofFileData(webSession , idMedia, idProduct, dateParution, page).Tables[0];
				#endregion

				dtResult = new DataTable();
				dtResult.Columns.Add("idMedia", System.Type.GetType("System.String"));
				dtResult.Columns.Add("media", System.Type.GetType("System.String"));
				dtResult.Columns.Add("dateFacial", System.Type.GetType("System.DateTime"));
				dtResult.Columns.Add("datePublication", System.Type.GetType("System.DateTime"));
				dtResult.Columns.Add("advertiser", System.Type.GetType("System.String"));
				dtResult.Columns.Add("product", System.Type.GetType("System.String"));
				dtResult.Columns.Add("group_", System.Type.GetType("System.String"));
				dtResult.Columns.Add("media_paging", System.Type.GetType("System.String"));
				dtResult.Columns.Add("area_page", System.Type.GetType("System.Decimal"));
				dtResult.Columns.Add("area_mmc", System.Type.GetType("System.Decimal"));
				dtResult.Columns.Add("color", System.Type.GetType("System.String"));
				dtResult.Columns.Add("format", System.Type.GetType("System.String"));
				dtResult.Columns.Add("rank_sector", System.Type.GetType("System.String"));
				dtResult.Columns.Add("rank_group_", System.Type.GetType("System.String"));
				dtResult.Columns.Add("rank_media", System.Type.GetType("System.String"));
				dtResult.Columns.Add("id_advertisement", System.Type.GetType("System.String"));
				dtResult.Columns.Add("visual", System.Type.GetType("System.String"));
				dtResult.Columns.Add("expenditure_euro", System.Type.GetType("System.Decimal"));
				dtResult.Columns.Add("location", System.Type.GetType("System.String"));
				dtResult.Columns.Add("number_page_media", System.Type.GetType("System.Decimal"));

				if (data.Rows.Count>0){
					foreach(DataRow currentRow in data.Rows){
						dr = dtResult.NewRow();
						dtResult.Rows.Add(dr);
						dr["idMedia"]			= currentRow["id_media"].ToString();
						dr["media"]				= currentRow["media"].ToString();
						dr["advertiser"]		= currentRow["advertiser"].ToString();
						dr["product"]			= currentRow["product"].ToString();
						dr["group_"]			= currentRow["group_"].ToString();
						dr["media_paging"]		= currentRow["media_paging"].ToString();
                        dr["area_page"]         = Decimal.Parse(currentRow[UnitsInformation.List[WebCste.CustomerSessions.Unit.pages].Id.ToString()].ToString()) / 1000;
                        dr["area_mmc"]          = Decimal.Parse(currentRow[UnitsInformation.List[WebCste.CustomerSessions.Unit.mmPerCol].Id.ToString()].ToString());
						dr["color"]				= currentRow["color"].ToString();
						dr["format"]			= currentRow["format"].ToString();
						dr["rank_sector"]		= currentRow["rank_sector"].ToString();
						dr["rank_group_"]		= currentRow["rank_group_"].ToString();
						dr["rank_media"]		= currentRow["rank_media"].ToString();
						dr["visual"]			= currentRow["visual"].ToString();
                        dr["expenditure_euro"]  = Decimal.Parse(currentRow[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString());

						#region Date
                        if (currentRow["date_cover_num"].ToString().Length > 0)
                        {
                            dateResult = currentRow["date_cover_num"].ToString();
                            dr["dateFacial"] = new DateTime(int.Parse(dateResult.Substring(0, 4)), int.Parse(dateResult.Substring(4, 2)), int.Parse(dateResult.Substring(6, 2)));
						}
						dateResult = currentRow["date_media_num"].ToString();
						dr["datePublication"] = new DateTime(int.Parse(dateResult.Substring(0,4)), int.Parse(dateResult.Substring(4,2)), int.Parse(dateResult.Substring(6,2)));
						#endregion
						
						#region Récupération du descriptif
						
						dr["id_advertisement"] = currentRow["id_advertisement"].ToString();

						dataLocation = DataAccess.Results.ProofDataAccess.GetDataLocation(webSession, currentRow["id_advertisement"].ToString(), idMedia, dateCoverNum).Tables[0];
					
						string location="";
						foreach(DataRow row in dataLocation.Rows){
							location += row.ItemArray.GetValue(1).ToString()+"<br>";
						}
						dr["location"] = location;
						#endregion

						#region Récupération du nombre de pages du media
                        nbMediaPage = DataAccess.Results.ProofDataAccess.GetDataPages(webSession, idMedia, currentRow["date_media_num"].ToString());
						dr["number_page_media"]	= decimal.Parse(nbMediaPage.ToString());
						#endregion

					}
				}

			}
			catch(System.Exception err){
				throw(new ProofRulesException("Erreur lors du traitement des données de la fiche justificative ",err));
			}

			return dtResult;
		}
		#endregion

		#region Méthodes internes

		#region Nombre de ligne du tableau de résultats
		/// <summary>
		/// Calcul la taille du tableau de résultats
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <returns>nombre de ligne du tableau de résultats</returns>
		private static int GetTableResultSize(WebSession webSession, DataTable dt){

			Int64 OldL1Id=0;			
			Int64 nbL1Id=0;
			Int64 OldL2Id=0;		
			Int64 nbL2Id=0;			
			Int64 nbL3Id=0;
		
			Int64 idOldAdvertisement = -1;
			Int64 idOldDate = -1;
			
			Int64 nbLine=0;
		
			if(dt!=null && dt.Rows.Count>0){
				foreach(DataRow dr in dt.Rows){
					//NOUvelle régie
					if(long.Parse(dr["id_media_seller"].ToString()) != OldL1Id ){
						nbL1Id++;
						OldL1Id = long.Parse(dr["id_media_seller"].ToString());
						OldL2Id=idOldAdvertisement=idOldDate=-1;
					}
					//Nouveau support
					if(long.Parse(dr["id_media"].ToString()) != OldL2Id ){					
						nbL2Id++;
						OldL2Id = long.Parse(dr["id_media"].ToString());
						idOldAdvertisement=idOldDate=-1;
					}

					//Nouvelle insertion
					if(int.Parse(dr["id_advertisement"].ToString()) != idOldAdvertisement || int.Parse(dr["date_media_num"].ToString()) != idOldDate){
						idOldAdvertisement = int.Parse(dr["id_advertisement"].ToString());
						idOldDate = int.Parse(dr["date_media_num"].ToString());
						nbL3Id++;
					}
				}
				
			}
			if((nbL1Id>0) || (nbL2Id>0) || (nbL3Id>0)){
				nbLine=nbL1Id+nbL2Id+nbL3Id+1;
			}
			return (int)nbLine;
		}
		#endregion

		#region Definit une ligne du tableau
		/// <summary>
		/// Initialise un ligne de détail du tableau
		/// </summary>
        ///<param name="webSession">Session du client</param>
		/// <param name="tab">Tableau de résultat</param>
		/// <param name="iCurLine">Ligne courante</param>
        private static void InitProofLine(WebSession webSession, ResultTable tab, int iCurLine)
        {
			
			// produit
			tab[iCurLine, PRODUCT_COL_INDEX] =  new CellEmpty();

			//Lien vers justifs
			tab[iCurLine, PROOF_COL_INDEX] = new CellProofLink(webSession,string.Empty,string.Empty,string.Empty,string.Empty,string.Empty);

			// Euro
			tab[iCurLine, EUROS_COL_INDEX] = new CellEuro(0.0);

			// Pages
			tab[iCurLine, PAGE_COL_INDEX] = new CellPage(0.0);

			// Insertion
			tab[iCurLine, INSERTION_COL_INDEX] = new CellInsertion(0.0);

			// Format
			tab[iCurLine, FORMAT_COL_INDEX] = new CellEmpty();

			// Emplacement
			tab[iCurLine, LOCATION_COL_INDEX] = new CellEmpty();
		
		}
		#endregion

		#endregion
	}
}
