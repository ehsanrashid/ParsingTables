using System;

namespace Parser
{
    public sealed class GotoEntry
    {
        private readonly Closure _closureI;

        private readonly Entity _entityX;

        private readonly Closure _gotoI;

        public GotoEntry(Closure closureI, Entity entityX, Closure gotoI)
        {
            _closureI = closureI;
            _entityX = entityX;
            _gotoI = gotoI;
        }

        public Closure I
        {
            get { return _closureI; }
        }

        public Entity X
        {
            get { return _entityX; }
        }

        public Closure Goto
        {
            get { return _gotoI; }
        }

        public bool Equals(GotoEntry gotoEntry)
        {
            return (this == gotoEntry);
        }

        public bool NotEquals(GotoEntry gotoEntry)
        {
            return !Equals(gotoEntry); //(this != gotoEntry);
        }

        #region Overrided Methods

        public override bool Equals(Object obj)
        {
            return (obj is GotoEntry) ? Equals(obj as GotoEntry) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override String ToString()
        {
            return String.Format("Goto ({0}, {1}) = {2}", _closureI.Title, _entityX, _gotoI.Title);
        }

        #endregion

        #region Static Methods

        public static bool operator ==(GotoEntry gotoEntry1, GotoEntry gotoEntry2)
        {
            if (ReferenceEquals(gotoEntry1, gotoEntry2)) return true;
            if (ReferenceEquals(null, gotoEntry1) || ReferenceEquals(null, gotoEntry2)) return false;

            return (gotoEntry1._closureI == gotoEntry2._closureI)
                   && (gotoEntry1._entityX == gotoEntry2._entityX)
                   && (gotoEntry1._gotoI == gotoEntry2._gotoI);
        }

        public static bool operator !=(GotoEntry gotoEntry1, GotoEntry gotoEntry2)
        {
            return !(gotoEntry1 == gotoEntry2);
        }

        #endregion
    }
}