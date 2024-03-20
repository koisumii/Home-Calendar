﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Globalization;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Events
    //        - A collection of Event items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    public class Events
    {
        private static String DefaultFileName = "calendar.txt";
        private List<Event> _Events = new List<Event>();
        private string _FileName;
        private string _DirName;

        private SQLiteConnection dbConnection;
        // ====================================================================
        // Properties
        // ====================================================================
        public String FileName { get { return _FileName; } }
        public String DirName { get { return _DirName; } }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================

        public Events()
        {

        }
        public Events(SQLiteConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        /*public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current Events,
            // so clear out any old definitions
            // ---------------------------------------------------------------
            _Events.Clear();

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
            // read the Events from the xml file
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use?
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);


        }*/

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        /*public void SaveToFile(String filepath = null)
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
        }*/



        // ====================================================================
        // Add Event
        // ====================================================================
        /*private void Add(Event exp)
        {
            _Events.Add(exp);
        }*/

        public void Add(string date, int category, Double duration, String details)
        {
            var pragmaOff = new SQLiteCommand("PRAGMA foreign_keys=OFF", this.dbConnection);
            pragmaOff.ExecuteNonQuery();

            string query = "INSERT INTO events(CategoryId, StartDateTime, DurationInMinutes, Details) VALUES(@CategoryId, @StartDateTime, @DurationInMinutes, @Details)";
            using var cmd = new SQLiteCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@CategoryId", category);
            cmd.Parameters.AddWithValue("@StartDateTime", date);
            cmd.Parameters.AddWithValue("@DurationInMinutes", duration);
            cmd.Parameters.AddWithValue("@Details", details);
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Delete Event
        // ====================================================================
        public void Delete(int EventId)
        {
            try
            {
                var pragmaOff = new SQLiteCommand("PRAGMA foreign_keys=OFF", this.dbConnection);
                pragmaOff.ExecuteNonQuery();

                using var cmd = new SQLiteCommand(this.dbConnection);
                cmd.CommandText = "DELETE FROM events WHERE Id = @EventId;";
                cmd.Parameters.AddWithValue("@EventId", EventId);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    //No rows affected means nothing was deleted.
                    return;
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while deleting an event from the database");
            }
        }

        public void UpdateEvent(int id, int category, DateTime date, double duration, String details)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
                cmd.CommandText = "UPDATE events SET CategoryId = @category, StartDateTime = @date, DurationInMinutes = @duration, Details = @details WHERE Id = @Id;";

                cmd.Parameters.AddWithValue("@CategoryId", category);
                cmd.Parameters.AddWithValue("@StartDateTime", date);
                cmd.Parameters.AddWithValue("@DurationInMinutes", duration);
                cmd.Parameters.AddWithValue("@Details", details);
                cmd.Parameters.AddWithValue("@EventId", id);

                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while updating an event from the database");
            }          
        }

        // ====================================================================
        // Return list of Events
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        public List<Event> List()
        {
            //List<Event> newList = new List<Event>();
            //foreach (Event Event in _Events)
            //{
            //    newList.Add(new Event(Event));
            //}
            //return newList;
            List<Event> eventsList = new List<Event>();

            string query = "SELECT * FROM events ";
            SQLiteCommand cmd = new SQLiteCommand(query, this.dbConnection);
            using SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int categoryId = reader.GetInt32(1);
                string startDateTime = reader.GetString(2);
                float durationInMinutes = reader.GetFloat(3);
                string details = reader.GetString(4);
                //DateTime startDate = DateTime.ParseExact(startDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                //Console.WriteLine($"{id}, {description}, {typeId}");
                Add(startDateTime, categoryId, durationInMinutes, details);
            }


            return eventsList;
        }


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        //private void _ReadXMLFile(String filepath)
        //{


        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(filepath);

        //        // Loop over each Event
        //        foreach (XmlNode Event in doc.DocumentElement.ChildNodes)
        //        {
        //            // set default Event parameters
        //            int id = int.Parse((((XmlElement)Event).GetAttributeNode("ID")).InnerText);
        //            String description = "";
        //            DateTime date = DateTime.Parse("2000-01-01");
        //            int category = 0;
        //            Double DurationInMinutes = 0.0;

        //            // get Event parameters
        //            foreach (XmlNode info in Event.ChildNodes)
        //            {
        //                switch (info.Name)
        //                {
        //                    case "StartDateTime":
        //                        date = DateTime.Parse(info.InnerText);
        //                        break;
        //                    case "DurationInMinutes":
        //                        DurationInMinutes = Double.Parse(info.InnerText);
        //                        break;
        //                    case "Items":
        //                        description = info.InnerText;
        //                        break;
        //                    case "Category":
        //                        category = int.Parse(info.InnerText);
        //                        break;
        //                }
        //            }

        //            // have all info for Event, so create new one
        //            this.Add(new Event(id, date, category, DurationInMinutes, description));

        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("ReadFromFileException: Reading XML " + e.Message);
        //    }
        //}


        // ====================================================================
        // write to an XML file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            // ---------------------------------------------------------------
            // loop over all categories and write them out as XML
            // ---------------------------------------------------------------
            try
            {
                // create top level element of Events
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Events></Events>");

                // foreach Category, create an new xml element
                foreach (Event exp in _Events)
                {
                    // main element 'Event' with attribute ID
                    XmlElement ele = doc.CreateElement("Event");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = exp.Id.ToString();
                    ele.SetAttributeNode(attr);
                    doc.DocumentElement.AppendChild(ele);

                    // child attributes (date, description, DurationInMinutes, category)
                    XmlElement d = doc.CreateElement("StartDateTime");
                    XmlText dText = doc.CreateTextNode(exp.StartDateTime.ToString());
                    ele.AppendChild(d);
                    d.AppendChild(dText);

                    XmlElement de = doc.CreateElement("Items");
                    XmlText deText = doc.CreateTextNode(exp.Details);
                    ele.AppendChild(de);
                    de.AppendChild(deText);

                    XmlElement a = doc.CreateElement("DurationInMinutes");
                    XmlText aText = doc.CreateTextNode(exp.DurationInMinutes.ToString());
                    ele.AppendChild(a);
                    a.AppendChild(aText);

                    XmlElement c = doc.CreateElement("Category");
                    XmlText cText = doc.CreateTextNode(exp.Category.ToString());
                    ele.AppendChild(c);
                    c.AppendChild(cText);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("SaveToFileException: Reading XML " + e.Message);
            }
        }

    }
}

