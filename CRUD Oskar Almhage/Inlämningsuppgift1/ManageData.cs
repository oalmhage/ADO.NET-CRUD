using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämningsuppgift1
{
    public class ManageData
    {
        public static (string customerID, string companyName) GetIDAndName()
        {
            Console.WriteLine("Add new customer");

            Console.Write("Enter ID: ");
            string customerID = Console.ReadLine();

            Console.Write("Enter company name: ");
            string companyName = Console.ReadLine();

            return  (customerID, companyName);
        }
        public static string AddCustomer(string connectionString)
        {
            (string customerID, string companyName) = GetIDAndName();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    "INSERT INTO Customers (CustomerID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax)" +
                    "VALUES (@CustomerID, @CompanyName, @ContactName, @ContactTitle, @Address, @City, @Region, @PostalCode, @Country, @Phone, @Fax)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerID);
                    command.Parameters.AddWithValue("@CompanyName", companyName);
                    command.Parameters.AddWithValue("@ContactName", "Anders Andersson");
                    command.Parameters.AddWithValue("@ContactTitle", "Sales");
                    command.Parameters.AddWithValue("@Address", "Drottninggatan 45");
                    command.Parameters.AddWithValue("@City", "Stockholm");
                    command.Parameters.AddWithValue("@Region", "Stockholms län");
                    command.Parameters.AddWithValue("@PostalCode", "103 16");
                    command.Parameters.AddWithValue("@Country", "Sweden");
                    command.Parameters.AddWithValue("@Phone", "0701234567");
                    command.Parameters.AddWithValue("@Fax", "123456");
                    command.ExecuteNonQuery();
                    return customerID;
                }

            }
        }
        public static void DeleteByID(string connectionString, string customerID)

        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteOrderDetailsQuery = "DELETE FROM [Order Details] WHERE OrderID IN (SELECT OrderID FROM Orders WHERE CustomerID = @CustomerId)";
                using (SqlCommand deleteOrderDetailsCommand = new SqlCommand(deleteOrderDetailsQuery, connection))
                {
                    deleteOrderDetailsCommand.Parameters.AddWithValue("@CustomerId", customerID);
                    deleteOrderDetailsCommand.ExecuteNonQuery();
                }
                string deleteOrdersQuery = "DELETE FROM Orders WHERE CustomerID = @CustomerId";
                using (SqlCommand deleteOrdersCommand = new SqlCommand(deleteOrdersQuery, connection))
                {
                    deleteOrdersCommand.Parameters.AddWithValue("@CustomerId", customerID);
                    deleteOrdersCommand.ExecuteNonQuery();
                }

                string deleteCustomerQuery = "DELETE FROM Customers WHERE CustomerID = @CustomerID";
                using (SqlCommand deleteCustomerCommand = new SqlCommand(deleteCustomerQuery, connection))
                {
                    deleteCustomerCommand.Parameters.AddWithValue("@CustomerID", customerID);
                    deleteCustomerCommand.ExecuteNonQuery();
                }
            }
        }
        public static void DeleteByName(string connectionString, string companyName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteDetailQuery = "DELETE FROM [Order Details] " +
                "WHERE OrderID IN " +
                "(SELECT OrderID FROM Orders " +
                "WHERE CustomerID = " +
                "(SELECT CustomerID FROM Customers WHERE CompanyName = @companyName))";

                using (SqlCommand deleteDetailCommand = new SqlCommand(deleteDetailQuery, connection))
                {
                    deleteDetailCommand.Parameters.AddWithValue("@CompanyName", companyName);
                    deleteDetailCommand.ExecuteNonQuery();
                }

                string deleteOrderQuery = "DELETE FROM Orders " +
                "WHERE CustomerID = " +
                "(SELECT CustomerID " +
                "FROM Customers " +
                "WHERE CompanyName = @companyName)";

                using (SqlCommand deleteOrderCommand = new SqlCommand(deleteOrderQuery, connection))
                {
                    deleteOrderCommand.Parameters.AddWithValue("@CompanyName", companyName);
                    deleteOrderCommand.ExecuteNonQuery();
                }

                string deleteCustomer = "DELETE FROM Customers WHERE CompanyName = @companyName";

                using (SqlCommand command = new SqlCommand(deleteCustomer, connection))
                {
                    command.Parameters.AddWithValue("@companyName", companyName);
                    command.ExecuteNonQuery();
                }
            }
            

        }
        public static void UpdateData(string connectionString)
        {
            Console.WriteLine("Enter the customerID you want to change the address for: ");
            string customerID = Console.ReadLine();

            Console.Write("Enter new address: ");
            string newAddress = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Customers SET Address = @newAddress WHERE CustomerID = @customerID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@newAddress", newAddress);
                    command.Parameters.AddWithValue("@CustomerID", customerID);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void ShowCountrySales(string connectionString)
        {
            Console.Write("Enter country: ");
            string country = Console.ReadLine();
            Console.WriteLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT E.FirstName + ' ' + E.LastName AS SalesPerson,
                            SUM(OD.Quantity * OD.UnitPrice) AS TotalSales
                            FROM Orders O
                            JOIN Employees E ON O.EmployeeID = E.EmployeeID
                            JOIN Customers C ON O.CustomerID = C.CustomerID
                            JOIN [Order Details] OD ON O.OrderID = OD.OrderID
                            WHERE C.Country = @Country
                            GROUP BY E.FirstName, E.LastName
                            ORDER BY TotalSales DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Country", country);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine($"{"SALESPERSON",-16} {"TOTALSALES",15}");
                        Console.WriteLine(new string('-', 32));
                        while (reader.Read())
                        {
                            string salesPerson = reader["SALESPERSON"].ToString();
                            decimal totalSales = (decimal)reader["TOTALSALES"];

                            Console.WriteLine($"{salesPerson,-16} {totalSales,15:C}");
                        }
                    }
                }
            }
        }
        public static void AddCustomerWithOrder(string connectionString)
        {
            string customerID = AddCustomer(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string addOrderQuery = "INSERT INTO Orders (CustomerID, OrderDate) " +
                    "VALUES (@CustomerID, @OrderDate); SELECT SCOPE_IDENTITY();";

                using (SqlCommand addOrderCommand = new SqlCommand(addOrderQuery, connection))
                {
                    addOrderCommand.Parameters.AddWithValue("@CustomerID", customerID);
                    addOrderCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);

                    int orderID = Convert.ToInt32(addOrderCommand.ExecuteScalar());
                    int productID = 40;
                    int quantity = 5;

                    string addOrderDetailQuery = "INSERT INTO [Order Details] (OrderID, ProductID, Quantity) " +
                        "VALUES (@OrderID, @ProductID, @Quantity)";
                    using (SqlCommand addOrderDetailCommand = new SqlCommand(addOrderDetailQuery, connection))
                    {
                        addOrderDetailCommand.Parameters.AddWithValue("@OrderID", orderID);
                        addOrderDetailCommand.Parameters.AddWithValue("@ProductID", productID);
                        addOrderDetailCommand.Parameters.AddWithValue("@Quantity", quantity);
                        addOrderDetailCommand.ExecuteNonQuery();
                    }
                } 
            }
        }
    }
}
