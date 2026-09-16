using Paradox_Editor.Parsers;
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
            cultureBox.Items.Clear();
            continentsComboBox.Items.Clear();
            continentsComboBox.Items.Add("all continents");
			foreach (var continent in ModData.MAP_DATA.GetContinents())
            {
                continentsComboBox.Items.Add(continent.Key);
            }
            foreach (var culture in ModData.CULTURES_DATA.GetCultures())
            {
				cultureBox.Items.Add(culture);
                cultureSpecifier.Items.Add(culture);
			}
        }

        private void deletePopsCheckbox_Checked(object sender, RoutedEventArgs e)
        {

        }

		private void addToListButton_Click(object sender, RoutedEventArgs e)
		{
            if (continentsComboBox.SelectedIndex != -1)
            {
                if (continentsComboBox.SelectedIndex == 0)
                {
                    //If ALL Continents are selected
                } else
                {
                    ModData.MAP_DATA.GetContinents().TryGetValue(continentsComboBox.Text, out Continent continent);
                    foreach (var province in continent.Provinces) //Temporary; for testing
                    {
                        //Already have a list of province ID's. Now just need to access the POP files.
                    }
                    
                    //If only one continent is selected
                }
            }
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

		private void cultureBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

        }

	}
}
