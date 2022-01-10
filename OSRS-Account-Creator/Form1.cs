using CefSharp;
using CefSharp.WinForms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSRS_Account_Creator
{

    public partial class MainForm : Form
    {
        public static List<string> DictionaryNames_A = new List<string>();
        public static List<string> DictionaryNames_B = new List<string>();
        public static string[] DictionaryPassword = new string[62] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public static string osrs_signup = "https://secure.runescape.com/m=account-creation/l=1/create_account?theme=oldschool#_ga=2.154472056.1744721716.1641683285-1198593986.1638198018";
        public static ChromiumWebBrowser browser;
        public static PictureBox placeholder;
        public static PictureBox loading;
        public static PictureBox timecounter;

        public static string dir_accounts = "";
        public static string dir_proxys = "";
        public static string dir_word_lists = "";

        public static string parent_host = "";
        public static string parent_port = "";
        public static string parent_name = "";
        public static string parent_password = "";
        public static bool success = false;
        public static bool success_trigger = false;

        public static string html;



        public static int proxy_connection_success = 0;
        public static bool proxy_icon_loader = false;

        public static bool connection_checker_waiter = false;

        public static bool can_action = false;
        public static bool main_thread_run = true;
        public static bool main_thread_check_general_conenction = false;
        public static bool main_thread_check_general_conenction_mode = false;
        public static int connection_result = 0;
        public static bool main_thread_check_proxy_connection = false;
        public static bool main_thread_osrs_connect = false;

        public static bool osrs_connected = true;

        public static Thread mainThread;



        public static long old_account_size = 0;
        public static long old_proxy_size = 0;
        public static long old_wordlist_a_size = 0;
        public static long old_wordlist_b_size = 0;

        public static List<string> list_proxys = new List<string>();
        public static List<string> list_wordlist_a = new List<string>();
        public static List<string> list_wordlist_b = new List<string>();
        public static List<string> list_accounts = new List<string>();

        public static bool created_at_least_one_account = false;

        public static int index_to_remove = 0;

        public static int session = 0;



        public static List<string> pattern = new List<string>();

        public static string dir_config = Directory.GetCurrentDirectory() + @"\Config";



        public static List<string> save_state = new List<string>();

        public MainForm()
        {
            InitializeComponent();

            //textbox_trashmail.Text = "mytrashmailer.com";

            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Config")) Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Config");

            if(File.Exists(dir_config + @"\OSRS-Website.txt"))
            {
                osrs_signup = File.ReadAllLines(dir_config + @"\OSRS-Website.txt")[0];
            }
            else
            {
                File.WriteAllLines(dir_config + @"\OSRS-Website.txt", new string[1] { osrs_signup });
            }

            if (File.Exists(dir_config + @"\Settings.txt"))
            {
                string[] tmp_lines = File.ReadAllLines(dir_config + @"\Settings.txt");
                dir_accounts = tmp_lines[0];
                dir_proxys = tmp_lines[1];
                dir_word_lists = tmp_lines[2];
            }
            else
            {
                string curr = Directory.GetCurrentDirectory();

                string[] dirs = new string[3];
                dirs[0] = curr + @"\Data\Generated Accounts";
                dirs[1] = curr + @"\Data\Proxys";
                dirs[2] = curr + @"\Data\Wordlists";

                File.WriteAllLines(dir_config + @"\Settings.txt", dirs);

                if (!Directory.Exists(curr + @"\Data"))
                {
                    Directory.CreateDirectory(curr + @"\Data");
                }

                if (!Directory.Exists(curr + @"\Data\Generated Accounts"))
                {
                    Directory.CreateDirectory(curr + @"\Data\Generated Accounts");
                }

                if (!Directory.Exists(curr + @"\Data\Proxys"))
                {
                    Directory.CreateDirectory(curr + @"\Data\Proxys");
                }

                if (!Directory.Exists(curr + @"\Data\Wordlists"))
                {
                    Directory.CreateDirectory(curr + @"\Data\Wordlists");
                }

                dir_accounts = dirs[0];
                dir_proxys = dirs[1];
                dir_word_lists = dirs[2];
            }



            textbox_pp_host.ForeColor = Color.LightGray;
            textbox_pp_port.ForeColor = Color.LightGray;
            textbox_pp_name.ForeColor = Color.LightGray;
            textbox_pp_password.ForeColor = Color.LightGray;

            textbox_pp_host.Text = "Enter host...";
            textbox_pp_port.Text = "Enter port...";
            textbox_pp_name.Text = "Enter username...";
            textbox_pp_password.Text = "Enter password...";



            if (!File.Exists(dir_config + @"\Save.sav"))
            {
                List<string> save_preset = new List<string>();

                save_preset.Add("1,0,0,0,0,0");
                save_preset.Add("-");
                save_preset.Add("-");
                save_preset.Add("-");
                save_preset.Add("-");
                save_preset.Add("mytrashmailer.com");
                save_preset.Add("1");

                File.WriteAllLines(dir_config + @"\Save.sav", save_preset.ToArray());
            }

            string[] save_lines = File.ReadAllLines(dir_config + @"\Save.sav");

            foreach (string s in save_lines[0].Split(',')) save_state.Add(s);

            save_state.Add(save_lines[1]);
            save_state.Add(save_lines[2]);
            save_state.Add(save_lines[3]);
            save_state.Add(save_lines[4]);
            save_state.Add(save_lines[5]);
            save_state.Add(save_lines[6]);

            //Load save
            if (save_state[0] == "1") checkbox_pin.Checked = true;
            if (save_state[1] == "1") checkbox_parameter_a.Checked = true;
            if (save_state[2] == "1") checkbox_parameter_b.Checked = true;
            if (save_state[3] == "1") checkbox_parameter_c.Checked = true;
            if (save_state[4] == "1") checkbox_account_proxy.Checked = true;
            if (save_state[5] == "1") checkbox_parent.Checked = true;

            if(save_state[6] != "-")
            {
                textbox_pp_host.Text = save_state[6];
                textbox_pp_host.ForeColor = Color.Black;
            }

            if (save_state[7] != "-")
            {
                textbox_pp_port.Text = save_state[7];
                textbox_pp_port.ForeColor = Color.Black;
            }

            if (save_state[8] != "-")
            {
                textbox_pp_name.Text = save_state[8];
                textbox_pp_name.ForeColor = Color.Black;
            }

            if (save_state[9] != "-")
            {
                textbox_pp_password.Text = save_state[9];
                textbox_pp_password.ForeColor = Color.Black;
            }

            if (save_state[10] != "-")
            {
                textbox_trashmail.Text = save_state[10];
            }

            if (save_state[11] == "1")
            {
                (new Thread(() => {
                    Thread.Sleep(150);
                    MessageBox.Show("Welcome to the OSRS Account Maker!\n\nIf you need a little guidance, look at the github repository \"https://github.com/MovEaxEax/osrs-account-maker\".\n\nDon't worry about saving data and stuff, as long as\nyou close the application normally, everything will save smooth.\n\nEnjoy your account creation process!");
                })).Start();
            }

            textbox_account_dir.Text = dir_accounts;
            textbox_proxy_dir.Text = dir_proxys;
            textbox_wordlist_dir.Text = dir_word_lists;

            if (!File.Exists(dir_accounts + @"\GeneratedAccounts.txt"))File.Create(dir_accounts + @"\GeneratedAccounts.txt").Close();
            if (!File.Exists(dir_proxys + @"\Proxys.txt")) File.Create(dir_proxys + @"\Proxys.txt").Close();
            if (!File.Exists(dir_word_lists + @"\Wordlist_A.txt")) File.Create(dir_word_lists + @"\Wordlist_A.txt").Close();
            if (!File.Exists(dir_word_lists + @"\Wordlist_B.txt")) File.Create(dir_word_lists + @"\Wordlist_B.txt").Close();
            if (!File.Exists(dir_word_lists + @"\GeneratedNames.txt")) File.Create(dir_word_lists + @"\GeneratedNames.txt").Close();

            if (!File.Exists(dir_config + @"\OptionalParameter.txt"))
            {
                List<string> parameter_preset = new List<string>();

                parameter_preset.Add("[Parameter A]");
                parameter_preset.Add("Value 1");
                parameter_preset.Add("Value 2");
                parameter_preset.Add("Value 3");
                parameter_preset.Add("[Parameter B]");
                parameter_preset.Add("Value 1");
                parameter_preset.Add("Value 2");
                parameter_preset.Add("Value 3");
                parameter_preset.Add("[Parameter C]");
                parameter_preset.Add("Value 1");
                parameter_preset.Add("Value 2");
                parameter_preset.Add("Value 3");

                File.WriteAllLines(dir_config + @"\OptionalParameter.txt", parameter_preset.ToArray());
            }

            List<string> parameter_data = File.ReadAllLines(dir_config + @"\OptionalParameter.txt").ToList<string>();


            int removeCounter = 0;
            List<string> list_parameter_a = new List<string>();
            if (parameter_data.Count > 0)
            {
                bool parameter_found = false;
                bool parameter_done = false;
                foreach(string parameter in parameter_data)
                {
                    if (!parameter_found)
                    {
                        if (parameter.Length > 2)
                        {
                            if (parameter[0] == '[' && parameter[parameter.Length - 1] == ']')
                            {
                                list_parameter_a.Add(parameter.Substring(1, parameter.Length - 2));
                                parameter_found = true;
                            }
                        }
                        removeCounter++;
                    }
                    else
                    {
                        if(parameter.Length > 2)
                        {
                            if (parameter[0] == '[' && parameter[parameter.Length - 1] == ']')
                            {
                                parameter_done = true;
                                break;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(parameter))
                                {
                                    list_parameter_a.Add(parameter);
                                    removeCounter++;
                                }
                            }
                        }
                        else if(!string.IsNullOrWhiteSpace(parameter))
                        {
                            list_parameter_a.Add(parameter);
                            removeCounter++;
                        }
                        else if (!parameter_done)
                        {
                            removeCounter++;
                        }
                    }
                }

                if (list_parameter_a.Count > 0)
                {
                    checkbox_parameter_a.Text = list_parameter_a[0];
                    list_parameter_a.RemoveAt(0);
                    if (list_parameter_a.Count > 0)
                    {
                        foreach (string s in list_parameter_a)
                        {
                            combo_parameter_a.Items.Add(s);
                        }
                    }
                    else
                    {
                        combo_parameter_a.Items.Add("None");
                    }
                }
                else
                {
                    checkbox_parameter_a.Text = "Parameter A";
                    combo_parameter_a.Items.Add("None");
                }

            }
            else
            {
                checkbox_parameter_a.Text = "Parameter C";
                combo_parameter_a.Items.Add("None");
            }
            if (parameter_data.Count > 0) if (removeCounter > 0) parameter_data.RemoveRange(0, removeCounter);

            removeCounter = 0;
            List<string> list_parameter_b = new List<string>();
            if (parameter_data.Count > 0)
            {
                bool parameter_found = false;
                bool parameter_done = false;
                foreach (string parameter in parameter_data)
                {
                    if (!parameter_found)
                    {
                        if (parameter.Length > 2)
                        {
                            if (parameter[0] == '[' && parameter[parameter.Length - 1] == ']')
                            {
                                list_parameter_b.Add(parameter.Substring(1, parameter.Length - 2));
                                parameter_found = true;
                            }
                        }
                        removeCounter++;
                    }
                    else
                    {
                        if (parameter.Length > 2)
                        {
                            if (parameter[0] == '[' && parameter[parameter.Length - 1] == ']')
                            {
                                parameter_done = true;
                                break;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(parameter))
                                {
                                    list_parameter_b.Add(parameter);
                                    removeCounter++;
                                }
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(parameter))
                        {
                            list_parameter_b.Add(parameter);
                            removeCounter++;
                        }
                        else if (!parameter_done)
                        {
                            removeCounter++;
                        }
                    }
                }

                if (list_parameter_b.Count > 0)
                {
                    checkbox_parameter_b.Text = list_parameter_b[0];
                    list_parameter_b.RemoveAt(0);
                    if (list_parameter_b.Count > 0)
                    {
                        foreach (string s in list_parameter_b)
                        {
                            combo_parameter_b.Items.Add(s);
                        }
                    }
                    else
                    {
                        combo_parameter_b.Items.Add("None");
                    }
                }
                else
                {
                    checkbox_parameter_b.Text = "Parameter B";
                    combo_parameter_b.Items.Add("None");
                }
            }
            else
            {
                checkbox_parameter_b.Text = "Parameter C";
                combo_parameter_b.Items.Add("None");
            }
            if (parameter_data.Count > 0) if (removeCounter > 0) parameter_data.RemoveRange(0, removeCounter);

            removeCounter = 0;
            List<string> list_parameter_c = new List<string>();
            if (parameter_data.Count > 0)
            {
                bool parameter_found = false;
                bool parameter_done = false;
                foreach (string parameter in parameter_data)
                {
                    if (!parameter_found)
                    {
                        if (parameter.Length > 2)
                        {
                            if (parameter[0] == '[' && parameter[parameter.Length - 1] == ']')
                            {
                                list_parameter_c.Add(parameter.Substring(1, parameter.Length - 2));
                                parameter_found = true;
                            }
                        }
                        removeCounter++;
                    }
                    else
                    {
                        if (parameter.Length > 2)
                        {
                            if (parameter[0] == '[' && parameter[parameter.Length - 1] == ']')
                            {
                                parameter_done = true;
                                break;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(parameter))
                                {
                                    list_parameter_c.Add(parameter);
                                    removeCounter++;
                                }
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(parameter))
                        {
                            list_parameter_c.Add(parameter);
                            removeCounter++;
                        }
                        else if (!parameter_done)
                        {
                            removeCounter++;
                        }
                    }
                }


                if (list_parameter_c.Count > 0)
                {
                    checkbox_parameter_c.Text = list_parameter_c[0];
                    list_parameter_c.RemoveAt(0);
                    if (list_parameter_c.Count > 0)
                    {
                        foreach (string s in list_parameter_c)
                        {
                            combo_parameter_c.Items.Add(s);
                        }
                    }
                    else
                    {
                        combo_parameter_c.Items.Add("None");
                    }
                }
                else
                {
                    checkbox_parameter_c.Text = "Parameter C";
                    combo_parameter_c.Items.Add("None");
                }
            }
            else
            {
                checkbox_parameter_c.Text = "Parameter C";
                combo_parameter_c.Items.Add("None");
            }
            if (parameter_data.Count > 0) if (removeCounter > 0) parameter_data.RemoveRange(0, removeCounter);



            if (!File.Exists(dir_config + @"\SavePattern.txt"))
            {
                List<string> save_pattern = new List<string>();

                save_pattern.Add("-----------------------------------------");
                save_pattern.Add("Email:{textbox_email}");
                save_pattern.Add("Password:{textbox_password}");
                save_pattern.Add("Charactername:{textbox_name}");
                save_pattern.Add("PIN:{textbox_pin}");
                save_pattern.Add("Birthdate:{textbox_birthdate}");
                save_pattern.Add("[checkbox_parameter_a]Parameter A:{textbox_parameter_a}");
                save_pattern.Add("[checkbox_parameter_b]Parameter B:{textbox_parameter_b}");
                save_pattern.Add("[checkbox_parameter_c]Parameter C:{textbox_parameter_c}");
                save_pattern.Add("[checkbox_proxy]Proxy:{textbox_proxy_host},{textbox_proxy_port}");
                File.WriteAllLines(dir_config + @"\SavePattern.txt", save_pattern.ToArray());
            }

            pattern = File.ReadAllLines(dir_config + @"\SavePattern.txt").ToList<string>();



            FileInfo[] dir_account_info = (new DirectoryInfo(dir_word_lists)).GetFiles();
            foreach (FileInfo fi in dir_account_info)
            {
                if (fi.Name.Contains("GeneratedNames"))
                {
                    if (fi.Length != old_account_size)
                    {
                        string[] tmp_accounts = File.ReadAllLines(dir_word_lists + @"\GeneratedNames.txt");
                        if (tmp_accounts.Length > 0) list_accounts = tmp_accounts.ToList<string>();
                        old_account_size = fi.Length;
                        break;
                    }
                }
            }

            FileInfo[] dir_proxy_info = (new DirectoryInfo(dir_proxys)).GetFiles();
            foreach (FileInfo fi in dir_proxy_info)
            {
                if (fi.Name.Contains("Proxys"))
                {
                    string[] tmp_proxys = File.ReadAllLines(dir_proxys + @"\Proxys.txt");
                    if (tmp_proxys.Length > 0) list_proxys = tmp_proxys.ToList<string>();
                    old_proxy_size = fi.Length;
                    break;
                }
            }

            FileInfo[] dir_wordlist_info = (new DirectoryInfo(dir_word_lists)).GetFiles();
            foreach (FileInfo fi in dir_wordlist_info)
            {
                if (fi.Name.Contains("Wordlist_A"))
                {
                    string[] tmp_wordlist = File.ReadAllLines(dir_word_lists + @"\Wordlist_A.txt");
                    if (tmp_wordlist.Length > 0) list_wordlist_a = tmp_wordlist.ToList<string>();
                    old_wordlist_a_size = fi.Length;
                }
                if (fi.Name.Contains("Wordlist_B"))
                {
                    string[] tmp_wordlist = File.ReadAllLines(dir_word_lists + @"\Wordlist_B.txt");
                    if (tmp_wordlist.Length > 0) list_wordlist_b = tmp_wordlist.ToList<string>();
                    old_wordlist_b_size = fi.Length;
                }
            }

            FileInfo[] dir_account_info_session = (new DirectoryInfo(dir_accounts)).GetFiles();
            for (int i = 0; i < dir_account_info_session.Length; i++)
            {
                string fname = dir_account_info_session[i].Name.Split('.')[0];
                if (fname.Contains("Session"))
                {
                    int factor = int.Parse(fname.Split('_')[2]);
                    if(factor > session)
                    {
                        session = factor;
                    }
                }
            }

            session++;


            timecounter = new PictureBox();
            this.Controls.Add(timecounter);
            timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_0;
            timecounter.Location = new Point(25, 23);
            timecounter.Size = new Size(40, 40);
            timecounter.SizeMode = PictureBoxSizeMode.StretchImage;
            timecounter.Visible = false;

            loading = new PictureBox();
            this.Controls.Add(loading);
            loading.Image = OSRS_Account_Creator.Properties.Resources.loading;
            loading.Location = new Point(875, 23);
            loading.Size = new Size(40, 40);
            loading.SizeMode = PictureBoxSizeMode.StretchImage;
            loading.Visible = false;

            placeholder = new PictureBox();
            this.Controls.Add(placeholder);
            placeholder.Image = OSRS_Account_Creator.Properties.Resources.Browser_Playceholder;
            placeholder.BackColor = Color.Transparent;
            placeholder.Location = new Point(12, 12);
            placeholder.Size = new Size(915, 552);
            placeholder.SizeMode = PictureBoxSizeMode.StretchImage;
            placeholder.Visible = true;


            Thread initilizer = new Thread(() => {
                while (!browser.IsBrowserInitialized)
                {
                    //browser.LoadUrl("https://google.com");
                    Thread.Sleep(10);
                }
                MessageBox.Show("Chromium subsystem initialized!");
            });

            //initilizer.Start();

            InitBrowser();

            osrs_connected = true;








            can_action = true;
            main_thread_check_general_conenction = false;
            connection_result = 1;
            main_thread_check_proxy_connection = false;
            main_thread_osrs_connect = false;

            main_thread_run = true;
            mainThread = new Thread(() => {

                bool main_thread_check_general_conenction_init = true;
                bool main_thread_check_general_conenction_waiter = true;
                int connection_time_out = 0;

                while (main_thread_run)
                {
                    try
                    {
                        if (main_thread_check_general_conenction)
                        {
                            //Checks connection and stores the result in 'connection_result'
                            //Set 'main_thread_check_general_conenction' to true to invoke
                            //'main_thread_check_general_conenction_mode' true for IP and false for OSRS
                            if (main_thread_check_general_conenction_init)
                            {
                                browser.BeginInvoke(new MethodInvoker(delegate ()
                                {
                                    Cef.GetGlobalCookieManager().DeleteCookies("", "");
                                    //if (browser.IsBrowserInitialized) browser.GetBrowser().Reload(true);
                                    if (main_thread_check_general_conenction_mode)
                                    {
                                        browser.LoadUrl("https://www.showmyip.gr/");
                                    }
                                    else
                                    {
                                        browser.LoadUrl(osrs_signup);
                                    }
                                }));

                                connection_time_out = 0;
                                connection_result = 1;
                                main_thread_check_general_conenction_waiter = true;
                                main_thread_check_general_conenction_init = false;
                            }
                            else
                            {
                                if (main_thread_check_general_conenction_waiter)
                                {
                                    bool getter = false;
                                    browser.BeginInvoke(new MethodInvoker(delegate ()
                                    {
                                        main_thread_check_general_conenction_waiter = browser.IsLoading;
                                        getter = true;
                                    }));

                                    while (!getter) { }

                                    if (connection_time_out < 2000)
                                    {
                                        connection_time_out++;
                                        if (connection_time_out % 100 == 0 && connection_time_out > 0)
                                        {
                                            switch (connection_time_out)
                                            {
                                                case 100:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_1;
                                                    }));
                                                    break;
                                                case 200:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_2;
                                                    }));
                                                    break;
                                                case 300:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_3;
                                                    }));
                                                    break;
                                                case 400:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_4;
                                                    }));
                                                    break;
                                                case 500:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_5;
                                                    }));
                                                    break;
                                                case 600:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_6;
                                                    }));
                                                    break;
                                                case 700:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_7;
                                                    }));
                                                    break;
                                                case 800:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_8;
                                                    }));
                                                    break;
                                                case 900:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_9;
                                                    }));
                                                    break;
                                                case 1000:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_10;
                                                    }));
                                                    break;
                                                case 1100:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_11;
                                                    }));
                                                    break;
                                                case 1200:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_12;
                                                    }));
                                                    break;
                                                case 1300:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_13;
                                                    }));
                                                    break;
                                                case 1400:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_14;
                                                    }));
                                                    break;
                                                case 1500:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_15;
                                                    }));
                                                    break;
                                                case 1600:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_16;
                                                    }));
                                                    break;
                                                case 1700:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_17;
                                                    }));
                                                    break;
                                                case 1800:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_18;
                                                    }));
                                                    break;
                                                case 1900:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_19;
                                                    }));
                                                    break;
                                                case 2000:
                                                    this.BeginInvoke(new MethodInvoker(delegate ()
                                                    {
                                                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_20;
                                                    }));
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        main_thread_check_general_conenction_waiter = false;
                                    }
                                }
                                else
                                {
                                    browser.BeginInvoke(new MethodInvoker(delegate ()
                                    {
                                        browser.Stop();
                                    }));
                                    if (connection_time_out >= 2000)
                                    {
                                        connection_result = -1;
                                    }
                                    else
                                    {
                                        this.BeginInvoke(new MethodInvoker(delegate ()
                                        {
                                            if (browser.IsBrowserInitialized)
                                            {
                                                browser.GetSourceAsync().ContinueWith(taskHtml =>
                                                {
                                                    var html = taskHtml.Result;

                                                    if (main_thread_check_general_conenction_mode)
                                                    {
                                                        MessageBox.Show("555-a");
                                                        if (html.ToLower().Contains("your ip is"))
                                                        {
                                                            connection_result = 2;
                                                        }
                                                        else
                                                        {
                                                            connection_result = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (html.ToLower().Contains("runescape"))
                                                        {
                                                            connection_result = 2;
                                                        }
                                                        else
                                                        {
                                                            connection_result = -1;
                                                        }
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                connection_result = -1;
                                            }
                                        }));
                                    }

                                    main_thread_check_general_conenction_init = true;
                                    main_thread_check_general_conenction = false;
                                    connection_time_out = 0;
                                }

                            }

                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("Error Occured: " + e.Message);
                    }




                    Thread.Sleep(10);
                }

            });
            mainThread.Start();








        }

        private async void button_start_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                browser.Stop();

                if (checkbox_parent.Checked)
                {
                    //Parent Proxy
                    if (textbox_pp_host.Text.Length > 4 && (textbox_pp_host.Text.Contains(".") || textbox_pp_host.Text.Contains(":")) && textbox_pp_host.Text != "Enter host...")
                    {
                        if (textbox_pp_port.Text.Length > 0 && textbox_pp_port.Text != "Enter port..." && textbox_pp_port.Text.Length < 6)
                        {
                            await CefEnableProxy(textbox_pp_host.Text, textbox_pp_port.Text);
                        }
                        else
                        {
                            await CefDisableProxy();
                        }
                    }
                    else
                    {
                        await CefDisableProxy();
                    }
                }
                else
                {
                    //User Proxy
                    if (checkbox_account_proxy.Checked)
                    {
                        if (!string.IsNullOrWhiteSpace(textbox_host.Text) && textbox_host.Text != "none" && !string.IsNullOrWhiteSpace(textbox_port.Text) && textbox_port.Text != "none")
                        {
                            await CefEnableProxy(textbox_host.Text, textbox_port.Text);
                        }
                        else
                        {
                            await CefDisableProxy();
                        }
                    }
                    else
                    {
                        await CefDisableProxy();
                    }
                }

                can_action = false;

                loading.Visible = true;
                timecounter.Visible = true;
                timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_0;
                placeholder.Visible = true;

                connection_result = 1;
                main_thread_check_general_conenction = true;
                main_thread_check_general_conenction_mode = false;

                Thread proxyTask = new Thread(() =>
                {

                    while (connection_result == 1) Thread.Sleep(10);

                    if (connection_result == -1)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate ()
                        {
                            loading.Visible = false;
                            timecounter.Visible = false;
                        }));
                        MessageBox.Show("Can't connect to OSRS Sign-Up! Check the URL in the \"" + Directory.GetCurrentDirectory() + @"\Settings.txt" + "\" file or the proxy you connecting with (if you use one). Proxy is propably blacklisted.");
                    }
                    else if (connection_result == 2)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate ()
                        {
                            loading.Visible = false;
                            timecounter.Visible = false;
                            placeholder.Visible = false;
                        }));
                    }

                    can_action = true;
                });

                proxyTask.Start();
            }

        }

        private void button_end_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;
                if (textbox_email.Text.Length > 5 && textbox_email.Text.Contains("@") && textbox_email.Text.Contains("."))
                {
                    if (textbox_password.Text.Length >= 5 && textbox_password.Text.Length <= 20 && isValidPassword(textbox_password.Text))
                    {
                        if (textbox_name.Text.Length >= 3 && textbox_name.Text.Length <= 12 && isValidUsername(textbox_password.Text))
                        {
                            if ((combo_parameter_a.SelectedIndex > -1 && combo_parameter_a.SelectedIndex < combo_parameter_a.Items.Count) || !checkbox_parameter_a.Checked)
                            {
                                if ((combo_parameter_b.SelectedIndex > -1 && combo_parameter_b.SelectedIndex < combo_parameter_b.Items.Count) || !checkbox_parameter_b.Checked)
                                {
                                    if ((combo_parameter_c.SelectedIndex > -1 && combo_parameter_c.SelectedIndex < combo_parameter_c.Items.Count) || !checkbox_parameter_c.Checked)
                                    {
                                        if (textbox_birthdate.Text.Contains("."))
                                        {
                                            if (textbox_birthdate.Text.Split('.').Length == 3)
                                            {
                                                bool alright = false;
                                                try
                                                {
                                                    DateTime.Parse(textbox_birthdate.Text);
                                                    alright = true;
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show("Birthdate error on parsing, something is wrong: \n" + ex.Message);
                                                }

                                                if (alright)
                                                {
                                                    created_at_least_one_account = true;
                                                    string[] AllAccounts = File.ReadAllLines(dir_accounts + @"\GeneratedAccounts.txt");

                                                    List<string> AccountOutput = new List<string>();

                                                    for(int i = 0; i < pattern.Count; i++)
                                                    {
                                                        string tmp = pattern[i];
                                                        bool check_trigger = true;

                                                        if (tmp.Contains("[") && tmp.Contains("]"))
                                                        {
                                                            string last_part = tmp.Substring(tmp.IndexOf('['), tmp.Length - tmp.IndexOf('['));

                                                            string left_part = tmp.Substring(0, tmp.IndexOf('['));
                                                            string right_part = last_part.Substring(last_part.IndexOf(']') + 1, last_part.Length - last_part.IndexOf(']') - 1);
                                                            string payload_part = last_part.Substring(0, last_part.IndexOf(']') + 1);

                                                            payload_part = payload_part.Replace("[", "").Replace("]", "");

                                                            switch (payload_part)
                                                            {
                                                                case "checkbox_parameter_a":
                                                                    check_trigger = checkbox_parameter_a.Checked;
                                                                    break;
                                                                case "checkbox_parameter_b":
                                                                    check_trigger = checkbox_parameter_b.Checked;
                                                                    break;
                                                                case "checkbox_parameter_c":
                                                                    check_trigger = checkbox_parameter_c.Checked;
                                                                    break;
                                                                case "checkbox_pin":
                                                                    check_trigger = checkbox_pin.Checked;
                                                                    break;
                                                                case "checkbox_proxy":
                                                                    check_trigger = checkbox_account_proxy.Checked;
                                                                    break;
                                                            }
                                                            tmp = left_part + (check_trigger ? right_part : "");
                                                        }

                                                        if(tmp.Contains("{") && tmp.Contains("}"))
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(textbox_email.Text)) tmp = tmp.Replace("{textbox_email}", textbox_email.Text);
                                                            if (!string.IsNullOrWhiteSpace(textbox_name.Text)) tmp = tmp.Replace("{textbox_name}", textbox_name.Text);
                                                            if (!string.IsNullOrWhiteSpace(textbox_password.Text)) tmp = tmp.Replace("{textbox_password}", textbox_password.Text);
                                                            if (!string.IsNullOrWhiteSpace(textbox_pin.Text)) tmp = tmp.Replace("{textbox_pin}", textbox_pin.Text);
                                                            if (combo_parameter_a.SelectedIndex > -1) if (!string.IsNullOrWhiteSpace(combo_parameter_a.SelectedItem.ToString())) tmp = tmp.Replace("{textbox_parameter_a}", combo_parameter_a.SelectedItem.ToString());
                                                            if (combo_parameter_b.SelectedIndex > -1) if (!string.IsNullOrWhiteSpace(combo_parameter_b.SelectedItem.ToString())) tmp = tmp.Replace("{textbox_parameter_b}", combo_parameter_b.SelectedItem.ToString());
                                                            if (combo_parameter_c.SelectedIndex > -1) if (!string.IsNullOrWhiteSpace(combo_parameter_c.SelectedItem.ToString())) tmp = tmp.Replace("{textbox_parameter_c}", combo_parameter_c.SelectedItem.ToString());
                                                            if (!string.IsNullOrWhiteSpace(textbox_birthdate.Text)) tmp = tmp.Replace("{textbox_birthdate}", textbox_birthdate.Text);
                                                            if (!string.IsNullOrWhiteSpace(textbox_host.Text)) tmp = tmp.Replace("{textbox_proxy_host}", textbox_host.Text);
                                                            if (!string.IsNullOrWhiteSpace(textbox_port.Text)) tmp = tmp.Replace("{textbox_proxy_port}", textbox_port.Text);
                                                            if (!string.IsNullOrWhiteSpace(textbox_username.Text)) tmp = tmp.Replace("{textbox_proxy_username}", textbox_username.Text);
                                                            if (!string.IsNullOrWhiteSpace(textbox_ppassword.Text)) tmp = tmp.Replace("{textbox_proxy_password}", textbox_ppassword.Text);
                                                        }

                                                        if(!string.IsNullOrWhiteSpace(tmp)) AccountOutput.Add(tmp);
                                                    }

                                                    if(AccountOutput.Count > 0)
                                                    {
                                                        string[] acc_data = AccountOutput.ToArray();
                                                        File.AppendAllLines(dir_accounts + @"\GeneratedAccounts.txt", acc_data);
                                                        File.AppendAllLines(dir_accounts + @"\Accounts_Session_" + session.ToString() + ".txt", acc_data);
                                                    }

                                                    list_accounts.RemoveAt(index_to_remove);
                                                    textbox_email.Text = "";
                                                    textbox_name.Text = "";
                                                    textbox_password.Text = "";
                                                    textbox_pin.Text = "";
                                                    textbox_birthdate.Text = "";
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Birthdate not long enough!");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Birthdate has to be in EU format!");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("No skill selected!");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No fate selected!");
                                }
                            }
                            else
                            {
                                MessageBox.Show("No behaviour selected!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Username isn't valid!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password isn't valid!");
                    }
                }
                else
                {
                    MessageBox.Show("Email isn't valid!");
                }
                can_action = true;
            }
        }

        private void button_random_name_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;

                FileInfo[] dir_account_info = (new DirectoryInfo(dir_word_lists)).GetFiles();
                foreach (FileInfo fi in dir_account_info)
                {
                    if (fi.Name.Contains("GeneratedNames"))
                    {
                        if (fi.Length != old_account_size)
                        {
                            string[] tmp_accounts = File.ReadAllLines(dir_word_lists + @"\GeneratedNames.txt");
                            if (tmp_accounts.Length > 0) list_accounts = tmp_accounts.ToList<string>();
                            old_account_size = fi.Length;
                            break;
                        }
                    }
                }

                if (list_accounts.Count > 0)
                {

                    Random rand = new Random();

                    string email = "";
                    string name = "";
                    string password = "";
                    string pin = "none";
                    string birthdate = "";

                    int name_index = rand.Next(0, list_accounts.Count);

                    name = list_accounts[name_index];

                    index_to_remove = name_index;

                    email = name.Replace(" ", ".").Replace("-", ".") + (string.IsNullOrWhiteSpace(textbox_trashmail.Text) ? "@mytrashmailer.com" : "@" + textbox_trashmail.Text);

                    for (int i = 0; i < 16; i++)
                    {
                        password = password + DictionaryPassword[rand.Next(0, 62)];
                    }

                    birthdate = rand.Next(10, 22).ToString() + "." + rand.Next(10, 13).ToString() + "." + rand.Next(1975, 1998).ToString();

                    if (checkbox_pin.Checked)
                    {
                        pin = "";
                        for (int i = 0; i < 4; i++)
                        {
                            pin = pin + rand.Next(0, 10);
                        }
                    }

                    textbox_email.Text = email;
                    textbox_name.Text = name;
                    textbox_password.Text = password;
                    textbox_pin.Text = pin;
                    textbox_birthdate.Text = birthdate;
                }
                else
                {
                    MessageBox.Show("No account names found, generate some! (Button in the left bottom corner)");
                }
                can_action = true;
            }
        }

        private void button_random_fate_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;
                Random rand = new Random();

                combo_parameter_a.SelectedIndex = rand.Next(0, combo_parameter_a.Items.Count);
                combo_parameter_b.SelectedIndex = rand.Next(0, combo_parameter_b.Items.Count);
                combo_parameter_c.SelectedIndex = rand.Next(0, combo_parameter_c.Items.Count);
                can_action = true;
            }
        }

        private void button_random_proxy_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;
                FileInfo[] dir_proxy_info = (new DirectoryInfo(dir_proxys)).GetFiles();
                foreach (FileInfo fi in dir_proxy_info)
                {
                    if (fi.Name.Contains("Proxys"))
                    {
                        if (fi.Length != old_proxy_size)
                        {
                            string[] tmp_proxys = File.ReadAllLines(dir_proxys + @"\Proxys.txt");
                            if (tmp_proxys.Length > 0) list_proxys = tmp_proxys.ToList<string>();
                            old_proxy_size = fi.Length;
                        }
                        break;
                    }
                }

                string Host = "none";
                string Port = "none";
                string Username = "none";
                string Password = "none";

                if (checkbox_account_proxy.Checked)
                {
                    if (list_proxys.Count > 0)
                    {
                        Random rand = new Random();

                        string tmp_proxy = list_proxys[rand.Next(0, list_proxys.Count)];

                        if (tmp_proxy.Length > 8)
                        {
                            if (tmp_proxy.Contains(":"))
                            {
                                string[] tmp_proxy_split = tmp_proxy.Split(':');
                                if (tmp_proxy_split.Length > 0) Host = tmp_proxy_split[0];
                                if (tmp_proxy_split.Length > 1) Port = tmp_proxy_split[1];
                                if (tmp_proxy_split.Length > 2) Username = tmp_proxy_split[2];
                                if (tmp_proxy_split.Length > 3) Password = tmp_proxy_split[3];
                            }
                        }
                    }

                }

                textbox_host.Text = Host;
                textbox_port.Text = Port;
                textbox_username.Text = Username;
                textbox_ppassword.Text = Password;

                can_action = true;
            }
        }

        private async void button_fill_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;
                if (osrs_connected)
                {
                    if (browser.CanExecuteJavascriptInMainFrame)
                    {
                        string js_script = "";
                        if (!string.IsNullOrWhiteSpace(textbox_email.Text)) js_script = js_script + "document.getElementById('create-email').value = '" + textbox_email.Text + "';";
                        if (!string.IsNullOrWhiteSpace(textbox_password.Text)) js_script = js_script + "document.getElementById('create-password').value = '" + textbox_password.Text + "';";
                        if (!string.IsNullOrWhiteSpace(textbox_birthdate.Text)) if (textbox_birthdate.Text.Contains(".")) if (textbox_birthdate.Text.Split('.').Length > 0) js_script = js_script + "document.getElementsByName('day')[0].value = '" + textbox_birthdate.Text.Split('.')[0] + "';";
                        if (!string.IsNullOrWhiteSpace(textbox_birthdate.Text)) if (textbox_birthdate.Text.Contains(".")) if (textbox_birthdate.Text.Split('.').Length > 1) js_script = js_script + "document.getElementsByName('month')[0].value = '" + textbox_birthdate.Text.Split('.')[1] + "';";
                        if (!string.IsNullOrWhiteSpace(textbox_birthdate.Text)) if (textbox_birthdate.Text.Contains(".")) if (textbox_birthdate.Text.Split('.').Length > 2) js_script = js_script + "document.getElementsByName('year')[0].value = '" + textbox_birthdate.Text.Split('.')[2] + "';";
                        js_script = js_script + "document.getElementsByName('agree_terms')[0].checked = true;";

                        JavascriptResponse rsp = await browser.EvaluateScriptAsync(js_script);
                    }
                    else
                    {
                        MessageBox.Show("Can't execute JS :(");
                    }
                }
                can_action = true;
            }
        }



        public void Textbox_Enter(object sender, EventArgs e)
        {
            TextBox me = (sender as TextBox);

            string name = me.Name;

            switch (name)
            {
                case "textbox_pp_host":
                    if(me.Text == "Enter host...")
                    {
                        me.ForeColor = Color.Black;
                        me.Text = "";
                        parent_host = "";
                    }
                    break;
                case "textbox_pp_port":
                    if (me.Text == "Enter port...")
                    {
                        me.ForeColor = Color.Black;
                        me.Text = "";
                        parent_port = "";
                    }
                    break;
                case "textbox_pp_name":
                    if (me.Text == "Enter username...")
                    {
                        me.ForeColor = Color.Black;
                        me.Text = "";
                        parent_name = "";
                    }
                    break;
                case "textbox_pp_password":
                    if (me.Text == "Enter password...")
                    {
                        me.ForeColor = Color.Black;
                        me.Text = "";
                        parent_password = "";
                    }
                    break;
            }
        }

        public void Textbox_Leave(object sender, EventArgs e)
        {
            TextBox me = (sender as TextBox);

            string name = me.Name;

            switch (name)
            {
                case "textbox_pp_host":
                    if (string.IsNullOrWhiteSpace(me.Text))
                    {
                        me.ForeColor = Color.LightGray;
                        me.Text = "Enter host...";
                        parent_host = "";
                    }
                    else
                    {
                        parent_host = me.Text;
                    }
                    break;
                case "textbox_pp_port":
                    if (string.IsNullOrWhiteSpace(me.Text))
                    {
                        me.ForeColor = Color.LightGray;
                        me.Text = "Enter port...";
                        parent_port = "";
                    }
                    else
                    {
                        parent_port = me.Text;
                    }
                    break;
                case "textbox_pp_name":
                    if (string.IsNullOrWhiteSpace(me.Text))
                    {
                        me.ForeColor = Color.LightGray;
                        me.Text = "Enter username...";
                        parent_name = "";
                    }
                    else
                    {
                        parent_name = me.Text;
                    }
                    break;
                case "textbox_pp_password":
                    if (string.IsNullOrWhiteSpace(me.Text))
                    {
                        me.ForeColor = Color.LightGray;
                        me.Text = "Enter password...";
                        parent_password = "";
                    }
                    else
                    {
                        parent_password = me.Text;
                    }
                    break;
            }
        }

        public bool isValidPassword(string password)
        {
            foreach(char c in password)
            {
                byte code = (byte)c;
                if( !((code >= 48 && code <= 57) || (code >= 65 && code <= 90) || (code >= 97 && code <= 122)))
                {
                    return false;
                }
            }
            return true;
        }
        public bool isValidUsername(string name)
        {
            foreach (char c in name)
            {
                byte code = (byte)c;
                if (!((code >= 48 && code <= 57) || (code >= 65 && code <= 90) || (code >= 97 && code <= 122) || code == 23 || code == 45))
                {
                    return false;
                }
            }
            return true;
        }

        private async void button_checkproxy_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                browser.Stop();

                if (textbox_pp_host.Text.Length > 4 && (textbox_pp_host.Text.Contains(".") || textbox_pp_host.Text.Contains(":")) && textbox_pp_host.Text != "Enter host...")
                {
                    if (textbox_pp_port.Text.Length > 0 && textbox_pp_port.Text != "Enter port..." && textbox_pp_port.Text.Length < 6)
                    {
                        await CefEnableProxy(textbox_pp_host.Text, textbox_pp_port.Text);

                        osrs_connected = false;
                        loading.Visible = true;
                        timecounter.Visible = true;
                        timecounter.Image = OSRS_Account_Creator.Properties.Resources.num_0;
                        placeholder.Visible = true;

                        can_action = false;

                        connection_result = 1;
                        main_thread_check_general_conenction = true;
                        main_thread_check_general_conenction_mode = true;
                        Thread proxyTask = new Thread(() =>
                        {

                            while (connection_result == 1) Thread.Sleep(10);

                            if (connection_result == -1)
                            {
                                this.BeginInvoke(new MethodInvoker(delegate ()
                                {
                                    CefDisableProxy();
                                    loading.Visible = false;
                                    timecounter.Visible = false;
                                }));
                            }
                            else if (connection_result == 2)
                            {
                                this.BeginInvoke(new MethodInvoker(delegate ()
                                {
                                    CefDisableProxy();
                                    loading.Visible = false;
                                    timecounter.Visible = false;
                                    placeholder.Visible = false;
                                }));
                            }


                            can_action = true;
                            MessageBox.Show(connection_result == -1 ? "Connection Error: Proxy can't connect!" : "Proxy Connected!");
                        });

                        proxyTask.Start();

                    }
                    else
                    {
                        if (textbox_pp_host.Text.Length < 6)
                        {
                            MessageBox.Show("Parent Proxy Port: Too long!");
                        }
                        else
                        {
                            MessageBox.Show("Parent Proxy Port: No port typed in!");
                        }
                    }
                }
                else
                {
                    if (textbox_pp_host.Text == "Enter host...")
                    {
                        MessageBox.Show("Parent Proxy Host: No host typed in!");
                    }
                    else
                    {
                        MessageBox.Show("Parent Proxy Host: Not valid! Propably a typo!");
                    }
                }
            }

        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Write save
            save_state[0] = checkbox_pin.Checked ? "1" : "0";
            save_state[1] = checkbox_parameter_a.Checked ? "1" : "0";
            save_state[2] = checkbox_parameter_b.Checked ? "1" : "0";
            save_state[3] = checkbox_parameter_c.Checked ? "1" : "0";
            save_state[4] = checkbox_account_proxy.Checked ? "1" : "0";
            save_state[5] = checkbox_parent.Checked ? "1" : "0";

            if (!string.IsNullOrWhiteSpace(textbox_pp_host.Text) && textbox_pp_host.Text != "Enter host...") { save_state[6] = textbox_pp_host.Text; } else { save_state[6] = "-"; }
            if (!string.IsNullOrWhiteSpace(textbox_pp_port.Text) && textbox_pp_port.Text != "Enter port...") { save_state[7] = textbox_pp_port.Text; } else { save_state[7] = "-"; }
            if (!string.IsNullOrWhiteSpace(textbox_pp_name.Text) && textbox_pp_name.Text != "Enter username...") { save_state[8] = textbox_pp_name.Text; } else { save_state[8] = "-"; }
            if (!string.IsNullOrWhiteSpace(textbox_pp_password.Text) && textbox_pp_password.Text != "Enter password...") { save_state[9] = textbox_pp_password.Text; } else { save_state[9] = "-"; }
            if (!string.IsNullOrWhiteSpace(textbox_trashmail.Text)) { save_state[10] = textbox_trashmail.Text; } else { save_state[10] = "-"; }

            //Tutorial Message
            save_state[11] = "0";
            
            string[] save_array = new string[7] { save_state[0] + "," + save_state[1] + "," + save_state[2] + "," + save_state[3] + "," + save_state[4] + "," + save_state[5], save_state[6], save_state[7], save_state[8], save_state[9], save_state[10], save_state[11] };

            File.WriteAllLines(dir_config + @"\Save.sav", save_array);

            //Overwrite lists
            if (created_at_least_one_account)
            {
                File.WriteAllLines(dir_word_lists + @"\GeneratedNames.txt", list_accounts.ToArray());
            }

            main_thread_run = false;
            Thread.Sleep(250);
            if (mainThread.IsAlive) mainThread.Abort();
            Cef.Shutdown();
            Environment.Exit(0);
        }



        public async Task<bool> CefEnableProxy(string server, string port, string user = "", string password = "")
        {
            await Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var rc = browser.GetBrowser().GetHost().RequestContext;
                var dict = new Dictionary<string, object>();
                dict.Add("mode", "fixed_servers");
                dict.Add("server", "" + server + ":" + port + "");
                string error;
                bool success = rc.SetPreference("proxy", dict, out error);
            });

            return true;
        }

        public async Task<bool> CefDisableProxy()
        {
            await Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var rc = browser.GetBrowser().GetHost().RequestContext;
                var dict = new Dictionary<string, object>();
                dict.Add("mode", "direct");
                string error;
                bool success = rc.SetPreference("proxy", dict, out error);
            });

            return true;
        }

        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser();
            this.Controls.Add(browser);
            browser.LoadUrl("https://google.com");
            browser.Visible = false;
            browser.CreateControl();
            browser.Location = new Point(12, 39);
            browser.Size = new Size(915, 525);
            browser.Visible = true;

        }



        private void button_account_dir_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = dir_accounts;
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string result = dialog.FileName;
                    if (result.Contains("."))
                    {
                        if (result.Split('.').Length == 2)
                        {
                            result = result.Replace("\\" + result.Split('\\')[result.Split('\\').Length - 1], "");
                        }
                    }
                    if (Directory.Exists(result))
                    {
                        dir_accounts = result;
                        textbox_account_dir.Text = dir_accounts;

                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Settings.txt"))
                        {
                            string[] lines = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\Settings.txt");
                            lines[0] = dir_accounts;
                            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\Settings.txt", lines);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Directory \"" + result + "\" couldn't be found!");
                    }
                }
                can_action = true;
            }
        }
        private void button_proxy_dir_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = dir_proxys;
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string result = dialog.FileName;
                    if (result.Contains("."))
                    {
                        if (result.Split('.').Length == 2)
                        {
                            result = result.Replace("\\" + result.Split('\\')[result.Split('\\').Length - 1], "");
                        }
                    }
                    if (Directory.Exists(result))
                    {
                        dir_proxys = result;
                        textbox_wordlist_dir.Text = dir_proxys;

                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Settings.txt"))
                        {
                            string[] lines = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\Settings.txt");
                            lines[1] = dir_proxys;
                            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\Settings.txt", lines);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Directory \"" + result + "\" couldn't be found!");
                    }
                }
                can_action = true;
            }
        }

        private void button_wordlist_dir_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = dir_word_lists;
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string result = dialog.FileName;
                    if (result.Contains("."))
                    {
                        if (result.Split('.').Length == 2)
                        {
                            result = result.Replace("\\" + result.Split('\\')[result.Split('\\').Length - 1], "");
                        }
                    }
                    if (Directory.Exists(result))
                    {
                        dir_word_lists = result;
                        textbox_proxy_dir.Text = dir_word_lists;

                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Settings.txt"))
                        {
                            string[] lines = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\Settings.txt");
                            lines[2] = dir_word_lists;
                            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\Settings.txt", lines);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Directory \"" + result + "\" couldn't be found!");
                    }
                }
                can_action = true;
            }
        }

        private void button_generate_names_Click(object sender, EventArgs e)
        {
            if (can_action)
            {
                can_action = false;

                FileInfo[] dir_wordlist_info = (new DirectoryInfo(dir_word_lists)).GetFiles();
                foreach (FileInfo fi in dir_wordlist_info)
                {
                    if (fi.Name.Contains("Wordlist_A"))
                    {
                        if (fi.Length != old_wordlist_a_size)
                        {
                            string[] tmp_wordlist = File.ReadAllLines(dir_word_lists + @"\Wordlist_A.txt");
                            if (tmp_wordlist.Length > 0) list_wordlist_a = tmp_wordlist.ToList<string>();
                            old_wordlist_a_size = fi.Length;
                        }
                    }
                    if (fi.Name.Contains("Wordlist_B"))
                    {
                        if (fi.Length != old_wordlist_b_size)
                        {
                            string[] tmp_wordlist = File.ReadAllLines(dir_word_lists + @"\Wordlist_B.txt");
                            if (tmp_wordlist.Length > 0) list_wordlist_b = tmp_wordlist.ToList<string>();
                            old_wordlist_b_size = fi.Length;
                        }
                    }
                }

                if (list_wordlist_a.Count > 0 && list_wordlist_b.Count > 0)
                {
                    List<string> tmp_accounts = new List<string>();

                    foreach (string word_a in list_wordlist_a)
                    {
                        foreach (string word_b in list_wordlist_b)
                        {
                            if (tmp_accounts.Count < 2000000)
                            {
                                List<string> tmp_sublist = new List<string>();

                                Random rand = new Random();

                                string tmp_word_a = word_a;
                                string tmp_word_b = word_b;

                                if (tmp_word_a.Length + tmp_word_b.Length > 12)
                                {
                                    while (tmp_word_a.Length + tmp_word_b.Length > 12)
                                    {
                                        tmp_word_a = tmp_word_a.Remove(rand.Next(0, tmp_word_a.Length), 1);
                                        if (tmp_word_a.Length + tmp_word_b.Length > 12)
                                        {
                                            tmp_word_b = tmp_word_b.Remove(rand.Next(0, tmp_word_b.Length), 1);
                                        }
                                    }
                                }

                                if (tmp_word_a.Length + tmp_word_b.Length < 12)
                                {
                                    tmp_sublist.Add(tmp_word_a + " " + tmp_word_b);
                                    tmp_sublist.Add(tmp_word_b + " " + tmp_word_a);
                                    tmp_sublist.Add(tmp_word_a + "-" + tmp_word_b);
                                    tmp_sublist.Add(tmp_word_b + "-" + tmp_word_a);
                                }
                                else
                                {
                                    tmp_sublist.Add(tmp_word_a + tmp_word_b);
                                    tmp_sublist.Add(tmp_word_b + tmp_word_b);
                                }

                                int counter = tmp_sublist.Count;
                                for (int i = 0; i < counter; i++)
                                {
                                    if (tmp_sublist[i].ToLower().Contains("a") || tmp_sublist[i].ToLower().Contains("e") || tmp_sublist[i].ToLower().Contains("i") || tmp_sublist[i].ToLower().Contains("o"))
                                    {
                                        tmp_sublist.Add(tmp_sublist[i].Replace("A", "4").Replace("a", "4").Replace("E", "3").Replace("e", "3").Replace("I", "1").Replace("i", "1").Replace("O", "0").Replace("o", "0"));
                                    }
                                }

                                foreach (string tec in tmp_sublist)
                                {
                                    tmp_accounts.Add(tec);
                                }

                                int num_space = 12 - tmp_sublist[0].Length;
                                if (num_space > 0)
                                {
                                    string tmp_factor = "";
                                    for (int i = 0; i < num_space; i++) tmp_factor = tmp_factor + "9";
                                    int factor = int.Parse(tmp_factor) + 1;
                                    for (int i = 0; i < factor; i++)
                                    {
                                        foreach (string labra in tmp_sublist)
                                        {
                                            tmp_accounts.Add(labra + i.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }

                    File.WriteAllLines(dir_word_lists + @"\GeneratedNames.txt", tmp_accounts.ToArray());
                    MessageBox.Show("Name list generated in \"" + dir_word_lists + "\\GeneratedNames.txt\"");
                }
                else
                {
                    MessageBox.Show("Not enough words in list to create a name list!");
                }

                can_action = true;
            }
        }
    }
}


