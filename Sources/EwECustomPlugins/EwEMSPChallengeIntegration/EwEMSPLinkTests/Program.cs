using MSWSupport;
namespace MEL
{
	class Program
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			MEL? mel = null;
			try
			{
				mel = new MEL(args);
			}
			catch (Exception e)
			{
	            ConsoleColor orgColor = Console.ForegroundColor;
	            Console.ForegroundColor = ConsoleColor.Red;
	            ConsoleLogger.Error(e.Message);
	            Console.ForegroundColor = orgColor;
				Environment.Exit(1);
			}

			// mel loop
			while (true)
			{
				Thread.Sleep(MEL.TICK_DELAY_MS);
				mel.Tick();

				// todo: decide if we want to test continuously or just once
				break; // for now, we only do a single tick, enough to start a single simulation and test the program
				// Listen to the console for a command to stop the program. Either Escape or Ctrl+C
				if (!Console.KeyAvailable)
					continue;
				ConsoleKeyInfo key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Escape || (key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.C)
				{
					break;
				}
			}

			Environment.Exit(0);
		}

		static void CurrentDomain_UnhandledException(object aSender, UnhandledExceptionEventArgs aException)
        {
	        ConsoleLogger.Error(((Exception) aException.ExceptionObject).Message);
        }
	}
}