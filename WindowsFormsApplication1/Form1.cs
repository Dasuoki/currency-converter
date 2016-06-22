﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;


namespace CurrencyConverter
{
    public partial class Form1 : Form
    {
        public Dictionary<string, float> values = new Dictionary<string, float>();
        public float multi = 1;
        public string unit1;
        public string unit2;
        
        public void baza()
        {
            string json = new WebClient().DownloadString("https://openexchangerates.org/api/latest.json?app_id=ab92a57b10b54a399946c3d1cf2f2fa1");
            json = json.Remove(0, 585);
            json = json.Remove(json.Length - 1, 1);
            values = JsonConvert.DeserializeObject<Dictionary<string, float>>(json);

            using (var writer = new StreamWriter(@"values.csv"))
            {
                foreach (var pair in values)
                {
                    writer.WriteLine("{0};{1};", pair.Key, pair.Value);
                }
            }
            var t = File.GetLastWriteTime(@"values.csv");
            label3.Text = "Baza osvježena: " + t.ToString();
            label3.Visible = true;
        }

        public Form1()
        {
            InitializeComponent();

            if (!File.Exists(@"values.csv"))
            {
                var writer = new StreamWriter(File.Create(@"values.csv"));
                writer.Close();
                baza();
            }
            else
            {
                FileInfo fil = new FileInfo(@"values.csv");
                if (fil.LastWriteTime < DateTime.Now.AddDays(-1))
                {
                    baza();
                }
                else
                {
                    var reader = new StreamReader(File.OpenRead(@"values.csv"));
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var val = line.Split(';');
                        var key = val[0];
                        var value = Convert.ToSingle(val[1]);
                        values.Add(key, value);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            unit1 = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            unit2 = comboBox2.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                textBox1.Text = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                textBox1.Text = null;
            else
            {
                multi = (values["USD"] / values[unit1]) * float.Parse(textBox1.Text);
                textBox2.Text = (values[unit2] * multi).ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            baza();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text == "")
                textBox1.Text = null;
            else
            {
                multi = (values["USD"] / values[unit1]) * float.Parse(textBox1.Text);
                textBox2.Text = (values[unit2] * multi).ToString();
            }
        }
    }
}
//(float)Convert.ToDouble