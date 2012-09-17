using System;
using System.IO;

using Parser;

namespace ParsingTables
{
    public static class ParsingTables
    {

        public static void Parse(String fn_in, String fn_out)
        {
            var parser = new
                SLRParser
                //CLRParser 
                (fn_in);

            WriteToFile(parser, fn_out);
        }


        public static void Parse(String[] grammar, String fn_out)
        {
            var parser = new
                SLRParser
                //CLRParser 
                (grammar);

            WriteToFile(parser, fn_out);
        }

        public static void WriteToFile(SLRParser parser, String fn_out)
        {
            try
            {
                // Write to Output File
                using (var stream = new FileStream(fn_out, FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Grammar >>>");
                    writer.WriteLine(parser.Grammar);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    writer.WriteLine("Grammar Entities >>>");
                    writer.WriteLine(parser.EntityCol);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    writer.WriteLine("First & Follow >>>");
                    writer.WriteLine(parser.First_Follow());
                    writer.WriteLine("Closures >>>");
                    writer.WriteLine(parser.Cloures_GoToTable());
                    writer.WriteLine(parser.LALRTable());
                    writer.Close();
                }

                /*
                var g = GrammarIO.ReadGrammar(args[ 0 ]);

                Console.WriteLine(g);
                var eSet = g.Entities;

                StringBuilder sb = new StringBuilder();
                foreach( Entity entity in eSet )
                {
                    //// if grammar is not left recursive
                    sb.AppendLine("FIRST( " + entity + " ) : " + g.First(entity));

                    if( entity is NonTerminal )
                        sb.AppendLine("FOLLOW( " + entity + " ) : " + g.Follow(entity));
                }
                sb.Append("--------");
                Console.WriteLine(sb.ToString());

                Console.Read();
                */
            }
            catch (FileNotFoundException expNoFile)
            {
                Console.WriteLine(expNoFile);
            }
            catch (IOException expIO)
            {
                Console.WriteLine(expIO);
            }
            catch (FormatException expFormat)
            {
                Console.WriteLine(expFormat);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        public static void WriteToFile(CLRParser parser, String fn_out)
        {
            try
            {
                // Write to Output File
                using (var stream = new FileStream(fn_out, FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Grammar >>>");
                    writer.WriteLine(parser.Grammar);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    writer.WriteLine("Grammar Entities >>>");
                    writer.WriteLine(parser.EntityCol);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    writer.WriteLine("First & Follow >>>");
                    writer.WriteLine(parser.First_Follow());
                    writer.WriteLine("Closures >>>");
                    writer.WriteLine(parser.Cloures_GoToTable());
                    writer.WriteLine(parser.LALRTable());
                    writer.Close();
                }

                /*
                var g = GrammarIO.ReadGrammar(args[ 0 ]);

                Console.WriteLine(g);
                var eSet = g.Entities;

                StringBuilder sb = new StringBuilder();
                foreach( Entity entity in eSet )
                {
                    //// if grammar is not left recursive
                    sb.AppendLine("FIRST( " + entity + " ) : " + g.First(entity));

                    if( entity is NonTerminal )
                        sb.AppendLine("FOLLOW( " + entity + " ) : " + g.Follow(entity));
                }
                sb.Append("--------");
                Console.WriteLine(sb.ToString());

                Console.Read();
                */
            }
            catch (FileNotFoundException expNoFile)
            {
                Console.WriteLine(expNoFile);
            }
            catch (IOException expIO)
            {
                Console.WriteLine(expIO);
            }
            catch (FormatException expFormat)
            {
                Console.WriteLine(expFormat);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

    }

}
