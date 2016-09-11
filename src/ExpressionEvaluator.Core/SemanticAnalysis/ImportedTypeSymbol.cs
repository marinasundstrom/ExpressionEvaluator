using System;
using System.Reflection;

namespace ExpressionEvaluator.SemanticAnalysis
{

    class ImportedTypeSymbol : ITypeSymbol
    {
        private TypeInfo type;

        public ImportedTypeSymbol(Type type)
        {
            this.type = type.GetTypeInfo();
        }

        public string Name
        {
            get
            {
                return type.Name;
            }
        }

        public TypeInfo GetTypeInfo()
        {
            return type;
        }
    }
}