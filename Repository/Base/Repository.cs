using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Repository.Base
{
    using Infrastructure.Domain;
    using Infrastructure.Mapping;
    using Infrastructure.UnitOfWork;

    public abstract class Repository<T> : ReadOnlyRepository<T>, IRepository<T>, IUnitOfWorkRepository
        where T : EntityBase, IAggregateRoot
    {
        private IUnitOfWork _uow;

        public Repository(IUnitOfWork uow)
        {
            _uow = uow;
        }

        #region IRepository

        public void Add(T entity)
        {
            _uow.RegisterNew(entity, this);
        }
        public void Update(T entity)
        {
            _uow.RegisterDirty(entity, this);
        }

        public void Remove(T entity)
        {
            _uow.RegisterRemoved(entity, this);
        }

        #endregion

        #region IUnitOfWorkRepository

        void IUnitOfWorkRepository.PersistCreationOf(IEntity entity) { DataMapper.Insert((T)entity); }
        void IUnitOfWorkRepository.PersistUpdateOf(IEntity entity) { DataMapper.Update((T)entity); }
        void IUnitOfWorkRepository.PersistDeletionOf(IEntity entity) { DataMapper.Delete((T)entity); }

        #endregion
    }
}
