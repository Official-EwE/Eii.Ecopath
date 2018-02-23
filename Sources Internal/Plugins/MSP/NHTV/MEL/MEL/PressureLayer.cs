using System;
using System.Collections.Generic;

using System.IO;
using EwEShell;
using System.Drawing;

namespace MEL {
	public class PressureLayer {
		public List<Layer> layers = new List<Layer>();

		public string name;
		public string layername;
		private string filepath;

		public bool redraw = true;

		public cPressure pressure;

		public double[,] rawdata { get; private set; }

		public PressureLayer(string name) {
			this.name = name;
			this.layername = MEL.ConvertLayerName(this.name);
			this.filepath = MEL.OUTPUTDIR + this.layername + ".tif";
		}

		/// <summary>
		/// add a layer to the pressure
		/// </summary>
		public void Add(Layer layer) {
			if(layer.wkt == "Invalid layer name, are you sure it's written correctly?") {
				return;
			}

			this.layers.Add(layer);
		}

		public void RasterizeLayers(MEL mel) {
			if(this.redraw) {
				//double total = 0f;

				this.rawdata = new double[MEL.x_res, MEL.y_res];
				Console.WriteLine("rasterizing " + this.name);

				this.redraw = false;

				try {
					foreach(Layer layer in this.layers) {
						if(layer == null) {
							Console.WriteLine("null layer");
							continue;
						}

						if(layer.wkt == "") continue;

						for(int i = 0; i < MEL.x_res; i++) {
							for(int j = 0; j < MEL.y_res; j++) {
								this.rawdata[i, j] += layer.rawdata[i, j];

								if(this.rawdata[i, j] > 1) {
									this.rawdata[i, j] = 1;
								}

								//total += this.rawdata[i, j];
							}
						}
					}
				}
				catch(Exception e) {
					Console.WriteLine(e);
				}

				//Console.WriteLine(this.name + " : " + total.ToString());

				Bitmap bitmap = Rasterizer.ToBitmapSlow(this.rawdata);
				bitmap.Save(this.filepath);

				//set the data to be sent to EwE
				this.pressure = new cPressure(this.name, MEL.x_res, MEL.y_res, this.rawdata);

				MEL.HttpGet(new System.Net.WebClient(), "/api/mel/UpdateLayer/" + this.layername);
			}
		}

	}
}
