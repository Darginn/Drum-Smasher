using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DevConsole.Commands;

namespace Assets.Scripts.DevConsole
{
    public class DevConsole : MonoBehaviour
    {
        public static DevConsole Instance { get; private set; }

        public static CommandHandler Commands => Instance._commands;

        [SerializeField] GameObject _content;
        [SerializeField] Text _templateText;
        [SerializeField] InputField _userInput;
        [SerializeField] int _maxLines = 50;

        List<Text> _lines;
        Color _fontColor;

        CommandHandler _commands;

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="line"></param>
        public void WriteLine(string line)
        {
            Text text = Instantiate(_templateText.gameObject).GetComponent<Text>();
            text.text = line;
            text.gameObject.SetActive(true);

            _lines.Add(text);
            text.transform.SetParent(_content.transform);

            if (_lines.Count > _maxLines)
                ClearLine(_lines.Count - 1);
        }

        /// <summary>
        /// <inheritdoc cref="WriteLine(string)"/>
        /// </summary>
        public void WriteLine(string line, params object[] args)
        {
            WriteLine(string.Format(line, args));
        }

        /// <summary>
        /// Appends a string to the last written line or creates a new line if no lines exist
        /// </summary>
        public void Write(string input)
        {
            if (_lines.Count == 0)
            {
                WriteLine(input);
                return;
            }

            Text text = _lines[_lines.Count - 1];
            text.text += input;
        }

        /// <summary>
        /// <inheritdoc cref="Write(string)"/>
        /// </summary>
        public void Write(string input, params object[] args)
        {
            Write(string.Format(input, args));
        }


        /// <summary>
        /// Clears a specific amount of lines
        /// </summary>
        /// <param name="lineCount">Lines to clear</param>
        /// <param name="order">The order in which the lines will be cleared</param>
        public void Clear(int lineCount = 1, ClearOrder order = ClearOrder.OldestToNewest)
        {
            if (_lines.Count == 0)
                return;

            lineCount = Math.Min(lineCount, _lines.Count);

            switch (order)
            {
                default:
                case ClearOrder.OldestToNewest:
                    for (int i = 0; i < lineCount; i++)
                        ClearLine(0);
                    break;

                case ClearOrder.NewestToOldest:
                    for (int i = 0; i < lineCount; i++)
                        ClearLine(_lines.Count - 1);
                    break;
            }

        }

        /// <summary>
        /// Clears all lines
        /// </summary>
        public void ClearAll()
        {
            while (_lines.Count > 0)
                ClearLine(0);
        }

        /// <summary>
        /// Clears the line at the specified index
        /// </summary>
        /// <param name="line">Line index</param>
        public void ClearLine(int line)
        {
            Text text = _lines.ElementAt(line);
            Destroy(text.gameObject);
            _lines.RemoveAt(line);
        }


        /// <summary>
        /// Sets the current font color and updates all lines with it
        /// </summary>
        public void SetExistingFontColor(Color c)
        {
            SetFontColor(c);

            for (int i = 0; i < _lines.Count; i++)
                _lines[i].color = c;
        }

        /// <summary>
        /// Sets the current font color
        /// </summary>
        public void SetFontColor(Color c)
        {
            _fontColor = c;
        }

        /// <summary>
        /// Gets the current font color
        /// </summary>
        public Color GetFontColor()
        {
            return _fontColor;
        }

        /// <summary>
        /// Gets the default font color
        /// </summary>
        public Color GetDefaultFontColor()
        {
            return _templateText.color;
        }

        /// <summary>
        /// Resets the font color to the default color
        /// </summary>
        public void ResetFontColor()
        {
            SetFontColor(_templateText.color);
        }

        /// <summary>
        /// Resets the current font color and all existing lines font color to the default color
        /// </summary>
        public void ResetExistingFontColor()
        {
            SetExistingFontColor(_templateText.color);
        }

        /// <summary>
        /// Invoked when the user submits a string in the console
        /// </summary>
        public void OnUserInputSubmit()
        {
            string line = _userInput.text;
            _userInput.text = string.Empty;

            if (string.IsNullOrEmpty(line))
                return;

            WriteLine(line);
            _commands.TryParseLineToCommand(line);
        }

        void Start()
        {
            
        }

        void Awake()
        {
            Instance = this;
            _commands = new CommandHandler();
            _lines = new List<Text>();
            ResetFontColor();
        }

        void OnLevelWasLoaded(int level)
        {
            ClearAll();
        }

        public enum ClearOrder
        {
            OldestToNewest,
            NewestToOldest
        }
    }
}
