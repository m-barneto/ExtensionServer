using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionServer {
    internal class PandoraBot : Module {

        public override async void HandleRequest(HttpListenerContext ctx) {
            HttpListenerRequest req = ctx.Request;

        }
    }
}
