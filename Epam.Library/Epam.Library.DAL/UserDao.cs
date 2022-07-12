using System.Data;
using System.Data.SqlClient;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.DAL;

public class UserDao : IUserDao
{
    private readonly string? _connectionString;

    public UserDao(SqlConfig config)
    {
        _connectionString = config.ConnectionString;
    }

    public bool Auth(User user)
    {
        if (GetUserByUsername(user.Username) is null)
        {
            return false;
        }
        return GetUserByUsername(user.Username).Password == user.Password;
    }

    public List<User> GetAllUsers()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetAllUsers", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var reader = cmd.ExecuteReader();
                return ReadAllUsers(reader);
            }
        }
    }

    public User GetUserByUsername(string username)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetUserByUsername", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return GetUser(reader);
                }

                return null;
            }
        }
    }

    public bool Register(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("AddUser", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddUserParameters(user, cmd);

                cmd.ExecuteScalar();

                var result = cmd.Parameters["@Id"].Value;
                if (result == DBNull.Value) return false;
                user.Id = Convert.ToInt32(result);
                return true;
            }
        }
    }

    public void RemoveUser(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("DeleteUserFromLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdToDelete", id);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public bool UpdateUser(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("UpdateUserInLibrary", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddUserParameters(user, cmd);
                cmd.Parameters["@Id"].Direction = ParameterDirection.Input;

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
    public User GetUserById(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("GetUserById", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return GetUser(reader);
                }

                return null;
            }
        }
    }

    public void ClearUsers()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand("ClearUsers", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
    }

    private void AddUserParameters(User user, SqlCommand cmd)
    {
        cmd.Parameters.AddWithValue("@Id", user.Id).Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("@Username", user.Username);
        cmd.Parameters.AddWithValue("@Password", user.Password);
        cmd.Parameters.AddWithValue("@Role", user.Role);
    }

    private List<User> ReadAllUsers(SqlDataReader reader)
    {
        var users = new List<User>();

        while (reader.Read())
        {
            users.Add(GetUser(reader));
        }

        return users;
    }

    private User GetUser(SqlDataReader reader)
    {
        return new User(
                reader["Username"].ToString(),
                reader["Password"].ToString(),
                reader["Role"].ToString())
            {Id = (int) reader["Id"]};
    }
}