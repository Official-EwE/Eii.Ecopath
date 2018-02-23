using EwEShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEL {
	class Pressures : KPI{

		public double pressures;

		public override void Calculate(MEL mel) {
			foreach(KeyValuePair<string, PressureLayer> pressure in mel.pressurelayers) {
				this.pressures += Utility.SumArrayToValue(pressure.Value.rawdata) / (MEL.x_res * MEL.y_res);
			}

			this.pressures /= mel.pressurelayers.Count;

			Console.WriteLine("KPI: Pressures Calculated");
		}
	}
}
