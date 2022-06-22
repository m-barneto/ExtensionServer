using System.Net;
using ExtensionServer;

PandoraRPC pandoraRPC = new PandoraRPC();
PandoraBot pandoraBot = new PandoraBot();
HttpListener httpListener = new HttpListener();
httpListener.Prefixes.Add("http://localhost:8080/");
httpListener.Start();

Task listenTask = HandleIncomingConnections();
listenTask.Wait();

httpListener.Close();


async Task HandleIncomingConnections() {
    while (true) {
        HttpListenerContext ctx = await httpListener.GetContextAsync();

        HttpListenerRequest req = ctx.Request;
        HttpListenerResponse resp = ctx.Response;
        resp.AppendHeader("Access-Control-Allow-Origin", "*");

        if (req.HttpMethod.Equals("POST")) {
            string? dest = req.QueryString["dest"]?.ToLower();
            if (dest == null) {
                resp.Close();
            } else {
                switch (dest) {
                    case "pandorarpc":
                        pandoraRPC.HandleRequest(ctx);
                        break;
                    case "pandorabot":
                        pandoraBot.HandleRequest(ctx);
                        break;
                }
            }
        }
        resp.Close();
    }
}