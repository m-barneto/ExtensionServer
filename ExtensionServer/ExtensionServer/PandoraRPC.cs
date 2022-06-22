using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ExtensionServer {
    internal class PandoraRPC : Module {
        const string applicationID = "855107548037644318";
        public long heartbeat = DateTime.Now.Ticks;
        DiscordRpcClient? client = null;
        readonly string appPath = Directory.GetCurrentDirectory();

        string lastSongName = "";
        readonly Dictionary<string, HashSet<string>> stations = new();
        readonly System.Timers.Timer timer;

        public PandoraRPC() {
            timer = new System.Timers.Timer(10000);
            timer.Elapsed += HeartBeat;
            timer.AutoReset = true;
            timer.Enabled = true;

            client = new DiscordRpcClient(applicationID);
            client.Initialize();
        }
        ~PandoraRPC() {
            client?.Dispose();
            timer.Stop();
            timer.Dispose();
        }

        public void HeartBeat(object? source, ElapsedEventArgs e) {
            if (DateTime.Now.Ticks - heartbeat >= 2 * 1000 * 10000) {
                if (client != null) {
                    client.Dispose();
                    client = null;
                }
                heartbeat = DateTime.Now.Ticks;
            }
        }

        public override async void HandleRequest(HttpListenerContext ctx) {
            HttpListenerRequest req = ctx.Request;

            heartbeat = DateTime.Now.Ticks;
            RichPresence rpc = new() {
                Details = req.QueryString["details"],
                State = (req.QueryString["artist"] + " - " + req.QueryString["album"]).Length > 128 ? (req.QueryString["artist"] + " - " + req.QueryString["album"]).Substring(0, 128) : (req.QueryString["artist"] + " - " + req.QueryString["album"]),
                Assets = new Assets() {
                    LargeImageKey = req.QueryString["largeImageKey"],
                    LargeImageText = req.QueryString["largeImageText"]
                },
                Timestamps = Timestamps.FromTimeSpan(double.Parse(req.QueryString["endTimestamp"]!))
            };
            if (client == null) {
                client = new DiscordRpcClient(applicationID);
                client.Initialize();
            }
            client.SetPresence(rpc);

            if (lastSongName != req.QueryString["details"]) {
                lastSongName = req.QueryString["details"]!;
                OnSongChanged(req.QueryString["details"]!, req.QueryString["artist"] + " - " + req.QueryString["album"], req.QueryString["largeImageText"]!);
            }
        }

        void OnSongChanged(string song, string details, string station) {
            try {
                // Construct the filepath for the active station
                string stationFilePath = appPath + Path.DirectorySeparatorChar + station + ".txt";

                // If the file doesnt exist, create it
                if (!File.Exists(stationFilePath)) {
                    FileStream fs = File.Create(stationFilePath);
                    fs.Close();
                }

                // If the station isn't already initialized this listening session, initialize it and add previous songs to our list
                if (!stations.ContainsKey(stationFilePath)) {
                    stations.Add(stationFilePath, new HashSet<string>());
                    foreach (string line in File.ReadLines(stationFilePath)) {
                        stations[stationFilePath].Add(line);
                    }
                }

                // Add the song to the station's file
                stations[stationFilePath].Add($"{ details }^{ song }");

                // Save our song list (with the new song) to the station's file
                File.WriteAllLines(stationFilePath, stations[stationFilePath]);
            }
            catch (Exception ex) {
                Console.Error.WriteLine($"{ ex.StackTrace }: { ex.Message }", $"Error while listening to { station }");
            }
        }
    }
}
