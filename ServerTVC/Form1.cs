using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace ServerTVC
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        List<string[]> data1 = new List<string[]>();
        List<string[]> data2 = new List<string[]>();

        private void Form1_Load(object sender, EventArgs e)
        {
            new Server(80);
        }




        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {

            string sqlExpression = "SELECT * FROM PMServiceLogs WHERE ErrStatus = 1 AND ID >" + Properties.Settings.Default.PMServiseLogs_id + "  ORDER BY ID";
            string sqlExpression1 = "SELECT * FROM PMServiceLogs_H WHERE ErrStatus = 1 AND ID >" + Properties.Settings.Default.PMServiseLogs_H_id + " ORDER BY ID";
            string sqlExpression2 = "SELECT * FROM PointMasterLogs WHERE (Msg LIKE '%Error%' OR Msg LIKE '%error%' OR Msg LIKE '%ERROR%') AND ID >" + Properties.Settings.Default.PMServiseLogs_H_id + " ORDER BY ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.bdlink))
                {
                    connection.Open();
                    Console.WriteLine("Подключение открыто");
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlDataReader reader = command.ExecuteReader();


                    List<string> list = new List<string>();
                    if (reader.HasRows) // если есть данные
                    {
                        // выводим названия столбцов
                        //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3), reader.GetName(4), reader.GetName(5), reader.GetName(6), reader.GetName(7), reader.GetName(8));

                        while (reader.Read()) // построчно считываем данные
                        {
                            data1.Add(new string[10]);
                            data1[data1.Count - 1][0] = reader[0].ToString();
                            data1[data1.Count - 1][1] = " ";
                            data1[data1.Count - 1][2] = reader[1].ToString();
                            data1[data1.Count - 1][3] = reader[2].ToString();
                            data1[data1.Count - 1][4] = reader[3].ToString();
                            data1[data1.Count - 1][5] = reader[4].ToString();
                            data1[data1.Count - 1][6] = reader[5].ToString();
                            data1[data1.Count - 1][7] = reader[6].ToString();
                            data1[data1.Count - 1][8] = reader[7].ToString();
                            data1[data1.Count - 1][9] = reader[8].ToString();
                            if (!list.Contains(reader[4].ToString()))
                            {
                                list.Add(reader[4].ToString());
                            }
                        }
                    }
                    reader.Close();

                    SqlCommand command1 = new SqlCommand(sqlExpression1, connection);
                    SqlDataReader reader1 = command1.ExecuteReader();

                    if (reader1.HasRows) // если есть данные
                    {


                        while (reader1.Read()) // построчно считываем данные
                        {
                            data1.Add(new string[10]);
                            data1[data1.Count - 1][0] = reader1[0].ToString();
                            data1[data1.Count - 1][1] = " ";
                            data1[data1.Count - 1][2] = reader1[1].ToString();
                            data1[data1.Count - 1][3] = reader1[2].ToString();
                            data1[data1.Count - 1][4] = reader1[3].ToString();
                            data1[data1.Count - 1][5] = reader1[4].ToString();
                            data1[data1.Count - 1][6] = reader1[5].ToString();
                            data1[data1.Count - 1][7] = reader1[6].ToString();
                            data1[data1.Count - 1][8] = reader1[7].ToString();
                            data1[data1.Count - 1][9] = reader1[8].ToString();
                            if (!list.Contains(reader1[4].ToString()))
                            {
                                list.Add(reader1[4].ToString());
                            }
                        }
                    }

                    reader1.Close();

                    SqlCommand command2 = new SqlCommand(sqlExpression2, connection);
                    SqlDataReader reader2 = command2.ExecuteReader();

                    if (reader2.HasRows) // если есть данные
                    {


                        while (reader2.Read()) // построчно считываем данные
                        {
                            data2.Add(new string[10]);
                            data2[data2.Count - 1][0] = reader2[0].ToString();
                            data2[data2.Count - 1][1] = reader2[1].ToString();
                            data2[data2.Count - 1][2] = reader2[5].ToString();
                            data2[data2.Count - 1][3] = reader2[2].ToString();
                            data2[data2.Count - 1][4] = " ";
                            data2[data2.Count - 1][5] = " ";
                            data2[data2.Count - 1][6] = reader2[3].ToString();
                            data2[data2.Count - 1][7] = reader2[4].ToString();
                            data2[data2.Count - 1][8] = " ";
                            data2[data2.Count - 1][9] = reader2[6].ToString();
                            //if (!list.Contains(reader2[4].ToString()))
                            //{
                            //    list.Add(reader2[4].ToString());
                            //}
                        }
                    }

                    reader2.Close();

                    /*
                    foreach (string[] s in data1) {

                    }
                        dataGridView1.Rows.Add(s);


                    foreach (string[] s in data2)
                        dataGridView1.Rows.Add(s);
                        */
                }

            }
            catch
            {
                MessageBox.Show("Проверьте настройки подключения к базе данных, либо наличие необходимых таблиц", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

    }
    class Server
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public Server(int Port)
        {
            // Создаем "слушателя" для указанного порта
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем новых клиентов
                new Client(Listener.AcceptTcpClient());
            }
        }

        // Остановка сервера
        ~Server()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }

    }

    class Client
    {
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            // Код простой HTML-странички
            string Html = "<html><body><h1>It works!</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            Client.Close();
        }
    }

}
