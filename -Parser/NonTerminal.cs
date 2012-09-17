namespace Parser
{
    using System;

    public sealed class NonTerminal : Entity
    {
        public NonTerminal(String title)
            : base(title)
        { }

        public NonTerminal()
            : base()
        { }

        public NonTerminal(NonTerminal nonTerminal)
            : base(nonTerminal)
        { }

        public static explicit operator NonTerminal(String title)
        {
            return new NonTerminal(title);
        }
    }
}