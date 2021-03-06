﻿using System.Collections.Generic;

namespace GraphQLCore.Language.AST
{
    public class GraphQLFieldDefinition : GraphQLTypeDefinition, IWithDirectives
    {
        public IEnumerable<GraphQLInputValueDefinition> Arguments { get; set; }

        public IEnumerable<GraphQLDirective> Directives { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.FieldDefinition;
            }
        }

        public GraphQLName Name { get; set; }
        public GraphQLType Type { get; set; }
    }
}