using System.Data.SqlClient;
using System.Threading;
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

            while (isRunning)
            {
                Console.Write("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("\nEnter the Choice to perform an action:");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("1. Create Table");
                Console.WriteLine("2. Insert Data");
                Console.WriteLine("3. Read Table");
                Console.WriteLine("4. Update Data");
                Console.WriteLine("5. Delete Table");
                Console.WriteLine("6. Display Available Tables");
                Console.WriteLine("7. Close Application");
                Console.Write("\nYour Choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                SqlCommand command;

                switch (choice)
                {
                    case 1:
                        Console.Write("\nEnter table name: ");
                        string tableNameCreate = Console.ReadLine();
                        Console.Write("Enter column count: ");
                        int columnCount = Convert.ToInt32(Console.ReadLine());

                        // Collecting column details
                        string columnDetails = "";
                        for (int i = 0; i < columnCount; i++)
                        {
                            Console.Write($"Enter column {i + 1} name: ");
                            string columnName = Console.ReadLine();
                            Console.Write($"Enter {columnName} type: ");
                            string columnType = Console.ReadLine();
                            if (columnType == "VARCHAR")
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

                        command = new SqlCommand($"CREATE TABLE {tableNameCreate} ({columnDetails})", connection);
                        command.ExecuteNonQuery();
                        Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                        Console.WriteLine("|| Table created successfully ||");
                        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        break;

                    case 2:
                        Console.Write("\nEnter table name: ");
                        string tableNameInsert = Console.ReadLine();
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
                        command = new SqlCommand(insertQuery, connection);
                        command.ExecuteNonQuery();
                        Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                        Console.WriteLine("|| Values inserted successfully ||");
                        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        break;
                    case 3:
                        Console.Write("\nEnter table name: ");
                        string tableNameRead = Console.ReadLine();
                        command = new SqlCommand($"SELECT * FROM {tableNameRead}", connection);
                        SqlDataReader readReader = command.ExecuteReader();

                        // Collecting column names
                        var columnNamesRead = new List<string>();
                        for (int i = 0; i < readReader.FieldCount; i++)
                        {
                            columnNamesRead.Add(readReader.GetName(i));
                        }

                        // Formatting table details
                        Console.WriteLine();
                        foreach (var columnName in columnNamesRead)
                        {
                            Console.Write($"{columnName,-15}");
                        }
                        Console.WriteLine("\n");
                        while (readReader.Read())
                        {
                            foreach (var columnName in columnNamesRead)
                            {
                                Console.Write($"{readReader[columnName],-15}");
                            }
                            Console.WriteLine();
                        }

                        readReader.Close();
                        Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                        Console.WriteLine("|| Table readed successfully  ||");
                        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        break;
                    case 4:
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

                        command = new SqlCommand($"UPDATE {tableNameUpdate} SET {updateColumn} = '{updateValue}' WHERE {conditionColumn} = '{conditionValue}'", connection);
                        command.ExecuteNonQuery();
                        Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                        Console.WriteLine("|| Table updated successfully ||");
                        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        break;
                    case 5:
                        Console.Write("\nEnter table name: ");
                        string tableNameDelete = Console.ReadLine();
                        command = new SqlCommand($"DROP TABLE IF EXISTS {tableNameDelete}", connection);
                        command.ExecuteNonQuery();
                        Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                        Console.WriteLine("|| Table deleted successfully ||");
                        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n");
                        break;

                    case 6:
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

                    case 7:
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                // Delay before displaying choices again
                Thread.Sleep(2000);
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
            {
                connection.Close();
            }

        }
    }
}
