namespace Parser
{
    using System;
    using System.Text;
    using System.IO;
    using System.Collections.Generic;

    public abstract class Parser
    {
        protected Grammar _grammar;

        protected EntityCollection<Entity> _entityCol;
        public EntityCollection<Entity> EntityCol { get { return _entityCol; } }

        protected EntityCollection<Terminal> _terminalCol;
        public EntityCollection<Terminal> TerminalCol { get { return _terminalCol; } }

        protected EntityCollection<NonTerminal> _nonterminalCol;
        public EntityCollection<NonTerminal> NonTerminalCol { get { return _nonterminalCol; } }

        //  un-augmented grammar entities
        protected EntityCollection<Entity> _entityColO;

        public Parser(String filename)
        {
            try
            {
                Grammar = Grammar.Read(filename);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        public Parser(params String[] grmProductions)
        {
            try
            {
                Grammar = Grammar.Read(grmProductions);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public Grammar Grammar
        {
            get { return _grammar; }
            set
            {
                _grammar = value;
                Extract();
            }
        }

        public const String SEPARATOR = "________________________________________";

        public String First_Follow()
        {
            StringBuilder sbFF = new StringBuilder();
            foreach (Entity entity in _entityColO)
            {
                // if grammar is not left recursive
                sbFF.AppendLine(String.Format("FIRST {0} : {1}", entity, _grammar.First(entity)));
                if (entity is NonTerminal)
                {
                    sbFF.AppendLine(String.Format("FOLLOW {0} : {1}", entity, _grammar.Follow(entity)));
                }
                sbFF.AppendLine("------");
            }
            sbFF.Append(SEPARATOR);
            return sbFF.ToString();
        }

        public abstract void Extract();
    }

    public class SLRParser : Parser
    {
        //ClosureCol & GoTo
        private SLRProduction _prods;

        private ClosureCollection _closureCol;
        public ClosureCollection ClosureCol { get { return _closureCol; } }

        private int _gotoCount;
        private int _dotCount;

        private GotoEntry[] _gotoTable;
        private String[,] _tableForm;

        public SLRParser(String filename) : base(filename) { }

        public SLRParser(params String[] grmProductions) : base(grmProductions) { }

        public override void Extract()
        {
            try
            {
                _entityCol = _grammar.Entities;
                _terminalCol = _grammar.Terminals;
                _nonterminalCol = _grammar.NonTerminals;

                // removing augmented entity
                Entity augment = _entityCol[0];
                _entityColO = _entityCol - augment;
                _nonterminalCol.Remove((NonTerminal)augment);

                //Cloure_GoToTable
                _prods = new SLRProduction(_grammar[0].Producer, _grammar[0].Product);
                
                _gotoCount = 0;
                _dotCount = 0;

                _closureCol = new ClosureCollection(new Closure[] { new SLRClosure(GotoTitle(), _grammar, new SLRProduction[] { _prods }) });

                ++_gotoCount;
                for (int c = 0; c < _closureCol.Count; ++c)
                {
                    SLRClosure closure = (SLRClosure)_closureCol[c];
                    foreach (Entity entity in _entityColO - (Terminal)"$")
                    {
                        Closure gotoClosure = closure.GoToEntity(entity);
                        if (!gotoClosure.IsNull)
                        {
                            int index =
                                //_closureCol.List.FindIndex((Closure clr) => (clr == gotoClosure));
                                _closureCol.List.IndexOf(gotoClosure);
                            if (index == -1)
                            {
                                // add new Goto State
                                gotoClosure.Title = GotoTitle();
                                _closureCol += gotoClosure;
                                ++_gotoCount;
                            }
                            else
                            {
                                gotoClosure = _closureCol[index];
                            }

                            int length = (_gotoTable == default(GotoEntry[])) ? 0 : _gotoTable.Length;
                            GotoEntry[] newTable = new GotoEntry[length + 1];
                            for (int g = 0; g < length; ++g)
                            {
                                newTable[g] = _gotoTable[g];
                            }
                            newTable[length] = new GotoEntry(closure, entity, gotoClosure);
                            _gotoTable = newTable;
                        }
                        ++_dotCount;
                    }
                }

                // LR Table
                _tableForm = new String[_entityCol.Count, _closureCol.Count];

                for (int c = 0; c < _closureCol.Count; ++c)
                {
                    int terminalLength = _terminalCol.Count;
        
                    //Shift
                    for (int t = 0; t < terminalLength; ++t)
                    {
                        foreach (GotoEntry gotoEntity in _gotoTable)
                        {
                            if (gotoEntity.X == _terminalCol[t] && gotoEntity.I == _closureCol[c])
                            {
                                _tableForm[t, c] = "s" + gotoEntity.Goto.Title.Split('[', ']')[1];
                                break;
                            }
                        }
                    }

                    //Reduce
                    for (int p = 0; p < _closureCol[c].Count; ++p)
                    {
                        SLRProduction production = _closureCol[c][p];
                        if (production.DotPosition == (production.Count - 1)
                        && production == _closureCol[1][0] // S' --> S .$
                          )  // Accepted
                        {
                            _tableForm[_terminalCol & (Terminal)"$", 1] = "!!";
                        }
                        if (production.DotPosition == production.Count)
                        {
                            EntityCollection<Terminal> followCol = _grammar.Follow(production.Producer);
                            if (followCol == default(EntityCollection<Terminal>)) continue;
                            //followCol.Remove( (Terminal)"$" );
                            foreach (Entity follow in followCol)
                            {
                                _tableForm[_terminalCol & follow, c] = "r" + (_grammar & production);
                            }
                        }
                    }

                    // Goto
                    for (int n = 0; n < _nonterminalCol.Count; ++n)
                    {
                        foreach (GotoEntry gotoEntity in _gotoTable)
                        {
                            if (gotoEntity.X == _nonterminalCol[n] && gotoEntity.I == _closureCol[c])
                            {
                                _tableForm[(terminalLength + 0) + n, c] = gotoEntity.Goto.Title.Split('[', ']')[1];
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private String GotoTitle()
        {
            return "I[" + _gotoCount + "]";
        }

        public String Cloures_GoToTable()
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.AppendLine(_closureCol.ToString());
            sbTable.AppendLine(SEPARATOR);
            sbTable.AppendLine("Done.... " + (_gotoCount - 1) + " states achieved. " + _dotCount + " Dots");

            for (int index = 0; index < _gotoTable.Length; ++index)
            {
                sbTable.AppendLine(_gotoTable[index].ToString());
            }
            sbTable.Append(SEPARATOR);
            return sbTable.ToString();
        }

        public String LALRTable()
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.AppendLine();
            sbTable.Append("\t");

            foreach (Terminal terminal in _terminalCol.List)
            {
                sbTable.Append("◄ " + terminal + " ►\t");
            }
            foreach (NonTerminal nonTerminal in _nonterminalCol.List)
            {
                sbTable.Append("◄ " + nonTerminal + " ►\t");
            }
            for (int c = 0; c < _closureCol.Count; ++c)
            {
                sbTable.AppendLine();
                sbTable.Append(c);
                for (int e = 0; e < _entityColO.Count; ++e) // +1 for '$'
                {
                    //sbSLRTable.Append("\t " + (String.IsNullOrEmpty(_tableForm[ e, c ]) ? "." : _tableForm[ e, c ]));
                    sbTable.Append(String.Format("\t  {0} ", (String.IsNullOrEmpty(_tableForm[e, c]) ? "." : _tableForm[e, c])));
                }
            }
            sbTable.AppendLine();
            sbTable.Append(SEPARATOR);
            return sbTable.ToString();
        }
    }

    public class CLRParser : Parser
    {
        //ClosureCol & GoTo
        private CLRProduction _prods;

        private ClosureCollection _closureCol;
        public ClosureCollection ClosureCol { get { return _closureCol; } }

        private int _gotoCount;
        private int _dotCount;

        private GotoEntry[] _gotoTable;
        private String[,] _tableForm;

        public CLRParser(String filename) : base(filename) { }

        public CLRParser(params String[] grmProductions) : base(grmProductions) { }

        public override void Extract()
        {
            try
            {
                _entityCol = _grammar.Entities;
                _terminalCol = _grammar.Terminals;
                _nonterminalCol = _grammar.NonTerminals;

                // removing augmented entity
                Entity augment = _entityCol[0];
                _entityColO = _entityCol - augment;
                _nonterminalCol.Remove((NonTerminal)augment);

                //Cloure_GoToTable
                _prods = new CLRProduction(_grammar[0].Producer, _grammar[0].Product, new EntityCollection<Terminal>(new Terminal[] { (Terminal)"$" }));

                _gotoCount = 0;
                _dotCount = 0;

                _closureCol = new ClosureCollection(new Closure[] { new CLRClosure(GotoTitle(), _grammar, new SLRProduction[] { _prods }) });
                
                ++_gotoCount;
                _gotoTable = new GotoEntry[0];
                for (int c = 0; c < _closureCol.Count; ++c)
                {
                    SLRClosure closure = (SLRClosure)_closureCol[c];
                    EntityCollection<Entity> tmp = _entityColO - (Terminal)"$";
                    for (int e = 0; e < tmp.Count; ++c)
                    //foreach( Entity entity in _entityColO - (Terminal) "$" )
                    {
                        Entity entity = tmp[e];
                        Closure gotoClosure = closure.GoToEntity(entity);
                        if (!gotoClosure.IsNull && gotoClosure.Count != 0)
                        {
                            int index =
                                //_closureCol.List.FindIndex((Closure clr) => (clr == gotoClosure));
                                _closureCol.List.IndexOf(gotoClosure);
                            if (index == -1)
                            {
                                // add new Goto State
                                gotoClosure.Title = GotoTitle();
                                _closureCol += gotoClosure;
                                ++_gotoCount;
                            }
                            else
                            {
                                // error here
                                //CLRClosure closureC = gotoClosure as CLRClosure;
                                //closureC.AddLookAhead(_closureCol[ index ].SLRProductions.ToArray());
                                //gotoClosure = closureC;


                                CLRClosure closureC = gotoClosure as CLRClosure;
                                closureC.AddLookAhead(_closureCol[index].SLRProductions.ToArray());
                                gotoClosure.Title = _closureCol[index].Title;

                            }
                            int length = _gotoTable.Length;
                            GotoEntry[] newTable = new GotoEntry[length + 1];
                            for (int g = 0; g < length; ++g)
                            {
                                newTable[g] = _gotoTable[g];
                            }
                            newTable[length] = new GotoEntry(closure, entity, gotoClosure);
                            _gotoTable = newTable;
                        }
                        ++_dotCount;
                    }
                }

                // LR Table
                _tableForm = new String[_entityCol.Count, _closureCol.Count];

                for (int c = 0; c < _closureCol.Count; ++c)
                {
                    int terminalLength = _terminalCol.Count;
                    //Shift
                    for (int t = 0; t < terminalLength; ++t)
                    {
                        foreach (GotoEntry gotoEntity in _gotoTable)
                        {
                            if (gotoEntity.X == _terminalCol[t] && gotoEntity.I == _closureCol[c])
                            {
                                _tableForm[t, c] = "s" + gotoEntity.Goto.Title.Split('[', ']')[1];
                                break;
                            }
                        }
                    }

                    //Reduce
                    for (int p = 0; p < _closureCol[c].Count; ++p)
                    {
                        SLRProduction production = _closureCol[c][p];
                        if (production.DotPosition == (production.Count - 1)
                        && production == _closureCol[1][0] // S' --> S .$
                          )  // Accepted
                        {
                            _tableForm[_terminalCol & (Terminal)"$", 1] = "!!";
                        }
                        if (production.DotPosition == production.Count)
                        {
                            EntityCollection<Terminal> followCol = _grammar.Follow(production.Producer);
                            if (followCol == default(EntityCollection<Terminal>)) continue;
                            //followCol.Remove( (Terminal)"$" );
                            foreach (Entity follow in followCol)
                            {
                                _tableForm[_terminalCol & follow, c] = "r" + (_grammar & production);
                            }
                        }
                    }

                    // Goto
                    for (int n = 0; n < _nonterminalCol.Count; ++n)
                    {
                        foreach (GotoEntry gotoEntity in _gotoTable)
                        {
                            if (gotoEntity.X == _nonterminalCol[n] && gotoEntity.I == _closureCol[c])
                            {
                                _tableForm[(terminalLength + 0) + n, c] = gotoEntity.Goto.Title.Split('[', ']')[1];
                            }
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private String GotoTitle()
        {
            return "I[" + _gotoCount + "]";
        }

        public String Cloures_GoToTable()
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.AppendLine(_closureCol.ToString());
            sbTable.AppendLine(SEPARATOR);
            sbTable.AppendLine("Done.... " + (_gotoCount - 1) + " states achieved. " + _dotCount + " Dots");
            for (int index = 0; index < _gotoTable.Length; ++index)
            {
                sbTable.AppendLine(_gotoTable[index].ToString());
            }
            sbTable.Append(SEPARATOR);
            return sbTable.ToString();
        }

        public String LALRTable()
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.AppendLine();
            sbTable.Append("\t");

            foreach (Terminal terminal in _terminalCol.List)
            {
                sbTable.Append("◄ " + terminal + " ►\t");
            }
            foreach (NonTerminal nonTerminal in _nonterminalCol.List)
            {
                sbTable.Append("◄ " + nonTerminal + " ►\t");
            }
            for (int c = 0; c < _closureCol.Count; ++c)
            {
                sbTable.AppendLine();
                sbTable.Append(c);
                for (int e = 0; e < _entityColO.Count; ++e) // +1 for '$'
                {
                    //sbSLRTable.Append("\t " + (String.IsNullOrEmpty(_tableForm[ e, c ]) ? "." : _tableForm[ e, c ]));
                    sbTable.Append(String.Format("\t  {0} ", (String.IsNullOrEmpty(_tableForm[e, c]) ? "." : _tableForm[e, c])));
                }
            }
            sbTable.AppendLine();
            sbTable.Append(SEPARATOR);
            return sbTable.ToString();
        }
    }
}