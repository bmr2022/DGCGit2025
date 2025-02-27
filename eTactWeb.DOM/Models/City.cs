using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class City
    {
        public Int16 CityId { get; set; }
        public Int16 StateId { get; set; }
        public Int16 CountryId { get; set; }
        public String CountryName { get; set; }
        public String State { get; set; }
        public String CityName { get; set; }
        public IList<City>? CountryList { get; set; }
        public IList<City>? StateList { get; set; }
        public IList<City>? CityList { get; set; }



    }
}
