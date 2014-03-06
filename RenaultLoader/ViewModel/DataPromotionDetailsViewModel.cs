using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Results;
using System.IO;
using Microsoft.Win32;
using TNS.AdExpressI.VP.Loader.Classification;
using TNS.AdExpressI.VP.Loader.Data;
using TNS.AdExpress.VP.Loader.Domain.Web;
using System.Reflection;
using TNS.FrameWork.DB.Common;
using System.Windows;
using TNS.AdExpressI.VP.Loader.Exceptions;
using TNS.FrameWork.Exceptions;
using RenaultLoader.Exceptions;
using Oracle.DataAccess.Client;

namespace RenaultLoader.ViewModel
{
    public class DataPromotionDetailsViewModel : INotifyPropertyChanged {

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
        #endregion

        #endregion

        #region  Properties
        public RelayCommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(ExecuteOpenFileDialog)); }
        }
        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(ExecuteSave)); }
        }
        public RelayCommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand(ExecuteDelete)); }
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string pstrPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pstrPropertyName));

        }
        #endregion

        #region Default Constructor
        /// <summary>
        /// 
        /// </summary>
        public DataPromotionDetailsViewModel()
        {
            if (ApplicationParameters.CommonApplicationData!=null 
                && ApplicationParameters.CommonApplicationData.PathFile != null
                && ApplicationParameters.CommonApplicationData.PathFile.Exists) {
                CurrentFile = ApplicationParameters.CommonApplicationData.PathFile;
            }
        }
        #endregion

        #region Traitment Command

        #region ExecuteOpenFileDialog
        /// <summary>
        /// Tratment when open command is execute
        /// </summary>
        private void ExecuteOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Excel Files (.xls;*xlsx)|*.xls;*xlsx";
            openFileDialog.FilterIndex = 1;
            openFileDialog.CheckFileExists = true;
            if (CurrentFile != null && CurrentFile.Exists)
                openFileDialog.InitialDirectory = Path.GetDirectoryName(CurrentFile.FullName);

            if (openFileDialog.ShowDialog().Value) {
                CurrentFile = new FileInfo(openFileDialog.FileName);
                ApplicationParameters.CommonApplicationData.PathFile = CurrentFile;
            }

        }
        #endregion

        #region ExecuteSave
        /// <summary>
        /// Tratment when save command is execute
        /// </summary>
        private void ExecuteSave() {

            IDataVeillePromo veillePromo = null;
            try
            {
                IsIndeterminateLoading = true;

                veillePromo = (IDataVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + 
                    ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].AssemblyName
                    , ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null,
                    new object[] { ApplicationParameters.DataBaseDescription }, null, null);
                DataPromotionDetails dataPromotionDetails = veillePromo.GetDataPromotionDetailList(new FileDataSource(new FileStream(CurrentFile.FullName, FileMode.Open)));

                veillePromo.BeginTransaction();
                if (veillePromo.HasData(dataPromotionDetails.DateTraitment, dataPromotionDetails.DateTraitment
                    , Vehicles.names.webPromotion.GetHashCode()))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Des données existent en base de données pour cette periode, voulez vous les effacer ?", ""
                        , MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (messageBoxResult == MessageBoxResult.Cancel)
                        return;
                    if (messageBoxResult == MessageBoxResult.Yes)
                        veillePromo.DeleteData(dataPromotionDetails.DateTraitment, dataPromotionDetails.DateTraitment, Vehicles.names.webPromotion.GetHashCode());
                }

                veillePromo.InsertDataPromotionDetails(dataPromotionDetails);

                veillePromo.CommitTransaction();
                MessageBox.Show("Les données ont été enregistrées ", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                if (veillePromo != null) veillePromo.RollbackTransaction();
                OnError(e);
            }
            finally
            {
                IsIndeterminateLoading = false;
            }
        }
        #endregion

        #region ExecuteDelete
        /// Tratment when delete command is execute
        /// </summary>
        private void ExecuteDelete()
        {
            try
            {
                IsIndeterminateLoading = true;
                var veillePromo = (IDataVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory 
                    + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].AssemblyName,
                    ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, new object[] { ApplicationParameters.DataBaseDescription }, null, null);

                if (veillePromo.HasData(_periodBeginningToDelete, _periodEndToDelete,Vehicles.names.webPromotion.GetHashCode()))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Des données existent en base de données pour cette periode, voulez vous les effacer ?", ""
                        , MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (messageBoxResult == MessageBoxResult.Cancel)
                        return;
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {

                        veillePromo.DeleteData(_periodBeginningToDelete, _periodEndToDelete, Vehicles.names.webPromotion.GetHashCode());
                        MessageBox.Show("Les données ont été supprimés", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                else MessageBox.Show("Aucune données en base pour cette période", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
        #endregion

        #endregion

        #region Gestion des erreurs
        /// <summary>
        /// Evènement d'erreur
        /// </summary>
        /// <param name="e">Argument</param>
        protected void OnError(Exception e) {
            Exception err = null;
            CustomerWebException cwe = null;
            try {
                err = (BaseException)e;
                cwe = new CustomerWebException(err.Message, ((BaseException)err).GetHtmlDetail());
                cwe.SendMail();
#if DEBUG
                MessageBox.Show(cwe.FormatError());
#endif
            }
            catch {

                try {
                    err = (Exception)e;
                    cwe = new CustomerWebException(err.Message, err.StackTrace);
                    cwe.SendMail();
                }
                catch {
#if DEBUG
                    MessageBox.Show(cwe.FormatError());
#endif
                }
            }
            if (e is VeillePromoExcelInvalidDateException)
            {
                MessageBox.Show("Les dates sont invalides sur la cellule  [" + ((VeillePromoExcelInvalidDateException)e).CellExcel.LineId + ","
                    + ((VeillePromoExcelInvalidDateException)e).CellExcel.ColumnId + "]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoExcelCellException)
            {
                MessageBox.Show("Une erreur est survenue sur la cellule  [" + ((VeillePromoExcelCellException)e).CellExcel.LineId + "," 
                    + ((VeillePromoExcelCellException)e).CellExcel.ColumnId + "]", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoExcelException) {
                MessageBox.Show("Le format du fichier est incorrect. Le format doit être du type 'Renault_yyyyMM'", "Erreur"
                    , MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoExcelOpenFileException) {
                MessageBox.Show("Impossible d'ouvrir le fichier", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoExcelVisualException) {
                MessageBox.Show("Une erreur est survenue lors du chargement du fichier Excel: Impossible de retrouver un fichier visuel", "Erreur"
                    , MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoIncorrectPictureFileNameNumberException) {
                MessageBox.Show("Impossible de retrouver le fichier image. Il existe plusieurs fichiers avec le meme nom, mais avec des extentions différentes", "Erreur"
                    , MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoIncorrectPictureFileNameException) {
                MessageBox.Show("Impossible de retrouver le fichier image", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoInsertDbException) {
                MessageBox.Show("Une erreur est survenue lors du chargement du fichier Excel", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else {
                MessageBox.Show("Une erreur est survenue", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }
        #endregion      

    }
}
