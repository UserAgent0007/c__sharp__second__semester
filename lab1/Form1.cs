using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace lab1
{
    public partial class Form1 : Form
    {
        Database db = new Database();
        public Form1()
        {
            InitializeComponent();
        }

        public class Database
        {
            private readonly string connectionString;

            public Database()
            {
                // Додано TrustServerCertificate=True для коректної роботи з Microsoft.Data.SqlClient на LocalDB
                connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectContracts;Integrated Security=True";
            }

            public DataTable GetData(string query)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = """
                SELECT
                    e.LastName + ' ' + e.FirstName + ' ' + e.MiddleName AS Employee,
                    p.PositionName,
                    p.Salary,
                    e.Address,
                    e.Characteristic,
                    pr.ProjectName,
                    pr.Description AS ProjectDescription,
                    ep.StartDate,
                    ep.EndDate,
                    DATEDIFF(DAY, ep.StartDate, ep.EndDate) + 1 AS Days
                FROM EmployeeProjects ep
                INNER JOIN Employees e
                    ON ep.EmployeeId = e.EmployeeId
                INNER JOIN Positions p
                    ON e.PositionId = p.PositionId
                INNER JOIN Projects pr
                    ON ep.ProjectId = pr.ProjectId
                ORDER BY e.EmployeeId;
             """;
            try
            {
                dataGridView1.DataSource = db.GetData(sql);
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка підключення: " + ex.Message);
            }
        }
    }
}
