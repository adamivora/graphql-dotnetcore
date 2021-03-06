﻿namespace GraphQLCore.Tests.Validation.Rules
{
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class UniqueInputFieldNamesTests : ValidationTestBase
    {
        [Test]
        public void InputObjectWithFields()
        {
            var errors = this.Validate(@"
            {
                field(arg: { f: true }) { foo }
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void SameInputObjectWithTwoArgs()
        {
            var errors = this.Validate(@"
            {
                field(arg: { f: true }, arg1: { f: true }) { foo }
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void MultipleInputObjectFields()
        {
            var errors = this.Validate(@"
            {
                 "+"field(arg: { f1: \"value\", f2: \"value\", f3: \"value\" })"+ @" { foo }
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void AllowsForNestedObjectsWithSimilarFields()
        {
            var errors = this.Validate(@"
            {
                field(arg: {
                    deep: {
                        deep: {
                            id: 1
                        }
                        id: 1
                    }
                    id: 1
                }) { foo }
            }
            ");

            Assert.IsEmpty(errors);
        }

        [Test]
        public void DuplicateInputObjectFields()
        {
            var errors = this.Validate(@"
            {
                " + "field(arg: { f1: \"value\", f1: \"value\"})" + @"  { foo }
            }
            ");

            ErrorAssert.AreEqual("There can be only one input field named \"f1\".",
                errors.Single(), new[] { 3, 30 }, new[] { 3, 43 });
        }

        [Test]
        public void ManyDuplicateInputObjectFields()
        {
            var errors = this.Validate(@"
            {
                " + "field(arg: { f1: \"value\", f1: \"value\", f1: \"value\"})" + @" { foo }
            }
            ");

            Assert.AreEqual(2, errors.Count());

            ErrorAssert.AreEqual("There can be only one input field named \"f1\".",
                errors.ElementAt(0), new[] { 3, 30 }, new[] { 3, 43 });
            ErrorAssert.AreEqual("There can be only one input field named \"f1\".",
                errors.ElementAt(1), new[] { 3, 30 }, new[] { 3, 56 });
        }
    }
}
