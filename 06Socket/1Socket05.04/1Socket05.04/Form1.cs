using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace _1Socket05._04
{
    public partial class Form1 : Form
    {
        public SynchronizationContext uiContext;
        public Form1()
        {
            InitializeComponent();

            uiContext = SynchronizationContext.Current;
        }


        private async void ServerReceiveAsync(Socket handler)
        {
            await Task.Run(() =>
            {
                try
                {
                    string client = null;
                    string data = null;

                    byte[] bytes = new byte[1024];

                    int bytesRec = handler.Receive(bytes);
                    client = Encoding.Default.GetString(bytes, 0, bytesRec); 
                    client += "(" + handler.RemoteEndPoint.ToString() + ")";
                    while (true)
                    {
                        bytesRec = handler.Receive(bytes); 
                        if (bytesRec == 0)
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close(); 
                            return;
                        }
                        data = Encoding.Default.GetString(bytes, 0, bytesRec);

                        

                        uiContext.Send(d => listBox1.Items.Add(client), null);
                        uiContext.Send(d => listBox1.Items.Add(data), null);
                        if (data.Trim().ToLower() == "привет")
                        {
                            string greeting = "Привет, " + GetDay();
                            byte[] reply = Encoding.Default.GetBytes(greeting);
                            handler.Send(reply);


                            uiContext.Send(d => listBox1.Items.Add("Сервер: " + greeting), null);
                        }
                        if (data.IndexOf("<end>") > -1) 
                            break;
                    }
                    string theReply = "Я завершаю обработку сообщений";
                    byte[] msg = Encoding.Default.GetBytes(theReply); 
                    handler.Send(msg); 
                    handler.Shutdown(SocketShutdown.Both); 
                    handler.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сервер: " + ex.Message);
                    handler.Shutdown(SocketShutdown.Both); 
                    handler.Close();                    
                }
            });
        }


        private async void ServerListenAsync()
        {
            await Task.Run(() =>
            {
                try
                {

                    IPEndPoint ipEndPoint = new IPEndPoint(
                   IPAddress.Any ,
                   49152 );


                    Socket sListener = new Socket(AddressFamily.InterNetwork , SocketType.Stream, ProtocolType.Tcp );



                    sListener.Bind(ipEndPoint); 


                    sListener.Listen(2);
                    while (true)
                    {

                        Socket handler = sListener.Accept();
                        ServerReceiveAsync(handler);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сервер: " + ex.Message);
                }
            });
        }
        static string GetDay()
        {
            int hour = DateTime.Now.Hour;
            if (hour >= 5 && hour < 12)
                return "доброе утро!";
            else if (hour >= 12 && hour < 18)
                return "добрый день!";
            else
                return "добрый вечер!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerListenAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ServerListenAsync();
        }
    }
}
