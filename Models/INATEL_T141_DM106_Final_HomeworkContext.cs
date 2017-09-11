using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace INATEL_T141_DM106_Final_Homework.Models
{
    public class INATEL_T141_DM106_Final_HomeworkContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public INATEL_T141_DM106_Final_HomeworkContext() : base("name=INATEL_T141_DM106_Final_HomeworkContext")
        {
        }

        public System.Data.Entity.DbSet<INATEL_T141_DM106_Final_Homework.Models.Product> Products { get; set; }
    }
}
