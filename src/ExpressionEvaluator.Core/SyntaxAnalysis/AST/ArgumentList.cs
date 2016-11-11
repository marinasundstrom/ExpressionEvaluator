using System;
using System.Collections;
using System.Collections.Generic;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class ArgumentList<T> : IEnumerable<Argument<T>>
    {
        private List<Argument<T>> _items;

        public ArgumentList(SyntaxToken openParen, IEnumerable<Argument<T>> arguments, SyntaxToken closeParen)
        {
            OpenParen = openParen;
            CloseParen = closeParen;

            _items = new List<Argument<T>>(arguments);
        }

        public SyntaxToken OpenParen { get; }

        public SyntaxToken CloseParen { get; }

        public void Add(Argument<T> arg)
        {
            _items.Add(arg);
        }

        public IEnumerator<Argument<T>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}