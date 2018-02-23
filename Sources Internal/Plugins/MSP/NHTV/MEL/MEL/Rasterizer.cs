using System;
using System.Collections.Generic;
using ClipperLib;
using System.Drawing;
using System.Drawing.Imaging;

namespace MEL {
	class Rasterizer {
		private const double intConversion = 10000000.0f;
		private const long longintConversion = 10000000;

		/// <summary>
		/// Creates a raster of the weighted input layers. 
		/// Polygon vertices should be ordered depending on the coordinate system:
		///     Bottom left zero: counter clockwise
		///     Top left zero: clockwise
		/// </summary>
		public static double[,] RasterizePolygons(List<Geometry> layers, double weight, double maxWeightPerPoly, int width, int height, Rect rasterBounds) {
            double[,] output = new double[width, height];

			//Offset to the first square (for both axes)
			long voffset = (long)(rasterBounds.yMin * intConversion);
			long hoffset = (long)(rasterBounds.xMin * intConversion);
			long vsquaresize = (long)((rasterBounds.yMax - rasterBounds.yMin) * intConversion)  / height; //The rounding here can create errors for large grids
			long hsquaresize = (long)((rasterBounds.xMax - rasterBounds.xMin) * intConversion) / width;
            double squaresurface = (rasterBounds.yMax - rasterBounds.yMin) * (rasterBounds.xMax - rasterBounds.xMin) / (width * height);

            foreach (Geometry layer in layers) {
				foreach(List<Double[]> polygon in layer.geometry)
                {
					//Convert to int poly
					List<IntPoint> intpoly = VectorToIntPoint(polygon);

					//Get bounding box
					long left, right, top, bot;
					GetBounds(intpoly, out left, out right, out top, out bot);

					//Determine squares within bounding box
					long xmin = (left - hoffset) / hsquaresize;
					long xmax = (right - hoffset) / hsquaresize;
					long ymin = (bot - voffset) / vsquaresize;
					long ymax = (top - voffset) / vsquaresize;

                    //Foreach overlapping square: calculate intersecting area
                    for (long x = xmin < 0 ? 0 : xmin; x <= xmax && x < width; x++)
                    {
                        for (long y = ymin < 0 ? 0 : ymin; y <= ymax && y < height; y++)
                        {
                            Clipper clipper = new Clipper();

                            //Construct polygon paths (of poly and grid square)
                            clipper.AddPaths(new List<List<IntPoint>>() { intpoly }, PolyType.ptSubject, true);
                            clipper.AddPaths(new List<List<IntPoint>>() { GetSquarePoly(x * hsquaresize + hoffset, (x + 1) * hsquaresize + hoffset, 
                                                                                        y * vsquaresize + voffset, (y + 1) * vsquaresize + voffset) }, PolyType.ptClip, true);

                            //Calculate intersection
                            List<List<IntPoint>> intersection = new List<List<IntPoint>>();

                            clipper.Execute(ClipType.ctIntersection, intersection, PolyFillType.pftNonZero);

                            //Calculate part of square that is covered by the intersection and add it to the result
                            if (intersection.Count > 0 && intersection[0].Count > 0)                         
                                output[x, height - 1 - y] += Math.Min((GetPolygonArea(intersection) / squaresurface) * weight, maxWeightPerPoly);
                        }
                    }
				}
			}
			return output;
		}

        /// <summary>
        /// Creates a raster of the weighted input layers, which should contain lines.
        /// </summary>
        public static double[,] RasterizeLines(List<Geometry> layers, double weight, double maxWeightPerLine, int width, int height, Rect rasterBounds)
        {
            double[,] output = new double[width, height];

            //Offset to the first square (for both axes)
            long voffset = (long)(rasterBounds.yMin * intConversion);
            long hoffset = (long)(rasterBounds.xMin * intConversion);
            double vsquaresize = (rasterBounds.yMax - rasterBounds.yMin) / (double)height;
            double hsquaresize = (rasterBounds.xMax - rasterBounds.xMin) / (double)width;
            long vsquaresizelong = (long)(vsquaresize *  intConversion);//The rounding here can create errors for large grids
            long hsquaresizelong = (long)(hsquaresize * intConversion);

            foreach (Geometry layer in layers)
            {
                foreach (List<Double[]> line in layer.geometry)
                {
                    //Convert to int poly
                    List<IntPoint> intpoly = VectorToIntPoint(line);

                    //Get bounding box
                    long left, right, top, bot;
                    GetBounds(intpoly, out left, out right, out top, out bot);

                    //Determine squares within bounding box
                    long xmin = (left - hoffset) / hsquaresizelong;
                    long xmax = (right - hoffset) / hsquaresizelong;
                    long ymin = (bot - voffset) / vsquaresizelong;
                    long ymax = (top - voffset) / vsquaresizelong;

                    //Foreach overlapping square: calculate intersecting area
                    for (long x = xmin < 0 ? 0 : xmin; x <= xmax && x < width; x++)
                    {
                        for (long y = ymin < 0 ? 0 : ymin; y <= ymax && y < height; y++)
                        {
                            Clipper clipper = new Clipper();

                            //Construct polygon paths (of poly and grid square)
                            clipper.AddPaths(new List<List<IntPoint>>() { intpoly }, PolyType.ptSubject, false);
                            clipper.AddPaths(new List<List<IntPoint>>() { GetSquarePoly(x * hsquaresizelong + hoffset, (x + 1) * hsquaresizelong + hoffset,
                                                                                        y * vsquaresizelong + voffset, (y + 1) * vsquaresizelong + voffset) }, PolyType.ptClip, true);

                            //Calculate intersection
                            PolyTree intersection = new PolyTree(); //Requires a polytree because open paths are being clipped

                            clipper.Execute(ClipType.ctIntersection, intersection, PolyFillType.pftNonZero);

                            //Calculate part of square that is covered by the intersection and add it to the result
                            if (intersection.ChildCount > 0 && intersection.GetFirst().Contour.Count > 0)
                                output[x, height - 1 - y] += Math.Min(GetLinesLength(intersection, hsquaresize, vsquaresize) * weight, maxWeightPerLine);
                        }
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Creates a raster of the weighted input layers, which should contain points.
        /// </summary>
        public static double[,] RasterizePoints(List<Geometry> layers, double weight, int width, int height, Rect rasterBounds)
        {
            double[,] output = new double[width, height];

            double vsquaresize = (rasterBounds.yMax - rasterBounds.yMin) / (double)height;
            double hsquaresize = (rasterBounds.xMax - rasterBounds.xMin) / (double)width;

            foreach (Geometry layer in layers)
            {
                foreach (List<Double[]> point in layer.geometry)
                {
                    int x = (int)((point[0][0] - rasterBounds.xMin) / hsquaresize);
                    int y = (int)((point[0][1] - rasterBounds.yMin) / vsquaresize);

                    if(x < width && x >= 0 && y < height && y >= 0)
                        output[x, height - 1 - y] += weight;
                }
            }
            return output;
        }

		public static double[,] PNGToArray(Bitmap bitmap, float influence, int width, int height) {
			double[,] output = new double[width, height];

			//double total = 0;

			for(int x = 0; x < width; x++) {
				for(int y = 0; y < height; y++) {
					Color tmp = bitmap.GetPixel(x, y);

					//genuinely not sure if this is the right way to get the value
					float val = tmp.GetBrightness() * influence;
					if(val > 1)
						val = 1;

					//total += val;

					output[x, y] = val;
				}
			}

			//Console.WriteLine(total);

			return output;
		}

		/// <summary>
		/// Gets the bounds of a polygon. Returned rect is non-rotated.
		/// </summary>
		public static void GetBounds(List<IntPoint> poly, out long left, out long right, out long top, out long bot) {
			left = long.MaxValue;
            right = long.MinValue;
            top = long.MinValue;
            bot = long.MaxValue;

			foreach(IntPoint v in poly) {
				if(v.X > right)
					right = v.X;
				if(v.X < left)
					left = v.X;
				if(v.Y > top)
					top = v.Y;
				if(v.Y < bot)
					bot = v.Y;
			}
		}

        private static List<IntPoint> VectorToIntPoint(List<double[]> points)
        {
            List<IntPoint> verts = new List<IntPoint>();
            for (int i = points.Count - 1; i >= 0; i--)
            {
                verts.Add(new IntPoint(points[i][0] * intConversion, points[i][1] * intConversion ));
            }
            return verts;
        }

		private static List<IntPoint> GetSquarePoly(long xmin, long xmax, long ymin, long ymax) {
			return new List<IntPoint>()
			{
			new IntPoint(xmin, ymin),
			new IntPoint(xmin, ymax),
			new IntPoint(xmax, ymax),
			new IntPoint(xmax, ymin)
		};
		}

		private static double GetPolygonArea(List<List<IntPoint>> polygons) {
			double area = 0;
			foreach(List<IntPoint> polygon in polygons) {
				for(int i = 0; i < polygon.Count; ++i) {
					int j = (i + 1) % polygon.Count;
					area += (double)((polygon[i].Y / longintConversion) * (polygon[j].X / longintConversion) - (polygon[i].X / longintConversion) * (polygon[j].Y / longintConversion));
				}
			}
			return Math.Abs(area * 0.5);
		}

        //private static double GetLinesLength(List<List<IntPoint>> lines, double squareWidth, double squareHeight)
        //{
        //    double length = 0;
        //    foreach (List<IntPoint> line in lines)
        //    {
        //        for (int i = 0; i < line.Count - 1; ++i)
        //        {
        //            double x = (double)((line[i].X - line[i + 1].X) / longintConversion) / squareWidth;
        //            double y = (double)((line[i].Y - line[i + 1].Y) / longintConversion) / squareHeight;
        //            length += Math.Sqrt(x * x + y * y);
        //        }
        //    }
        //    return Math.Abs(length);
        //}

        private static double GetLinesLength(PolyTree polyTree, double squareWidth, double squareHeight)
        {
            double length = 0;
            foreach (PolyNode node in polyTree.Childs)
            {
                List<IntPoint> line = node.Contour;
                for (int i = 0; i < line.Count - 1; ++i)
                {
                    double x = (double)((line[i].X - line[i + 1].X) / longintConversion) / squareWidth;
                    double y = (double)((line[i].Y - line[i + 1].Y) / longintConversion) / squareHeight;
                    length += Math.Sqrt(x * x + y * y);
                }
            }
            return Math.Abs(length);
        }

        public static bool PolygonIsClockwise(List<double[]> polygon)
        {
            double result = 0;
            for (int i = 0; i < polygon.Count; ++i)
            {
                int j = (i + 1) % polygon.Count;
                result += (polygon[j][0] - polygon[i][0]) * (polygon[j][1] + polygon[i][1]);
            }
            return result > 0;
        }

        public static unsafe Bitmap ToBitmap(double[,] rawImage) {
			int width = rawImage.GetLength(0);
			int height = rawImage.GetLength(1);

			Bitmap Image = new Bitmap(width, height);
			BitmapData bitmapData = Image.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite,
				PixelFormat.Format32bppArgb
			);
			ColorARGB* startingPosition = (ColorARGB*)bitmapData.Scan0;


			for(int i = 0; i < width; i++)
				for(int j = 0; j < height; j++) {
					double color = rawImage[i, j];
					byte rgb = (byte)(color * 255f);

					ColorARGB* position = startingPosition + i + (height - j - 1) * width;
					position->A = 255;
					position->R = rgb;
					position->G = rgb;
					position->B = rgb;
				}

			Image.UnlockBits(bitmapData);
			return Image;
		}

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

    public struct ColorARGB {
		public byte B;
		public byte G;
		public byte R;
		public byte A;

		public ColorARGB(Color color) {
			A = color.A;
			R = color.R;
			G = color.G;
			B = color.B;
		}

		public ColorARGB(byte a, byte r, byte g, byte b) {
			A = a;
			R = r;
			G = g;
			B = b;
		}

		public Color ToColor() {
			return Color.FromArgb(A, R, G, B);
		}
	}

	struct Vector2 {
		public float x;
		public float y;

	}

	struct Rect {
		public float x;
		public float y;
		public float xmax;
		public float ymax;

		public float xMin { get { return x; } }
		public float yMin { get { return y; } }
		public float xMax { get { return xmax; } }
		public float yMax { get { return ymax; } }

		public Rect(float x, float y, float xmax, float ymax) {
			this.x = x;
			this.y = y;
			this.xmax = xmax;
			this.ymax = ymax;
		}
	}
}