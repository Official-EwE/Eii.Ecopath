using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OSGeo.OGR;

namespace MEL {
	/// <summary>
	/// test class that grabs local data
	/// </summary>
	class MELTest : MEL{
		public MELTest() {
			this.LoadConfig();
		}

		/// <summary>
		/// set up a simple pressure layer to test with
		/// </summary>
		public override void LoadConfig() {
			var watch = System.Diagnostics.Stopwatch.StartNew();

			string name = "noise";
			this.pressurelayers[name] = new PressureLayer(name);
			this.pressurelayers[name].Add(new Layer(1, 0.1f, true));
			this.pressurelayers[name].Add(new Layer(2, 0.2f, true));
			this.pressurelayers[name].Add(new Layer(3, 0.3f, true));

			name = "something";
			this.pressurelayers[name] = new PressureLayer(name);
			this.pressurelayers[name].Add(new Layer(1, 0.1f, true));
			this.pressurelayers[name].Add(new Layer(2, 0.2f, true));

			this.cellsize = 0.1f;

			this.x_min = -5;
			this.x_max = 9;
			this.y_min = 50;
			this.y_max = 62;

			MEL.x_res = (int)Math.Abs((this.x_min - this.x_max) / this.cellsize);
			MEL.y_res = (int)Math.Abs((this.y_min - this.y_max) / this.cellsize);
			watch.Stop();

			Console.WriteLine(watch.ElapsedMilliseconds);
			Console.WriteLine("DONE");
		}
	}
}
