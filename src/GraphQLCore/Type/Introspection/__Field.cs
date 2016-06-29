﻿namespace GraphQLCore.Type.Introspection
{
    using System.Linq;
    using System.Linq.Expressions;
    using Utils;

    public class __Field : GraphQLObjectType
    {
        public __Field(string fieldName, string fieldDescription, LambdaExpression expression, GraphQLSchema schema, bool isAccessor) : base("__Field",
            "Object and Interface types are described by a list of Fields, each of " +
            "which has a name, potentially a list of arguments, and a return type."
            , null)
        {
            this.Field("name", () => fieldName);
            this.Field("description", () => fieldDescription);
            this.Field("isDeprecated", () => null as bool?);
            this.Field("deprecationReason", () => null as string);

            this.schema = schema;
            this.CreateTypeRelatedFields(expression, isAccessor);
        }

        private void CreateTypeRelatedFields(LambdaExpression expression, bool isAccessor)
        {
            var fieldType = ReflectionUtilities.GetReturnValueFromLambdaExpression(expression);

            this.Field("args", () => isAccessor ? TypeUtilities.FetchInputArguments(expression, this.schema).Skip(1).ToArray() : TypeUtilities.FetchInputArguments(expression, this.schema));
            this.Field("type", () => TypeUtilities.ResolveObjectFieldType(fieldType, this.schema));
        }
    }
}