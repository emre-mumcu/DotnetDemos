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

/*

https://learn.microsoft.com/en-us/ef/core/modeling/relationships/foreign-and-principal-keys
https://learn.microsoft.com/en-us/ef/core/modeling/relationships/navigations
https://learn.microsoft.com/en-us/ef/core/modeling/relationships/conventions
https://learn.microsoft.com/en-us/ef/core/modeling/relationships/mapping-attributes


https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many

One-to-many relationships are used when a single entity is associated with any number of other entities. For example, a Blog can have many associated Posts, but each Post is associated with only one Blog.

Required one-to-many

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

A one-to-many relationship is made up from:

* One or more primary or alternate key properties on the principal entity; that is the "one" end of the relationship. For example, Blog.Id.
* One or more foreign key properties on the dependent entity; that is the "many" end of the relationship. For example, Post.BlogId.
* Optionally, a collection navigation on the principal entity referencing the dependent entities. For example, Blog.Posts.
* Optionally, a reference navigation on the dependent entity referencing the principal entity. For example, Post.Blog.
So, for the relationship in this example:

* The foreign key property Post.BlogId is not nullable. This makes the relationship "required" because every dependent (Post) must be related to some principal (Blog), since its foreign key property must be set to some value.
* Both entities have navigations pointing to the related entity or entities on the other side of the relationship.

A required relationship ensures that every dependent entity must be associated with some principal entity. However, a principal entity can always exist without any dependent entities. That is, a required relationship does not indicate that there will always be at least one dependent entity. There is no way in the EF model, and also no standard way in a relational database, to ensure that a principal is associated with a certain number of dependents. If this is needed, then it must be implemented in application (business) logic.

A relationship with two navigations, one from dependent to principal, and an inverse from principal to dependents, is known as a bidirectional relationship.

This relationship is discovered by convention. That is:

* Blog is discovered as the principal in the relationship, and Post is discovered as the dependent.
* Post.BlogId is discovered as a foreign key of the dependent referencing the Blog.Id primary key of the principal. * The relationship is discovered as required because Post.BlogId is not nullable.
* Blog.Posts is discovered as the collection navigation.
* Post.Blog is discovered as the reference navigation.

For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .HasMany(e => e.Posts)
        .WithOne(e => e.Blog)
        .HasForeignKey(e => e.BlogId)
        .IsRequired();
}

In the example above, configuration of the relationships starts with HasMany on the principal entity type (Blog) and then follows this with WithOne. As with all relationships, it is exactly equivalent to start with dependent entity type (Post) and use HasOne followed by WithMany. For example:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasOne(e => e.Blog)
        .WithMany(e => e.Posts)
        .HasForeignKey(e => e.BlogId)
        .IsRequired();
}

Neither of these options is better than the other; they both result in exactly the same configuration.








*/