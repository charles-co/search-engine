using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Engine;

namespace Gui.MVVM.View {
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : UserControl {
        public Homepage() {
            //From the App.xaml
            InitializeComponent();
        }
        
        private BaseDocument[] _documents;
        
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_OnSearch(sender, e);
            }
        }
        
        private async void Button_OnSearch(object sender, RoutedEventArgs routedEventArgs) {
            var start = DateTime.Now;
            var querier = new Querier();
            _documents = await querier.Search(SearchInput.Text);
            var seconds = (DateTime.Now - start).TotalMilliseconds;
            
            SearchResults.Children.Clear();

            var endText = _documents.Length != 1 ? "s" : "";
            NumberOfResults.Text = $"{_documents.Length} Result{endText}";

            if (_documents.Length > 0) {

                ResponseTime.Text = $"Response time: {seconds}ms";

                foreach (var document in _documents) {
                    TextBlock tb = new TextBlock();
                    tb.Style = Resources["DownloadLinkWrapper"] as Style;

                    Hyperlink hyperlink = new Hyperlink();
                    Run run = new Run();

                    run.Text = document.name;
                    hyperlink.NavigateUri = new Uri(document.url);
                    hyperlink.Style = Resources["DownloadLink"] as Style;
                    hyperlink.Inlines.Add(run);

                    hyperlink.RequestNavigate += (_, e) => { System.Diagnostics.Process.Start(e.Uri.ToString()); };

                    tb.Inlines.Add(hyperlink);
                    SearchResults.Children.Add(tb);
                }
            }
            else {
                TextBlock tb = new TextBlock();
                tb.Style = Resources["DownloadLinkWrapper"] as Style;
                tb.Text = $"No Items match your search query: {SearchInput.Text}";
                SearchResults.Children.Add(tb);
            }
        }
    }
}