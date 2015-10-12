using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Session
{
    using Infrastructure.Session;

    public class LockManager : ILockManager
    {
        private Dictionary<Guid, LockType> locks;
        private static readonly string CREATE_QUERY = "INSERT INTO Locks VALUES(@LockableId, @OwnerId, @LockType)";
        private static readonly string READ_QUERY = "SELECT OwnerId, LockType FROM Locks WHERE LockableId = @LockableId";
        private static readonly string UPDATE_QUERY = "UPDATE Locks SET LockType = @LockType WHERE LockableId = @LockableId AND OwnerId = @OwnerId";
        private static readonly string DELETE_QUERY = "DELETE FROM Locks WHERE LockableId = @LockableId AND OwnerId = @OwnerId";
        private static readonly string DELETEALL_QUERY = "DELETE FROM Locks WHERE OwnerId = @OwnerId";
        private static readonly string SQL_CONNECTION = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\GitHub\ImplicitLock\MVCApp\App_Data\TestDB.mdf;Integrated Security=True;Connect Timeout=30";

        public LockManager(ISession session)
        {
            this.locks = new Dictionary<Guid, LockType>();
            Session = session;
        }

        public ISession Session { get; private set; }

        public void GetLock(Guid entityId, LockType lockType)
        {
            LockType value;
            if (locks.TryGetValue(entityId, out value) && lockType == value)
                return;

            var connection = new SqlConnection(SQL_CONNECTION);
            connection.Open();
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var reader = ExecuteReader(entityId, connection, transaction);
                    var query = CREATE_QUERY;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var currentOwnerId = reader.GetGuid(0);
                            var type = reader.GetInt16(1);

                            if (!Enum.TryParse<LockType>(type.ToString(), out value))
                            {
                                reader.Close();
                                throw new Exception("Wrong LockType fetched. " + type);
                            }

                            if ((lockType == LockType.Write || value == LockType.Write) && Session.Id.CompareTo(currentOwnerId) != 0)
                            {
                                reader.Close();
                                throw new Exception(string.Format("Can't get {0} lock for {1}, session ID '{2}' has {3} lock.", lockType.ToString(), Session.Name, currentOwnerId.ToString(), value.ToString()));
                            }

                            if (Session.Id.CompareTo(currentOwnerId) == 0)
                                query = UPDATE_QUERY;
                        }
                    }
                    reader.Close();
                    ExecuteNonQuery(query, entityId, lockType, connection, transaction);
                    transaction.Commit();
                    if (locks.ContainsKey(entityId))
                        locks[entityId] = lockType;
                    else
                        locks.Add(entityId, lockType);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public bool ReleaseLock(Guid entityId)
        {
            if (!locks.ContainsKey(entityId))
                return true;

            var connection = new SqlConnection(SQL_CONNECTION);
            connection.Open();
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteDelete(entityId, connection, transaction);
                    transaction.Commit();
                    locks.Remove(entityId);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }

        public bool ReleaseAllLocks()
        {
            var connection = new SqlConnection(SQL_CONNECTION);
            connection.Open();
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteDeleteAll(Session.Id, connection, transaction);
                    transaction.Commit();
                    locks.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }

        private SqlDataReader ExecuteReader(Guid entityId, IDbConnection connection, IDbTransaction transaction)
        {
            var command = new SqlCommand(READ_QUERY, (SqlConnection)connection, (SqlTransaction)transaction);
            command.Parameters.AddWithValue("@LockableId", entityId);
            return command.ExecuteReader();
        }

        private int ExecuteNonQuery(string query, Guid entityId, LockType lockType, IDbConnection connection, IDbTransaction transaction)
        {
            var command = new SqlCommand(query, (SqlConnection)connection, (SqlTransaction)transaction);
            command.Parameters.AddWithValue("@LockableId", entityId);
            command.Parameters.AddWithValue("@OwnerId", Session.Id);
            command.Parameters.AddWithValue("@LockType", lockType);
            return command.ExecuteNonQuery();
        }

        private int ExecuteDelete(Guid entityId, IDbConnection connection, IDbTransaction transaction)
        {
            var command = new SqlCommand(DELETE_QUERY, (SqlConnection)connection, (SqlTransaction)transaction);
            command.Parameters.AddWithValue("@LockableId", entityId);
            command.Parameters.AddWithValue("@OwnerId", Session.Id);
            return command.ExecuteNonQuery();
        }

        private int ExecuteDeleteAll(Guid ownerId, IDbConnection connection, IDbTransaction transaction)
        {
            var command = new SqlCommand(DELETEALL_QUERY, (SqlConnection)connection, (SqlTransaction)transaction);
            command.Parameters.AddWithValue("@OwnerId", ownerId);
            return command.ExecuteNonQuery();
        }
    }
}
