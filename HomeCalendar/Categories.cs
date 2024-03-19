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

        // ====================================================================
        // Constructor
        // ====================================================================
        public Categories(SQLiteConnection dbConnection, bool newDB)
        {
            this.dbConnection = dbConnection;
            // save the dbConnection object
            // if newDB, call set the CategoryTypes, setCategoryDefaults
            //this works ! :D
            if (newDB)
            {
                SetCategoriesToDefaults();


                _dbconnection = dbconnection;
                _newDB = newDB;

                //if there is an existing db
                if (newDB)
                {
                    SetCategoriesToDefaults();
                }
            }
        }

        //retrieves category information from db and makes new Categories instances with it.
        private void SetCategoriesUsingDB()
        {
            _dbconnection.Open();
            using (DbCommand cmd = _dbconnection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, description, type_id FROM categories";

            // Use flag to determine whether to load categories from the/existing database or set them to defaults
             //should this be the opposite ?? if it is NOT a new db set it to the default 
            //if (!newDB)
            //{
            //    //no need,, already open
            //    //dbConnection.Open();
            //    string query = "SELECT Id, Description, TypeId FROM categories ORDER BY Id";
            //    using var cmd = new SQLiteCommand(query, dbConnection);
            //    using SQLiteDataReader reader = cmd.ExecuteReader();
            //    //_Categories.Clear();
            //    while (reader.Read())
            //    {
            //        int id = reader.GetInt32(0);
            //        string description = reader.GetString(1);
            //        int typeId = reader.GetInt32(2);
            //        CategoryType categoryType = GetCategoryTypeFromTypeId(typeId);
                    
                    
            //        //Populating table 
            //        //cmd.CommandText = @"INSERT INTO categories(Id, Description, TypeId) VALUES(@Id, @Description, @TypeId)";
            //        //cmd.Parameters.AddWithValue("@Id", id);
            //        //cmd.Parameters.AddWithValue("@Description", description);
            //        //cmd.Parameters.AddWithValue("@TypeId", typeId);
            //        //cmd.ExecuteNonQuery();

            //        _Categories.Add(new Category(id, description, categoryType));
            //        AddNewCategoryToDatabase(description, categoryType);

            //        /*if (Enum.TryParse(reader.GetString(2), out Category.CategoryType categoryType))
            //        {
            //            _Categories.Add(new Category(id, description, categoryType));
            //        }*/
            //    }
            //    //reader.Close();
            //    //PopulateCategoriesTable(cmd);
            //    //dbConnection.Close(); //??
            //}
            //else
            //{
            //    //when it is a new db it should have no data 
            //    SetCategoriesToDefaults();
           //}
        

        public CategoryType GetCategoryTypeFromTypeId(int typeId)
        {
            if (typeId == 1)
            {
                return CategoryType.Event; 
            }
            if(typeId == 2)
            {
                return CategoryType.Availability;
            }
            if(typeId == 3)
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

            //int new_num = 1;
            //if (_Categories.Count > 0)
            //{
            //    new_num = (from c in _Categories select c.Id).Max();
            //    new_num++;
            //}
            ////AddToDatabase
            //_Categories.Add(new Category(new_num, desc, type));

        }

        //public void AddNewCategoryToDatabase(string description, Category.CategoryType type)
        //{
        
        //    //dbConnection.Close();
        //}

        public void UpdateCategory(int id, string newDescription, Category.CategoryType newType)
        {

            // get the category from the db
            // modify it
            // save back to database
            SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
            cmd.CommandText = "UPDATE categories SET Description = @newDescription, TypeId = @newType WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@newDescription", newDescription);
            int x = ((int)newType);
            cmd.Parameters.AddWithValue("@newType", x);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Prepare();
            //int x = ((int)newType);
            //cmd.Parameters.AddWithValue("@newType", x);
            cmd.ExecuteNonQuery();

            //int index = _Categories.FindIndex(c => c.Id == id);
            //if(index == -1)
            //{
            //    throw new Exception("Category with ID of: {id} was not found! =(");
            //}
            //using var cmd = new SQLiteCommand(dbConnection);
            //dbConnection.Open();
            //cmd.CommandText = "UPDATE Categories SET Description = @newDescription, Type = @newType WHERE Id = @Id";
            //// used to assign actual values to these placeholders
            //// the placeholders in the SQL query above will be replaced with the value of these variables
            //cmd.Parameters.AddWithValue("@newDescription", newDescription); 
            //cmd.Parameters.AddWithValue("@newType", newType.ToString());
            //cmd.Parameters.AddWithValue("@Id", id);

            //cmd.ExecuteNonQuery();

            //dbConnection.Close();

            //should check if update failed and no category was updated? row = 0
        }

        // ====================================================================
        // Delete category
        // ====================================================================
        public void Delete(int Id)
        {
            // delete from database

            //int i = _Categories.FindIndex(x => x.Id == Id);
            
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
                    //
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

                //Console.WriteLine($"{id}, {description}, {typeId}");
                categoriesList.Add(new Category(id, description, type));
            }


            return categoriesList;

            // query database and return results



        }



    }
}

