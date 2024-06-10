using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garden_Centre
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static void ImportCustomers(SqlConnection connection)
        {
            using (StreamReader reader = new StreamReader("C:\\Users\\tahir\\Desktop\\Garden_Centre\\Garden_Centre\\klanten.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('|');
                    var command = new SqlCommand(
                        "INSERT INTO Customers (CustomerID, Name, Address) VALUES (@CustomerID, @Name, @Address)", connection);
                    command.Parameters.AddWithValue("@CustomerID", int.Parse(parts[0]));
                    command.Parameters.AddWithValue("@Name", parts[1]);
                    command.Parameters.AddWithValue("@Address", parts[2]);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2601 || ex.Number == 2627)
                        {
                            Console.WriteLine("Duplicate insertion detected. Skipping entry...");
                        }
                        else
                        {
                            Console.WriteLine($"SQL Error: {ex.Message}");
                        }
                        return;
                    }
                }
            }
        }

        static void ImportProducts(SqlConnection connection)
        {
            var lines = File.ReadAllLines("C:\\Users\\tahir\\Desktop\\Garden_Centre\\Garden_Centre\\producten.txt");
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                var command = new SqlCommand(
                    "INSERT INTO Products (ProductID, DutchName, ScientificName, Description, Price) VALUES (@ProductID, @DutchName, @ScientificName, @Description, @Price)", connection);
                command.Parameters.AddWithValue("@ProductID", int.Parse(parts[0]));
                command.Parameters.AddWithValue("@DutchName", parts[1]);
                command.Parameters.AddWithValue("@ScientificName", parts[2]);
                command.Parameters.AddWithValue("@Description", parts[4]);
                command.Parameters.AddWithValue("@Price", decimal.Parse(parts[3], CultureInfo.InvariantCulture));
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        Console.WriteLine("Duplicate insertion detected. Skipping entry...");
                    }
                    else
                    {
                        Console.WriteLine($"SQL Error: {ex.Message}");
                    }
                    return;
                }

            }
        }

        static bool ImportQuotations(SqlConnection connection)
        {
            bool flg = true;
            var lines = File.ReadAllLines("C:\\Users\\tahir\\Desktop\\Garden_Centre\\Garden_Centre\\offertes.txt");
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                var command = new SqlCommand(
                    "INSERT INTO Quotations (QuotationID, Date, CustomerID, Pickup, Installation) VALUES (@QuotationID, @Date, @CustomerID, @Pickup, @Installation)", connection);
                command.Parameters.AddWithValue("@QuotationID", int.Parse(parts[0]));
                command.Parameters.AddWithValue("@Date", DateTime.Parse(parts[1]));
                command.Parameters.AddWithValue("@CustomerID", int.Parse(parts[2]));
                command.Parameters.AddWithValue("@Pickup", bool.Parse(parts[3]));
                command.Parameters.AddWithValue("@Installation", bool.Parse(parts[4]));
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        Console.WriteLine("Duplicate insertion detected. Skipping entry...");
                    }
                    else
                    {
                        Console.WriteLine($"SQL Error: {ex.Message}");
                    }
                    flg = false;
                    return flg;
                }
            }
            return flg;
        }

        static void ImportQuotationProducts(SqlConnection connection)
        {
            var lines = File.ReadAllLines("C:\\Users\\tahir\\Desktop\\Garden_Centre\\Garden_Centre\\offerte_producten.txt");
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                var command = new SqlCommand(
                    "INSERT INTO QuotationProducts (QuotationID, ProductID, Quantity) VALUES (@QuotationID, @ProductID, @Quantity)", connection);
                command.Parameters.AddWithValue("@QuotationID", int.Parse(parts[0]));
                command.Parameters.AddWithValue("@ProductID", int.Parse(parts[1]));
                command.Parameters.AddWithValue("@Quantity", int.Parse(parts[2]));
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        Console.WriteLine("Duplicate insertion detected. Skipping entry...");
                    }
                    else
                    {
                        Console.WriteLine($"SQL Error: {ex.Message}");
                    }
                    return;

                }
            }
        }
        static void CalculateQuotationCosts(SqlConnection connection)
        {
            var command = new SqlCommand(
               "SELECT q.QuotationID, q.Installation, q.Pickup, " +
               "SUM(p.Price * qp.Quantity) AS ProductTotal " +
               "FROM Quotations q " +
               "JOIN QuotationProducts qp ON q.QuotationID = qp.QuotationID " +
               "JOIN Products p ON qp.ProductID = p.ProductID " +
               "GROUP BY q.QuotationID, q.Installation, q.Pickup", connection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int quotationID = reader.GetInt32(0);
                    bool installation = reader.GetBoolean(1);
                    bool pickup = reader.GetBoolean(2);
                    decimal productTotal = reader.GetDecimal(3);
                    decimal discount = 0;
                    decimal deliveryCost = 0;
                    decimal installationCost = 0;

                    // Apply discounts
                    if (productTotal > 5000)
                        discount = 0.10m;
                    else if (productTotal > 2000)
                        discount = 0.05m;

                    decimal discountedTotal = productTotal * (1 - discount);

                    // Calculate delivery cost
                    if (!pickup)
                    {
                        if (discountedTotal < 500)
                            deliveryCost = 100;
                        else if (discountedTotal < 1000)
                            deliveryCost = 50;
                        else
                            deliveryCost = 0;
                    }

                    // Calculate installation cost
                    if (installation)
                    {
                        if (productTotal > 5000)
                            installationCost = productTotal * 0.05m;
                        else if (productTotal > 2000)
                            installationCost = productTotal * 0.10m;
                        else
                            installationCost = productTotal * 0.15m;
                    }

                    decimal totalCost = discountedTotal + deliveryCost + installationCost;

                    // Update quotation with calculated total cost
                    using (var updateCommand = new SqlCommand(
                        "UPDATE Quotations SET TotalCost = @TotalCost WHERE QuotationID = @QuotationID", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@TotalCost", totalCost);
                        updateCommand.Parameters.AddWithValue("@QuotationID", quotationID);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {
          

         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                ImportCustomers(connection);
                ImportProducts(connection);
                bool excep = ImportQuotations(connection);
                ImportQuotationProducts(connection);
                if (excep)
                {
                    Console.WriteLine("Data Entered");
                    CalculateQuotationCosts(connection);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form form = new Retrieve__Quotation();
            this.Hide();
            form.ShowDialog();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Form form = new Display();
            this.Hide();
            form.ShowDialog();
        }

        private void create_Click(object sender, EventArgs e)
        {
            Form form = new CreateQuotation();
            this.Hide();
            form.ShowDialog();
        }

        private void Update_Click(object sender, EventArgs e)
        {
            Form form = new UpdateQuotation();
            this.Hide();
            form.ShowDialog();
        }
    }
}
