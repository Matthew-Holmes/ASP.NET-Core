
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<NetworkClient>();
builder.Services.AddSingleton<MessageFactory>();
builder.Services.AddSingleton(
    new EmailServerSettings(
            Host: "a.server.com",
            Port: 42
        ));

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("/register/{username}", RegisterUser);

app.Run();


string RegisterUser(string username, IEmailSender emailSender)
{
    emailSender.SendEmail(username);
    return $"email sent to {username}";
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