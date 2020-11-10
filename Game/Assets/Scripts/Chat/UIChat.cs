using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Assets.Scripts.Chat
{
    public class UIChat : MonoBehaviour
    {
        public static UIChat Chat;

        public int MaxLines
        {
            get
            {
                return _maxLines;
            }
            set
            {
                _maxLines = Math.Min(Math.Max(value, 100), 50000);

                if (_extraLinesOnCleanup > _maxLines)
                    _extraLinesOnCleanup = _maxLines;
            }
        }
        public int ExtraLinesOnCleanup
        {
            get
            {
                return _extraLinesOnCleanup;
            }
            set
            {
                _extraLinesOnCleanup = Math.Min(_maxLines, Math.Max(value, 5));
            }
        }

        public event EventHandler<string> OnNewUserMessage;

        [SerializeField] int _extraLinesOnCleanup = 50;
        [SerializeField] int _maxLines = 1000;
        [SerializeField] List<string> _lines { get; } = new List<string>();
        [SerializeField] InputField _inputText;
        [SerializeField] Text _chatText;

        bool _refreshNextUpdate;

        public void ToggleHidden()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }

        void Start()
        {
        }

        void Awake()
        {
            Chat = this;
        }

        void Update()
        {
            CheckForInput();
        }

        void FixedUpdate()
        {
            if (_refreshNextUpdate)
                RefreshChat();

            if (_lines.Count <= _maxLines)
                return;

            CleanupExtraLines();
        }

        public void CheckForInput()
        {
            if (!PressedEnter() ||
                string.IsNullOrEmpty(_inputText.text))
                return;

            SendMessage(_inputText.text);
            _inputText.text = "";
        }

        bool PressedEnter()
        {
            return Input.GetKeyDown(KeyCode.KeypadEnter) ||
                   Input.GetKeyDown(KeyCode.Return);
        }

        public void SysMsg(string line)
        {
            AddLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} SYSTEM: {line}");
        }

        public void AddLine(string line)
        {
            lock(((ICollection)_lines).SyncRoot)
            {
                _lines.Add(line);
            }

            _refreshNextUpdate = true;
        }

        void RefreshChat()
        {
            lock(((ICollection)_lines).SyncRoot)
            {
                StringBuilder lineBuilder = new StringBuilder();
                for (int i = 0; i < _lines.Count; i++)
                    lineBuilder.AppendLine(_lines[i]);

                _chatText.text = lineBuilder.ToString();
            }
        }

        new void SendMessage(string message)
        {
            OnNewUserMessage?.Invoke(this, message);
        }

        void CleanupExtraLines()
        {
            lock (((ICollection)_lines).SyncRoot)
            {
                int linesToClean = _lines.Count - _maxLines;

                for (int i = 0;
                         i < linesToClean + _extraLinesOnCleanup && _lines.Count > 0;
                         i++)
                    _lines.RemoveAt(0);
            }
        }
    }
}
