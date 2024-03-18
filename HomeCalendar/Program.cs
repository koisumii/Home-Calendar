using Calendar;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
namespace HomeCalendar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Events e1 = new Events();

            //conecting to db 
            Database.newDatabase("C:\\Users\\2256255\\Downloads\\Sprint2\\Milestone3_tests\\testDBInput.db");

            Categories cat = new Categories(Database.dbConnection, false);
        }
    }
}