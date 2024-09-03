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

/*


https://learn.microsoft.com/en-us/ef/core/modeling/relationships/foreign-and-principal-keys
https://learn.microsoft.com/en-us/ef/core/modeling/relationships/navigations
https://learn.microsoft.com/en-us/ef/core/modeling/relationships/conventions
https://learn.microsoft.com/en-us/ef/core/modeling/relationships/mapping-attributes

One-to-one relationships are used when one entity is associated with at most one other entity. For example, a Blog has one BlogHeader, and that BlogHeader belongs to a single Blog.

Required one-to-one

// Principal (parent)
public class Blog
{
    public int Id { get; set; }
    public BlogHeader? Header { get; set; } // Reference navigation to dependent
}

// Dependent (child)
public class BlogHeader
{
    public int Id { get; set; }
    public int BlogId { get; set; } // Required foreign key property
    public Blog Blog { get; set; } = null!; // Required reference navigation to principal
}


A one-to-one relationship is made up from:

* One or more primary or alternate key properties on the principal entity. For example, Blog.Id.
* One or more foreign key properties on the dependent entity. For example, BlogHeader.BlogId.
* Optionally, a reference navigation on the principal entity referencing the dependent entity. For example, Blog.* Header.
* Optionally, a reference navigation on the dependent entity referencing the principal entity. For example, * BlogHeader.Blog.



It is not always obvious which side of a one-to-one relationship should be the principal, and which side should be the dependent. Some considerations are:

* If the database tables for the two types already exist, then the table with the foreign key column(s) must map to the dependent type.
* A type is usually the dependent type if it cannot logically exist without the other type. For example, it makes no * sense to have a header for a blog that does not exist, so BlogHeader is naturally the dependent type.
* If there is a natural parent/child relationship, then the child is usually the dependent type.

So, for the relationship in this example:

* The foreign key property BlogHeader.BlogId is not nullable. This makes the relationship "required" because every dependent (BlogHeader) must be related to some principal (Blog), since its foreign key property must be set to some value.
* Both entities have navigations pointing to the related entity on the other side of the relationship.


A required relationship ensures that every dependent entity must be associated with some principal entity. However, a principal entity can always exist without any dependent entity. That is, a required relationship does not indicate that there will always be a dependent entity. There is no way in the EF model, and also no standard way in a relational database, to ensure that a principal is associated with a dependent. If this is needed, then it must be implemented in application (business) logic. See Required navigations for more information.

A relationship with two navigations--one from dependent to principal and an inverse from principal to dependent--is known as a bidirectional relationship.

This relationship is discovered by convention. That is:

* Blog is discovered as the principal in the relationship, and BlogHeader is discovered as the dependent.
* BlogHeader.BlogId is discovered as a foreign key of the dependent referencing the Blog.Id primary key of the * principal. The relationship is discovered as required because BlogHeader.BlogId is not nullable.
* Blog.BlogHeader is discovered as a reference navigation.
* BlogHeader.Blog is discovered as a reference navigation.


For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly. For example:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .HasOne(e => e.Header)
        .WithOne(e => e.Blog)
        .HasForeignKey<BlogHeader>(e => e.BlogId)
        .IsRequired();
}

In the example above, configuration of the relationships starts the principal entity type (Blog). As with all relationships, it is exactly equivalent to start with dependent entity type (BlogHeader) instead. For example:


protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<BlogHeader>()
        .HasOne(e => e.Blog)
        .WithOne(e => e.Header)
        .HasForeignKey<BlogHeader>(e => e.BlogId)
        .IsRequired();
}

Neither of these options is better than the other; they both result in exactly the same configuration.

It is never necessary to configure a relationship twice, once starting from the principal, and then again starting from the dependent. Also, attempting to configure the principal and dependent halves of a relationship separately generally does not work. Choose to configure each relationship from either one end or the other and then write the configuration code only once.




*/