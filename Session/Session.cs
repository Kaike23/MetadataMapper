using System;
using System.Data;
using System.Data.SqlClient;

namespace Session
{
    using Infrastructure.Identity;
    using Infrastructure.Session;

    public class Session : ISession
    {
        private IdentityMap map;

        public Session(string name, string connectionInfo)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.map = new IdentityMap();
            this.DbInfo = new DbSessionInfo(connectionInfo);
            this.LockManager = new LockManager(this);
        }

        #region ISession implementation

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public IDbSessionInfo DbInfo { get; private set; }

        public ILockManager LockManager { get; private set; }

        public IdentityMap GetIdentityMap()
        {
            return this.map;
        }

        public void Close()
        {
            this.map.Clear();
            if (DbInfo.Transaction != null)
                DbInfo.Transaction.Rollback();
            DbInfo.Connection.Close();
        }

        #endregion

        private sealed class DbSessionInfo : IDbSessionInfo
        {
            private IDbTransaction transaction;

            public DbSessionInfo(string info)
            {
                this.Connection = new SqlConnection(info);
            }

            public IDbConnection Connection { get; private set; }

            public IDbTransaction Transaction
            {
                get { return this.transaction; }
                set
                {
                    if (this.transaction != null && value != null)
                        throw new InvalidOperationException();
                    this.transaction = value;
                }
            }
        }
    }
}
