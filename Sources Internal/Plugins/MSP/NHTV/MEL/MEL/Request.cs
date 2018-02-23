using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace MEL {
	//obsolete
	class Request {
		public Request() { }

		public static async void Get(string url, Action<string> callback) {
			using(HttpClient client = new HttpClient()) {
				string response = await client.GetStringAsync(url);

				callback(response);
			}
		}

		public static void GetExport(string layer, Action<string> callback) {
			Request.Get("http://localhost/api/layer/export/" + layer, callback);
		}
	}
}
