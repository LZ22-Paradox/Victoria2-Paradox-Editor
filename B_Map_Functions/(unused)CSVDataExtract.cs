using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.B_Map_Functions
{
    class CSVDataExtract
    {

        /*                List<string> CSVProvince = new List<string>();
                List<string[]> CSVRGB = new List<string[]>();
                List<string> CSVProvinceName = new List<string>();

                var CSVFile = vic2DefinitionCSVFile[0]; //Reads each entry from the filepath (the CSV File) as a part of an array
                using (var reader = new StreamReader(CSVFile)) //THIS FUNCTION GETS ALL PROVINCE CSV DATA (INCLUDING RGB)
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine(); //Current line of the CSV being read
                        var values = line.Split(';'); //Current values of said-line

                        CSVProvince.Add(values[0]); //Adding the province ID of current line to the Province ID List
                        string[] strArr = { values[1] + " " + " " + values[2] + " " + values[3] }; //Adding RGB codes
                        CSVProvinceName.Add(values[4]); //Adding province name of current line to Province Name List

                        CSVRGB.Add(strArr); //Adds the RGB Array Values from current line into the Province RGB List
                        var strings = CSVRGB[0].Cast<string>().ToArray(); //Sets array values as strings
                        Debug.WriteLine(strings); //Get RGB values for each pass
                    }
                } //May be overshadowed later by methods implimented in DrawMapColors*/
    }
}
