using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Parser
{
    public sealed class EntityCollection<T> : Collection<T>, ISet where T : Entity
    {
        public EntityCollection() { }

        public EntityCollection(IList<T> list)
            : base(list) { }

        public EntityCollection(EntityCollection<T> entityCol)
            : base(entityCol.Items) { }


        public List<T> Entities
        {
            get { return Items as List<T>; }
        }

        public EntityCollection<T> GetRange(int index, int count)
        {
            return (count <= 0)
                       ? new EntityCollection<T>() //default(EntityCollection<T>) :
                       : new EntityCollection<T>(Entities.GetRange(index, count));
        }

        public EntityCollection<T> GetRange(int index) { return GetRange(index, Count - index); }


        public bool Equals(EntityCollection<T> entityCol) { return (this == entityCol); }

        public bool NotEquals(EntityCollection<T> entityCol)
        {
            return !Equals(entityCol); //(this != entityCol);
        }

        public bool Equals(params T[] arrEntity) { return Equals(new EntityCollection<T>(arrEntity)); }

        public bool NotEquals(params T[] arrEntity) { return !Equals(arrEntity); }

        #region ISet Members

        public ISet RemoveRedundancy()
        {
            for (var i = Count - 1; i >= 0; --i)
            {
                var index = Entities.IndexOf(this[i], i + 1);
                if (-1 == index) continue;
                RemoveAt(index);
            }
            return this;
        }

        #endregion

        #region Overrided

        public override bool Equals(Object obj) { return (obj is EntityCollection<T>) ? Equals(obj as EntityCollection<T>) : base.Equals(obj); }

        public override int GetHashCode() { return ToString().GetHashCode() ^ base.GetHashCode(); }

        public override String ToString()
        {
            var sb = new StringBuilder();
            var first = true;
            foreach (var entity in this)
            {
                if (first) first = false;
                else sb.Append(" ");
                sb.Append(entity);
            }
            return sb.ToString();
        }

        #endregion

        #region Static

        public static explicit operator EntityCollection<T>(T entity) { return new EntityCollection<T>(new[] { entity }); }

        public static implicit operator EntityCollection<T>(T[] arrEntity) { return new EntityCollection<T>(new List<T>(arrEntity)); }

        public static explicit operator T[](EntityCollection<T> entityCol) { return entityCol.Entities.ToArray(); }

        public static EntityCollection<T> operator +(EntityCollection<T> entityCol, Entity entity)
        {
            if (null == entityCol) return new EntityCollection<T>(new[] { entity as T });
            if (null == entity) return new EntityCollection<T>(entityCol);

            var entCol = new EntityCollection<T>();
            var count = entityCol.Count;
            for (var index = 0; index < count + 1; ++index)
                entCol.Add((index < count) ? entityCol[index] : entity as T);
            return entCol;
        }

        public static EntityCollection<T> operator -(EntityCollection<T> entityCol, Entity entity)
        {
            if (null == entityCol) return default(EntityCollection<T>);
            if (null == entity) return new EntityCollection<T>(entityCol);

            var idxEnt = entityCol & entity;
            if (-1 == idxEnt) return new EntityCollection<T>(entityCol);

            var entCol = new EntityCollection<T>();
            for (var index = 0; index < entityCol.Count - 1; ++index)
                entCol.Add(entityCol[index + ((index < idxEnt) ? 0 : 1)]);
            return entCol;
        }

        public static EntityCollection<T> operator +(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        {
            if (null == entityCol1) return new EntityCollection<T>(entityCol2);
            if (null == entityCol2) return new EntityCollection<T>(entityCol1);

            var count1 = entityCol1.Count;
            var count2 = entityCol2.Count;
            var entityCol = new EntityCollection<T>();
            for (var index = 0; index < count1 + count2; ++index)
                entityCol.Add((index < count1) ? entityCol1[index] : entityCol2[index - count1]);
            return entityCol;
        }

        public static EntityCollection<T> operator -(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        {
            if (null == entityCol1) return default(EntityCollection<T>);
            if (null == entityCol2) return new EntityCollection<T>(entityCol1);

            //var entCol = new EntityCollection<T>();
            //foreach (var entity in entityCol2)
            //{
            //    var idxEnt = entityCol1 & entity;
            //    if (idxEnt == -1) continue;
            //    for (var index = 0; index < entityCol1.Count - 1; ++index)
            //        entCol.Add(entityCol1[index + ((index < idxEnt) ? 0 : 1)]);
            //}

            var commonItem = 0;
            var entCol = new EntityCollection<T>(entityCol1);
            foreach (var entity in entityCol2)
            {
                var idxEnt = entityCol1 & entity;
                if (-1 == idxEnt) continue;
                entCol.RemoveAt(idxEnt - commonItem);
                ++commonItem;
            }

            return entCol;
        }

        public static int operator &(EntityCollection<T> entityCol, Entity entity)
        { return entityCol.IndexOf(entity as T); }

        public static bool operator ==(EntityCollection<T> entityCol, Entity entity)
        {
            if (ReferenceEquals(null, entityCol) || ReferenceEquals(null, entity)) return false;
            return (entityCol.Count == 1) && entityCol[0] == entity;
        }

        public static bool operator !=(EntityCollection<T> entityCol, Entity entity)
        { return !(entityCol == entity); }

        public static bool operator ==(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        {
            if (ReferenceEquals(entityCol1, entityCol2)) return true;
            if (ReferenceEquals(null, entityCol1) || ReferenceEquals(null, entityCol2)) return false;

            if (entityCol1.Count != entityCol2.Count) return false;

            for (var index = 0; index < entityCol1.Count; ++index)
                if (entityCol1[index] != entityCol2[index])
                    return false;
            return true;
        }

        public static bool operator !=(EntityCollection<T> entityCol1, EntityCollection<T> entityCol2)
        { return !(entityCol1 == entityCol2); }

        public static bool operator ==(EntityCollection<T> entityCol1, EntityCollection<Terminal> entityCol2)
        {
            if (ReferenceEquals(entityCol1, entityCol2)) return true;
            if (ReferenceEquals(null, entityCol1) || ReferenceEquals(null, entityCol2)) return false;

            if (entityCol1.Count != entityCol2.Count) return false;

            for (var index = 0; index < entityCol1.Count; ++index) if (entityCol1[index] != entityCol2[index]) return false;
            return true;
        }

        public static bool operator !=(EntityCollection<T> entityCol1, EntityCollection<Terminal> entityCol2)
        { return !(entityCol1 == entityCol2); }

        public static bool operator ==(EntityCollection<T> entityCol1, EntityCollection<NonTerminal> entityCol2)
        {
            if (ReferenceEquals(entityCol1, entityCol2)) return true;
            if (ReferenceEquals(null, entityCol1) || ReferenceEquals(null, entityCol2)) return false;

            if (entityCol1.Count != entityCol2.Count) return false;

            for (var index = 0; index < entityCol1.Count; ++index) if (entityCol1[index] != entityCol2[index]) return false;
            return true;
        }

        public static bool operator !=(EntityCollection<T> entityCol1, EntityCollection<NonTerminal> entityCol2)
        { return !(entityCol1 == entityCol2); }

        #endregion
    }
}