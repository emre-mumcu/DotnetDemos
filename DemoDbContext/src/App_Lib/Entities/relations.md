1-1

namespace src.App_Lib.Entities;

// Principal (parent)
public class Doc
{
	public int Id { get; set; }
	public DocHeader? Header { get; set; } // Reference navigation to dependent
}

// Dependent (child)
public class DocHeader
{
	public int Id { get; set; }
	public int DocId { get; set; } // Required foreign key property
	public Doc Doc { get; set; } = null!; // Required reference navigation to principal
}

1-N

namespace src.App_Lib.Entities;

// Principal (parent)
public class Blog
{
	public int Id { get; set; }
	public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
}

// Dependent (child)
public class Post
{
	public int Id { get; set; }
	public int BlogId { get; set; } // Required foreign key property
	public Blog Blog { get; set; } = null!; // Required reference navigation to principal
}



N-N

namespace src.App_Lib.Entities;

// Basic many-to-many

public class Entry
{
	public int Id { get; set; }
	public List<Tag> Tags { get; } = [];
}

public class Tag
{
	public int Id { get; set; }
	public List<Entry> Entries { get; } = [];
}

