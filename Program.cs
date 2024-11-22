using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;

public class SingleThreadedTcpClient
{
    private readonly string _serverAddress;
    private readonly int _port;
    private readonly string _message;
    public List<string> devices = new List<string>();
    public SingleThreadedTcpClient(string serverAddress, int port, string message)
    {
        _serverAddress = serverAddress;
        _port = port;
        _message = message;
        populatedevices();
    }

    public void SendDataStateless(string msg)
    {

        try
        {
            TcpClient client = new TcpClient(_serverAddress, _port);
            NetworkStream stream = client.GetStream();

            byte[] messageBytes = Encoding.ASCII.GetBytes(msg);
            stream.Write(messageBytes, 0, messageBytes.Length);
            Console.WriteLine($"Sent message: {msg} to server {_serverAddress}:{_port}\n");
            Thread.Sleep(1000);
            byte[] data = new byte[256];
            int bytesRead = stream.Read(data, 0, data.Length);

            if (bytesRead > 0)
            {
                string responseData = Encoding.ASCII.GetString(data, 0, bytesRead);
                Console.WriteLine("Received from Server: {0}", responseData);
            }


            stream.Close();
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to server: {ex.Message}");
        }
    }

    public static void Main(string[] args)
    {
        int i = 0;
        // Replace with the actual server address, port, and message
        string serverAddress = "127.0.0.1";
        // string serverAddress = "122.176.158.202";
        // string serverAddress = "122.176.158.205";
        int port = 6001;
        //string message = ">*,1005,860906047220709,1,152041,07072022,101,38.87,1,28.321245,22.878765,0,0,0,0,#\r\n";
        string message = "*@,1001,860906047220709,2,1,101,152041,07072022,1226,24,1,101,28.9,38.87,##\r\n"; // for maondelez
                                                                                                            //string message = "*@,2002,80646F3D2F1C,1,1,1,043641,22072024,324,45,2,101,190,0.00,##";
                                                                                                            // string message = ">*,1010,1,34945412EBD8,1,1,999,241212,28122023,1226,65,1,111,101,21.21,86.12,0,0,$"; // for GW
                                                                                                            // string message = ">*,1010,1,34945412EBD8,1,1,999,241212,28122023,1226,65,1,111,101,21.21,86.12,0,0,$";
                                                                                                            // string message = ">*,2,205,$";
                                                                                                            //string message= "*@,3,205,##";
        var client = new SingleThreadedTcpClient(serverAddress, port, message);
        while (true)
        {
            i = 0;

            while (i < 1000)
            {
                Random random = new Random();
                // Generate a random number between 20 and 45.
                foreach (string devImie in client.devices)
                {
                    Thread.Sleep(500);
                    int randomNumber = random.Next(20, 46);
                    //  message = "*@,2002," + devImie + ",2,1,101,152041,07072022,1226,24,1,101," + randomNumber + ",138.87,##"; // for maondelez
                    // message = "*@,2002,80646F3D2F1C,1,1,1,043641,22072024,324,45,2,101,190,0.00,##";
                    //message = ">*,1010,1,"+ devImie +",1,1,999,241212,28122023,1226,65,1,111,101,21.21,86.12,0,0,$";// for GW
                    message = "*@,1001," + devImie + ",2,1,101,152041,07072022,1226,24,1,101,28.9,38.87,##\r\n"; // for maondelez
                    client.SendDataStateless(message);
                }
                Thread.Sleep(10);
                i++;
            }
            Thread.Sleep(1000 * 60 * 4);
        }
    }


    private void populatedevices()
    {
        //   StreamReader rdr = new StreamReader(@"c:\temp\fererodata.txt");
        StreamReader rdr = new StreamReader(@"./FereroData.txt");
        //string listofdevices = rdr.ReadToEnd();
        while (!rdr.EndOfStream)
        {
            devices.Add(rdr.ReadLine());
        }
    }
}