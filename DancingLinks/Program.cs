using System;

namespace DancingLinks
{
	public class Program
	{
		const bool TESTING = true; 

		static void Main(string[] args)
		{
			SudokuGenerator generator = new SudokuGenerator();
			Node root = generator.GenerateSudokuConstraints(TESTING);
			SudokuSolver solver = new SudokuSolver();
			solver.SolveSudoku(root, TESTING);

			Console.ReadLine();
		}
	}
}
