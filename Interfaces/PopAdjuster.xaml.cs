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

namespace Paradox_Editor.Interfaces
{
    /// <summary>
    /// Interaction logic for PopAdjuster.xaml
    /// </summary>
    public partial class PopAdjuster : UserControl
    {
        public PopAdjuster()
        {
            InitializeComponent();
        }

        private void deletePopsCheckbox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void typesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature Not Implemented; Can't Read For POP Types");
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void clearList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cultureBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void religionBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
