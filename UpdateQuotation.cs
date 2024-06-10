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
    public partial class UpdateQuotation : Form
    {
        public UpdateQuotation()
        {
            InitializeComponent();
        }

        private void UpdateQuotation_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form form = new Form1();
            this.Hide();
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve input values from the user
            int quotationID;
            if (!int.TryParse(textBoxQuotationID.Text, out quotationID))
            {
                MessageBox.Show("Please enter a valid Quotation ID.");
                return;
            }

            bool pickup = checkBoxPickup.Checked;
            bool installation = checkBoxInstallation.Checked;

            // Perform modification in the database
            ModifyQuotation(quotationID, pickup, installation);
        }


        private void ModifyQuotation(int quotationID, bool pickup, bool installation)
        {
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";
            string query = "UPDATE Quotations SET Pickup = @Pickup, Installation = @Installation WHERE QuotationID = @QuotationID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Pickup", pickup);
                command.Parameters.AddWithValue("@Installation", installation);
                command.Parameters.AddWithValue("@QuotationID", quotationID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Quotation modified successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to modify quotation. Please check the provided Quotation ID.");
                }
            }
        }


    }
}
