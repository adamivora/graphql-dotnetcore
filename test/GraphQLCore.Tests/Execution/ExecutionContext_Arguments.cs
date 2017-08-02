﻿namespace GraphQLCore.Tests.Execution
{
    using GraphQLCore.Execution;
    using GraphQLCore.Type;
    using GraphQLCore.Type.Directives;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    [TestFixture]
    public class ExecutionContext_Arguments
    {
        private GraphQLSchema schema;

        [Test]
        public void Execute_ArrayViaArgument_PrintsCorrectList()
        {
            var result = this.schema.Execute("{ withArray(ids: [1,2,3]) }");

            Assert.AreEqual(3, result.Data.withArray);
        }

        [Test]
        public void Execute_EntityFetchedWithIntegerArgument_PrintsCorrectValues()
        {
            var result = this.schema.Execute("{ nested(id: 42) { Id, StringField } }");

            Assert.AreEqual(42, result.Data.nested.Id);
            Assert.AreEqual("Test with id 42", result.Data.nested.StringField);
        }

        [Test]
        public void Execute_FloatViaArgument_PrintsCorrectFloat()
        {
            var result = this.schema.Execute("{ withFloat(value: 3.14) }");

            Assert.AreEqual(3.14f, result.Data.withFloat);
        }

        [Test]
        public void Execute_FloatViaArgumentWithInt_PrintsCorrectFloat()
        {
            var result = this.schema.Execute("{ withFloat(value: 3) }");

            Assert.AreEqual(3.0f, result.Data.withFloat);
        }

        [Test]
        public void Execute_IEnumerableViaArgument_PrintsCorrectList()
        {
            var result = this.schema.Execute("{ withIEnumerable(ids: [1,2,3]) }");

            Assert.AreEqual(3, result.Data.withIEnumerable);
        }

        [Test]
        public void Execute_NestedEntityFetchedWithArguments_PrintsCorrectValues()
        {
            var result = this.schema.Execute("{ nested(id: 42) { nested(id: 24) { text(id: 12 str: \"string argument\") } } }");

            Assert.AreEqual("I received 12 with string argument", result.Data.nested.nested.text);
        }

        [Test]
        public void Execute_NothingInNonMandatoryValue_InvokesResolverWithNullValue()
        {
            var result = this.schema.Execute("{ isNull }");

            Assert.AreEqual(true, result.Data.isNull);
        }

        [Test]
        public void Execute_ValueInNonMandatoryValue_InvokesResolverWitCorrecthNullValue()
        {
            var result = this.schema.Execute("{ isNull(nonMandatory: 1) }");

            Assert.AreEqual(false, result.Data.isNull);
        }

        [Test]
        public void Execute_AstObjectArgument_ReturnsCorrectValue()
        {
            var result = this.schema.Execute("{ withObjectArg(obj: { stringField: \"abc\" }) { StringField } }");

            Assert.AreEqual("abc", result.Data.withObjectArg.StringField);
        }

        [Test]
        public void Execute_WithList_SingleValue()
        {
            var result = this.schema.Execute("{ withList(ids: 1) }");

            Assert.AreEqual(1, ((IEnumerable<object>)result.Data.withList).ElementAt(0));
        }

        [Test]
        public void Execute_WithList_MultipleValues()
        {
            var result = this.schema.Execute("{ withList(ids: [4 8 6]) }");

            Assert.AreEqual(4, ((IEnumerable<object>)result.Data.withList).ElementAt(0));
            Assert.AreEqual(8, ((IEnumerable<object>)result.Data.withList).ElementAt(1));
            Assert.AreEqual(6, ((IEnumerable<object>)result.Data.withList).ElementAt(2));
        }

        [Test]
        public void Execute_AstObjectListArgument_CorrectlyTranslatesIntoOutput()
        {
            var result = this.schema.Execute("{ withObjectListArg(obj: [{ stringField: \"abc\" }, { stringField: \"efg\" }]) { StringField } }");

            Assert.AreEqual("abc", ((IEnumerable<dynamic>)result.Data.withObjectListArg).ElementAt(0).StringField);
            Assert.AreEqual("efg", ((IEnumerable<dynamic>)result.Data.withObjectListArg).ElementAt(1).StringField);
        }

        [Test]
        public void Execute_AstObjectListArgumentWithListProperty_CorrectlyTranslatesIntoOutput()
        {
            var result = this.schema.Execute("{ withObjectListArg(obj: { stringArray: [\"abc\", \"efg\"] }) { StringArray } }");

            Assert.AreEqual("abc", ((IEnumerable<dynamic>)((IEnumerable<dynamic>)result.Data.withObjectListArg).ElementAt(0).StringArray).ElementAt(0));
            Assert.AreEqual("efg", ((IEnumerable<dynamic>)((IEnumerable<dynamic>)result.Data.withObjectListArg).ElementAt(0).StringArray).ElementAt(1));
        }

        [Test]
        public void Execute_WithNestedListSingleValueArgument_CorrectlyTranslatesIntoOutput()
        {
            var result = this.schema.Execute(@"
                {
                    withObjectNestedListArg(obj: [
                        [
                            { stringArray: ""abc"" },
                            { stringArray: [""ABC"", ""EFG""] },
                        ],
                        { stringArray: [""hij"", ""klm""] }
                    ]) { 
                        StringArray 
                    }
                }");
            
            Assert.AreEqual("abc", result.Data.withObjectNestedListArg[0][0].StringArray[0]);
            Assert.AreEqual("klm", result.Data.withObjectNestedListArg[1][0].StringArray[1]);
        }

        [Test]
        public void Execute_SingleValueInNestedList_CorrectlyTranslatesIntoOutput()
        {
            var result = this.schema.Execute(@"
                {
                    withNestedArray(matrix: [
                        [
                            [1, 2],
                            3
                        ],
                        [1, 2, [1, 2, 3]],
                        4
                    ])
                }");

            var expectedResult = new int?[][][]
            {
                new int?[][]
                {
                    new int?[] { 1, 2 },
                    new int?[] { 3 }
                },
                new int?[][]
                {
                    new int?[] { 1 },
                    new int?[] { 2 },
                    new int?[] { 1, 2, 3 }
                },
                new int?[][]
                {
                    new int?[] { 4 },
                }
            };

            Assert.AreEqual(expectedResult, result.Data.withNestedArray);
        }

        [Test]
        public void Execute_EnumSingleValue_CorrectlyTranslatesIntoOutput()
        {
            var result = this.schema.Execute(@"
                {
                    withObjectArg(obj: {
                        enumField: [One]
                    }) {
                        enumField
                    }
                }");

            Assert.AreEqual(new string[] { TestEnum.One.ToString() }, result.Data.withObjectArg.enumField);
        }

        [Test]
        public void Execute_NullValue_CorrectlyTranslatesIntoOutput()
        {
            var result = this.schema.Execute(@"
                {
                    isNull(nonMandatory: null)
                }");

            Assert.IsTrue(result.Data.isNull);
        }

        [Test]
        public void Execute_WithDefaultValue_ReturnsDefaultValue()
        {
            var result = this.schema.Execute(@"
                {
                    withDefaultValue
                }");

            Assert.AreEqual("default", result.Data.withDefaultValue);
        }

        [Test]
        public void Execute_WithComplexDefaultValue_ReturnsDefaultValue()
        {
            var result = this.schema.Execute(@"
                {
                    withDefaultComplexValue {
                        stringField : StringField
                    }
                }");

            Assert.AreEqual("defaultCreatedByObject", result.Data.withDefaultComplexValue.stringField);
        }

        [Test]
        public void Execute_WithComplexDefaultValueInField_ReturnsDefaultValue()
        {
            var result = this.schema.Execute(@"
                {
                    withDefaultComplexValue (obj: {}) {
                        stringField : StringField
                        withSupplied : defaultField(argument: ""abc"")
                        withNull : defaultField(argument: null)
                        withDefault : defaultField
                    }
                }");

            Assert.AreEqual("default", result.Data.withDefaultComplexValue.stringField);
            Assert.AreEqual("default+abc", result.Data.withDefaultComplexValue.withSupplied);
            Assert.AreEqual("default+", result.Data.withDefaultComplexValue.withNull);
            Assert.AreEqual("default+defaultArgument", result.Data.withDefaultComplexValue.withDefault);
        }

        [Test]
        public void Execute_WithDirectiveWithDefaultValue_UsesDefaultValue()
        {
            var result = this.schema.Execute(@"
                {
                    withDefaultComplexValue {
                        stringField : StringField @default
                    }
                }");

            Assert.AreEqual("default value", result.Data.withDefaultComplexValue.stringField);
        }

        [SetUp]
        public void SetUp()
        {
            this.schema = new GraphQLSchema();
            var rootType = new RootQueryType(this.schema);
            var nestedTypeNonGeneric = new NestedNonGenericQueryType();
            var nestedType = new NestedQueryType(nestedTypeNonGeneric);

            this.schema.AddKnownType(rootType);
            this.schema.AddKnownType(nestedTypeNonGeneric);
            this.schema.AddKnownType(nestedType);
            this.schema.AddKnownType(new InputTestObjectType());
            this.schema.AddKnownType(new TestEnumType());
            this.schema.AddOrReplaceDirective(new DefaultArgumentDirectiveType());

            this.schema.Query(rootType);
        }

        private class DefaultArgumentDirectiveType : GraphQLDirectiveType
        {
            public DefaultArgumentDirectiveType() : base("default", "", DirectiveLocation.FIELD)
            {
                this.Argument("default")
                    .WithDescription("description")
                    .WithDefaultValue("default value");
            }

            public override LambdaExpression GetResolver(Func<Task<object>> valueGetter, object parentValue)
            {
                Expression<Func<string, object>> resolver = (@default) => @default;

                return resolver;
            }
        }

        private class NestedNonGenericQueryType : GraphQLObjectType
        {
            public NestedNonGenericQueryType() : base("NestedNonGenericQueryType", "")
            {
                this.Field("text", (int id, string str) => $"I received {id} with {str}");
            }
        }

        private class NestedQueryType : GraphQLObjectType<TestObject>
        {
            public NestedQueryType(NestedNonGenericQueryType nestedTypeNonGeneric) : base("NestedQueryType", "")
            {
                this.Field(instance => instance.Id);
                this.Field(instance => instance.StringField);
                this.Field(instance => instance.StringArray);
                this.Field("enumField", instance => instance.Enum);
                this.Field("nested", (int id) => nestedTypeNonGeneric);
                this.Field("defaultField", (IContext<TestObject> context, string argument) => $"{context.Instance.StringField}+{argument}")
                    .WithDefaultValue("argument", "defaultArgument");
            }
        }

        private class InputTestObjectType : GraphQLInputObjectType<TestObject>
        {
            public InputTestObjectType() : base("InputTestObjectType", "")
            {
                this.Field("stringField", instance => instance.StringField).WithDefaultValue("default");
                this.Field("stringArray", instance => instance.StringArray);
                this.Field("enumField", instance => instance.Enum);
            }
        }

        private class RootQueryType : GraphQLObjectType
        {
            public RootQueryType(GraphQLSchema schema) : base("RootQueryType", "")
            {
                this.Field("nested", (int id) => new TestObject() { Id = id, StringField = "Test with id " + id });
                this.Field("withArray", (int[] ids) => ids.Count());
                this.Field("isNull", (int? nonMandatory) => !nonMandatory.HasValue);
                this.Field("withFloat", (float value) => value);
                this.Field("withList", (List<int> ids) => ids);
                this.Field("withIEnumerable", (IEnumerable<int> ids) => ids.Count());
                this.Field("withObjectArg", (TestObject obj) => obj);
                this.Field("withObjectListArg", (IEnumerable<TestObject> obj) => obj);
                this.Field("withObjectNestedListArg", (IEnumerable<IEnumerable<TestObject>> obj) => obj);
                this.Field("withNestedArray", (int?[][][] matrix) => matrix);
                this.Field("withDefaultValue", (string value) => value).WithDefaultValue("value", "default");
                this.Field("withDefaultComplexValue", (TestObject obj) => obj)
                    .WithDefaultValue("obj", new TestObject() { Id = 1, StringField = "defaultCreatedByObject" });
            }
        }

        private class TestEnumType : GraphQLEnumType<TestEnum>
        {
            public TestEnumType() : base("TestEnumType", "")
            {
            }
        }

        private class TestObject
        {
            public int Id { get; set; }
            public string StringField { get; set; }
            public string[] StringArray { get; set; }
            public TestEnum[] Enum { get; set; }
        }

        private enum TestEnum { One, Two, Three }
    }
}
