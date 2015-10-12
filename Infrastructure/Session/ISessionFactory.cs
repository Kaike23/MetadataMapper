using System;

namespace Infrastructure.Session
{
    public interface ISessionFactory
    {
        ISession Open();
        void Close();
    }
}