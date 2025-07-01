using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class DashboardModel
    {
        public List<Item> ItemsList { get; set; }
        public int NoOfItemInStock { get; set; }
    }
    public class Item
    {
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal Value { get; set; }
        public decimal Rate { get; set; }
    }
}
