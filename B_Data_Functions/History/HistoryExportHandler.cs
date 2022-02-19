using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Paradox_Editor.B_Data_Functions
{
    public class HistoryExportHandler
    {

        /*        public string Path { get; set; }
                public HistoryExportHandler(string path)
                {
                    Path = path;
                }*/

        public MainWindow MainWindow { get; set; } = (MainWindow)Application.Current.MainWindow;
        public HistoryFile GetInterfaceEntries()
        {
            var current_Interface = new HistoryFile();

            var ownerList = new List<string>();
            var controllerList = new List<string>();
            var coreList = new List<string>();
            var tradeGoodList = new List<string>();
            var lifeRatingList = new List<string>();
            var terrainList = new List<string>();
            var colonialList = new List<string>();

            var stateBuildingList = new List<StateBuilding>();
            var navalBaseList = new List<string>();
            var fortList = new List<string>();
            var railRoadList = new List<string>();

            ownerList.Add(MainWindow.FileInterface.OWNERBOX.Text);
            controllerList.Add(MainWindow.FileInterface.CONTROLLERBOX.Text);
            foreach (CoreData item in MainWindow.FileInterface.COREGRID.Items)
            {
                if (item.TAG is not "" or null)
                {
                    coreList.Add(Convert.ToString(item.TAG));
                }
            }
            tradeGoodList.Add(MainWindow.FileInterface.TRADEGOODBOX.Text);
            lifeRatingList.Add(MainWindow.FileInterface.LIFERATINGBOX.Text);
            terrainList.Add(MainWindow.FileInterface.TERRAINBOX.Text);
            colonialList.Add(MainWindow.FileInterface.COLONIALBOX.Text);
            fortList.Add(MainWindow.FileInterface.FORTBOX.Text);
            railRoadList.Add(MainWindow.FileInterface.RAILROADBOX.Text);

            //stateBuildingList = new List<string>();
            ///Above is Unused. See HistoryFile.cs & Todo.txt
            navalBaseList.Add(MainWindow.FileInterface.NAVALBASEBOX.Text);
            
            return current_Interface = new HistoryFile()
            {
                Owner = ownerList,
                Controller = controllerList,
                Core = coreList,
                TradeGoods = tradeGoodList,
                LifeRating = lifeRatingList,
                Terrain = terrainList,
                State_Building = stateBuildingList,
                Colonial = colonialList,
                Naval_Base = navalBaseList,
                Fort = fortList,
                Railroad = railRoadList
            };
        }
        

        public List<string> ExportEntries(HistoryFile current_Interface)
        {
            var fileBuild = new List<string>();

            if (!current_Interface.Owner[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("owner = " + current_Interface.Owner[0]);
            }
            if (!current_Interface.Controller[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("controller = " + current_Interface.Controller[0]);
            }
            foreach (var entry in current_Interface.Core)
            {
                if (!entry.IsEmptyOrWhiteSpace())
                {
                    fileBuild.Add("add_core = " + entry);
                }
            }
            if (!current_Interface.TradeGoods[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("trade_goods = " + current_Interface.TradeGoods[0]);
            }
            if (!current_Interface.LifeRating[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("life_rating = " + current_Interface.LifeRating[0]);
            }

            if (!current_Interface.Terrain[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("terrain = " + current_Interface.Terrain[0]);
            }
            if (!current_Interface.Fort[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("fort = " + current_Interface.Fort[0]);
            }

            if (!current_Interface.Railroad[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("railroad = " + current_Interface.Railroad[0]);
            }

            if (!current_Interface.Naval_Base[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("naval_base = " + current_Interface.Naval_Base[0]);
            }


            //add state_building list




            if (!current_Interface.Colonial[0].IsEmptyOrWhiteSpace())
            {
                fileBuild.Add("colonial = " + current_Interface.Colonial[0]);
            }


            return fileBuild;

        }

    }
}
