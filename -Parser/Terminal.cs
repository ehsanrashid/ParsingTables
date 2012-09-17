namespace Parser
{
    using System;

    public sealed class Terminal : Entity
    {
        public Terminal(String title)
            : base(title)
        { }

        public Terminal()
            : base()
        { }

        public Terminal(Terminal terminal)
            : base(terminal)
        { }

        public static explicit operator Terminal(String title)
        {
            return new Terminal(title);
        }
    }
}