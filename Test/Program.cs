using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using System.Data;
using Newtonsoft.Json.Serialization;

namespace Test
{
    internal class Program
    {

        static async Task Main()
        {
            // SQL Server verilənlən bazasının məlumatları
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\Documents\test.mdf;Integrated Security=True;Connect Timeout=30";

            // JSONPlaceholder API-dən məlumatları əldə etmək üçün URL
            string apiUrl = "https://jsonplaceholder.typicode.com/comments?postId=1";


            // HttpClient yaratmaq və məlumatları əldə etmək
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string jsonResult = await client.GetStringAsync(apiUrl);

                    // JSON məlumatlarını List<User> obyektinə çevirmək
                    List<User> userList = JsonConvert.DeserializeObject<List<User>>(jsonResult, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include,
                        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
                        ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
                    });

                    // SQL Server-də olan verilənlən bazasına qoşulmaq
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // List<User> obyektindəki məlumatları SQL Server-də olan tabloya əlavə etmək üçün SqlCommand yaratmaq
                        foreach (var user in userList)
                        {
                            using (SqlCommand command = new SqlCommand("INSERT INTO Employee (postId,id, name, email,body) VALUES (@PostId,@Id, @Name, @Email,@Body)", connection))
                            {
                                // Parametrləri təyin etmək
                                command.Parameters.Add("@PostId", SqlDbType.Int).Value = user.PostId;
                                command.Parameters.Add("@Id", SqlDbType.Int).Value = user.Id;
                                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = user.Name;
                                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = user.Email;
                                command.Parameters.Add("@Body", SqlDbType.NVarChar).Value = user.Body;
                   

                               


                                // SqlCommand-i icra etmək
                                int affectedRows = command.ExecuteNonQuery();
                                Console.WriteLine($"Affected Rows: {affectedRows}");
                            }

                        }

                        Console.WriteLine("Data inserted into SQL Server table successfully.");
                        Console.WriteLine("JSON Result: " + jsonResult );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.ReadLine();
        }
        public class User
        {
            public int PostId { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }

            public string Body { get; set; }



        }


    }
      
}
