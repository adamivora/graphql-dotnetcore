﻿namespace GraphQLCore.Tests.Language.Validation
{
    using Exceptions;
    using GraphQLCore.Language;
    using NUnit.Framework;

    [TestFixture]
    public class LexerValidationTests
    {
        [Test]
        public void Lex_InvalidCharacter_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\u0007"))));

            Assert.AreEqual("Syntax Error GraphQL (1:1) Invalid character \\u0007.", exception.Message);
        }

        [Test]
        public void Lex_UnterminatedString_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:2) Unterminated string.", exception.Message);
        }

        [Test]
        public void Lex_UnterminatedStringWithText_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"no end quote"))));

            Assert.AreEqual("Syntax Error GraphQL (1:14) Unterminated string.", exception.Message);
        }

        [Test]
        public void Lex_UnescapedControlChar_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"contains unescaped \u0007 control char"))));

            Assert.AreEqual("Syntax Error GraphQL (1:21) Invalid character within String: \\u0007.", exception.Message);
        }

        [Test]
        public void Lex_NullByteInString_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"null-byte is not \u0000 end of file"))));

            Assert.AreEqual("Syntax Error GraphQL (1:19) Invalid character within String: \\u0000.", exception.Message);
        }

        [Test]
        public void Lex_LineBreakInMiddleOfString_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"multi\nline\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Unterminated string.", exception.Message);
        }

        [Test]
        public void Lex_CarriageReturnInMiddleOfString_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"multi\rline\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Unterminated string.", exception.Message);
        }

        ////

        [Test]
        public void Lex_InvalidEscapeSequenceZetCharacter_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"bad \\z esc\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Invalid character escape sequence: \\z.", exception.Message);
        }

        [Test]
        public void Lex_InvalidEscapeSequenceXCharacter_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"bad \\x esc\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Invalid character escape sequence: \\x.", exception.Message);
        }

        [Test]
        public void Lex_InvalidUnicode_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"bad \\u1 esc\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Invalid character escape sequence: \\u1 es.", exception.Message);
        }

        [Test]
        public void Lex_InvalidUnicode2_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"bad \\u0XX1 esc\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Invalid character escape sequence: \\u0XX1.", exception.Message);
        }

        [Test]
        public void Lex_InvalidUnicode3_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"bad \\uFXXX esc\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Invalid character escape sequence: \\uFXXX.", exception.Message);
        }

        [Test]
        public void Lex_InvalidUnicode4_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"bad \\uXXXX esc\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Invalid character escape sequence: \\uXXXX.", exception.Message);
        }

        [Test]
        public void Lex_InvalidUnicode5_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\"bad \\uXXXF esc\""))));

            Assert.AreEqual("Syntax Error GraphQL (1:7) Invalid character escape sequence: \\uXXXF.", exception.Message);
        }


        [Test]
        public void Lex_NumberDoubleZeros_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("00"))));

            Assert.AreEqual("Syntax Error GraphQL (1:2) Invalid number, unexpected digit after 0: \"0\"", exception.Message);
        }

        [Test]
        public void Lex_NumberPlusOne_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("+1"))));

            Assert.AreEqual("Syntax Error GraphQL (1:1) Unexpected character \"+\"", exception.Message);
        }

        [Test]
        public void Lex_NumberNoDecimalPartEOFInstead_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("1."))));

            Assert.AreEqual("Syntax Error GraphQL (1:3) Invalid number, expected digit but got: <EOF>", exception.Message);
        }

        [Test]
        public void Lex_NumberStartingWithDot_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source(".123"))));

            Assert.AreEqual("Syntax Error GraphQL (1:1) Unexpected character \".\"", exception.Message);
        }

        [Test]
        public void Lex_NonNumericCharInNumber_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("1.A"))));

            Assert.AreEqual("Syntax Error GraphQL (1:3) Invalid number, expected digit but got: \"A\"", exception.Message);
        }

        [Test]
        public void Lex_NonNumericCharInNumber2_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("-A"))));

            Assert.AreEqual("Syntax Error GraphQL (1:2) Invalid number, expected digit but got: \"A\"", exception.Message);
        }

        [Test]
        public void Lex_MissingExponentInNumber_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("1.0e"))));

            Assert.AreEqual("Syntax Error GraphQL (1:5) Invalid number, expected digit but got: <EOF>", exception.Message);
        }

        [Test]
        public void Lex_NonNumericCharacterInNumberExponent_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("1.0eA"))));

            Assert.AreEqual("Syntax Error GraphQL (1:5) Invalid number, expected digit but got: \"A\"", exception.Message);
        }

        [Test]
        public void Lex_IncompleteSpread_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source(".."))));

            Assert.AreEqual("Syntax Error GraphQL (1:1) Unexpected character \".\"", exception.Message);
        }

        [Test]
        public void Lex_LonelyQuestionMark_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("?"))));

            Assert.AreEqual("Syntax Error GraphQL (1:1) Unexpected character \"?\"", exception.Message);
        }

        [Test]
        public void Lex_NotAllowedUnicode_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\\u203B"))));

            Assert.AreEqual("Syntax Error GraphQL (1:1) Unexpected character \"\\u203B\"", exception.Message);
        }

        [Test]
        public void Lex_NotAllowedUnicode1_ThrowsExceptionWithCorrectMessage()
        {
            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("\\u200b"))));

            Assert.AreEqual("Syntax Error GraphQL (1:1) Unexpected character \"\\u200b\"", exception.Message);
        }

        [Test]
        public void Lex_DashesInName_ThrowsExceptionWithCorrectMessage()
        {
            var token = new Lexer().Lex(new Source("a-b"));

            Assert.AreEqual(TokenKind.NAME, token.Kind);
            Assert.AreEqual(0, token.Start);
            Assert.AreEqual(1, token.End);
            Assert.AreEqual("a", token.Value);

            var exception = Assert.Throws<GraphQLException>(
                new TestDelegate(() => new Lexer().Lex(new Source("a-b"), token.End)));

            Assert.AreEqual("Syntax Error GraphQL (1:3) Invalid number, expected digit but got: \"b\"", exception.Message);
        }
    }
}
