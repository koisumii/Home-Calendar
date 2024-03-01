using Calendar;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
namespace Calendar;

internal class Program
{
    static void Main(string[] args)
    {

            //conecting to db 
            Database.newDatabase("C:\\Users\\2232607\\OneDrive - John Abbott College\\app dev\\HomeCalendar\\Milestone3_tests\\testDBInput.db");

    }
}