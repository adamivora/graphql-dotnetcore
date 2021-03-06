﻿using System.Collections.Generic;

namespace GraphQLCore.Language.AST
{
    public class GraphQLInterfaceTypeDefinition : GraphQLTypeDefinition, IWithDirectives
    {
        public IEnumerable<GraphQLDirective> Directives { get; set; }

        public IEnumerable<GraphQLFieldDefinition> Fields { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.InterfaceTypeDefinition;
            }
        }

        public GraphQLName Name { get; set; }
    }
}