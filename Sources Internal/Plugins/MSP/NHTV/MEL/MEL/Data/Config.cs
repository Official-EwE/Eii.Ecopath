using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEL {
	public class Config {
		public string region { get; set; }
		public string modelfile { get; set; }
		public string mode { get; set; }
		public int rows { get; set; }
		public int columns { get; set; }
		public double cellsize { get; set; }
		public double biomassvalue { get; set; }
		public double fishvalue { get; set; }
		public List<Pressure> pressures { get; set; }
		public List<Fishing> fishing { get; set; }
		public List<Outcome> outcomes { get; set; }

		public int x_min { get; set; }
		public int y_min { get; set; }
		public int x_max { get; set; }
		public int y_max { get; set; }
	}
}
