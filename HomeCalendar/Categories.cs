using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Data.Common;
using System.Data.SQLite;

using static Calendar.Category;
using System.Net.Http.Headers;
using System.Configuration;
using System.Security.Cryptography;
using System.Globalization;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    public class Categories
    {
        private SQLiteConnection dbConnection;
        // documentation done 
        // ====================================================================
        // Constructor
        // ====================================================================
        public Categories(SQLiteConnection dbConnection, bool newDB)
        {
            this.dbConnection = dbConnection;
            // save the dbConnection object
            // if newDB, call set the CategoryTypes, setCategoryDefaults
            // this works ! :D
            if (newDB)
            {
                PopulateCategoriesTypeTable();
                SetCategoriesToDefaults();
            }
        }
      
        public CategoryType GetCategoryTypeFromTypeId(int typeId)
        {
            if (typeId == 1)
            {
                return CategoryType.Event;
            }
            if (typeId == 2)
            {
                return CategoryType.Availability;
            }
            if (typeId == 3)
            {
                return CategoryType.AllDayEvent;
            }

            return CategoryType.Holiday;
        }


        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================
        //public Category GetCategoryFromId(int i)
        //{
        //    // query database

        //}


        // ====================================================================
        // set categories to default
        // ====================================================================
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            string query = "DELETE FROM categories";
            SQLiteCommand cmd = new SQLiteCommand(query, this.dbConnection);
            cmd.ExecuteNonQuery();

            Add("School", Category.CategoryType.Event);
            Add("Personal", Category.CategoryType.Event);
            Add("VideoGames", Category.CategoryType.Event);
            Add("Medical", Category.CategoryType.Event);
            Add("Sleep", Category.CategoryType.Event);
            Add("Vacation", Category.CategoryType.AllDayEvent);
            Add("Travel days", Category.CategoryType.AllDayEvent);
            Add("Canadian Holidays", Category.CategoryType.Holiday);
            Add("US Holidays", Category.CategoryType.Holiday);
        }

        public void SetCategoryTypes()
        {

        }

        //this works !
        public void Add(String desc, Category.CategoryType type)
        {
            var pragmaOff = new SQLiteCommand("PRAGMA foreign_keys=OFF", this.dbConnection);
            pragmaOff.ExecuteNonQuery();
            // use the database to insert the Category into the database
            string query = "INSERT INTO categories(Description, TypeId) VALUES(@Description, @TypeId)";
            using var cmd = new SQLiteCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@Description", desc);
            int x = ((int)type);
            cmd.Parameters.AddWithValue("@TypeId", x);
            cmd.ExecuteNonQuery();

        }

        public void PopulateCategoriesTypeTable()
        {
            SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
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

        public void UpdateCategory(int id, string newDescription, Category.CategoryType newType)
        {
            SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
            cmd.CommandText = "UPDATE categories SET Description = @newDescription, TypeId = @newType WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@newDescription", newDescription);
            int x = ((int)newType);
            cmd.Parameters.AddWithValue("@newType", x);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Delete category
        // ====================================================================
        public void Delete(int Id)
        {
            // delete from database
            try
            {
                var pragmaOff = new SQLiteCommand("PRAGMA foreign_keys=OFF", this.dbConnection);
                pragmaOff.ExecuteNonQuery();

                using var cmd = new SQLiteCommand(this.dbConnection);
                cmd.CommandText = "DELETE FROM categories WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", Id);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception($"The category with ID {Id} does not exist.");
                }
            }

            catch (Exception e)
            {
                throw new Exception("An error occurred when deleting the category from the database.", e);
            }



        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        public List<Category> List()
        {
            List<Category> categoriesList = new List<Category>();

            string query = "SELECT * FROM categories ";
            SQLiteCommand cmd = new SQLiteCommand(query, this.dbConnection);
            using SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string description = reader.GetString(1);
                int typeId = reader.GetInt32(2);
                Category.CategoryType type = GetCategoryTypeFromTypeId(typeId);

                categoriesList.Add(new Category(id, description, type));
            }


            return categoriesList;

            // query database and return results



        }



    }
}
