using System.Windows;
using System.Windows.Controls;

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
        public void Update()
        {
            foreach (var continent in ModData.MAP_DATA.GetContinents())
            {
                continentsComboBox.Items.Add(continent.Key);
            }
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
