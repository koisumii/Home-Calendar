using Calendar;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
namespace Calendar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Events e1 = new Events();

            //conecting to db 
            Database.newDatabase("C:\\Users\\2232607\\Documents\\Sprint2\\Milestone3_tests\\testDBInput.db");

            Categories cat = new Categories(Database.dbConnection, false);
            cat.UpdateCategory(1, "testing update", Category.CategoryType.Holiday);
            List<Category> list = cat.List();
        }
    }
}