using System;
using System.IO;
using Parser;

namespace ParsingTables
{
    public static class ParsingTables
    {
        public static void Parse(String fnIn, String fnOut)
        {
            var parser = new
                SLRParser
                //CLRParser 
                (fnIn);

            WriteToFile(parser, fnOut);
        }

        public static void Parse(String[] grammar, String fnOut)
        {
            var parser = new
                SLRParser
                //CLRParser 
                (grammar);

            WriteToFile(parser, fnOut);
        }

        public static void WriteToFile(SLRParser parser, String fnOut)
        {
            try
            {
                // Write to Output File
                using (var stream = new FileStream(fnOut, FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Grammar >>>");
                    writer.WriteLine(parser.Grammar);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    
                    writer.WriteLine("Grammar Entities >>>");
                    writer.WriteLine(parser.EntityCol);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    
                    writer.WriteLine("First & Follow >>>");
                    writer.WriteLine(parser.FirstnFollow());
                    
                    writer.WriteLine("Closures >>>");
                    writer.WriteLine(parser.ClouresGoToTable());
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

        public static void WriteToFile(CLRParser parser, String fnOut)
        {
            try
            {
                // Write to Output File
                using (var stream = new FileStream(fnOut, FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Grammar >>>");
                    writer.WriteLine(parser.Grammar);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    writer.WriteLine("Grammar Entities >>>");
                    writer.WriteLine(parser.EntityCol);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    writer.WriteLine("First & Follow >>>");
                    writer.WriteLine(parser.FirstnFollow());
                    writer.WriteLine("Closures >>>");
                    writer.WriteLine(parser.ClouresGoToTable());
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