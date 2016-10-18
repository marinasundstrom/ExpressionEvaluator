using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public enum SyntaxKind
    {
        Missing,

        Invalid,
        EndOfFile,

        Newline,
        Tab,
        CarriageReturn,
        Whitespace,

        Number,
        Plus,
        Minus,
        Star,
        Slash,
        OpenParen,
        CloseParen,
        Equal,
        NotEquals,
        Is,
        IsNot,
        Less,
        CloseAngleBracket,
        Negate,
        Assign,
        OpenAngleBracket,
        GreaterOrEqual,
        Caret,
        Identifier,
        Or,
        And,
        Percent,
        NotKeyword,
        Period,
        Comma,
        Colon,
        Semicolon,

        IfKeyword,
        ThenKeyword,
        ElseKeyword,
        ForKeyword,
        EachKeyword,
        WhileKeyword,
        DoKeyword,
        EndKeyword,
        TrueKeyword,
        FalseKeyword,
        LetKeyword
    }
}
