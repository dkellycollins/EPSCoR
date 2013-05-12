using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Models
{
    public class DynamicTable
    {
        public string TableName { get; set; }

        public DynamicTableRow this[int id]
        {
            get
            {
                return _rows.Find((r) => r.ID == id); 
            }
        }

        public ICollection<string> ColumnNames { get; set; }

        public void AddRow(DynamicTableRow row)
        {
            if (_rows.Exists((r) => r.ID == row.ID))
                throw new ArgumentException("Row already exsits!");
            _rows.Add(row);
        }

        public void DeleteRow(int id)
        {
            DynamicTableRow row = this[id];
            if (row != null)
                _rows.Remove(row);
        }

        private List<DynamicTableRow> _rows = new List<DynamicTableRow>();
    }

    public class DynamicTableRow
    {
        public int ID {get; set;}
        
        public object this[string propertyName]
        {
            get
            {
                if(_properties.ContainsKey(propertyName))
                    return _properties[propertyName];
                return null;
            }
            set
            {
                _properties[propertyName] = value;
            }
        }

        private Dictionary<string, object> _properties = new Dictionary<string,object>();
    }
}