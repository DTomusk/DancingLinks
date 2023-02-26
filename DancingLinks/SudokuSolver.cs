using System;

namespace DancingLinks
{
	public class SudokuSolver
	{
		private Node root;
		private Node[] solutionSet;
		private int[,] solvedSudoku;

		public void SolveSudoku(Node matrixRoot)
		{
			solutionSet = new Node[81];
			solvedSudoku = new int[9, 9];
			root = matrixRoot;

			Console.WriteLine("Searching for sudoku solution");

			findNextSolutionNode(0);
		}

		/// <summary>
		/// Recursive function that finds the kth row in the solution set 
		/// </summary>
		/// <param name="k"></param>
		private void findNextSolutionNode(int k)
		{ 
			// If there are no more columns left in the matrix, then a solution has been found
			if (root.East == root)
            {
				printSolution();
				Console.ReadLine();
				return;
            }
			
			// Covering the column with the least children reduces the branching factor of the algorithm
			Node header = findColumnWithLeastChildren();
			coverColumn(header);

			Node currentNode = header.South;

			// Loop over all offspring of the column header
			while (currentNode != header)
            {
				// Assume the chosen node is in the solution
				solutionSet[k] = currentNode;
				Node nodeInRow = currentNode.East;
				while (nodeInRow != currentNode)
                {
					coverColumn(nodeInRow.Header);
					nodeInRow = nodeInRow.East;
                }
				findNextSolutionNode(k + 1);
				currentNode = solutionSet[k];
				header = currentNode.Header;
				
				nodeInRow = currentNode.West;
				while (nodeInRow != currentNode)
                {
					uncoverColumn(nodeInRow.Header);
					nodeInRow = nodeInRow.West;
                }
            }

			// If all of the nodes in the column have been iterated over and a solution hasn't been found
			// Then uncover the column and keep searching 
			uncoverColumn(header);
			return;
		}

		/// <summary>
		/// Disconnects the given header from the matrix by connecting its east and west nodes to each other
		/// For any nodes in the column, all of their sibling nodes are disconnected from their columns
		/// This effectively removes all the populated rows in the column from the matrix 
		/// </summary>
		private void coverColumn(Node header)
		{
			header.West.East = header.East;
			header.East.West = header.West;

			Node currentChild = header.South;

			// for each child of the header 
			while (currentChild != header)
			{
				// remove all of the siblings 
				Node currentSibling = currentChild.East;
				while (currentSibling != currentChild)
				{
					currentSibling.North.South = currentSibling.South;
					currentSibling.South.North = currentSibling.North;
					currentSibling.Header.Children--;
					currentSibling = currentSibling.East;
				}
				currentChild = currentChild.South;
			} 
		}

		/// <summary>
		/// The reverse of the coverColumn method 
		/// Working in reverse order to coverColumn (north and west rather than south and east),
		/// this connects a column and all of the associated rows to their original place in the matrix
		/// Care needs to be taken to ensure this is done in the correct order 
		/// </summary>
		/// <param name="header"></param>
		private void uncoverColumn(Node header)
		{
			Node currentNode = header.North;
			while (currentNode != header)
            {
				Node currentSibling = currentNode.West;
				while (currentSibling != currentNode)
                {
					currentSibling.Header.Children++;
					currentSibling.South.North = currentSibling;
					currentSibling.North.South = currentSibling;
					currentSibling = currentSibling.West;
                }
				currentNode = currentNode.North;
            }
			header.East.West = header;
			header.West.East = header;
		}

		private Node findColumnWithLeastChildren()
		{
			Node searchNode = root.East;
			Node minNode = searchNode;
			int minChildren = searchNode.Children;

			// Find the column with the lowest number of offspring
			while (searchNode != root)
			{
				if (searchNode.Children < minChildren)
				{
					minChildren = searchNode.Children;
					minNode = searchNode;
				}
				searchNode = searchNode.East;
			}

			return minNode;
		}

		/// <summary>
		/// Populates solvedSudoku with values based on the labels of the nodes in the solutionSet and then prints them
		/// </summary>
		/// <param name="solution"></param>
		private void printSolution()
		{ 
			for (int i = 0; i < solutionSet.Length; i++)
            {
				if (solutionSet[i] != null)
                {
					Node printNode = solutionSet[i];

					solvedSudoku[printNode.Label.Row - 1, printNode.Label.Column - 1] = printNode.Label.Value;
                }
            }

			for (int i = 0; i < 9; i++)
            {
				for (int j = 0; j < 9; j++)
                {
					Console.Write($"{solvedSudoku[i, j]} ");
                }
				Console.WriteLine();
            }

			Console.WriteLine("Solution has been printed");
		}
	}
}
