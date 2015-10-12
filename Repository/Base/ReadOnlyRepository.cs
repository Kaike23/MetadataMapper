using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Base
{
    using Infrastructure.Domain;
    using Infrastructure.Mapping;

    public abstract class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : EntityBase, IAggregateRoot
    {
        private IDataMapper dataMapper;
        protected abstract string TableName { get; }

        protected IDataMapper DataMapper
        {
            get
            {
                if (dataMapper == null)
                    dataMapper = CreateDataMapper();
                return dataMapper;
            }
        }
        protected abstract IDataMapper CreateDataMapper();

        public T FindBy(Guid id)
        {
            return DataMapper.Find(id) as T;
        }
        public IEnumerable<T> FindAll()
        {
            return DataMapper.FindMany(new FindAllStatement(TableName)).ConvertAll<T>(x => x as T);
        }
        private class FindAllStatement : IStatementSource
        {
            public FindAllStatement(string query)
            {
                Query = "SELECT * FROM " + query;
            }
            public List<IDbDataParameter> Parameters { get { return new List<IDbDataParameter>(); } }
            public string Query { get; private set; }
        }

    }
}
