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
using System.IO;
using Microsoft.Win32;

namespace Gui.MVVM.View
{
    /// <summary>
    /// Interaction logic for UploadContent.xaml
    /// </summary>
    public partial class UploadContent : UserControl
    {
        public UploadContent()
        {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Word Documents|*.doc|Excel Worksheets|*.xls,*.xlsx|PowerPoint Presentations|*.ppt,*.ppts" +
             "|Office Files|*.doc;*.xls;*.ppt,*.pdf" + 
             "|Web files|*.html,*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                fileName.Text = openFileDialog.FileName;
            }
            else
            {
                fileName.Text = "No file selected";
            }
                
        }
    }
}
