using System.Drawing;

namespace MEL {
	/// <summary>
	/// Rasterizes input data such as geometry onto a raster.
	/// </summary>
	class Rasterizer {

        public static Bitmap ToBitmapSlow(double[,] rawImage)
        {
            int width = rawImage.GetLength(0);
            int height = rawImage.GetLength(1);

            Bitmap newimage = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int val = Math.Min(255, (int)(rawImage[i, j] * 255f));

					//newimage.SetPixel(i, height-j-1, Color.FromArgb(val, val, val));
					newimage.SetPixel(i, j, Color.FromArgb(val, val, val));
				}
            }

            return newimage;
        }
    }
}