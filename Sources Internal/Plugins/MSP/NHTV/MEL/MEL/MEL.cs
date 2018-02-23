using EwEShell;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace MEL
{
    /// <summary>
    /// MEL connects the MSP api with EwE, allowing for a ecological simulation inside MSP
    /// </summary>
    /// 
    public class MEL {

		public const int TICKRATE = 100;   //in ms

		public static string url=  "http://localhost/dev";
		//public static string url = "https://msp.guraas.com";

		public static string OUTPUTDIR = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../raster/";

		public int lastupdatedmonth = -1;

		//cache layers
		public Dictionary<string, Layer> layers = new Dictionary<string, Layer>();

		public Dictionary<string, PressureLayer> pressurelayers = new Dictionary<string, PressureLayer>();
		public Config config;
		private string configstring;

		public double cellsize;
		public float x_min;
		public float x_max;
		public float y_min;
		public float y_max;

		public static int x_res;
		public static int y_res;

		private List<Thread> threads = new List<Thread>();
		public List<string> threadslayers = new List<string>();

		private cEwEShell shell;
		private List<cPressure> pressures = new List<cPressure>();
		private List<cPressure> cfishingpressures = new List<cPressure>();
		public List<cGrid> outputs = new List<cGrid>();

        /// <summary>Start values for fishing intensity as returned by EwEShell.</summary>
        public List<cScalar> fishingscalarstart = new List<cScalar>();

		public MEL() {
			if(MEL.OUTPUTDIR.Contains("stable")) {
				MEL.url = "http://localhost/stable";
			}

			this.shell = new cEwEShell();

			this.LoadConfig();

            //read from config? 
			this.x_min = this.config.x_min;
			this.y_min = this.config.y_min;

			this.x_max = this.config.x_max;
			this.y_max = this.config.y_max;

			this.cellsize = this.config.cellsize;

			InitPressureLayers();

			WaitForThreads();
			RasterizeLayers();

			foreach(KeyValuePair<string, PressureLayer> pressure in this.pressurelayers) {
				this.pressures.Add(pressure.Value.pressure);
			}

			//is this supposed to work like this?
			foreach(Fishing fishingscalar in this.config.fishing) {
				this.pressures.Add(new cPressure(fishingscalar.name, fishingscalar.scalar));
				this.cfishingpressures.Add(new cPressure(fishingscalar.name, fishingscalar.scalar));
			}

			UpdateFishing();

			WaitForThreads();
			
			if(this.shell.Configuration(this.configstring, fishingscalarstart)) {
				NameValueCollection values = new NameValueCollection();

				foreach(cScalar fish in fishingscalarstart) {
					values.Add("fish__" + fish.Name, fish.Value.ToString());
				}
				
				string tmp = MEL.HttpGet(new WebClient(), "/api/mel/InitialFishing", values);
				Console.WriteLine(tmp);

				// Dump game version for testing purposes
				Console.WriteLine("Loaded EwE model '{0}', {1}, {2}", this.shell.CurrentGame.Version, this.shell.CurrentGame.Author, this.shell.CurrentGame.Contact);
                
				//eweshell initialised fine
                this.shell.Startup();

				Console.WriteLine("Startup done");
			}
            else {
				//something went wrong here
				Console.WriteLine("EwE Startup failed");
			}
		}

		#region Initialize

		/// <summary>
		/// load the config file from the server
		/// </summary>
		public virtual void LoadConfig() {
			WebClient webClient = new WebClient();
			//file name should probably be obtained from the server
			this.configstring = MEL.HttpGet(webClient, "/api/mel/config");

			this.config = JsonConvert.DeserializeObject<Config>(this.configstring);

			this.cellsize = this.config.cellsize;
			MEL.x_res = this.config.columns;
			MEL.y_res = this.config.rows;

			foreach(Outcome o in this.config.outcomes) {
				//Console.WriteLine(o.name);
				this.outputs.Add(new cGrid(o.name, MEL.x_res, MEL.y_res));
			}
		}

		/// <summary>
		/// Initialise the pressure layers by loading in the WKT from the server
		/// </summary>
		public virtual void InitPressureLayers() {
			foreach(Pressure pressure in config.pressures) {
				this.pressurelayers[pressure.name] = new PressureLayer(pressure.name);

				foreach(Layer layer in pressure.layers) {
					if(!this.layers.ContainsKey(layer.name)) {
						this.layers[layer.name] = layer;

						if(layer.influence > 0f) {
							Thread t = new Thread(() => LoadThreaded(layer, pressure.name));
							this.threads.Add(t);
							t.Start();
						}
					}
					else {
						this.pressurelayers[pressure.name].Add(this.layers[layer.name]);
					}
				}

			}
		}

		/// <summary>
		/// Retrieve the WKT of a single layer from the server
		/// </summary>
		/// <param name="layer">Layer object to be loaded</param>
		/// <param name="name">Name of the pressure layer</param>
		public void LoadThreaded(Layer layer, string name) {
			bool protection = name.ToLower().Contains("protection");
			layer.GetWKT(this, protection);
			this.pressurelayers[name].Add(layer);
		}
		#endregion

		#region Tick
		/// <summary>
		/// Update tick for MEL, runs once per second
		/// </summary>
		public virtual void Tick() {
			var watch = System.Diagnostics.Stopwatch.StartNew();
			//Console.WriteLine("Trying tick");
			string request = MEL.HttpGet(new WebClient(), "/api/mel/ShouldUpdate/" + this.lastupdatedmonth);

			if(request == "-1") return;

			this.lastupdatedmonth = int.Parse(request);

			Console.WriteLine("Executing month: " + this.lastupdatedmonth);

			WaitForThreads();

			//Console.WriteLine("all threads are cleared");

			//update pressure layers where needed
			UpdatePressureLayers();

			//Console.WriteLine("updated pressure layers");
			
			WaitForThreads();

			UpdateFishing();
			RasterizeLayers();

			//Start EwE tick
			this.shell.Tick(this.pressures, this.outputs);

			StoreTick();
			KPI();
			TickDone();

			WaitForThreads();

			watch.Stop();
			Console.WriteLine("Month " + this.lastupdatedmonth.ToString() + " executed in: " + watch.ElapsedMilliseconds + "ms");
			
			Console.WriteLine("------------------");
		}

		/// <summary>
		/// Query the server to check for updates on any layers, then load the new WKT
		/// </summary>
		public virtual void UpdatePressureLayers() {
			//get the list of layers that need to be updated
			string request = MEL.HttpGet(new WebClient(), "/api/mel/Update");
			

			string[] toupdate = request.Split(',');

			
			if(toupdate.Length == 0 || (toupdate.Length == 1 && toupdate[0] == "")) {
				return;
			}

			List<string> updated = new List<string>();
			
			foreach(KeyValuePair<string, PressureLayer> pressure in this.pressurelayers) {
				foreach(Layer layer in pressure.Value.layers) {
					if(layer == null) continue;

					foreach(string basename in toupdate) {
						if(layer.name.Contains(basename)) {
							//tag the pressure layer to be redrawn
							pressure.Value.redraw = true;

							if(!updated.Contains(layer.name)) {
								updated.Add(layer.name);
								//layer has changed, update it
								Thread t = new Thread(() => UpdateThreaded(layer));
								this.threads.Add(t);
								t.Start();
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Overwrite the WKT with a new version from the server
		/// </summary>
		/// <param name="layer">Layer object to be updated</param>
		public void UpdateThreaded(Layer layer) {
			layer.GetWKT(this);
		}

		public void UpdateFishing() {
			string fishstring = MEL.HttpGet(new WebClient(), "/api/mel/GetFishing/" + this.lastupdatedmonth);
			if(fishstring == "")
				return;

			Fishing[] fishing = JsonConvert.DeserializeObject<Fishing[]>(fishstring);

			for(int i = 0; i < this.cfishingpressures.Count; i++) { 
				foreach(Fishing f in fishing) {
					if(this.cfishingpressures[i].Name == f.name) {
						this.cfishingpressures[i] = new cPressure(f.name, f.scalar);
					}
				}
			}
		}

		/// <summary>
		/// Calculate the KPIs 
		/// </summary>
		public void KPI() {
			//this is slow
			//var watch = System.Diagnostics.Stopwatch.StartNew();
			foreach(cGrid outcome in this.outputs) {
				Thread t = new Thread(() => ThreadedKPI(outcome));
				this.threads.Add(t);
				t.Start();
			}

			//watch.Stop();
			//Console.WriteLine("KPI: " + watch.ElapsedMilliseconds);
		}

		private void ThreadedKPI(cGrid outcome) {
			NameValueCollection values = new NameValueCollection() {
					{ "name" , outcome.Name },
					{ "value" , outcome.Mean.ToString() },
					{ "type" , "ECOLOGY" },
					{ "unit" , outcome.Units }
				};

			MEL.HttpGet(new WebClient(), "/api/kpi/post", values);
		}

		public void TickDone() {
			MEL.HttpGet(new WebClient(), "/api/mel/TickDone");
		}
		#endregion

		#region Internal

		//this is slow
		private void StoreTick() {
			foreach(cGrid grid in this.outputs) {
				Thread t = new Thread(() => StoreTickThreaded(grid));
				this.threads.Add(t);
				t.Start();
			}
		}

		private void StoreTickThreaded(cGrid grid) {
			Bitmap bitmap = Rasterizer.ToBitmapSlow(grid.Cell);
			bitmap.Save(MEL.OUTPUTDIR + MEL.ConvertLayerName(grid.Name) + ".tif");
			MEL.HttpGet(new WebClient(), "/api/mel/UpdateLayer/" + MEL.ConvertLayerName(grid.Name));
		}

		/// <summary>
		/// rasterize the loaded layers to .png files
		/// </summary>
		public virtual void RasterizeLayers() {
			//var watch = System.Diagnostics.Stopwatch.StartNew();
			this.pressures.Clear();

			foreach(KeyValuePair<string, PressureLayer> entry in this.pressurelayers) {
				if(entry.Value.redraw)
					entry.Value.RasterizeLayers(this);

				this.pressures.Add(entry.Value.pressure);
			}
			
			foreach(cPressure fishing in this.cfishingpressures) {
				this.pressures.Add(new cPressure(fishing.Name, fishing.Scalar));
			}

			//watch.Stop();
			//Console.WriteLine("RasterizeLayers: " + watch.ElapsedMilliseconds);
		}

		/// <summary>
		/// Wait for all threads to be finished until moving on
		/// </summary>
		
		private void WaitForThreads() {
			if(this.threads.Count == 0) return;

			bool isready = true;

			while(isready) {
				isready = false;
				for(int i = 0; i < this.threads.Count; i++) { 
					if(this.threads[i].IsAlive) {
						isready = true;
						break;
					}
				}
			}

			this.threads.Clear();
		}

		public static string HttpGet(WebClient client, string url, NameValueCollection values = null) {
			if(values == null) values = new NameValueCollection();
			byte[] response = client.UploadValues(MEL.url + url, values);

			return System.Text.Encoding.UTF8.GetString(response);
		}

		public static string ConvertLayerName(string name) {
			return "mel_" + name.Replace(' ', '_');
		}

		#endregion
	}
}
