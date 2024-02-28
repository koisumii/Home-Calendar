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
            var con = new SQLiteConnection(cs);
            con.Open();

            //put this inside a method 
            using var cmd = new SQLiteCommand(con);

            //cmd.CommandText = "SET FOREIGN_KEY_CHECKS=0; ";
            //cmd.ExecuteNonQuery();

            var pragmaOff = new SQLiteCommand("PRAGMA foreign_keys=OFF", con);
            pragmaOff.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS category_types";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS events";
            cmd.ExecuteNonQuery();

            var pragmaOn = new SQLiteCommand("PRAGMA foreign_keys=ON", con);
            pragmaOn.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE category_types(id INTEGER PRIMARY KEY, description TEXT);";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categories(id INTEGER PRIMARY KEY, description TEXT, type_id INT NOT NULL, FOREIGN KEY(type_id) REFERENCES category_types(id));";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE events(id INTEGER PRIMARY KEY, start_date_time TEXT, details TEXT, duration_in_minutes REAL, category_id INT NOT NULL, FOREIGN KEY(category_id) REFERENCES categories(id));";
            cmd.ExecuteNonQuery();

            //populate the tables 
            //there is supposed to be a method that gets all the categories and the events and we populate the tables with that method 

            //put this in a method, possibly inside a loop? loop over the enum?
            //PopulateCategoriesTypeTable(cmd);

            //populating categories table 
            //PopulateCategoriesTable(cmd);

            //now populating the events table 
            //PopulateEventsTable(cmd);
        }

        // ===================================================================
        // 
        // ===================================================================
        public static void PopulateCategoriesTypeTable(SQLiteCommand cmd)
        {
            //there is supposed to be a method that gets all the categories and the events and we populate the tables with that method
            /*cmd.CommandText = "INSERT INTO category_types(description) VALUES(@description);";
            cmd.Parameters.AddWithValue("@description", Category.CategoryType.Event.ToString()); //i feel like this is wrong
            cmd.Prepare();
            cmd.ExecuteNonQuery(); 

            cmd.CommandText = "INSERT INTO category_types(description) VALUES(@description);";
            cmd.Parameters.AddWithValue("@description", Category.CategoryType.AllDayEvent.ToString());
            cmd.Prepare();
            cmd.ExecuteNonQuery(); 

            cmd.CommandText = "INSERT INTO category_types(description) VALUES(@description);";
            cmd.Parameters.AddWithValue("@description", Category.CategoryType.Holiday.ToString());
            cmd.Prepare();
            cmd.ExecuteNonQuery();*/
        }

        // ===================================================================
        // 
        // ===================================================================
        public static void PopulateCategoriesTable(SQLiteCommand cmd) 
        {
            /*Categories c1 = new Categories(); //this would be the default categories ?
            List<Category> categoriesList = c1.List();

            for(int i = 0; i < categoriesList.Count; i++) 
            {
                cmd.CommandText = $"INSERT INTO categories(description, type_id) VALUES(@description, @type_id);";
                cmd.Parameters.AddWithValue("@description", categoriesList[i].Description);
                cmd.Parameters.AddWithValue("@type_id", categoriesList[i].Type.ToString());
                cmd.Prepare();
                cmd.ExecuteNonQuery(); //row inserted
            }
            Categories c1 = new Categories();
            c1.SetCategoriesToDefaults();*/

        }

        public static void PopulateEventsTable(SQLiteCommand cmd) 
        { 
            /*Events e1 = new Events();
            List<Event> eventsList = e1.List();

            //testing date 
            eventsList[0].StartDateTime.ToString("yyyy-MM-dd");*/
        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // your code
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
