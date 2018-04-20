using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab7_Bai3
{
    public partial class Form1 : Form
    {
        private static int pingstart, pingstop, elapsedtime;
        private static TextBox hostbox, databox;
        private static ListBox results;
        private static Thread pinger;
        private Socket sock;
        public Form1()
        {
            Text = "Advanced Ping Program";
            Size = new Size(400, 380);
            Label label1 = new Label();
            label1.Parent = this;
            label1.Text = "Enter host to ping:";
            label1.AutoSize = true;
            label1.Location = new Point(10, 30);
            hostbox = new TextBox();
            hostbox.Parent = this;
            hostbox.Size = new Size(200, 2 * Font.Height);
            hostbox.Location = new Point(10, 55);
            results = new ListBox();
            results.Parent = this;
            results.Location = new Point(10, 85);
            results.Size = new Size(360, 18 * Font.Height);
            Label label2 = new Label();
            label2.Parent = this;
            label2.Text = "Packet data:";
            label2.AutoSize = true;
            label2.Location = new Point(10, 330);
            databox = new TextBox();
            databox.Parent = this;
            databox.Text = "test packet";
            databox.Size = new Size(200, 2 * Font.Height);
            databox.Location = new Point(80, 325);
            Button sendit = new Button();
            sendit.Parent = this;
            sendit.Text = "Start";
            sendit.Location = new Point(220, 52);
            sendit.Size = new Size(5 * Font.Height, 2 * Font.Height);
            sendit.Click += new EventHandler(btnStart_Click);
            Button stopit = new Button();
            stopit.Parent = this;
            stopit.Text = "Stop";
            stopit.Location = new Point(295, 52);
            stopit.Size = new Size(5 * Font.Height, 2 * Font.Height);
            stopit.Click += new EventHandler(btnStop_Click);
            Button closeit = new Button();
            closeit.Parent = this;
            closeit.Text = "Close";
            closeit.Location = new Point(300, 320);
            closeit.Size = new Size(5 * Font.Height, 2 * Font.Height);
            closeit.Click += new EventHandler(btnClose_Click);
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            pinger = new Thread(new ThreadStart(sendPing));
            pinger.IsBackground = true;
            pinger.Start();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            pinger.Abort();
            this.Invoke((MethodInvoker)(() => results.Items.Add("Ping stopped")));
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            sock.Close();
            Close();
        }
        void sendPing()
        {
            IPHostEntry iphe = Dns.Resolve(hostbox.Text);
            IPEndPoint iep = new IPEndPoint(iphe.AddressList[0], 0);
            EndPoint ep = (EndPoint)iep;
            ICMP packet = new ICMP();
            int recv, i = 1;
            packet.Type = 0x08;
            packet.Code = 0x00;
            Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 0, 2);
            byte[] data = Encoding.ASCII.GetBytes(databox.Text);
            Buffer.BlockCopy(data, 0, packet.Message, 4, data.Length);
            packet.MessageSize = data.Length + 4;
            int packetsize = packet.MessageSize + 4;
            this.Invoke((MethodInvoker)(() => results.Items.Add("Pinging " + hostbox.Text)));
            while (true)
            {
                packet.Checksum = 0;
                Buffer.BlockCopy(BitConverter.GetBytes(i), 0, packet.Message, 2, 2);
                UInt16 chcksum = packet.getChecksum();
                packet.Checksum = chcksum;
                pingstart = Environment.TickCount;
                sock.SendTo(packet.getBytes(), packetsize, SocketFlags.None, iep);
                try
                {
                    data = new byte[1024];
                    recv = sock.ReceiveFrom(data, ref ep);
                    pingstop = Environment.TickCount;
                    elapsedtime = pingstop - pingstart;
                    this.Invoke((MethodInvoker)(() => results.Items.Add("reply from: " + ep.ToString() + ", seq: " + i + ", time = " + elapsedtime + "ms")));
                }
                catch (SocketException)
                {
                    this.Invoke((MethodInvoker)(() => results.Items.Add("no reply from host")));
                }
                i++;
                Thread.Sleep(3000);
            }
        }
    }
}
