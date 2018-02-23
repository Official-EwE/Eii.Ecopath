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
			//var watch = System.Diagnostics.Stopwatch.StartNew();
            //Ogr.RegisterAll();
            //Gdal.AllRegister();

            //init ewe here

            //load and configure the initial state of MEL
            MEL mel = new MEL();
			


			//rasterize all the layers
			//mel.RasterizeLayers();


			//watch.Stop();

			//Console.WriteLine(watch.ElapsedMilliseconds);

			//while(true) { }

			while(true) {
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
