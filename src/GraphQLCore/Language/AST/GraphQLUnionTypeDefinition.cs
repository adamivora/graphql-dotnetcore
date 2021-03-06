﻿using System.Collections.Generic;

namespace GraphQLCore.Language.AST
{
    public class GraphQLUnionTypeDefinition : GraphQLTypeDefinition, IWithDirectives
    {
        public IEnumerable<GraphQLDirective> Directives { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.UnionTypeDefinition;
            }
        }

        public GraphQLName Name { get; set; }
        public IEnumerable<GraphQLNamedType> Types { get; set; }
    }
}