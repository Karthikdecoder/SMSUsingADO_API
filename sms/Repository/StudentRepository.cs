using sms.Repository.IRepository;
using System.Data.SqlClient;
using System.Data;
using sms.Models;
using System.Security.Claims;
using sms.Models.Dto;

namespace sms.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        public StudentRepository(IConfiguration config)
        {
            configuration = config;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<StudentGetDTO>> GetAll()
        {
            List<StudentGetDTO> studentList = new List<StudentGetDTO>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentClassMap", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetStudents";

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        StudentGetDTO student = new();

                        student.StudentId = dr.GetValue("StudentId").ToString();
                        student.Name = dr.GetValue("Name").ToString();
                        student.RollNo = dr.GetValue("RollNo").ToString();
                        student.ClassName = dr.GetValue("ClassName").ToString();
                        student.DateOfBirth = dr.GetValue("DateOfBirth").ToString();
                        student.Email = dr.GetValue("Email").ToString();
                        student.ContactNo = dr.GetValue("ContactNo").ToString();
                        student.CreatedBy = dr.GetValue("Username").ToString(); // change done here for Createdby
                        student.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        student.UpdatedBy = dr.GetValue("Username").ToString();
                        student.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                        studentList.Add(student);
                    }
                }
                connection.Close();
            }

            return studentList;
        }

        public async Task<StudentGetDTO> Get(int StudentId)
        {
            StudentGetDTO student = null; // change

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentClassMap", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetStudent";
                command.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = StudentId;

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        student = new StudentGetDTO();

                        student.StudentId = dr.GetValue("StudentId").ToString();
                        student.Name = dr.GetValue("Name").ToString();
                        student.RollNo = dr.GetValue("RollNo").ToString();
                        student.ClassName = dr.GetValue("ClassName").ToString();
                        student.DateOfBirth = dr.GetValue("DateOfBirth").ToString();
                        student.Email = dr.GetValue("Email").ToString();
                        student.ContactNo = dr.GetValue("ContactNo").ToString();
                        student.CreatedBy = dr.GetValue("Username").ToString();
                        student.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        student.UpdatedBy = dr.GetValue("Username").ToString();
                        student.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                    }
                }
                connection.Close();
            }
            return student;
        }

        public async Task<Student> Create(Student student, string userName)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentClassMap", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "CreateStudent";

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = student.Name;
                command.Parameters.AddWithValue("@RollNo", SqlDbType.Int).Value = student.RollNo;
                command.Parameters.AddWithValue("@ClassId", SqlDbType.Int).Value = student.ClassId;
                command.Parameters.AddWithValue("@DateOfBirth", SqlDbType.NVarChar).Value = student.DateOfBirth;
                command.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = student.Email;
                command.Parameters.AddWithValue("@ContactNo", SqlDbType.NVarChar).Value = student.ContactNo;
                command.Parameters.AddWithValue("@CreatedBy", SqlDbType.NVarChar).Value = userName;
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = userName;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

            return student;
        }

        public async Task Remove(int StudentId, string userName)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentClassMap", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "RemoveStudent";

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = StudentId;

                connection.Open();

                int n = command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public async Task Update(int StudentId, StudentDTO student, string userName)
        {

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentClassMap", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "UpdateStudent";
                command.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = StudentId;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = student.Name;
                command.Parameters.AddWithValue("@RollNo", SqlDbType.Int).Value = student.RollNo;
                command.Parameters.AddWithValue("@DateOfBirth", SqlDbType.NVarChar).Value = student.DateOfBirth;
                command.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = student.Email;
                command.Parameters.AddWithValue("@ClassId", SqlDbType.NVarChar).Value = student.ClassId;
                command.Parameters.AddWithValue("@ContactNo", SqlDbType.NVarChar).Value = student.ContactNo;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = userName;

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public bool CreateCheck(Student student)
        {
            bool flag = false;

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM Student2 WHERE RollNo = @RollNo";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RollNo", student.RollNo);

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

        public bool UpdateCheck(int StudentId)
        {
            bool flag = false;

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM Student2 WHERE StudentId = @StudentId AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@StudentId", StudentId);

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

        public bool RemoveCheck(int StudentId)
        {
            bool flag = false;

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM Student2 WHERE StudentId = @StudentId AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@StudentId", StudentId);

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

        public bool RollNoCheck(int RollNo, int StudentId)
        {
            bool flag = false;

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM Student2 WHERE RollNo = @RollNo AND StudentId <> @StudentId";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RollNo", RollNo);
                    command.Parameters.AddWithValue("@StudentId", StudentId);

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
