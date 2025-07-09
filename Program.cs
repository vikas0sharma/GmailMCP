using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Protocol;

var builder = WebApplication.CreateBuilder(args);
//Add MCP server services to the DI
builder.Services.AddMcpServer(o =>
{
    o.ServerInfo = new Implementation { Name = "Gmail MCP Server", Version = "1.0.0", Title = "GmailMCP" };
})
.WithHttpTransport()
.WithToolsFromAssembly();

// Gmail Sevices

builder.Services.AddSingleton(sp =>
{

    UserCredential credential;
    string[] Scopes = { GmailService.Scope.GmailModify };
    // Load client secrets.
    using (var stream =
           new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
    {
        /* The file token.json stores the user's access and refresh tokens, and is created
         automatically when the authorization flow completes for the first time. */
        string credPath = "token.json";
        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)).Result;
        Console.WriteLine("Credential file saved to: " + credPath);
    }

    return new GmailService(new Google.Apis.Services.BaseClientService.Initializer
    {
        HttpClientInitializer = credential,
        ApplicationName = "Gmail MCP Application",
    });
});

var app = builder.Build();
app.MapMcp();
await app.RunAsync();