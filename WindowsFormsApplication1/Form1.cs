using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;


namespace CurrencyConverter
{
    public partial class Form1 : Form
    {
        //definiranje globalnih
        public float Multi = 1;
        public string Unit1;
        public string Unit2;
        public string ApiKey = "ab92a57b10b54a399946c3d1cf2f2fa1";
        CurrencyRates json = new CurrencyRates();


        public void Baza()
        {
            //preuzimanje podataka sa openexchangerates api
            string rawJson = new WebClient().DownloadString("https://openexchangerates.org/api/latest.json?app_id="+ApiKey);

            json = JsonConvert.DeserializeObject<CurrencyRates>(rawJson);

            DateTime t = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            t = t.AddSeconds(json.Timestamp).ToLocalTime();

            label3.Text = "Baza osvježena: " + t;
            label3.Visible = true;
        }

        public Form1()
        {
            InitializeComponent();
            Baza();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Unit1 = comboBox1.Text.ToUpper();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Unit2 = comboBox2.Text.ToUpper();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //treba bit null jer se kreša
            if (textBox1.Text == "")
                textBox1.Text = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateValues();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //preuzmi svjeze podatke
            Baza();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            updateValues();
        }

        private void updateValues()
        {
            if (textBox1.Text == "")
                textBox1.Text = null;
            else
            {
                Multi = (json.Rates["USD"] / json.Rates[Unit1]) * float.Parse(textBox1.Text);
                textBox2.Text = (json.Rates[Unit2] * Multi).ToString(CultureInfo.CurrentCulture);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        //pomicanje prozora sa misom
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

    }

    public class CurrencyRates
    {
        public string Disclaimer { get; set; }
        public string License { get; set; }
        public int Timestamp { get; set; }
        public string Base { get; set; }
        public Dictionary<string, float> Rates { get; set; }
    }
}