using System;
using System.Collections.Generic;
using System.Text;

namespace Runes.Text
{
    public sealed class TextCaseConstraint
    {
        public static TextCaseConstraint ANY = new TextCaseConstraint(0, "Any");
        public static TextCaseConstraint LOWERCASED = new TextCaseConstraint(1, "Lowercased");
        public static TextCaseConstraint UPPERCASED = new TextCaseConstraint(2, "Uppercased");

        private TextCaseConstraint(int id, string constraint)
        {
            Id = id;
            Constraint = constraint;
        }

        public int Id { get; }

        public string Constraint { get; }

        public bool Equals(TextCaseConstraint other) =>
            other != null && other.Id == Id;

        public override bool Equals(object obj) =>
            obj is TextCaseConstraint other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Constraint;
    }
}
