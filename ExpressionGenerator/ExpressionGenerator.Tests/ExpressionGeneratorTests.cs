using System;
using Expressions;
using NUnit.Framework;

namespace ExpressionGenerator.Tests
{
    [TestFixture]
    public class ExpressionGeneratorTests
    {
        private static readonly ExpressionGenerator<FiltrableModel> ExpressionGenerator = new ExpressionGenerator<FiltrableModel>();

        private FiltrableModel[] _testData =
        {
            new FiltrableModel { Age = 78, Name = "Tony", CreateDate = DateTime.Parse("25.04.1940") },
            new FiltrableModel { Age = 32, Name = "Alf", CreateDate = DateTime.Parse("22.09.1986") },
            new FiltrableModel { Age = 15, Name = "Arty", CreateDate = DateTime.Parse("01.07.2003") },
        };


        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var expression = ExpressionGenerator.GenerateExpression(model => model.Age, 15, Operator.GreaterThan);
        }
    }
}