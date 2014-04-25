using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdVolumeChecker.Search.ViewModel;
using KMI.AdExpress.AdVolumeChecker;
using KMI.AdExpress.AdVolumeChecker.Domain;
using Microsoft.Win32;
using MultiSelectionTreeView;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Net.Mail;

namespace AdVolumeChecker {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private BackgroundWorker _backgroundWorker = new BackgroundWorker();

        #region Variables
        /// <summary>
        /// Mail qui envoie le log
        /// </summary>
        private SmtpUtilities _errMail;
        /// <summary>
        /// Version List
        /// </summary>
        private string _versionList = string.Empty;
        /// <summary>
        /// Advertiser List
        /// </summary>
        private string _advertiserList = string.Empty;
        /// <summary>
        /// Product List
        /// </summary>
        private string _productList = string.Empty;
        /// <summary>
        /// Inclusion or Exclusion
        /// </summary>
        private bool _isIn = false;
        /// <summary>
        /// Excel path
        /// </summary>
        private string _path = string.Empty;
        /// <summary>
        /// Exit Code
        /// </summary>
        private int _exitCode = 0;
        /// <summary>
        /// Period selected
        /// </summary>
        private string _period = string.Empty;
        #endregion

        #region Main Window
        /// <summary>
        /// Main Window
        /// </summary>
        public MainWindow() {
            InitializeComponent();

            DateTime startDate;
            DateTime endDate;
            string configurationPathDirecory = AppDomain.CurrentDomain.BaseDirectory + Constantes.Application.APPLICATION_CONFIGURATION_DIRECTORY + @"\";
            DataBaseInformation.Init((new XmlReaderDataSource(configurationPathDirecory + Constantes.Application.DATABASE_CONFIGURATION_FILE)));
            FilterInformations.Init((new XmlReaderDataSource(configurationPathDirecory + Constantes.Application.FILTER_CONFIGURATION_FILE)));
            _errMail = new SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory + @"\Configuration\Mail.xml");
            searchControl.Clicked += searchControl_Clicked;
            //Configure the ProgressBar
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            this._backgroundWorker.WorkerReportsProgress = true;
            this._backgroundWorker.WorkerSupportsCancellation = true;
            this._backgroundWorker.DoWork += new DoWorkEventHandler(bw_DoWork);
            this._backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            this._backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;

            startDate = DateTime.Now.AddDays(-7).StartOfWeek(DayOfWeek.Monday);
            endDate = startDate.AddDays(6);

            for (int i = 0; i < 5; i++) {
                periodList.Items.Add("Du " + startDate.ToString("dd/MM/yyyy") + " au " + endDate.ToString("dd/MM/yyyy"));
                startDate = startDate.AddDays(-7);
                endDate = startDate.AddDays(6);
            }
            periodList.SelectedIndex = 0;

        }

        
        #endregion

        #region SearchControl Clicked
        /// <summary>
        /// SearchControl Clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed Event Args</param>
        void searchControl_Clicked(object sender, RoutedEventArgs e) {

            foreach (var item in searchControl.classificationTreeView.SelectedItems) {
                ClassificationLevelItemViewModel cli = item.Header as ClassificationLevelItemViewModel;
                if (cli != null) {
                    if (searchControl.advertiserRadioButton.IsChecked.Value) {
                        if (!((MultipleSelectionTreeViewItem)selectioncontrol.filterTreeView.Items[0]).Items.Contains(cli.ClassificationLevelItemName))
                            ((MultipleSelectionTreeViewItem)selectioncontrol.filterTreeView.Items[0]).Items.Add(cli.ClassificationLevelItemName);
                        ((MultipleSelectionTreeViewItem)selectioncontrol.filterTreeView.Items[0]).IsExpanded = true;
                    }
                    else if (searchControl.productRadioButton.IsChecked.Value) {
                        if (!((MultipleSelectionTreeViewItem)selectioncontrol.filterTreeView.Items[1]).Items.Contains(cli.ClassificationLevelItemName))
                            ((MultipleSelectionTreeViewItem)selectioncontrol.filterTreeView.Items[1]).Items.Add(cli.ClassificationLevelItemName);
                        ((MultipleSelectionTreeViewItem)selectioncontrol.filterTreeView.Items[1]).IsExpanded = true;
                    }
                }
            }

        }
        #endregion

        #region Generate Excel Click
        /// <summary>
        /// Generate Excel Click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed Event Args</param>
        private void Generate_Excel_Click(object sender, RoutedEventArgs e) {

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "KantarMedia - CSA - Volumes publicitaires.xlsx";
            dialog.Filter = "excel files (*.xlsx)|*.xls|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true) {

                _advertiserList = string.Empty;
                _productList = string.Empty;
                _versionList = string.Empty;
                _path = dialog.FileName;
                bool advertiser = true;

                _versionList = versionList.Text;

                foreach (MultipleSelectionTreeViewItem item in selectioncontrol.filterTreeView.Items) {

                    foreach (var value in item.Items)
                        if (advertiser)
                            _advertiserList += ExtractId(value.ToString()) + ",";
                        else
                            _productList += ExtractId(value.ToString()) + ",";

                    if (advertiser) {
                        if (_advertiserList.Length > 0)
                            _advertiserList = _advertiserList.Substring(0, _advertiserList.Length - 1);
                    }
                    else {
                        if (_productList.Length > 0)
                            _productList = _productList.Substring(0, _productList.Length - 1);
                    }

                    advertiser = false;
                }

                _period = periodList.SelectedItem as string;

                //if (selectioncontrol.inclusionRadioButton.IsChecked == true)
                //    _isIn = true;

                searchControl.IsEnabled = false;
                selectioncontrol.IsEnabled = false;
                versionList.IsEnabled = false;
                generateExcel.IsEnabled = false;

                this._backgroundWorker.RunWorkerAsync();
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {

            // Set the Value porperty when porgress changed.
            progressBar.Value = (double)e.ProgressPercentage;

        }

        void bw_DoWork(object sender, DoWorkEventArgs e) {

            try {
                BackgroundWorker _worker = sender as BackgroundWorker;

                if (_worker != null) {

                    AdVolumeCheckerShell shell = new AdVolumeCheckerShell(_errMail);
                    shell.Message += shell_Message;
                    shell.DoTask(_versionList, _advertiserList, _productList, _isIn, _path, _period);
                }
            }
            catch (Exception ex) {
                _errMail.SendWithoutThread("Ad Volume Checker Error", "<html><body><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">An error occurred during Ad Volume Checker treatment task.</font><br>" + ex.Message + "</body></html>", true, false);
                MessageBox.Show("Une erreur est survenue au moment de la génération du fichier Excel", "Erreur");
                _exitCode = 110;
                
            }
        }

        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

            if(_exitCode == 110)
                Application.Current.Shutdown(_exitCode);
            searchControl.IsEnabled = true;
            selectioncontrol.IsEnabled = true;
            versionList.IsEnabled = true;
            generateExcel.IsEnabled = true;
            progressBar.Value = 0;
            MessageBox.Show("Le fichier a été généré correctement", "Information");
        }

        void shell_Message(int progress) {
            _backgroundWorker.ReportProgress(progress);
        }
        #endregion

        #region Extract Id
        /// <summary>
        /// Extract Id
        /// </summary>
        /// <param name="s">String</param>
        /// <returns></returns>
        private string ExtractId(string s) {

            int i = s.IndexOf("(");

            return s.Substring(i+1).Replace(")","");

        }
        #endregion

    }

    #region Extension Method
    /// <summary>
    /// Get first and last day of week
    /// </summary>
    public static class DateTimeExtensions {
        /// <summary>
        /// Get dateTime of the day of week passed in parameter
        /// </summary>
        /// <param name="dt">Date Time</param>
        /// <param name="startOfWeek">Day Of Week</param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek) {

            int diff = dt.DayOfWeek - startOfWeek;

            if (diff < 0) {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
    }
    #endregion

}
