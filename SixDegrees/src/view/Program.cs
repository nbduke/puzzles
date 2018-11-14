using System;
using System.Collections.Generic;
using System.Linq;

using SixDegrees.Model;

namespace SixDegrees.View
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("SIX DEGREES");
			Console.WriteLine("Enter a pair of actors and find the smallest number of links between them.");
			Console.WriteLine("Press q at any time to exit.");
			Console.WriteLine("---------------------------------------------------------------------------------");

			while (true)
			{
				Console.WriteLine();

				Console.WriteLine("Enter an actor's first and last name: ");
				string actorA = Console.ReadLine();

				if (actorA.ToLower() == "q")
					break;

				Console.WriteLine("Enter another actor's name: ");
				string actorB = Console.ReadLine();

				if (actorB.ToLower() == "q")
					break;

				Console.WriteLine("Computing the shortest path from {0} to {1}. This may take a while....", actorA, actorB);

				try
				{
					var finder = new ConnectionsFinder();
					var path = finder.GetConnections(actorA, actorB);

					if (path.Count() == 0)
					{
						Console.WriteLine("There don't appear to be any connections between those actors.");
					}
					else
					{
						Console.WriteLine("Got it!");
						PrintPath(path.ToList());
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("The following error occurred:\n{0}", e.Message);
				}
			}
		}

		static void PrintPath(List<ActorNode> path)
		{
			for (int i = 0; i < path.Count - 1; ++i)
			{
				Console.WriteLine(
					"{0} is in \"{1}\" with {2}",
					path[i],
					path[i + 1].MovieSharedWithParent,
					path[i + 1]);
			}
		}
	}
}
