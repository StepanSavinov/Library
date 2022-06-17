using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.DAL;

public class AuthorDao : IAuthorDao
{
    private readonly string? _connectionString;

    public AuthorDao(SqlConfig config)
    {
        _connectionString = config.ConnectionString;
    }

    public bool AddAuthor(Author author)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("AddAuthorToLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddAuthorParameters(author, cmd);

                cmd.ExecuteScalar();
                var result = cmd.Parameters["@Id"].Value;
                if (result != DBNull.Value)
                {
                    author.Id = Convert.ToInt32(result);
                    return true;
                }
            }
        }

        return false;
    }

    public bool UpdateAuthor(Author author)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("UpdateAuthorInLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddAuthorParameters(author, cmd);
                cmd.Parameters["@Id"].Direction = ParameterDirection.Input;

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public void ClearAuthors()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("ClearAuthors", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Author> GetAllAuthors()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetAllAuthors", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var reader = cmd.ExecuteReader();
                return ReadAllAuthors(reader);
            }
        }
    }

    public Author GetAuthorById(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetAuthorById", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return GetAuthor(reader);
                }

                return null;
            }
        }
    }

    public List<Author> GetAuthorsByIds(List<int> authorIds)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetAuthorsByIds", connection))
            {
                var authors = new List<Author>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AuthorIds", JsonSerializer.Serialize(authorIds));

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    authors.Add(GetAuthor(reader));
                }

                return authors;
            }
        }
    }

    public void RemoveAuthor(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("DeleteAuthorFromLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdToDelete", id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void MarkAuthorAsDeleted(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("MarkAuthorAsDeleted", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("@IdToDelete", id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    private List<Author> ReadAllAuthors(SqlDataReader reader)
    {
        var authorList = new List<Author>();

        while (reader.Read())
        {
            authorList.Add(GetAuthor(reader));
        }

        return authorList;
    }

    private void AddAuthorParameters(Author author, SqlCommand cmd)
    {
        cmd.Parameters.AddWithValue("@Id", author.Id).Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("@Firstname", author.Firstname);
        cmd.Parameters.AddWithValue("@Lastname", author.Lastname);
    }

    private Author GetAuthor(SqlDataReader reader)
    {
        return new Author(
                reader["Firstname"].ToString(),
                reader["Lastname"].ToString())
            {Id = (int) reader["Id"]};
    }
}