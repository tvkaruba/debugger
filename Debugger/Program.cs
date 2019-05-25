using System;
using System.IO;
using System.Text;

namespace Debugger
{
    class Program
    {
        // Ну тут все простенько конечно, но оставлю пару комментов.
        static void Main(string[] args)
        {
            Debugger dbg;

            // Считывание текста программы из input.txt
            try
            {
                using (FileStream fs = File.OpenRead(@"..\..\input.txt"))
                {
                    byte[] arr = new byte[fs.Length];
                    fs.Read(arr, 0, arr.Length);
                    var text = Encoding.Default.GetString(arr);
                    dbg = new Debugger(text);

                    Console.WriteLine(text);
                }
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key for exit.");
                Console.ReadKey();
                return;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key for exit.");
                Console.ReadKey();
                return;
            }
            catch (Exception)
            {
                Console.WriteLine("Something was wrong. );");
                Console.WriteLine("Press any key for exit.");
                Console.ReadKey();
                return;
            }

            DisplayCommands();

            // Описания комманд можно почитать в методе DisplayCommands чуть ниже main.
            // В случае возникновения ошибки, печатается стектрайс и программа завершается
            // (сообщение характерное для ошибки печатается на более низком уровне).
            bool isExit = false;
            while (!isExit)
            {
                try
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
                                Console.Write("\nEnd of program.");
                                isExit = true;
                            }

                            break;

                        case 'o':
                            if (!dbg.StepOver())
                            {
                                Console.Write("\nEnd of program.");
                                isExit = true;
                            }

                            break;

                        case 't':
                            var trace = dbg.GetStackTrace();
                            Console.Write(trace);
                            break;

                        case 'v':
                            var list = dbg.GetVariablesList();
                            Console.Write(list);
                            break;

                        case 'e':
                            isExit = true;
                            break;

                        default:
                            Console.Write("\nIncorrect command. Try again.");
                            break;
                    }
                }
                catch (Exception)
                {
                    var trace = dbg.GetStackTrace();
                    Console.Write("\n" + trace);
                    isExit = true;
                }
            }

            Console.WriteLine("\nPress any key for exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Выводит на экран список команд для ввода.
        /// </summary>
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
