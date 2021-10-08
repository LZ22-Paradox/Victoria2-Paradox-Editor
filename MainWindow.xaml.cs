using System;
using System.Diagnostics;
using System.Windows;
using static Paradox_Editor.ProgramProperties;
using System.Windows.Input;
using Paradox_Editor.C_Window_Functions;

//F1 to see WIKI detail on part
//F12 to see mechanicla usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private MapNavigation Navigator;
        private FolderSelect SelectMap;
        //private MapEditor UpdateTest;
        public MainWindow()
        {
            InitializeComponent();

            Navigator =
                new MapNavigation(mapCanvas)
                .AddImage(mapBackground)
                .AddImage(testimage1)
                .AddImage(testimage2);

            SelectMap = new FolderSelect(mapCanvas, mapBackground);

            DataContext = SelectMap;
        }

        private void MainWindow_Load(object _1, EventArgs _2)
        {
            MapModesControl.CurrentMapMode = 1;
            MapModesControl.updateMapMode();
        }

        private void Testbox_TextChanged(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                MapEditor.UpdatePoliticalMap();
            }
        }

        public void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Navigator.MouseLeftButtonUp(sender, e);
        }

        public void map_MouseLeave(object sender, MouseEventArgs e)
        {
            Navigator.MouseLeave(sender, e);
        }

        public void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Navigator.MouseLeftButtonDown(sender, e);
        }

        public void map_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Navigator.MouseWheel(sender, e);
        }

        public void map_MouseMove(object sender, MouseEventArgs e)
        {
            Navigator.MouseMove(sender, e);
        }

        public void SelectMasterFolder_Click(object sender, EventArgs e)
        {
            SelectMap.SelectMainFolder(sender, e);
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            SelectMap.Click_Specific_Entry(sender, e);
        }

        public static explicit operator MainWindow(WindowCollection v)
        {
            throw new NotImplementedException();
        }

    }
}
