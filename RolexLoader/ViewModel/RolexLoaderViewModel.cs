using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Win32;
using RolexLoader.Exceptions;
using RolexLoader.Helpers;
using TNS.AdExpress.Rolex.Loader.DAL;
using TNS.AdExpress.Rolex.Loader.DAL.Exceptions;
using TNS.AdExpress.Rolex.Loader.Domain;
using TNS.FrameWork.Date;
using TNS.FrameWork.Exceptions;

namespace RolexLoader.ViewModel
{
    public class RolexLoaderViewModel : INotifyPropertyChanged
    {
        #region Variables
        /// <summary>
        /// File Info to load
        /// </summary>
        private FileInfo _fInfo;
        /// <summary>
        /// End period to delete
        /// </summary>
        private DateTime _periodEndToDelete = DateTime.Now;

        /// <summary>
        /// Begining period to delete
        /// </summary>
        private DateTime _periodBeginningToDelete = DateTime.Now;

        private bool _isIndeterminateLoading = false;

        #endregion

        #region  Properties
        public RelayCommand OpenCommand
        {
            get
            {
                if (this._openCommand == null)
                    this._openCommand = new RelayCommand(ExecuteOpenFileDialog);

                return this._openCommand;
            }
        }


        public RelayCommand SaveCommand
        {
            get
            {
                if (this._saveCommand == null)
                    this._saveCommand = new RelayCommand(ExecuteSave);

                return this._saveCommand;
            }
        }


        public RelayCommand DeleteCommand
        {
            get
            {
                if (this._deleteCommand == null)
                    this._deleteCommand = new RelayCommand(ExecuteDelete);

                return this._deleteCommand;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public bool IsIndeterminateLoading
        {
            get { return _isIndeterminateLoading; }
            set
            {
                _isIndeterminateLoading = value;
                RaisePropertyChanged("IsIndeterminateLoading");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime PeriodBeginningToDelete
        {
            get { return _periodBeginningToDelete; }
            set
            {
                _periodBeginningToDelete = value;
                RaisePropertyChanged("PeriodBeginningToDelete");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime PeriodEndToDelete
        {
            get { return _periodEndToDelete; }
            set
            {
                _periodEndToDelete = value;
                RaisePropertyChanged("PeriodEndToDelete");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public FileInfo CurrentFile
        {
            get { return _fInfo; }
            set
            {
                _fInfo = value;
                RaisePropertyChanged("CurrentFile");
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public RolexLoaderViewModel()
        {
            String connexionSting =
                 String.Format(
                     Provider.GetConnectionStringByProvider(ConfigurationManager.AppSettings["ProviderDataAccess"]),
                     ConfigurationManager.AppSettings["DbUserName"],
                     ConfigurationManager.AppSettings["DbPassword"],
                     ConfigurationManager.AppSettings["DbDataSource"]);

            //Load revalo configuration
            _rolexConfig = new RolexConfiguration
                               {
                                   ConfigurationReportFilePath =
                                       ConfigurationManager.AppSettings["ConfigurationReportFilePath"],
                                   CustomerMailFrom = ConfigurationManager.AppSettings["Sender"],
                                   CustomerMailPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]),
                                   CustomerMailServer = ConfigurationManager.AppSettings["SmtpServer"],
                                   ProviderDataAccess = ConfigurationManager.AppSettings["ProviderDataAccess"],
                                   Recipients = ConfigurationManager.AppSettings["Recipients"],
                                   ConnectionString = connexionSting,
                                   ViualPath = ConfigurationManager.AppSettings["VisualPath"]
                               };

        }
        #endregion

        #region Commands
        /// <summary>
        /// Open File Command
        /// </summary>
        private RelayCommand _openCommand;
        /// <summary>
        /// Save command
        /// </summary>
        private RelayCommand _saveCommand;
        /// <summary>
        /// Delete command
        /// </summary>
        private RelayCommand _deleteCommand;

        private RolexConfiguration _rolexConfig;

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string pstrPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pstrPropertyName));

        }
        #endregion

        #region ExecuteOpenFileDialog
        private void ExecuteOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Excel Files (.xls;*xlsx;*xlsm)|*.xls;*xlsx;*xlsm";
            openFileDialog.FilterIndex = 1;
            openFileDialog.CheckFileExists = true;
            if (CurrentFile != null && CurrentFile.Exists)
                openFileDialog.InitialDirectory = Path.GetDirectoryName(CurrentFile.FullName);

            if (openFileDialog.ShowDialog().Value)
            {
                CurrentFile = new FileInfo(openFileDialog.FileName);
            }
        }
        #endregion

        #region ExecuteSave
        private void ExecuteSave()
        {
            try
            {

                IsIndeterminateLoading = true;

                var dal = new DataAccessDAL(_rolexConfig.ConnectionString, _rolexConfig.ProviderDataAccess);
                using (var db = dal.DataAccessDb.CreateDbManager())
                {
                    try
                    {
                        db.BeginTransaction();

                        List<PictureMatching>  pictures;
                        long weekDate;

                        var rolexDetails = dal.GetRolexDetails(CurrentFile.FullName,out pictures, db);
                        try
                        {
                            //Check if has data
                            string str = Path.GetFileNameWithoutExtension(CurrentFile.FullName).Replace("Rolex_", "");
                             weekDate = long.Parse(str);
                        }
                        catch (Exception ex)
                        {
                            throw new ExcelFormatException("Incorrect File Name", ex);
                        }
                     

                        if (dal.HasData(weekDate, weekDate, db))
                        {
                            MessageBoxResult messageBoxResult = MessageBox.Show("Des données existent en base de données pour cette periode, voulez vous les effacer ?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                            if (messageBoxResult == MessageBoxResult.Cancel) return;
                            if (messageBoxResult == MessageBoxResult.Yes) dal.Delete(weekDate, weekDate, db);
                        }
                        dal.Insert(rolexDetails, weekDate, db);

                        //Save visuals in directory file
                        #region Copy Pictures
                        const string imagetteDirectoryName = "imagette";
                        foreach (PictureMatching pictureMatching in pictures)
                        {
                            File.Copy(pictureMatching.PathIn, Path.Combine(_rolexConfig.ViualPath, pictureMatching.PathOut), true);
                            SaveImagette(imagetteDirectoryName, Path.Combine(_rolexConfig.ViualPath, pictureMatching.PathOut));
                        }                      
                        #endregion


                        db.CommitTransaction();
                        MessageBox.Show("Les données ont été enregistrées ", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        db.RollbackTransaction();
                        OnError(ex);
                    }

                }

            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                IsIndeterminateLoading = false;
            }
        }
        #endregion

        #region ExecuteDelete
        private void ExecuteDelete()
        {
            try
            {               

                IsIndeterminateLoading = true;
                var dal = new DataAccessDAL(_rolexConfig.ConnectionString, _rolexConfig.ProviderDataAccess);


                using (var db = dal.DataAccessDb.CreateDbManager())
                {
                    try
                    {
                        string endWeek;
                        string beginningWeek;
                        GetWeeks(out endWeek, out beginningWeek);

                        if (dal.HasData(Convert.ToInt64(beginningWeek), Convert.ToInt64(endWeek), db))
                        {
                            MessageBoxResult messageBoxResult =
                                MessageBox.Show(
                                    "Des données existent en base de données pour cette periode, voulez vous les effacer ?",
                                    "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                            if (messageBoxResult == MessageBoxResult.Cancel)
                                return;
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {

                                dal.Delete(Convert.ToInt64(beginningWeek), Convert.ToInt64(endWeek), db);
                                MessageBox.Show("Les données ont été supprimés", "Information", MessageBoxButton.OK,
                                                MessageBoxImage.Information);
                            }
                        }

                        else
                            MessageBox.Show("Aucune données en base pour cette période", "Information", MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        db.RollbackTransaction();
                        OnError(ex);
                    }
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
            finally
            {
                IsIndeterminateLoading = false;

            }
        }

        private void GetWeeks(out string endWeek, out string beginningWeek)
        {
            var dateBegin = new AtomicPeriodWeek(_periodBeginningToDelete);
            beginningWeek = dateBegin.Year.ToString(CultureInfo.InvariantCulture) +
                            ((dateBegin.Week.ToString(CultureInfo.InvariantCulture).Length == 1)
                                 ? "0" + dateBegin.Week.ToString(CultureInfo.InvariantCulture)
                                 : dateBegin.Week.ToString(CultureInfo.InvariantCulture));

            var dateEnd = new AtomicPeriodWeek(_periodEndToDelete);
            endWeek = dateEnd.Year.ToString(CultureInfo.InvariantCulture) +
                      ((dateEnd.Week.ToString(CultureInfo.InvariantCulture).Length == 1)
                           ? "0" + dateEnd.Week.ToString(CultureInfo.InvariantCulture)
                           : dateEnd.Week.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region Error management
        /// <summary>
        ///Error  Event handler
        /// </summary>
        /// <param name="e">Argument</param>
        protected void OnError(Exception e)
        {


            Exception err = null;
            CustomerWebException cwe = null;
            try
            {
                err = (BaseException)e;
                cwe = new CustomerWebException(err.Message, ((BaseException)err).GetHtmlDetail());             
                cwe.SendMail(_rolexConfig.CustomerMailServer, _rolexConfig.CustomerMailPort, _rolexConfig.CustomerMailFrom, _rolexConfig.Recipients);
#if DEBUG
                MessageBox.Show(cwe.FormatError());
#endif
            }
            catch
            {

                try
                {
                    err = (Exception)e;
                    cwe = new CustomerWebException(err.Message, err.StackTrace);
                    cwe.SendMail(_rolexConfig.CustomerMailServer, _rolexConfig.CustomerMailPort, _rolexConfig.CustomerMailFrom,  _rolexConfig.Recipients);
                }
                catch
                {
#if DEBUG
                    MessageBox.Show(cwe.FormatError());
#endif
                }
            }
            if (e is DataAccessDALExcelInvalidDateException)
            {
                MessageBox.Show("Les dates sont invalides sur la cellule  [" + ((DataAccessDALExcelInvalidDateException)e).CellExcel.LineId+ "," + ((DataAccessDALExcelInvalidDateException)e).CellExcel.ColumnId + "]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALExcelPresenceTypeException)
            {
                MessageBox.Show(" Impossible de retrouver le type de présence saisi sur la ligne  [" + ((DataAccessDALExcelPresenceTypeException)e).CellExcel.LineId +  "]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALExcelLocationException)
            {
                MessageBox.Show(" Impossible de retrouver l'emplacement saisi sur la ligne  [" + ((DataAccessDALExcelLocationException)e).CellExcel.LineId +  "]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALExcelSiteException)
            {
                MessageBox.Show(" Impossible de retrouver le Site saisi sur la ligne  [" + ((DataAccessDALExcelSiteException)e).CellExcel.LineId +  "]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALClassificationException)
            {
                MessageBox.Show("Une erreur est survenue lors de la récupération de la nomenclature","Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALExcelFormatException || e is ExcelFormatException)
            {
                MessageBox.Show("Le format du fichier est incorrect. Le format doit être du type 'Rolex_yyyyMM'", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALExcelOpenFileException)
            {
                MessageBox.Show("Impossible d'ouvrir le fichier", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALExcelVisualException)
            {
                MessageBox.Show("Impossible de retrouver le(s) visuel(s) saisi(s) sur la ligne [" + ((DataAccessDALExcelVisualException)e).CellExcel.LineId + "].\n Pour rappel les visuels doivent avoir l'extension JPG et ne peuvent être séparés que par les caractères virgule [,]  ou point virgule [;]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALBadPictureNumberException)
            {
                MessageBox.Show("Impossible de retrouver le(s) visuel(s). Il existe plusieurs visuels avec le meme nom, mais avec des extentions différentes sur la ligne [" + ((DataAccessDALBadPictureNumberException)e).CellExcel.LineId + "].\n Pour rappel les visuels doivent avoir l'extension JPG et ne peuvent être séparés que par les caractères virgule [,]  ou point virgule [;]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALBadPictureNameException)
            {
                MessageBox.Show("Impossible de retrouver le(s) visuel(s) de la ligne [" + ((DataAccessDALBadPictureNameException)e).CellExcel.LineId +  "].\n Pour rappel les visuels doivent avoir l'extension JPG et ne peuvent être séparés que par les caractères virgule [,]  ou point virgule [;]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALInsertException)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement du fichier Excel", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is DataAccessDALRedundantPictureNameException)
            {
                MessageBox.Show("Plusieurs images ne peuvent pas porter le même nom sur la ligne [" + ((DataAccessDALRedundantPictureNameException)e).CellExcel.LineId + "].", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (cwe != null)
                    MessageBox.Show("Une erreur est survenue.\n Veuillez contacter le service Informatique. \n " + cwe.FormatError(), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show("Une erreur est survenue.\n Veuillez contacter le service Informatique. \n ", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        #endregion

        private  void SaveImagette(string imagetteDirectoryName, string sourceFileName)
        {
            using (var image = Image.FromFile(sourceFileName))
            {

                using (var imagette = ResizeImage(image, 350))
                {
                    string destFileName = Path.GetDirectoryName(sourceFileName) + @"\" + imagetteDirectoryName + @"\" +
                        Path.GetFileName(sourceFileName);
                    SaveAsJpeg(imagette, destFileName, 30L);
                }
            }
        }
        private  Image ResizeImage(Image imgPhoto, int WidthMax)
        {

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            float nPercent = 0;


            nPercent = ((float)WidthMax / (float)sourceWidth);



            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            if (destWidth == 0 || destHeight == 0)
                return null;

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, destWidth, destHeight),
                new Rectangle(0, 0, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            grPhoto.Dispose();
            grPhoto = null;

            return bmPhoto;
        }
        private  void SaveAsJpeg(Image img, string fileName, Int64 quality)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            using (EncoderParameters EP = new EncoderParameters(1))
            {
                using (EncoderParameter qualityEncoderParameter = new EncoderParameter(qualityEncoder, quality))
                {
                    EP.Param[0] = qualityEncoderParameter;
                    img.Save(fileName, jgpEncoder, EP);
                }
            }
        }
         private ImageCodecInfo GetEncoder(ImageFormat format) { return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid); }   
    }
}
