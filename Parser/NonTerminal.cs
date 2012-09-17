using System;

namespace Parser
{
    public sealed class NonTerminal : Entity
    {
        public NonTerminal(String title)
            : base(title) {}

        public NonTerminal() {}

        public NonTerminal(IEntity nonTerminal)
            : base(nonTerminal) {}

        public static explicit operator NonTerminal(String title)
        {
            return new NonTerminal(title);
        }
    }
}