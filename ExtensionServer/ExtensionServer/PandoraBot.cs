using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.WebSocket;

namespace ExtensionServer {
    internal class PandoraBot : Module {
        DiscordSocketClient client;
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

            client.Log += Log;

            var token = File.ReadAllText(".env");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

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
