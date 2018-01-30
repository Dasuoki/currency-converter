using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;


namespace CurrencyConverter
{
    public partial class Form1 : Form
    {
        //definiranje globalnih
        public Dictionary<string, float> Values = new Dictionary<string, float>();
        public float Multi = 1;
        public string Unit1;
        public string Unit2;
        public string ApiKey = "ab92a57b10b54a399946c3d1cf2f2fa1";


        public void Baza()
        {
            //preuzimanje podataka sa openexchangerates api
            string json = new WebClient().DownloadString("https://openexchangerates.org/api/latest.json?app_id="+ApiKey);
            json = json.Remove(0, 189);
            json = json.Remove(json.Length - 1, 1);
            Values = JsonConvert.DeserializeObject<Dictionary<string, float>>(json);

            //upis podataka u tablicu za kasnije koristenje
            using (var writer = new StreamWriter(@"values.csv"))
            {
                foreach (var pair in Values)
                {
                    writer.WriteLine("{0};{1};", pair.Key, pair.Value);
                }
            }
            var t = File.GetLastWriteTime(@"values.csv");
            label3.Text = "Baza osvježena: " + t;
            label3.Visible = true;
        }

        public Form1()
        {
            InitializeComponent();

            //provjera da li postoji stara tablica, ako ne postoji preuzmi novu
            if (!File.Exists(@"values.csv"))
            {
                var writer = new StreamWriter(File.Create(@"values.csv"));
                writer.Close();
                Baza();
            }
            else
            {
                //ako je starija od 24 sata preuzmi novu
                FileInfo fil = new FileInfo(@"values.csv");
                if (fil.LastWriteTime < DateTime.Now.AddDays(-1))
                {
                    Baza();
                }
                //ako je friska prenesi ju u Dict
                else
                {
                    var reader = new StreamReader(File.OpenRead(@"values.csv"));
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var val = line.Split(';');
                        var key = val[0];
                        var value = Convert.ToSingle(val[1]);
                        Values.Add(key, value);
                    }
                }
            }
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
                Multi = (Values["USD"] / Values[Unit1]) * float.Parse(textBox1.Text);
                textBox2.Text = (Values[Unit2] * Multi).ToString(CultureInfo.CurrentCulture);
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
}
//(float)Convert.ToDouble