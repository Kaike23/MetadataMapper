﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Repository.UnitOfWork
{
	using Infrastructure.Domain;
	using Infrastructure.Mapping;
	using Infrastructure.UnitOfWork;
	using Repository.Mapping;
	using Repository.Mapping.SQL;
	using Session;

	public class UnitOfWork : IUnitOfWork
	{
		private Dictionary<IUnitOfWorkRepository, List<IEntity>> newEntities;
		private Dictionary<IUnitOfWorkRepository, List<IEntity>> dirtyEntities;
		private Dictionary<IUnitOfWorkRepository, List<IEntity>> removedEntities;
		private SqlMappingRegistry mapRegistry;

		public UnitOfWork()
		{
			newEntities = new Dictionary<IUnitOfWorkRepository, List<IEntity>>();
			dirtyEntities = new Dictionary<IUnitOfWorkRepository, List<IEntity>>();
			removedEntities = new Dictionary<IUnitOfWorkRepository, List<IEntity>>();
			mapRegistry = new SqlMappingRegistry();
		}

		public void RegisterNew(IEntity entity, IUnitOfWorkRepository repository)
		{
			Debug.Assert(entity != null);
			if (dirtyEntities.ContainsKey(repository))
				Debug.Assert(!dirtyEntities[repository].Contains(entity), "object dirty");
			if (removedEntities.ContainsKey(repository))
				Debug.Assert(!removedEntities[repository].Contains(entity), "object removed");
			if (newEntities.ContainsKey(repository))
				Debug.Assert(!newEntities[repository].Contains(entity), "object already registered new");

			if (!newEntities.ContainsKey(repository))
				newEntities.Add(repository, new List<IEntity>());
			newEntities[repository].Add(entity);
		}

		public void RegisterDirty(IEntity entity, IUnitOfWorkRepository repository)
		{
			Debug.Assert(entity.Id != null, "id null");
			if (removedEntities.ContainsKey(repository))
				Debug.Assert(!removedEntities[repository].Contains(entity), "object removed");


			if ((!dirtyEntities.ContainsKey(repository) || (dirtyEntities.ContainsKey(repository) && !dirtyEntities[repository].Contains(entity)))
				&& (!newEntities.ContainsKey(repository) || (newEntities.ContainsKey(repository) && !newEntities[repository].Contains(entity))))
			{
				if (!dirtyEntities.ContainsKey(repository))
					dirtyEntities.Add(repository, new List<IEntity>());
				dirtyEntities[repository].Add(entity);
			}
		}

		public void RegisterRemoved(IEntity entity, IUnitOfWorkRepository repository)
		{
			Debug.Assert(entity.Id != null, "id null");
			if (newEntities.ContainsKey(repository) && newEntities[repository].Remove(entity)) return;
			if (dirtyEntities.ContainsKey(repository)) dirtyEntities[repository].Remove(entity);
			if (!removedEntities.ContainsKey(repository) || (removedEntities.ContainsKey(repository) && !removedEntities[repository].Contains(entity)))
			{
				if (!removedEntities.ContainsKey(repository))
					removedEntities.Add(repository, new List<IEntity>());
				removedEntities[repository].Add(entity);
			}
		}

		public void Commit()
		{
			var session = SessionFactory.GetCurrentSession();
			session.DbInfo.Connection.Open();
			using (var transaction = session.DbInfo.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
			{
				session.DbInfo.Transaction = transaction;
				try
				{
					foreach (var record in newEntities)
						foreach (var entity in record.Value)
							record.Key.PersistCreationOf(entity);

					foreach (var record in dirtyEntities)
						foreach (var entity in record.Value)
							record.Key.PersistUpdateOf(entity);

					foreach (var record in removedEntities)
						foreach (var entity in record.Value)
							record.Key.PersistDeletionOf(entity);

					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
				finally
				{
					newEntities.Clear();
					dirtyEntities.Clear();
					removedEntities.Clear();
					session.DbInfo.Transaction = null;
					session.DbInfo.Connection.Close();
				}
			}
		}

		public IDataMapper GetMapper(Type domainObject)
		{
			return mapRegistry.GetMapper(domainObject);
		}
	}
}
