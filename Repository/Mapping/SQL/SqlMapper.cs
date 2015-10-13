using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Repository.Mapping.SQL
{
	using Infrastructure.Domain;
	using Infrastructure.Mapping;
	using Infrastructure.Session;
	using Repository.Metadata;
	using Session;

	public abstract class SQLMapper<T> : IDataMapper
		where T : EntityBase
	{
		private IMetadataMap _metadataMap;

		public SQLMapper()
		{
			_metadataMap = LoadMetadataMap();
		}

		protected Dictionary<Guid, EntityBase> loadedMap = new Dictionary<Guid, EntityBase>();

		protected abstract IMetadataMap LoadMetadataMap();
		public IMetadataMap MetadataMap { get { return _metadataMap; } }

		#region IDataMapper

		public EntityBase Find(Guid id)
		{
			EntityBase result;
			if (loadedMap.TryGetValue(id, out result))
				return result;

			var findQuery = string.Format("SELECT {0} FROM {1} WHERE Id = @Id", MetadataMap.ColumnList(), MetadataMap.TableName);
			try
			{
				SessionFactory.GetCurrentSession().DbInfo.Connection.Open();
				using (var sqlCommand = new SqlCommand(findQuery, DBConnection))
				{
					sqlCommand.Parameters.AddWithValue("@Id", id);
					var reader = sqlCommand.ExecuteReader();
					result = Load(reader);
					reader.Close();
				}
				return result;
			}
			catch (SqlException e)
			{
				throw new ApplicationException(e.Message, e);
			}
			finally
			{
				SessionFactory.GetCurrentSession().DbInfo.Connection.Close();
			}
		}
		public List<EntityBase> FindMany(IStatementSource source)
		{
			try
			{
				SessionFactory.GetCurrentSession().DbInfo.Connection.Open();
				var sqlCommand = new SqlCommand(source.Query, DBConnection);
				sqlCommand.Parameters.AddRange(source.Parameters.ToArray());
				var reader = sqlCommand.ExecuteReader();
				var result = LoadAll(reader);
				reader.Close();
				return result;
			}
			catch (SqlException e)
			{
				throw new ApplicationException(e.Message, e);
			}
			finally
			{
				SessionFactory.GetCurrentSession().DbInfo.Connection.Close();
			}
		}
		public List<EntityBase> FindMany(string whereClause)
		{
			string sql = string.Format("SELECT {0} FROM {1} WHERE {2}", MetadataMap.ColumnList(), MetadataMap.TableName, whereClause);
			try
			{
				SessionFactory.GetCurrentSession().DbInfo.Connection.Open();
				var sqlCommand = new SqlCommand(sql, DBConnection);
				var reader = sqlCommand.ExecuteReader();
				var result = LoadAll(reader);
				reader.Close();
				return result;
			}
			catch (SqlException e)
			{
				throw new ApplicationException(e.Message, e);
			}
			finally
			{
				SessionFactory.GetCurrentSession().DbInfo.Connection.Close();
			}
		}
		public Guid Insert(EntityBase entity)
		{
			var insertQuery = "INSERT INTO " + MetadataMap.TableName + " VALUES (" + MetadataMap.InsertList() + ")";
			try
			{
				using (var sqlCommand = new SqlCommand(insertQuery, DBConnection, DBTransaction))
				{
					foreach (var columnMap in MetadataMap.Columns)
					{
						sqlCommand.Parameters.AddWithValue("@" + columnMap.FieldName, columnMap.GetValue(entity));
					}
					var rowCount = sqlCommand.ExecuteNonQuery();
					if (rowCount == 0)
					{
						throw new Exception(string.Format("Concurrency exception on {0}", entity.Id));
					}
				}
				loadedMap.Add(entity.Id, entity as T);
				return entity.Id;
			}
			catch (SqlException e)
			{
				throw new ApplicationException(e.Message, e);
			}
		}
		public void Update(EntityBase entity)
		{
			var updateQuery = "UPDATE " + MetadataMap.TableName + MetadataMap.UpdateList() + " WHERE Id = @Id";
			try
			{
				using (var sqlCommand = new SqlCommand(updateQuery, DBConnection, DBTransaction))
				{
					foreach (var columnMap in MetadataMap.Columns)
					{
						sqlCommand.Parameters.AddWithValue("@" + columnMap.FieldName, columnMap.GetValue(entity));
					}
					var rowCount = sqlCommand.ExecuteNonQuery();
					if (rowCount == 0)
					{
						throw new Exception(string.Format("Concurrency exception on {0}", entity.Id));
					}
				}
			}
			catch (SqlException e)
			{
				throw new ApplicationException(e.Message, e);
			}
		}
		public void Delete(EntityBase entity)
		{
			var deleteQuery = string.Format("DELETE FROM {0} WHERE Id = @Id AND Version = @Version", MetadataMap.TableName);
			try
			{
				var deleteCommand = new SqlCommand(deleteQuery, DBConnection, DBTransaction);
				deleteCommand.Parameters.AddWithValue("@Id", entity.Id);
				deleteCommand.Parameters.AddWithValue("@Version", entity.Version);
				deleteCommand.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				throw new ApplicationException(e.Message, e);
			}
		}

		#endregion

		private void LoadFields(SqlDataReader reader, EntityBase result)
		{
			for (var i = 0; i < MetadataMap.Columns.Count; i++)
			{
				var columnValue = reader.GetValue(i);
				var columnMap = MetadataMap.Columns[i];
				columnMap.SetField(result, columnValue);
			}
		}

		protected EntityBase Load(SqlDataReader reader)
		{
			if (!reader.HasRows || !reader.Read()) return default(T);
			var id = reader.GetGuid(0);
			if (loadedMap.ContainsKey(id)) return loadedMap[id];

			var resultEntity = (EntityBase)Activator.CreateInstance(MetadataMap.DomainClass);
			LoadFields(reader, resultEntity);

			loadedMap.Add(id, resultEntity);
			return resultEntity;
		}
		protected List<EntityBase> LoadAll(SqlDataReader reader)
		{
			var resultEntities = new List<EntityBase>();
			var entity = Load(reader);
			while (entity != null)
			{
				resultEntities.Add(entity);
				entity = Load(reader);
			}
			return resultEntities;
		}

		private SqlConnection DBConnection { get { return (SqlConnection)Session.DbInfo.Connection; } }
		private SqlTransaction DBTransaction { get { return (SqlTransaction)Session.DbInfo.Transaction; } }

		private ISession Session { get { return SessionFactory.GetCurrentSession(); } }
	}
}
