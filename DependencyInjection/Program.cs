
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<NetworkClient>();
builder.Services.AddSingleton<MessageFactory>();

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
    public void SendEmail(string username)
    {
        // sketch of what a true implementation might look like
        NetworkClient  nc = new NetworkClient();
        MessageFactory mf = new MessageFactory();


        Debug.WriteLine($"email sent to {username} by EmailSender");
    }
}

public class NetworkClient
{

}

public class MessageFactory
{ }

