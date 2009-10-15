using System;
using System.IO;
using System.Drawing;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using WebCst = TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.Date;

namespace TNS.AdExpress.PigeLuxottica.Rules
{
    public class TreatmentSystem
    {
        #region Variables
        /// <summary>
        /// Thread qui traite l'alerte
        /// </summary>
        private System.Threading.Thread _currentThread;

        TNS.AdExpress.PigeLuxottica.Rules.PigeLuxotticaConfiguration _config = null;

        /// <summary>
        /// Composant excel
        /// </summary>
        protected Workbook _excel = null;
        protected Worksheet _sheet = null;

        protected Cells _cells = null;
        protected List<Int64>  _mediaList = null;

        protected string _imagesPresseDirectory = "";
        protected string _excelFileName = "PigeLuxottica.xls";
        protected License _license = null;
        protected int _dirNum = 1;
        protected string _coversDir = "Couvertures";
        protected string _advertsDir = "Visuels";
        #endregion

        #region Events
        /// <summary>
        /// Start work
        /// </summary>
        public event StartWorkHandler StartWork;

        /// <summary>
        /// Stop  work
        /// </summary>
        public event StopWorkerJobHandler StopWorkerJob;
        /// <summary>
        /// Message 
        /// </summary>
        public event MessageHandler Message;
         /// <summary>
        /// Message 
        /// </summary>
        public event WarningMessageHandler WarningMessage;
        /// <summary>
        /// Error event 
        /// </summary>
        public event ErrorHandler Error;
        
        #endregion

        #region Delegates
        /// <summary>
        /// delegate event of error messages
        /// </summary>
        /// <param name="message">message error</param>
        /// <param name="e">Exception error</param>
        public delegate void ErrorHandler(string message, Exception e);
        /// <summary>
        /// delegate event of messages
        /// </summary>
        /// <param name="message">message</param>       
        public delegate void MessageHandler(string message);
         /// <summary>
        /// delegate event of warning messages
        /// </summary>
        /// <param name="message">message</param>       
        public delegate void WarningMessageHandler(string message);
        /// <summary>
        /// delegate event of Start work
        /// </summary>
        /// <param name="message">message</param>       
        public delegate void StartWorkHandler(string message);

        /// <summary>
        /// delegate event of stop worker job
        /// </summary>
        /// <param name="message">message</param>       
        public delegate void StopWorkerJobHandler(string message);
        #endregion

        #region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
        public TreatmentSystem(TNS.AdExpress.PigeLuxottica.Rules.PigeLuxotticaConfiguration config, string imagesPresseDirectory, string excelFileName)
        {
            _config = config;
            if (imagesPresseDirectory == null || imagesPresseDirectory.Length == 0) throw new ArgumentException(" Invalid parameter imagesPresseDirectory ");
            _imagesPresseDirectory = imagesPresseDirectory;
            if (excelFileName == null || excelFileName.Length == 0) throw new ArgumentException(" Invalid parameter excelFileName ");
            _excelFileName = excelFileName;
          
		}
		#endregion

       
     

        #region AbortTreatment

        /// <summary>
        /// Start treatment of insertions into a thread
        /// </summary>
        public void StartTreatment()
        {
            ThreadStart myThreadStart = new ThreadStart(ComputeTreatment);
            _currentThread = new Thread(myThreadStart);
            _currentThread.Name = " Pige Luxottica ";
            _currentThread.Start();
        }
        #endregion

        #region generate insertions excel file
		/// <summary>
		/// Generate the excel file for insertions and associated images
		/// </summary>
        private void ComputeTreatment()
        {
            try
            {
                Int64 oldAdvertiserId = -1;
                int cellRow = 1, cellColumn = 0, locationColumn = -1, sloganColumn = -1;
                bool isNewInsertion = false, firstlocationLabel = false;
                string oldDate = "", oldAdVertisement = "", oldIdMedia = "";
               
                //Get Data
                TNS.AdExpress.PigeLuxottica.DAL.PigeLuxotticaDAL dal = new TNS.AdExpress.PigeLuxottica.DAL.PigeLuxotticaDAL(_config.Source, _config.Login, _config.Password, _config.DataLanguage, _config.IdAdvertisers, _config.IdProducts, _config.IdVehicle, _config.BeginningDate, _config.EndDate);

                //Message(" Debut requête de chargement des données des insertions! ");
                DataSet ds = dal.GetData();
                //Message(" Fin requête de chargement des données des insertions! ");

                //Building Excel file
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    //Message(" Debut de génération du fichier Excel! ");
                    //Instance of excel work book
                    _excel = new Workbook();
                    _license = new License();
                    _license.SetLicense("Aspose.Cells.lic");
                    //Add color	
                    AddColor(Color.FromArgb(128, 128, 192));
                    _excel.Worksheets[0].IsGridlinesVisible = false;
                    try
                    {
                        string[] mediaList = TNS.AdExpress.Domain.Classification.Media.GetItemsList(WebCst.AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                        if (mediaList != null && mediaList.Length > 0)
                            _mediaList = new List<Int64>(Array.ConvertAll<string, Int64>(mediaList, (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
                    }
                    catch { }

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        isNewInsertion = false;

                        if (oldAdvertiserId != Convert.ToInt64(dr["id_advertiser"].ToString())
                            || dr["date_media_num"].ToString().CompareTo(oldDate) != 0
                            || dr["id_advertisement"].ToString().CompareTo(oldDate) != 0
                            || dr["id_media"].ToString().CompareTo(oldDate) != 0
                            )
                        {
                            isNewInsertion = true;
                            firstlocationLabel = true;
                            cellRow++;
                        }

                        if (oldAdvertiserId != Convert.ToInt64(dr["id_advertiser"].ToString()))
                        {
                            //For each New Advertiser create a new excel sheet
                            _sheet = _excel.Worksheets[_excel.Worksheets.Add()];
                            _sheet.Name = dr["advertiser"].ToString();
                            _sheet.PageSetup.Orientation = PageOrientationType.Landscape;
                            _sheet.IsGridlinesVisible = false;
                            _cells = _sheet.Cells;                           
                            oldAdvertiserId = -1;
                            //Ajout du logo TNS
                            cellRow = 3;
                          
                            cellColumn = 0; locationColumn = -1;

                            //Set columns headers
                            SetHeaders(ref cellRow, ref cellColumn, ref locationColumn, ref sloganColumn);
                             
                        }
                        //Set cells values
                        SetValues(ref cellRow, ref cellColumn, locationColumn, sloganColumn, isNewInsertion, ref firstlocationLabel, dr);
                     
                        //Ajout du logo TNS
                        Pictures pics = _sheet.Pictures;
                        string tnsLogoPath = @"Images\logoTNSMedia.gif";
                        string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tnsLogoPath);
                        int picIndex = pics.Add(0, 0, logoPath);
                        pics[picIndex].Placement = Aspose.Cells.PlacementType.Move;

                        oldAdvertiserId = Convert.ToInt64(dr["id_advertiser"].ToString());
                        oldDate = dr["date_media_num"].ToString();
                        oldAdVertisement = dr["id_advertisement"].ToString();
                        oldIdMedia = dr["id_media"].ToString();
                    }
                    _excel.Save(_imagesPresseDirectory + _excelFileName, FileFormatType.Excel2003);
                    //Message(" Fin de génération du fichier Excel! ");
                }
                else
                {
                    WarningMessage(" Aucune données trouvées ! ");
                }
            }
            catch (Exception ex)
            {
                Error(" Erreur : impossible de générer le fichier Excel et ses images.", ex);
            }
            finally
            {
                _mediaList = null;
                _excel = null;
                _license = null;
                _sheet = null;
            }

        }
        #endregion

        #region AbortTreatment

        /// <summary>
        /// Stop thread
        /// </summary>
        public void AbortTreatment()
        {
            _currentThread.Abort();
        }
        #endregion
       
        #region Fire Event Methods
        /// <summary>
        /// Fire Error event 
        /// </summary>
        /// <param name="message">message error</param>
        /// <param name="e">Exception error</param>
        /// <returns></returns>
        protected void OnError(string message, Exception e)
        {
            if (Error != null)
                Error(message, e);
        }
        /// <summary>
        /// Fire Message event 
        /// </summary>
        /// <param name="message">message </param>     
        /// <returns></returns>
        protected void OnMessage(string message)
        {
            if (Message != null)
                Message(message);
        }
         /// <summary>
        /// Fire Warning Message event 
        /// </summary>
        /// <param name="message">message </param>     
        /// <returns></returns>
        protected void OnWarningMessage(string message)
        {
            if (WarningMessage != null)
                WarningMessage(message);
        }
        /// <summary>
        /// Fire strat work event 
        /// </summary>
        /// <param name="message">message</param>       
        /// <returns></returns>
        protected void OnStartWork(string message)
        {
            if (StartWork != null)
                StartWork(message);
        }
        /// <summary>
        /// Fire stop job event 
        /// </summary>
        /// <param name="message">message</param>       
        /// <returns></returns>
        protected void OnStopWorkerJob(string message)
        {
            if (StopWorkerJob != null)
                StopWorkerJob(message);
        }
        #endregion

        #region Méthodes internes
        /// <summary>
        /// Code couleur
        /// </summary>
        private int m_indexCouleur = 55;

        /// <summary>
        /// Création éventuel de la couleur
        /// </summary>
        /// <param name="couleur">Couleur existante ? si non créé la</param>
        private void AddColor(Color couleur)
        {
            // Création de la couleur
            if (!_excel.IsColorInPalette(couleur))
            {
                if (m_indexCouleur >= 0)
                    _excel.ChangePalette(couleur, m_indexCouleur--);
            }
        }

        protected void SetHeaders(ref int cellRow, ref int cellColumn, ref int locationColumn, ref  int sloganColumn)
        {
            int firstCol = 0, lastCol = 0;
            firstCol = cellColumn;
            _cells[cellRow, cellColumn].PutValue("Catégorie");
          
             cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Support");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Date");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Page");
            cellColumn++;   
            _cells[cellRow, cellColumn].PutValue("Couverture");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Visuels");
            cellColumn++;                             
            _cells[cellRow, cellColumn].PutValue("Annonceur");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Groupe");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Produit");
            cellColumn++;
            //Slogan
            sloganColumn = cellColumn;
            _cells[cellRow, cellColumn].PutValue("Version");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Centre d'intérêt");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Régie");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Format");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Surface");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Couleur");
            cellColumn++;
            _cells[cellRow, cellColumn].PutValue("Position");
            cellColumn++;
            //Price
            _cells[cellRow, cellColumn].PutValue("Prix €");
            cellColumn++;
            //Location
            locationColumn = cellColumn;
            lastCol = cellColumn;
            _cells[cellRow, cellColumn].PutValue("Descriptif");
           

            //Styles
            CellsHeaderStyle(_cells, cellRow, firstCol, lastCol, true, Color.White);
           
					
            cellColumn = 0;
            cellRow++;
        }

        protected void SetValues(ref int cellRow, ref int cellColumn, int locationColumn, int sloganColumn, bool isNewInsertion,ref bool firstlocationLabel, DataRow dr)
        {
            DateTime date;
            int hplIndex = 0;
            long idMedia = -1;
            string coverOldPath = "", coverPath = "", visualPath = "", visualOldPath="";
            FileInfo fileInfo = null;
            string temp = "";
            string dateParution = "";
            int firstCol = 0, lastCol = 0;
            if (isNewInsertion)
            {
                firstlocationLabel = true;

                //Get id vehicle
                idMedia = Convert.ToInt64(dr["id_media"].ToString());
                firstCol = cellColumn;
                //Sub media
                _cells[cellRow, cellColumn].PutValue(dr["category"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //vehicle
                _cells[cellRow, cellColumn].PutValue(dr["media"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Date
                date = new DateTime(int.Parse(dr["date_media_num"].ToString().Substring(0, 4)), int.Parse(dr["date_media_num"].ToString().Substring(4, 2)), int.Parse(dr["date_media_num"].ToString().Substring(6, 2)));
                _cells[cellRow, cellColumn].PutValue(String.Format("{0:dd/MM/yyyy}", date));
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Page
                _cells[cellRow, cellColumn].PutValue(dr["media_paging"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;

                //Cover
                if (dr["disponibility_visual"] != System.DBNull.Value && int.Parse(dr["disponibility_visual"].ToString()) <= 10)
                {
                    if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(idMedia))
                    {
                        coverPath = idMedia + @"\" + dr["date_media_num"].ToString() + @"\" + WebCst.CreationServerPathes.COUVERTURE;
                        dateParution = dr["date_media_num"].ToString().Trim();
                    }
                    else
                    {
                        coverPath = idMedia + @"\" + dr["date_cover_num"].ToString() + @"\" + WebCst.CreationServerPathes.COUVERTURE;
                        dateParution = dr["date_cover_num"].ToString().Trim();
                    }
                }
                else coverPath = "";

                if (coverPath.Length > 0)
                {
                    coverOldPath = WebCst.CreationServerPathes.LOCAL_PATH_IMAGE + coverPath;                 
                    if (File.Exists(coverOldPath))
                    {
                        if (!Directory.Exists(_imagesPresseDirectory + _coversDir)) Directory.CreateDirectory(_imagesPresseDirectory + _coversDir);
                        if (!Directory.Exists(_imagesPresseDirectory + _coversDir + @"\" + idMedia)) Directory.CreateDirectory(_imagesPresseDirectory + _coversDir + @"\" + idMedia);
                        if (!Directory.Exists(_imagesPresseDirectory + _coversDir + @"\" + idMedia + @"\" + dateParution)) Directory.CreateDirectory(_imagesPresseDirectory + _coversDir + @"\" + idMedia + @"\" + dateParution);
                        
                        fileInfo = new FileInfo(coverOldPath);
                        fileInfo.CopyTo(_imagesPresseDirectory +  _coversDir + @"\" + coverPath,true);
                        hplIndex = _sheet.Hyperlinks.Add(_cells[cellRow, cellColumn].Name, 1, 1,  _coversDir + @"\" + coverPath);
                        _sheet.Hyperlinks[hplIndex].TextToDisplay = "Couverture";
                      
                    }
                }
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;

                //Visuals
                if (dr["visual"] != System.DBNull.Value && dr["disponibility_visual"] != System.DBNull.Value && Convert.ToInt64(dr["disponibility_visual"].ToString()) <= 10
                     &&  Convert.ToInt64(dr["disponibility_visual"].ToString()) <= 100)
                {
                    
                    string[] visuals = dr["visual"].ToString().Split(',');
                    bool hasCopy = false;
                    for (int i = 0; i < visuals.Length; i++)
                    {
                        if (_mediaList != null && _mediaList.Count > 0 && _mediaList.Contains(idMedia))
                        {
                            visualPath = idMedia + @"\" + dr["date_media_num"].ToString() + @"\" + visuals[i];
                            dateParution = dr["date_media_num"].ToString().Trim();
                        }
                        else
                        {
                            visualPath = idMedia + @"\" + dr["date_cover_num"].ToString() + @"\" + visuals[i];
                            dateParution = dr["date_cover_num"].ToString().Trim();
                        }

                        visualOldPath = WebCst.CreationServerPathes.LOCAL_PATH_IMAGE + visualPath;
                        if (File.Exists(visualOldPath))
                        {
                            if (!Directory.Exists(_imagesPresseDirectory + _advertsDir)) Directory.CreateDirectory(_imagesPresseDirectory + _advertsDir);
                            if (!Directory.Exists(_imagesPresseDirectory + _advertsDir + @"\" + _dirNum)) Directory.CreateDirectory(_imagesPresseDirectory + _advertsDir + @"\" + _dirNum);
                        
                            fileInfo = new FileInfo(visualOldPath);
                            fileInfo.CopyTo(_imagesPresseDirectory + _advertsDir + @"\" + _dirNum + @"\" + visuals[i], true);
                            hplIndex = _sheet.Hyperlinks.Add(_cells[cellRow, cellColumn].Name, 1, 1, _imagesPresseDirectory + _advertsDir + @"\" + _dirNum);
                            _sheet.Hyperlinks[hplIndex].TextToDisplay = "Visuels";
                            hasCopy = true;
                        }
                    }
                   if(hasCopy) _dirNum++;
                                       
                }
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;

                //Advertser
                _cells[cellRow, cellColumn].PutValue(dr["advertiser"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Group
                _cells[cellRow, cellColumn].PutValue(dr["group_"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Product
                _cells[cellRow, cellColumn].PutValue(dr["product"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn = cellColumn + 2;
            }
            //Slogan
            _cells[cellRow, sloganColumn].PutValue(Convert.ToDouble(dr["slogan"].ToString()));
            CellsStyle(_cells, cellRow, sloganColumn, false, Color.Black);

            if (isNewInsertion)
            {
                //Media genre
                _cells[cellRow, cellColumn].PutValue(dr["interest_center"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Media owner
                _cells[cellRow, cellColumn].PutValue(dr["media_seller"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Format
                _cells[cellRow, cellColumn].PutValue(dr["format"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Surface
                _cells[cellRow, cellColumn].PutValue(Convert.ToDouble(dr["area_page"].ToString())/1000);
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Color
                _cells[cellRow, cellColumn].PutValue(dr["color"].ToString());
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Position
                _cells[cellRow, cellColumn].PutValue(Convert.ToInt64(dr["rank_media"].ToString()));
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;
                //Price
                _cells[cellRow, cellColumn].PutValue(Convert.ToDouble(dr["expenditure_euro"].ToString()));
                CellsStyle(_cells, cellRow, cellColumn, false, Color.Black);
                cellColumn++;

              
            }

            //Descritpiton
            if (firstlocationLabel)
            {
                _cells[cellRow, locationColumn].PutValue(dr["location"].ToString());
                CellsStyle(_cells, cellRow, locationColumn, false, Color.Black);
            }
            else
            {
                object val = _cells[cellRow, locationColumn].Value;
                if (val != null) temp = val.ToString();
                temp = (temp.Length > 0) ? temp + "," + dr["location"].ToString() : temp;
                _cells[cellRow, locationColumn].PutValue(temp);
                CellsStyle(_cells, cellRow, locationColumn, false, Color.Black);
            }
            firstlocationLabel = false;

            lastCol = cellColumn;

            _sheet.AutoFitColumns();
           

            cellColumn = 0;

        }
        #endregion

        #region Style des cellules en-têtes
        /// <summary>
        /// Met le style des cellules en-têtes
        /// </summary>
        /// <param name="cells">cellules</param>
        /// <param name="data">donnée</param>
        /// <param name="row">ligne</param>
        /// <param name="firstColumn">1ere colonne de la collection</param>
        /// <param name="lastColumn">dernière colonne de la collection</param>
        /// <param name="isBold">vrai si police en gras</param>
        /// <param name="color">Couleur de la police</param>
        internal  void CellsHeaderStyle(Aspose.Cells.Cells cells,  int row, int firstColumn, int lastColumn, bool isBold, System.Drawing.Color color)
        {
            for (int i = firstColumn; i <= lastColumn; i++)
            {
                cells[row, i].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                cells[row, i].Style.ForegroundColor = Color.FromArgb(128, 128, 192);
                cells[row, i].Style.Pattern = BackgroundType.Solid;
                cells[row, i].Style.Font.Color = color;
                cells[row, i].Style.Font.IsBold = isBold;
            }
        }

        #region Insertion d'une donnée dans une cellule
        /// <summary>
        /// Insert une donnée dans une cellule
        /// </summary>
        /// <param name="cells">Cellules</param>
        /// <param name="data">donnée</param>
        /// <param name="row">ligne</param>
        /// <param name="column">colonne</param>
        /// <param name="isBold">vrai si police en gras</param>
        /// <param name="color">couleur de la police</param>
        internal  void CellsStyle(Aspose.Cells.Cells cells,  int row, int column, bool isBold, System.Drawing.Color color)
        {
           
            cells[row, column].Style.Font.Color = color;
            cells[row, column].Style.Font.IsBold = isBold;
            cells[row, column].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            cells[row, column].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            cells[row, column].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            cells[row, column].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        }
        #endregion

        #endregion

    }
}
