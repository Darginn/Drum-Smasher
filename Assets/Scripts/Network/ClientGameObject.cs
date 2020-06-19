using Assets.Scripts.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Network
{
    public class ClientGameObject : MonoBehaviour
    {
        public User User
        {
            get
            {
                if (_user == null)
                    _user = new User(IPAddress.Parse(_serverIP), _serverPort,
                                     _accountName.text, _accountPass.text,
                                     Logger.GetLogger());

                return _user;
            }
        }

        private User _user;

        [SerializeField] private InputField _accountName;
        [SerializeField] private InputField _accountPass;
        [SerializeField] private string _serverIP;
        [SerializeField] private int _serverPort;


        public void ConnectAndLogin()
        {
            if (string.IsNullOrEmpty(_accountPass.text) ||
                string.IsNullOrEmpty(_accountName.text))
                return;

            UIChat.Chat.SysMsg("Connecting...");
			try
            {
                User.Connect();
            }
            catch (SocketException se)
            {
                UIChat.Chat.SysMsg($"Failed to connect: {se.Message}");
                return;
            }

            UIChat.Chat.SysMsg("Connected");
			
            User.StartReceiving();

            UIChat.Chat.SysMsg("Starting Authentication");
            StartCoroutine(User.AuthenticateCoroutine());
        }

        public void Exit()
        {
            User.Dispose();
            _user = null;
        }
    }
}
