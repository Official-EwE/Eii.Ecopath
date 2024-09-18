using Newtonsoft.Json.Linq;

namespace MEL
{
	/// <summary>
	/// Pressure configuration as provided by the MSP Platform API
	/// Each pressure has a name and a set of layer data which specifies the influences of these pressures.
	/// </summary>
	public class Pressure
	{
		public string name { get; set; }
		public JObject? policy_filters { get; set; } = null;
		public List<LayerData> layers { get; set; }
	}
}
