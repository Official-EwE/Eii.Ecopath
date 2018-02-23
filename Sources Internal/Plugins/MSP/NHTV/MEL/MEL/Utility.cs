using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEL {
	class Utility {
		public static double SumArrayToValue(double[,] arr) {
			double val = 0;

			for(int x = 0; x < MEL.x_res; x++) {
				for(int y = 0; y < MEL.y_res; y++) {
					val += arr[x, y];
				}
			}

			return val;
		}

		public static double[,] SumArray(double[,] arr1, double[,] arr2) {
			for(int x = 0; x < MEL.x_res; x++) {
				for(int y = 0; y < MEL.y_res; y++) {
					arr1[x, y] += arr2[x, y];
				}
			}

			return arr1;
		}

		public static double[,] SumArray(double[,] arr1, double[,] arr2, int max) {
			for(int x = 0; x < MEL.x_res; x++) {
				for(int y = 0; y < MEL.y_res; y++) {
					arr1[x, y] += arr2[x, y];
					if(arr1[x, y] > 1) {
						arr1[x, y] = 1;
					}
				}
			}

			return arr1;
		}

		public static void InsideOutsideArea(double[,] biomass, double[,] protectedarea) {

		}
	}
}
