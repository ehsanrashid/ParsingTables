namespace Parser
{
    using System;
    using System.Text;
    using System.Collections;

    public class Production : IEnumerable, ICloneable
    {
        #region Constructors
        public Production(NonTerminal producer, EntityCollection<Entity> product)
        {
            Producer = producer;
            Product = product;
        }

        public Production(NonTerminal producer) : this(producer, default(EntityCollection<Entity>)) { }

        public Production(EntityCollection<Entity> product) : this(default(NonTerminal), product) { }

        public Production() : this(new NonTerminal(), new EntityCollection<Entity>()) { }
        
        #endregion

        // LeftHand Side (Producer)
        public NonTerminal Producer { get; private set; }

        public EntityCollection<Entity> Product { get; private set; }

        public int Count { get { return Product.Count; } }

        public bool Equals(Production production) { return this == production; }

        public bool NotEquals(Production production) { return this != production; }

        #region Static Methods

        public static bool operator ==(Production production1, Production production2)
        {
            if (ReferenceEquals(production1, production2)) return true;
            if (ReferenceEquals(null, production1) || ReferenceEquals(null, production2)) return false;
            return
                (production1.Producer == production2.Producer)
             && (production1.Product == production2.Product);
        }

        public static bool operator !=(Production production1, Production production2)
        {
            return !(production1 == production2);
        }

        #endregion

        #region Overrided Methods
        
        public override bool Equals(Object obj)
        {
            return (obj is Production) ? Equals(obj as Production) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode() ^ base.GetHashCode();
        }

        public override String ToString()
        {
            return String.Concat(Producer, " --> ", Product);
        } 
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Product).GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        public Object Clone()
        {
            return new Production
                (
                    new NonTerminal(Producer),
                    new EntityCollection<Entity>(Product)
                );
        }

        #endregion
    }

    public class SLRProduction : Production
    {
        protected int _positionDot;
        public int DotPosition
        {
            get { return _positionDot; }
            set
            {
                if (0 > value || value > Count)
                {
                    throw new IndexOutOfRangeException("Dot Out of Bounds");
                }
                _positionDot = value;
            }
        }

        public SLRProduction(NonTerminal producer, EntityCollection<Entity> product, int posDot)
            : base(producer, product)
        {
            DotPosition = posDot;
        }

        public SLRProduction(NonTerminal producer, EntityCollection<Entity> product) : this(producer, product, default(int)) { }

        public SLRProduction() : this(new NonTerminal(), new EntityCollection<Entity>()) { }


        public bool Equals(SLRProduction prod)
        {
            return base.Equals(prod) && _positionDot == prod.DotPosition;
        }

        public bool NotEquals(SLRProduction prod)
        {
            return !Equals(prod);
        }

        public override String ToString()
        {
            var sbProd = new StringBuilder(Producer + " --> ");
            var length = Count;
            for (var index = 0; index < length; ++index)
            {
                if (index == DotPosition)
                {
                    sbProd.Append(".");
                }
                sbProd.Append(Product[index] + " ");
            }
            if (length == DotPosition)
            {
                sbProd.Append(".");
            }
            return sbProd.ToString();
        }
    }

    public class CLRProduction : SLRProduction
    {
        public EntityCollection<Terminal> LookAheads { get; private set; }

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol, int posDot, EntityCollection<Terminal> lookAheads)
            : base(nonTerm, entityCol, posDot)
        {
            /* 
            if( lookAheads != default(EntityCollection<Terminal>) )
                foreach( Terminal terminal in lookAheads )
                {
                    if( !(terminal is Terminal) )
                    {
                        _lookAheads = new EntityCollection<Terminal>((Terminal) "$");
                        return;
                    }
                }
            */
            LookAheads = lookAheads;
        }

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol, EntityCollection<Terminal> lookAheads) : this(nonTerm, entityCol, default(int), lookAheads) { }

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol) : this(nonTerm, entityCol, default(EntityCollection<Terminal>)) { }

        public CLRProduction() : this(new NonTerminal(), new EntityCollection<Entity>()) { }

        public bool Equals(CLRProduction prod)
        {
            return base.Equals(prod) && LookAheads == prod.LookAheads;
        }

        public bool NotEquals(CLRProduction prod)
        {
            return !Equals(prod);
        }

        public override String ToString()
        {
            var sbProd = new StringBuilder();
            sbProd.Append(", ");
            const String entitySep = " | ";
            foreach (var terminal in LookAheads)
            {
                sbProd.Append(terminal);
                sbProd.Append(entitySep);
            }
            sbProd.Remove(sbProd.Length - entitySep.Length, entitySep.Length);
            return String.Concat(base.ToString(), sbProd);
        }
    }
}