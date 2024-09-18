namespace MSWSupport
{
    public static class ConsoleLogger
    {
        private static void Write(string aMessage)
        {
            Console.WriteLine(aMessage);
        }

        private static void WriteWithColor(string aMessage, ConsoleColor aColor)
        {
            ConsoleColor orgColor = Console.ForegroundColor;
            Console.ForegroundColor = aColor;
            Write(aMessage);
            Console.ForegroundColor = orgColor;
        }

        public static void Error(string aMessage)
        {
            WriteWithColor(aMessage, ConsoleColor.Red);
        }

        public static void Warning(string aMessage)
        {
            WriteWithColor(aMessage, ConsoleColor.Yellow);
        }

        public static void Info(string aMessage)
        {
            Write(aMessage);
        }
    }
}
