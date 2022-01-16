using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Data;

namespace Paradox_Editor.C_Window_Functions
{

    public class HistoryfileInterface
    {

        private MainWindow MainWindow;
        private Color PixelColor;
        private Dictionary<uint, string> ColorToID;
        private ProvinceOutputData ProvIDToData;
        private Dictionary<string, string> TAGToName;


        public HistoryfileInterface(MainWindow mainWindow, Color pixelColor, Dictionary<uint, string> storedColorToID, ProvinceOutputData storedProvinceIDToData, Dictionary<string, string> storedTagToCountryName)
        {
            PixelColor = pixelColor;
            ColorToID = storedColorToID;
            ProvIDToData = storedProvinceIDToData;
            TAGToName = storedTagToCountryName;
            MainWindow = mainWindow;
        }

        //POTENTIALLY CHANGE TO DATAGRID
        public void PutTAGDataIntoInferface()
        {

            if (ColorToID.TryGetValue((uint)PixelColor.ToArgb(), out var ProvinceID))
            {
                MainWindow.FileInterface.PROVIDBOX.Text = ProvinceID;

                if (ProvIDToData.IDToName.TryGetValue(ProvinceID, out var ProvinceName))
                ///THE PROVINCE NAME IS READ AS THE NAME OF THE FILE ; CHANGE TO READ CSV PROPER NAME
                /// IDToName is the fault of it
                {
                    MainWindow.FileInterface.NAMEBOX.Text = Convert.ToString(ProvinceName);
                }
                if (ProvIDToData.IDToOwner.TryGetValue(ProvinceID, out var ownerTAG))
                {
                    MainWindow.FileInterface.OWNERBOX.Text = Convert.ToString(ownerTAG);
                }
                if (ProvIDToData.IDToController.TryGetValue(ProvinceID, out var controllerTAG))
                {
                    MainWindow.FileInterface.CONTROLLERBOX.Text = Convert.ToString(controllerTAG);
                }

                if (ProvIDToData.IDToCores.TryGetValue(ProvinceID, out var ProvinceCores))
                {

                    var outputCores = ProvinceCores;
                }


                MainWindow.FileInterface.COLORRGB.Text = Convert.ToString(PixelColor.R + "," + PixelColor.G + "," + PixelColor.B);

            }
        }

        public List<string> GetCoreList()
        {

            if (ColorToID.TryGetValue((uint)PixelColor.ToArgb(), out var ProvinceID))
            {
                if (ProvIDToData.IDToCores.TryGetValue(ProvinceID, out var ProvinceCores))
                {
                    var outputCores = ProvinceCores;
                    return outputCores;
                }
                
            }
            return null;
        }

        public void PutOtherDataIntoInferface()
        //need to make into a 2-way observable collection that correlates w. the rest of the data
        {
            if (ColorToID.TryGetValue((uint)PixelColor.ToArgb(), out var ProvinceID))
            {
                if (ProvIDToData.IDToHistory.TryGetValue(ProvinceID, out var historyData))
                {
                    if (historyData.TradeGoods.Count > 0)
                    {
                        MainWindow.FileInterface.TRADEGOODBOX.Text = Convert.ToString(historyData.TradeGoods[0]);
                    }
                    else
                    {
                        MainWindow.FileInterface.TRADEGOODBOX.Text = null;
                    }
                    if (historyData.LifeRating.Count > 0)
                    {
                        MainWindow.FileInterface.LIFERATINGBOX.Text = Convert.ToString(historyData.LifeRating[0]);
                    }
                    else
                    {
                        MainWindow.FileInterface.LIFERATINGBOX.Text = null;
                    }
                    if (historyData.Colonial.Count > 0)
                    {
                        MainWindow.FileInterface.COLONIALBOX.Text = Convert.ToString(historyData.Colonial[0]);
                    }
                    else
                    {
                        MainWindow.FileInterface.COLONIALBOX.Text = null;
                    }
                    if (historyData.Naval_Base.Count > 0)
                    {
                        MainWindow.FileInterface.NAVALBASEBOX.Text = Convert.ToString(historyData.Naval_Base[0]);
                    }
                    else
                    {
                        MainWindow.FileInterface.NAVALBASEBOX.Text = null;
                    }
                    if (historyData.Terrain.Count > 0)
                    {
                        MainWindow.FileInterface.TERRAINBOX.Text = Convert.ToString(historyData.Terrain[0]);
                    }
                    else
                    {
                        MainWindow.FileInterface.TERRAINBOX.Text = null;
                    }
                    if (historyData.Fort.Count > 0)
                    {
                        MainWindow.FileInterface.FORTBOX.Text = Convert.ToString(historyData.Fort[0]);
                    }
                    else
                    {
                        MainWindow.FileInterface.FORTBOX.Text = null;
                    }
                    if (historyData.Railroad.Count > 0)
                    {
                        MainWindow.FileInterface.RAILROADBOX.Text = Convert.ToString(historyData.Railroad[0]);
                    }
                    else
                    {
                        MainWindow.FileInterface.RAILROADBOX.Text = null;
                    }

                    //Add State_Building stuff here

                }
            }
        }

    }
}
