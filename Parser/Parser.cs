using System;
using System.Text;

namespace Parser
{
    public abstract class Parser
    {
        public const String SEPARATOR = "________________________________________";
        protected EntityCollection<Entity> _entityCol;

        //  un-augmented grammar entities
        protected EntityCollection<Entity> _entityColO;
        protected Grammar _grammar;
        protected EntityCollection<NonTerminal> _nonterminalCol;
        protected EntityCollection<Terminal> _terminalCol;

        protected Parser(String filename)
        {
            Grammar = Grammar.Read(filename);
        }

        protected Parser(params String[] grmProductions)
        {
            Grammar = Grammar.Read(grmProductions);
        }

        public EntityCollection<Entity> EntityCol
        {
            get { return _entityCol; }
        }

        public EntityCollection<Terminal> TerminalCol
        {
            get { return _terminalCol; }
        }

        public EntityCollection<NonTerminal> NonTerminalCol
        {
            get { return _nonterminalCol; }
        }

        public Grammar Grammar
        {
            get { return _grammar; }
            set
            {
                if (default(Grammar) != value)
                {
                    Extract(value);
                }
                else
                {
                    _grammar = default(Grammar);
                }
            }
        }

        public String First_Follow()
        {
            var sb = new StringBuilder();
            foreach (var entity in _entityColO)
            {
                // if grammar is not left recursive
                sb.AppendLine(String.Format("FIRST {0} : {1}", entity, _grammar.First(entity)));
                if (entity is NonTerminal)
                    sb.AppendLine(String.Format("FOLLOW {0} : {1}", entity, _grammar.Follow(entity)));
                sb.AppendLine("------");
            }
            sb.Append(SEPARATOR);
            return sb.ToString();
        }

        public abstract void Extract(Grammar grammar);
    }

    public class SLRParser : Parser
    {
        //ClosureCol & GoTo

        private ClosureCollection _closureCol;

        private int _dotCount;
        private int _gotoCount;

        private GotoEntry[] _gotoTable;
        private SLRProduction _prods;
        private String[,] _tableForm;

        public SLRParser(String filename)
            : base(filename)
        {
        }

        public SLRParser(params String[] grmProductions)
            : base(grmProductions)
        {
        }

        public ClosureCollection ClosureCol
        {
            get { return _closureCol; }
        }

        public override void Extract(Grammar grammar)
        {
            try
            {
                _grammar = grammar;
                _entityCol = grammar.Entities;
                _terminalCol = grammar.Terminals;
                _nonterminalCol = grammar.NonTerminals;

                // removing augmented entity
                var augment = _entityCol[0];
                _entityColO = _entityCol - augment;

                //_nonterminalCol.Remove((NonTerminal) augment);
                _nonterminalCol = _nonterminalCol - augment;

                //Cloure_GoToTable
                _prods = new SLRProduction(grammar[0].Producer, grammar[0].Product);

                _gotoCount = 0;
                _dotCount = 0;

                _closureCol = new ClosureCollection(new Closure[] { new SLRClosure(GotoTitle(), grammar, new[] { _prods }) });

                // TODO:: closure production is wrong
                ++_gotoCount;
                _gotoTable = new GotoEntry[0];
                for (var c = 0; c < _closureCol.Count; ++c)
                {
                    var closure = (SLRClosure) _closureCol[c];
                    foreach (var entity in _entityColO - (Terminal) "$")
                    {
                        var gotoClosure = closure.GoToEntity(entity);
                        if (!gotoClosure.IsEmpty)
                        {
                            var index =
                                //_closureCol.List.FindIndex((Closure clr) => (clr == gotoClosure));
                                _closureCol.Closures.IndexOf(gotoClosure);
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

                            var length = (_gotoTable == default(GotoEntry[])) ? 0 : _gotoTable.Length;
                            var newGotoTable = new GotoEntry[length + 1];

                            //for (var g = 0; g < length; ++g) newGotoTable[g] = _gotoTable[g];
                            Array.Copy(_gotoTable, newGotoTable, length);

                            newGotoTable[length] = new GotoEntry(closure, entity, gotoClosure);
                            _gotoTable = newGotoTable;
                        }
                        ++_dotCount;
                    }
                }

                // LR Table
                _tableForm = new String[_entityCol.Count, _closureCol.Count];

                for (var c = 0; c < _closureCol.Count; ++c)
                {
                    var terminalLength = _terminalCol.Count;

                    //Shift
                    for (var t = 0; t < terminalLength; ++t)
                        foreach (var gotoEntity in _gotoTable)
                        {
                            if (gotoEntity.X != _terminalCol[t] || gotoEntity.I != _closureCol[c]) continue;

                            _tableForm[t, c] = "s" + gotoEntity.Goto.Title.Split('[', ']')[1];
                            break;
                        }

                    //Reduce
                    //for (int p = 0; p < _closureCol[c].Count; ++p)
                    foreach (SLRProduction production in _closureCol[c])
                    {
                        //SLRProduction production = _closureCol[c][p];
                        if (production.DotPosition == (production.Count - 1)
                            && production == _closureCol[1][0] // S' --> S .$
                            ) // Accepted
                        {
                            _tableForm[_terminalCol & (Terminal) "$", 1] = "!!";
                        }

                        if (production.DotPosition != production.Count) continue;

                        var followCol = grammar.Follow(production.Producer);
                        if (null == followCol) continue;

                        //followCol.Remove( (Terminal)"$" );
                        foreach (var follow in followCol)
                            _tableForm[_terminalCol & follow, c] = "r" + (grammar & production);
                    }

                    // Goto
                    for (var n = 0; n < _nonterminalCol.Count; ++n)
                        foreach (var gotoEntity in _gotoTable)
                            if (gotoEntity.X == _nonterminalCol[n] && gotoEntity.I == _closureCol[c])
                                _tableForm[(terminalLength + 0) + n, c] = gotoEntity.Goto.Title.Split('[', ']')[1];
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp);
            }
        }

        private String GotoTitle()
        {
            return "I[" + _gotoCount + "]";
        }

        public String Cloures_GoToTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_closureCol.ToString());
            sb.AppendLine(SEPARATOR);
            sb.AppendLine("Done.... " + (_gotoCount - 1) + " states achieved. " + _dotCount + " Dots");

            foreach (var gotoE in _gotoTable)
                sb.AppendLine(gotoE.ToString());

            sb.Append(SEPARATOR);
            return sb.ToString();
        }

        public String LALRTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("\t");

            foreach (var terminal in _terminalCol.Entities)
                sb.Append(String.Concat("◄ ", terminal, " ►\t"));
            foreach (var nonTerminal in _nonterminalCol.Entities)
                sb.Append(String.Concat("◄ ", nonTerminal, " ►\t"));
            for (var c = 0; c < _closureCol.Count; ++c)
            {
                sb.AppendLine();
                sb.Append(c);
                for (var e = 0; e < _entityColO.Count; ++e) // +1 for '$'
                {
                    //sbSLRTable.Append("\t " + (String.IsNullOrEmpty(_tableForm[ e, c ]) ? "." : _tableForm[ e, c ]));
                    sb.Append(String.Format("\t  {0} ", (String.IsNullOrEmpty(_tableForm[e, c]) ? "." : _tableForm[e, c])));
                }
            }
            sb.AppendLine();
            sb.Append(SEPARATOR);
            return sb.ToString();
        }
    }

    public class CLRParser : Parser
    {
        //ClosureCol & GoTo

        private ClosureCollection _closureCol;

        private int _dotCount;
        private int _gotoCount;

        private GotoEntry[] _gotoTable;
        private CLRProduction _prods;
        private String[,] _tableForm;

        public CLRParser(String filename)
            : base(filename)
        {
        }

        public CLRParser(params String[] grmProductions)
            : base(grmProductions)
        {
        }

        public ClosureCollection ClosureCol
        {
            get { return _closureCol; }
        }

        public override void Extract(Grammar grammar)
        {
            _grammar = grammar;
            _entityCol = grammar.Entities;
            _terminalCol = grammar.Terminals;
            _nonterminalCol = grammar.NonTerminals;

            // removing augmented entity
            var augment = _entityCol[0];
            _entityColO = _entityCol - augment;
            _nonterminalCol.Remove((NonTerminal) augment);

            //Cloure_GoToTable
            _prods = new CLRProduction(grammar[0].Producer, grammar[0].Product, new EntityCollection<Terminal>(new[] { (Terminal) "$" }));

            _gotoCount = 0;
            _dotCount = 0;

            _closureCol = new ClosureCollection(new Closure[] { new CLRClosure(GotoTitle(), grammar, new SLRProduction[] { _prods }) });

            ++_gotoCount;
            _gotoTable = new GotoEntry[0];
            for (var c = 0; c < _closureCol.Count; ++c)
            {
                var closure = (SLRClosure) _closureCol[c];
                var tmp = _entityColO - (Terminal) "$";
                for (var e = 0; e < tmp.Count; ++e)
                //foreach (var entity in _entityColO - (Terminal) "$")
                {
                    var entity = tmp[e];
                    var gotoClosure = closure.GoToEntity(entity);
                    if (!gotoClosure.IsEmpty)
                    {
                        var index =
                            //_closureCol.List.FindIndex((Closure clr) => (clr == gotoClosure));
                            _closureCol.Closures.IndexOf(gotoClosure);
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

                            var closureC = gotoClosure as CLRClosure;
                            if (null != closureC) closureC.AddLookAhead(_closureCol[index].SLRProductions.ToArray());
                            gotoClosure.Title = _closureCol[index].Title;
                        }

                        var length = _gotoTable.Length;
                        var newTable = new GotoEntry[length + 1];
                        Array.Copy(_gotoTable, newTable, length);

                        newTable[length] = new GotoEntry(closure, entity, gotoClosure);
                        _gotoTable = newTable;
                    }
                    ++_dotCount;
                }
            }

            // LR Table
            _tableForm = new String[_entityCol.Count, _closureCol.Count];

            for (var c = 0; c < _closureCol.Count; ++c)
            {
                var terminalLength = _terminalCol.Count;

                //Shift
                for (var t = 0; t < terminalLength; ++t)
                    foreach (var gotoEntity in _gotoTable)
                    {
                        if (gotoEntity.X != _terminalCol[t] || gotoEntity.I != _closureCol[c]) continue;

                        _tableForm[t, c] = "s" + gotoEntity.Goto.Title.Split('[', ']')[1];
                        break;
                    }

                //Reduce
                for (var p = 0; p < _closureCol[c].Count; ++p)
                {
                    var production = _closureCol[c][p];
                    if (production.DotPosition == (production.Count - 1)
                        && production == _closureCol[1][0] // S' --> S .$
                        ) // Accepted
                    {
                        _tableForm[_terminalCol & (Terminal) "$", 1] = "!!";
                    }

                    if (production.DotPosition != production.Count) continue;
                    var followCol = grammar.Follow(production.Producer);
                    if (default(EntityCollection<Terminal>) == followCol) continue;
                    //followCol.Remove( (Terminal)"$" );
                    foreach (var follow in followCol)
                        _tableForm[_terminalCol & follow, c] = "r" + (grammar & production);
                }

                // Goto
                for (var n = 0; n < _nonterminalCol.Count; ++n)
                    foreach (var gotoEntity in _gotoTable)
                        if (gotoEntity.X == _nonterminalCol[n] && gotoEntity.I == _closureCol[c])
                            _tableForm[(terminalLength + 0) + n, c] = gotoEntity.Goto.Title.Split('[', ']')[1];
            }
        }

        private String GotoTitle()
        {
            return "I[" + _gotoCount + "]";
        }

        public String Cloures_GoToTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_closureCol.ToString());
            sb.AppendLine(SEPARATOR);
            sb.AppendLine("Done.... " + (_gotoCount - 1) + " states achieved. " + _dotCount + " Dots");
            for (var index = 0; index < _gotoTable.Length; ++index)
                sb.AppendLine(_gotoTable[index].ToString());
            sb.Append(SEPARATOR);
            return sb.ToString();
        }

        public String LALRTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("\t");

            foreach (var terminal in _terminalCol.Entities)
                sb.Append("◄ " + terminal + " ►\t");
            foreach (var nonTerminal in _nonterminalCol.Entities)
                sb.Append("◄ " + nonTerminal + " ►\t");
            for (var c = 0; c < _closureCol.Count; ++c)
            {
                sb.AppendLine();
                sb.Append(c);
                for (var e = 0; e < _entityColO.Count; ++e) // +1 for '$'
                {
                    //sbSLRTable.Append("\t " + (String.IsNullOrEmpty(_tableForm[ e, c ]) ? "." : _tableForm[ e, c ]));
                    sb.Append(String.Format("\t  {0} ",
                                            (String.IsNullOrEmpty(_tableForm[e, c]) ? "." : _tableForm[e, c])));
                }
            }
            sb.AppendLine();
            sb.Append(SEPARATOR);
            return sb.ToString();
        }
    }
}