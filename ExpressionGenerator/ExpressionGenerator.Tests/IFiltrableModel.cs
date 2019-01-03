using System;

namespace ExpressionGenerator.Tests
{
    internal interface IFiltrableModel
    {
        string Name { get; }

        int Age { get; }

        DateTime CreateDate { get; }
    }
}