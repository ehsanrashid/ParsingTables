using System;
using System.Collections;
using System.Text;

namespace Parser
{
    public class Production : IEnumerable, ICloneable
    {
        // LeftHand Side (Producer)
        public NonTerminal Producer { get; private set; }

        // RightHand Side (Product)
        public EntityCollection<Entity> Product { get; private set; }


        public Production(NonTerminal producer, EntityCollection<Entity> product)
        {
            Producer = producer;
            Product = product;
        }

        public Production(NonTerminal producer) : this(producer, default(EntityCollection<Entity>)) { }

        public Production(EntityCollection<Entity> product) : this(default(NonTerminal), product) { }

        public Production() : this(new NonTerminal(), new EntityCollection<Entity>()) { }



        public int Count
        {
            get { return Product.Count; }
        }

        #region ICloneable Members

        public Object Clone() { return new Production(new NonTerminal(Producer), new EntityCollection<Entity>(Product)); }

        #endregion

        public bool Equals(Production production) { return (this == production); }

        public bool NotEquals(Production production) { return !Equals(production); } //(this != production);

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable) Product).GetEnumerator(); }

        #endregion

        #region Overrided Methods

        public override bool Equals(Object obj) { return (obj is Production) ? Equals(obj as Production) : base.Equals(obj); }

        public override int GetHashCode() { return ToString().GetHashCode() ^ base.GetHashCode(); }

        public override String ToString() { return String.Format("{0} --> {1}", Producer, Product); }

        #endregion

        #region Static Methods

        public static bool operator ==(Production production1, Production production2)
        {
            if (ReferenceEquals(production1, production2)) return true;
            if (ReferenceEquals(null, production1) || ReferenceEquals(null, production2)) return false;
            return
                (production1.Producer == production2.Producer)
                && (production1.Product == production2.Product);
        }

        public static bool operator !=(Production production1, Production production2) { return !(production1 == production2); }

        #endregion
    }

    public class SLRProduction : Production
    {
        protected int _positionDot;

        public SLRProduction(NonTerminal producer, EntityCollection<Entity> product, int posDot)
            : base(producer, product) { DotPosition = posDot; }

        public SLRProduction(NonTerminal producer, EntityCollection<Entity> product)
            : this(producer, product, default(int)) { }

        public SLRProduction() : this(new NonTerminal(), new EntityCollection<Entity>()) { }

        public int DotPosition
        {
            get { return _positionDot; }
            set
            {
                if (0 > value || value > Count) throw new IndexOutOfRangeException("Dot Out of Bounds");
                _positionDot = value;
            }
        }

        public bool Equals(SLRProduction prod) { return base.Equals(prod) && (_positionDot == prod.DotPosition); }

        public bool NotEquals(SLRProduction prod) { return !Equals(prod); }

        public override String ToString()
        {
            var sb = new StringBuilder(Producer + " --> ");
            var length = Count;
            for (var index = 0; index < length; ++index)
            {
                if (index == DotPosition) sb.Append(".");
                sb.Append(Product[index] + " ");
            }
            return (length == DotPosition) ? sb.Append(".").ToString() : sb.ToString();
        }
    }

    public class CLRProduction : SLRProduction
    {
        readonly EntityCollection<Terminal> _lookAheads;

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol, int posDot,
                             EntityCollection<Terminal> lookAheads)
            : base(nonTerm, entityCol, posDot)
        {
            if (default(EntityCollection<Terminal>) != lookAheads)
                foreach (var terminal in lookAheads)
                {
                    if (null != terminal) continue;

                    _lookAheads = new EntityCollection<Terminal>((EntityCollection<Terminal>) ((Terminal) "$"));
                    return;
                }

            _lookAheads = lookAheads;
        }

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol,
                             EntityCollection<Terminal> lookAheads)
            : this(nonTerm, entityCol, default(int), lookAheads) { }

        public CLRProduction(NonTerminal nonTerm, EntityCollection<Entity> entityCol)
            : this(nonTerm, entityCol, default(EntityCollection<Terminal>)) { }

        public CLRProduction() : this(new NonTerminal(), new EntityCollection<Entity>()) { }

        public EntityCollection<Terminal> LookAheads
        {
            get { return _lookAheads; }
        }

        public bool Equals(CLRProduction prod) { return base.Equals(prod) && (_lookAheads == prod.LookAheads); }

        public bool NotEquals(CLRProduction prod) { return !Equals(prod); }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append(", ");

            var first = true;
            foreach (var terminal in _lookAheads)
            {
                if (first) first = false;
                else sb.Append(" | "); // Entity Sep
                
                sb.Append(terminal);
            }

            return String.Concat(base.ToString(), sb);
        }
    }
}