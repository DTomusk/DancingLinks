using System;

namespace DancingLinks
{
	public class SudokuGenerator
	{
		const string SQUARE_HAS_VALUE = "SquareContainsValue";
		const string ROW_CONTAINS_NUMBER = "RowContainsNumber";
		const string COLUMN_CONTAINS_NUMBER = "ColumnContainsNumber";
		const string BLOCK_CONTAINS_NUMBER = "BlockContainsNumber";
		// Currently not used fully as FindBlockFromCoordinates behaves independently of SUDOKU_DIMENSION
		const int SUDOKU_DIMENSION = 9;
		const bool TESTING_ENABLED = true;
		const int EXPECTED_NUMBER_OF_CONSTRAINTS = 324;
		// Currently not used, hard to verify whether there are 729 distinct rows in the data structure 
		const int EXPECTED_NUMBER_OF_CHOICES = 729;
		const int EXPECTED_NUMBER_OF_CHILDREN_PER_HEADER = 9;
		const int EXPECTED_NUMBER_OF_NODES_PER_ROW = 4;

		public Node GenerateSudokuConstraints()
		{
			Node root = createRoot();
			addColumns(root);
			addPossibleMoves(root);
			if (TESTING_ENABLED)
			{
				verifyNodeStructure(root);
				Console.WriteLine("All tests passed, data structure has been verified");
			}
			return root;
		}

		#region Populating Data Structure
		private Node createRoot()
		{
			Node root = new Node("root");
			root.North = root;
			root.South = root;

			return root;
		}
		
		private void addColumns(Node root)
		{
			Node currentNode = root;

			currentNode = addSetOfConstraints(currentNode, SQUARE_HAS_VALUE);
			currentNode = addSetOfConstraints(currentNode, ROW_CONTAINS_NUMBER);
			currentNode = addSetOfConstraints(currentNode, COLUMN_CONTAINS_NUMBER);
			currentNode = addSetOfConstraints(currentNode, BLOCK_CONTAINS_NUMBER);

			currentNode.East = root;
			root.West = currentNode;
		}

		private Node addSetOfConstraints(Node currentNode, string label)
		{
			for (int i = 1; i <= SUDOKU_DIMENSION; i++)
			{
				for (int j = 1; j <= SUDOKU_DIMENSION; j++)
				{
					currentNode.East = new Node(label + $" ({i}, {j})")
					{
						West = currentNode
					};
					currentNode = currentNode.East;
				}
			}

			return currentNode;
		}

		private void addPossibleMoves(Node root)
		{
			Node headerNode;
			Node currentNode;
			// Iterate over the numbers that can be put in a square
			for (int val = 1; val <= SUDOKU_DIMENSION; val++)
			{
				// Iterate over the rows 
				for (int i = 1; i <= SUDOKU_DIMENSION; i++)
				{
					// Iterate over the columns
					for (int j = 1; j <= SUDOKU_DIMENSION; j++)
					{
						Node firstNode = new Node($"Value: {val} at ({i}, {j})");
						currentNode = firstNode;
						headerNode = root.East;

						currentNode = appendChildNode(headerNode, currentNode, SQUARE_HAS_VALUE, i, j);
						currentNode = appendChildNode(headerNode, currentNode, ROW_CONTAINS_NUMBER, i, val);
						currentNode = appendChildNode(headerNode, currentNode, COLUMN_CONTAINS_NUMBER, j, val);
						currentNode = appendChildNode(headerNode, currentNode, BLOCK_CONTAINS_NUMBER, FindBlockFromCoordinates(i, j), val);
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

		// Appends appendNode to the southmost offspring of headerNode and returns a node that will be appended to the next constraint
		private Node appendChildNode(Node headerNode, Node appendNode, string searchString, int i, int j)
		{
			while (headerNode.Label != $"{searchString} ({i}, {j})")
			{
				headerNode = headerNode.East;
				if (headerNode.Label == "root")
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

			headerNode.Children += 1;

			return appendNode.East;
		}
		#endregion

		#region Testing

		// A series of tests to verify that the data structure is as expected
		private void verifyNodeStructure(Node root)
		{
			Console.WriteLine("Runing Data Structure Tests");
			Console.WriteLine("Testing East-West header connections");

			// Test that the headers are connected east to west and west to east to the root
			if (root.East == null)
				throw new Exception("Root has no east node");

			Node currentNode = root.East;
			int numberOfHeaders = 0;

			while (currentNode.Label != "root")
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
				// To do: run tests on the header's children 
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
