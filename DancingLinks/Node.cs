using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
	public class Node
	{
		public Node North { get; set; }

		public Node East { get; set; }

		public Node South { get; set; }

		public Node West { get; set; }

		public Node Header { get; set; }

		public Label Label { get; set; }

		public int Children { get; set; }

		public Node(Label label)
		{
			Label = label;
			Children = 0;
		}

		public Node()
        {
			Label = new Label();
			Children = 0;
        }
	}

	public class Label
	{
		public int Row { get; set; }

		public int Column { get; set; }

		public int Block { get; set; }

		public int Value { get; set; }

		public ConstraintType Constraint { get; set; }

		public Label(int r, int c, int b, int v, ConstraintType constraint)
        {
			Row = r;
			Column = c;
			Block = b;
			Value = v;
			Constraint = constraint;
        }

		public Label()
        {
			Row = 0;
			Column = 0;
			Block = 0;
			Value = 0;
			Constraint = 0;
		}

        public bool Equals(Label obj)
        {
			return (Row == obj.Row 
				&& Column == obj.Column 
				&& Block == obj.Block
				&& Value == obj.Value
				&& Constraint == obj.Constraint);
        }
    }

	public enum ConstraintType
    {
		None = 0,
		Box = 1,
		Row = 2,
		Column = 3, 
		Block = 4
    }
}
