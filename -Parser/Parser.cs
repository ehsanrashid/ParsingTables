using System.Linq;

namespace Parser
{
    using System;
    using System.Text;

    public abstract class Parser
    {
        protected Grammar _grammar;
        public Grammar Grammar
        {
            get { return _grammar; }
            set
            {
                _grammar = value;
                Extract();
            }
        }

        public EntityCollection<Entity> EntityCol { get; protected set; }

        public EntityCollection<Terminal> TerminalCol { get; protected set; }

        public EntityCollection<NonTerminal> NonTerminalCol { get; protected set; }

        //  un-augmented grammar entities
        protected EntityCollection<Entity> _entityColO;


        protected Parser(String filename)
        {
            Grammar = Grammar.Read(filename);
        }

        protected Parser(params String[] grmProductions)
        {
            Grammar = Grammar.Read(grmProductions);
        }

        public const String SEPARATOR = "________________________________________";

        public String FirstnFollow()
        {
            var sbFF = new StringBuilder();
            foreach (var entity in _entityColO)
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
                EntityCol = _grammar.Entities;
                TerminalCol = _grammar.Terminals;
                NonTerminalCol = _grammar.NonTerminals;

                // removing augmented entity
                var augment = EntityCol[0];
                _entityColO = EntityCol - augment;
                NonTerminalCol.Remove((NonTerminal) augment);

                //Cloure_GoToTable
                _prods = new SLRProduction(_grammar[0].Producer, _grammar[0].Product);

                _gotoCount = 0;
                _dotCount = 0;

                _closureCol = new ClosureCollection(new Closure[] { new SLRClosure(GotoTitle(), _grammar, new[] { _prods }) });

                ++_gotoCount;
                for (int c = 0; c < _closureCol.Count; ++c)
                {
                    var closure = (SLRClosure) _closureCol[c];
                    foreach (var entity in _entityColO - (Terminal) "$")
                    {
                        var gotoClosure = closure.GoToEntity(entity);
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
                            var newTable = new GotoEntry[length + 1];
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
                _tableForm = new String[EntityCol.Count, _closureCol.Count];

                for (var c = 0; c < _closureCol.Count; ++c)
                {
                    var terminalLength = TerminalCol.Count;

                    //Shift
                    for (var t = 0; t < terminalLength; ++t)
                    {
                        foreach (GotoEntry gotoEntity in _gotoTable)
                        {
                            if (gotoEntity.X == TerminalCol[t] && gotoEntity.I == _closureCol[c])
                            {
                                _tableForm[t, c] = "s" + gotoEntity.Goto.Title.Split('[', ']')[1];
                                break;
                            }
                        }
                    }

                    //Reduce
                    for (var p = 0; p < _closureCol[c].Count; ++p)
                    {
                        var production = _closureCol[c][p];
                        if (production.DotPosition == (production.Count - 1)
                        && production == _closureCol[1][0] // S' --> S .$
                          )  // Accepted
                        {
                            _tableForm[TerminalCol & (Terminal) "$", 1] = "!!";
                        }
                        if (production.DotPosition == production.Count)
                        {
                            var followCol = _grammar.Follow(production.Producer);
                            if (followCol == default(EntityCollection<Terminal>)) continue;
                            //followCol.Remove( (Terminal)"$" );
                            foreach (var follow in followCol)
                            {
                                _tableForm[TerminalCol & follow, c] = "r" + (_grammar & production);
                            }
                        }
                    }

                    // Goto
                    for (int n = 0; n < NonTerminalCol.Count; ++n)
                    {
                        foreach (GotoEntry gotoEntity in _gotoTable)
                        {
                            if (gotoEntity.X == NonTerminalCol[n] && gotoEntity.I == _closureCol[c])
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
            var sbTable = new StringBuilder();
            sbTable.AppendLine(_closureCol.ToString());
            sbTable.AppendLine(SEPARATOR);
            sbTable.AppendLine("Done.... " + (_gotoCount - 1) + " states achieved. " + _dotCount + " Dots");

            foreach (var item in _gotoTable)
            {
                sbTable.AppendLine(item.ToString());
            }
            sbTable.Append(SEPARATOR);
            return sbTable.ToString();
        }

        public String LALRTable()
        {
            var sbTable = new StringBuilder();
            sbTable.AppendLine();
            sbTable.Append("\t");

            foreach (var terminal in TerminalCol.List)
            {
                sbTable.Append("◄ " + terminal + " ►\t");
            }
            foreach (var nonTerminal in NonTerminalCol.List)
            {
                sbTable.Append("◄ " + nonTerminal + " ►\t");
            }
            for (var c = 0; c < _closureCol.Count; ++c)
            {
                sbTable.AppendLine();
                sbTable.Append(c);
                for (var e = 0; e < _entityColO.Count; ++e) // +1 for '$'
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

            EntityCol = _grammar.Entities;
            TerminalCol = _grammar.Terminals;
            NonTerminalCol = _grammar.NonTerminals;

            // removing augmented entity
            Entity augment = EntityCol[0];
            _entityColO = EntityCol - augment;
            NonTerminalCol.Remove((NonTerminal) augment);

            //Cloure_GoToTable
            _prods = new CLRProduction(_grammar[0].Producer, _grammar[0].Product, new EntityCollection<Terminal>(new[] { (Terminal) "$" }));

            _gotoCount = 0;
            _dotCount = 0;

            _closureCol = new ClosureCollection(new Closure[] { new CLRClosure(GotoTitle(), _grammar, new SLRProduction[] { _prods }) });

            ++_gotoCount;
            _gotoTable = new GotoEntry[0];
            for (int c = 0; c < _closureCol.Count; ++c)
            {
                var closure = (SLRClosure) _closureCol[c];
                var entityColO = _entityColO - (Terminal) "$";
                foreach (var entity in entityColO)
                {
                    var gotoClosure = closure.GoToEntity(entity);
                    if (!gotoClosure.IsNull && gotoClosure.Count != 0)
                    {
                        var index =
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
                            //var closureC = gotoClosure as CLRClosure;
                            //closureC.AddLookAhead(_closureCol[ index ].SLRProductions.ToArray());
                            //gotoClosure = closureC;


                            var closureC = gotoClosure as CLRClosure;
                            closureC.AddLookAhead(_closureCol[index].SLRProductions.ToArray());
                            gotoClosure.Title = _closureCol[index].Title;

                        }
                        var length = _gotoTable.Length;
                        var newTable = new GotoEntry[length + 1];
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
            _tableForm = new String[EntityCol.Count, _closureCol.Count];

            for (var c = 0; c < _closureCol.Count; ++c)
            {
                int terminalLength = TerminalCol.Count;
                //Shift
                for (var t = 0; t < terminalLength; ++t)
                {
                    foreach (var gotoEntity in _gotoTable)
                    {
                        if (gotoEntity.X == TerminalCol[t] && gotoEntity.I == _closureCol[c])
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
                        _tableForm[TerminalCol & (Terminal) "$", 1] = "!!";
                    }
                    if (production.DotPosition == production.Count)
                    {
                        var followCol = _grammar.Follow(production.Producer);
                        if (followCol == default(EntityCollection<Terminal>)) continue;
                        //followCol.Remove( (Terminal)"$" );
                        foreach (var follow in followCol)
                        {
                            _tableForm[TerminalCol & follow, c] = "r" + (_grammar & production);
                        }
                    }
                }

                // Goto
                for (var n = 0; n < NonTerminalCol.Count; ++n)
                {
                    foreach (var gotoEntity in _gotoTable.Where(gotoEntity => gotoEntity.X == NonTerminalCol[n] && gotoEntity.I == _closureCol[c]))
                    {
                        _tableForm[(terminalLength + 0) + n, c] = gotoEntity.Goto.Title.Split('[', ']')[1];
                    }
                }
            }



        }

        private String GotoTitle()
        {
            return "I[" + _gotoCount + "]";
        }

        public String ClouresGoToTable()
        {
            var sbTable = new StringBuilder();
            sbTable.AppendLine(_closureCol.ToString());
            sbTable.AppendLine(SEPARATOR);
            sbTable.AppendLine("Done.... " + (_gotoCount - 1) + " states achieved. " + _dotCount + " Dots");
            foreach (var item in _gotoTable)
            {
                sbTable.AppendLine(item.ToString());
            }
            sbTable.Append(SEPARATOR);
            return sbTable.ToString();
        }

        public String LALRTable()
        {
            var sbTable = new StringBuilder();
            sbTable.AppendLine();
            sbTable.Append("\t");

            foreach (var terminal in TerminalCol.List)
            {
                sbTable.Append("◄ " + terminal + " ►\t");
            }
            foreach (var nonTerminal in NonTerminalCol.List)
            {
                sbTable.Append("◄ " + nonTerminal + " ►\t");
            }
            for (var c = 0; c < _closureCol.Count; ++c)
            {
                sbTable.AppendLine();
                sbTable.Append(c);
                for (var e = 0; e < _entityColO.Count; ++e) // +1 for '$'
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