
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// extracted adding multiple services to a utility method
builder.Services.AddEmailSender();

// scoping demo

// Transient
// The row counts returned by the datacontext vs the repo need not be the same
// even on the same request handling
//builder.Services.AddTransient<DataContext>();
//builder.Services.AddTransient<Repository>();

// Scoped
// The row counts returned by the datacontext vs the repo will be identical
// on the same request handling, since the datacontext is reused upon Repository
// construction (since same request = same scope)
//builder.Services.AddScoped<DataContext>();
//builder.Services.AddScoped<Repository>();

// Singleton
// only ever one Datacontext and one Repository, row counts invariant under requests
// Warning: these must be thread safe operations (hence use concurrent queue)
builder.Services.AddSingleton<DataContext>();
builder.Services.AddSingleton<Repository>();

builder.Services.AddSingleton(
    new ConcurrentQueue<string>()); // quick hack, should be a IHistoryService or something

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/register/{username}", RegisterUser);
app.MapGet("/scoping", RowCounts);

app.Run();

static string RowCounts(
    DataContext db,
    Repository repository,
    ConcurrentQueue<string> previous)
{
    int dbCount = db.RowCount;
    int repositoryCount = repository.RowCount;

    string ret = $"DataContext: {dbCount}, Repository: {repositoryCount}";

    previous.Enqueue(ret);

    StringBuilder message = new StringBuilder(ret);
    message.AppendLine();
    message.AppendLine();
    message.AppendLine("history:");

    foreach (string s in previous)
    {
        message.AppendLine(s);
    }

    return message.ToString();
}


#region DI demo
string RegisterUser(string username, IEmailSender emailSender)
{
    emailSender.SendEmail(username);
    return $"email sent to {username}";
}

// create an extension method to tidy up adding multiple, linked, services
public static class EmailSenderServiceCollectionExtensions
{
    // create an extension on the IServiceCollection using `this` keyword
    public static IServiceCollection AddEmailSender(
        this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<NetworkClient>();
        services.AddSingleton<MessageFactory>();
        //builder.Services.AddSingleton(
        //    new EmailServerSettings(
        //            Host: "a.server.com",
        //            Port: 42
        //        ));
        services.AddScoped(
            provider =>
                new EmailServerSettings(
                        Host: "a.server.com",
                        Port: 42
                    ));
        return services;
    }
}

public interface IEmailSender
{
    public void SendEmail(string username);
}

public class EmailSender : IEmailSender
{
    private readonly NetworkClient _networkClient;
    private readonly MessageFactory _messageFactory;
    private readonly EmailServerSettings _emailServerSettings;

    public EmailSender(NetworkClient networkClient, MessageFactory messageFactory, EmailServerSettings emailServerSettings)
    {
        _networkClient = networkClient;
        _messageFactory = messageFactory;
        _emailServerSettings = emailServerSettings;
    }

    public void SendEmail(string username)
    {
        Debug.WriteLine($"Email sent to {username} using server {_emailServerSettings.Host}:{_emailServerSettings.Port}");
    }
}

public class NetworkClient
{ }

public class MessageFactory
{ }

public record EmailServerSettings(string Host, int Port);

#endregion

#region DI scope demo


public class Repository
{
    private readonly DataContext _dataContext;
    public Repository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public int RowCount => _dataContext.RowCount;
}


// models a DataContext on an underlying database
public class DataContext
{
    public int RowCount { get; }
        = Random.Shared.Next(1, 1_000_000_000);
}

#endregion

