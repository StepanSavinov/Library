using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.DAL;

public class LibraryDao : ILibraryDao
{
    private readonly string? _connectionString;

    public LibraryDao(SqlConfig config)
    {
        _connectionString = config.ConnectionString;
    }

    public bool AddToLibrary(Polygraphy polygraphy)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            switch (polygraphy)
            {
                case Book book:
                {
                    using (var cmd = new SqlCommand("AddBookToLibrary", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        AddPolygraphyParameters(cmd, book);
                        AddBookParameters(cmd, book);

                        cmd.ExecuteScalar();
                        var result = cmd.Parameters["@Id"].Value;
                        if (result != DBNull.Value)
                        {
                            book.Id = Convert.ToInt32(result);
                            return true;
                        }
                    }

                    break;
                }
                case NewspaperIssue newspaperIssue:
                {
                    using (var cmd = new SqlCommand("AddNewspaperIssueToLibrary", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        AddPolygraphyParameters(cmd, newspaperIssue);
                        AddNewspaperIssueParameters(cmd, newspaperIssue);

                        cmd.ExecuteScalar();
                        var result = cmd.Parameters["@Id"].Value;
                        if (result != DBNull.Value)
                        {
                            newspaperIssue.Id = Convert.ToInt32(result);
                            return true;
                        }
                    }

                    break;
                }
                case Patent patent:
                {
                    using (var cmd = new SqlCommand("AddPatentToLibrary", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        AddPolygraphyParameters(cmd, patent);
                        AddPatentParameters(cmd, patent);

                        cmd.ExecuteScalar();
                        var result = cmd.Parameters["@Id"].Value;
                        if (result != DBNull.Value)
                        {
                            patent.Id = Convert.ToInt32(result);
                            return true;
                        }
                    }
                    break;
                }
            }
            return false;
        }
    }

    public void ClearLibrary()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("ClearLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Polygraphy> GetAllLibrary()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetAllLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var reader = cmd.ExecuteReader();

                return ReadAllLibrary(reader);
            }
        }
    }

    public List<NewspaperIssue> GetAllNewspaperIssuesByNewspaperId(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetAllNewspaperIssuesWithId", connection))
            {
                cmd.Parameters.AddWithValue("@NewspaperId", id);
                cmd.CommandType = CommandType.StoredProcedure;
                
                var issues = new List<NewspaperIssue>();  
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    issues.Add(GetNewspaperIssue(reader));
                }

                return issues;
            }
        }
    }

    public Polygraphy GetPolygraphyById(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetPolygraphyById", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    switch ((int) reader["ObjectType"])
                    {
                        case 1:
                            return GetBook(reader);

                        case 2:
                            return GetNewspaperIssue(reader);

                        case 3:
                            return GetPatent(reader);

                        default:
                            return null;
                    }
                }

                return null;
            }
        }
    }

    public List<Polygraphy> GetSortedPolygraphies(bool reverse)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetSortedPolygraphies", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Reversed", reverse);
                var reader = cmd.ExecuteReader();

                return ReadAllLibrary(reader);
            }
        }
    }

    public void RemoveFromLibrary(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("DeleteFromLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void MarkPolygraphyAsDeleted(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("MarkPolygraphyAsDeleted", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Polygraphy> SearchByName(string name)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SearchByName", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);

                var reader = cmd.ExecuteReader();
                return ReadAllLibrary(reader);
            }
        }
    }

    private List<Polygraphy> ReadAllLibrary(SqlDataReader reader)
    {
        var polys = new List<Polygraphy>();
        while (reader.Read())
        {
            switch ((int) reader["ObjectType"])
            {
                case 1:
                    polys.Add(GetBook(reader));
                    break;

                case 2:
                    polys.Add(GetNewspaperIssue(reader));
                    break;

                case 3:
                    polys.Add(GetPatent(reader));
                    break;
            }
        }

        return polys;
    }

    public List<Polygraphy> SearchByAuthor(string firstname, string lastname, PolygraphyEnum.PolyType type)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SearchByAuthor", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Firstname", firstname);
                cmd.Parameters.AddWithValue("@Lastname", lastname);
                cmd.Parameters.AddWithValue("@TypeOfSearch", (int) type);

                var reader = cmd.ExecuteReader();
                var polys = new List<Polygraphy>();
                while (reader.Read())
                {
                    switch (type)
                    {
                        case PolygraphyEnum.PolyType.Book:
                            polys.Add(GetBook(reader));
                            break;

                        case PolygraphyEnum.PolyType.Patent:
                            polys.Add(GetPatent(reader));
                            break;

                        case PolygraphyEnum.PolyType.BookAndPatent:
                            if ((int) reader["ObjectType"] == 1)
                            {
                                polys.Add(GetBook(reader));
                            }
                            else if ((int) reader["ObjectType"] == 3)
                            {
                                polys.Add(GetPatent(reader));
                            }

                            break;
                    }
                }

                return polys;
            }
        }
    }

    public bool UpdatePolygraphyInLibrary(Polygraphy poly)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            if (poly is Book book)
            {
                using (var cmd = new SqlCommand("UpdateBookInLibrary", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    AddPolygraphyParameters(cmd, book);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Input;
                    AddBookParameters(cmd, book);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            else if (poly is NewspaperIssue newspaper)
            {
                using (var cmd = new SqlCommand("UpdateNewspaperInLibrary", connection))
                {
                    AddPolygraphyParameters(cmd, newspaper);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Input;
                    AddNewspaperIssueParameters(cmd, newspaper);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            else if (poly is Patent patent)
            {
                using (var cmd = new SqlCommand("UpdatePatentInLibrary", connection))
                {
                    AddPolygraphyParameters(cmd, patent);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Input;
                    AddPatentParameters(cmd, patent);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }

            return false;
        }
    }

    public List<Book> GetAllBooks()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetAllBooks", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var books = new List<Book>();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(GetBook(reader));
                }

                return books;
            }
        }
    }

    //newspapers
    public bool AddNewspaperToLibrary(Newspaper newspaper)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new SqlCommand("AddNewspaperToLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddNewspaperParameters(cmd, newspaper);

                cmd.ExecuteScalar();
                var result = cmd.Parameters["@Id"].Value;
                if (result == DBNull.Value) return false;
                newspaper.Id = Convert.ToInt32(result);
                return true;
            }
        }
    }

    public bool UpdateNewspaperInLibrary(Newspaper newspaper)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new SqlCommand("UpdateNewspaperInLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddNewspaperParameters(cmd, newspaper);
                cmd.Parameters["@Id"].Direction = ParameterDirection.Input;

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public Newspaper GetNewspaperById(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new SqlCommand("GetNewspaperById", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return GetNewspaper(reader);
                }

                return null;
            }
        }
    }

    public List<Newspaper> GetAllNewspapers()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new SqlCommand("GetAllNewspapers", connection))
            {
                var newspapers = new List<Newspaper>();
                cmd.CommandType = CommandType.StoredProcedure;

                var reader = cmd.ExecuteReader();

                return ReadAllNewspapers(reader);
            }
        }
    }

    private List<Newspaper> ReadAllNewspapers(SqlDataReader reader)
    {
        var newspapers = new List<Newspaper>();
        while (reader.Read())
        {
            newspapers.Add(GetNewspaper(reader));
        }

        return newspapers;
    }

    public List<Newspaper> SearchNewspaperByName(string name)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new SqlCommand("SearchNewspaperByName", connection))
            {
                var newspapers = new List<Newspaper>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    newspapers.Add(GetNewspaper(reader));
                }

                return newspapers;
            }
        }
    }

    public void RemoveNewspaperFromLibrary(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("DeleteNewspaperFromLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void ClearNewspapers()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("ClearNewspapers", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
    }

    private Book GetBook(SqlDataReader reader)
    {
        return new Book(
                name: reader["Name"].ToString(),
                authors: JsonSerializer.Deserialize<List<int>>(reader["AuthorIds"].ToString()),
                city: reader["City"].ToString(),
                publisher: reader["Publisher"].ToString(),
                created: (DateTime) reader["CreationDate"],
                totalPages: (int) reader["TotalPages"],
                footnote: reader["Footnote"].ToString(),
                isbn: reader["ISBN"].ToString())
            {Id = (int) reader["Id"]};
    }

    private NewspaperIssue GetNewspaperIssue(SqlDataReader reader)
    {
        return new NewspaperIssue(
            name: reader["Name"].ToString(),
            number: (int) reader["NewspaperNumber"],
            city: reader["NewspaperCity"].ToString(),
            publisher: reader["NewspaperPublisher"].ToString(),
            created: (DateTime) reader["CreationDate"],
            totalPages: (int) reader["TotalPages"],
            footnote: reader["Footnote"].ToString())
        {
            NewspaperId = (int) reader["NewspaperId"],
            Id = (int) reader["Id"]
        };
    }

    private Newspaper GetNewspaper(SqlDataReader reader)
    {
        if (reader["IssueIds"] == DBNull.Value)
        {
            return new Newspaper(
                    name: reader["Name"].ToString(),
                    issues: new List<int>(),
                    issn: reader["ISSN"].ToString())
                {Id = (int) reader["Id"]};
        }

        return new Newspaper(
                name: reader["Name"].ToString(),
                issues: JsonSerializer.Deserialize<List<int>>(reader["IssueIds"].ToString()),
                issn: reader["ISSN"].ToString())
            {Id = (int) reader["Id"]};
    }

    private Patent GetPatent(SqlDataReader reader)
    {
        return new Patent(
                name: reader["Name"].ToString(),
                authors: JsonSerializer.Deserialize<List<int>>(reader["AuthorIds"].ToString()),
                country: reader["Country"].ToString(),
                number: (int) reader["PatentNumber"],
                created: (DateTime) reader["CreationDate"],
                published: (DateTime) reader["PatentPublishmentDate"],
                totalPages: (int) reader["TotalPages"],
                footnote: reader["Footnote"].ToString())
            {Id = (int) reader["Id"]};
    }

    private void AddPolygraphyParameters(SqlCommand cmd, Polygraphy poly)
    {
        cmd.Parameters.AddWithValue("@Id", poly.Id).Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("@Name", poly.Name);
        cmd.Parameters.AddWithValue("@CreationDate", poly.Created);
        cmd.Parameters.AddWithValue("@Footnote", poly.Footnote);
        cmd.Parameters.AddWithValue("@TotalPages", poly.TotalPages);
    }

    private void AddBookParameters(SqlCommand cmd, Book book)
    {
        cmd.Parameters.AddWithValue("@AuthorIds", JsonSerializer.Serialize(book.Authors));
        cmd.Parameters.AddWithValue("@City", book.City);
        cmd.Parameters.AddWithValue("@Publisher", book.Publisher);
        cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
    }

    private void AddNewspaperIssueParameters(SqlCommand cmd, NewspaperIssue newspaperIssue) // incorrect id??
    {
        cmd.Parameters.AddWithValue("@NewspaperId", newspaperIssue.NewspaperId);
        cmd.Parameters.AddWithValue("@City", newspaperIssue.City);
        cmd.Parameters.AddWithValue("@Publisher", newspaperIssue.Publisher);
        cmd.Parameters.AddWithValue("@Number", newspaperIssue.Number);
    }

    private void AddNewspaperParameters(SqlCommand cmd, Newspaper newspaper)
    {
        cmd.Parameters.AddWithValue("@Id", newspaper.Id).Direction = ParameterDirection.Output;
        //cmd.Parameters.AddWithValue("@IssueIds", JsonSerializer.Serialize<List<int>>(newspaper.Issues));
        cmd.Parameters.AddWithValue("@Name", newspaper.Name);
        cmd.Parameters.AddWithValue("@ISSN", newspaper.ISSN);
    }

    private void AddPatentParameters(SqlCommand cmd, Patent patent)
    {
        cmd.Parameters.AddWithValue("@AuthorIds", JsonSerializer.Serialize(patent.Authors));
        cmd.Parameters.AddWithValue("@Country", patent.Country);
        cmd.Parameters.AddWithValue("@PublishmentDate", patent.Published);
        cmd.Parameters.AddWithValue("@Number", patent.Number);
    }
}