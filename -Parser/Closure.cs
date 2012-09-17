namespace Parser
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public abstract class Closure : IEnumerable, ICloneable
    {
        public Closure(String title, Grammar grammar)
        {
            _title = title;
            _grammar = grammar;
            _productions = new List<SLRProduction>();
        }

        protected String _title;
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        protected Grammar _grammar;
        public Grammar Grammar { get { return _grammar; } }

        protected List<SLRProduction> _productions;
        public List<SLRProduction> SLRProductions { get { return _productions; } }

        [IndexerName("SLRProduction")]
        public SLRProduction this[int index] { get { return _productions[index]; } }
        
        public int Count { get { return (_productions == default(List<SLRProduction>)) ? 0 : _productions.Count; } }

        public bool IsNull { get { return Count == 0; } }

        public void Add(SLRProduction production)
        {
            if (production != default(CLRProduction))
            {
                _productions.Add(production);
            }
        }

        public void AddRange(IEnumerable<SLRProduction> productions)
        {
            if (productions != default(IEnumerable<CLRProduction>))
            {
                _productions.AddRange(productions);
            }
        }

        public int FindFirstIndex(SLRProduction find, int startIndex)
        {
            return _productions.FindIndex(startIndex, delegate(SLRProduction production) { return (production == find); });
        }
        public int FindFirstIndex(CLRProduction find) { return FindFirstIndex(find, 0); }

        public void RemoveAt(int index) { _productions.RemoveAt(index); }

        public void Remove(CLRProduction production) { RemoveAt(FindFirstIndex(production)); }

        public void RemoveRedundancy()
        {
            int count = Count - 1;
            for (int index = 0; index < count; )
            {
                int findIdx = FindFirstIndex(this[index], index + 1);
                if (findIdx != -1)
                {
                    RemoveAt(findIdx);
                    --count;
                    continue;
                }
                ++index;
            }
        }

        public SLRProduction[] ToArray()
        {
            return _productions.ToArray();
        }

        public EntityCollection<Terminal> Terminals
        {
            get
            {
                EntityCollection<Terminal> arrTerms = default(EntityCollection<Terminal>);
                foreach (Production production in this)
                {
                    foreach (Entity entity in production.Product)
                    {
                        //if (entity is Terminal)
                        {
                            arrTerms += entity as Terminal;
                        }
                    }
                }
                return arrTerms.RemoveRedundancy() as EntityCollection<Terminal>;
            }
        }

        public EntityCollection<NonTerminal> NonTerminals
        {
            get
            {
                EntityCollection<NonTerminal> arrNonTerms = default(EntityCollection<NonTerminal>);
                foreach (Production production in this)
                {
                    foreach (Entity entity in production.Product)
                    {
                        //if (entity is NonTerminal)
                        {
                            arrNonTerms += entity as NonTerminal;
                        }
                    }
                }
                return arrNonTerms.RemoveRedundancy() as EntityCollection<NonTerminal>; 
            }
        }

        public EntityCollection<Entity> Entities
        {
            get
            {
                EntityCollection<Entity> entityCol = default(EntityCollection<Entity>);
                foreach (Production production in this)
                {
                    foreach (Entity entity in production.Product)
                    {
                        entityCol += entity;
                    }
                }
                return entityCol.RemoveRedundancy() as EntityCollection<Entity>;
            }
        }

        public bool Equals(Closure closure) { return this == closure; }

        public bool NotEquals(Closure closure) { return this != closure; }

        public static bool operator ==(Closure closure1, Closure closure2)
        {
            if (Object.ReferenceEquals(closure1, closure2)) return true;
            if (null == (Object)closure1 || null == (Object)closure2) return false;
            int count1 = closure1.Count;
            int count2 = closure2.Count;
            if (count1 != count2) return false;

            if (closure1._grammar != closure2._grammar) return false;
            for (int index = 0; index < count1; ++index)
            {
                if (closure1[index] != closure2[index])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(Closure closure1, Closure closure2)
        {
            return !(closure1 == closure2);
        }

        public override bool Equals(Object obj)
        {
            return (obj is Closure) ? Equals(obj as Closure) : base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override String ToString()
        {
            StringBuilder sbClosure = new StringBuilder(Title);
            sbClosure.AppendLine();
            foreach (SLRProduction production in this)
            {
                sbClosure.Append(production);
                sbClosure.AppendLine();
            }
            sbClosure.Remove(sbClosure.Length - 1, 1);
            return sbClosure.ToString();
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _productions.GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        public abstract Object Clone();

        #endregion

        public abstract Closure GoToEntity(Entity X);
    }

    class SLRClosure : Closure
    {
        public SLRClosure(String title, Grammar grammar, IEnumerable<SLRProduction> productLR)
            : base(title, grammar)
        {
            if (productLR == default(SLRProduction[])) return;

            _productions = new List<SLRProduction>(productLR);
            NonTerminal lastB = default(NonTerminal);
            //A --> α.Bβ, a
            for (int index = 0; index < Count; ++index)
            {
                SLRProduction product = this[index];
                if (product.DotPosition != product.Count)
                {
                    Entity entity = product.Product[product.DotPosition];

                    if (entity is NonTerminal)
                    {
                        NonTerminal B = entity as NonTerminal;  //B Non-Terminal
                        if (B != lastB)
                        {
                            //B --> .rX, b
                            foreach (Production production in grammar.Productions)
                            {
                                if (production.Producer == B)
                                {
                                    Add(new SLRProduction((NonTerminal)B, production.Product)); // gamma
                                }
                            }
                            lastB = B;
                        }
                    }
                }
            }
            RemoveRedundancy();
        }

        public SLRClosure(String title, Grammar grammar)
            : base(title, grammar)
        { }

        public SLRClosure()
            : this(default(String), new Grammar(), new SLRProduction[] { new SLRProduction() })
        { }

        public SLRClosure(String title, Grammar grammar, SLRClosure closure)
            : base(title, grammar)
        {
            _productions = (closure == default(SLRClosure)) ? default(List<SLRProduction>) : closure._productions;
        }


        public override Closure GoToEntity(Entity X)
        {
            SLRClosure closure = new SLRClosure("gotoX", _grammar);
            for (int index = 0; index < Count; ++index)
            {
                SLRProduction product = this[index];
                int dot = product.DotPosition;
                if (dot != product.Count)
                {
                    Entity Y = product.Product[dot];
                    if (Y == X)
                    {
                        closure.Add(new SLRProduction(product.Producer, product.Product, dot + 1));
                    }
                }
            }
            return closure;
        }

        public static explicit operator SLRProduction[](SLRClosure closure)
        {
            return closure.ToArray();
        }

        public override Object Clone()
        {
            return new SLRClosure(_title, _grammar, new List<SLRProduction>(_productions));
        }
    }

    class CLRClosure : SLRClosure
    {
        #region Constructors
        public CLRClosure(String title, Grammar grammar, IEnumerable<SLRProduction> productLALR)
            : base(title, grammar)
        {
            _productions = ((productLALR == default(CLRProduction[])) ? new List<SLRProduction>(new CLRProduction[] { }) : new List<SLRProduction>(productLALR));
            NonTerminal lastB = default(NonTerminal);
            //A --> α.Bβ, a
            for (int index = 0; index < Count; ++index)
            {
                EntityCollection<Entity> beta;     //β Next Entity to B
                EntityCollection<Terminal> a;      //a Look Aheads
                CLRProduction product = this[index] as CLRProduction;
                if (product.DotPosition != product.Count)
                {
                    Entity entity = product.Product[product.DotPosition];
                    if (entity is NonTerminal)
                    {
                        NonTerminal B = entity as NonTerminal;  //B Non-Terminal
                        if (B != lastB)
                        {
                            beta = product.Product.GetRange(product.DotPosition + 1);
                            a = product.LookAheads;
                            //B --> .rX, b
                            foreach (Production production in grammar.Productions)
                            {
                                if (production.Producer == B)
                                {
                                    //b = First(βa);
                                    EntityCollection<Terminal> b = default(EntityCollection<Terminal>);
                                    foreach (Terminal term in a)
                                    {
                                        b += (EntityCollection<Terminal>)grammar.First(beta + term);
                                    }
                                    b = (EntityCollection<Terminal>)(b != default(EntityCollection<Terminal>) ? b.RemoveRedundancy() : new EntityCollection<Terminal>(new Terminal[] { (Terminal)"#" }));

                                    //Adding B --> .rX, b
                                    Add(new CLRProduction((NonTerminal)B, production.Product, b)); // gamma
                                }
                            }
                            lastB = B;
                        }
                    }
                }
            }
            RemoveRedundancy();
        }
        
        public CLRClosure(String title, Grammar grammar)
            : base(title, grammar) 
        { }

        public CLRClosure()
            : this(default(String), new Grammar(), new SLRProduction[] { new CLRProduction() })
        { }

        public CLRClosure(String title, Grammar grammar, CLRClosure closure)
            : base(title, grammar, closure)
        {
            //_productions = (closure == default(LALRClosure)) ? default(List<LRProduction>) : new List<LRProduction>(closure.ToArray());
        }

        #endregion

        public void AddLookAhead(SLRProduction[] prodLALR)
        {
            for (int index = 0; index < Count; ++index)
            {
                CLRProduction prod1 = this[index] as CLRProduction;
                CLRProduction prod2 = prodLALR[index] as CLRProduction;

                //foreach (Terminal lookahead in prod2.LookAheads)
                prod1.LookAheads.List.AddRange(prod2.LookAheads);
                prod1 = prod1.LookAheads.RemoveRedundancy() as CLRProduction;
            }
        }

        public override Closure GoToEntity(Entity X)
        {
            CLRClosure closure = new CLRClosure("gotoX", _grammar);
            for (int index = 0; index < Count; ++index)
            {
                CLRProduction prodLALR = this[index] as CLRProduction;
                int dot = prodLALR.DotPosition;
                if (dot != prodLALR.Count)
                {
                    Entity Y = prodLALR.Product[dot];
                    if (Y == X)
                    {
                        closure.Add(new CLRProduction(prodLALR.Producer, prodLALR.Product, dot + 1, prodLALR.LookAheads));
                    }
                }
            }
            return closure;
        }

        public static explicit operator CLRProduction[](CLRClosure closure) { return (CLRProduction[])closure.ToArray(); }

        public override Object Clone()
        {
            return new SLRClosure(_title, _grammar, new List<SLRProduction>(_productions));
        }
    }
}