using System;

namespace Parser
{
    public sealed class GotoEntry
    {
        public Closure I { get; private set; }

        public Entity X { get; private set; }

        public Closure Goto { get; private set; }

        public GotoEntry(Closure closureI, Entity entityX, Closure gotoI)
        {
            I = closureI;
            X = entityX;
            Goto = gotoI;
        }

        public bool Equals(GotoEntry gotoEntry) { return (this == gotoEntry); }

        public bool NotEquals(GotoEntry gotoEntry) { return !Equals(gotoEntry); //(this != gotoEntry);
        }

        #region Overrided Methods

        public override bool Equals(Object obj) 
        { return (obj is GotoEntry) ? Equals(obj as GotoEntry) : base.Equals(obj); }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public override String ToString() 
        { return String.Format("Goto ({0}, {1}) = {2}", I.Title, X, Goto.Title); }

        #endregion

        #region Static Methods

        public static bool operator ==(GotoEntry gotoEntry1, GotoEntry gotoEntry2)
        {
            if (ReferenceEquals(gotoEntry1, gotoEntry2)) return true;
            if (ReferenceEquals(null, gotoEntry1) || ReferenceEquals(null, gotoEntry2)) return false;

            return (gotoEntry1.I == gotoEntry2.I)
                   && (gotoEntry1.X == gotoEntry2.X)
                   && (gotoEntry1.Goto == gotoEntry2.Goto);
        }

        public static bool operator !=(GotoEntry gotoEntry1, GotoEntry gotoEntry2) 
        { return !(gotoEntry1 == gotoEntry2); }

        #endregion
    }
}