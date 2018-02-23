using EwEShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEL {
	class MPA : KPI{
		public double mpa = 0;
		public double[,] protectedarea;

		public override void Calculate(MEL mel) {
			this.protectedarea = new double[MEL.x_res, MEL.y_res];

			//creates the protected area list

			this.protectedarea = Utility.SumArray(this.protectedarea, mel.pressurelayers["Protection bottom trawl"].rawdata, 1);
			this.protectedarea = Utility.SumArray(this.protectedarea, mel.pressurelayers["Protection industrial trawl"].rawdata, 1);
			this.protectedarea = Utility.SumArray(this.protectedarea, mel.pressurelayers["Protection nets"].rawdata, 1);

			double inside = 0;
			double outside = 0;

			foreach(cGrid output in mel.outputs) {
				inside = 0;
				outside = 0;

				for(int x = 0; x < MEL.x_res; x++) {
					for(int y = 0; y < MEL.y_res; y++) {
						if(this.protectedarea[x, y] != 0) {
							inside += output.Cell[x, y];
						}
						else {
							outside += output.Cell[x, y];
						}
					}
				}

				this.mpa += inside / outside;
			}

			Console.WriteLine(this.mpa);
		}
	}
}
