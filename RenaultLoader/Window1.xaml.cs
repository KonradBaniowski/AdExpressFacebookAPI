using System;
using System.Collections.Generic;
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
using Microsoft.Win32;

namespace RenaultLoader
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            this.DataContext = new ViewModel.DataPromotionDetailsViewModel();
            InitializeComponent();
            
        }

        //private void btnBrowse_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog fdlg = new OpenFileDialog();
        //    fdlg.Title = "File Upload";
        //    fdlg.Filter = "Xml File|*.xml";
        //    //fdlg.FilterIndex = 2;
        //    fdlg.RestoreDirectory = true;
        //    Nullable<bool> result = fdlg.ShowDialog();
        //    if (result == true)
        //    {
        //        txtFileUpload.Text = fdlg.FileName;
        //    }
        //}
    }
}
