namespace Parser
{
    using System;
    using System.Collections.Generic;

    public abstract class Entity : IEntity, ICloneable
    {
        public Entity(String title)
        {
            _title = title;
        }

        public Entity()
            : this(String.Empty)
        { }

        public Entity(Entity entity)
            : this(entity._title)
        { }
        
        //~Entity() { }

        protected String _title;
        public String Title
        {
            get { return _title; }
            //protected 
            set { _title = value; }
        }

        public bool Equals(Entity entity) { return this == entity; }

        public bool NotEquals(Entity entity) { return this != entity; }

        public override bool Equals(Object obj)
        {
            return obj is Entity ? Equals(obj as Entity) : base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override String ToString() { return _title; }

        #region ICloneable Members

        Object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region Static Methods
        
        public static EntityCollection<Entity> operator +(Entity entity1, Entity entity2) { return new EntityCollection<Entity>(new Entity[] { entity1, entity2 }); }

        public static implicit operator EntityCollection<Entity>(Entity entity) { return new EntityCollection<Entity>(new Entity[] { entity }); }

        public static bool operator ==(Entity entity1, Entity entity2)
        {
            if (Object.ReferenceEquals(entity1, entity2)) return true;
            if (null == (Object)entity1 || null == (Object)entity2) return false;
            // Terminal or NonTerminal
            return entity1.GetType() == entity2.GetType() && entity1._title == entity2._title;
        }

        public static bool operator !=(Entity entity1, Entity entity2)
        {
            return !(entity1 == entity2);
        }
        
        #endregion
    }
}