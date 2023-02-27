using System;
using System.Diagnostics;

namespace DancingLinks
{
	public class SudokuSolver
	{
		private Node root;
		private Node[] solutionSet;
		private int[,] sudokuState;
		private readonly Stopwatch timer = new Stopwatch();

		public void SolveSudoku(Node matrixRoot)
		{
			solutionSet = new Node[81];
			sudokuState = new int[9, 9];
			root = matrixRoot;

			addUserMoves();

			Console.WriteLine("Initial state of sudoku: \n");

			printSudokuState();

			Console.WriteLine("Searching for sudoku solution...\n");

			timer.Start();

			findNextSolutionNode(0);
		}

        #region Algorithm DLX
        /// <summary>
        /// Recursive function that finds the kth row in the solution set 
        /// </summary>
        private void findNextSolutionNode(int k)
		{
			Console.WriteLine($"Attempting to find the {k} value in the solution set");
			// If there are no more columns left in the matrix, then a solution has been found
			if (root.East == root)
            {
				printSudokuState();

				timer.Stop();

				Console.WriteLine($"Time taken to solve sudoku: {timer.ElapsedMilliseconds} ms");
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

				// Once the solution has been found we can simply exit the loop
				if (root.East == root)
					return;

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
		/// </summary>
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
		#endregion

		#region UI

		/// <summary>
		/// Allows users to fill in the initial state of the sudoku
		/// </summary>
		private void addUserMoves()
        {
			bool done = false;

			while (!done)
			{
				Console.WriteLine("Input a move as three digits or type 'd' when done");
				string input = Console.ReadLine();
				if (input == "d")
					done = true;
				else
                {
					// Expect a very specific format, currently no protection against invalid inputs 
					int[] userMove = Array.ConvertAll(input.ToCharArray(), i => (int)char.GetNumericValue(i));

					// TODO: given the user move, we need to find the nodes that correspond to that move and cover 
					// all their columns 

					Console.WriteLine($"Insert value: {userMove[0]} at ({userMove[1]}, {userMove[2]})");

					Node moveNode = findNodeCorrespondingToMove(userMove);

					Console.WriteLine($"Found node corresponding to move {moveNode.Label}");

					sudokuState[userMove[1] - 1, userMove[2] - 1] = userMove[0];

					printSudokuState();

					Node coverNode = moveNode;

					do
					{
						coverColumn(coverNode.Header);
						coverNode = coverNode.East;
					} while (coverNode != moveNode);
                }
			}
        }

		private Node findNodeCorrespondingToMove(int[] move)
        {
			Node headerNode = root;
			Label headerLabel = new Label(move[1], move[2], 0, 0, ConstraintType.Cell);

			while (!headerNode.Label.Equals(headerLabel))
            {
				headerNode = headerNode.East;
				if (headerNode == root)
					throw new Exception("No header found corresponding to move");
            }

			Node searchNode = headerNode.South;
			
			while (searchNode.Label.Value != move[0])
            {
				searchNode = searchNode.South;
				if (searchNode == headerNode)
					throw new Exception("No node found corresponding to move");
            }

			return searchNode;
        }

        #endregion

        /// <summary>
        /// Populates solvedSudoku with values from the labels of the nodes in the solutionSet and prints them
        /// </summary>
        /// <param name="solution"></param>
        private void printSudokuState()
		{
			setSudokuStateFromNodes();

			Console.WriteLine("-------------------------");

			for (int i = 0; i < 9; i++)
            {
				for (int j = 0; j < 9; j++)
                {
					if (j == 0)
						Console.Write("| ");
					Console.Write($"{sudokuState[i, j]} ");
					if (j % 3 == 2)
						Console.Write("| ");
                }
				Console.WriteLine();

				if (i % 3 == 2)
					Console.WriteLine("-------------------------");
            }

			Console.WriteLine();
		}

		/// <summary>
		/// Translates node labels from the solution set to values in the sudoku state 
		/// </summary>
		private void setSudokuStateFromNodes()
        {
			for (int i = 0; i < solutionSet.Length; i++)
			{
				if (solutionSet[i] != null)
				{
					Node printNode = solutionSet[i];

					sudokuState[printNode.Label.Row - 1, printNode.Label.Column - 1] = printNode.Label.Value;
				}
			}
		}
	}
}
