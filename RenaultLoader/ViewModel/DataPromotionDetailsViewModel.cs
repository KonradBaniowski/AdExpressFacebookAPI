﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
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

namespace RenaultLoader.ViewModel
{
    public class DataPromotionDetailsViewModel : INotifyPropertyChanged {

        #region Variables
        /// <summary>
        /// File Info to load
        /// </summary>
        private FileInfo _fInfo;

        private DateTime _monthToDelete;

        #region Commands
        /// <summary>
        /// Open File Command
        /// </summary>
        private RelayCommand _openCommand;
        /// <summary>
        /// Save command
        /// </summary>
        private RelayCommand _saveCommand;

        private RelayCommand _deleteCommand;
        #endregion

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
        public DateTime MonthToDelete
        {
            get { return _monthToDelete; }
            set
            {
                _monthToDelete = value;
                RaisePropertyChanged("MonthToDelete");
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
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(pstrPropertyName));

        }
        #endregion

        #region Default Constructor
        /// <summary>
        /// 
        /// </summary>
        public DataPromotionDetailsViewModel()
        {
        }
        #endregion

        #region Traitment Command
        /// <summary>
        /// Tratment when open command is execute
        /// </summary>
        private void ExecuteOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { };
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Excel Files (.xls;*xlsx)|*.xls;*xlsx";
            openFileDialog.FilterIndex = 1;
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog().Value)
                CurrentFile = new FileInfo(openFileDialog.FileName);

        }

        /// <summary>
        /// Tratment when save command is execute
        /// </summary>
        private void ExecuteSave() {
            try {
                IDataVeillePromo veillePromo = (IDataVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { ApplicationParameters.DataBaseDescription }, null, null, null);
                DataPromotionDetails dataPromotionDetails = veillePromo.GetDataPromotionDetailList(new FileDataSource(new FileStream(CurrentFile.FullName, FileMode.Open)));

                if (veillePromo.HasData(dataPromotionDetails.DateTraitment)) {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Des données existent en base de données pour cette periode, voulez vous les effacer ?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (messageBoxResult == MessageBoxResult.Cancel)
                        return;
                    if (messageBoxResult == MessageBoxResult.Yes)
                        veillePromo.DeleteData(dataPromotionDetails.DateTraitment, dataPromotionDetails.DateTraitment);
                }

                veillePromo.InsertDataPromotionDetails(dataPromotionDetails);
                MessageBox.Show("Le fichier a été chargé", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e) {
                OnError(e);
            }
        }
        /// Tratment when delete command is execute
        /// </summary>
        private void ExecuteDelete()
        {
            try
            {
                IDataVeillePromo veillePromo = (IDataVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.rules].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { ApplicationParameters.DataBaseDescription }, null, null, null);

                if (veillePromo.HasData(_monthToDelete))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Des données existent en base de données pour cette periode, voulez vous les effacer ?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (messageBoxResult == MessageBoxResult.Cancel)
                        return;
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        veillePromo.DeleteData(_monthToDelete, _monthToDelete);
                        MessageBox.Show("Les données ont été supprimés", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                else  MessageBox.Show("Aucune données en base pour cette période", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }
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

            if (e is VeillePromoExcelException) {
                MessageBox.Show("Le format du fichier est incorrect. Le format doit être du type 'Renault_yyyyMM'", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e is VeillePromoExcelOpenFileException) {
                MessageBox.Show("Impossible d'ouvrir le fichier", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
