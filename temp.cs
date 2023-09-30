using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public bool e = false;
        public bool f = false;
        static public int s = 0;
        public Form1()
        {
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
                    File.WriteAllText("Videos/vidnum.txt", "0");
                    f = true;
                }
                if (!f)
                {
                    s = int.Parse(File.ReadAllLines("Videos/vidnum.txt")[0]);
                    Console.WriteLine(s);
                }
                try
                {
                    File.WriteAllText($"Videos/out{s}.avi", "");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                e = true;
            }
            InitializeComponent();

            Thread bg = new Thread(new ThreadStart(Test));
            bg.IsBackground = true;
            bg.Start();
        }
        public bool del = false;
        public bool on = true;
        public bool done = false;
        public static OpenCvSharp.Size dsize = new OpenCvSharp.Size(640, 480);

        private void button1_Click(object sender, EventArgs e)
        {
            on = false;
            while (!del) { }
            Console.WriteLine(del);
            s += 1;

            while (!done)
            {
                try
                {
                    File.WriteAllText("Videos/vidnum.txt", s.ToString());
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
            del = false;
            label2.Text = "Stop & Save";

        }
        private void button2_Click(object sender, EventArgs e)
        {

            on = false;
            while (!del) { }
            Console.WriteLine(del);
            while (!done)
            {

                try
                {
                    File.Delete($"Videos/out{s}.avi");
                    File.WriteAllText($"Videos/out{s}.avi", "");
                    done = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            done = false;
            on = true;
            Thread bg = new Thread(new ThreadStart(Test));
            bg.IsBackground = true;
            bg.Start();
            del = false;
            label2.Text = "Stop & Delete";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            label2.Text = "Quit";
            on = false;
            while (!del) { }
            Console.WriteLine(del);
            while (!done)
            {
                try
                {
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
        void Test()
        {

            using (VideoWriter writer = new VideoWriter($"Videos/out{s}.avi", FourCC.XVID, 24, dsize))
            using (VideoCapture capture = new VideoCapture(1))
            using (Window window = new Window("Camera"))
            using (Mat image = new Mat())
            {

                while (on)
                {
                    capture.Read(image);
                    window.ShowImage(image);
                    int key = Cv2.WaitKey(1);
                    writer.Write(image);
                }
                writer.Release();
                capture.Release();
                image.Release();
                del = true;
            }
        }

    }

}
