using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.XImgProc;

namespace ReTake
{
    public partial class Form1 : Form
    {
        public int Camnum = 0;
        public bool e = false;
        public bool f = false;
        static public int s = 0;
        public Form1()
        {
            int maxcam = 0;
            while (true)
            {
                VideoCapture currentcam = new VideoCapture(maxcam+1);
                if (currentcam.Read(new Mat()))
                {
                    maxcam += 1;
                }
                else {
                    Console.WriteLine(maxcam);
                    break;
                }
            }
            if (!e)
            {
                if (!Directory.Exists("Videos"))
                {
                    Directory.CreateDirectory("Videos");
                }
                if (!File.Exists("Videos/vidnum.txt"))
                {
                    FileStream Fs = File.Open("Videos/vidnum.txt", FileMode.Create);
                    Fs.Close();
                    File.WriteAllText("Videos/vidnum.txt", "0\n0");
                    f = true;
                }
                if (!f)
                {
                    s = int.Parse(File.ReadAllLines("Videos/vidnum.txt")[0]);
                    Camnum = int.Parse(File.ReadAllLines("Videos/vidnum.txt")[1]);
                    Console.WriteLine(s);
                    Console.WriteLine(Camnum);
                }
                try
                {
                    File.WriteAllText($"Videos/out{s}.avi", "");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                e = true;
            }
            InitializeComponent();
            numericUpDown1.Maximum = maxcam;
            numericUpDown1.Value = Camnum;
            Thread bg = new Thread(new ThreadStart(Test));
            bg.IsBackground = true;
            bg.Start();
        }
        
        public bool on = true;
        public bool done = false;
        public static OpenCvSharp.Size dsize = new OpenCvSharp.Size(640, 480);
        public ManualResetEvent signal = new ManualResetEvent(initialState: false);
        public bool camchange = false;

        private void button1_Click(object sender, EventArgs e)
        {
            Thread alert = new Thread(new ThreadStart(save_alert));
            alert.Start();
            on = false;
            signal.WaitOne();
            signal.Reset();
            s += 1;
            
            while (!done)
            {
                try
                {
                    File.WriteAllText($"Videos/out{s}.avi", "");
                    done = true;
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            done = false;
            on = true;
            Thread bg = new Thread(new ThreadStart(Test));
            bg.IsBackground = true;
            bg.Start();

        }
        private void button2_Click(object sender, EventArgs e)
        {
            Thread alert = new Thread(new ThreadStart(del_alert));
            alert.Start();
            on = false;
            signal.WaitOne();
            signal.Reset();
            while (!done)
            {

                try
                {
                    File.Delete($"Videos/out{s}.avi");
                    File.WriteAllText($"Videos/out{s}.avi", "");
                    done = true;
                }
                catch (Exception ex) { 
                    Console.WriteLine(ex.Message); 
                }
            }
            done = false;
            on = true;
            Thread bg = new Thread(new ThreadStart(Test));
            bg.IsBackground = true;
            bg.Start();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Thread alert = new Thread(new ThreadStart(quit_alert));
            alert.Start();
            on = false;
            signal.WaitOne();
            signal.Reset();
            while (!done)
            {
                try
                {
                    File.WriteAllText("Videos/vidnum.txt", s.ToString() + $"\n{Camnum}");
                    File.Delete($"Videos/out{s}.avi");
                    done = true;
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            done = false;
            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.Kill();

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void numericValueChange(object sender, EventArgs e) {
            Camnum = (int)numericUpDown1.Value;
            camchange = true;
        }
        void Test()
        {
            VideoCapture capture = new VideoCapture(Camnum);
            using (VideoWriter writer = new VideoWriter($"Videos/out{s}.avi", FourCC.XVID, 24, dsize))
            using (Window window = new Window("Camera"))
            using (Mat image = new Mat())
            {

                while (on)
                {
                    if (camchange) {
                        capture.Release();
                        capture = new VideoCapture(Camnum);
                        Console.WriteLine();
                        camchange = false;
                    }
                    try
                    {
                        capture.Read(image);
                        window.ShowImage(image);
                        int key = Cv2.WaitKey(1);
                        writer.Write(image);
                    }
                    catch { }

                }
                writer.Release();
                capture.Release();
                image.Release();
                signal.Set();
            }
        }

        void msg(string Message)
        {
            label1.Invoke((MethodInvoker)(() => label1.Text = Message));
            Thread.Sleep(3000);
            label1.Invoke((MethodInvoker)(() => label1.Text = ""));
        }
        
        void save_alert() { msg("Stopping \nand Saving"); }
        void del_alert() { msg("Stopping \nand Deleting"); }
        void quit_alert() { msg("Stopping \nand Quitting"); }
    }

}
