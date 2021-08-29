using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gui.MVVM.View
{
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : UserControl
    {
        public Homepage()
        {
            //From the App.xaml
            InitializeComponent();
            bindListBox();
        }

        private readonly string[] result = { "Stanley","Stans","Osy","Femi","Christian", "Stanley", "Stans", "Osy", "Femi", "Christian", "Stanley", "Stans", "Osy", "Femi", "Christian", "Stanley", "Stans", "Osy", "Femi", "Christian", "Stanley", "Stans", "Osy", "Femi", "Christian", "Stanley", "Stans", "Osy", "Femi", "Christian" };

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Console.Write(e);
        }

        //Bind result array to SearchResults
        private void bindListBox()
        {
            SearchResults.ItemsSource = result;
        }

        private void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(SearchResults.SelectedItem.ToString(), "Search Results", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        }
    }
}
