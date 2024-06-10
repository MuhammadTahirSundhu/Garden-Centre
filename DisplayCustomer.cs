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
    public partial class Display : Form
    {
        public Display()
        {
            InitializeComponent();
        }

        private void DisplayCustomer_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";

            // Determine if the search term is a numeric value (customer ID) or a string (customer name)
            bool isNumeric = int.TryParse(searchTerm, out _);

            // Construct the SQL query to join Customers and Quotations tables
            string query = $@"
            SELECT 
                c.CustomerID, 
                c.Name AS CustomerName, 
                c.Address, 
                q.QuotationID, 
                q.TotalCost 
            FROM 
                Customers c
            LEFT JOIN 
                Quotations q ON c.CustomerID = q.CustomerID
            WHERE 
             {(isNumeric ? "c.CustomerID" : "c.Name")} = @SearchTerm
";

            DataTable customerQuotationTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchTerm", searchTerm);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(customerQuotationTable);
            }

            customersDataGridView.DataSource = customerQuotationTable;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void customersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Back_Click(object sender, EventArgs e)
        {
            Form form = new Form1();
            this.Hide();
            form.ShowDialog();
        }
    }
}

