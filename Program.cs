using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garden_Centre
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Globalization;

    class Program
    {
        static void Main()
        {
            string connectionString = "Data Source=TS_SUNDHU\\SQLEXPRESS;Initial Catalog=Garden_Centre;Integrated Security=True;MultipleActiveResultSets=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
        }

    }
