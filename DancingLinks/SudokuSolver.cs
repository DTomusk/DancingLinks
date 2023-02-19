using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
	public class SudokuSolver
	{
		public void SolveSudoku(Node root)
		{
			// Run the algorithm here 
			// Choose a column with > 0 children
			// Choose a row in that column 
			// Add that row to the solution set 
			// Cover the column and all the columns associated with elements in the row 

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

			// Choose row in search Node to be added to the solution set 
		}

		// Remove the header from the header row 
		// For all the children, take all their siblings and connect their parents to their children 
		private void CoverColumn(Node header)
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
	}
}
