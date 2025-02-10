using Microsoft.Data.SqlClient;
namespace NetCoreLinqToSqlInjection.Repositories

{
    public class RepositoryDoctores
    {
        private class RepositoryDoctoresSQLServer
        {
            private DateTable tableDoctores;
            private SqlConnection cn;
            private SqlCommand com;

            public RepositoryDoctoresSQLServer()
            {
                string connectionString = @"";
                this.cn = new SqlConnection(connectionString);
                this.com = new SqlCommand();
                this.com.Connection = this.cn;
                this.tableDoctores = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter
                    ("select * from DOCTOR", this.cn);
                ad.Fill(this.tableDoctores);
            }
        }
    }
}
