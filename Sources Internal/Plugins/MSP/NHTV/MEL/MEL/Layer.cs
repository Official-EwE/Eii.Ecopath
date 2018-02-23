using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OSGeo.OGR;
using System.Net;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MEL {
	public class Layer {
		public string name;
		public float influence;
		public bool? construction = false;

		public string wkt;

        public Geometry geometry;

        public Graphics graphics;

		public double[,] rawdata;
		public double total;


		public Layer() { }

		public Layer(string layerid, float influence, bool construction = false) {
			this.name = layerid.ToString();
			this.influence = influence;
			this.construction = construction;
		}

		public Layer(int layerid, float influence, bool fromfile, bool construction = false) {
			this.influence = influence;
			this.name = layerid.ToString();

			this.wkt = LoadFile(layerid);
		}

		public void GetWKT(MEL mel, bool protection = false) {
			//var watch = System.Diagnostics.Stopwatch.StartNew();
			Console.WriteLine("Getting: " + this.name);
			WebClient webClient = new WebClient();

			NameValueCollection values = new NameValueCollection() {
				{"name", this.name }
			};
			
			this.wkt = MEL.HttpGet(webClient, "/api/layer/GeometryExportName", values);

			if(this.wkt != "") {
				this.geometry = JsonConvert.DeserializeObject<Geometry>(this.wkt);

				List<Geometry> g = new List<Geometry>();
                g.Add(this.geometry);

				switch(this.geometry.geotype) {
					case "polygon":
						this.rawdata = Rasterizer.RasterizePolygons(g, this.influence, 1, MEL.x_res, MEL.y_res, new Rect(mel.x_min, mel.y_min, mel.x_max, mel.y_max));
						break;
					case "line":
						this.rawdata = Rasterizer.RasterizeLines(g, this.influence, 1, MEL.x_res, MEL.y_res, new Rect(mel.x_min, mel.y_min, mel.x_max, mel.y_max));
						break;
					case "point":
						this.rawdata = Rasterizer.RasterizePoints(g, this.influence, MEL.x_res, MEL.y_res, new Rect(mel.x_min, mel.y_min, mel.x_max, mel.y_max));
						break;
					case "raster":
						try {
							WebClient client = new WebClient();
							Stream stream = client.OpenRead(MEL.url + "/" + this.geometry.raster);
							Bitmap bitmap = new Bitmap(stream);

							stream.Flush();
							stream.Close();
							client.Dispose();

							this.rawdata = Rasterizer.PNGToArray(bitmap, this.influence, MEL.x_res, MEL.y_res);
							//Console.WriteLine("raster: " + this.name + " loaded");
						}
						catch(Exception e) {
							this.wkt = "";
							Console.WriteLine(this.name + " could not be loaded. Pressure layers will not be generated accurately!");
						}

						break;
				}
			}
			else {
				Console.WriteLine(this.name + " does not exist or does not have geometry");
			}

			//watch.Stop();
			//Console.WriteLine(this.name + " load time: " + watch.ElapsedMilliseconds);
		}

		public string LoadFile(int layerid) {
			return System.IO.File.ReadAllText(@"C:\Users\1002748\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\files\" + layerid.ToString() + ".txt");
		}

		public void CalculateTotal() {

		}
	}

	public class Geometry {
		public string geotype { get; set; }
		public List<List<double[]>> geometry { get; set; }
		public string raster { get; set; }
	}
}