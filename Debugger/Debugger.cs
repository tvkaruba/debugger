using System;
using System.Collections.Generic;
using System.Linq;

namespace Debugger
{
    public class Debugger
    {
        public class CustomString
        {
            public int Id { get; }

            public string Content { get; }
            public List<string> Items { get; }

            public CustomFunction Function { get; }

            public CustomString(int id, string content, CustomFunction function)
            {
                Id = id;
                Function = function;
                Content = content;
                Items = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            public override string ToString()
            {
                return $"{Id}: {Content} ({Function.Name})";
            }
        }

        public class CustomVariable
        {
            public string Name { get; }
            public string Value { get; set; }

            public CustomVariable(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public override string ToString()
            {
                return $"{Name}: {Value}";
            }
        }

        public class CustomFunction
        {
            public int Id { get; }

            public string Name { get; }
            public List<CustomString> Strings { get; set; } = new List<CustomString>();

            public CustomFunction(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public void Execute(int id, bool isStepOver, Debugger dbg)
            {
                try
                {
                    var cur = Strings.Find(s => s.Id == id);
                    switch (cur.Items[0])
                    {
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

                        case "call":
                            if (dbg.Trace.Any(f => f.Function.Name == cur.Items[1]))
                            {
                                throw new StackOverflowException();
                            }

                            if (isStepOver)
                            {
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

        public List<CustomFunction> Functions { get; set; } = new List<CustomFunction>();
        public List<CustomVariable> Variables { get; set; } = new List<CustomVariable>();
        public Stack<CustomString> Trace { get; set; } = new Stack<CustomString>();

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
                    Functions.Last().Strings.Add(new CustomString(Functions.Last().Strings.Count, str, Functions.Last()));
                }
            }
        }

        public bool StepInto()
        {
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

                    if (!StepInto())
                    {
                        return false;
                    }
                }
                else
                {
                    func.Execute(func.Strings.Find(f => f.Id == Trace.Peek().Id).Id + 1,
                        false, this);
                }
            }
            else
            {
                var func = Functions.Find(f => f.Name == "main");
                func.Execute(func.Strings.First().Id + 1,false, this);
            }

            return true;
        }

        public bool StepOver()
        {
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
                    func.Execute(func.Strings.Find(f => f.Id == Trace.Peek().Id).Id + 1,
                        true, this);
                }
            }
            else
            {
                var func = Functions.Find(f => f.Name == "main");
                func.Execute(func.Strings.First().Id + 1, true, this);
            }

            return true;
        }

        public string GetStackTrace()
        {
            string trace = "";
            foreach (var step in Trace)
            {
                trace += ("\n" + step);
            }

            return trace;
        }

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
