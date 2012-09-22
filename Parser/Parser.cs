using System;
using System.Text;

namespace Parser
{
    public abstract class Parser
    {
        public const String SEPARATOR = "________________________________________";

        protected Grammar _Grammar;
        public Grammar Grammar
        {
            get { return _Grammar; }
            set
            {
                if (default(Grammar) != value) Extract(value);
                else _Grammar = default(Grammar);
            }
        }

        //  un-augmented grammar entities
        protected EntityCollection<Entity> _EntityColO;

        public EntityCollection<Entity> EntityCol { get; protected set; }

        public EntityCollection<Terminal> TerminalCol { get; protected set; }

        public EntityCollection<NonTerminal> NonTerminalCol { get; protected set; }

        protected Parser(String filename) { Grammar = Grammar.Read(filename); }

        protected Parser(params String[] grmProductions) { Grammar = Grammar.Read(grmProductions); }

        public String FirstnFollow()
        {
            var sb = new StringBuilder();
            foreach (var entity in _EntityColO)
            {
                // if grammar is not left recursive
                sb.AppendLine(String.Format("FIRST {0} : {1}", entity, _Grammar.First(entity)));
                if (entity is NonTerminal) sb.AppendLine(String.Format("FOLLOW {0} : {1}", entity, _Grammar.Follow(entity)));
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

        int _DotCount;
        int _GotoCount;

        GotoEntry[] _GotoEntries;
        SLRProduction _Production;
        String[,] _GoToTable;

        public SLRParser(String filename)
            : base(filename) { }

        public SLRParser(params String[] grmProductions)
            : base(grmProductions) { }

        public ClosureCollection ClosureCol { get; private set; }

        public override void Extract(Grammar grammar)
        {
            try
            {
                _Grammar = grammar;
                EntityCol = grammar.Entities;
                TerminalCol = grammar.Terminals;
                NonTerminalCol = grammar.NonTerminals;

                // removing augmented entity
                var augment = EntityCol[0];
                _EntityColO = EntityCol - augment;

                //_nonterminalCol.Remove((NonTerminal) augment);
                NonTerminalCol = NonTerminalCol - augment;

                //Cloure_GoToTable
                _Production = new SLRProduction(grammar[0].Producer, grammar[0].Product);

                _GotoCount = 0;
                _DotCount = 0;

                ClosureCol = new ClosureCollection(new Closure[] { new SLRClosure(GotoTitle(), grammar, new[] { _Production }) });

                // TODO:: closure production is wrong
                ++_GotoCount;
                _GotoEntries = new GotoEntry[0];
                for (var c = 0; c < ClosureCol.Count; ++c)
                {
                    var closure = (SLRClosure) ClosureCol[c];
                    foreach (var entity in _EntityColO - (Terminal) "$")
                    {
                        var gotoClosure = closure.GoToEntity(entity);
                        if (!gotoClosure.IsEmpty)
                        {
                            var index =
                                //_closureCol.List.FindIndex((Closure clr) => (clr == gotoClosure));
                                ClosureCol.Closures.IndexOf(gotoClosure);
                            if (index == -1)
                            {
                                // add new Goto State
                                gotoClosure.Title = GotoTitle();
                                ClosureCol += gotoClosure;
                                ++_GotoCount;
                            }
                            else gotoClosure = ClosureCol[index];

                            var length = (_GotoEntries == default(GotoEntry[])) ? 0 : _GotoEntries.Length;
                            var newGotoEntries = new GotoEntry[length + 1];

                            //for (var g = 0; g < length; ++g) newGotoTable[g] = _gotoTable[g];
                            Array.Copy(_GotoEntries, newGotoEntries, length);

                            newGotoEntries[length] = new GotoEntry(closure, entity, gotoClosure);
                            _GotoEntries = newGotoEntries;
                        }
                        ++_DotCount;
                    }
                }

                // LR Table
                _GoToTable = new String[EntityCol.Count, ClosureCol.Count];

                for (var c = 0; c < ClosureCol.Count; ++c)
                {
                    var terminalLength = TerminalCol.Count;

                    //Shift
                    for (var t = 0; t < terminalLength; ++t)
                        foreach (var gotoEntity in _GotoEntries)
                        {
                            if (gotoEntity.X != TerminalCol[t] || gotoEntity.I != ClosureCol[c]) continue;

                            _GoToTable[t, c] = "s" + gotoEntity.Goto.Title.Split('[', ']')[1];
                            break;
                        }

                    //Reduce
                    //for (int p = 0; p < _closureCol[c].Count; ++p)
                    foreach (SLRProduction production in ClosureCol[c])
                    {
                        //SLRProduction production = _closureCol[c][p];
                        if (production.DotPosition == (production.Count - 1)
                            && production == ClosureCol[1][0] // S' --> S .$
                            ) // Accepted
                            _GoToTable[TerminalCol & (Terminal) "$", 1] = "!!";

                        if (production.DotPosition != production.Count) continue;

                        var followCol = grammar.Follow(production.Producer);
                        if (default(EntityCollection<Entity>) == followCol) continue;

                        //followCol.Remove( (Terminal)"$" );
                        foreach (var follow in followCol)
                            _GoToTable[TerminalCol & follow, c] = "r" + (grammar & production);
                    }

                    // Goto
                    for (var n = 0; n < NonTerminalCol.Count; ++n)
                        foreach (var gotoEntity in _GotoEntries)
                            if (gotoEntity.X == NonTerminalCol[n] && gotoEntity.I == ClosureCol[c])
                                _GoToTable[(terminalLength + 0) + n, c] = gotoEntity.Goto.Title.Split('[', ']')[1];
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp);
            }
        }

        String GotoTitle() { return String.Concat("I[", _GotoCount, "]"); }

        public String ClouresGoToTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine(ClosureCol.ToString());
            sb.AppendLine(SEPARATOR);
            sb.AppendLine("Done.... " + (_GotoCount - 1) + " states achieved. " + _DotCount + " Dots");

            foreach (var gotoE in _GotoEntries)
                sb.AppendLine(gotoE.ToString());

            sb.Append(SEPARATOR);
            return sb.ToString();
        }

        public String LALRTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("\t");

            foreach (var terminal in TerminalCol.Entities)
                sb.Append(String.Concat("◄ ", terminal, " ►\t"));
            foreach (var nonTerminal in NonTerminalCol.Entities)
                sb.Append(String.Concat("◄ ", nonTerminal, " ►\t"));
            for (var c = 0; c < ClosureCol.Count; ++c)
            {
                sb.AppendLine();
                sb.Append(c);
                for (var e = 0; e < _EntityColO.Count; ++e) // +1 for '$'
                    //sbSLRTable.Append("\t " + (String.IsNullOrEmpty(_tableForm[ e, c ]) ? "." : _tableForm[ e, c ]));
                    sb.Append(String.Format("\t  {0} ",
                            (String.IsNullOrEmpty(_GoToTable[e, c]) ? "." : _GoToTable[e, c])));
            }
            sb.AppendLine();
            sb.Append(SEPARATOR);
            return sb.ToString();
        }
    }

    public class CLRParser : Parser
    {
        //ClosureCol & GoTo

        ClosureCollection _ClosureCol;

        int _DotCount;
        int _GotoCount;

        GotoEntry[] _GotoEntries;
        CLRProduction _Production;
        String[,] _GoTotable;

        public CLRParser(String filename)
            : base(filename) { }

        public CLRParser(params String[] grmProductions)
            : base(grmProductions) { }

        public ClosureCollection ClosureCol
        {
            get { return _ClosureCol; }
        }

        public override void Extract(Grammar grammar)
        {
            _Grammar = grammar;
            EntityCol = grammar.Entities;
            TerminalCol = grammar.Terminals;
            NonTerminalCol = grammar.NonTerminals;

            // removing augmented entity
            var augment = EntityCol[0];
            _EntityColO = EntityCol - augment;
            NonTerminalCol.Remove((NonTerminal) augment);

            //Cloure_GoToTable
            _Production = new CLRProduction(grammar[0].Producer, grammar[0].Product,
                                       new EntityCollection<Terminal>(new[] { (Terminal) "$" }));

            _GotoCount = 0;
            _DotCount = 0;

            _ClosureCol =
                new ClosureCollection(new Closure[] { new CLRClosure(GotoTitle(), grammar, new SLRProduction[] { _Production }) });

            ++_GotoCount;
            _GotoEntries = new GotoEntry[0];
            for (var c = 0; c < _ClosureCol.Count; ++c)
            {
                var closure = (SLRClosure) _ClosureCol[c];
                var tmp = _EntityColO - (Terminal) "$";
                for (var e = 0; e < tmp.Count; ++e) //foreach (var entity in _entityColO - (Terminal) "$")
                {
                    var entity = tmp[e];
                    var gotoClosure = closure.GoToEntity(entity);
                    if (!gotoClosure.IsEmpty)
                    {
                        var index =
                            //_closureCol.List.FindIndex((Closure clr) => (clr == gotoClosure));
                            _ClosureCol.Closures.IndexOf(gotoClosure);
                        if (-1 == index)
                        {
                            // add new Goto State
                            gotoClosure.Title = GotoTitle();
                            _ClosureCol += gotoClosure;
                            ++_GotoCount;
                        }
                        else
                        {
                            // error here
                            //CLRClosure closureC = gotoClosure as CLRClosure;
                            //closureC.AddLookAhead(_ClosureCol[ index ].SLRProductions.ToArray());
                            //gotoClosure = closureC;

                            var closureC = gotoClosure as CLRClosure;
                            if (default(Closure) != closureC) closureC.AddLookAhead(_ClosureCol[index].SLRProductions.ToArray());
                            gotoClosure.Title = _ClosureCol[index].Title;
                        }

                        var length = _GotoEntries.Length;
                        var newGotoEntries = new GotoEntry[length + 1];
                        Array.Copy(_GotoEntries, newGotoEntries, length);

                        newGotoEntries[length] = new GotoEntry(closure, entity, gotoClosure);
                        _GotoEntries = newGotoEntries;
                    }
                    ++_DotCount;
                }
            }

            // LR Table
            _GoTotable = new String[EntityCol.Count, _ClosureCol.Count];

            for (var c = 0; c < _ClosureCol.Count; ++c)
            {
                var terminalLength = TerminalCol.Count;

                //Shift
                for (var t = 0; t < terminalLength; ++t)
                    foreach (var gotoEntity in _GotoEntries)
                    {
                        if (gotoEntity.X != TerminalCol[t] || gotoEntity.I != _ClosureCol[c]) continue;

                        _GoTotable[t, c] = "s" + gotoEntity.Goto.Title.Split('[', ']')[1];
                        break;
                    }

                //Reduce
                for (var p = 0; p < _ClosureCol[c].Count; ++p)
                {
                    var production = _ClosureCol[c][p];
                    if (production.DotPosition == (production.Count - 1)
                        && production == _ClosureCol[1][0] // S' --> S .$
                        ) // Accepted
                        _GoTotable[TerminalCol & (Terminal) "$", 1] = "!!";

                    if (production.DotPosition != production.Count) continue;
                    var followCol = grammar.Follow(production.Producer);
                    if (default(EntityCollection<Terminal>) == followCol) continue;
                    //followCol.Remove( (Terminal)"$" );
                    foreach (var follow in followCol) _GoTotable[TerminalCol & follow, c] = "r" + (grammar & production);
                }

                // Goto
                for (var n = 0; n < NonTerminalCol.Count; ++n)
                    foreach (var gotoEntity in _GotoEntries)
                        if (gotoEntity.X == NonTerminalCol[n] && gotoEntity.I == _ClosureCol[c])
                            _GoTotable[(terminalLength + 0) + n, c] = gotoEntity.Goto.Title.Split('[', ']')[1];
            }
        }

        String GotoTitle() { return "I[" + _GotoCount + "]"; }

        public String ClouresGoToTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_ClosureCol.ToString());
            sb.AppendLine(SEPARATOR);
            sb.AppendLine("Done.... " + (_GotoCount - 1) + " states achieved. " + _DotCount + " Dots");
            for (var index = 0; index < _GotoEntries.Length; ++index) sb.AppendLine(_GotoEntries[index].ToString());
            sb.Append(SEPARATOR);
            return sb.ToString();
        }

        public String LALRTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("\t");

            foreach (var terminal in TerminalCol.Entities) sb.Append("◄ " + terminal + " ►\t");
            foreach (var nonTerminal in NonTerminalCol.Entities) sb.Append("◄ " + nonTerminal + " ►\t");
            for (var c = 0; c < _ClosureCol.Count; ++c)
            {
                sb.AppendLine();
                sb.Append(c);
                for (var e = 0; e < _EntityColO.Count; ++e) // +1 for '$'
                    //sbSLRTable.Append("\t " + (String.IsNullOrEmpty(_tableForm[ e, c ]) ? "." : _tableForm[ e, c ]));
                    sb.Append(String.Format("\t  {0} ",
                             (String.IsNullOrEmpty(_GoTotable[e, c]) ? "." : _GoTotable[e, c])));
            }
            sb.AppendLine();
            sb.Append(SEPARATOR);
            return sb.ToString();
        }
    }
}