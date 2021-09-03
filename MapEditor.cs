using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Paradox_Editor.ProgramProperties;

public static class MapEditor
{

    public static ProvinceFile TestProvinceData { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath, colour


    private static void MainWindow_Load(object _1, EventArgs _2)
    {
    }
    //Above is currently useless: Use for graphical loading of map a later point.

    public static void TestProvinceUpdate() //The test process for how map loading should work.
    {

        List<string> insertedIDs = new List<string>();
        List<string[]> insertedRGBs = new List<string[]>();
        List<string> insertedPaths = new List<string>();
        List<string> insertedNames = new List<string>();

        var Paths = new string[] { "Path 1", "Path 2", "Path 3", "Path 4", "Path 5", "Path 6" };
        var TestCSVFile = new string[] { "1;255;0;0;Red", "2;38;0;255;Blue", "3;118;255;0;Green", "4;250;0;255;Purple", "5;0;242;255;Cyan", "6;250;255;0;Yellow" }; //Reads each entry from the filepath (the CSV File) as a part of an array

        foreach (var entry in TestCSVFile)
        {
            var values = entry.Split(';'); //Current values of said-line

            insertedIDs.Add(values[0]);
            string[] strArr = { values[1] + " " + " " + values[2] + " " + values[3] }; //Adding RGB codes
            insertedNames.Add(values[4]); //Adding province name of current line to Province Name List
            insertedRGBs.Add(strArr); //Adds the RGB Array Values from current line into the Province RGB List

        }


        //\\

    }

}