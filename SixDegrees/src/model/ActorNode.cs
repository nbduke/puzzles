using Tools;

namespace SixDegrees.Model
{
	public class ActorNode
	{
		public readonly string Name;
		public readonly int Id;
		public string MovieSharedWithParent { get; set; }

		public ActorNode(string name, int id)
			: this(name, id, string.Empty)
		{
		}

		public ActorNode(
			string name,
			int id,
			string movieSharedWithParent)
		{
			Validate.IsNotNullOrEmpty(name);
			Validate.IsNotNull(movieSharedWithParent, "movieSharedWithParent");

			Name = name;
			Id = id;
			MovieSharedWithParent = movieSharedWithParent;
		}

		public override bool Equals(object obj)
		{
			return (obj is ActorNode other) && other.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
