using Calendar;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
namespace HomeCalendar
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //conecting to db 
            Database.newDatabase("C:\\Users\\2232607\\OneDrive - John Abbott College\\app dev\\HomeCalendar\\HomeCalendar\\testDBInput.db");

        }
    }
}