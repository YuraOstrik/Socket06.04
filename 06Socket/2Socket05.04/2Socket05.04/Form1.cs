using System.Net.Sockets;
using System.Net;
using System.Text;

namespace _2Socket05._04
{
    public partial class Form1 : Form
    {
        Socket sock;
        public Form1()
        {
            InitializeComponent();
        }

        private async void Connect()
        {
            await Task.Run(() =>
            {
                try
                {

                    IPAddress ipAddr = IPAddress.Loopback;

                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr /* IP-����� */, 49152 /* ���� */);

                    sock = new Socket(AddressFamily.InterNetwork /*����� ���������*/, SocketType.Stream /*��� ������*/, ProtocolType.Tcp /*��������*/);

                    sock.Connect(ipEndPoint);
                    byte[] msg = Encoding.Default.GetBytes(Dns.GetHostName() );
                    int bytesSent = sock.Send(msg);
                    MessageBox.Show("������ " + Dns.GetHostName() + " ��������� ���������� � " + sock.RemoteEndPoint.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("������: " + ex.Message);
                }
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private async void Exchange()
        {
            await Task.Run(() =>
            {
                try
                {
                    string theMessage = textBox1.Text;
                    byte[] msg = Encoding.Default.GetBytes(theMessage); 
                    int bytesSent = sock.Send(msg); 
                    if (theMessage.IndexOf("<end>") > -1) 
                    {
                        byte[] bytes = new byte[1024];
                        int bytesRec = sock.Receive(bytes); 
                        MessageBox.Show("������ (" + sock.RemoteEndPoint.ToString() + ") �������: " + Encoding.Default.GetString(bytes, 0, bytesRec) /*������������ ������ ������ � ������*/);
                        sock.Shutdown(SocketShutdown.Both); 
                        sock.Close(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("������: " + ex.Message);
                }
            });
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Exchange();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                sock.Shutdown(SocketShutdown.Both); 
                sock.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("������: " + ex.Message);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Connect();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Exchange();
        }
    }
}
