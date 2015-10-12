using System;
using System.Collections.Generic;

namespace Model.Customer
{
    using Infrastructure.Domain;
    using Model.Order;
    using Session;

    public class Customer : EntityBase, IAggregateRoot
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual IEnumerable<Order> Orders { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public Customer() : base() { }

        public Customer(Guid id, string firstName, string lastName)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Orders = new List<Order>();
        }

        public static Customer Create(string firstName, string lastName)
        {
            var session = SessionFactory.GetCurrentSession();

            var customer = new Customer(Guid.NewGuid(), firstName, lastName);
            customer.CreatedBy = session.Name;
            customer.Created = DateTime.Now;
            customer.SetSystemFields(0, customer.CreatedBy, customer.Created);
            return customer;
        }
    }
}