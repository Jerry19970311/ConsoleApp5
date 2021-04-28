using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("请输入指令:");
            string message = Console.ReadLine();
            Console.WriteLine(Send("192.168.1.101", 5001, message));
            Console.ReadKey();
        }
        public static string Send(string host, int port, string data)
        {
            string result = string.Empty;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(host, port);
            clientSocket.Send(StringToHex(data));
            Console.WriteLine("Send：" + data);
            result = Receive(clientSocket, 5000 * 2); //5*2 seconds timeout.
            Console.WriteLine("Receive：" + result);
            DestroySocket(clientSocket);
            return result;
        }
        private static string Receive(Socket socket, int timeout)
        {
            StringBuilder result = new StringBuilder();
            socket.ReceiveTimeout = timeout;
            byte[] bytes = new byte[1024];
            Queue<string> ssQueue = new Queue<string>();
            int length = 0;
            try
            {
                while ((length = socket.Receive(bytes)) > 0)
                {
                    for(int i = 0; i < length; i++)
                    {
                        string s = Convert.ToString(bytes[i], 16).ToUpper();
                        if (s.Length.Equals(1))
                        {
                            s = "0" + s;
                        }
                        ssQueue.Enqueue(s);
                    }
                    if (length < bytes.Length)
                    {
                        break;
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            while (false == 0.Equals(ssQueue.Count))
            {
                result.Append(ssQueue.Dequeue()+" ");
            }
            return result.ToString();
        }
        private static void DestroySocket(Socket socket)
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }
        //16进制字符串转化为字节数组
        static byte[] StringToHex(string origin)
        {
            //去掉指令中的空格
            string withoutspace = origin.Replace(" ", "");
            //若非偶数个，则在末尾再加一个空格，使字符个数成为偶数
            if ((withoutspace.Length % 2) != 0)
                withoutspace += " ";
            //一个16进制数为4位，一个byte为8位，故字节数组的长度为字符串一半
            byte[] returnBytes = new byte[withoutspace.Length / 2];
            //每两个16进制字符转换成一个字节
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(withoutspace.Substring(i * 2, 2), 16);
            return returnBytes;
        }

    }
}
