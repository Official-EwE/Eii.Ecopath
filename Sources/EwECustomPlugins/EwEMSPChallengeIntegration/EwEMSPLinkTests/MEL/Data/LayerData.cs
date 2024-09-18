using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MEL
{
	/// <summary>
	/// Layer data for MEL as returned by the MSP Platform API in a deserialised form
	/// </summary>
	public class LayerData
	{
		[JsonProperty(PropertyName = "name")]
		private string? encoded_name = null;

		public string? layer_name;
		public int layer_type = -1;

		public float influence;
		public bool construction;

		[OnDeserialized]
		private void OnDeserializedMethod(StreamingContext context)
		{
			int splitterIndex = encoded_name.IndexOf('|');
			if (splitterIndex != -1)
			{
				layer_name = encoded_name.Substring(0, splitterIndex);
				layer_type = int.Parse(encoded_name.Substring(splitterIndex + 1));
			}
			else
			{
				layer_name = encoded_name;
			}
		}
	}
}