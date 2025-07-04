# GmailMCP

GmailMCP is a .NET 8 application that exposes Gmail operations (read, send, draft emails) as Model Context Protocol (MCP) server tools. It uses the Google Gmail API and can be integrated with MCP-compatible clients.

## Features
- **Read Latest Email**: Fetches the most recent email from your Gmail inbox.
- **Send Email**: Sends an email using your Gmail account.
- **Draft Email**: Creates a draft email in your Gmail account.

## Requirements
- .NET 8 SDK
- Google Cloud project with Gmail API enabled
- OAuth 2.0 credentials (`credentials.json`)

## Setup
1. **Clone the repository**
2. **Enable Gmail API** in your Google Cloud project and download `credentials.json`.
3. **Place `credentials.json`** in the project root directory.
4. **Build and run the project**:
   ```sh
   dotnet build
   dotnet run
   ```
5. On first run, authenticate in the browser. A `token.json` file will be created for future runs.

## Usage
The MCP server exposes the following tools:
- `ReadLatestEmail`: Reads the latest email.
- `SendEmail(to, subject, body)`: Sends an email.
- `DraftEmail(to, subject, body)`: Creates a draft email.

You can interact with these tools using any MCP-compatible client.

## Project Structure
- `GmailTool.cs`: Implements the Gmail operations as MCP tools.
- `Program.cs`: Configures dependency injection, Gmail API, and MCP server.
- `credentials.json`: OAuth 2.0 credentials (not included; provide your own).

## Dependencies
- [Google.Apis.Gmail.v1](https://www.nuget.org/packages/Google.Apis.Gmail.v1)
- [ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol)
- [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting)

## License
This project is for demonstration purposes. See individual package licenses for details.
