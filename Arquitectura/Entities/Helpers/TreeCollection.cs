using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Helpers
{
    public class TreeCollection
    {
        public long Id { get; set; }
        public long parentId { get; set; }
        public bool isChecked { get; set; }
        public bool hasChild { get; set; }
        public String sNombre { get; set; }
    }
}
