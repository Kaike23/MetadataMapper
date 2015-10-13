using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Repository.Mapping.SQL
{
	using Infrastructure.Mapping;
	using Model.Customer;
	using Repository.Metadata;

	public class CustomerMapper : SQLMapper<Customer>
	{
		public CustomerMapper() : base() { }

		protected override MetadataMap LoadMetadataMap()
		{
			var metadataMap = new MetadataMap(typeof(Customer), "Customers");
			metadataMap.AddColumn("FirstName", "FirstName");
			metadataMap.AddColumn("LastName", "LastName");
			metadataMap.AddColumn("Version", "Version");
			metadataMap.AddColumn("CreatedBy", "CreatedBy");
			metadataMap.AddColumn("Created", "Created");
			metadataMap.AddColumn("ModifiedBy", "ModifiedBy");
			metadataMap.AddColumn("Modified", "Modified");

			return metadataMap;
		}

		public List<Customer> FindBy(string firstName, string lastName)
		{
			return FindMany(new FindByNameStatement(firstName, lastName, MetadataMap)).ConvertAll<Customer>(x => x as Customer);
		}
		private class FindByNameStatement : IStatementSource
		{
			private string _firstName;
			private string _lastName;
			private MetadataMap _metadataMap;
			public FindByNameStatement(string firstName, string lastName, MetadataMap metadataMap)
			{
				_firstName = firstName;
				_lastName = lastName;
				_metadataMap = metadataMap;
			}
			public List<IDbDataParameter> Parameters
			{
				get
				{
					var parameters = new List<IDbDataParameter>();
					parameters.Add(new SqlParameter("@FirstName", _firstName));
					parameters.Add(new SqlParameter("@LastName", _lastName));
					return parameters;
				}
			}
			public string Query
			{
				get
				{
					return "SELECT " + _metadataMap.ColumnList() +
						   " FROM " + _metadataMap.TableName +
						   " WHERE UPPER(FirstName) like UPPER(@FisrtName)" +
						   "   AND UPPER(LastName) like UPPER(@LastName)" +
						   " ORDER BY LastName";
				}
			}
		}
	}
}
