using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Parser
{
    public sealed class EntityCollection<T> : Collection<T>, ISet where T : Entity
    {
        #region Constructors

        public EntityCollection() {}

        public EntityCollection(IList<T> list)
            : base(list) {}

        public EntityCollection(EntityCollection<T> entityCol)
            : base(entityCol.Items) {}

        #endregion

        public List<T> List
        {
            get { return (List<T>) Items; }
        }

        #region Range

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection != default(IEnumerable<T>)) ((List<T>) Items).AddRange(collection);
        }

        public void ForEach(Action<T> action)
        {
            ((List<T>) Items).ForEach(action);
        }

        public EntityCollection<T> GetRange(int index, int count)
        {
            if (count <= 0) return default(EntityCollection<T>);

            //var subEntityCol = new EntityCollection<T>();
            //for (int idx = 0; idx < count; ++idx)
            //{
            //    subEntityCol.Add(this[index + idx]);
            //}

            var subEntityCol = new EntityCollection<T>(((List<T>) Items).GetRange(index, count));

            return subEntityCol;
        }

        public EntityCollection<T> GetRange(int index)
        {
            return GetRange(index, Count - index);
        }

        #endregion

        #region Find & Index

        public bool Exists(Predicate<T> match)
        {
            return ((List<T>) Items).Exists(match);
        }

        public T Find(Predicate<T> match)
        {
            return ((List<T>) Items).Find(match);
        }

        public T FindLast(Predicate<T> match)
        {
            return ((List<T>) Items).FindLast(match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return ((List<T>) Items).FindIndex(startIndex, count, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return ((List<T>) Items).FindIndex(startIndex, match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return ((List<T>) Items).FindIndex(match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return ((List<T>) Items).FindLastIndex(startIndex, count, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return ((List<T>) Items).FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return ((List<T>) Items).FindLastIndex(match);
        }

        public int IndexOf(T item, int index, int count)
        {
            return ((List<T>) Items).IndexOf(item, index, count);
        }

        public int IndexOf(T item, int index)
        {
            return ((List<T>) Items).IndexOf(item, index);
        }

        public new int IndexOf(T item)
        {
            return ((List<T>) Items).IndexOf(item);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            return ((List<T>) Items).LastIndexOf(item, index, count);
        }

        public int LastIndexOf(T item, int index)
        {
            return ((List<T>) Items).LastIndexOf(item, index);
        }

        public int LastIndexOf(T item)
        {
            return ((List<T>) Items).LastIndexOf(item);
        }

        #endregion

        #region ISet Members

        public ISet RemoveRedundancy()
        {
            int count = Count;
            for (int index = 0; index < count - 1;)
            {
                int findIdx;
                //for (findIdx = index + 1; findIdx < count; ++findIdx)
                //    if (this[index] == this[findIdx])
                //findIdx = FindIndex(index + 1, (T entity) => (entity == this[index]));
                findIdx = IndexOf(this[index], index + 1);
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

        //public static implicit operator EntityCollection<T>(T entity) { return new EntityCollection<T>(new T[] { entity }); }
        public static implicit operator EntityCollection<T>(T[] arrEntity)
        {
            return new EntityCollection<T>(new List<T>(arrEntity));
        }

        public static explicit operator T[](EntityCollection<T> entityCol)
        {
            return ((List<T>) entityCol.Items).ToArray();
        }

        public static EntityCollection<T> operator +(EntityCollection<T> entityCol, Entity entity)
        {
            if (null == entityCol) return new EntityCollection<T>(new[] {entity as T});
            if (null == entity) return new EntityCollection<T>(entityCol);

            var combineCol = new EntityCollection<T>();
            int count = entityCol.Count;
            for (int index = 0; index < count + 1; ++index) combineCol.Add((index < count) ? entityCol[index] : (T) entity);
            return combineCol;
        }

        public static EntityCollection<T> operator +(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        {
            if (entityCol1 == default(EntityCollection<T>)) return new EntityCollection<T>(entityCol2);
            if (entityCol2 == default(EntityCollection<T>)) return new EntityCollection<T>(entityCol1);
            var count1 = entityCol1.Count;
            var count2 = entityCol2.Count;
            var combineCol = new EntityCollection<T>();
            for (var index = 0; index < count1 + count2; ++index) combineCol.Add((index < count1) ? entityCol1[index] : entityCol2[index - count1]);
            return combineCol;
        }

        public static EntityCollection<T> operator -(EntityCollection<T> entityCol, Entity entity)
        {
            if (entityCol == default(EntityCollection<T>)) return default(EntityCollection<T>);
            if (entity == default(Entity)) return new EntityCollection<T>(entityCol);

            var findIdx = entityCol & entity;
            if (findIdx == -1) return new EntityCollection<T>(entityCol);

            var removeCol = new EntityCollection<T>();
            var count = entityCol.Count;
            for (var index = 0; index < count - 1; ++index) 
                removeCol.Add(entityCol[index + ((index < findIdx) ? 0 : 1)]);
            return removeCol;
        }

        public static EntityCollection<T> operator -(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        {
            if (entityCol1 == default(EntityCollection<T>)) return default(EntityCollection<T>);
            if (entityCol2 == default(EntityCollection<T>)) return new EntityCollection<T>(entityCol1);

            var findIdx = 0;
            foreach (var entity in entityCol2)
            {
                findIdx = entityCol1 & entity;
                if (findIdx != -1) break;
            }
            if (findIdx == -1) return new EntityCollection<T>(entityCol1);

            var removeCol = new EntityCollection<T>();
            int count = entityCol1.Count;
            for (int index = 0; index < count - 1; ++index) removeCol.Add(entityCol1[index + ((index < findIdx) ? 0 : 1)]);
            return removeCol;
        }

        public static int operator &(EntityCollection<T> entityCol, Entity entity)
        {
            return entityCol.IndexOf(entity as T);
        }

        #region Equals & !Equals

        public static bool operator ==(EntityCollection<T> entityCol, Entity entity)
        {
            if (ReferenceEquals(null, entityCol) || ReferenceEquals(null, entity)) return false;

            return (entityCol.Count == 1) && entityCol[0] == entity;
        }

        public static bool operator !=(EntityCollection<T> entityCol, Entity entity)
        {
            return !(entityCol == entity);
        }

        public static bool operator ==(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        {
            if (ReferenceEquals(entityCol1, entityCol2)) return true;
            if (ReferenceEquals(null, entityCol1) || ReferenceEquals(null, entityCol2)) return false;

            if (entityCol1.Count != entityCol2.Count) return false;

            //for (var index = 0; index < entityCol1.Count; ++index) 
            //    if (entityCol1[index] != entityCol2[index]) 
            //        return false;
            //return true;

            return !entityCol1.Where((entity, index) => entity != entityCol2[index]).Any();
        }

        public static bool operator !=(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        {
            return !(entityCol1 == entityCol2);
        }

        public static bool operator ==(EntityCollection<T> entityCol1, EntityCollection<Terminal> entityCol2)
        {
            if (ReferenceEquals(entityCol1, entityCol2)) return true;
            if (ReferenceEquals(null, entityCol1) || ReferenceEquals(null, entityCol2)) return false;

            var length1 = entityCol1.Count;
            var length2 = entityCol2.Count;
            if (length1 != length2) return false;

            for (int index = 0; index < length1; ++index) if (entityCol1[index] != entityCol2[index]) return false;
            return true;
        }

        public static bool operator !=(EntityCollection<T> entityCol1, EntityCollection<Terminal> entityCol2)
        {
            return !(entityCol1 == entityCol2);
        }

        public static bool operator ==(EntityCollection<T> entityCol1, EntityCollection<NonTerminal> entityCol2)
        {
            if (ReferenceEquals(entityCol1, entityCol2)) return true;
            if (ReferenceEquals(null, entityCol1) || ReferenceEquals(null, entityCol2)) return false;

            var length1 = entityCol1.Count;
            var length2 = entityCol2.Count;
            if (length1 != length2) return false;

            for (var index = 0; index < length1; ++index) if (entityCol1[index] != entityCol2[index]) return false;
            return true;
        }

        public static bool operator !=(EntityCollection<T> entityCol1, EntityCollection<NonTerminal> entityCol2)
        {
            return !(entityCol1 == entityCol2);
        }

        #endregion

        #endregion

        public bool Equals(EntityCollection<T> entityCol)
        {
            return this == entityCol;
        }

        public bool NotEquals(EntityCollection<T> entityCol)
        {
            return this != entityCol;
        }

        public bool Equals(params T[] arrEntity)
        {
            return Equals(new EntityCollection<T>(arrEntity));
        }

        public bool NotEquals(params T[] arrEntity)
        {
            return NotEquals(new EntityCollection<T>(arrEntity));
        }

        #region Overrided Methods

        public override bool Equals(Object obj)
        {
            return obj is EntityCollection<T> ? Equals(obj as EntityCollection<T>) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode() ^ base.GetHashCode();
        }

        public override String ToString()
        {
            var strBuild = new StringBuilder();
            foreach (Entity entity in this)
            {
                strBuild.Append(entity);
                strBuild.Append(" ");
            }
            strBuild.Remove(strBuild.Length - 1, 1);
            return strBuild.ToString();
        }

        #endregion
    }

    #region CollectionBase, ISet where T : Entity

    /*
    public sealed class EntityCollection<T> : CollectionBase, ISet where T : Entity
    {
        #region Constructors

        public EntityCollection(params T[] arrEntity)
            : base()
        {
            foreach (T entity in arrEntity)
            {
                if (entity != default(Entity))
                {
                    List.Add(entity);
                }
            }
        }

        public EntityCollection(EntityCollection<T> entityCol)
            : base()
        {
            foreach (T entity in entityCol)
            {
                if (entity != default(Entity))
                {
                    List.Add(entity);
                }
            }
        }

        #endregion

        //public bool IsNull { get { return List == default(IList); } }
        //new public int Count { get { return IsNull ? 0 : List.Count; } }

        public List<T> Entities
        {
            get { return List.ConvertToList<T>(); }
            //set { List = value; }
        }

        [IndexerName("Entity")]
        public T this[ int index ]
        {
            get { return (T) List[ index ]; }
            set { List[ index ] = value; }
        }

        public int Add(T entity)
        {
            if (entity == default(T))
            {
                return -1;
            }
            return List.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            if (entities == default(IEnumerable<T>))
            {
                foreach (T entity in entities)
                {
                    Add(entity);
                }
            }
        }

        public void AddRange(EntityCollection<T> entityCol)
        {
            if (entityCol != default(EntityCollection<T>))
            {
                foreach (T entity in entityCol)
                {
                    Add(entity);
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

        public void Remove(T entity)
        {
            if (entity != default(T))
            {
                List.Remove(entity);
            }
        }

        public bool Contains(T entity)
        {
            if (entity == default(T))
            {
                return false;
            }
            return List.Contains(entity);
        }

        public int IndexOf(T entity)
        {
            if (entity != default(T))
            {
                return -1;
            }
            return List.IndexOf(entity);
        }

        public void CopyTo(T[] arrEntity, int index)
        {
            List.CopyTo(arrEntity, index);
        }


       
       
        #region Overrided Methods

        public override bool Equals(Object obj)
        {
            return obj is EntityCollection<T> ? Equals(obj as EntityCollection<T>) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override String ToString()
        {
            StringBuilder sbEntityCol = new StringBuilder();
            foreach (Entity entity in this)
            {
                sbEntityCol.Append(entity);
                sbEntityCol.Append(" ");
            }
            sbEntityCol.Remove(sbEntityCol.Length - 1, 1);
            return sbEntityCol.ToString();
        }
        #endregion
    }
    */

    #endregion
}