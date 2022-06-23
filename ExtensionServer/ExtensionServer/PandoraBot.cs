using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ExtensionServer {
    public class CommandHandler {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands) {
            this.commands = commands;
            this.client = client;
        }

        public async Task InstallCommandsAsync() {
            client.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam) {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }
    }

    public class InfoModule : ModuleBase<SocketCommandContext> {
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        // ReplyAsync is a method on ModuleBase 
    }


    internal class PandoraBot : Module {
        DiscordSocketClient client;
        CommandHandler commands;
        CommandService commandService;
        public PandoraBot() {
            Task.Run(async () => {
                await BotMain();
            });
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task BotMain() {
            client = new DiscordSocketClient();
            commandService = new CommandService();
            commands = new CommandHandler(client, commandService);

            client.Log += Log;

            var token = File.ReadAllText(".env");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await commands.InstallCommandsAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public override async void HandleRequest(HttpListenerContext ctx) {
            HttpListenerRequest req = ctx.Request;
            string url = HttpUtility.UrlDecode(req.QueryString["url"]!);
            double secs = double.Parse(req.QueryString["time"]!);
            Console.WriteLine($"URL: {url}\nTime: {secs}");
        }
    }
}
