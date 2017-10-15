using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace Client
{
    public partial class Form1 : Form
    {

        TcpClient tcpclnt;
        Stream stm;

        public Form1()
        {
            InitializeComponent();
            tcpclnt = new TcpClient();

        }

        private delegate void ObjectDelegate(object obj);

        private void UpdateTextBox(object obj)
        {
            // do we need to switch threads?
            if (InvokeRequired)
            {
                // slightly different now, as we dont need params
                // we can just use MethodInvoker
                ObjectDelegate method = new ObjectDelegate(UpdateTextBox);
                Invoke(method, obj);
                return;
            }

            string text = (string)obj;

            textBox2.AppendText("Server: " + text + "\n");

        }

        private void WorkThread(object obj)
        {

            ObjectDelegate del = (ObjectDelegate)obj;

            while (true)
            {

                if (stm == null) continue;

                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);

                String rcv = "";
                for (int i = 0; i < k; i++)
                    rcv += Convert.ToChar(bb[i]);

                del.Invoke(rcv);

            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            if (textBox1.Text == "")
            {
                textBox2.AppendText("Please provide the server port \n");
                return;
            }

            if (textBox3.Text == "")
            {
                textBox2.AppendText("Please provide the server IP \n");
                return;
            }

            try {

                textBox2.AppendText("Connecting to " + textBox1.Text + "\n");
                tcpclnt.Connect(textBox3.Text, Int32.Parse(textBox1.Text) );

                ObjectDelegate del = new ObjectDelegate(UpdateTextBox);

                Thread th = new Thread(new ParameterizedThreadStart(WorkThread));
                th.Start(del);

            }
            catch( Exception err )
            {
                textBox2.AppendText("An error occured: " + err.ToString() + "\n");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (textBox4.Text == "") return;

            String str = textBox4.Text;

            stm = tcpclnt.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(str);

            stm.Write(ba, 0, ba.Length);

            textBox2.AppendText("Client: " + textBox4.Text + " \n");
            textBox4.Text = "";

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
