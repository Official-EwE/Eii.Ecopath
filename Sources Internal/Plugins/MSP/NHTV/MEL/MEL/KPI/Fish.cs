using EwEShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEL {
	class Fish : KPI {
		public double fishing;

		public override void Calculate(MEL mel) {
			int layercount = 0;

			foreach(cGrid output in mel.outputs) {
				foreach(Outcome outcome in mel.config.outcomes) {
					if(outcome.subcategory == "Fish") {
						this.fishing += Utility.SumArrayToValue(output.Cell) / mel.config.fishvalue;
						layercount++;
						break;
					}
				}
			}

			this.fishing /= layercount;

			Console.WriteLine("KPI: Fishing calculated");
		}
	}
}
