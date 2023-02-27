using System;

namespace DancingLinks
{
	public class Program
	{
		const bool TESTING = false;
		const bool VERBOSE = true;

		static void Main(string[] args)
		{
			SudokuGenerator generator = new SudokuGenerator();
			Node root = generator.GenerateSudokuConstraints(TESTING, VERBOSE);
			SudokuSolver solver = new SudokuSolver();
			solver.SolveSudoku(root);

			Console.ReadLine();
		}
	}
}
