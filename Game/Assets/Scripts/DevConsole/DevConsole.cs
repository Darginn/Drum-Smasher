using Assets.Scripts.DevConsole.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;

namespace Assets.Scripts.DevConsole
{
    public class DevConsole : MonoBehaviour
    {
        public Text OutputText;
        public InputField InputField;

        List<string> _lines;


        const int _maxLinesOnScreen = 26;
        const int _maxLineLength = 9999;
        
        static List<ICommand> _commands = new List<ICommand>();
        public static List<ICommand> Commands => _commands;

        // Start is called before the first frame update
        void Start()
        {
            _lines = new List<string>();
            GlobalConfig tss = (GlobalConfig)ConfigManager.GetOrLoadOrAdd<GlobalConfig>();

            if (tss.DefaultConsoleMessage != null)
                WriteLine(tss.DefaultConsoleMessage);
        }

        // Update is called once per frame
        void Update()
        {
            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && 
                InputField.text.Length > 0)
            {
                WriteLine(InputField.text, false);

                InputField.text = "";
            }
        }

        public void WriteLine(string line, bool doNotParse = true)
        {
            if (line.Length > _maxLineLength)
            {
                int i = 0;

                while (i < line.Length)
                {
                    int lineLength = line.Length - 1;
                    int subLength = (lineLength == _maxLineLength ? _maxLineLength :
                                    (lineLength < _maxLineLength ? lineLength : _maxLineLength));

                    string sub = line.Substring(i, subLength);
                    i += subLength;

                    if (doNotParse)
                        _lines.Insert(0, "> " + sub);
                    else
                     _lines.Insert(0, sub);
                }

                RefreshLines();
                return;
            }

            if (doNotParse)
                _lines.Insert(0, "> " + line);
            else
                _lines.Insert(0, line);

            if (!doNotParse && !line.StartsWith(">"))
                ParseCommand(line);

            RefreshLines();
        }

        void ParseCommand(string line)
        {
            if (line.StartsWith("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                Destroy(transform.parent.gameObject);
                return;
            }
            else if (line.StartsWith("cls", StringComparison.CurrentCultureIgnoreCase) || line.StartsWith("clear", StringComparison.CurrentCultureIgnoreCase))
            {
                _lines.Clear();
                return;
            }

            List<string> split = line.Split(' ').ToList();
            string cmd = split[0];
            split.RemoveAt(0);

            ExecuteCommand(cmd, split.ToArray());
        }

        public void ExecuteCommand(string command, params string[] args)
        {
            ICommand cmd = _commands.Find(c => c.Command.Equals(command, StringComparison.CurrentCultureIgnoreCase));

            if (cmd == null)
            {
                WriteLine("Could not find command: " + command);
                return;
            }

            try
            {
                cmd.DevConsole = this;
                cmd.Execute(args);
            }
            catch (Exception ex)
            {
                string[] exSplit = ex.ToString().Split(Environment.NewLine.ToCharArray());

                foreach (string str in exSplit)
                    if (!string.IsNullOrEmpty(str))
                        WriteLine(str);
            }
        }

        void RefreshLines()
        {
            int max = Math.Min(_maxLinesOnScreen - 1, _lines.Count - 1);
            string toShow = _lines[max];

            for (int i = max - 1; i >= 0; i--)
                toShow += Environment.NewLine + _lines[i];

            OutputText.text = toShow;
        }
    }
}