using sms.Models.Dto;
using sms.Repository.IRepository;
using System.Data.SqlClient;
using System.Data;
using sms.Models;

namespace sms.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IConfiguration configuration;
        public StudentRepository(IConfiguration config)
        {
            configuration = config;
        }

        public async Task<List<Student>> GetAll()
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            List<Student> studentList = new List<Student>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentSP", connection);
                command.Parameters.AddWithValue("@Query", SqlDbType.NVarChar).Value = 4;

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Student student = new Student();

                        student.Name = dr.GetValue("Name").ToString();
                        student.RollNo = dr.GetInt32("RollNo").ToString();
                        student.Class = dr.GetInt32("Class").ToString();
                        student.DateOfBirth = dr.GetValue("DateOfBirth").ToString();
                        student.Email = dr.GetValue("Email").ToString();
                        student.ContactNo = dr.GetValue("ContactNo").ToString();

                        studentList.Add(student);
                    }
                }
                connection.Close();
            }

            return studentList;
        }

        public async Task<Student> Get(int RollNo, int Class)
        {
            Student student = null; // change

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Student WHERE RollNo = '" + RollNo + "' AND Class = '" + Class + "' AND IsDeleted = 0;", connection);
                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        student = new Student();

                        student.Name = dr.GetValue("Name").ToString();
                        student.RollNo = dr.GetInt32("RollNo").ToString(); // roll number
                        student.Class = dr.GetInt32("Class").ToString(); // class
                        student.DateOfBirth = dr.GetValue("DateOfBirth").ToString();
                        student.Email = dr.GetValue("Email").ToString();
                        student.ContactNo = dr.GetValue("ContactNo").ToString();

                    }
                }
                connection.Close();
            }
            return student;
        }

        public async Task<Student> Create(Student student)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentSP", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = student.Name; // check
                command.Parameters.AddWithValue("@RollNo", SqlDbType.Int).Value = student.RollNo;
                command.Parameters.AddWithValue("@Class", SqlDbType.Int).Value = student.Class;
                command.Parameters.AddWithValue("@DateOfBirth", SqlDbType.NVarChar).Value = student.DateOfBirth;
                command.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = student.Email;
                command.Parameters.AddWithValue("@ContactNo", SqlDbType.NVarChar).Value = student.ContactNo;
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@Query", SqlDbType.NVarChar).Value = 1;

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

            return student;
        }

        public async Task Remove(int RollNo, int Class)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentSP", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@RollNo", SqlDbType.Int).Value = RollNo;
                command.Parameters.AddWithValue("@Class", SqlDbType.Int).Value = Class;
                command.Parameters.AddWithValue("@Query", SqlDbType.NVarChar).Value = 3;

                connection.Open();

                int n = command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public async Task Update(int RollNo, int Class, Student student)
        {

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("StudentSP", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = student.Name;
                command.Parameters.AddWithValue("@RollNo", SqlDbType.Int).Value = RollNo;
                command.Parameters.AddWithValue("@Class", SqlDbType.Int).Value = Class;
                command.Parameters.AddWithValue("@DateOfBirth", SqlDbType.NVarChar).Value = student.DateOfBirth;
                command.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = student.Email;
                command.Parameters.AddWithValue("@ContactNo", SqlDbType.NVarChar).Value = student.ContactNo;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@Query", SqlDbType.NVarChar).Value = 2;

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

                string query = "SELECT COUNT(*) FROM Student WHERE RollNo = @RollNo AND Class = @Class";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RollNo", student.RollNo); 
                    command.Parameters.AddWithValue("@Class", student.Class);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return true;
                    }
                }

                con.Close();
            }

            return false;
        }

        public bool UpdateCheck(int RollNo, int Class)
        {
            bool flag = false;

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM Student WHERE RollNo = @RollNo AND Class = @Class AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RollNo", RollNo); // rename id
                    command.Parameters.AddWithValue("@Class", Class);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return true;
                    }
                }

                con.Close();
            }

            return false;
        }

        public bool RemoveCheck(int RollNo, int Class)
        {
            bool flag = false;

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM Student WHERE RollNo = @RollNo AND Class = @Class AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@RollNo", RollNo); 
                    command.Parameters.AddWithValue("@Class", Class);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return true;
                    }
                }

                con.Close();
            }

            return false;
        }
    }
}
