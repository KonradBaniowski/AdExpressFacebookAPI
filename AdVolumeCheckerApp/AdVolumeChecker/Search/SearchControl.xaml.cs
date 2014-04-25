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
using AdVolumeChecker.Search.DataBase;
using AdVolumeChecker.Search.ViewModel;
using KMI.AdExpress.AdVolumeChecker.Domain.DataModel;
using cst = KMI.AdExpress.AdVolumeChecker.Domain.Constantes;

namespace AdVolumeChecker.Search {
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl {

        private ClassificationViewModel viewModel;

        // Register the routed event
        public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent("Clicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchControl));
 
        // .NET wrapper
        public event RoutedEventHandler Clicked {
            add { AddHandler(ClickedEvent, value); }
            remove { RemoveHandler(ClickedEvent, value); }
        }
       

        #region Constructor
        public SearchControl() {
            InitializeComponent();

            ClassificationLevel[] classificationLevels = Database.GetClassificationLevels();

            viewModel = new ClassificationViewModel(classificationLevels);
            base.DataContext = viewModel;

        }
        #endregion

        #region Search Text Box Key Down
        void searchTextBox_KeyDown(object sender, KeyEventArgs e) {

            if (searchTextBox.Text.Length > 0 && e.Key == Key.Enter) {
                foreach (ClassificationLevelViewModel c in viewModel.ClassificationLevels)
                    c.RemoveChildren();


                if (advertiserRadioButton.IsChecked.Value)
                    viewModel.SearchCommand.Execute(cst.ClassificationLevelId.Advertiser);
                else if (productRadioButton.IsChecked.Value)
                    viewModel.SearchCommand.Execute(cst.ClassificationLevelId.Product);
            }
        }
        #endregion

        #region Radio Button Checked
        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            // ... Get RadioButton reference.
            var button = sender as RadioButton;

            // ... Display button content as title.
            //button.Title = button.Content.ToString();
        }
        #endregion

        #region Button Click
        private void Button_Click(object sender, RoutedEventArgs e) {

            if (searchTextBox.Text.Length > 0) {
                foreach (ClassificationLevelViewModel c in viewModel.ClassificationLevels)
                    c.RemoveChildren();


                if (advertiserRadioButton.IsChecked.Value)
                    viewModel.SearchCommand.Execute(cst.ClassificationLevelId.Advertiser);
                else if (productRadioButton.IsChecked.Value)
                    viewModel.SearchCommand.Execute(cst.ClassificationLevelId.Product);
            }
        }
        #endregion

        private void Fill_Click(object sender, RoutedEventArgs e) {

            // Raise the routed event "selected"
            RaiseEvent(new RoutedEventArgs(ClickedEvent));

        }

   

    }
}
