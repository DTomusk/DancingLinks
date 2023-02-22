using System;
using System.Collections.Generic;
using System.Linq;

namespace DancingLinks
{
	public class SudokuSolver
	{
		public void SolveSudoku(Node root, bool testing)
		{
			SolutionSet solution = new SolutionSet();

			// If the program is not in test mode then run the normal algorithm
			if (!testing)
			{
				while (root.East != root)
				{
					// Choose the column with the minimum number of children 
					Node minHeader = findColumnWithLeastChildren(root, testing);
					if (minHeader.Children == 0)
					{
						// backtrack
						Console.WriteLine("No more possible moves, need to backtrack");
					}
					else
					{
						// choose a row to add to the solution set 
						// for each node in the solution set, cover the column 
						// first: add row to the solution set 
						// second: connect the parents of each node to their children and update headers accordingly 
						// third: for each header: connect their siblings then go to all of their childrens' siblings and rewire their parents and children, updating headers as necessary 
						// cover columns and continue

						// As a prototype, let the user choose which moves to do and see what happens 
						Console.WriteLine("Enter move as a three digit number");
						string move = Console.ReadLine();
						Node nodeToAdd = findNodeForMove(move, root);

						solution.Push(nodeToAdd);

						coverAllColumnsInRow(nodeToAdd);

						solution.PrintSolutionSet();
					}
				}
				Console.WriteLine("Sudoku has been solved");
			}
			// Otherwise test a certain set of operations
			else
			{
				Console.WriteLine("Testing solver");
				runTests(root);
			}
		}

		// Return the column header with the least number of children 
		private Node findColumnWithLeastChildren(Node root, bool testing)
		{
			Node searchNode = root.East;
			int minChildren = searchNode.Children;

			// Find the column with the lowest number of members
			while (searchNode != root)
			{
				if (searchNode.Children < minChildren)
					minChildren = searchNode.Children;
				searchNode = searchNode.East;
			}

			searchNode = root.East;

			while (searchNode.Children != minChildren)
				searchNode = searchNode.East;

			if (testing)
				Console.WriteLine($"Header with least children was {searchNode.Label} with {searchNode.Children} children");

			return searchNode;
		}

		private void removeNode(Node node)
		{
			node.North.South = node.South;
			node.South.North = node.North;
			node.Header.Children--;
		}

		// Remove the header from the header row 
		// Connect all the childrens' siblings' parents to their children 
		private void coverColumn(Node header)
		{
			Console.WriteLine($"Covering column with header {header.Label}");
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
					removeNode(currentSibling);
					currentSibling = currentSibling.East;
				}
				currentChild = currentChild.South;
			} 
		}

		// Take the node added to the solution and for it and each of its siblings cover the column 
		private void coverAllColumnsInRow(Node node)
		{
			Node currentNode = node;
			do
			{
				coverColumn(currentNode.Header);
				currentNode = currentNode.East;
			} while (currentNode != node);
		}

		#region UI
		private Node findNodeForMove(string move, Node root)
		{
			// Break the input string down into digits
			// Assume the input consists of 3 digits 
			List<int> numbers = move.Select(digit => int.Parse(digit.ToString())).ToList();

			Console.WriteLine($"Trying to add the value {numbers[2]} to ({numbers[0]}, {numbers[1]})");

			Node header = root;

			while (header.Label != $"SquareContainsValue ({numbers[0]}, {numbers[1]})")
            {
				header = header.East;
				if (header == root)
					throw new Exception($"No column found satisfying constraint {move}");
            }

			Node searchNode = header.South;

			while (searchNode.Label != $"Value: {numbers[2]} at ({numbers[0]}, {numbers[1]})")
            {
				searchNode = searchNode.South;
				if (searchNode == header)
					throw new Exception($"No node found corresponding to move");
            }

			Console.WriteLine("Found node corresponding to move");

			return searchNode;
		}
        #endregion

        #region Testing Functions 
        private void runTests(Node root)
		{
			// need to come up with a comprehensive set of tests 
			// test that find column with least children works 
			Node header = findColumnWithLeastChildren(root, true);
			if (header.Children != 9)
				throw new Exception($"All headers at the start should have 9 children, header {header.Label} had {header.Children}");

			coverColumn(header);

			header = root.East;
			while (header != root)
			{
				if (header.Children == 8)
					Console.WriteLine($"Header with label {header.Label} has 8 children");
				header = header.East;
			}

			header = findColumnWithLeastChildren(root, true);

			if (header.Children != 8)
				throw new Exception($"With one column covered, the minimum number of children should be 8, for header {header.Label} it was {header.Children}");
			// test that covering a column works 
			// test that backtracking works 

			header = root.East;

			coverAllColumnsInRow(header.South);
		}
		#endregion

	}

	#region Solution Set
	public class SolutionSet : IEnumerable<Node>
	{
		LinkedList<Node> rowStack = new LinkedList<Node>();

		public void Push(Node candidate)
		{
			rowStack.AddLast(candidate);
		}

		public Node Pop()
        {
			if (rowStack.Count == 0)
			{
				throw new Exception("Attempted to pop from empty stack");
			}
			Node returnNode = rowStack.Last.Value;
			rowStack.RemoveLast();
			return returnNode;
        }

		public void PrintSolutionSet()
		{
			for (LinkedListNode<Node> node = rowStack.First; node != null; node = node.Next)
			{
				Console.WriteLine(node.Value.Label);
			}
		}

		public IEnumerator<Node> GetEnumerator()
		{
			return rowStack.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return rowStack.GetEnumerator();
		}
	}
    #endregion
}
