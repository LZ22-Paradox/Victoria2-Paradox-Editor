using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Paradox_Editor.B_Data_Functions
{
    public class _tempBorders
    {
        public string Id { get; set; }


        //Currently for edge detection || SLOWER THAN LOCKING BITMAPS || TOO COMPLICATED TO USE
        //(tebeco#0205 or Cif3#8372 Can assist)
        void d1() 
        {

            Bitmap input = new Bitmap("TestProvince.png");
            Bitmap output = new Bitmap(input);

            for (int x = 1; x < input.Width - 1; x++)
            {
                for (int y = 1; y < input.Height - 1; y++)
                {
                    Color x1 = input.GetPixel(x + 1, y);
                    Color y1 = input.GetPixel(x, y + 1);

                    Color xn1 = input.GetPixel(x - 1, y);
                    Color yn1 = input.GetPixel(x, y - 1);

                    Color currentPixel = input.GetPixel(x, y);

                    if (currentPixel != x1 ||
                        currentPixel != y1)
                    {
                        output.SetPixel(x, y, Color.Black);
                    }

                    if (currentPixel != xn1 ||
                        currentPixel != yn1)
                    {
                        output.SetPixel(x, y, Color.Black);
                    }
                }
            }

            output.Save("Outlined.png");

        }
    }


}
