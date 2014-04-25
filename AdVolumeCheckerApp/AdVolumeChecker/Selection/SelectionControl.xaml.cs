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
using AdVolumeChecker.Search.ViewModel;
using MultiSelectionTreeView;

namespace AdVolumeChecker.Selection {
    /// <summary>
    /// Interaction logic for SelectionControl.xaml
    /// </summary>
    public partial class SelectionControl : UserControl {
        public SelectionControl() {
            InitializeComponent();
        }

        private void Remove_Click(object sender, RoutedEventArgs e) {

            string name = string.Empty;

            foreach (MultipleSelectionTreeViewItem item in filterTreeView.Items) {

                List<string> values = new List<string>();

                foreach (var value in item.Items)
                    values.Add(value as string);

                foreach (string value in values) {
                    item.Items.Remove(value);
                }
            }

        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e) {

            foreach (var item in filterTreeView.SelectedItems) {

                string name = item.Header as string;
                ((MultipleSelectionTreeViewItem)filterTreeView.Items[0]).Items.Remove(name);
                ((MultipleSelectionTreeViewItem)filterTreeView.Items[1]).Items.Remove(name);
            }

        }
    }
}
