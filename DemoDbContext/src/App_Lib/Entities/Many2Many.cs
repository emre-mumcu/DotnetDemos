namespace src.App_Lib.Entities;

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



*/






