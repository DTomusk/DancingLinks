using System;
using System.Collections.Generic;

namespace DancingLinks
{
	public class SudokuSolver
	{
		public void SolveSudoku(Node root, bool testing)
		{
			// If the program is not in test mode then run the normal algorithm
			if (!testing)
			{
				while (root.East != root)
				{
					// Choose the column with the minimum number of children 
					Node minHeader = findColumnWithLeastChildren(root);
					if (minHeader.Children == 0)
					{
						// backtrack
					}
					else
					{
						// choose a row to add to the solution set 
						// for each node in the solution set, cover the column 
						// first: add row to the solution set 
						// second: connect the parents of each node to their children and update headers accordingly 
						// third: for each header: connect their siblings then go to all of their childrens' siblings and rewire their parents and children, updating headers as necessary 
						// cover columns and continue
					}
				}
			}
			// Otherwise test a certain set of operations
			else
			{
				runTests(root);
			}
		}

		// Return the column header with the least number of children 
		private Node findColumnWithLeastChildren(Node root)
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

			return searchNode;
		}

		// Remove the header from the header row 
		// Connect all the parents of the siblings 
		private void coverColumn(Node header)
		{
			header.West.East = header.East;
			header.East.West = header.West;

			Node childNode = header.South.East;

			// need to update header children counts as well 
			while (childNode != header.South)
			{
				childNode.North.South = childNode.South;
				childNode.South.North = childNode.North;
				childNode = childNode.East;
			}
		}

		#region Testing Functions 
		private void runTests(Node root)
		{ 
			// need to come up with a comprehensive set of tests 
			// test that find column with least children works 
			// test that covering a column works 
			// test that backtracking works 
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
