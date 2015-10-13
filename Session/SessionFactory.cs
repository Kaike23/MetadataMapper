using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Session
{
	using Infrastructure.Session;

	public sealed class SessionFactory : ISessionFactory
	{
		private static ISessionFactory _sessionFactory;

		private static readonly string SQL_CONNECTION = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\GitHub\MetadataMapper\TestApp\TestDB.mdf;Integrated Security=True";

		private SessionFactory()
		{ }

		private static ISessionFactory GetSessionFactory()
		{
			if (_sessionFactory == null)
				_sessionFactory = new SessionFactory();
			return _sessionFactory;
		}
		private static ISession GetNewSession()
		{
			return GetSessionFactory().Open();
		}
		public static ISession GetCurrentSession()
		{
			ISessionStorageContainer sessionStorageContainer = SessionStorageFactory.GetStorageContainer();
			ISession currentSession = sessionStorageContainer.GetCurrentSession();
			if (currentSession == null)
			{
				currentSession = GetNewSession();
				sessionStorageContainer.Store(currentSession);
			}
			return currentSession;
		}


		#region ISessionFactory implementation

		public Guid Current { get; set; }

		public ISession Open()
		{
			var name = "Session_" + new Random().Next(1000).ToString();
			return new Session(name, SQL_CONNECTION);
		}

		public void Close()
		{
			ISessionStorageContainer sessionStorageContainer = SessionStorageFactory.GetStorageContainer();
			ISession currentSession = sessionStorageContainer.GetCurrentSession();
			currentSession.Close();
		}

		#endregion
	}
}
