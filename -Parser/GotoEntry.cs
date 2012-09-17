namespace Parser
{
    using System;

    public sealed class GotoEntry
    {
        private Closure _closureI;
        private Entity _entityX;
        private Closure _gotoI;

        public Closure I { get { return _closureI; } }

        public Entity X { get { return _entityX; } }

        public Closure Goto { get { return _gotoI; } }

        public GotoEntry(Closure closureI, Entity entityX, Closure gotoI)
        {
            _closureI = closureI;
            _entityX = entityX;
            _gotoI = gotoI;
        }

        public bool Equals(GotoEntry gotoEntry) { return this == gotoEntry; }

        public bool NotEquals(GotoEntry gotoEntry) { return this != gotoEntry; }

        public static bool operator ==(GotoEntry gotoEntry1, GotoEntry gotoEntry2)
        {
            if (Object.ReferenceEquals(gotoEntry1, gotoEntry2)) return true;
            if (null == (Object) gotoEntry1 || null == (Object) gotoEntry2) return false;

            return gotoEntry1._closureI == gotoEntry2._closureI
                && gotoEntry1._entityX == gotoEntry2._entityX
                && gotoEntry1._gotoI == gotoEntry2._gotoI;
        }

        public static bool operator !=(GotoEntry gotoEntry1, GotoEntry gotoEntry2)
        {
            return !(gotoEntry1 == gotoEntry2);
        }

        public override bool Equals(Object obj)
        {
            return obj is GotoEntry ? Equals(obj) : base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override String ToString()
        {
            return String.Format("Goto ({0}, {1}) = {2}", _closureI.Title, _entityX, _gotoI.Title);
        }
    }
}