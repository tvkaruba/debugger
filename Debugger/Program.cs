using System;
using System.IO;
using System.Text;

namespace dbg
{
    class Program
    {
        static void Main(string[] args)
        {
            Debugger dbg;

            using (FileStream fs = File.OpenRead(@"..\..\input.txt"))
            {
                byte[] arr = new byte[fs.Length];
                fs.Read(arr, 0, arr.Length);
                var text = Encoding.Default.GetString(arr);
                dbg = new Debugger(text);

                Console.WriteLine(text);
            }
            
            DisplayCommands();

            bool isExit = false;
            while (!isExit)
            {
                Console.WriteLine();
                switch (Console.ReadKey().KeyChar)
                {
                    case 'h':
                        DisplayCommands();
                        break;

                    case 'i':
                        if (!dbg.StepInto())
                        {
                            Console.WriteLine("End of program.");
                            isExit = true;
                        }
                        break;

                    case 'o':
                        if (!dbg.StepOver())
                        {
                            Console.WriteLine("End of program.");
                            isExit = true;
                        }
                        break;

                    case 't':
                        dbg.DisplayStackTrace();
                        break;

                    case 'v':
                        dbg.DisplayVariables();
                        break;

                    case 'e':
                        isExit = true;
                        break;

                    default:
                        Console.Write("Incorrect command. Try again.");
                        break;
                }
            }

            Console.WriteLine("Press any key for exit.");
            Console.ReadKey();
        }

        private static void DisplayCommands()
        {
            Console.WriteLine("\n'h' - display list of commands\n" +
                              "'i' - step into\n" +
                              "'o' - step over\n" +
                              "'t' - display stack trace\n" +
                              "'v' - display list of variables\n" +
                              "'e' - exit\n");
        }
    }
}
