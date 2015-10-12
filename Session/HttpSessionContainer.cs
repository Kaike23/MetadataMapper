using System.Web;

namespace Session
{
    using Infrastructure.Session;

    public class HttpSessionContainer : ISessionStorageContainer
    {
        private string _sessionKey = "HTTPSession";
        public ISession GetCurrentSession()
        {
            ISession session = null;
            if (HttpContext.Current.Session[_sessionKey] != null)
                session = (ISession)HttpContext.Current.Session[_sessionKey];
            return session;
        }
        public void Store(ISession session)
        {
            if (HttpContext.Current.Session[_sessionKey] != null)
                HttpContext.Current.Session[_sessionKey] = session;
            else
                HttpContext.Current.Session.Add(_sessionKey, session);
        }
    }
}
