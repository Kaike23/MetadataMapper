using System.Collections.Generic;

namespace Repository.Repositories
{
	using Infrastructure.Mapping;
	using Infrastructure.UnitOfWork;
	using Model.Customer;
	using Repository.Base;
	using Repository.Mapping.SQL;

	public class CustomerRepository : Repository<Customer>, ICustomerRepository
	{
		public CustomerRepository(IUnitOfWork uow) : base(uow) { }
		protected override sealed string TableName { get { return "Customers"; } }
		protected override sealed IDataMapper CreateDataMapper() { return new CustomerMapper(); }
		public IEnumerable<Customer> FindBy(string firstName, string lastName) { return ((CustomerMapper)DataMapper).FindBy(firstName, lastName); }
	}
}
