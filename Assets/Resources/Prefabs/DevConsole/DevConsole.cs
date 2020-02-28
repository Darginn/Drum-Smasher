using DrumSmasher.Prefab.DevConsole.Commands;
using DrumSmasher.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Prefab.DevConsole
{
    public class DevConsole : MonoBehaviour
    {
        public Text OutputText;
        public InputField InputField;

        private List<string> _lines;


        private const int _maxLinesOnScreen = 26;
        private const int _maxLineLength = 9999;

        private readonly List<ICommand> _commands = new List<ICommand>()
        {
            new ApproachRateCommand(),
            new AutoplayCommand(),
        };

        // Start is called before the first frame update
        void Start()
        {
            _lines = new List<string>();
            TitleScreenSettings tss = SettingsManager.SettingsStorage["TitleScreen"] as TitleScreenSettings;

            if (tss.Data.DefaultConsoleMessage != null)
                WriteLine(tss.Data.DefaultConsoleMessage);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) && InputField.text.Length > 0)
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

        private void ParseCommand(string line)
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

        private void RefreshLines()
        {
            int max = Math.Min(_maxLinesOnScreen - 1, _lines.Count - 1);
            string toShow = _lines[max];

            for (int i = max - 1; i >= 0; i--)
                toShow += Environment.NewLine + _lines[i];

            OutputText.text = toShow;
        }
    }
}