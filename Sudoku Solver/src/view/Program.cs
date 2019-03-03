using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Tools.DataStructures;
using SudokuSolver.Model;

namespace SudokuSolver.View
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				DisplayHelp();
				return;
			}

			string inputFileName = args[0];
			if (TryParseInputs(inputFileName, out Grid<int> inputs))
			{
				DisplayInputs(inputs);

				var solution = Model.SudokuSolver.Solve(inputs);
				if (solution == null)
					Console.WriteLine("Could not find a solution.");
				else
					DisplaySolution(solution);
			}
		}

		private static void DisplayHelp()
		{
			Console.WriteLine("SudokuSolver <input_file>");
		}

		private static bool TryParseInputs(string inputFileName, out Grid<int> inputs)
		{
			inputs = new Grid<int>(SudokuGrid.GRID_SIZE, SudokuGrid.GRID_SIZE);
			using (var file = new StreamReader(inputFileName))
			{
				GridCell currentLocation = new GridCell(0, 0);
				while (!file.EndOfStream && currentLocation.Row < SudokuGrid.GRID_SIZE)
				{
					string line = file.ReadLine();
					if (line.Length != SudokuGrid.GRID_SIZE)
					{
						DisplayFileMessage(
							inputFileName,
							$"Each row must contain {SudokuGrid.GRID_SIZE} integers or spaces"
						);
						return false;
					}

					foreach (char c in line)
					{
						if (c == ' ')
						{
							inputs[currentLocation] = 0;
						}
						else if (char.IsDigit(c))
						{
							int value = c - '0';
							inputs[currentLocation] = value;
						}
						else
						{
							DisplayFileMessage(inputFileName, $"Invalid character {c}");
							return false;
						}
						currentLocation.Column++;
					}

					currentLocation.Row++;
					currentLocation.Column = 0;
				}

				if (currentLocation.Row < SudokuGrid.GRID_SIZE)
				{
					DisplayFileMessage(inputFileName, $"There must be {SudokuGrid.GRID_SIZE} rows");
					return false;
				}
			}

			return true;
		}

		private static void DisplayFileMessage(string inputFileName, string message)
		{
			Console.WriteLine($"Error in {inputFileName}: {message}.");
		}

		private static void DisplayInputs(Grid<int> inputs)
		{
			Console.WriteLine("Running SudokuSolver on:");
			DisplayGrid(inputs);
		}

		private static void DisplaySolution(Grid<int> solution)
		{
			Console.WriteLine("Got it! Solution:");
			DisplayGrid(solution);
		}

		private static void DisplayGrid(Grid<int> grid)
		{
			for (int row = 0; row < grid.Rows; ++row)
			{
				foreach (int value in grid.RowAt(row))
				{
					if (value == 0)
						Console.Write("  ");
					else
						Console.Write($"{value} ");
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
	}
}
