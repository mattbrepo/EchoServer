using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace EchoServer
{
	public partial class MainForm : Form
	{
		const string BTN_START = "Start";
		const string BTN_STOP = "Stop";
		const string BTN_TEMP = "...";

		private TcpListener _tcpListener;
		private Thread _listenThread;
		private bool _on = false;

		public MainForm()
		{
			InitializeComponent();
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			if (buttonStart.Text == BTN_STOP)
			{
				try
				{
					_on = false;
					_tcpListener.Stop();
				}
				catch (Exception ex)
				{
					ex.ToString();
				}
				buttonStart.Text = BTN_START;
			}
			else if (buttonStart.Text == BTN_START)
			{
				buttonStart.Text = BTN_TEMP;
				try
				{
					_on = true;
					_tcpListener = new TcpListener(IPAddress.Any, int.Parse(textBoxPort.Text));
					_listenThread = new Thread(new ThreadStart(ListenForClients));
					_listenThread.Start();
				}
				catch (Exception ex)
				{
					MessageBox.Show("buttonStart: " + ex.ToString());
				}

			}
		}

		private void ListenForClients()
		{
			try
			{
				_tcpListener.Start();

				buttonStart.Do((x) => { x.Text = BTN_STOP; });

				while (_on)
				{
					//blocks until a client has connected to the server
					TcpClient client = _tcpListener.AcceptTcpClient();

					//create a thread to handle communication
					//with connected client
					Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
					clientThread.Start(client);
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private void HandleClientComm(object client)
		{
			try
			{
				TcpClient tcpClient = (TcpClient)client;
				NetworkStream clientStream = tcpClient.GetStream();

				byte[] message = new byte[1];
				int bytesRead;

				while (_on)
				{
					bytesRead = 0;

					try
					{
						//blocks until a client sends a message
						bytesRead = clientStream.Read(message, 0, message.Length);
					}
					catch
					{
						//a socket error has occured
						break;
					}

					if (bytesRead == 0)
					{
						//the client has disconnected from the server
						break;
					}

					clientStream.Write(message, 0, message.Length);
				}

				tcpClient.Close();
			}
			catch
			{
			}
		}
	}
}
