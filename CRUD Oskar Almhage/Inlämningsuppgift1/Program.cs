using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Inlämningsuppgift1
{

    internal class Program
    {
        public static string connectionString = @"Data Source= Initial Catalog= "
        + "Integrated Security=true;TrustServerCertificate=true;";

        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("1. Add");
                Console.WriteLine("2. Delete");
                Console.WriteLine("3. Update");
                Console.WriteLine("4. Show ordervalue");
                Console.WriteLine("5. Add customer with order");
                Console.WriteLine("6. Exit");
                Console.WriteLine();
                int option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        Console.Clear();
                        ManageData.AddCustomer(connectionString);
                        Console.Clear();
                        Console.WriteLine("Customer has been added");
                        break;

                    case 2:
                        bool deleteMenuRunning = true;
                        while (deleteMenuRunning)
                        {
                            Console.Clear();
                            Console.WriteLine("1. Delete by id");
                            Console.WriteLine("2. Delete by name");
                            int deleteOption = int.Parse(Console.ReadLine());

                            switch (deleteOption)
                            {
                                case 1:
                                    Console.Write("Enter id:");
                                    string customerID = Console.ReadLine();
                                    ManageData.DeleteByID(connectionString, customerID);
                                    Console.Clear();
                                    Console.WriteLine("Data has been deleted");
                                    deleteMenuRunning = false;
                                    break;

                                case 2:
                                    Console.Write("Enter name:");
                                    string companyName = (Console.ReadLine());
                                    ManageData.DeleteByName(connectionString, companyName);
                                    Console.Clear();
                                    Console.WriteLine("Data has been deleted");
                                    deleteMenuRunning = false;                                   
                                    break;
                            }
                        }
                        break;

                    case 3:
                        Console.Clear();
                        ManageData.UpdateData(connectionString);
                        Console.Clear();
                        Console.WriteLine("Data has been updated");
                        break;

                    case 4:
                        Console.Clear();
                        ManageData.ShowCountrySales(connectionString);                     
                        break;

                    case 5:
                        Console.Clear();
                        ManageData.AddCustomerWithOrder(connectionString);
                        Console.Clear();
                        Console.WriteLine("Customer and order with product have been added");
                        break;

                    case 6:
                        Console.Clear();
                        Console.WriteLine("Bye!");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
            

        }

    }
}