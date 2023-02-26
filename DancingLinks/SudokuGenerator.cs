using System;

namespace DancingLinks
{
	public class SudokuGenerator
	{
		// Currently not used fully as FindBlockFromCoordinates behaves independently of SUDOKU_DIMENSION
		const int SUDOKU_DIMENSION = 9;
		const int EXPECTED_NUMBER_OF_CONSTRAINTS = 324;
		const int EXPECTED_NUMBER_OF_CHILDREN_PER_HEADER = 9;
		const int EXPECTED_NUMBER_OF_NODES_PER_ROW = 4;

		Node root;

		public Node GenerateSudokuConstraints(bool testing)
		{
			root = createRoot();
			addColumns();
			addPossibleMoves();
			if (testing)
			{
				verifyNodeStructure();
				Console.WriteLine("All tests passed, data structure has been verified");
			}
			return root;
		}

		#region Populating Data Structure
		/// <summary>
		/// Creates the root of the matrix which serves as a universal starting point for traversing the matrix
		/// </summary>
		private Node createRoot()
		{
			Node root = new Node();
			root.North = root;
			root.South = root;

			return root;
		}
		
		/// <summary>
		/// Adds all the columns (constraints) to the matrix 
		/// Each of the four types of constraint is added separately
		/// </summary>
		private void addColumns()
		{
			Node currentNode = root;

			currentNode = addSetOfConstraints(currentNode, ConstraintType.Box);
			currentNode = addSetOfConstraints(currentNode, ConstraintType.Row);
			currentNode = addSetOfConstraints(currentNode, ConstraintType.Column);
			currentNode = addSetOfConstraints(currentNode, ConstraintType.Block);

			currentNode.East = root;
			root.West = currentNode;
		}

		/// <summary>
		/// Adds 81 constraint columns to the matrix corresponding to the type of constraint specified by constraint
		/// </summary>
		private Node addSetOfConstraints(Node currentNode, ConstraintType constraint)
		{
			for (int i = 1; i <= SUDOKU_DIMENSION; i++)
			{
				for (int j = 1; j <= SUDOKU_DIMENSION; j++)
				{
					currentNode.East = new Node(getColumnLabel(i, j, constraint))
					{
						West = currentNode
					};
					currentNode = currentNode.East;
					currentNode.Header = currentNode;
				}
			}

			return currentNode;
		}

		/// <summary>
		/// Adds all 729 possible sudoku moves to their respective colummns in the matrix 
		/// </summary>
		private void addPossibleMoves()
		{
			Node headerNode;
			Node currentNode;
			// Iterate over the numbers that can be put in a square
			for (int val = 1; val <= SUDOKU_DIMENSION; val++)
			{
				// Iterate over the rows 
				for (int row = 1; row <= SUDOKU_DIMENSION; row++)
				{
					// Iterate over the columns
					for (int col= 1; col <= SUDOKU_DIMENSION; col++)
					{
						// Each move fulfils four contraints
						Node firstNode = new Node(new Label(row, col, FindBlockFromCoordinates(row, col), val, 0));
						headerNode = root.East;

						currentNode = appendChildNode(firstNode, ConstraintType.Box, row, col);
						currentNode = appendChildNode(currentNode, ConstraintType.Row, row, val);
						currentNode = appendChildNode(currentNode, ConstraintType.Column, col, val);
						currentNode = appendChildNode(currentNode, ConstraintType.Block, FindBlockFromCoordinates(row, col), val);
						currentNode.West.East = firstNode;
						firstNode.West = currentNode.West;
					}
				}
			}

			headerNode = root.East;
			currentNode = headerNode;

			while (headerNode != root)
			{
				while (currentNode.South != null)
					currentNode = currentNode.South;

				headerNode.North = currentNode;
				currentNode.South = headerNode;

				headerNode = headerNode.East;
				currentNode = headerNode;
			}
		}

		/// <summary>
		/// Using the label parameters provided, find the column for appendNode to be added to and add it to the bottom
		/// Return the next node to be appended 
		/// </summary>
		private Node appendChildNode(Node appendNode, ConstraintType constraintType, int i, int j)
		{
			// Find the column corresponding to the 
			Node headerNode = root.East;
			Label searchLabel = getColumnLabel(i, j, constraintType);

			while (!headerNode.Label.Equals(searchLabel))
			{
				headerNode = headerNode.East;
				if (headerNode == root)
					throw new Exception($"Header corresponding to ({i}, {j}) not found");
			}

			Node parentNode = headerNode;

			while (parentNode.South != null)
			{
				parentNode = parentNode.South;
			}

			parentNode.South = appendNode;
			appendNode.North = parentNode;

			appendNode.East = new Node(appendNode.Label);
			appendNode.East.West = appendNode;
			appendNode.Header = headerNode;

			headerNode.Children += 1;

			return appendNode.East;
		}

		/// <summary>
		/// Given the integers and constraint type provided, generate the appropriate label for a column header 
		/// </summary>
		public Label getColumnLabel(int i, int j, ConstraintType constraint)
        {
			switch (constraint)
			{
				case ConstraintType.Box:
					{
						return new Label(i, j, 0, 0, constraint);
					}
				case ConstraintType.Row:
					{
						return new Label(i, 0, 0, j, constraint);
					}
				case ConstraintType.Column:
					{
						return new Label(0, i, 0, j, constraint);
					}
				case ConstraintType.Block:
					{
						return new Label(i, j, FindBlockFromCoordinates(i, j), 0, constraint);
					}
				default:
					return new Label();
			}
		}
		#endregion

		#region Testing

		// A series of tests to verify that the data structure is as expected
		private void verifyNodeStructure()
		{
			Console.WriteLine("Runing Data Structure Tests");
			Console.WriteLine("Testing East-West header connections");

			// Test that the headers are connected east to west and west to east to the root
			if (root.East == null)
				throw new Exception("Root has no east node");

			Node currentNode = root.East;
			int numberOfHeaders = 0;

			while (currentNode != root)
			{
				// Test that each header has an east node
				if (currentNode.East == null)
					throw new Exception($"Header node with label {currentNode.Label} has no east node");
				// Test that each header has a west node 
				if (currentNode.East.West == null)
					throw new Exception($"Header node with label {currentNode.East.Label} has no west node");
				// Test that a header's east's west node is the header 
				if (currentNode.East.West != currentNode)
					throw new Exception($"The west of {currentNode.Label}'s east ({currentNode.East.Label} was {currentNode.East.West.Label})");
				if (currentNode.Children != EXPECTED_NUMBER_OF_CHILDREN_PER_HEADER)
					throw new Exception($"Header with label {currentNode.Label} has {currentNode.Children} children rather than the expected {EXPECTED_NUMBER_OF_CHILDREN_PER_HEADER}");

				runTestsOnHeaderChildren(currentNode);

				currentNode = currentNode.East;
				numberOfHeaders += 1;
			}

			Console.WriteLine("Test Passed: Each header has East and West nodes that are properly connected");

			if (numberOfHeaders != EXPECTED_NUMBER_OF_CONSTRAINTS)
				throw new Exception($"Number of constraints wasn't the expected {EXPECTED_NUMBER_OF_CONSTRAINTS}, it was: {numberOfHeaders}");

			Console.WriteLine("Test Passed: Number of headers matched the expected number of constraints");
		}

		private void runTestsOnHeaderChildren(Node header)
		{
			Console.WriteLine($"Running tests on the children of header {header.Label}");

			// Test that there are four nodes in a row
			if (header.South == null)
				throw new Exception($"Header with label {header.Label} has no children");

			Node currentNode = header.South;
			int numberOfOffspring = 0;

			while (currentNode != header)
			{
				checkAllDirections(currentNode);
				runTestsOnRow(currentNode);

				currentNode = currentNode.South;
				numberOfOffspring += 1;

				if (currentNode.Header != header)
					throw new Exception($"Header of node with label {currentNode.Label} was not the expected {header.Label}, it was {currentNode.Header.Label}");
			}

			if (numberOfOffspring != EXPECTED_NUMBER_OF_CHILDREN_PER_HEADER)
				throw new Exception($"Header with label {header.Label} has {numberOfOffspring} offspring and not the expected {EXPECTED_NUMBER_OF_CHILDREN_PER_HEADER}");

			// Test that they are correctly connected
		}

		private void runTestsOnRow(Node originalNode)
		{
			Console.WriteLine($"Checking the row with nodes corresponding to the choice: {originalNode.Label}");

			Node currentNode = originalNode;
			int numberOfNodesInRow = 1;

			while (currentNode.East != originalNode)
			{
				checkAllDirections(currentNode);
				currentNode = currentNode.East;
				numberOfNodesInRow += 1;
			}

			if (numberOfNodesInRow != EXPECTED_NUMBER_OF_NODES_PER_ROW)
				throw new Exception($"Expected {EXPECTED_NUMBER_OF_NODES_PER_ROW} nodes in row, instead got {numberOfNodesInRow}");

			Console.WriteLine($"Test Passed: row containing nodes labelled {originalNode.Label} has the expected number of nodes");
		}

		private void checkAllDirections(Node node)
		{
			if (node.South == null)
				throw new Exception($"Node with label {node.Label} does not have a child");
			if (node.North == null)
				throw new Exception($"Node with label {node.Label} does not have a parent");
			if (node.East == null)
				throw new Exception($"Node with label {node.Label} does not have an east node");
			if (node.West == null)
				throw new Exception($"Node with label {node.Label} does not have a west node");
		}

		#endregion

		#region Utility Functions
		private int FindBlockFromCoordinates(int i, int j)
		{
			if (i >= 1 && i <= 3)
			{
				if (j >= 1 && j <= 3)
					return 1;
				else if (j <= 6)
					return 2;
				else if (j <= 9)
					return 3;
				else throw new Exception($"j was not within the range (1-9): {j}.");
			}
			else if (i <= 6)
			{
				if (j >= 1 && j <= 3)
					return 4;
				else if (j <= 6)
					return 5;
				else if (j <= 9)
					return 6;
				else throw new Exception($"j was not within the range (1-9): {j}.");
			}
			else if (i <= 9)
			{
				if (j >= 1 && j <= 3)
					return 7;
				else if (j <= 6)
					return 8;
				else if (j <= 9)
					return 9;
				else throw new Exception($"j was not within the range (1-9): {j}.");
			}
			else throw new Exception($"i was not within the range (1-9): {i}.");
		}
		#endregion
	}
}
