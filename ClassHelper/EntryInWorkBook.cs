using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelDepartment.ClassHelper
{
    public class EntryInWorkBook
    {
        
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public string MixingTitle { get; set; } // Название из таблицы Mixing
    }
}

