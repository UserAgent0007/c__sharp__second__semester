using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using lab1.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace lab1
{
    public partial class Form1 : Form
    {
        Database db = new Database();
        ProjectContractsContext dbEFCORE = new ProjectContractsContext(Environment.GetEnvironmentVariable("DATABASEURL"));

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
                    p.Name as PositionName,
                    ep.StartDate,
                    ep.EndDate,
                    pr.Name,
                    p.Salary,
                    e.Address,
                    e.Characteristic,
                    pr.Description AS ProjectDescription,
                    DATEDIFF(DAY, ep.StartDate, ep.EndDate) + 1 AS Days
                FROM EmployeeProjects ep
                INNER JOIN Employees e
                    ON ep.EmployeeId = e.Id
                INNER JOIN Positions p
                    ON e.PositionId = p.Id
                INNER JOIN Projects pr
                    ON ep.ProjectId = pr.Id
                ORDER BY e.Id;
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var rawData = dbEFCORE.Employees
                    .Include(e => e.EmployeeProjects)
                        .ThenInclude(ep => ep.Project)
                    .Where(e => e.EmployeeProjects.Any(ep1 =>
                        e.EmployeeProjects.Any(ep2 =>
                            ep1.ProjectId != ep2.ProjectId &&
                            (ep1.StartDate <= ep2.EndDate) &&
                            (ep2.StartDate <= ep1.EndDate)
                        )))
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        OverlappingProjects = e.EmployeeProjects
                            .Where(ep1 => e.EmployeeProjects.Any(ep2 =>
                                ep1.ProjectId != ep2.ProjectId &&
                                (ep1.StartDate <= ep2.EndDate) &&
                                (ep2.StartDate <= ep1.EndDate)
                            ))
                            .Select(ep => ep.Project.Name)
                            .Distinct()
                    })
                    .ToList();
            

                var gridData = rawData.Select(e => new
                {
                    NameSurname = $"{e.LastName} {e.FirstName}",
                    ProjectsList = string.Join(", ", e.OverlappingProjects)
                }).ToList();

                dataGridView2.DataSource = gridData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка підключення: " + ex.Message);
            }
        }
    }
}
