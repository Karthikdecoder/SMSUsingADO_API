using sms.Models;
using sms.Repository.IRepository;
using System.Data.SqlClient;
using System.Data;

namespace sms.Repository
{
    public class ClassMasterRepository : IClassMasterRepository
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        public ClassMasterRepository(IConfiguration config)
        {
            configuration = config;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<ClassMasterDTO>> GetAll()
        {
            List<ClassMasterDTO> classList = new List<ClassMasterDTO>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spClassMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetClasses";

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ClassMasterDTO classMaster = new();

                        classMaster.ClassId = dr.GetValue("ClassId").ToString();
                        classMaster.ClassName = dr.GetValue("ClassName").ToString();
                        classMaster.CreatedBy = dr.GetValue("Username").ToString();
                        classMaster.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        classMaster.UpdatedBy = dr.GetValue("Username").ToString();
                        classMaster.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                        classList.Add(classMaster);
                    }
                }
                connection.Close();
            }

            return classList;
        }

        public async Task<ClassMasterDTO> Get(int classId)
        {
            ClassMasterDTO classMaster = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spClassMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetClass";
                command.Parameters.AddWithValue("@classId", SqlDbType.Int).Value = classId;

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        classMaster = new ClassMasterDTO();

                        classMaster.ClassId = dr.GetValue("ClassId").ToString();
                        classMaster.ClassName = dr.GetValue("ClassName").ToString();
                        classMaster.CreatedBy = dr.GetValue("Username").ToString();
                        classMaster.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        classMaster.UpdatedBy = dr.GetValue("Username").ToString();
                        classMaster.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                    }
                }
                connection.Close();
            }
            return classMaster;
        }

        public async Task<ClassMasterCUDTO> Create(ClassMasterCUDTO classMaster, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spClassMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "CreateClass";

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClassName", SqlDbType.NVarChar).Value = classMaster.ClassName;
                command.Parameters.AddWithValue("@CreatedBy", SqlDbType.NVarChar).Value = userName;
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = userName;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

            return classMaster;
        }

        public bool CreateCheck(ClassMasterCUDTO classMaster)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM ClassMaster WHERE ClassName = @ClassName";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ClassName", classMaster.ClassName);

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

        public async Task Remove(int classId, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spClassMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "RemoveClass";

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClassId", SqlDbType.Int).Value = classId;

                connection.Open();

                int n = command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public bool RemoveCheck(int classId)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM ClassMaster WHERE ClassId = @ClassId AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ClassId", classId);

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

        public async Task Update(int classId, ClassMasterCUDTO classMaster, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("spClassMaster", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "UpdateClass";
                command.Parameters.AddWithValue("@ClassId", SqlDbType.Int).Value = classId;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClassName", SqlDbType.NVarChar).Value = classMaster.ClassName;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = userName;

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public bool UpdateCheck(int classId, string className)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM ClassMaster WHERE ClassId = @ClassId AND ClassName = @ClassName AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ClassId", classId);
                    command.Parameters.AddWithValue("@ClassName", className);

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
