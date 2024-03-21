using Calendar;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Calendar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Events e1 = new Events();

            //conecting to db 
            //Database.newDatabase("C:\\Users\\rania\\Downloads\\Sprint2-Milestone\\Milestone3_tests\\testDBInput.db");
            HomeCalendar h1 = new HomeCalendar("C:\\Users\\2232607\\Documents\\Sprint2\\Milestone3_tests\\testDBInput.db", false);
            //h1.GetCalendarItems();
            DateTime tmp = DateTime.Now;
            tmp.ToString("MM/dd/yyyy hh:mm tt");
            //string y = tmp.ToString();
            //y.Remove(y.Length - 3, 3);
            //DateTime date = DateTime.Parse(y);
            //DateTime x = DateTime.ParseExact(, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Console.WriteLine(tmp);
        }
    }
}