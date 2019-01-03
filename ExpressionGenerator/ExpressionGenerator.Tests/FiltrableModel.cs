using System;

namespace ExpressionGenerator.Tests
{
    internal class FiltrableModel : IFiltrableModel
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime CreateDate { get; set; }
    }
}