using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Repository.Mapping.SQL
{
    using Infrastructure.Mapping;
    using Model.Order;
    using Repository.Metadata;
    using Session;

    public class OrderMapper : SQLMapper<Order>
    {
        public OrderMapper() : base() { }

        protected override MetadataMap LoadMetadataMap()
        {
            var metadataMap = new MetadataMap(typeof(Order), "Orders");
            metadataMap.AddColumn("CustomerId", "CustomerId");
            metadataMap.AddColumn("OrderDate", "OrderDate");
            metadataMap.AddColumn("Version", "Version");
            metadataMap.AddColumn("CreatedBy", "CreatedBy");
            metadataMap.AddColumn("Created", "Created");
            metadataMap.AddColumn("ModifiedBy", "ModifiedBy");
            metadataMap.AddColumn("Modified", "Modified");
            return metadataMap;
        }

        public IEnumerable<Order> FindAllBy(DateTime orderDate)
        {
            return FindMany(new FindByDateStatement(orderDate, MetadataMap)).ConvertAll<Order>(x => x as Order);
        }
        private class FindByDateStatement : IStatementSource
        {
            private DateTime _orderDate;
            private MetadataMap _metadataMap;
            public FindByDateStatement(DateTime orderDate, MetadataMap metadataMap)
            {
                _orderDate = orderDate;
                _metadataMap = metadataMap;
            }
            public List<IDbDataParameter> Parameters
            {
                get
                {
                    var parameters = new List<IDbDataParameter>();
                    parameters.Add(new SqlParameter("@OrderDate", _orderDate));
                    return parameters;
                }
            }
            public string Query
            {
                get
                {
                    return "SELECT " + _metadataMap.ColumnList() +
                           " FROM " + _metadataMap.TableName +
                           " WHERE OrderDate  = @OrderDate" +
                           " ORDER BY OrderDate";
                }
            }
        }
        public IEnumerable<Order> FindAllBy(Guid customerId)
        {
            return FindMany(new FindByCustomerStatement(customerId, MetadataMap)).ConvertAll<Order>(x => x as Order);
        }
        private class FindByCustomerStatement : IStatementSource
        {
            private Guid _customerId;
            private MetadataMap _metadataMap;
            public FindByCustomerStatement(Guid customerId, MetadataMap metadataMap)
            {
                _customerId = customerId;
                _metadataMap = metadataMap;
            }
            public List<IDbDataParameter> Parameters
            {
                get
                {
                    var parameters = new List<IDbDataParameter>();
                    parameters.Add(new SqlParameter("@CustomerId", _customerId));
                    return parameters;
                }
            }
            public string Query
            {
                get
                {
                    return "SELECT " + _metadataMap.ColumnList() +
                           " FROM " + _metadataMap.TableName +
                           " WHERE CustomerId  = @CustomerId" +
                           " ORDER BY OrderDate";
                }
            }
        }
    }
}
