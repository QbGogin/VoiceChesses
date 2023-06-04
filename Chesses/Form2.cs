using NAudio.Wave;
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

namespace Chesses
{
    public partial class Form2 : Form
    {
        WaveIn waveIn;
        WaveFileWriter writer;
        public string outputFilename = "all/hods/",wav=".wav";
        public int file_index = 1;
        public Image chessSprites;
        public int[,] map = new int[8, 8]
        {
            {15,14,13,12,11,13,14,15 },
            {16,16,16,16,16,16,16,16 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {26,26,26,26,26,26,26,26 },
            {25,24,23,22,21,23,24,25 },
        };
        public Button[,] butts = new Button[8, 8];
        string[] s = new string[4];
        int motion = 0;
        public int currPlayer;
        public int but;
        public Button prevButton;
        public Button btn1, btn2, btn3, btn4;
        Dictionary<string, string> d = new Dictionary<string, string>()
        {
            {"9","A" },{"10","B" },{"11","C" },{"12","D" },{"13","E" },{"14","F" },{"15","G" },{"16","H" }
        };
        public bool isMoving = false;
        static NeuralNetwork net,net_num;
        static int[] layers = new int[3] { 960, 60, 8 };
        static int[] layers1 = new int[3] { 924, 111, 8 };
        static string[] activation = new string[2] { "logistic", "logistic" };
        public Form2()
        { 
            net = new NeuralNetwork(layers, activation);
            net_num = new NeuralNetwork(layers1, activation);
            //net.Load("../../../MLP.txt");
            //net_num.Load("../../../MLP.txt");
            InitializeComponent();
            chessSprites = new Bitmap("all/chess.png");
            Init();
        }
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<WaveInEventArgs>(waveIn_DataAvailable), sender, e);
            }
            else
            {
                //Записываем данные из буфера в файл
                writer.WriteData(e.Buffer, 0, e.BytesRecorded);
            }

        }
        void StopRecording()
        {
            waveIn.StopRecording();
            
        }
        private void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler(waveIn_RecordingStopped), sender, e);
            }
            else
            {
                if (waveIn!=null) waveIn.Dispose();
                waveIn = null;
                if (writer!=null)writer.Close();
                writer = null;
            }
        }
        public void Init()
        {
            map = new int[8, 8]
            {
            {25,24,23,22,21,23,24,25 },
            {26,26,26,26,26,26,26,26 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {16,16,16,16,16,16,16,16 },
            {15,14,13,12,11,13,14,15 },
            };
            but = 0;
            currPlayer = 1;
            CreateMap();
            CreateLable();
            btn1 = new Button();
            btn1.Size = new Size(20, 25);
            btn1.Location = new Point(500, 280);
            btn1.Enabled = false;
            btn1.Click += button1_Click_5;
            this.Controls.Add(btn1);
            btn2 = new Button();
            btn2.Size = new Size(20, 25);
            btn2.Location = new Point(525, 280);
            btn2.Enabled = false;
            btn2.Click += button1_Click_5;
            this.Controls.Add(btn2);
            btn3 = new Button();
            btn3.Size = new Size(20, 25);
            btn3.Location = new Point(550, 280);
            btn3.Enabled = false;
            btn3.Click += button1_Click_5;
            this.Controls.Add(btn3);
            btn4 = new Button();
            btn4.Size = new Size(20, 25);
            btn4.Location = new Point(575, 280);
            btn4.Enabled = false;
            btn4.Click += button1_Click_5;
            this.Controls.Add(btn4);
            Button restart = new Button();
            restart.Size = new Size(50, 25);
            restart.Location = new Point(500, 45);
            restart.Text = "Restart";
            restart.Click += button1_Click;
            this.Controls.Add(restart);
            Button record = new Button();
            record.Size = new Size(100, 25);
            record.Location = new Point(500, 200);
            record.Text = "Record and stop";
            record.Click += button1_Click_1;
            this.Controls.Add(record);
            Button hod = new Button();
            hod.Size = new Size(100, 25);
            hod.Location = new Point(500, 250);
            hod.Text = "Step";
            hod.Click += button1_Click_2;
            this.Controls.Add(hod);
            Button learn = new Button();
            learn.Size = new Size(100, 25);
            learn.Location = new Point(500, 350);
            learn.Text = "learn";
            learn.Click += button1_Click_3;
            learn.Visible = false;
            this.Controls.Add(learn);
            Button load = new Button();
            load.Size = new Size(100, 25);
            load.Location = new Point(500, 400);
            load.Text = "load";
            load.Click += button1_Click_4;
            load.Visible = false;
            this.Controls.Add(load);
        }

        public void CreateLable()
        {
            string[] numb = new string[8] { "1", "2", "3", "4", "5", "6", "7", "8" };
            string[] letters = new string[8] { "A", "B", "C", "D", "E", "F", "G", "H" };
            for (int i = 0; i < 8; i++)
            {
                Label label = new Label();
                label.Text = numb[7 - i];
                label.Font = new Font("Microsoft Sans Serif", 22, FontStyle.Bold);
                label.Location = new Point(15, 7 + 50 * (i + 1));
                label.Size = new Size(33, 36);
                this.Controls.Add(label);
                Label label1 = new Label();
                label1.Text = numb[7 - i];
                label1.Font = new Font("Microsoft Sans Serif", 22, FontStyle.Bold);
                label1.Location = new Point(449, 7 + 50 * (i + 1));
                label1.Size = new Size(33, 36);
                this.Controls.Add(label1);
                Label label2 = new Label();
                label2.Text = letters[i];
                label2.Font = new Font("Microsoft Sans Serif", 22, FontStyle.Bold);
                label2.Location = new Point(7 + 50 * (i + 1), 15);
                label2.Size = new Size(37, 36);
                this.Controls.Add(label2);
                Label label3 = new Label();
                label3.Text = letters[i];
                label3.Font = new Font("Microsoft Sans Serif", 22, FontStyle.Bold);
                label3.Location = new Point(7 + 50 * (i + 1), 450);
                label3.Size = new Size(37, 36);
                this.Controls.Add(label3);

            }
        }

        public void CreateMap()
        {
            string[] numb = new string[8] { "1", "2", "3", "4", "5", "6", "7", "8" };
            string[] letters = new string[8] { "A", "B", "C", "D", "E", "F", "G", "H" };
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j] = new Button();

                    Button butt = new Button();
                    butt.Size = new Size(50, 50);
                    butt.Name = letters[j] + numb[7 - i];
                    butt.Location = new Point(50 + j * 50, 50 + i * 50);

                    switch (map[i, j] / 10)
                    {
                        case 1:
                            Image part = new Bitmap(50, 50);
                            Graphics g = Graphics.FromImage(part);
                            g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * (map[i, j] % 10 - 1), 0, 150, 150, GraphicsUnit.Pixel);
                            butt.BackgroundImage = part;
                            break;
                        case 2:
                            Image part1 = new Bitmap(50, 50);
                            Graphics g1 = Graphics.FromImage(part1);
                            g1.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * (map[i, j] % 10 - 1), 150, 150, 150, GraphicsUnit.Pixel);
                            butt.BackgroundImage = part1;
                            break;
                    }
                    if ((i + j) % 2 == 0)
                    {
                        butt.BackColor = Color.Khaki;
                    }
                    else
                    {
                        butt.BackColor = Color.DarkGoldenrod;
                    }
                    butt.Click += new EventHandler(OnFigurePress);
                    this.Controls.Add(butt);

                    butts[i, j] = butt;
                }
            }
        }

        public void Motion(string from_x, string from_y, string to_x, string to_y)
        {
            string from = from_x + from_y, to = to_x + to_y;
            Button pc1 = Controls[from] as Button;
            Button pc2 = Controls[to] as Button;
            EventArgs e = new EventArgs();
            OnFigurePress(pc1, e);
            OnFigurePress(pc2, e);
        }

        public void OnFigurePress(object sender, EventArgs e)
        {

            Button pressedButton = sender as Button;
            if (map[pressedButton.Location.Y / 50 - 1, pressedButton.Location.X / 50 - 1] != 0 && map[pressedButton.Location.Y / 50 - 1, pressedButton.Location.X / 50 - 1] / 10 == currPlayer)
            {
                CloseSteps();
                DeactivateAllButtons();
                pressedButton.Enabled = true;
                Color c = pressedButton.BackColor;
                ShowSteps(pressedButton.Location.Y / 50 - 1, pressedButton.Location.X / 50 - 1, map[pressedButton.Location.Y / 50 - 1, pressedButton.Location.X / 50 - 1]);

                if (isMoving)
                {
                    CloseSteps();
                    ActivateAllButtons();
                    isMoving = false;
                }
                else
                    isMoving = true;
            }
            else
            {
                if (isMoving)
                {
                    int temp = map[pressedButton.Location.Y / 50 - 1, pressedButton.Location.X / 50 - 1];
                    map[pressedButton.Location.Y / 50 - 1, pressedButton.Location.X / 50 - 1] = map[prevButton.Location.Y / 50 - 1, prevButton.Location.X / 50 - 1];
                    map[prevButton.Location.Y / 50 - 1, prevButton.Location.X / 50 - 1] = temp;
                    pressedButton.BackgroundImage = prevButton.BackgroundImage;
                    prevButton.BackgroundImage = null;
                    isMoving = false;
                    CloseSteps();
                    ActivateAllButtons();
                    SwitchPlayer();
                }
            }

            prevButton = pressedButton;
            Dead_King();
        }

        public void ShowSteps(int IcurrFigure, int JcurrFigure, int currFigure)
        {
            int dir = currPlayer == 1 ? -1 : 1;
            switch (currFigure % 10)
            {
                case 6:
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure))
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure] == 0)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure].Enabled = true;
                            if (InsideBorder(IcurrFigure + 2 * dir, JcurrFigure) && (IcurrFigure == 1 || IcurrFigure == 6))
                            {
                                if (map[IcurrFigure + 2 * dir, JcurrFigure] == 0)
                                {
                                    butts[IcurrFigure + 2 * dir, JcurrFigure].BackColor = Color.Yellow;
                                    butts[IcurrFigure + 2 * dir, JcurrFigure].Enabled = true;
                                }
                            }
                        }
                    }

                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1))
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != currPlayer)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].Enabled = true;
                        }
                    }
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1))
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].Enabled = true;
                        }
                    }
                    break;
                case 5:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    break;
                case 3:
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 2:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 1:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure, true);
                    ShowDiagonal(IcurrFigure, JcurrFigure, true);
                    break;
                case 4:
                    ShowHorseSteps(IcurrFigure, JcurrFigure);
                    break;
            }
        }

        public void ShowHorseSteps(int IcurrFigure, int JcurrFigure)
        {
            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure - 2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure - 2);
            }
        }

        public void DeactivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = false;
                }
            }
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = true;
                }
            }
        }

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
        }

        public void ShowVerticalHorizontal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }
                if (isOneStep)
                    break;
            }
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }
                if (isOneStep)
                    break;
            }
            for (int j = JcurrFigure + 1; j < 8; j++)
            {
                if (InsideBorder(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }
                if (isOneStep)
                    break;
            }
            for (int j = JcurrFigure - 1; j >= 0; j--)
            {
                if (InsideBorder(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }
                if (isOneStep)
                    break;
            }
        }

        public bool DeterminePath(int IcurrFigure, int j)
        {
            if (map[IcurrFigure, j] == 0)
            {
                butts[IcurrFigure, j].BackColor = Color.Yellow;
                butts[IcurrFigure, j].Enabled = true;
            }
            else
            {
                if (map[IcurrFigure, j] / 10 != currPlayer)
                {
                    butts[IcurrFigure, j].BackColor = Color.Yellow;
                    butts[IcurrFigure, j].Enabled = true;
                }
                return false;
            }
            return true;
        }

        public bool InsideBorder(int ti, int tj)
        {
            if (ti >= 8 || tj >= 8 || ti < 0 || tj < 0)
                return false;
            return true;
        }

        public void CloseSteps()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0) butts[i, j].BackColor = Color.Khaki;
                    else butts[i, j].BackColor = Color.DarkGoldenrod;
                }
            }
        }

        public void SwitchPlayer()
        {
            if (currPlayer == 1)
                currPlayer = 2;
            else currPlayer = 1;
        }

        private void Dead_King()
        {
            Image part = new Bitmap(50, 50);
            Graphics g = Graphics.FromImage(part);
            g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0, 0, 150, 150, GraphicsUnit.Pixel);
            Image part1 = new Bitmap(50, 50);
            Graphics g1 = Graphics.FromImage(part1);
            g1.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0, 150, 150, 150, GraphicsUnit.Pixel);

            bool t1 = false, t2 = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!(butts[i, j].BackgroundImage is null))
                    {
                        if (Equality(butts[i, j].BackgroundImage, part)) t1 = true;
                        if (Equality(butts[i, j].BackgroundImage, part1)) t2 = true;
                    }

                }
            }
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            if (t1 && t2) return;
            else if (!t2)
            {
                result = MessageBox.Show("White Win", "Win", buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    EventArgs e = new EventArgs();
                    button1_Click(result, e);
                }
                if (result == System.Windows.Forms.DialogResult.No) DeactivateAllButtons();
            }
            else
            {
                result = MessageBox.Show("Black Win", "Win", buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    EventArgs e = new EventArgs();
                    button1_Click(result, e);
                }
                if (result == System.Windows.Forms.DialogResult.No) DeactivateAllButtons();
            }
        }

        bool Equality(Image Img1, Image Img2)
        {
            Bitmap Bmp1 = (Bitmap)Img1;
            Bitmap Bmp2 = (Bitmap)Img2;
            if (Bmp1.Size == Bmp2.Size)
            {
                for (int i = 0; i < Bmp1.Width; i++)
                    for (int j = 0; j < Bmp1.Height; j++)
                        if (Bmp1.GetPixel(i, j) != Bmp2.GetPixel(i, j)) return false;
                return true;
            }
            else return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            Init();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (but == 0)
            {
                
                try
                {
                    btn.Text = "Recording";
                    waveIn = new WaveIn();
                    waveIn.DeviceNumber = 0;
                    waveIn.DataAvailable += waveIn_DataAvailable;
                    waveIn.RecordingStopped += waveIn_RecordingStopped;
                    waveIn.WaveFormat = new WaveFormat(44100, 2);
                    writer = new WaveFileWriter(outputFilename+file_index.ToString()+wav, waveIn.WaveFormat);
                    file_index++;
                    waveIn.StartRecording();
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message); }
                but = 1;
            }
            else
            {
                if (waveIn != null)
                {
                    btn.Text = "Record and stop";
                    StopRecording();
                    motion = 0;
                }
                but = 0;
                
            }
        }

        public static List<List<float>> Normalization(List<List<float>> list, float fmax = 0, float fmin = 1000)
        {
            float max = 0, min = 10000;
            foreach (List<float> flo in list)
            {
                max = Math.Max(max, flo.Max());
                min = Math.Min(min, flo.Min());
            }
            max = max > fmax ? max : fmax;
            min = min < fmin ? min : fmin;
            StreamWriter sw = new StreamWriter("all/minmax.txt");
            sw.WriteLine(max.ToString());
            sw.WriteLine(min.ToString());
            sw.Close(); 
            float n = max - min;
            List<List<float>> dop = new List<List<float>>();
            foreach (List<float> flo in list)
            {
                List<float> dop_flo = new List<float>();
                foreach (float t in flo)
                {
                    if ((t >= 1) && (t <= 16)) dop_flo.Add(t);
                    else dop_flo.Add((t - min) / n);
                }
                dop.Add(dop_flo);
            }
            return dop;
        }

        private async void button1_Click_2(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;
            if (motion == 0)
            {
                MFCC.Handling(file_index - 1);
                StreamReader sr = new StreamReader("all/hod.txt");
                StreamWriter sw = new StreamWriter("all/hod1.txt");
                string line;
                List<List<float>> list = new List<List<float>>();
                int maximus = 0;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var floats = line.Split('\t').Select(float.Parse).ToList();
                    list.Add(floats);
                }
                sr.Close();
                for (int i = 0; i < list.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        double[] dop = new double[960];
                        int j = 1;
                        for (; j < list[i].Count && j <= 960; j++)
                        {
                            dop[j - 1] = list[i][j];
                        }
                        j--;
                        for (; j < 960; j++)
                        {
                            dop[j] = 0;
                        }
                        s[i] = Predict1.__Spreadshe_MLP_960_96_8(dop);
                    }
                    else
                    {
                        double[] dop = new double[924];
                        int j = 1;
                        for (; j < list[i].Count && j <= 924; j++)
                        {
                            dop[j - 1] = list[i][j];
                        }
                        j--;
                        for (; j < 924; j++)
                        {
                            dop[j] = 0;
                        }
                        s[i] = Predict.__Spreadshe_MLP_924_112_8(dop);
                    }

                }
                s[0] = d[s[0]];
                s[2] = d[s[2]];
                btn.Enabled = true;
                btn1.Text = s[0];
                btn2.Text = s[1];
                btn3.Text = s[2];
                btn4.Text = s[3];
                btn1.Enabled = true;
                btn2.Enabled = true;
                btn3.Enabled = true;
                btn4.Enabled = true;
                motion = 1;
                sw.Close();
            }
            else
            {
                Motion(s[0], s[1], s[2], s[3]);
                btn.Text = "Step";
                btn1.Text = "";
                btn2.Text = "";
                btn3.Text = "";
                btn4.Text = "";
                btn1.Enabled = false;
                btn2.Enabled = false;
                btn3.Enabled = false;
                btn4.Enabled = false;
            }
            btn.Enabled = true;

        }

        
        private async void button1_Click_3(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;
            MFCC.Dop_main();
            StreamReader sr = new StreamReader("all/neural.txt");
            string line;
            List<List<float>> list = new List<List<float>>();
            int maximus = 0;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                var floats = line.Split('\t').Select(float.Parse).ToList();
                list.Add(floats);
                maximus = Math.Max(maximus, floats.Count);
            }
            sr.Close();
            list = Normalization(list);
            List<List<float>> dop_list = new List<List<float>>();
            foreach (List<float> floats in list)
            {
                List<float> t = floats;
                while (t.Count < maximus)
                {
                    t.Add(0);
                }
                dop_list.Add(t);
            }
            list = dop_list;
            for (int i = 0; i < 10000; i++)
            {
                foreach (List<float> flo in list)
                {
                    float[] t = new float[flo.Count - 1];
                    for (int it = 1; it < flo.Count; it++)
                    {
                        t[it - 1] = flo[it];
                    }
                    if (flo[0]>8)
                    net.BackPropagate(new float[] { NeuralNetwork.Norm2(flo[0]) }, t);
                    else net_num.BackPropagate(new float[] { NeuralNetwork.Norm(flo[0]) }, t);
                }
            }
            StreamWriter sw = new StreamWriter("all/max.txt");
            sw.WriteLine(maximus.ToString());
            for (int i = 1; i < layers.Length; i++)
            {
                sw.WriteLine(layers[i].ToString());
            }
            sw.Close();
            net.Save("all/MLP2.txt");
            net_num.Save("all/MLP3.txt");
            btn.Enabled = true;
        }

        private async void button1_Click_4(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;
            net.Load("all/MLP2.txt");
            net_num.Load("all/MLP3.txt");
            btn.Enabled = true;
        }

        private void button1_Click_5(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            MFCC.Handling1(file_index - 1);
            StreamReader sr = new StreamReader("all/hod.txt");
            StreamWriter sw = new StreamWriter("../../../sjklfka.txt");
            var floats = sr.ReadLine().Split('\t').Select(double.Parse).ToList();
            sr.Close(); 
            int i = (btn.Location.X-500)/25;
            if (i % 2 == 0)
            {
                double[] dop = new double[960];
                int j = 1;
                for (; j < floats.Count && j <= 960; j++)
                {
                    dop[j - 1] = floats[j];
                    sw.WriteLine(dop[j - 1].ToString());
                }
                j--;
                for (; j < 960; j++)
                {
                    dop[j] = 0;
                    sw.WriteLine(dop[j].ToString());
                }
                s[i] = Predict1.__Spreadshe_MLP_960_96_8(dop);
                s[i] = d[s[i]];
            }
            else
            {
                double[] dop = new double[924];
                int j = 1;
                for (; j < floats.Count && j <= 924; j++)
                {
                    dop[j - 1] = floats[j];
                    sw.WriteLine(dop[j - 1].ToString());
                }
                j--;
                for (; j < 924; j++)
                {
                    dop[j] = 0;
                    sw.WriteLine(dop[j].ToString());
                }
                s[i] = Predict.__Spreadshe_MLP_924_112_8(dop);
            }
            btn.Text = s[i];
            sw.Close();
            motion = 1;
        }



    }
}
