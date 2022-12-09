using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ramen
{
    class Ramen
    {
        public int ID { get; set; }
        public int BrandFK { get; set; }
        public Brand Brand { get; set; }
        public string Name { get; set; }
        public int CountryFK { get; set; }
        public Country Country { get; set; }
        public double Stars { get; set; }
    }
}
