using System;

namespace Parser
{
    public sealed class Terminal : Entity
    {
        public Terminal(String title)
            : base(title) {}

        public Terminal() {}

        public Terminal(Entity terminal)
            : base(terminal) {}

        public static explicit operator Terminal(String title)
        {
            return new Terminal(title);
        }
    }
}