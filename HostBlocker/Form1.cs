using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Speech.Synthesis;
using System.Speech.Recognition;


namespace HostBlocker
{
    public partial class Form1 : Form
    {
        public Label cr;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        SpeechSynthesizer speech = new SpeechSynthesizer();
        SpeechRecognitionEngine spRecEngine = new SpeechRecognitionEngine();
        Choices list = new Choices();
        Stream stream;
        int GreetingId = 0;
        int WakeId = 0;
        int SleepId = 1;
        Boolean wake = false;

        string fname = Path.GetPathRoot(Environment.SystemDirectory) + @"\windows\system32\drivers\etc\hosts";
        string[] temp;
        string[] temp2;


        //SPEECH RECOGNITION
        public void Speaker()
        {
            list.Add(new String[] {
                "hello",
                "hi",
                "wake",
                "gosleep",
                "paste",
                "block now",
                "open file",
                "cleartext",
                "remove selected",
                "minimize",
                "normal state",
                "save",
                "exit",

                "Hello",
                "Hi",
                "Wake",
                "Gosleep",
                "Paste",
                "Block Now",
                "Open File",
                "Cleartext",
                "Remove Selected",
                "Minimize",
                "Normal State",
                "Save",
                "Exit",
            });
            Grammar grammar = new Grammar(new GrammarBuilder(list));


            try
            {
                spRecEngine.RequestRecognizerUpdate();
                spRecEngine.LoadGrammar(grammar);
                spRecEngine.SpeechRecognized += SpRecEngine_SpeechRecognized;
                spRecEngine.SetInputToDefaultAudioDevice();
                spRecEngine.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch
            {
                System.Diagnostics.Process.Start("HostBlocker.exe");
            }
            finally
            {
                Refresh();
            }
        }

        public void Bot_Return(String speak)
        {
            speech.Speak(speak);
        }
        private void SpRecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                String command = e.Result.Text;
                Random rand = new Random();
                List<string> Greeting = new List<string> { "how can i help you?", "say to me a command" };

                if (command == "wake" || command == "Wake") wake = true;
                if (command == "sleep" || command == "Sleep") wake = false;


                if (wake == true)
                {
                    SleepId = 0;

                    if (command == "wake" || command == "Wake")
                    {
                        if (WakeId == 0)
                        {
                            label1.Text = "●";
                            if (stream == null) Environment.Exit(0);
                            label1.ForeColor = Color.FromArgb(0, 160, 40);
                            Bot_Return("Awake!");
                            WakeId = 1;
                        }
                        else if (WakeId == 1)
                        {
                            Bot_Return("I am ready");
                        }
                    }

                    if (command == "hello" || command == "hi" || command == "Hello" || command == "Hi")
                    {
                        if (GreetingId == 0)
                        {
                            //Bot_Return("Hello, " + Greeting[rand.Next(2)]);
                            GreetingId = 1;
                        }
                        else
                        {
                            List<string> hello = new List<string> { "Hello, ", "Hi, ", "" };
                            Bot_Return(hello[rand.Next(3)] + Greeting[rand.Next(2)]);
                        }
                    }

                    if (command == "paste" || command == "Paste")
                    {
                        richTextBox1.Paste();
                        spRecEngine.RequestRecognizerUpdate();
                    }

                    if (command == "block now" || command == "Block Now")
                    {
                        Block();
                        spRecEngine.RequestRecognizerUpdate();
                    }
                    if (command == "open file" || command == "Open File")
                    {
                        OpenFile();
                    }
                    if (command == "cleartext" || command == "Cleartext")
                    {
                        ClearText();
                        spRecEngine.RequestRecognizerUpdate();
                    }
                    if (command == "remove selected" || command == "Remove Selected")
                    {
                        richTextBox1.SelectedText = "";
                        spRecEngine.RequestRecognizerUpdate();
                    }

                    if (command == "save" || command == "Save")
                    {
                        Save();
                        spRecEngine.RequestRecognizerUpdate();
                    }
                    if (command == "exit" || command == "Exit")
                    {
                        Bot_Return("Good Bye");
                        Environment.Exit(0);
                    }

                }

                if (command == "minimize" || command == "Minimize")
                {
                    this.WindowState = FormWindowState.Minimized;
                    spRecEngine.RequestRecognizerUpdate();
                }

                if (command == "normal state" || command == "Normal State")
                {
                    this.WindowState = FormWindowState.Normal;
                    spRecEngine.RequestRecognizerUpdate();
                }


                if (command == "gosleep" || command == "Gosleep")
                {
                    if (SleepId == 0)
                    {
                        label1.Text = "●";
                        label1.ForeColor = Color.FromArgb(160, 0, 40);
                        Bot_Return("Okay!");
                        wake = false;
                    }
                    SleepId = 1;
                    GreetingId = 0;
                    WakeId = 0;
                }
            }
            catch
            {
                Refresh();
            }
            finally
            {
                Refresh();
            }

        }

        //ACTIONS
        public void Block()
        {
            try
            {
                if (string.IsNullOrEmpty(richTextBox1.Text))
                {
                    onChanges("გთხოვთ შეიყვანოთ მისამართი", default);
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(fname, true))
                    {
                        sw.WriteLine("");
                        sw.WriteLine("#HostBlocker");
                        for (int i = 0; i < richTextBox1.Lines.Length; i++)
                        {
                            if (string.IsNullOrEmpty(richTextBox1.Lines[i]))
                            {

                            }
                            else
                            {
                                sw.WriteLine("127.0.0.1" + "    " + richTextBox1.Lines[i]);
                            }
                        }
                        onChanges("საიტები დაიბლოკა წარმატებით", Color.FromArgb(0, 160, 40));
                    }
                    temp = File.ReadAllLines(fname);
                    temp2 = richTextBox1.Lines;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void OpenFile()
        {
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                richTextBox1.Lines = File.ReadAllLines(fname);
                temp2 = richTextBox1.Lines;
            }
            else
            {
                string message = "ტექსტური უჯრა არ არის ცარიელი, გთხოვთ დარწმუნდით რომ ლინკების ბლოკირება " +
                    "განხორციელებულია, host ფაილის გასახსნელად საჭიროა ტექსტური ფანჯრის გასუფთავება თქვენიდან მოხდეს.";
                onChanges(message, default);
            }
            spRecEngine.RequestRecognizerUpdate();
        }
        public void ClearText()
        {
            richTextBox1.Clear();
        }
        public void Save()
        {
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                onChanges("ტექსტური ველი არ შეიძლება იყოს ცარიელი", default);
            }
            else if (temp2 == null || temp2.Length != temp.Length)
            {
                onChanges("გთხოვთ დარწმუნდეთ რომ გახსნილი გაქვთ host ფაილი", default);
            }
            else
            {
                try
                {
                    File.WriteAllLines(fname, richTextBox1.Lines);

                    onChanges("შენახულია წარმატებით", Color.FromArgb(0, 160, 40));

                    temp = File.ReadAllLines(fname);
                    temp2 = richTextBox1.Lines;
                }
                catch { }
            }
        }

        //MAIN CONSTRUCTOR
        public Form1()
        {
            Speaker();
            speech.SelectVoiceByHints(VoiceGender.Female);
            //speech.Speak("Hello, My name is HostBlocker, howday!");
            InitializeComponent();
            SetTransparencyControls();
        }

        //ExecuteImageTransparencyAfterInitializedComponents
        public void SetTransparencyControls()
        {
            //FormControlBox
            pictureBox2.Controls.Add(pictureBox3);
            cr = new Label();
            pictureBox3.Location = new Point(5, 6 / 2);
            pictureBox3.BackColor = Color.Transparent;

            pictureBox2.Controls.Add(pictureBox4);
            pictureBox4.Location = new Point(35, 6 / 2);
            pictureBox4.BackColor = Color.Transparent;

            //Textbox
            pictureBox5.Controls.Add(pictureBox6);
            pictureBox6.Location = new Point(3, 3);
            pictureBox6.BackColor = Color.Transparent;

            //ExplorerBox
            pictureBox7.Controls.Add(pictureBox8);
            pictureBox8.Location = new Point(3, 3);
            pictureBox8.BackColor = Color.Transparent;

            pictureBox7.Controls.Add(label2);
            label2.Location = new Point(3, 23);
            label2.BackColor = Color.Transparent;
            label2.ForeColor = Color.White;

            pictureBox7.Controls.Add(pictureBox15);
            pictureBox15.Location = new Point(23 / 2, 303);
            pictureBox15.BackColor = Color.Transparent;

            //ControlPanel
            pictureBox10.Controls.Add(pictureBox9);
            pictureBox9.Location = new Point(405, 26);
            pictureBox9.BackColor = Color.Transparent;

            pictureBox10.Controls.Add(pictureBox11);
            pictureBox11.Location = new Point(10, 26);
            pictureBox11.BackColor = Color.Transparent;

            pictureBox10.Controls.Add(pictureBox12);
            pictureBox12.Location = new Point(109, 26);
            pictureBox12.BackColor = Color.Transparent;

            pictureBox10.Controls.Add(pictureBox13);
            pictureBox13.Location = new Point(208, 26);
            pictureBox13.BackColor = Color.Transparent;

            pictureBox10.Controls.Add(pictureBox14);
            pictureBox14.Location = new Point(307, 26);
            pictureBox14.BackColor = Color.Transparent;

        }

        //Author
        public void CR()
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                stream = client.OpenRead("https://raw.githubusercontent.com/ioane-stacks/AuthorName/master/auth.txt");
                using (StreamReader str = new StreamReader(stream, true))
                {
                    this.Controls.Add(cr);
                    cr.Location = new Point(720, 470);
                    cr.AutoSize = true;
                    cr.Font = new Font("Microsoft Sans Serif", 9.5F);
                    cr.ForeColor = Color.FromArgb(255, 55, 55, 55);
                    cr.Text = str.ReadLine();
                    cr.Click += new EventHandler(cr_Click);
                    cr.MouseHover += new EventHandler(cr_MouseHover);
                }
            }
            catch
            {

            }

        }
        private void cr_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/IoaneStacks/");
        }
        private void cr_MouseHover(object sender, EventArgs e)
        {
            cr.Cursor = Cursors.Hand;
        }

        //MESSAGES
        private void onGuid(string message)
        {
            label2.ForeColor = Color.White;
            label2.Text = message;
        }
        private void onChanges(string message, Color? c)
        {
            if (c == null) c = Color.FromArgb(160, 0, 40);
            label2.ForeColor = c.Value;
            label2.Text = message;
        }

        //Start
        private void Form1_Load(object sender, EventArgs e)
        {
            temp = File.ReadAllLines(fname);
            label2.MaximumSize = new System.Drawing.Size(pictureBox7.Width - 5, default);
            CR();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        //RichTextBox
        #region richtextbox
        private void richTextBox1_MouseHover(object sender, EventArgs e)
        {
            string message = "ტექსტური ფანჯრის გამოყენება შეგიძლიათ ჰოსტების დასაბლოკად, " +
                "თუ გსურთ ერთი ან რამდენიმე ჰოსტის დაბლოკვა, ტექსტურ ფანჯარაში ჩაწერეთ \n" +
                "მაგ: www.example.com \n" +
                "ან: example.com \n" +
                "ასევე შესაძლებელია ტექსტური ფანჯრიდან Host ფაილის რედაქტირება სადაც განთავსებულია " +
                "აწ. დაბლოკილი ლინკები, ფაილზე ორიენტირებისთვის დააკლიკეთ OPEN ღილაკს.";
            onGuid(message);
        }

        private void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            label2.Text = "";
        }

        #endregion

        //FormMinimize
        #region FormMinimize
        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            pictureBox3.Image = new Bitmap(HostBlocker.Properties.Resources.MinimizeBtn2);
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.Image = new Bitmap(HostBlocker.Properties.Resources.MinimizeBtn);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        //FormExit
        #region FormExit
        private void pictureBox4_MouseHover(object sender, EventArgs e)
        {
            pictureBox4.Image = new Bitmap(HostBlocker.Properties.Resources.ExitBtn2);
        }

        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
            pictureBox4.Image = new Bitmap(HostBlocker.Properties.Resources.ExitBtn);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        //ToggleExplorer
        #region toggleexplorer
        bool isActive = true;

        private void pictureBox9_MouseHover(object sender, EventArgs e)
        {
            if (isActive == true)
            {
                pictureBox9.Image = new Bitmap(HostBlocker.Properties.Resources.ToggleExplorerActiveHover);
            }
            else
            {
                pictureBox9.Image = new Bitmap(HostBlocker.Properties.Resources.ToggleExplorerHover);
            }
        }

        private void pictureBox9_MouseLeave(object sender, EventArgs e)
        {
            if (isActive == true)
            {
                pictureBox9.Image = new Bitmap(HostBlocker.Properties.Resources.ToggleExplorerActive);
            }
            else
            {
                pictureBox9.Image = new Bitmap(HostBlocker.Properties.Resources.ToggleExplorer);
            }
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            CollapseExplorer();
        }

        public void CollapseExplorer()
        {
            if (isActive == true)
            {
                isActive = false;
                pictureBox9.Image = new Bitmap(HostBlocker.Properties.Resources.ToggleExplorer);

                this.Width = 518;
                panel1.Left = 410;
                pictureBox7.Visible = false;
            }
            else
            {
                isActive = true;
                pictureBox9.Image = new Bitmap(HostBlocker.Properties.Resources.ToggleExplorerActive);

                this.Width = 756;
                panel1.Left = 645;
                pictureBox7.Visible = true;
            }
        }
        #endregion

        //BlockUrl
        #region blockurl
        private void pictureBox11_MouseHover(object sender, EventArgs e)
        {
            pictureBox11.Image = new Bitmap(HostBlocker.Properties.Resources.BlockUrlBtnHover);
            string message = "თუ უკვე შეყვანილი გყავთ ტექსტურ ფანჯარაში ლინკი/ები, BLOCK ღილაკის დაჭერით " +
                "შეძლებთ ამავე ლინკების დაბლოკვას.";
            onGuid(message);
        }

        private void pictureBox11_MouseLeave(object sender, EventArgs e)
        {
            pictureBox11.Image = new Bitmap(HostBlocker.Properties.Resources.BlockUrlBtn);
            label2.Text = "";
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            Block();
        }
        #endregion

        //ClearText
        #region cleartext
        private void pictureBox12_MouseHover(object sender, EventArgs e)
        {
            pictureBox12.Image = new Bitmap(HostBlocker.Properties.Resources.ClearBtnHover);
            string message = "CLEAR ღილაკი გაგისუფთავებთ ტექსტურ ფანჯარაში ჩაწერილ ტექსტს.";
            onGuid(message);
        }

        private void pictureBox12_MouseLeave(object sender, EventArgs e)
        {
            pictureBox12.Image = new Bitmap(HostBlocker.Properties.Resources.ClearBtn);
            label2.Text = "";

        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            ClearText();
        }
        #endregion

        //OpenFile
        #region openfile
        private void pictureBox13_MouseHover(object sender, EventArgs e)
        {
            pictureBox13.Image = new Bitmap(HostBlocker.Properties.Resources.OpenFileBtnHover);
            string message = "OPEN ღილაკის დაკლიკვის შედეგად ტექსტურ უჯრაში გამოჩნდება host.txt ფაილში მოთავსებული " +
                "ტექსტი, სადაც მითითებულია ის ლინკები რომელთა დაბლოკვაც სასურველია, host ფაილი მდებარეობს\n" +
                $@"{Path.GetPathRoot(Environment.CurrentDirectory)}Windows\System32\Drivers\etc\ დირექტორიაში, " +
                "თქვენ მოგეცემათ საშუალება დაარედაქტიროთ host ფაილი";
            onGuid(message);
        }

        private void pictureBox13_MouseLeave(object sender, EventArgs e)
        {
            pictureBox13.Image = new Bitmap(HostBlocker.Properties.Resources.OpenFileBtn);
            label2.Text = "";
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        #endregion

        //SaveFile
        #region savefile
        private void pictureBox14_MouseHover(object sender, EventArgs e)
        {
            pictureBox14.Image = new Bitmap(HostBlocker.Properties.Resources.SaveFileBtnHover);
            string message = "მას შემდეგ რაც დაარედაქტირებთ host ფაილს, SAVE ღილაკის დაკლიკვით მოხდება ცვლილებების განახლება " +
                "და შენახვა.";
            onGuid(message);
        }

        private void pictureBox14_MouseLeave(object sender, EventArgs e)
        {
            pictureBox14.Image = new Bitmap(HostBlocker.Properties.Resources.SaveFileBtn);
            label2.Text = "";
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            Save();
        }
        #endregion


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Menu.Show(this.Location.X + 10, this.Location.Y + 10);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void pictureBox15_MouseHover(object sender, EventArgs e)
        {
            label2.Text =
                "wake: გააქტიურება\n" +
                "gosleep: დეაქტივაცია\n" +
                "open file: host ფაილის გახსნა\n" +
                "paste: მონიშნული ტექსტის ჩასმა\n" +
                "blok now: ლინკის დაბლოკვა\n" +
                "save: რედაქტირებულის დამახსოვრება\n" +
                "clear text: ტექსტური უჯრის გასუფთავება\n" +
                "remove selected: მონიშნული ტექსტის წაშლა\n" +
                "minimize: პროგრამის ჩაკეცვა\n" +
                "normal state: პროგრამის ამოკეცვა\n" +
                "exit: პროგრამის გათიშვა";
        }
    }
}