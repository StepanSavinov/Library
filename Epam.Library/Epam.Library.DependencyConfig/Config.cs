using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.DAL;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Epam.Library.DependencyConfig;

public static class Config
{
    public static IServiceProvider RegisterServices(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        services.AddSingleton<IConfiguration>(builder);
        services.AddSingleton<IUserDao, UserDao>();
        services.AddSingleton<ILibraryDao, LibraryDao>();
        services.AddSingleton<IAuthorDao, AuthorDao>();
        services.AddSingleton<ILoggingDao, LoggingDao>();

        services.AddSingleton<IUserLogic, UserLogic>();
        services.AddSingleton<ILibraryLogic, LibraryLogic>();
        services.AddSingleton<IAuthorLogic, AuthorLogic>();
        services.AddSingleton<ILoggingLogic, LoggingLogic>();

        services.AddSingleton<IValidatable<User>, UserValidator>();
        services.AddSingleton<IValidatable<Author>, AuthorValidator>();
        services.AddSingleton<IValidatable<Book>, BookValidator>();
        services.AddSingleton<IValidatable<Newspaper>, NewspaperValidator>();
        services.AddSingleton<IValidatable<NewspaperIssue>, NewspaperIssueValidator>();
        services.AddSingleton<IValidatable<Patent>, PatentValidator>();

        services.AddSingleton(cfg => new SqlConfig(builder));
        
        return services.BuildServiceProvider();
    }
}