using System;

namespace DancingLinks
{
	public class Program
	{
		static void Main(string[] args)
		{
			SudokuGenerator generator = new SudokuGenerator();
			Node root = generator.GenerateSudokuConstraints();
			SudokuSolver solver = new SudokuSolver();
			solver.SolveSudoku(root);

			Console.ReadLine();
		}
	}
}
