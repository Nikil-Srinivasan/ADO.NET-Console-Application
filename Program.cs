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
            Console.WriteLine("Enter the Choice to perform an action:\n\n1.Create Table\n2.Insert Data\n3.Read Table\n4.Update Data\n5.Delete Table\n6.Display Available Tables");
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
                            columnDetails += $"{columnName} {columnType})";
                        if (i < columnCount - 1)
                            columnDetails += ", ";
                    }

                    command = new SqlCommand($"CREATE TABLE {tableNameCreate} ({columnDetails})", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nTable created successfully");
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
                    Console.WriteLine("\nValues inserted into table successfully");
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
                        Console.Write($"{columnName}\t");
                    }
                    Console.WriteLine("\n---------------------------------------------------");
                    while (readReader.Read())
                    {
                        foreach (var columnName in columnNamesRead)
                        {
                            Console.Write($"{readReader[columnName]}\t");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("---------------------------------------------------");

                    readReader.Close();
                    Console.WriteLine("\nTable read successfully");
                    break;
                case 4:
                    Console.Write("\nEnter table name: ");
                    string tableNameUpdate = Console.ReadLine();
                    Console.Write("Enter column name for condition: ");
                    string conditionColumn = Console.ReadLine();
                    Console.Write($"Enter value for {conditionColumn}: ");
                    string conditionValue = Console.ReadLine();
                    Console.Write("Enter column name to update: ");
                    string updateColumn = Console.ReadLine();
                    Console.Write($"Enter new value for {updateColumn}: ");
                    string updateValue = Console.ReadLine();

                    command = new SqlCommand($"UPDATE {tableNameUpdate} SET {updateColumn} = '{updateValue}' WHERE {conditionColumn} = '{conditionValue}'", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nTable updated successfully");
                    break;
                case 5:
                    Console.Write("\nEnter table name: ");
                    string tableNameDelete = Console.ReadLine();
                    command = new SqlCommand($"DROP TABLE IF EXISTS {tableNameDelete}", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nTable deleted successfully");
                    break;
                case 6:
                    command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", connection);
                    SqlDataReader tablesReader = command.ExecuteReader();

                    Console.WriteLine("\nAvailable Tables:");
                    Console.WriteLine("------------------");
                    while (tablesReader.Read())
                    {
                        Console.WriteLine(tablesReader["TABLE_NAME"]);
                    }
                    tablesReader.Close();
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
