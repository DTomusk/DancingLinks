namespace DancingLinks
{
	/// <summary>
	/// The nodes that make up the matrix containing the state of the sudoku, available moves etc.
	/// Nodes have pointers to their left, right, up and down adjacent nodes for ease of traversal 
	/// and to make removing nodes from and adding nodes to the matrix cheap 
	/// </summary>
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

	/// <summary>
	/// Used for labelling the nodes in the matrix 
	/// Allows for quick identification of which nodes correspond to which moves 
	/// and which headers correspond to which constraints 
	/// </summary>
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

		/// <summary>
		/// Compare whether two labels or equivalent
		/// </summary>
        public bool Equals(Label obj)
        {
			return (Row == obj.Row 
				&& Column == obj.Column 
				&& Block == obj.Block
				&& Value == obj.Value
				&& Constraint == obj.Constraint);
        }
    }

	/// <summary>
	/// Describes the constraints that a move can fulfil
	/// Cell: each cell must contain one and only one number
	/// Row: each row can only contain a digit 1-9 once
	/// Column: each column can only contain a digit 1-9 once
	/// Block: each 3*3 block can only contain a digit 1-9 once
	/// </summary>
	public enum ConstraintType
    {
		None = 0,
		Cell = 1,
		Row = 2,
		Column = 3, 
		Block = 4
    }
}
