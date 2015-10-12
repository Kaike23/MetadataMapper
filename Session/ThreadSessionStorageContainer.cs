using System.Collections;
using System.Threading;

namespace Session
{
    using Infrastructure.Session;

    public class ThreadSessionStorageContainer : ISessionStorageContainer
    {
        private static readonly Hashtable _sessions = new Hashtable();
        public ISession GetCurrentSession()
        {
            ISession nhSession = null;
            if (_sessions.Contains(GetThreadName()))
                nhSession = (ISession)_sessions[GetThreadName()];
            return nhSession;
        }
        public void Store(ISession session)
        {
            if (_sessions.Contains(GetThreadName()))
                _sessions[GetThreadName()] = session;
            else
                _sessions.Add(GetThreadName(), session);
        }
        private static string GetThreadName()
        {
            return Thread.CurrentThread.Name;
        }
    }
}
