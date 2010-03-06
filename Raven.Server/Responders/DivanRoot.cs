using System.Net;

namespace Raven.Server.Responders
{
    public class DivanRoot : RequestResponder
    {
        public override string UrlPattern
        {
            get { return "^/divan$"; }
        }

        public override string[] SupportedVerbs
        {
            get { return new[] {"GET"}; }
        }

        public override void Respond(HttpListenerContext context)
        {
            context.Response.Redirect("/divan/index.html");
        }
    }
}