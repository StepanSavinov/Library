using Microsoft.Extensions.Configuration;

namespace Epam.Library.DAL;

public class SqlConfig
{
    public SqlConfig(IConfiguration config)
    {
        ConnectionString = config.GetConnectionString("DefaultConnection");
    }
    public string? ConnectionString { get; }
}