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
    public partial class Retrieve__Quotation : Form
    {
        public Retrieve__Quotation()
        {
            InitializeComponent();
        }

        private void Name_Click(object sender, EventArgs e)
        {
            // Determine the search criteria (customer, date, or quotation number)
            string searchCriteria = comboBoxSearchCriteria.SelectedItem.ToString();
            string searchValue = textBox1.Text;

            // Perform the search based on the selected criteria
            DataTable searchResults = SearchQuotations(searchCriteria, searchValue);

            // Display the search results in a DataGridView or any other appropriate control
            dataGridViewSearchResults.DataSource = searchResults;
        }

        private DataTable SearchQuotations(string searchCriteria, string searchValue)
        {
            DataTable searchResults = new DataTable();

            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";
            string query = "";

            // Construct the SQL query based on the selected search criteria
            switch (searchCriteria)
            {
                case "Customer":
                    query = "SELECT * FROM Quotations WHERE CustomerID = @SearchValue";
                    break;
                case "Date":
                    query = "SELECT * FROM Quotations WHERE Date = @SearchValue";
                    break;
                case "Quotation Number":
                    query = "SELECT * FROM Quotations WHERE QuotationID = @SearchValue";
                    break;
                default:
                    MessageBox.Show("Invalid search criteria.");
                    return searchResults;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchValue", searchValue);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(searchResults);
            }

            return searchResults;
        }

        private void Back_Click(object sender, EventArgs e)
        {
            Form form = new Form1();
            this.Hide();
            form.ShowDialog();
        }
    }
}
