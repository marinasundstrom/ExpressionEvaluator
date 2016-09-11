using ExpressionEvaluator.SyntaxAnalysis.AST;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using ExpressionEvaluator.Diagnostics;
using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SemanticAnalysis
{
    public class SemanticModel
    {
        private List<ITypeSymbol> typeSymbols = new List<ITypeSymbol>();
        private List<IExpressionInfo> expressionInfos = new List<IExpressionInfo>();

        public SemanticModel(DiagnosticsBag diagnostics)
        {
            Diagnostics = diagnostics;

            typeSymbols.Add(new ImportedTypeSymbol(typeof(object)));
            typeSymbols.Add(new ImportedTypeSymbol(typeof(int)));
            typeSymbols.Add(new ImportedTypeSymbol(typeof(double)));
            typeSymbols.Add(new ImportedTypeSymbol(typeof(bool)));
        }

        public DiagnosticsBag Diagnostics { get; }

        public IExpressionInfo GetExpressionInfo(Expression expression)
        {
            var expressionInfo = expressionInfos.FirstOrDefault(x => x.Expression == expression);
            if(expressionInfo == null)
            {
                expressionInfo = AnalyzeExpression(expression);
                expressionInfos.Add(expressionInfo);
            }
            return expressionInfo;
        }

        private IExpressionInfo AnalyzeExpression(Expression expression)
        {
            IExpressionInfo expressionInfo = null;
            ITypeSymbol type = null;

            switch(expression)
            {
                case LetExpression le:
                    var let = GetExpressionInfo(le.AssignedExpression);
                    expressionInfo = new ExpressionInfo(le, type);
                    break;

                case BinaryExpression be:
                    expressionInfo = AnalyzeBinaryExpression(be);
                    break;

                case NumberExpression ne:
                    expressionInfo = AnalyzeNumberExpression(ne);
                    break;

                case BooleanLiteralExpression boole:
                    expressionInfo = AnalyzeBooleanLiteralExpression(boole);
                    break;

                case IfThenExpression be:
                    expressionInfo = AnalyzeIfThenExpression(be);
                    break;

                default:
                    throw new Exception();
            }
          
            return expressionInfo;
        }

        private IExpressionInfo AnalyzeIfThenExpression(IfThenExpression be)
        {
            IExpressionInfo expressionInfo = null;
            ITypeSymbol type = null;

            var body = GetExpressionInfo(be.Body);
            var elseBody = GetExpressionInfo(be.ElseBody);

            var objectSymbol = GetTypeSymbol(typeof(object));
            var intSymbol = GetTypeSymbol(typeof(int));
            var doubleSymbol = GetTypeSymbol(typeof(double));

            if (body.Type == elseBody.Type)
            {
                type = body.Type;
            }
            else if ((body.Type == intSymbol || elseBody.Type == doubleSymbol) || (body.Type == doubleSymbol || elseBody.Type == intSymbol))
            {
                type = doubleSymbol;
            }
            else
            {
                Diagnostics.AddError("The return types in each codepath are not compatible with each other.", new SourceSpan(be.IfKeyword.GetStartLocation(), be.EndKeyword.GetEndLocation()));
                type = objectSymbol;
            }

            expressionInfo = new ExpressionInfo(be, type);
            return expressionInfo;
        }

        private IExpressionInfo AnalyzeBinaryExpression(BinaryExpression be)
        {
            IExpressionInfo expressionInfo = null;
            ITypeSymbol type = null;

            var left = GetExpressionInfo(be.Left);
            var right = GetExpressionInfo(be.Right);

            var intSymbol = GetTypeSymbol(typeof(int));
            var doubleSymbol = GetTypeSymbol(typeof(double));

            if (left.Type == right.Type)
            {
                type = left.Type;
            }
            else if (left.Type == doubleSymbol || right.Type == doubleSymbol)
            {
                type = doubleSymbol;
            }
            else
            {
                type = intSymbol;
            }

            expressionInfo = new ExpressionInfo(be, type);
            return expressionInfo;
        }

        private IExpressionInfo AnalyzeBooleanLiteralExpression(BooleanLiteralExpression boole)
        {
            var type = GetTypeSymbol(typeof(bool));
            return new ExpressionInfo(boole, type);
        }

        private IExpressionInfo AnalyzeNumberExpression(Expression expression)
        {
            IExpressionInfo expressionInfo = null;
            ITypeSymbol type = null;

            switch (expression)
            {
                case IntegerNumberExpression ne:
                    type = GetTypeSymbol(typeof(int));
                    expressionInfo = new ExpressionInfo(ne, type);
                    break;

                case RealNumberExpression ne:
                    type = GetTypeSymbol(typeof(double));
                    expressionInfo = new ExpressionInfo(ne, type);
                    break;

                default:
                    throw new Exception();
            }

            return expressionInfo;
        }

        public ITypeSymbol GetTypeSymbol(Type type)
        {
            return typeSymbols
                .OfType<ImportedTypeSymbol>()
                .First(x => x.GetTypeInfo() == type.GetTypeInfo());
        }
    }
}
