using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Calendar
{
    public class Database
    {

        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // filename: full file path to the database file
        // ===================================================================
        public static void newDatabase(string filename)
        {
            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            // your code
            //connecting to the db and opening it  
            string cs = $"Data Source={filename}; Foreign Keys=1";
            _connection = new SQLiteConnection(cs);
            _connection.Open();

            //put this inside a method 
            using var cmd = new SQLiteCommand(_connection);

            var pragmaOff = new SQLiteCommand("PRAGMA foreign_keys=OFF", _connection);
            pragmaOff.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS events";
            cmd.ExecuteNonQuery();

            var pragmaOn = new SQLiteCommand("PRAGMA foreign_keys=ON", _connection);
            pragmaOn.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY, Description TEXT);";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categories(Id INTEGER PRIMARY KEY, Description TEXT, TypeId INT NOT NULL, FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id));";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE events(Id INTEGER PRIMARY KEY, CategoryId INT NOT NULL, StartDateTime TEXT, DurationInMinutes REAL, Details TEXT, FOREIGN KEY(CategoryId) REFERENCES categories(Id));";
            cmd.ExecuteNonQuery();

            //cmd.CommandText = @"INSERT INTO events(CategoryId INT NOT NULL, StartDateTime TEXT, DurationInMinutes REAL, Details TEXT, FOREIGN KEY(CategoryId) REFERENCES categories(Id)) VALUES(1,1,'2024-02-14', 55, 'BLABLA');";

            //have to comment it out to pass 'SQLite_TestNewDatabase_newDBDoesExist_shouldHaveNoData'
            //PopulateCategoriesTypeTable(cmd);

            //populating categories table 
            //PopulateCategoriesTable(cmd);

            //PopulateEventsTable(cmd);


        }

        //tested and it works

        public static void PopulateCategoriesTypeTable(SQLiteCommand cmd)
        {
            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES(@Description);";
            cmd.Parameters.AddWithValue("@Description", Category.CategoryType.Event.ToString());
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES(@Description);";
            cmd.Parameters.AddWithValue("@Description", Category.CategoryType.Availability.ToString());
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES(@Description);";
            cmd.Parameters.AddWithValue("@Description", Category.CategoryType.AllDayEvent.ToString());
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES(@Description);";
            cmd.Parameters.AddWithValue("@Description", Category.CategoryType.Holiday.ToString());
            cmd.Prepare();
            cmd.ExecuteNonQuery();

        }




        public static void PopulateEventsTable(SQLiteCommand cmd)
        {
            Events e = new Events();
            List<Event> newList = e.List();


        }


        // ===================================================================
        // open an existing database
        // ===================================================================
        public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // your code
            string cs = $"Data Source={filename}; Foreign Keys=1";
            _connection = new SQLiteConnection(cs);
            _connection.Open();
            //using var cmd = new SQLiteCommand(_connection);
            //PopulateCategoriesTypeTable(cmd);
        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();


                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }

}