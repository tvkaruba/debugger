using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Debugger
{
    /// <summary>
    /// Содержит поля и методы необходимые для построчного исполнения кода.
    /// </summary>
    public class Debugger
    {
        /// <summary>
        /// Содержит данные строки.
        /// </summary>
        public class CustomString
        {
            /// <summary>
            /// идентификатор строки.
            /// </summary>
            public int Id { get; }
            /// <summary>
            /// Строка в изначальном виде.
            /// </summary>
            public string Content { get; }
            /// <summary>
            /// Значащие элементы содержащиеся в строке.
            /// </summary>
            public List<string> Items { get; }
            /// <summary>
            /// Функция, которой принадлежит строка.
            /// </summary>
            public CustomFunction Function { get; }
            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="id">Идентификатор строки.</param>
            /// <param name="content">Строка.</param>
            /// <param name="function">Функция, которой принадлежит строка.</param>
            public CustomString(int id, string content, CustomFunction function)
            {
                Id = id;
                Function = function;
                Content = content;
                Items = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            /// <summary>
            /// Вывод данных строки для стектрейса.
            /// </summary>
            /// <returns>Строка с указанием ее номера и функции, которой она принадлежит.</returns>
            public override string ToString()
            {
                return $"{Id}: {Content} ({Function.Name})";
            }
        }
        /// <summary>
        /// Содержит данные переменной.
        /// </summary>
        public class CustomVariable
        {
            /// <summary>
            /// Имя переменной.
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// Значение переменной.
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="name">Имя.</param>
            /// <param name="value">Значение.</param>
            public CustomVariable(string name, string value)
            {
                Name = name;
                Value = value;
            }
            /// <summary>
            /// Вывод данных переменной.
            /// </summary>
            /// <returns>Строка с именем и значением переменной.</returns>
            public override string ToString()
            {
                return $"{Name}: {Value}";
            }
        }
        /// <summary>
        /// Содержит данные функции.
        /// </summary>
        public class CustomFunction
        {
            /// <summary>
            /// Идентификатор функции.
            /// </summary>
            public int Id { get; }
            /// <summary>
            /// Имя функции.
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// Список строк функции.
            /// </summary>
            public List<CustomString> Strings { get; set; } = new List<CustomString>();
            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="id">Идентификатор.</param>
            /// <param name="name">Имя.</param>
            public CustomFunction(int id, string name)
            {
                Id = id;
                Name = name;
            }
            /// <summary>
            /// Исполняет строку с заданым идентификатором.
            /// </summary>
            /// <param name="id">Идентификатор исполняемой строки.</param>
            /// <param name="isStepOver">Флаг, указывающий, выполнять шаг с заходом, либо шаг с обходом.</param>
            /// <param name="dbg">Текущий экземпляр дебагера.</param>
            public void Execute(int id, bool isStepOver, Debugger dbg)
            {
                try
                {
                    // По хорошему все это нужно по разным методам разнести, но коль уж их тут всего 3,
                    // то я прописал их прямо в кейсах.
                    var cur = Strings.Find(s => s.Id == id);
                    switch (cur.Items[0])
                    {
                        // Печатает значение переменной или кидает ошибку, если такой переменной не существует.
                        case "print":
                            if (dbg.Variables.Find(v => v.Name == cur.Items[1]) == null)
                            {
                                throw new ArgumentNullException(cur.Items[1]);
                            }
                            else
                            {
                                Console.Write("\n" + dbg.Variables.Find(v => v.Name == cur.Items[1]).Value);
                            }

                            dbg.Trace.Push(cur);
                            break;

                        // Изменяет значение существующей переменной или создает новую.
                        case "set":
                            if (dbg.Variables.Find(v => v.Name == cur.Items[1]) == null)
                            {
                                dbg.Variables.Add(new CustomVariable(cur.Items[1], cur.Items[2]));
                            }
                            else
                            {
                                dbg.Variables.Find(v => v.Name == cur.Items[1]).Value = cur.Items[2];
                            }

                            dbg.Trace.Push(cur);
                            break;

                        // Вызов функции.
                        case "call":
                            if (dbg.Trace.Any(f => f.Function.Name == cur.Items[1]))
                            {
                                // Хорошо что у наш язык такой простой, а то б пришлось какие-нибудь
                                // циклы в графе исполнения искать.
                                throw new StackOverflowException();
                            }

                            if (isStepOver)
                            {
                                // Для шага с обходом выполняет все строки вызванной функции.
                                dbg.Trace.Push(cur);
                                var func = dbg.Functions.Find(f => f.Name == cur.Items[1]);
                                for (int i = 1; i < func.Strings.Count; ++i)
                                {
                                    func.Execute(func.Strings[i].Id, true, dbg);
                                }

                                while (dbg.Trace.Peek().Function.Name == func.Name)
                                {
                                    dbg.Trace.Pop();
                                }
                            }
                            else
                            {
                                // Для шага с заходом, выполняет первую строку вызванной функции.
                                dbg.Trace.Push(cur);
                                var func = dbg.Functions.Find(f => f.Name == cur.Items[1]);
                                func.Execute(func.Strings.First().Id + 1, false, dbg);
                            }

                            break;

                        default:
                            throw new InvalidOperationException(cur.Items[0]);
                            break;
                    }
                }
                catch (StackOverflowException)
                {
                    Console.WriteLine("\nOops! Seems, its stack overflow...");
                    throw new Exception();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("\nInvalid operator: " + ex.Message);
                    throw new Exception();
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("\nUndefined variable: " + ex.Message);
                    throw new Exception();
                }
                catch (Exception msg)
                {
                    Console.WriteLine("\n" + msg);
                    throw new Exception();
                }
            }
        }
        /// <summary>
        /// Список функций.
        /// </summary>
        public List<CustomFunction> Functions { get; set; } = new List<CustomFunction>();
        /// <summary>
        /// Список переменных.
        /// </summary>
        public List<CustomVariable> Variables { get; set; } = new List<CustomVariable>();
        /// <summary>
        /// Стэктрейс.
        /// </summary>
        public Stack<CustomString> Trace { get; set; } = new Stack<CustomString>();
        /// <summary>
        /// Конструктор. Разбивает переданный код на функции.
        /// </summary>
        /// <param name="text">Код программы.</param>
        public Debugger(string text)
        {
            var strs = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in strs)
            {
                var items = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (items[0] == "sub")
                {
                    Functions.Add(new CustomFunction(Functions.Count, items[1]));
                    Functions.Last().Strings.Add(new CustomString(Functions.Last().Strings.Count, str, Functions.Last()));
                }
                else
                {
                    if (!Functions.Any())
                    {
                        throw new InvalidDataException("Invalid program.");
                    }
                    Functions.Last().Strings.Add(new CustomString(Functions.Last().Strings.Count, str, Functions.Last()));
                }
            }
        }
        /// <summary>
        /// Шаг с заходом.
        /// </summary>
        /// <returns>Возвращает true, если шаг выполнен, false - иначе.</returns>
        public bool StepInto()
        {
            if (Trace.Any())
            {
                // Проверяем не является ли последняя строчка записанная в стеке, последней в своей функции.
                var func = Functions.Find(f => f.Name == Trace.Peek().Function.Name);
                if (Trace.Peek().Id == func.Strings.Last().Id)
                {
                    // Если эта строчка была последней, то выталкиваем все вызовы этой функции из стека.
                    while (Trace.Peek().Function.Name == func.Name)
                    {
                        Trace.Pop();
                        // Если стек пуст - это конец.
                        if (!Trace.Any())
                        {
                            return false;
                        }
                    }
                    // После удаления всех вызовов текущей функции из стека, снова вызываем шаг с заходом.
                    // Теперь он выполнит следующую за вызовом завершенной функции строчку во внешней функции.
                    if (!StepInto())
                    {
                        return false;
                    }
                }
                else
                {
                    // Выполняем строчку, следующую за последней записанной в стеке.
                    func.Execute(func.Strings.Find(f => f.Id == Trace.Peek().Id).Id + 1, false, this);
                }
            }
            else
            {
                // Если это начало программы, то выполняем первую строчку main.
                var func = Functions.Find(f => f.Name == "main");
                func.Execute(func.Strings.First().Id + 1,false, this);
            }

            return true;
        }
        /// <summary>
        /// Шаг с обходом.
        /// </summary>
        /// <returns>Возвращает true, если шаг выполнен, false - иначе.</returns>
        public bool StepOver()
        {
            // Тут все аналогично шагу с заходом, только исполнение строчки вызывается с соответствующим флагом.
            if (Trace.Any())
            {
                var func = Functions.Find(f => f.Name == Trace.Peek().Function.Name);
                if (Trace.Peek().Id == func.Strings.Last().Id)
                {
                    while (Trace.Peek().Function.Name == func.Name)
                    {
                        Trace.Pop();

                        if (!Trace.Any())
                        {
                            return false;
                        }
                    }

                    if (!StepOver())
                    {
                        return false;
                    }
                }
                else
                {
                    func.Execute(func.Strings.Find(f => f.Id == Trace.Peek().Id).Id + 1, true, this);
                }
            }
            else
            {
                var func = Functions.Find(f => f.Name == "main");
                func.Execute(func.Strings.First().Id + 1, true, this);
            }

            return true;
        }
        /// <summary>
        /// Получает стектрейс.
        /// </summary>
        /// <returns>Строку с данными стектрейса.</returns>
        public string GetStackTrace()
        {
            string trace = "";
            foreach (var step in Trace)
            {
                trace += ("\n" + step);
            }

            return trace;
        }
        /// <summary>
        /// Получает список переменных.
        /// </summary>
        /// <returns>Строку с данными переменных.</returns>
        public string GetVariablesList()
        {
            string list = "";
            foreach (var variable in Variables)
            {
                list += ("\n" + variable);
            }

            return list;
        }
    }
}
