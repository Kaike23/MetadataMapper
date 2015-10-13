using Model.Customer;
using Model.Order;
using Repository.Repositories;
using Repository.UnitOfWork;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Thread.CurrentThread.Name = "TEST";

			var uow = new UnitOfWork();
			var customerRepository = new CustomerRepository(uow);
			var orderRepository = new OrderRepository(uow);

			Console.WriteLine("SIMPLE CRUD TEST APP");
			//FindAll TEST
			Console.WriteLine("FINDALL TEST");
			Console.WriteLine("List of Customers:");
			var customers = customerRepository.FindAll().ToList<Customer>();
			for (var index = 0; index < customers.Count; index++)
				Console.WriteLine(string.Format("{0}\t{1}", index + 1, customers[index].FullName));
			PressTo("CONTINUE");

			//Update TEST
			Console.WriteLine("UPDATE TEST");
			var customerEntity = customers[0];
			Console.WriteLine(string.Format("Customer current FirstName: {0}", customerEntity.FirstName));
			Console.WriteLine("ENTER Customer NEW FirstName: ");
			var newName = Console.ReadLine();
			var oldName = customerEntity.FirstName;
			customerEntity.FirstName = newName;
			Console.WriteLine("Update data? (Y/N): ");

			var keyPressed = Console.ReadKey(true).Key;
			while (keyPressed != ConsoleKey.Y && keyPressed != ConsoleKey.N) { keyPressed = Console.ReadKey(true).Key; }
			if (keyPressed == ConsoleKey.Y)
			{
				customerRepository.Update(customerEntity);
				uow.Commit();
				Console.WriteLine("Data updated!");
			}
			else
			{
				customerEntity.FirstName = oldName;
				Console.WriteLine("Change reverted!");
			}

			Console.WriteLine(string.Format("Customer Full Name: {0}", customerEntity.FullName));
			PressTo("CONTINUE");

			//Insert TEST
			Console.WriteLine("INSERT TEST");

			Console.WriteLine("ENTER NEW Customer FirstName: ");
			var newFirstName = Console.ReadLine();
			Console.WriteLine("ENTER NEW Customer LastName: ");
			var newLastName = Console.ReadLine();
			customerEntity = Customer.Create(newFirstName, newLastName);
			customerRepository.Add(customerEntity);
			uow.Commit();
			Console.WriteLine("NEW Customer CREATED!");
			Console.WriteLine("");
			Console.WriteLine("List of Customers:");
			customers = customerRepository.FindAll().ToList<Customer>();
			for (var index = 0; index < customers.Count; index++)
				Console.WriteLine(string.Format("{0}\t{1}", index + 1, customers[index].FullName));
			PressTo("CONTINUE");

			//Delete TEST
			Console.WriteLine("DELETE TEST");

			Console.WriteLine("ENTER Customer's number to DELETE: ");
			int deleteIndex;
			while (!int.TryParse(Console.ReadLine(), out deleteIndex) || (deleteIndex < 0 || deleteIndex > customers.Count))
			{
				Console.WriteLine("ENTER a valid Customer's number to DELETE: ");
			}
			customerEntity = customers[deleteIndex - 1];
			customerRepository.Remove(customerEntity);
			uow.Commit();
			Console.WriteLine(string.Format("Customer '{0}' was DELETED!", customerEntity.FullName));
			Console.WriteLine("");
			Console.WriteLine("List of Customers:");
			customers = customerRepository.FindAll().ToList<Customer>();
			for (var index = 0; index < customers.Count; index++)
				Console.WriteLine(string.Format("{0}\t{1}", index + 1, customers[index].FullName));
			PressTo("CONTINUE");

			//FindAll TEST [Orders]
			Console.WriteLine("FINDALL TEST [Orders]");
			Console.WriteLine("Enter Customer's number to display the list of Orders:");
			int customerIndex;
			while (!int.TryParse(Console.ReadLine(), out customerIndex) || (customerIndex < 0 || customerIndex > customers.Count))
			{
				Console.WriteLine("ENTER a valid Customer's number: ");
			}
			customerEntity = customers[customerIndex - 1];
			Console.WriteLine(string.Format("{0}'s ORDERS LIST:", customerEntity.FirstName));
			var orders = orderRepository.FindAllBy(customerEntity.Id).ToList<Order>();
			for (var index = 0; index < orders.Count; index++)
				Console.WriteLine(string.Format("{0}\t{1}", index + 1, orders[index].OrderDate));
			PressTo("CONTINUE");
			PressTo("EXIT");

		}

		private static void PressTo(string condition)
		{
			Console.WriteLine(string.Format("PRESS SPACEBAR TO {0}!", condition));
			while (Console.ReadKey(true).Key != ConsoleKey.Spacebar) { }
			Console.WriteLine("");
		}
	}
}
