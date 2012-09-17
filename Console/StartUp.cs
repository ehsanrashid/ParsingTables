using System;

namespace ParsingTables
{
    public static class StartUp
    {
        [STAThread]
        public static void Main(String[] args)
        {
            if (null != args)
            {
                if (args.Length == 2)
                {
                    ParsingTables.Parse(args[0], args[1]);
                }
                else
                {

                    var grammar = new[]
                        //{
                        //    "D --> S $",
                        //    "S --> i E t S S'",
                        //    "S --> a",
                        //    "S' --> e S",
                        //    "S' --> #",
                        //    "E --> b"
                        //};
                        //{
                        //    "D --> E $",
                        //    "E --> T E'",
                        //    "E' --> + T E'",
                        //    "E' --> #",
                        //    "T --> F T'",
                        //    "T' --> * F T'",
                        //    "T' --> #",
                        //    "F --> ( E )",
                        //    "F --> i"
                        //};

                        {
                            "D --> S $",
                            "S --> C C",
                            "C --> c C",
                            "C --> d"
                        };


                    ParsingTables.Parse(grammar, "Output.txt");
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.Read();
        }
    }
}