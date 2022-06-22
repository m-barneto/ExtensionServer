using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionServer {
    abstract class Module {
        public abstract void HandleRequest(HttpListenerContext ctx);
    }
}
