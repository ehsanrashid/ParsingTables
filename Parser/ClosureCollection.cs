using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Parser
{
    public sealed class ClosureCollection : Collection<Closure>, ISet
    {
        public ClosureCollection() { }

        public ClosureCollection(IList<Closure> list)
            : base(list) { }

        public ClosureCollection(ClosureCollection closureCol)
            : base(closureCol.Items) { }

        public List<Closure> Closures
        {
            get { return Items as List<Closure>; }
        }

        public ClosureCollection GetRange(int index, int count)
        {
            return (count > 0)
                       ? new ClosureCollection(Closures.GetRange(index, count))
                       : default(ClosureCollection);
        }

        public ClosureCollection GetRange(int index) { return GetRange(index, Count - index); }

        public bool Equals(ClosureCollection closureCol) { return (this == closureCol); }

        public bool NotEquals(ClosureCollection closureCol) { return !Equals(closureCol); //(this != closureCol);
        }

        #region ISet Members

        public ISet RemoveRedundancy()
        {
            for (var i = Count - 1; i >= 0; --i)
            {
                var index = Closures.IndexOf(this[i], i + 1);
                if (index == -1) continue;
                RemoveAt(index);
            }
            return this;
        }

        #endregion

        #region Overrided

        public override bool Equals(Object obj) 
        { return (obj is ClosureCollection) ? Equals(obj as ClosureCollection) : base.Equals(obj); }

        public override int GetHashCode() { return ToString().GetHashCode() ^ base.GetHashCode(); }

        public override String ToString()
        {
            var sb = new StringBuilder();
            var first = true;
            foreach (var closer in this)
            {
                if (first) first = false;
                else sb.AppendLine();
                sb.Append(closer);
            }
            return sb.ToString();
        }

        #endregion

        #region Static

        public static implicit operator ClosureCollection(Closure closure) { return new ClosureCollection(new[] {closure}); }

        public static implicit operator ClosureCollection(Closure[] arrClosure) { return new ClosureCollection(new List<Closure>(arrClosure)); }

        public static explicit operator Closure[](ClosureCollection closureCol) { return ((List<Closure>) closureCol.Items).ToArray(); }

        public static ClosureCollection operator +(ClosureCollection closureCol1, ClosureCollection closureCol2)
        {
            if (null == closureCol1) return new ClosureCollection(closureCol2);
            if (null == closureCol2) return new ClosureCollection(closureCol1);

            var closureCol = new ClosureCollection();
            closureCol.Closures.AddRange(closureCol1);
            closureCol.Closures.AddRange(closureCol2);
            closureCol.RemoveRedundancy();
            return closureCol;
        }

        public static ClosureCollection operator -(ClosureCollection closureCol1, ClosureCollection closureCol2)
        {
            if (default(ClosureCollection) == closureCol1) return default(ClosureCollection);
            if (default(ClosureCollection) == closureCol2) return new ClosureCollection(closureCol1);

            var commonItem = 0;
            var closureCol = new ClosureCollection(closureCol1);
            foreach (var closure in closureCol2)
            {
                var idxCls = closureCol1.Closures.IndexOf(closure, commonItem);
                if (idxCls == -1) continue;

                closureCol.RemoveAt(idxCls - commonItem);
                ++commonItem;
            }
            return closureCol;
        }

        public static int operator &(ClosureCollection closureCol, Closure closure) { return closureCol.IndexOf(closure); }

        public static bool operator ==(ClosureCollection closureCol1, ClosureCollection closureCol2)
        {
            if (ReferenceEquals(closureCol1, closureCol2)) return true;
            if (ReferenceEquals(null, closureCol1) || ReferenceEquals(null, closureCol2)) return false;

            if (closureCol1.Count != closureCol2.Count) return false;

            for (var index = 0; index < closureCol1.Count; ++index)
                if (closureCol1[index] != closureCol2[index]) return false;
            return true;
        }

        public static bool operator !=(ClosureCollection closureCol1, ClosureCollection closureCol2) { return !(closureCol1 == closureCol2); }

        #endregion
    }
}