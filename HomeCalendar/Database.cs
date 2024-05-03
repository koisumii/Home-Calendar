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
//            var cmd = new SQLiteCommand(Database._dbConnection);
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
    /// <summary>
    /// This class manages the connection to a database and creating the tables
    /// </summary>
    public class Database
    {

        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // filename: full file path to the database file
        // ===================================================================
        /// <summary>
        /// Creates a new database to store information. In this case, we are creating events, categories and category types. 
        /// </summary>
        /// <param name="filename"> The full location to the database file. </param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Database.newDatabase("C:\\Users\\2232607\\Documents\\Sprint2\\Milestone3_tests\\testDBInput.db");
        /// ]]>
        /// </code>
        /// </example>
        public static void newDatabase(string filename)
        {
            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

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

            cmd.CommandText = @"CREATE TABLE events(Id INTEGER PRIMARY KEY, StartDateTime TEXT, Details TEXT, DurationInMinutes REAL, CategoryId INT NOT NULL, FOREIGN KEY(CategoryId) REFERENCES categories(Id));";
            cmd.ExecuteNonQuery();


        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        /// <summary>
        /// This method only goes inside the database (i.e connecting to it). No need to create a database because it already exists. 
        /// </summary>
        /// <param name="filename"> The full location to the database file. </param>
        public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // your code
            string cs = $"Data Source={filename}; Foreign Keys=1";
            _connection = new SQLiteConnection(cs);
            _connection.Open();
        }

        /// <summary>
        ///  close existing database, wait for garbage collector to release the lock before continuing
        /// </summary>
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