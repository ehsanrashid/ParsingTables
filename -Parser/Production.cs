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
            _producer = producer;
            _product = product;
        }

        public Production(NonTerminal producer) : this(producer, default(EntityCollection<Entity>)) { }

        public Production(EntityCollection<Entity> product) : this(default(NonTerminal), product) { }

        public Production() : this(new NonTerminal(), new EntityCollection<Entity>()) { }
        
        #endregion

        private NonTerminal _producer;       // LeftHand Side (Producer)
        public NonTerminal Producer { get { return _producer; } }

        private EntityCollection<Entity> _product;  // RightHand Side (Product)
        public EntityCollection<Entity> Product { get { return _product; } }

        public int Count { get { return _product.Count; } }

        public bool Equals(Production production) { return this == production; }

        public bool NotEquals(Production production) { return this != production; }

        #region Static Methods

        public static bool operator ==(Production production1, Production production2)
        {
            if (Object.ReferenceEquals(production1, production2)) return true;
            if (null == (Object)production1 || null == (Object)production2) return false;
            return
                production1._producer == production2._producer
             && production1._product == production2._product;
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
            return String.Format("{0} --> {1}", _producer, _product);
        } 
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_product).GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        public Object Clone()
        {
            return new Production
                (
                    new NonTerminal(_producer),
                    new EntityCollection<Entity>(_product)
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
            StringBuilder sbProd = new StringBuilder(Producer + " --> ");
            int length = Count;
            for (int index = 0; index < length; ++index)
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
        private EntityCollection<Terminal> _lookAheads;
        public EntityCollection<Terminal> LookAheads
        { get { return _lookAheads; } }

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
            _lookAheads = lookAheads;
        }

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol, EntityCollection<Terminal> lookAheads) : this(nonTerm, entityCol, default(int), lookAheads) { }

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol) : this(nonTerm, entityCol, default(EntityCollection<Terminal>)) { }

        public CLRProduction() : this(new NonTerminal(), new EntityCollection<Entity>()) { }

        public bool Equals(CLRProduction prod)
        {
            return base.Equals(prod) && _lookAheads == prod.LookAheads;
        }

        public bool NotEquals(CLRProduction prod)
        {
            return !Equals(prod);
        }

        public override String ToString()
        {
            StringBuilder sbProd = new StringBuilder();
            sbProd.Append(", ");
            String entitySep = " | ";
            foreach (Terminal terminal in _lookAheads)
            {
                sbProd.Append(terminal);
                sbProd.Append(entitySep);
            }
            sbProd.Remove(sbProd.Length - entitySep.Length, entitySep.Length);
            return base.ToString() + sbProd.ToString();
        }
    }
}