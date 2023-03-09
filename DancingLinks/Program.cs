using System;

namespace DancingLinks
{
	public class Program
	{
		const bool TESTING = true;
		const bool VERBOSE = true;

		static void Main(string[] args)
		{
			// Generate the matrix representing the constraints and possible moves in the sudoku
			SudokuGenerator generator = new SudokuGenerator();
			Node root = generator.GenerateSudokuConstraints(TESTING, VERBOSE);

			// Run optional tests for the DLX algorithm and associated functions
			if (TESTING)
            {
				TestSolver testSolver = new TestSolver();
				testSolver.RunTests();
			}

			// Solve the sudoku as it was entered 
			SudokuSolver solver = new SudokuSolver();
			solver.SolveSudoku(root);

			Console.ReadLine();
		}
	}
}
