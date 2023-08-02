using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public partial class Form1 : MaterialSkin.Controls.MaterialForm
    {
        Socket Sck;
        EndPoint endPointLocal, endPointRemote;
        int Clr = 0;


        public Form1()
        {
            InitializeComponent();
            Sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);  //socket created
            Sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); //set socket

            int Counter = ListMessageHere.Items.Count;
            if (Counter > 0)
            {
                ListMessageHere.SelectedIndex = Counter;           
            }

            textBox1.Text = getLocalIp();
            textBox3.Text = getLocalIp();
        }
        private string getLocalIp()
        {
            IPHostEntry Host;   //creating host obj
            Host = Dns.GetHostEntry(Dns.GetHostName());  
            foreach (IPAddress Ip in Host.AddressList) //getting ip of the system
            {
                if (Ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return Ip.ToString();
                }
            }
            return "Not Found"; // returning local ip address
        }
        private void messageCallBack(IAsyncResult aResult)
        {
            try
            {
                int Size = Sck.EndReceiveFrom(aResult, ref endPointRemote);
                if (Size > 0)
                {
                    byte[] receivedData = new byte[1500];  
                    receivedData = (byte[])aResult.AsyncState; // getting data in byte array
                    ASCIIEncoding eEncoding = new ASCIIEncoding(); //encoding msg
                    string receivedMessage = eEncoding.GetString(receivedData); // converting received message byte into string
                    
                    ListMessageHere.Items.Add("Friend: " + receivedMessage);
                }
                byte[] Buffer = new byte[1500];
                Sck.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref endPointRemote, new AsyncCallback(messageCallBack), Buffer);

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString()); 
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                endPointLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text));
                Sck.Bind(endPointLocal);

                endPointRemote = new IPEndPoint(IPAddress.Parse(textBox3.Text), Convert.ToInt32(textBox4.Text));
                Sck.Connect(endPointRemote);

                byte[] Buffer = new byte[1500];
                Sck.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref endPointRemote, new AsyncCallback(messageCallBack), Buffer);

                MessageBox.Show("Connection Established Successfully");
                button1.Text = "Connected";
                button1.Enabled = false;
                button2.Enabled = true;
                textBox5.Focus();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding Enc = new System.Text.ASCIIEncoding();
                byte[] Message = new byte[1500];
                Message = Enc.GetBytes(textBox5.Text);
                Sck.Send(Message);
                ListMessageHere.Items.Add("You: " + textBox5.Text);
                MessageBox.Show("Message Sent Successfully");
                textBox5.Clear();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Clr == 0)
            {
                ListMessageHere.ForeColor = Color.CadetBlue;
                Clr = 1;
            }
            else
            {
                ListMessageHere.ForeColor = Color.Chocolate;
                Clr = 0;
            }
            //ListMessageHere.Location = new Point(ListMessageHere.Location.X+5, ListMessageHere.Location.Y);
            //if (ListMessageHere.Location.X > this.Width)
            //{
            //    ListMessageHere.Location = new Point(0 - ListMessageHere.Width, ListMessageHere.Location.Y);
            //}
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            //ListMessageHere.Items.Add("Typing...");
        }
    }
}
