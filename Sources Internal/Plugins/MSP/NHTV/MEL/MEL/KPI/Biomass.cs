using EwEShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEL {
	class Biomass : KPI {

		public double biomass;

		public override void Calculate(MEL mel) {
			foreach(cGrid output in mel.outputs) {
				this.biomass += Utility.SumArrayToValue(output.Cell) / mel.config.biomassvalue;
			}

			this.biomass /= mel.outputs.Count;

			Console.WriteLine("Biomass calculated");
		}
	}
}
