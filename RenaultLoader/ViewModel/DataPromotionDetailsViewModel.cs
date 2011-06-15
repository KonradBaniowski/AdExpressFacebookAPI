using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TNS.AdExpress.Domain.Results;
using System.IO;
using Microsoft.Win32;

namespace RenaultLoader.ViewModel
{
    public class DataPromotionDetailsViewModel : INotifyPropertyChanged
    {
     

        private FileInfo _fInfo;

        private RelayCommand _openCommand;
        private RelayCommand _saveCommand;


        #region Commands

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
        private void ExecuteSave()
        {
            //
        }
       
    }
}
