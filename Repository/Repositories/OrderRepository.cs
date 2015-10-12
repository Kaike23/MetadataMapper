using System;
using System.Collections.Generic;

namespace Repository.Repositories
{
    using Infrastructure.Mapping;
    using Infrastructure.UnitOfWork;
    using Model.Order;
    using Repository.Base;
    using Repository.Mapping.SQL;

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork uow) : base(uow) { }
        protected override sealed string TableName { get { return "Orders"; } }
        protected override sealed IDataMapper CreateDataMapper() { return new OrderMapper(); }
        public IEnumerable<Order> FindAllBy(Guid customerId) { return ((OrderMapper)DataMapper).FindAllBy(customerId); }
    }
}
