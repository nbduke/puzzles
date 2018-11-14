using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using Tools;
using Tools.Algorithms.Search;

namespace SixDegrees.Model
{
	public class ConnectionsFinder
	{
		// From my profile at themoviedb.org
		private const string ApiKey = "7ffb238a4e41e4e37cc031bb3a410dbf";
		private static TMDbClient Client = new TMDbClient(ApiKey);

		private const int MinimumVotes = 100;
		private const double MinimumRating = 2.0;
		private const int MaximumCastPerMovie = 20;

		public IEnumerable<ActorNode> GetConnections(string actorA, string actorB)
		{
			return GetConnectionsAsync(actorA, actorB).Result;
		}

		public async Task<IEnumerable<ActorNode>> GetConnectionsAsync(string actorA, string actorB)
		{
			var lookupResults = new Task<ActorNode>[]
			{
				LookupActor(actorA),
				LookupActor(actorB)
			};

			var actors =  await Task.WhenAll(lookupResults);

			var bidiSearch = new BidirectionalSearch<ActorNode>(
				GetFirstDegreeConnections,
				GetFirstDegreeConnections,
				RepairPath);

			var path = bidiSearch.FindPath(actors[0], actors[1]);
			return path;
		}

		private static async Task<ActorNode> LookupActor(string name)
		{
			Validate.IsNotNullOrEmpty(name);

			var personSearchResult = await Client.SearchPersonAsync(name);
			Validate.IsTrue(personSearchResult.TotalResults > 0,
				string.Format("The name {0} could not be found.", name));

			var actor = personSearchResult.Results[0];
			return new ActorNode(actor.Name, actor.Id);
		}

		private static IEnumerable<ActorNode> GetFirstDegreeConnections(ActorNode actor)
		{
			foreach (var movie in GetMovieCreditsForActor(actor.Id))
			{
				var movieCast = movie.Credits.Cast;
				int actorsToConsider = Math.Min(movieCast.Count / 2 + 1, MaximumCastPerMovie);

				foreach (var person in movieCast.Take(actorsToConsider))
				{
					if (person.Id != actor.Id)
					{
						ActorNode connection = new ActorNode(person.Name, person.Id, movie.Title);
						yield return connection;
					}
				}
			}
		}

		private static IEnumerable<Movie> GetMovieCreditsForActor(int actorId)
		{
			var person = Client.GetPersonAsync(actorId, PersonMethods.MovieCredits).Result;
			var movieCredits = person.MovieCredits.Cast;

			// Sort the movies in descending order by release date
			movieCredits.Sort(
				(a, b) =>
				{
					if (a.ReleaseDate.HasValue && b.ReleaseDate.HasValue)
						return -a.ReleaseDate.Value.CompareTo(b.ReleaseDate.Value);
					else if (a.ReleaseDate.HasValue)
						return 1;
					else if (b.ReleaseDate.HasValue)
						return -1;
					else
						return 0;
				});

			foreach (var movieCredit in movieCredits)
			{
				var movie = Client.GetMovieAsync(movieCredit.Id, MovieMethods.Credits).Result;
				if (ShouldCountActingCredit(movie))
					yield return movie;
			}
		}

		private static bool ShouldCountActingCredit(Movie movie)
		{
			return movie.VoteCount >= MinimumVotes && movie.VoteAverage >= MinimumRating;
		}

		private static void RepairPath(IEnumerable<ActorNode> path)
		{
			string previousMovieSharedWithParent = path.First().MovieSharedWithParent;

			foreach (var actor in path.Skip(1))
			{
				string temp = actor.MovieSharedWithParent;
				actor.MovieSharedWithParent = previousMovieSharedWithParent;
				previousMovieSharedWithParent = temp;
			}
		}
	}
}
