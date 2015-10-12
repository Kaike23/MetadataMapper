using System;

namespace Model.Order
{
    using Infrastructure.Domain;
    using Session;

    public class Order : EntityBase, IAggregateRoot
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }

        public Order() : base() { }

        public Order(Guid id, Guid customerId, DateTime orderDate)
            : base(id)
        {
            CustomerId = customerId;
            OrderDate = orderDate;
        }

        public static Order Create(Guid customerId)
        {
            var session = SessionFactory.GetCurrentSession();

            var date = DateTime.Now;
            var order = new Order(Guid.NewGuid(), customerId, date);
            order.CreatedBy = session.Name;
            order.Created = date;
            order.SetSystemFields(0, order.CreatedBy, order.Created);
            return order;
        }
    }
}
