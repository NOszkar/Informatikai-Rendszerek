using Gyak5.Entities;
using Gyak5.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace Gyak5
{
    public partial class Form1 : Form
    {
        public BindingList<RateData> Rates = new BindingList<RateData>();
        public BindingList<string> Currencies = new BindingList<string>();

        public Form1()
        {
            InitializeComponent();
            
            var mnbService = new MNBArfolyamServiceSoapClient();
            var gcrb = new GetCurrenciesRequestBody() { };
            var response = mnbService.GetCurrencies(gcrb);
            var result = response.GetCurrenciesResult;

            var xml = new XmlDocument();
            xml.LoadXml(result);

            foreach(XmlElement element in xml.DocumentElement)
            {
                foreach(XmlElement e in element)
                {
                    Currencies.Add(e.InnerText);
                }
            }

            RefreshData();
        }

        public string RequestData()
        {
            Rates.Clear();

            var mnbService = new MNBArfolyamServiceSoapClient();
            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = comboBox1.Text,
                startDate = dateTimePicker1.Value.ToString(),
                endDate = dateTimePicker2.Value.ToString()
            };

            var response = mnbService.GetExchangeRates(request);

            var result = response.GetExchangeRatesResult;

            return result;
        }

        public void ProcessXML(string result)
        {
            // XML document létrehozása és az aktuális XML szöveg betöltése
            var xml = new XmlDocument();
            xml.LoadXml(result);

            // Végigmegünk a dokumentum fő elemének gyermekein
            foreach (XmlElement element in xml.DocumentElement)
            {
                // Létrehozzuk az adatsort és rögtön hozzáadjuk a listához
                // Mivel ez egy referencia típusú változó, megtehetjük, hogy előbb adjuk a listához és csak később töltjük fel a tulajdonságait
                var rate = new RateData();
                Rates.Add(rate);

                // Dátum
                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                // Valuta
                var childElement = (XmlElement)element.ChildNodes[0];
                if (childElement == null)
                    continue;
                rate.Currency = childElement.GetAttribute("curr");

                // Érték
                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                    rate.Value = value / unit;
            }

        }

        public void showDiagram(string result)
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        public void RefreshData()
        {
            string result = RequestData();
            ProcessXML(result);
            dataGridView1.DataSource = Rates;
            comboBox1.DataSource = Currencies;
            showDiagram(result);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
