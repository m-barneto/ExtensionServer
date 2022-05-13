using NYoutubeDL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionServer {
    internal class YoutubeClipper : Module {
        public YoutubeClipper() {

        }
        public override async void HandleRequest(HttpListenerContext ctx) {
            HttpListenerRequest req = ctx.Request;

            string videoUrl = req.QueryString["videoUrl"]!;
            float start = float.Parse(req.QueryString["start"]!);
            float end = float.Parse(req.QueryString["end"]!);
            int x = int.Parse(req.QueryString["x"]!);
            int y = int.Parse(req.QueryString["y"]!);
            int w = int.Parse(req.QueryString["w"]!);
            int h = int.Parse(req.QueryString["h"]!);
            string savePath = req.QueryString["savePath"]!;
            videoUrl = "https://www.youtube.com/watch?v=RTB5XhjbgZA";
            start = 0f;
            end = 10f;
            x = y = 0;
            w = h = 100;
            savePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Videos" + Path.DirectorySeparatorChar + "video.mp4";
            var youtubeDl = new YoutubeDL();
            youtubeDl.Options.FilesystemOptions.Output = savePath;
            youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDl.VideoUrl = videoUrl;

            // Or update the binary
            youtubeDl.Options.GeneralOptions.Update = true;

            // Optional, required if binary is not in $PATH
            youtubeDl.YoutubeDlPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "youtube-dl.exe";

            youtubeDl.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            youtubeDl.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);

            // Alternatively
            string commandToRun = youtubeDl.PrepareDownload();

            // Just let it run
            await youtubeDl.DownloadAsync();
        }
        public async void test() {
            string videoUrl = "https://www.youtube.com/watch?v=RTB5XhjbgZA";
            float start = 0f;
            float end = 10f;
            int x = 0;
            int y = 0;
            int w = 100;
            int h = 100;

            string savePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Videos" + Path.DirectorySeparatorChar + "video.mkv";


            string output = RunCommand("yt-dlp.exe", $"--youtube-skip-dash-manifest --throttled-rate 100K -g https://www.youtube.com/watch?v=RTB5XhjbgZA");
            string[] lines = output.Split("\n");
            Console.WriteLine(output);
            string video = lines[0];
            string audio = lines[1];
            // ffmpeg -ss 42:30 -i "$video_url" -ss 42:30 -i "$audio_url" -map 0:v -map 1:a -ss 30 -t 7:10 -c:v libx264 -c:a aac gog-vs-triv.mkv
            output = RunCommand("ffmpeg", $"-ss {"3:10"} -i \"{video}\" -ss {"3:10"} -i \"{audio}\" -map 0:v -map 1:a -t {"1:00"} -c:v libx264 -c:a aac {savePath}");


            Console.WriteLine(output);
        }

        private string RunCommand(string file, string args) {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = args;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
    }
}
