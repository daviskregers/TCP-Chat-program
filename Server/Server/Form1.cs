using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public partial class Form1 : Form
    {

        IPAddress ipAd;
        TcpListener myList;
        Socket s;

        public Form1()
        {
            InitializeComponent();
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

            textBox2.AppendText("Client: " + text + "\n");

        }

        private void WorkThread(object obj)
        {

            ObjectDelegate del = (ObjectDelegate)obj;

            while(true) {

                if (!s.Connected) continue;

                byte[] b = new byte[100];
                int k = s.Receive(b);

                String rcv = "";
                for (int i = 0; i < k; i++)
                    rcv += Convert.ToChar(b[i]);

                del.Invoke(rcv);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // ateTextBox();

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

            ipAd = IPAddress.Parse( textBox3.Text );
            myList = new TcpListener(ipAd, Int32.Parse(textBox1.Text) );
            myList.Start();

            textBox2.AppendText("The server is running at port "+ textBox1.Text +"... \n");
            textBox2.AppendText("The local End point is  :" + myList.LocalEndpoint + "\n");
            textBox2.AppendText("Waiting for a connection..... \n ");

            s = myList.AcceptSocket();
            textBox2.AppendText("Connection accepted from " + s.RemoteEndPoint + "\n");

            ObjectDelegate del = new ObjectDelegate(UpdateTextBox);

            Thread th = new Thread(new ParameterizedThreadStart(WorkThread));
            th.Start(del);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (textBox4.Text == "") return;
            ASCIIEncoding asen = new ASCIIEncoding();

            s.Send(asen.GetBytes(textBox4.Text));
            textBox2.AppendText("Server: " + textBox4.Text + " \n");

            textBox4.Text = "";

        }
    }
}
