using Newtonsoft.Json.Linq;

namespace MEL;

public class Eco
{
	public string name { get; set; }
	public JObject? policy_filters { get; set; } = null;
}
