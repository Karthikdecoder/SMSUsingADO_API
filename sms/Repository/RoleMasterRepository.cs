using sms.Models;
using sms.Repository.IRepository;
using System.Data.SqlClient;
using System.Data;

namespace sms.Repository
{
    public class RoleMasterRepository : IRoleMasterRepository
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        public RoleMasterRepository(IConfiguration config)
        {
            configuration = config;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<RoleMasterDTO>> GetAll()
        {
            List<RoleMasterDTO> roleList = new List<RoleMasterDTO>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spRoleMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetRoles";

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RoleMasterDTO roleMaster = new();

                        roleMaster.RoleId = dr.GetValue("RoleId").ToString();
                        roleMaster.RoleName = dr.GetValue("RoleName").ToString();
                        roleMaster.CreatedBy = dr.GetValue("Username").ToString();
                        roleMaster.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        roleMaster.UpdatedBy = dr.GetValue("Username").ToString();
                        roleMaster.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                        roleList.Add(roleMaster);
                    }
                }
                connection.Close();
            }

            return roleList;
        }

        public async Task<RoleMasterDTO> Get(int roleId)
        {
            RoleMasterDTO roleMaster = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spRoleMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetRole";
                command.Parameters.AddWithValue("@RoleId", SqlDbType.Int).Value = roleId;

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        roleMaster = new RoleMasterDTO();

                        roleMaster.RoleId = dr.GetValue("RoleId").ToString();
                        roleMaster.RoleName = dr.GetValue("RoleName").ToString();
                        roleMaster.CreatedBy = dr.GetValue("Username").ToString();
                        roleMaster.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        roleMaster.UpdatedBy = dr.GetValue("Username").ToString();
                        roleMaster.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                    }
                }
                connection.Close();
            }
            return roleMaster;
        }

        public async Task<RoleMasterCUDTO> Create(RoleMasterCUDTO roleMaster, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spRoleMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "CreateRole";

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@RoleName", SqlDbType.NVarChar).Value = roleMaster.RoleName;
                command.Parameters.AddWithValue("@CreatedBy", SqlDbType.NVarChar).Value = userName;
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = userName;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

            return roleMaster;
        }

        public bool CreateCheck(RoleMasterCUDTO roleMaster)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM RoleMaster WHERE RoleName = @RoleName";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RoleName", roleMaster.RoleName);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }

        public async Task Remove(int roleId, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spRoleMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "RemoveRole";

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@RoleId", SqlDbType.Int).Value = roleId;

                connection.Open();

                int n = command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public bool RemoveCheck(int roleId)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM RoleMaster WHERE RoleId = @RoleId AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RoleId", roleId);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }

        public async Task Update(int roleId, RoleMasterCUDTO roleMaster, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spRoleMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "UpdateRole";
                command.Parameters.AddWithValue("@RoleId", SqlDbType.Int).Value = roleId;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@RoleName", SqlDbType.NVarChar).Value = roleMaster.RoleName;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = userName;

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public bool UpdateCheck(int roleId)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM RoleMaster WHERE RoleId = @RoleId AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RoleId", roleId);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }
    }
}
