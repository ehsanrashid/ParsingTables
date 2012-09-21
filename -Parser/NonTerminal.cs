namespace Parser
{
    using System;

    public sealed class NonTerminal : Entity
    {
        public NonTerminal(String title)
            : base(title)
        { }

        public NonTerminal() { }

        public NonTerminal(Entity nonTerminal)
            : base(nonTerminal)
        { }

        public static explicit operator NonTerminal(String title)
        {
            return new NonTerminal(title);
        }
    }
}