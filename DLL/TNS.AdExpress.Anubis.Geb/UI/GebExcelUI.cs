#region Informations
// Auteur : B.Masson
// Date de création : 24/04/2006
// Date de modification :
#endregion

#region Namespaces
using System;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;

using Oracle.DataAccess.Client;

using TNS.AdExpress.Constantes.DB;
using ClassificationDA=TNS.AdExpress.DataAccess.Classification;
using Functions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;

using TNS.AdExpress.Geb;
using GebConfiguration=TNS.AdExpress.Geb.Configuration;
using GebAlertRequest=TNS.AdExpress.Geb.AlertRequest;

using TNS.AdExpress.Anubis.Geb;
using GebExceptions=TNS.AdExpress.Anubis.Geb.Exceptions;

using TNS.FrameWork.DB.Common;
using FrameworkDate=TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpress.Anubis.Geb.UI{
	/// <summary>
	/// Composant Excel
	/// </summary>
	public class GebExcelUI{

		#region Constantes
		/// <summary>
		/// Nombre de ligne par page, utile pour l'impression
		/// </summary>
		const int NB_MAX_ROW_BY_PAGE = 65;
		/// <summary>
		/// Index de la ligne de départ pour l'écriture du fichier
		/// </summary>
		const int INDEX_START_LINE = 4;
		/// <summary>
		/// Index de la colonne de départ pour l'écriture du fichier
		/// </summary>
		const int INDEX_START_COLUMN = 0;
		/// <summary>
		/// Index de la colonne de départ du rappel de sélection pour l'écriture du fichier
		/// </summary>
		const int INDEX_COLUMN_HEADER_START=1;
		#endregion
		
		#region Variables
		/// <summary>
		/// Composant excel
		/// </summary>
        protected Workbook _excel;
		/// <summary>
		/// Licence Aspose Excel
		/// </summary>
		License _license;
		/// <summary>
		/// Index de la ligne du header du tableau (utile à l'impression)
		/// </summary>
		protected int headerRowIndex=0;
		/// <summary>
		/// Booléen pour dire si le résultat est null
		/// </summary>
		protected bool _resultIsNull = true;
        /// <summary>
        /// Date Cover
        /// </summary>
        protected string _dateCoverNum;
        /// <summary>
        /// Booléen pour dire si le support est antidaté
        /// </summary>
        protected bool _mediaAntidated = false;
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public GebExcelUI(){
			_excel = new Workbook();
			_license = new License();
			_license.SetLicense("Aspose.Cells.lic");
			//Ajout de couleur			
			AddColor(Color.FromArgb(128,128,192));
		}
		#endregion

		#region Destructeur
		/// <summary>
		/// Destructeur
		/// </summary>
		~GebExcelUI(){
			_excel=null;
			_license=null;
		}
		#endregion

		#region Résultats
		/// <summary>
		/// Résultat du détail support
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="alertParametersBlob">Paramètres de l'alerte du BLOB</param>
		/// <param name="alertParameters">Paramètres de l'alerte de la table BDD</param>
        /// <param name="config">Configuration de Geb</param>
        protected void DetailMediaResult(IDataSource source,GebAlertRequest alertParametersBlob,GebConfiguration.Alert alertParameters,TNS.AdExpress.Anubis.Geb.Common.GebConfig config) {
			try{
				// GetData
				DataTable dt = DataAccess.GebExcelDataAccess.GetDetailMedia(source,alertParametersBlob,alertParameters).Tables[0];

				// Ecriture du fichier Excel
				if(dt!=null && dt.Rows.Count>0){

					#region Variables
					int s=1;
					int upperLeftColumn=5;
					string vPageBreaks="";
					int cellRow = INDEX_START_LINE; // Index de la ligne de départ
					Worksheet sheet = _excel.Worksheets[0];
					Cells cells = sheet.Cells;
                    string couvPath="";
					#endregion

                    #region Renseignement des variables
                    _resultIsNull = false;
                    _dateCoverNum = dt.Rows[0]["date_cover_num"].ToString();

                    if(Array.IndexOf(config.MediaItemsList.Split(','),alertParameters.MediaId.ToString()) > -1)
                        _mediaAntidated = true;
                    else
                        _mediaAntidated = false;
                    #endregion

					#region Rappel de sélection
					// Ecriture du rappel avec une méthode qui retour la valeur de la 'cellRow'
					cellRow = GetHeaderExcel(source, cells, dt, alertParameters, cellRow);
					#endregion

					#region Couverture du support et Chemin de fer
                    if(_mediaAntidated)
                        couvPath = @"\\frmitch-fs03\quanti_multimedia_perf\AdexDatas\Press\SCANS\" + alertParameters.MediaId + @"\" + dt.Rows[0]["date_media_num"].ToString() + @"\imagette\coe001.jpg";
                    else
                        couvPath = @"\\frmitch-fs03\quanti_multimedia_perf\AdexDatas\Press\SCANS\" + alertParameters.MediaId + @"\" + dt.Rows[0]["date_cover_num"].ToString() + @"\imagette\coe001.jpg";
                    
					if(File.Exists(couvPath)){
						// Lien du chemin de Fer
						cells.Merge(INDEX_START_LINE-1,INDEX_START_COLUMN+6,1,2);

                        // Ancienne version
                        //sheet.Hyperlinks.Add(INDEX_START_LINE-1,INDEX_START_COLUMN+6,1,1,"http://www.tnsadexpress.com/Public/PortofolioCreationMedia.aspx?idMedia="+alertParameters.MediaId+"&date="+alertParametersBlob.DateMediaNum+"&nameMedia="+alertParameters.MediaName);
                        
                        // Nouvelle version avec changement des noms de variable dans l'url
                        if(_mediaAntidated)
                            sheet.Hyperlinks.Add(INDEX_START_LINE - 1,INDEX_START_COLUMN + 6,1,1,"http://www.tnsadexpress.com/Public/PortofolioCreationMedia.aspx?idMedia=" + alertParameters.MediaId + "&dateCoverNum=" + dt.Rows[0]["date_media_num"].ToString() + "&dateMediaNum=" + dt.Rows[0]["date_media_num"].ToString() + "&nameMedia=" + alertParameters.MediaName);
                        else
                            sheet.Hyperlinks.Add(INDEX_START_LINE - 1,INDEX_START_COLUMN + 6,1,1,"http://www.tnsadexpress.com/Public/PortofolioCreationMedia.aspx?idMedia=" + alertParameters.MediaId + "&dateCoverNum=" + dt.Rows[0]["date_cover_num"].ToString() + "&dateMediaNum=" + dt.Rows[0]["date_media_num"].ToString() + "&nameMedia=" + alertParameters.MediaName);
						
						cells[INDEX_START_LINE-1,INDEX_START_COLUMN+6].PutValue(GestionWeb.GetWebWord(1397, alertParameters.LanguageId));
						cells[INDEX_START_LINE-1,INDEX_START_COLUMN+6].Style.Font.Color = Color.FromArgb(128,128,192);
						cells[INDEX_START_LINE-1,INDEX_START_COLUMN+6].Style.Font.Underline = FontUnderlineType.Single;
						cells[INDEX_START_LINE-1,INDEX_START_COLUMN+6].Style.HorizontalAlignment=TextAlignmentType.Center;

						// Couverture
						PutCellImage(sheet,cells,INDEX_START_LINE,INDEX_START_COLUMN+6,couvPath,false,0,0,0,0);
					}
					#endregion

					#region Header du tableau
					if(cellRow < 20) cellRow = 22;

					// Créations
					PutCellValue(cells,GestionWeb.GetWebWord(1928, alertParameters.LanguageId),cellRow-1,0,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);
					// Annonceur
					PutCellValue(cells,GestionWeb.GetWebWord(857, alertParameters.LanguageId),cellRow-1,1,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,1,1,true,Color.White);	
					// Produit
					PutCellValue(cells,GestionWeb.GetWebWord(858, alertParameters.LanguageId),cellRow-1,2,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,2,2,true,Color.White);
					// Famille
					PutCellValue(cells,GestionWeb.GetWebWord(1103, alertParameters.LanguageId),cellRow-1,3,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,3,3,true,Color.White);
					// Groupe
					PutCellValue(cells,GestionWeb.GetWebWord(859, alertParameters.LanguageId),cellRow-1,4,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,4,4,true,Color.White);
					// Page
					PutCellValue(cells,GestionWeb.GetWebWord(894, alertParameters.LanguageId),cellRow-1,5,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,5,5,true,Color.White);
					// Surface en page
					PutCellValue(cells,GestionWeb.GetWebWord(1767, alertParameters.LanguageId),cellRow-1,6,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,6,6,true,Color.White);
					// Surface en mmc
					PutCellValue(cells,GestionWeb.GetWebWord(1768, alertParameters.LanguageId),cellRow-1,7,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,7,7,true,Color.White);
					// Prix (euros)
					PutCellValue(cells,GestionWeb.GetWebWord(868, alertParameters.LanguageId),cellRow-1,8,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,8,8,true,Color.White);
					// Descriptifs
					PutCellValue(cells,GestionWeb.GetWebWord(1769, alertParameters.LanguageId),cellRow-1,9,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,9,9,true,Color.White);
					// Format
					PutCellValue(cells,GestionWeb.GetWebWord(1420, alertParameters.LanguageId),cellRow-1,10,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,10,10,true,Color.White);
					// Couleur
					PutCellValue(cells,GestionWeb.GetWebWord(1438, alertParameters.LanguageId),cellRow-1,11,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,11,11,true,Color.White);
					// Rang famille
					PutCellValue(cells,GestionWeb.GetWebWord(1426, alertParameters.LanguageId),cellRow-1,12,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,12,12,true,Color.White);
					// Rang groupe
					PutCellValue(cells,GestionWeb.GetWebWord(1427, alertParameters.LanguageId),cellRow-1,13,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,13,13,true,Color.White);
					// Rang support
					PutCellValue(cells,GestionWeb.GetWebWord(1428, alertParameters.LanguageId),cellRow-1,14,true,Color.Black, true);
					CellsHeaderStyle(cells,null,cellRow-1,14,14,true,Color.White);
					
					// Mise en mémoire de la ligne du header du tableau
					headerRowIndex=cellRow;
					#endregion

					#region Lignes du tableau
					cellRow++;
					string[] visuals=null;
					string url="";
					string urlStart="";
					string imgPath="";
					string location="";
					long idOldLine=-1;
					long idLine;
					
					foreach(DataRow dr in  dt.Rows){
						urlStart="";
						url="";
						idLine=(long)dr["id_advertisement"];
						if(idLine!=idOldLine){
							location="";
							if(dr["visual"]!=null && dr["visual"]!=System.DBNull.Value){
								// Construction du lien
								visuals = dr["visual"].ToString().Split(',');
                                for(int i=0;i < visuals.GetLength(0);i++) {
                                    if(_mediaAntidated)
                                        imgPath = @"\\frmitch-fs03\quanti_multimedia_perf\AdexDatas\Press\SCANS\" + alertParameters.MediaId + @"\" + dr["date_media_num"].ToString() + @"\imagette\" + visuals[i];
                                    else
                                        imgPath = @"\\frmitch-fs03\quanti_multimedia_perf\AdexDatas\Press\SCANS\" + alertParameters.MediaId + @"\" + dr["date_cover_num"].ToString() + @"\imagette\" + visuals[i];

                                    if(File.Exists(imgPath)) {
                                        if(_mediaAntidated)
                                            url += @"/ImagesPresse/" + alertParameters.MediaId + @"/" + dr["date_media_num"].ToString() + @"/" + visuals[i] + ",";
                                        else
                                            url += @"/ImagesPresse/" + alertParameters.MediaId + @"/" + dr["date_cover_num"].ToString() + @"/" + visuals[i] + ",";
                                    }
                                }
                                if(url.Length > 0 && url!="") {
                                    // Cellule avec lien hypertext si des visuels existent
                                    urlStart = @"http://www.tnsadexpress.com/Private/Results/ZoomCreationPopUp.aspx?creation=";
                                    url = urlStart + url.Substring(0,url.Length-1);
                                    PutCellValue(sheet,cells,GestionWeb.GetWebWord(1928,alertParameters.LanguageId),cellRow-1,0,false,Color.FromArgb(128,128,192),true,url);
                                }
								else{
									// Cellule sans lien hypertext
									PutCellValue(cells,"",cellRow-1,0,false,Color.Black, true);
								}
							}
							else{
								PutCellValue(cells,"",cellRow-1,0,false,Color.Black, true);
							}
							PutCellValue(cells,dr["advertiser"].ToString(),cellRow-1,1,false,Color.Black, true);
							PutCellValue(cells,dr["product"].ToString(),cellRow-1,2,false,Color.Black, true);
							PutCellValue(cells,dr["sector"].ToString(),cellRow-1,3,false,Color.Black, true);
							PutCellValue(cells,dr["group_"].ToString(),cellRow-1,4,false,Color.Black, true);
							PutCellValue(cells,dr["media_paging"].ToString().Trim(),cellRow-1,5,false,Color.Black, true);
						
							//PutCellValue(cells,(int)dr["area_page"]/1000,cellRow-1,6,false,Color.Black, true);
                            PutCellValue(cells,Functions.Units.ConvertUnitValueToString(dr["area_page"].ToString(),TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.pages,WebApplicationParameters.AllowedLanguages[WebApplicationParameters.DefaultLanguage].CultureInfo),cellRow-1,6,false,Color.Black,true);
						
							PutCellValue(cells,dr["area_mmc"],cellRow-1,7,false,Color.Black, true);
							PutCellValue(cells,dr["expenditure_euro"],cellRow-1,8,false,Color.Black, true);
						
							//PutCellValue(cells,dr["location"].ToString(),cellRow-1,9,false,Color.Black, true);
							location=dr["location"].ToString();
							PutCellValue(cells,location,cellRow-1,9,false,Color.Black, true);
							

							PutCellValue(cells,dr["format"].ToString(),cellRow-1,10,false,Color.Black, true);
							PutCellValue(cells,dr["color"].ToString(),cellRow-1,11,false,Color.Black, true);
							PutCellValue(cells,dr["rank_sector"],cellRow-1,12,false,Color.Black, true);
							PutCellValue(cells,dr["rank_group_"],cellRow-1,13,false,Color.Black, true);
							PutCellValue(cells,dr["rank_media"],cellRow-1,14,false,Color.Black, true);
							sheet.AutoFitRow(cellRow-1);
							cellRow++;
							idOldLine=idLine;
						}
						else{
							cellRow--;
							location+=", "+dr["location"].ToString();
							PutCellValue(cells,location,cellRow-1,9,false,Color.Black, true);
							cellRow++;
						}
					}
					#endregion

					#region AutoFit
					//Ajustement de la taile des cellules en fonction du contenu
					for(int c = 0; c <= 14; c++){
						sheet.AutoFitColumn(c);
					}
					#endregion
					
					#region Mise en page
					PageSettings(sheet,dt.Rows[0]["media"].ToString(),dt,NB_MAX_ROW_BY_PAGE,ref s,upperLeftColumn,vPageBreaks);
					#endregion
				}
			}
			catch(System.Exception e){
				throw (new GebExceptions.GebExcelUIException("Impossible d'insérer des données du détail support dans le fichier excel",e));
			}
		}
		#endregion

		#region Méthodes internes

		#region Rappel de sélection
		/// <summary>
		/// Insertion du rappel de sélection
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="cells">Cellules</param>
		/// <param name="dt">Données</param>
		/// <param name="alertParameters">Paramètres de l'alerte</param>
		/// <param name="cellRow">Ligne en cours</param>
		/// <returns>Renvoi la nouvelle valeur de la ligne après écriture du rappel</returns>
        internal static int GetHeaderExcel(IDataSource source,Aspose.Cells.Cells cells,DataTable dt,GebConfiguration.Alert alertParameters,int cellRow) {

			#region Paramètres du tableaux
			PutCellValue(cells,GestionWeb.GetWebWord(512, alertParameters.LanguageId).ToUpper(),cellRow-1,INDEX_COLUMN_HEADER_START,true,Color.Gray, false);
			cellRow++;
			#endregion

			#region Détail Support
			PutCellValue(cells,GestionWeb.GetWebWord(1378, alertParameters.LanguageId).ToUpper(),cellRow-1,INDEX_COLUMN_HEADER_START,true,Color.Black, false);
			cellRow++;
			#endregion
					
			#region Support
			PutCellValue(cells,GestionWeb.GetWebWord(971, alertParameters.LanguageId)+" : "+ dt.Rows[0]["media"].ToString(),cellRow-1,INDEX_COLUMN_HEADER_START,false,Color.Black, false);
			cellRow++;
			#endregion
					
			#region Date
			PutCellValue(cells,GestionWeb.GetWebWord(895, alertParameters.LanguageId)+" : "+ FrameworkDate.DateString.YYYYMMDDToDD_MM_YYYY(dt.Rows[0]["date_media_num"].ToString(), Language.FRENCH),cellRow-1,INDEX_COLUMN_HEADER_START,false,Color.Black, false);
			cellRow++;
			#endregion

			#region Encart
			if(alertParameters.Inset){
				// Avec encarts 
				PutCellValue(cells,GestionWeb.GetWebWord(1927, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,false,Color.Black, false);
			}
			else{
				// Hors encarts
				PutCellValue(cells,GestionWeb.GetWebWord(525, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,false,Color.Black, false);
			}
			cellRow++;
			#endregion

			#region Autopromo
			if(alertParameters.Autopromo){
				// Avec autopromo et abonnement
				PutCellValue(cells,GestionWeb.GetWebWord(1926, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,false,Color.Black, false);
			}
			else{
				// Hors autopromo et abonnement
				PutCellValue(cells,GestionWeb.GetWebWord(527, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,false,Color.Black, false);
			}
			#endregion

			#region Univers Famille
			if(alertParameters.SectorListId.Length>0){
				cellRow++;
				ClassificationDA.ProductBranch.PartialSectorLevelListDataAccess sectors = new ClassificationDA.ProductBranch.PartialSectorLevelListDataAccess(alertParameters.SectorListId,alertParameters.LanguageId,source);
				PutCellValue(cells,GestionWeb.GetWebWord(1103, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,true,Color.Black, false);
				cellRow++;

				int nbCell = 0;
				int i=0;
				string[] sectorsList=alertParameters.SectorListId.Split(',');
				foreach(string current in sectorsList){
					i++;
					PutCellValue(cells,sectors[long.Parse(current)],cellRow-1,INDEX_COLUMN_HEADER_START+nbCell,false,Color.Black, false);
					if(nbCell == 3){
						nbCell=0;
						if(i < sectorsList.Length) cellRow++;
					}
					else nbCell++;
				}
			}
			#endregion
					
			#region Univers Classe
			if(alertParameters.SubSectorListId.Length>0){
				cellRow++;
				ClassificationDA.ProductBranch.PartialSubSectorLevelListDataAccess subSectors = new ClassificationDA.ProductBranch.PartialSubSectorLevelListDataAccess(alertParameters.SubSectorListId,alertParameters.LanguageId,source);
				PutCellValue(cells,GestionWeb.GetWebWord(552, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,true,Color.Black, false);
				cellRow++;

				int nbCell = 0;
				int i=0;
				string[] subSectorsList=alertParameters.SubSectorListId.Split(',');
				foreach(string current in subSectorsList){
					i++;
					PutCellValue(cells,subSectors[long.Parse(current)],cellRow-1,INDEX_COLUMN_HEADER_START+nbCell,false,Color.Black, false);
					if(nbCell == 3){
						nbCell=0;
						if(i < subSectorsList.Length) cellRow++;
					}
					else nbCell++;
				}
			}
			#endregion
					
			#region Univers Groupe
			if(alertParameters.GroupListId.Length>0){
				cellRow++;
				ClassificationDA.ProductBranch.PartialGroupLevelListDataAccess groups = new ClassificationDA.ProductBranch.PartialGroupLevelListDataAccess(alertParameters.GroupListId,alertParameters.LanguageId,source);
				PutCellValue(cells,GestionWeb.GetWebWord(859, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,true,Color.Black, false);
				cellRow++;

				int nbCell = 0;
				int i=0;
				string[] groupsList=alertParameters.GroupListId.Split(',');
				foreach(string current in groupsList){
					i++;
					PutCellValue(cells,groups[long.Parse(current)],cellRow-1,INDEX_COLUMN_HEADER_START+nbCell,false,Color.Black, false);
					if(nbCell == 3){
						nbCell=0;
						if(i < groupsList.Length) cellRow++;
					}
					else nbCell++;
				}
			}
			#endregion
					
			#region Univers Variété
			if(alertParameters.SegmentListId.Length>0){
				cellRow++;
				ClassificationDA.ProductBranch.PartialSegmentLevelListDataAccess segments = new ClassificationDA.ProductBranch.PartialSegmentLevelListDataAccess(alertParameters.SegmentListId,alertParameters.LanguageId,source);
				PutCellValue(cells,GestionWeb.GetWebWord(592, alertParameters.LanguageId),cellRow-1,INDEX_COLUMN_HEADER_START,true,Color.Black, false);
				cellRow++;

				int nbCell = 0;
				int i=0;
				string[] segmentsList=alertParameters.SegmentListId.Split(',');
				foreach(string current in segmentsList){
					i++;
					PutCellValue(cells,segments[long.Parse(current)],cellRow-1,INDEX_COLUMN_HEADER_START+nbCell,false,Color.Black, false);
					if(nbCell == 3){
						nbCell=0;
						if(i < segmentsList.Length) cellRow++;
					}
					else nbCell++;
				}
			}
			#endregion

			#region Bordures
			// Top
			for(int i = INDEX_COLUMN_HEADER_START; i < 4+INDEX_COLUMN_HEADER_START; i++){
				cells[INDEX_START_LINE-1,i].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Hair;
			}
			// Top 2 (Juste en dessous du titre du tableau)
			for(int i = INDEX_COLUMN_HEADER_START; i < 4+INDEX_COLUMN_HEADER_START; i++){
				cells[INDEX_START_LINE,i].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Hair;
			}
			// Left
			for(int i = 3; i < cellRow; i++){
				cells[i,INDEX_COLUMN_HEADER_START].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Hair;
			}
			// Right
			for(int i = 3; i < cellRow; i++){
				cells[i,INDEX_COLUMN_HEADER_START+3].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Hair;
			}
			// Bottom
			for(int i = INDEX_COLUMN_HEADER_START; i < 4+INDEX_COLUMN_HEADER_START; i++){
				cells[cellRow-1,i].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Hair;
			}
			#endregion

			cellRow = cellRow+2;
			return cellRow;
		}
		#endregion

		#region Insertion d'une donnée dans une cellule
		/// <summary>
		/// Insertion d'une donnée dans une cellule
		/// </summary>
		/// <param name="cells">Cellules</param>
		/// <param name="data">Donnée</param>
		/// <param name="row">Ligne</param>
		/// <param name="column">Colonne</param>
		/// <param name="isBold">Vrai si police en gras</param>
		/// <param name="color">Couleur de la police</param>
		/// <param name="displayBorders">Affiche ou non les bordures</param>
        internal static void PutCellValue(Aspose.Cells.Cells cells,object data,int row,int column,bool isBold,System.Drawing.Color color,bool displayBorders) {
			cells[row,column].PutValue(data);
			cells[row,column].Style.Font.Color = color;
			cells[row,column].Style.Font.IsBold = isBold;
			cells[row,column].Style.Font.Size = 8;
			if(displayBorders){
				cells[row,column].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;	
				cells[row,column].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells[row,column].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cells[row,column].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
			}
		}

		/// <summary>
		/// Insertion d'une donnée dans une cellule avec lien hypertext
		/// </summary>
		/// <param name="sheet">Feuille</param>
		/// <param name="cells">Cellules</param>
		/// <param name="data">Donnée</param>
		/// <param name="row">Ligne</param>
		/// <param name="column">Colonne</param>
		/// <param name="isBold">Vrai si police en gras</param>
		/// <param name="color">Couleur de la police</param>
		/// <param name="displayBorders">Affiche ou non les bordures</param>
		/// <param name="url">Lien URL</param>
        internal static void PutCellValue(Worksheet sheet,Aspose.Cells.Cells cells,object data,int row,int column,bool isBold,System.Drawing.Color color,bool displayBorders,string url) {
			// Lien (au niveau de la cellule)
			if(url != null) sheet.Hyperlinks.Add(row,column,1,1,url);
			cells[row,column].Style.Font.Underline = FontUnderlineType.Single;
			cells[row,column].Style.HorizontalAlignment = TextAlignmentType.Left;
			PutCellValue(cells,data,row,column,isBold,color,displayBorders);
		}
		#endregion

		#region Insertion d'une image dans une cellule
		/// <summary>
		/// Insertion d'une image dans une cellule
		/// </summary>
		/// <param name="sheet">Feuille</param>
		/// <param name="cells">Cellules</param>
		/// <param name="row">Ligne</param>
		/// <param name="column">Colonne</param>
		/// <param name="imgPath">Chemin de l'image</param>
		/// <param name="displayBorders">Affiche ou non les bordures</param>
		/// <param name="paddingLeft">Espace gauche dans la cellule</param>
		/// <param name="paddingRight">Espace droite dans la cellule</param>
		/// <param name="paddingTop">Espace haut dans la cellule</param>
		/// <param name="paddingBottom">Espace bas dans la cellule</param>
        internal static void PutCellImage(Worksheet sheet,Aspose.Cells.Cells cells,int row,int column,string imgPath,bool displayBorders,int paddingLeft,int paddingRight,int paddingTop,int paddingBottom) {
			PutCellImage(sheet,cells,row,column,imgPath,displayBorders,paddingLeft,paddingRight,paddingTop,paddingBottom,null);
		}

		/// <summary>
		/// Insertion d'une image dans une cellule avec un lien URL
		/// </summary>
		/// <param name="sheet">Feuille</param>
		/// <param name="cells">Cellules</param>
		/// <param name="row">Ligne</param>
		/// <param name="column">Colonne</param>
		/// <param name="imgPath">Chemin de l'image</param>
		/// <param name="displayBorders">Affiche ou non les bordures</param>
		/// <param name="paddingLeft">Espace gauche dans la cellule</param>
		/// <param name="paddingRight">Espace droite dans la cellule</param>
		/// <param name="paddingTop">Espace haut dans la cellule</param>
		/// <param name="paddingBottom">Espace bas dans la cellule</param>
		/// <param name="url">Lien URL</param>
		internal static void PutCellImage(Worksheet sheet,Aspose.Cells.Cells cells,int row,int column,string imgPath,bool displayBorders,int paddingLeft,int paddingRight,int paddingTop,int paddingBottom, string url){
			// Ajout de l'image dans la cellule
			Pictures pics = sheet.Pictures;
			string pictFileName = System.IO.Path.GetFullPath(imgPath);
			int pos = sheet.Pictures.Add(row,column,pictFileName);
			Picture pic = sheet.Pictures[pos];
            pic.Placement= PlacementType.Move;
			
			// Espace dans la cellule pour pouvoir positionner l'image
			//if(paddingLeft>0)	pic.Left	= paddingLeft;
			//if(paddingRight>0)	pic.Right	= paddingRight;
			//if(paddingTop>0)	pic.Top		= paddingTop;
			//if(paddingBottom>0)	pic.Bottom	= paddingBottom;

			// Lien (au niveau de la cellule)
			if(url != null){
				sheet.Hyperlinks.Add(row,column,1,1,url);
				cells[row,column].PutValue(" ");
			}
			cells[row,column].Style.Font.Size = 8;
			if(displayBorders){
				cells[row,column].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
				cells[row,column].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells[row,column].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cells[row,column].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
			}
		}
		#endregion

		#region Style des cellules en-têtes
		/// <summary>
		/// Met le style des cellules en-têtes
		/// </summary>
		/// <param name="cells">Cellules</param>
		/// <param name="data">Donnée</param>
		/// <param name="row">Ligne</param>
		/// <param name="firstColumn">1ere colonne de la collection</param>
		/// <param name="lastColumn">Dernière colonne de la collection</param>
		/// <param name="isBold">Vrai si police en gras</param>
		/// <param name="color">Couleur de la police</param>
        internal static void CellsHeaderStyle(Aspose.Cells.Cells cells,object data,int row,int firstColumn,int lastColumn,bool isBold,System.Drawing.Color color) {
			for(int i=firstColumn;i<=lastColumn;i++){
				if(data!=null)cells[row,i].PutValue(data);
				cells[row,i].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells[row,i].Style.Font.Color = color;
				cells[row,i].Style.Font.IsBold = isBold;
                cells[row,i].Style.ForegroundColor = Color.FromArgb(128,128,192);
                cells[row,i].Style.Pattern = BackgroundType.Solid;
			}			
		}
		#endregion

		#region Mise en page
		/// <summary>
		/// Mise en page de la feuille excel
		/// </summary>
		/// <param name="sheet">Feuille excel</param>
		/// <param name="name">Nom de la feuille excel</param>
		/// <param name="dt">Table de données</param>
		/// <param name="nbMaxRowByPage">Nombre de ligne maximu par page</param>
		/// <param name="s">Compteur de page</param>
		/// <param name="printArea">Zone d'impression</param>
		/// <param name="upperLeftColumn">Colone la plus à gauche</param>
		/// <param name="vPageBreaks">Saut de page vertical</param>
        internal void PageSettings(Aspose.Cells.Worksheet sheet,string name,DataTable dt,int nbMaxRowByPage,ref int s,int upperLeftColumn,string vPageBreaks) {
			
			int nbPages=0;
			nbPages=(int)Math.Ceiling(dt.Rows.Count*1.0/nbMaxRowByPage);

			// Nom de la feuille
			sheet.Name=name;

			// Nom de ligne par page pour l'impression
			for(s=1;s<=nbPages+1;s++){
				sheet.HPageBreaks.Add(nbMaxRowByPage*s,0,8);
			}
			if(vPageBreaks!=null && vPageBreaks.Length>0)
				sheet.VPageBreaks.Add(vPageBreaks);

			sheet.IsGridlinesVisible = false;
			sheet.PageSetup.Orientation = PageOrientationType.Landscape; // Format paysage
            Aspose.Cells.PageSetup pageSetup = sheet.PageSetup;

			// Set margins, in unit of inches 					
			pageSetup.TopMarginInch = 0.3; 
			pageSetup.BottomMarginInch = 0.4; 
			pageSetup.HeaderMarginInch = 0.3; 
			pageSetup.FooterMarginInch = 0; 										
			pageSetup.RightMargin=0.3;
			pageSetup.LeftMargin=0.3;
			pageSetup.CenterHorizontally=true;
			pageSetup.FitToPagesTall=32000;
			pageSetup.FitToPagesWide=1;
			pageSetup.PrintTitleRows="$"+headerRowIndex.ToString()+":$"+headerRowIndex.ToString();

			// Ajout image du logo TNS
			Pictures pics = sheet.Pictures;
			string tnsLogoPath=@"Images\logoTNSMedia.gif";	
			string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
			pics.Add(0, 0,logoPath);

			// Set current date and current time at the center section of header and change the font of the header
			pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
			// Set current page number at the center section of footer
			pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");			
		}
		#endregion

		#region Couleur
		/// <summary>
		/// Code couleur
		/// </summary>
		private int m_indexCouleur=55;
  
		/// <summary>
		/// Création éventuel de la couleur
		/// </summary>
		/// <param name="couleur">Couleur existante ? si non créé la</param>
		private void AddColor(Color couleur) {
			// Création de la couleur
			if(!_excel.IsColorInPalette(couleur)) {
				if (m_indexCouleur>=0) 
					_excel.ChangePalette(couleur, m_indexCouleur--);
			}
		}
		#endregion

		#endregion

	}
}
