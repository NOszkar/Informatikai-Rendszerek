using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ramen
{
    public partial class Form1 : Form
    {
        private List<Country> countries = new List<Country>();
        private List<Brand> brands = new List<Brand>();
        private List<Ramen> ramens = new List<Ramen>();

        public Form1()
        {
            InitializeComponent();
            LoadData("ramen.csv");

            listCountries.DisplayMember = "Name";
            GetCountries();
        }

        private void LoadData(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName, Encoding.Default))
            {
                sr.ReadLine(); // Header
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');

                    var countryName = line[2];
                    var country = AddCountry(countryName);

                    var brandName = line[0];
                    var brand = AddBrand(brandName);

                    var ramen = new Ramen()
                    {
                        ID = ramens.Count + 1,
                        //Brand = line[0],
                        BrandFK = brand.ID,
                        Brand = brand,
                        Name = line[1],
                        CountryFK = country.ID,
                        Country = country,
                        Stars = Convert.ToDouble(line[3])
                    };
                    ramens.Add(ramen);
                }
            }
        }

        private Country AddCountry(string countryName)
        {
            var currentCountry = (from c in countries
                                  where c.Name.Equals(countryName)
                                  select c).FirstOrDefault();
            if (currentCountry == null)
            {
                currentCountry = new Country()
                {
                    ID = countries.Count + 1,
                    Name = countryName
                };
                countries.Add(currentCountry);
            }

            return currentCountry;
        }

        private Brand AddBrand(string brandName)
        {
            var currentBrand = (from b in brands
                                  where b.Name.Equals(brandName)
                                  select b).FirstOrDefault();
            if (currentBrand == null)
            {
                currentBrand = new Brand()
                {
                    ID = countries.Count + 1,
                    Name = brandName
                };
                brands.Add(currentBrand);
            }

            return currentBrand;
        }

        private void GetCountries()
        {
            var countriesList = from c in countries
                                where c.Name.Contains(txtCountryFilter.Text)
                                orderby c.Name
                                select c;
            listCountries.DataSource = countriesList.ToList();
        }

        private void txtCountryFilter_TextChanged(object sender, EventArgs e)
        {
            GetCountries();
        }

        private void listCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            var country = (Country)((ListBox)sender).SelectedItem;
            if (country == null)
                return;

            var countryRamens = from r in ramens
                                where r.CountryFK == country.ID
                                select r;

            var groupedRamens = from r in countryRamens
                                group r.Stars by r.Brand.Name into g
                                select new
                                {
                                    BrandName = g.Key,
                                    AverageRating = Math.Round(g.Average(), 2)
                                };

            var orderedGroups = from g in groupedRamens
                                orderby g.AverageRating descending
                                select g;

            dgwRamen.DataSource = orderedGroups.ToList();
        }
    }
}
