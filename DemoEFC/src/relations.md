
/*

https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

Many-to-many relationships are used when any number entities of one entity type is associated with any number of entities of the same or another entity type. For example, a Post can have many associated Tags, and each Tag can in turn be associated with any number of Posts.

Many-to-many relationships are different from one-to-many and one-to-one relationships in that they cannot be represented in a simple way using just a foreign key. Instead, an additional entity type is needed to "join" the two sides of the relationship. This is known as the "join entity type" and maps to a "join table" in a relational database. The entities of this join entity type contain pairs of foreign key values, where one of each pair points to an entity on one side of the relationship, and the other points to an entity on the other side of the relationship. Each join entity, and therefore each row in the join table, therefore represents one association between the entity types in the relationship.

EF Core can hide the join entity type and manage it behind the scenes. This allows the navigations of a many-to-many relationship to be used in a natural manner, adding or removing entities from each side as needed. However, it is useful to understand what is happening behind the scenes so that their overall behavior, and in particular the mapping to a relational database, makes sense.

Let's start with a relational database schema setup to represent a many-to-many relationship between posts and tags:

CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "Tags" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tags" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "PostTag" (
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostsId", "TagsId"),
    CONSTRAINT "FK_PostTag_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);

In this schema, PostTag is the join table. It contains two columns: PostsId, which is a foreign key to the primary key of the Posts table, and TagsId, which is a foreign key to primary key of the Tags table. Each row in this table therefore represents an association between one Post and one Tag.

A simplistic mapping for this schema in EF Core consists of three entity types--one for each table. If each of these entity types are represented by a .NET class, then those classes might look the following:

public class Post
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = [];
}

public class Tag
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = [];
}

public class PostTag
{
    public int PostsId { get; set; }
    public int TagsId { get; set; }
    public Post Post { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

Notice that in this mapping there is no many-to-many relationship, but rather two one-to-many relationships, one for each of the foreign keys defined in the join table. This is not an unreasonable way to map these tables, but doesn't reflect the intent of the join table, which is to represent a single many-to-many relationship, rather than two one-to-many relationships.

EF allows for a more natural mapping through the introduction of two collection navigations, one on Post containing its related Tags, and an inverse on Tag containing its related Posts. For example:

public class Post
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = [];
    public List<Tag> Tags { get; } = [];
}

public class Tag
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = [];
    public List<Post> Posts { get; } = [];
}

public class PostTag
{
    public int PostsId { get; set; }
    public int TagsId { get; set; }
    public Post Post { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

These new navigations are known as "skip navigations", because they skip over the join entity to provide direct access to the other side of the many-to-many relationship.

As is shown in the examples below, a many-to-many relationship can be mapped in this way--that is, with a .NET class for the join entity, and with both navigations for the two one-to-many relationships and skip navigations exposed on the entity types. However, EF can manage the join entity transparently, without a .NET class defined for it, and without navigations for the two one-to-many relationships. For example:

public class Post
{
    public int Id { get; set; }
    public List<Tag> Tags { get; } = [];
}

public class Tag
{
    public int Id { get; set; }
    public List<Post> Posts { get; } = [];
}

Indeed, EF model building conventions will, by default, map the Post and Tag types shown here to the three tables in the database schema at the top of this section. This mapping, without explicit use of the join type, is what is typically meant by the term "many-to-many".

This relationship is mapped by convention. Even though it is not needed, an equivalent explicit configuration for this relationship is shown below as a learning tool:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Posts);
}

Even with this explicit configuration, many aspects of the relationship are still configured by convention. A more complete explicit configuration, again for learning purposes, is:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Posts)
        .UsingEntity(
            "PostTag",
            l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
            r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsId").HasPrincipalKey(nameof(Post.Id)),
            j => j.HasKey("PostsId", "TagsId"));
}

Please don't attempt to fully configure everything even when it is not needed. As can be seen above, the code gets complicated quickly and its easy to make a mistake. And even in the example above there are many things in the model that are still configured by convention. It's not realistic to think that everything in an EF model can always be fully configured explicitly.

Regardless of whether the relationship is built by convention or using either of the shown explicit configurations, the resulting mapped schema (using SQLite) is:

CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "Tags" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tags" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "PostTag" (
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostsId", "TagsId"),
    CONSTRAINT "FK_PostTag_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);


Many-to-many with named join table

In the previous example, the join table was named PostTag by convention. It can be given an explicit name with UsingEntity. For example:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Posts)
        .UsingEntity("PostsToTagsJoinTable");
}


Many-to-many with join table foreign key names

Following on from the previous example, the names of the foreign key columns in the join table can also be changed. There are two ways to do this. The first is to explicitly specify the foreign key property names on the join entity. For example:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Posts)
        .UsingEntity(
            l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagForeignKey"),
            r => r.HasOne(typeof(Post)).WithMany().HasForeignKey("PostForeignKey"));
}

The second way is to leave the properties with their by-convention names, but then map these properties to different column names. For example:

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Posts)
        .UsingEntity(
            j =>
            {
                j.Property("PostsId").HasColumnName("PostForeignKey");
                j.Property("TagsId").HasColumnName("TagForeignKey");
            });
}

In either case, the mapping remains the same, with only the foreign key column names changed:

CREATE TABLE "PostTag" (
    "PostForeignKey" INTEGER NOT NULL,
    "TagForeignKey" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostForeignKey", "TagForeignKey"),
    CONSTRAINT "FK_PostTag_Posts_PostForeignKey" FOREIGN KEY ("PostForeignKey") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagForeignKey" FOREIGN KEY ("TagForeignKey") REFERENCES "Tags" ("Id") ON DELETE CASCADE);

*/


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