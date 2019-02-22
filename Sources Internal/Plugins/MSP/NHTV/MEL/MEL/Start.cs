using System;
using System.Collections.Generic;
//using OSGeo.GDAL;
//using OSGeo.OGR;

namespace MEL
{
    class Start
    {

        public static void Main(string[] args)
        {
            //load and configure the initial state of MEL
            MEL mel = new MEL();
            mel.TestPressureXfer();

            while (true) {
				System.Threading.Thread.Sleep(MEL.TICKRATE);
				//watch = System.Diagnostics.Stopwatch.StartNew();
				mel.Tick();
				//watch.Stop();
				//Console.WriteLine("tick time: " + watch.ElapsedMilliseconds);
			}
		}

		public void HandleCallback(string tmp) {
			//Console.WriteLine(tmp);
		}
    }
}
