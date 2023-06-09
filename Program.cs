using System.Data.SqlClient;
using System.Threading;
using Spectre.Console;
class Program
{
    static void Main()
    {
        SqlConnection connection = null;
        bool isRunning = true;

        try
        {
            // Creating Connection  
            connection = new SqlConnection("data source=.; database=office; integrated security=SSPI");
            connection.Open();
            AnsiConsole.Write(new FigletText("Connection Established").LeftJustified().Color(Color.Green));

            while (isRunning)
            {
                Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("Enter the Choice to perform an action:");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("1. Create Table");
                Console.WriteLine("2. Insert Data");
                Console.WriteLine("3. Read Table");
                Console.WriteLine("4. Update Data");
                Console.WriteLine("5. Delete Table");
                Console.WriteLine("6. Display Available Tables");
                Console.WriteLine("7. Close Application");
                Console.Write("\nYour Choice: ");
                string choice = Console.ReadLine();

                SqlCommand command;

                switch (choice)
                {
                    case "1":
                        // Create Table
                        Console.Write("\nEnter table name: ");
                        string tableNameCreate = Console.ReadLine();
                        Console.Write("Enter column count: ");
                        int columnCount = Convert.ToInt32(Console.ReadLine());

                        // Collecting column details
                        string columnDetails = "";
                        for (int i = 0; i < columnCount; i++)
                        {
                            Console.Write($"\nEnter column {i + 1} name: ");
                            string columnName = Console.ReadLine();
                            Console.Write($"Enter {columnName} type: ");
                            string columnType = Console.ReadLine();
                            if (columnType == "VARCHAR" || columnType == "varchar")
                            {
                                Console.Write($"Enter {columnType} size: ");
                                string columnSize = Console.ReadLine();
                                columnDetails += $"{columnName} {columnType}({columnSize})";
                            }
                            else
                            {
                                columnDetails += $"{columnName} {columnType}";
                            }
                            if (i < columnCount - 1)
                                columnDetails += ", ";
                        }

                        try
                        {
                            command = new SqlCommand($"CREATE TABLE {tableNameCreate} ({columnDetails})", connection);
                            command.ExecuteNonQuery();
                            Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                            Console.WriteLine("|| Table created successfully ||");
                            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        }
                        catch (SqlException)
                        {
                            Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                            Console.WriteLine("|| Invalid data type. Table creation failed. ||");
                            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        }
                        break;

                    case "2":
                        // Insert Data
                        Console.Write("\nEnter table name: ");
                        string tableNameInsert = Console.ReadLine();
                        try
                        {
                            command = new SqlCommand($"SELECT * FROM {tableNameInsert}", connection);
                            SqlDataReader insertReader = command.ExecuteReader();

                            // Collecting column names
                            var columnNamesInsert = new List<string>();
                            for (int i = 0; i < insertReader.FieldCount; i++)
                            {
                                columnNamesInsert.Add(insertReader.GetName(i));
                            }
                            insertReader.Close();

                            // Collecting values for each column
                            var columnValuesInsert = new List<string>();
                            foreach (var columnName in columnNamesInsert)
                            {
                                Console.Write($"Enter value for column '{columnName}': ");
                                string columnValue = Console.ReadLine();
                                columnValuesInsert.Add(columnValue);
                            }

                            // Generating SQL query dynamically
                            string insertQuery = $"INSERT INTO {tableNameInsert} ({string.Join(", ", columnNamesInsert)}) VALUES ('{string.Join("', '", columnValuesInsert)}')";
                            // INSERT INTO tbl_name (col_name_1, col_name_2, ..., col_name_n) VALUES (value_1, value_2, ... value_n)

                            command = new SqlCommand(insertQuery, connection);
                            command.ExecuteNonQuery();
                            Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                            Console.WriteLine("|| Values inserted successfully ||");
                            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 208)
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Invalid table name. Insertion failed. ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                            else
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Invalid data. Insertion failed ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                        }
                        break;

                    case "3":
                        // Read Table
                        Console.Write("\nEnter table name: ");
                        string tableNameToRead = Console.ReadLine();
                        try
                        {
                            command = new SqlCommand($"SELECT * FROM {tableNameToRead}", connection);
                            SqlDataReader reader = command.ExecuteReader();

                            // Collecting column names
                            var columnNamesToRead = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columnNamesToRead.Add(reader.GetName(i));
                            }

                            // Formatting table details
                            // Display column names
                            Console.WriteLine();
                            foreach (var columnName in columnNamesToRead)
                            {
                                Console.Write($"{columnName,-15}");
                            }
                            Console.WriteLine("\n");

                            // Display column values
                            while (reader.Read())
                            {
                                foreach (var columnName in columnNamesToRead)
                                {
                                    Console.Write($"{reader[columnName],-15}");
                                }
                                Console.WriteLine();
                            }

                            reader.Close();
                            Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                            Console.WriteLine("|| Table readed successfully  ||");
                            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 208)
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Invalid table name. Insertion failed. ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                        }
                        break;

                    case "4":
                        // Update Data
                        Console.Write("\nEnter table name: ");
                        string tableNameUpdate = Console.ReadLine();
                        Console.Write("Enter column for condition: ");
                        string conditionColumn = Console.ReadLine();
                        Console.Write($"Enter value for {conditionColumn}: ");
                        string conditionValue = Console.ReadLine();
                        Console.Write("Enter column to update: ");
                        string updateColumn = Console.ReadLine();
                        Console.Write($"Enter new value for '{updateColumn}': ");
                        string updateValue = Console.ReadLine();

                        try
                        {
                            command = new SqlCommand($"UPDATE {tableNameUpdate} SET {updateColumn} = '{updateValue}' WHERE {conditionColumn} = '{conditionValue}'", connection);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Table updated successfully ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                            else
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Table update failed ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 208)
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Invalid table name. Table update failed ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                            else if (ex.Number == 207)
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Invalid column name. Table update failed ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                            else
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Table update failed. ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                        }
                        break;

                    case "5":
                        // Delete Table
                        Console.Write("\nEnter table name: ");
                        string tableNameDelete = Console.ReadLine();
                        try
                        {
                            command = new SqlCommand($"DROP TABLE {tableNameDelete}", connection);
                            command.ExecuteNonQuery();
                            Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                            Console.WriteLine("|| Table deleted successfully ||");
                            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 3701)
                            {
                                Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                                Console.WriteLine("|| Table does not exist in database ||");
                                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                            }
                        }
                        break;

                    case "6":
                        // Display Available Tables
                        command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", connection);
                        SqlDataReader tablesReader = command.ExecuteReader();

                        Console.WriteLine("\n=================");
                        Console.WriteLine("Available Tables:");
                        Console.WriteLine("=================\n");
                        while (tablesReader.Read())
                        {
                            Console.WriteLine(tablesReader["TABLE_NAME"]);
                        }
                        tablesReader.Close();
                        break;

                    case "7":
                        // Close Application
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                        Console.WriteLine("|| Invalid choice. Please try again. ||");
                        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        break;
                }
                // Delay before displaying choices again
                Thread.Sleep(2000);
            }
        }
        catch (Exception)
        {
            Console.WriteLine("OOPs, something went wrong: ");
        }
        finally
        {
            // Closing the connection  
            if (connection != null)
            {
                connection.Close();
                AnsiConsole.Write(new FigletText("Connection Closed...").LeftJustified().Color(Color.Red));
            }

        }
    }
}
