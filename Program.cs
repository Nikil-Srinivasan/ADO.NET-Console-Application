using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        SqlConnection connection = null;
        try
        {
            // Creating Connection  
            connection = new SqlConnection("data source=.; database=office; integrated security=SSPI");
            Console.WriteLine("Enter the Choice to perform an action on the table:\n\n1.Create\n2.Insert\n3.Read\n4.Update\n5.Delete");
            Console.Write("\nYour Choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());

            // Opening Connection  
            connection.Open();

            // Creating a command object
            SqlCommand command;

            // Writing SQL queries based on the user's choice
            switch (choice)
            {
                case 1:
                    command = new SqlCommand("CREATE TABLE sample (id int not null, name varchar(100))", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Table created Successfully");
                    break;
                case 2:
                    command = new SqlCommand("INSERT INTO sample VALUES(10,'NIKIL')", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Values inserted into table Successfully");
                    break;
                case 3:
                    command = new SqlCommand("SELECT * FROM sample", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["id"]}, Name: {reader["name"]}");
                    }
                    reader.Close();
                    Console.WriteLine("Table read Successfully");
                    break;
                case 4:
                    command = new SqlCommand("UPDATE sample SET name = 'manoj' WHERE id = 10", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Table updated Successfully");
                    break;
                case 5:
                    command = new SqlCommand("DROP TABLE IF EXISTS sample", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Table deleted Successfully");
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("OOPs, something went wrong: " + e.Message);
        }
        finally
        {
            // Closing the connection  
            if (connection != null)
                connection.Close();
        }
    }
}