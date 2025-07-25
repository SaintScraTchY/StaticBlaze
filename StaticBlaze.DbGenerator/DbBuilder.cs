using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using StaticBlaze.Models;
using StaticBlaze.Utilities.MarkdownParser;

namespace StaticBlaze.DbGenerator;

public class DbBuilder
{
    private readonly string _dbPath;
    private readonly string _contentRoot;
    private readonly bool _seed;

    public DbBuilder(string dbPath, string contentRoot, bool seed)
    {
        _dbPath = dbPath;
        _contentRoot = contentRoot;
        _seed = seed;
    }

    public async Task RunAsync()
    {
        if (File.Exists(_dbPath)) File.Delete(_dbPath);

        var connString = new SqliteConnectionStringBuilder
        {
            DataSource = _dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();

        using var connection = new SqliteConnection(connString);
        await connection.OpenAsync();

        await CreateSchemaAsync(connection);

        if (_seed)
        {
            await SeedAsync(connection);
        }
    }

    private static async Task CreateSchemaAsync(IDbConnection connection)
    {
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Category (
                Id TEXT PRIMARY KEY,
                Title TEXT NOT NULL,
                Slug TEXT NOT NULL UNIQUE,
                LastUpdated TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Tag (
                Id TEXT PRIMARY KEY,
                Title TEXT NOT NULL,
                Slug TEXT NOT NULL UNIQUE,
                LastUpdated TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Author (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Username TEXT NOT NULL,
                Email TEXT NOT NULL,
                ProfilePictureUrl TEXT,
                AboutMe TEXT NOT NULL,
                Website TEXT,
                SocialLinks TEXT,
                Expertise TEXT,
                CreatedAt TEXT NOT NULL,
                LastUpdated TEXT
            );

            CREATE TABLE IF NOT EXISTS MetaPost (
                Guid TEXT PRIMARY KEY,
                Slug TEXT NOT NULL UNIQUE,
                Featured INTEGER NOT NULL,
                Title TEXT NOT NULL,
                ShortDescription TEXT NOT NULL,
                CreatedDateTime TEXT NOT NULL,
                ModifiedDateTime TEXT NOT NULL,
                PublishedAt TEXT NOT NULL,
                AuthorId TEXT NOT NULL,
                CategoryId TEXT NOT NULL,
                ReadTime INTEGER,
                Thumbnail TEXT,
                FOREIGN KEY (AuthorId) REFERENCES Author(Id),
                FOREIGN KEY (CategoryId) REFERENCES Category(Id)
            );

            CREATE TABLE IF NOT EXISTS MetaPostTag (
                MetaPostId TEXT NOT NULL,
                TagId TEXT NOT NULL,
                PRIMARY KEY (MetaPostId, TagId),
                FOREIGN KEY (MetaPostId) REFERENCES MetaPost(Guid),
                FOREIGN KEY (TagId) REFERENCES Tag(Id)
            );
        ");
    }

    private async Task SeedAsync(IDbConnection connection)
    {
        Console.WriteLine("Seeding database from content...");

        // Categories
        var categoryPath = Path.Combine(_contentRoot, "categories.json");
        if (File.Exists(categoryPath))
        {
            var categories = await JsonSerializer.DeserializeAsync<List<Category>>(File.OpenRead(categoryPath));
            if (categories is not null)
            {
                foreach (var c in categories)
                {
                    await connection.ExecuteAsync("INSERT INTO Category (Id, Title, Slug, LastUpdated) VALUES (@Id, @Title, @Slug, @LastUpdated)", c);
                }
            }
        }

        // Tags
        var tagPath = Path.Combine(_contentRoot, "tags.json");
        if (File.Exists(tagPath))
        {
            var tags = await JsonSerializer.DeserializeAsync<List<Tag>>(File.OpenRead(tagPath));
            if (tags is not null)
            {
                foreach (var t in tags)
                {
                    await connection.ExecuteAsync("INSERT INTO Tag (Id, Title, Slug, LastUpdated) VALUES (@Id, @Title, @Slug, @LastUpdated)", t);
                }
            }
        }

        // Authors
        var authorsDir = Path.Combine(_contentRoot, "authors");
        if (Directory.Exists(authorsDir))
        {
            foreach (var file in Directory.EnumerateFiles(authorsDir, "*.json"))
            {
                var author = await JsonSerializer.DeserializeAsync<Author>(File.OpenRead(file));
                if (author is not null)
                {
                    await connection.ExecuteAsync("INSERT INTO Author (Id, Name, Username, Email, ProfilePictureUrl, AboutMe, Website, SocialLinks, Expertise, CreatedAt, LastUpdated) VALUES (@Id, @Name, @Username, @Email, @ProfilePictureUrl, @AboutMe, @Website, @SocialLinksJson, @ExpertiseJson, @CreatedAt, @LastUpdated)",
                        new
                        {
                            author.Id,
                            author.Name,
                            author.Username,
                            author.Email,
                            author.ProfilePictureUrl,
                            author.AboutMe,
                            author.Website,
                            SocialLinksJson = JsonSerializer.Serialize(author.SocialLinks),
                            ExpertiseJson = JsonSerializer.Serialize(author.Expertise),
                            author.CreatedAt,
                            author.LastUpdated
                        });
                }
            }
        }

        // MetaPosts (only from frontmatter)
        var postsDir = Path.Combine(_contentRoot, "posts");
        if (Directory.Exists(postsDir))
        {
            foreach (var file in Directory.EnumerateFiles(postsDir, "*.md", SearchOption.TopDirectoryOnly))
            {
                var markdown = await File.ReadAllTextAsync(file);
                var meta = markdown.ParseMarkdown();

                // Insert MetaPost (basic matching only)
                await connection.ExecuteAsync("INSERT INTO MetaPost (Guid, Slug, Featured, Title, ShortDescription, CreatedDateTime, ModifiedDateTime, PublishedAt, AuthorId, CategoryId, ReadTime, Thumbnail) VALUES (@Guid, @Slug, @Featured, @Title, @ShortDescription, @CreatedDateTime, @ModifiedDateTime, @PublishedAt, @AuthorId, @CategoryId, @ReadTime, @Thumbnail)",
                    new
                    {
                        Guid = meta.Guid,
                        meta.Slug,
                        meta.Featured,
                        meta.Title,
                        meta.ShortDescription,
                        meta.CreatedDateTime,
                        meta.ModifiedDateTime,
                        meta.PublishedAt,
                        AuthorId = meta.Author, // GUID expected
                        CategoryId = meta.Category,
                        meta.ReadTime,
                        meta.Thumbnail
                    });

                // Insert MetaPostTag join
                var tags = meta.Tags?.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)) ?? [];
                foreach (var tag in tags)
                {
                    await connection.ExecuteAsync("INSERT OR IGNORE INTO MetaPostTag (MetaPostId, TagId) VALUES (@MetaPostId, @TagId)", new { MetaPostId = meta.Guid, TagId = tag });
                }
            }
        }
    }
}
