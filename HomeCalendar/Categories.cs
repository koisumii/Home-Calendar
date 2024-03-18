﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Data.Common;
using System.Data.SQLite;
using static Calendar.Category;
using System.Net.Http.Headers;

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
        private static String DefaultFileName = "calendarCategories.txt";
        private List<Category> _Categories = new List<Category>();
        private string? _FileName;
        private string? _DirName;
        private SQLiteConnection dbConnection;

        // ====================================================================
        // Properties
        // ====================================================================
        public String? FileName { get { return _FileName; } }
        public String? DirName { get { return _DirName; } }

        // ====================================================================
        // Constructor
        // ====================================================================
        public Categories()
        {
            SetCategoriesToDefaults();
        }

        public Categories(SQLiteConnection dbConnection, bool newDB)
        {
            // Use flag to determine whether to load categories from the/existing database or set them to defaults
             //should this be the opposite ?? if it is NOT a new db set it to the default 
            if (!newDB)
            {
                //no need,, already open
                //dbConnection.Open();
                string query = "SELECT Id, Description, TypeId FROM categories ORDER BY Id";
                using var cmd = new SQLiteCommand(query, dbConnection);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                _Categories.Clear();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string description = reader.GetString(1);
                    int typeId = reader.GetInt32(2);
                    CategoryType categoryType = GetCategoryTypeFromTypeId(typeId);
                    
                    
                    //Populating table 
                    //cmd.CommandText = @"INSERT INTO categories(Id, Description, TypeId) VALUES(@Id, @Description, @TypeId)";
                    //cmd.Parameters.AddWithValue("@Id", id);
                    //cmd.Parameters.AddWithValue("@Description", description);
                    //cmd.Parameters.AddWithValue("@TypeId", typeId);
                    //cmd.ExecuteNonQuery();

                    _Categories.Add(new Category(id, description, categoryType));
                    /*if (Enum.TryParse(reader.GetString(2), out Category.CategoryType categoryType))
                    {
                        _Categories.Add(new Category(id, description, categoryType));
                    }*/
                }
                //reader.Close();
                //PopulateCategoriesTable(cmd);
                //dbConnection.Close(); //??
            }
            else
            {
                //when it is a new db it should have no data 
                SetCategoriesToDefaults();
            }
        }

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

        public static void PopulateCategoriesTable(SQLiteCommand cmd)
        {
            Categories c1 = new Categories();
            List<Category> categoriesList = c1.List();

            for (int i = 0; i < categoriesList.Count; i++)
            {
                cmd.CommandText = $"INSERT INTO categories(Description, TypeId) VALUES(@Description, @TypeId);";
                cmd.Parameters.AddWithValue("@Description", categoriesList[i].Description);
                int temp = (int)categoriesList[i].Type;
                cmd.Parameters.AddWithValue("@TypeId", temp);
                cmd.Prepare();
                cmd.ExecuteNonQuery(); //row inserted
            }

        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================
        public Category GetCategoryFromId(int i)
        {
            Category? c = _Categories.Find(x => x.Id == i);
            if (c == null)
            {
                throw new Exception("Cannot find category with id " + i.ToString());
            }
            return c;
        }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================
        public void ReadFromFile(String? filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current categories,
            // ---------------------------------------------------------------
            _Categories.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // If file exists, read it
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        public void SaveToFile(String? filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // set categories to default
        // ====================================================================
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            _Categories.Clear();

            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
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

        // ====================================================================
        // Add category
        // ====================================================================
        private void Add(Category category)
        {
            _Categories.Add(category);
        }

        public void Add(String desc, Category.CategoryType type)
        {
            int new_num = 1;
            if (_Categories.Count > 0)
            {
                new_num = (from c in _Categories select c.Id).Max();
                new_num++;
            }
            //AddToDatabase
            _Categories.Add(new Category(new_num, desc, type));
        }

        public void UpdateCategory(int id, string newDescription, Category.CategoryType newType)
        {
            int index = _Categories.FindIndex(c => c.Id == id);
            if(index == -1)
            {
                throw new Exception("Category with ID of: {id} was not found! =(");
            }
            using var cmd = new SQLiteCommand(dbConnection);
            dbConnection.Open();
            cmd.CommandText = "UPDATE Categories SET Description = @newDescription, Type = @newType WHERE Id = @Id";
            // used to assign actual values to these placeholders
            // the placeholders in the SQL query above will be replaced with the value of these variables
            cmd.Parameters.AddWithValue("@newDescription", newDescription); 
            cmd.Parameters.AddWithValue("@newType", newType.ToString());
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();

            dbConnection.Close();

            //should check if update failed and no category was updated? row = 0
        }

        // ====================================================================
        // Delete category
        // ====================================================================
        public void Delete(int Id)
        {
            int i = _Categories.FindIndex(x => x.Id == Id);
            if ( i >= 0)
            {
                _Categories.RemoveAt(i);
            }
            
        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();
            foreach (Category category in _Categories)
            {
                newList.Add(new Category(category));
            }
            return newList;
        }

        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {

            // ---------------------------------------------------------------
            // read the categories from the xml file, and add to this instance
            // ---------------------------------------------------------------
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                foreach (XmlNode category in doc.DocumentElement.ChildNodes)
                {
                    String id = (((XmlElement)category).GetAttributeNode("ID")).InnerText;
                    String typestring = (((XmlElement)category).GetAttributeNode("type")).InnerText;
                    String desc = ((XmlElement)category).InnerText;

                    Category.CategoryType type;
                    switch (typestring.ToLower())
                    {
                        case "event":
                            type = Category.CategoryType.Event;
                            break;
                        case "allday":
                            type = Category.CategoryType.AllDayEvent;
                            break;
                        case "holiday":
                            type = Category.CategoryType.Holiday;
                            break;
                        default:
                            type = Category.CategoryType.Event;
                            break;
                    }
                    this.Add(new Category(int.Parse(id), desc, type));
                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadXMLFile: Reading XML " + e.Message);
            }

        }



        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            try
            {
                // create top level element of categories
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Categories></Categories>");

                // foreach Category, create an new xml element
                foreach (Category cat in _Categories)
                {
                    XmlElement ele = doc.CreateElement("Category");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = cat.Id.ToString();
                    ele.SetAttributeNode(attr);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = cat.Type.ToString();
                    ele.SetAttributeNode(type);

                    XmlText text = doc.CreateTextNode(cat.Description);
                    doc.DocumentElement.AppendChild(ele);
                    doc.DocumentElement.LastChild.AppendChild(text);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("_WriteXMLFile: Reading XML " + e.Message);
            }

        }

    }
}

