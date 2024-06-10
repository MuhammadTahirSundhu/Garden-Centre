using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garden_Centre
{
    public partial class CreateQuotation : Form
    {
        public CreateQuotation()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateComboBoxWithCustomers();
            PopulateComboBoxWithProducts();
        }

        private void PopulateComboBoxWithProducts()
        {
            DataTable productsTable = GetProductsFromDataSource();

            comboBox2.Items.Clear(); // Clear existing items

            foreach (DataRow row in productsTable.Rows)
            {
                string productName = row["DutchName"].ToString();
                string productID = row["ProductID"].ToString();
                comboBox2.Items.Add(new ComboBoxItem { Text = productName, Value = productID });
            }
        }


        private void PopulateComboBoxWithCustomers()
        {
            DataTable customersTable = GetCustomersFromDataSource();

            comboBox1.Items.Clear(); // Clear existing items

            foreach (DataRow row in customersTable.Rows)
            {
                string customerName = row["Name"].ToString();
                string customerID = row["CustomerID"].ToString();
                comboBox1.Items.Add(new ComboBoxItem { Text = customerName, Value = customerID });
            }
        }


        public DataTable GetCustomersFromDataSource()
        {
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";

            DataTable customersTable = new DataTable();

            // SQL query to select customer data from the database
            string query = "SELECT CustomerID, Name, Address FROM Customers";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                // Fill the DataTable with data retrieved from the database
                adapter.Fill(customersTable);
            }

            return customersTable;
        }
        public DataTable GetProductsFromDataSource()
        {
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";

            DataTable productsTable = new DataTable();

            // SQL query to select product data from the database
            string query = "SELECT ProductID, DutchName FROM Products";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                // Fill the DataTable with data retrieved from the database
                adapter.Fill(productsTable);
            }

            return productsTable;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                string selectedItem = comboBox1.SelectedItem.ToString();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                string selectedItem = comboBox2.SelectedItem.ToString();

            }
        }




        private void CreateQuotation_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 || string.IsNullOrEmpty(textBoxQuantity.Text))
            {
                MessageBox.Show("Please select a customer, product, and enter a quantity.");
                return;
            }

            int customerID = Convert.ToInt32(((ComboBoxItem)comboBox1.SelectedItem).Value);
            int productID = Convert.ToInt32(((ComboBoxItem)comboBox2.SelectedItem).Value);
            int quantity = int.Parse(textBoxQuantity.Text);
            bool delivery = radioButtonDelivery.Checked;
            bool installation = listBoxInstallation.Checked;
            Console.Write(installation);
            decimal productPrice = GetProductPrice(productID);
            decimal totalCost = productPrice * quantity;
            totalCost = ApplyDiscounts(totalCost);
            totalCost = ApplyDeliveryCharges(totalCost, delivery);
            totalCost = ApplyInstallationCharges(totalCost, installation);

            InsertQuotation(customerID, totalCost, delivery, installation);
        }


        private decimal GetProductPrice(int productID)
        {
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";
            decimal price = 0;
            string query = "SELECT Price FROM Products WHERE ProductID = @ProductID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);
                connection.Open();
                price = (decimal)command.ExecuteScalar();
                connection.Close();
            }

            return price;
        }

        private decimal ApplyDiscounts(decimal totalCost)
        {
            if (totalCost > 5000)
            {
                totalCost *= 0.90m;
            }
            else if (totalCost > 2000)
            {
                totalCost *= 0.95m;
            }
            return totalCost;
        }

        private decimal ApplyDeliveryCharges(decimal totalCost, bool delivery)
        {
            if (delivery)
            {
                if (totalCost < 500)
                {
                    totalCost += 100;
                }
                else if (totalCost < 1000)
                {
                    totalCost += 50;
                }
            }
            return totalCost;
        }

        private decimal ApplyInstallationCharges(decimal totalCost, bool installation)
        {
            if (installation)
            {
                if (totalCost > 5000)
                {
                    totalCost *= 1.05m;
                }
                else if (totalCost > 2000)
                {
                    totalCost *= 1.10m;
                }
                else
                {
                    totalCost *= 1.15m;
                }
            }
            return totalCost;
        }

        private void InsertQuotation(int customerID, decimal totalCost, bool delivery, bool installation)
        {
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";
            string query = "INSERT INTO Quotations (QuotationID, CustomerID, TotalCost, Pickup, Installation, Date) VALUES (@QuotationID, @CustomerID, @TotalCost, @Pickup, @Installation, @Date)";

            int quotationID;
            DateTime currentDate = DateTime.Now; // Get the current date

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Generate QuotationID
                SqlCommand getIdCommand = new SqlCommand("SELECT ISNULL(MAX(QuotationID), 0) + 1 FROM Quotations", connection);
                quotationID = (int)getIdCommand.ExecuteScalar();

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@QuotationID", quotationID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@TotalCost", totalCost);
                command.Parameters.AddWithValue("@Pickup", delivery);
                command.Parameters.AddWithValue("@Installation", installation);
                command.Parameters.AddWithValue("@Date", currentDate); // Add current date parameter

                command.ExecuteNonQuery();
            }

            MessageBox.Show("Quotation created successfully with ID: " + quotationID);
        }

        private void listBoxInstallation_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Back_Click(object sender, EventArgs e)
        {
            Form form = new Form1();
            this.Hide();
            form.ShowDialog();
        }
    }
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
