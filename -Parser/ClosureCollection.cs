using System.Linq;

namespace Parser
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class ClosureCollection : Collection<Closure>, ISet
    {
        #region Constructors

        public ClosureCollection() { }

        public ClosureCollection(IList<Closure> list)
            : base(list)
        { }


        public ClosureCollection(ClosureCollection closureCol)
            : base(closureCol.Items)
        { }

        #endregion

        public List<Closure> List
        {
            get { return (List<Closure>) Items; }
        }

        #region Range

        public void AddRange(IEnumerable<Closure> collection)
        {
            if (collection != default(IEnumerable<Closure>))
            {
                ((List<Closure>) Items).AddRange(collection);
            }
        }

        public void ForEach(Action<Closure> action)
        {
            ((List<Closure>) Items).ForEach(action);
        }

        public ClosureCollection GetRange(int index, int count)
        {
            if (count <= 0) return default(ClosureCollection);

            //var subClosureCol = new ClosureCollection();
            //for (int idx = 0; idx < count; ++idx)
            //{
            //    subClosureCol.Add(this[index + idx]);
            //}

            var subClosureCol = new ClosureCollection(((List<Closure>) Items).GetRange(index, count));

            return subClosureCol;
        }
        public ClosureCollection GetRange(int index)
        {
            return GetRange(index, Count - index);
        }
        #endregion

        #region Find & Index

        public bool Exists(Predicate<Closure> match)
        {
            return ((List<Closure>) Items).Exists(match);
        }

        public Closure Find(Predicate<Closure> match)
        {
            return ((List<Closure>) Items).Find(match);
        }
        public Closure FindLast(Predicate<Closure> match)
        {
            return ((List<Closure>) Items).FindLast(match);
        }

        public int FindIndex(int startIndex, int count, Predicate<Closure> match)
        {
            return ((List<Closure>) Items).FindIndex(startIndex, count, match);
        }
        public int FindIndex(int startIndex, Predicate<Closure> match)
        {
            return ((List<Closure>) Items).FindIndex(startIndex, match);
        }
        public int FindIndex(Predicate<Closure> match)
        {
            return ((List<Closure>) Items).FindIndex(match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<Closure> match)
        {
            return ((List<Closure>) Items).FindLastIndex(startIndex, count, match);
        }
        public int FindLastIndex(int startIndex, Predicate<Closure> match)
        {
            return ((List<Closure>) Items).FindLastIndex(startIndex, match);
        }
        public int FindLastIndex(Predicate<Closure> match)
        {
            return ((List<Closure>) Items).FindLastIndex(match);
        }

        public int IndexOf(Closure item, int index, int count)
        {
            return ((List<Closure>) Items).IndexOf(item, index, count);
        }
        public int IndexOf(Closure item, int index)
        {
            return ((List<Closure>) Items).IndexOf(item, index);
        }
        new public int IndexOf(Closure item)
        {
            return ((List<Closure>) Items).IndexOf(item);
        }

        public int LastIndexOf(Closure item, int index, int count)
        {
            return ((List<Closure>) Items).LastIndexOf(item, index, count);
        }
        public int LastIndexOf(Closure item, int index)
        {
            return ((List<Closure>) Items).LastIndexOf(item, index);
        }
        public int LastIndexOf(Closure item)
        {
            return ((List<Closure>) Items).LastIndexOf(item);
        }

        #endregion

        #region Overrided Methods

        public override int GetHashCode()
        {
            return ToString().GetHashCode() ^ base.GetHashCode();
        }

        public override String ToString()
        {
            var strBuild = new StringBuilder();
            foreach (Closure closer in this)
            {
                strBuild.Append(closer);
                strBuild.AppendLine();
            }
            return strBuild.ToString();
        }
        #endregion

        #region ISet Members

        public ISet RemoveRedundancy()
        {
            int count = Count;
            for (int index = 0; index < count - 1; )
            {
                int findIdx =
                    //FindIndex(index + 1, (Closure clr) => (clr == this[index]));
                    IndexOf(this[index], index + 1);
                if (findIdx != -1)
                {
                    RemoveAt(findIdx);
                    --count;
                    continue;
                }
                ++index;
            }
            return this;
        }
        #endregion

        #region Static Methods

        public static implicit operator ClosureCollection(Closure closure) { return new ClosureCollection(new[] { closure }); }
        public static implicit operator ClosureCollection(Closure[] arrClosure) { return new ClosureCollection(new List<Closure>(arrClosure)); }

        public static explicit operator Closure[](ClosureCollection closureCol) { return ((List<Closure>) closureCol.Items).ToArray(); }

        public static ClosureCollection operator +(ClosureCollection closureCol1, ClosureCollection closureCol2)
        {
            if (closureCol1 == default(ClosureCollection)) return closureCol2;
            if (closureCol2 == default(ClosureCollection)) return closureCol1;
            var combine = new ClosureCollection();
            combine.AddRange(closureCol1);
            combine.AddRange(closureCol2);
            //combine.RemoveRedundancy();
            return combine;
        }

        public static ClosureCollection operator -(ClosureCollection closureCol1, ClosureCollection closureCol2)
        {
            if (closureCol1 == default(ClosureCollection)) return default(ClosureCollection);
            if (closureCol2 == default(ClosureCollection)) return new ClosureCollection(closureCol1);

            var common = 0;
            var remove = new ClosureCollection(closureCol1);
            //foreach (var closure in closureCol2)
            //{
            //    var index =
            //        //closureCol1.FindIndex(common, (Closure clr) => (clr == closure));
            //        closureCol1.IndexOf(closure, common);
            //    if (index != -1)
            //    {
            //        remove.RemoveAt(index - common);
            //        ++common;
            //    }
            //}

            foreach (var index in closureCol2.Select(closure => closureCol1.IndexOf(closure, common)).Where(index => index != -1))
            {
                remove.RemoveAt(index - common);
                ++common;
            }
            return remove;
        }
        #endregion
    }

    #region CollectionBase, ISet
    /*
    public sealed class ClosureCollection : CollectionBase, ISet
    {

        #region Constructors

        public ClosureCollection(params Closure[] arrClosure)
            : base()
        {
            foreach (Closure closure in arrClosure)
            {
                List.Add(closure);
            }
        }

        public ClosureCollection(ClosureCollection closureCol)
            : base()
        {
            foreach (Closure closure in closureCol)
            {
                List.Add(closure);
            }
        }
        #endregion

        //public bool IsNull { get { return List == default(IList); } }
        //new public int Count { get { return IsNull ? 0 : List.Count; } }

        [IndexerName("Closure")]
        public Closure this[ int index ]
        {
            get { return (Closure) List[ index ]; }
            set { List[ index ] = value; }
        }

        public int Add(Closure closure)
        {
            return List.Add(closure);
        }

        public void AddRange(IEnumerable<Closure> closures)
        {
            if (closures != default(IEnumerable<Closure>))
            {
                foreach (Closure closure in closures)
                {
                    Add(closure);
                }
            }
        }

        public void AddRange(ClosureCollection closureCol)
        {
            if (closureCol != default(ClosureCollection))
            {
                foreach (Closure closure in closureCol)
                {
                    Add(closure);
                }
            }
        }

        new public void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                List.RemoveAt(index);
            }
        }

        public void Remove(Closure closure)
        {
            List.Remove(closure);
        }

        public bool Contains(Closure closure)
        {
            return List.Contains(closure);
        }

        public int IndexOf(Closure closure)
        {
            return List.IndexOf(closure);
        }

        public void CopyTo(Closure[] arrClosure, int index)
        {
            List.CopyTo(arrClosure, index);
        }


        #endregion
    }
    */
    #endregion
}
