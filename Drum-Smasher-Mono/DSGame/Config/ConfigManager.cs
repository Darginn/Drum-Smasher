using Drum_Smasher_Mono.DSGame.Config.Bindable;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IniFileParser;
using IniFileParser.Model;
using Microsoft.Xna.Framework.Input;
using Drum_Smasher_Mono.DSGame.Scheduling;

namespace Drum_Smasher_Mono.DSGame.Config
{
    public static class ConfigManager
    {
        /// <summary>
        /// The game directory
        /// </summary>
        static string _gameDirectory;
        internal static Bindable<string> GameDirectory { get; private set; }

        /// <summary>
        /// Debug log directory
        /// </summary>
        static string _logsDirectory;
        internal static Bindable<string> LogsDirectory { get; private set; }

        /// <summary>
        /// The data directory
        /// </summary>
        static string _dataDirectory;
        internal static Bindable<string> DataDirectory { get; private set; }

        /// <summary>
        /// The song directory
        /// </summary>
        static string _songDirectory;
        internal static Bindable<string> SongDirectory { get; private set; }

        /// <summary>
        /// The username of the player
        /// </summary>
        internal static Bindable<string> Username { get; private set; }

        /// <summary>
        /// The master volume of the game.
        /// </summary>
        internal static BindableInt VolumeGlobal { get; private set; }

        /// <summary>
        /// The SFX volume of the game.
        /// </summary>
        internal static BindableInt VolumeEffect { get; private set; }

        /// <summary>
        /// The Music volume of the gamne.
        /// </summary>
        internal static BindableInt VolumeMusic { get; private set; }

        /// <summary>
        /// The height of the window.
        /// </summary>
        internal static BindableInt WindowHeight { get; private set; }

        /// <summary>
        /// The width of the window.
        /// </summary>
        internal static BindableInt WindowWidth { get; private set; }

        /// <summary>
        /// Is the window fullscreen?
        /// </summary>
        internal static Bindable<bool> WindowFullScreen { get; private set; }

        /// <summary>
        /// Should the game display the FPS Counter?
        /// </summary>
        internal static Bindable<bool> FpsCounter { get; private set; }

        /// <summary>
        /// The selected judgement window preset
        /// </summary>
        internal static Bindable<string> JudgementWindows { get; private set; }

        /// <summary>
        /// Keybinding for leftward navigation.
        /// </summary>
        internal static Bindable<Keys> KeyNavigateLeft { get; private set; }

        /// <summary>
        /// Keybinding for rightward navigation.
        /// </summary>
        internal static Bindable<Keys> KeyNavigateRight { get; private set; }

        /// <summary>
        /// Keybinding for upward navigation.
        /// </summary>
        internal static Bindable<Keys> KeyNavigateUp { get; private set; }

        /// <summary>
        /// Keybinding for downward navigation.
        /// </summary>
        internal static Bindable<Keys> KeyNavigateDown { get; private set; }

        /// <summary>
        /// Keybinding for backward navigation.
        /// </summary>
        internal static Bindable<Keys> KeyNavigateBack { get; private set; }

        /// <summary>
        /// Keybinding for selection in navigation interface.
        /// </summary>
        internal static Bindable<Keys> KeyNavigateSelect { get; private set; }

        /// <summary>
        /// Keybindings for Taiko
        /// </summary>
        internal static Bindable<Keys> OuterLeftDrum { get; private set; }
        internal static Bindable<Keys> OuterRightDrum { get; private set; }
        internal static Bindable<Keys> InnerLeftDrum { get; private set; }
        internal static Bindable<Keys> InnerRightDrum { get; private set; }

        /// <summary>
        /// The key pressed to pause and menu-back.
        /// </summary>
        internal static Bindable<Keys> KeyPause { get; private set; }

        /// <summary>
        /// Dictates whether or not this is the first write of the file for the current game session.
        /// (Not saved in Config)
        /// </summary>
        static bool FirstWrite { get; set; }

        /// <summary>
        /// The last time we've wrote config.
        /// </summary>
        static long LastWrite { get; set; }

        /// <summary>
        /// Important!
        /// Responsible for initializing directory properties,
        /// writing a new config file if it doesn't exist and also reading config files.
        /// This should be the one of the first things that is called upon game launch.
        /// </summary>
        public static void Initialize()
        {
            _gameDirectory = Directory.GetCurrentDirectory();


            _logsDirectory = _gameDirectory + "/Logs";
            Directory.CreateDirectory(_logsDirectory);

            _dataDirectory = _gameDirectory + "/Data";
            Directory.CreateDirectory(_dataDirectory);
            Directory.CreateDirectory(_dataDirectory + "/r/");

            _songDirectory = _gameDirectory + "/Songs";
            Directory.CreateDirectory(_songDirectory);

            // If we already have a config file, we'll just want to read that.
            ReadConfigFile();
            Logger.Log("Config file successfully read.", LogLevel.Info);
        }

        /// <summary>
        /// Reads a dsgame.cfg file and sets all of the successfully read values.
        /// At the end of reading, we write the config file, changing any invalid data/
        /// </summary>
        static void ReadConfigFile()
        {
            // We'll want to write a dsgame.cfg file if it doesn't already exist.
            // There's no need to read the config file afterwards, since we already have
            // all of the default values.
            if (!File.Exists(_gameDirectory + "/dsgame.cfg"))
                File.WriteAllText(_gameDirectory + "/dsgame.cfg", "; Drum Smasher Configuration File");

            var data = new IniFileParser.IniFileParser(new ConcatenateDuplicatedKeysIniDataParser()).ReadFile(_gameDirectory + "/dsgame.cfg")["Config"];

            // Read / Set Config Values
            // NOTE: MAKE SURE TO SET THE VALUE TO AUTO-SAVE WHEN CHANGING! THIS ISN'T DONE AUTOMATICALLY.
            // YOU CAN DO THIS DOWN BELOW, AFTER THE CONFIG HAS WRITTEN FOR THE FIRST TIME.
            GameDirectory = ReadSpecialConfigType(SpecialConfigType.Directory, @"GameDirectory", _gameDirectory, data);
            LogsDirectory = ReadSpecialConfigType(SpecialConfigType.Directory, @"LogsDirectory", _logsDirectory, data);
            DataDirectory = ReadSpecialConfigType(SpecialConfigType.Directory, @"DataDirectory", _dataDirectory, data);
            SongDirectory = ReadSpecialConfigType(SpecialConfigType.Directory, @"SongDirectory", _songDirectory, data);
            Username = ReadValue(@"Username", "Player", data);
            VolumeGlobal = ReadInt(@"VolumeGlobal", 50, 0, 100, data);
            VolumeEffect = ReadInt(@"VolumeEffect", 20, 0, 100, data);
            VolumeMusic = ReadInt(@"VolumeMusic", 50, 0, 100, data);
            WindowHeight = ReadInt(@"WindowHeight", 768, 600, short.MaxValue, data);
            WindowWidth = ReadInt(@"WindowWidth", 1366, 800, short.MaxValue, data);
            WindowFullScreen = ReadValue(@"WindowFullScreen", false, data);
            FpsCounter = ReadValue(@"FpsCounter", false, data);
            KeyNavigateLeft = ReadValue(@"KeyNavigateLeft", Keys.Left, data);
            KeyNavigateRight = ReadValue(@"KeyNavigateRight", Keys.Right, data);
            KeyNavigateUp = ReadValue(@"KeyNavigateUp", Keys.Up, data);
            KeyNavigateDown = ReadValue(@"KeyNavigateDown", Keys.Down, data);
            KeyNavigateBack = ReadValue(@"KeyNavigateBack", Keys.Escape, data);
            KeyNavigateSelect = ReadValue(@"KeyNavigateSelect", Keys.Enter, data);
            OuterLeftDrum = ReadValue(@"OuterLeftDrum", Keys.D, data);
            OuterRightDrum = ReadValue(@"OuterRightDrum", Keys.K, data);
            InnerLeftDrum = ReadValue(@"InnerLeftDrum", Keys.F, data);
            InnerRightDrum = ReadValue(@"InnerRightDrum", Keys.J, data);
            KeyPause = ReadValue(@"KeyPause", Keys.Escape, data);
            JudgementWindows = ReadValue("JudgementWindows", "", data);

            // Have to do this manually...
            if (string.IsNullOrEmpty(Username.Value))
                Username.Value = "Player";

            // Write the config file with all of the changed/invalidated data.
            Task.Run(async () => await WriteConfigFileAsync())
                .ContinueWith(t =>
                {
                    // SET AUTO-SAVE FUNCTIONALITY FOR EACH BINDED VALUE.
                    GameDirectory.ValueChanged += AutoSaveConfiguration;
                    LogsDirectory.ValueChanged += AutoSaveConfiguration;
                    DataDirectory.ValueChanged += AutoSaveConfiguration;
                    SongDirectory.ValueChanged += AutoSaveConfiguration;
                    Username.ValueChanged += AutoSaveConfiguration;
                    VolumeGlobal.ValueChanged += AutoSaveConfiguration;
                    VolumeEffect.ValueChanged += AutoSaveConfiguration;
                    VolumeMusic.ValueChanged += AutoSaveConfiguration;
                    WindowHeight.ValueChanged += AutoSaveConfiguration;
                    WindowWidth.ValueChanged += AutoSaveConfiguration;
                    WindowFullScreen.ValueChanged += AutoSaveConfiguration;
                    FpsCounter.ValueChanged += AutoSaveConfiguration;
                    KeyNavigateLeft.ValueChanged += AutoSaveConfiguration;
                    KeyNavigateRight.ValueChanged += AutoSaveConfiguration;
                    KeyNavigateUp.ValueChanged += AutoSaveConfiguration;
                    KeyNavigateDown.ValueChanged += AutoSaveConfiguration;
                    KeyNavigateBack.ValueChanged += AutoSaveConfiguration;
                    KeyNavigateSelect.ValueChanged += AutoSaveConfiguration;
                    OuterLeftDrum.ValueChanged += AutoSaveConfiguration;
                    OuterRightDrum.ValueChanged += AutoSaveConfiguration;
                    InnerLeftDrum.ValueChanged += AutoSaveConfiguration;
                    InnerRightDrum.ValueChanged += AutoSaveConfiguration;
                    KeyPause.ValueChanged += AutoSaveConfiguration;
                    JudgementWindows.ValueChanged += AutoSaveConfiguration;
                });
        }

        /// <summary>
        /// Reads a Bindable<T>. Works on all types.
        /// </summary>
        /// <returns></returns>
        static Bindable<T> ReadValue<T>(string name, T defaultVal, KeyDataCollection ini)
        {
            var binded = new Bindable<T>(name, defaultVal);
            var converter = TypeDescriptor.GetConverter(typeof(T));

            // Attempt to parse the value and default it if it can't.
            try
            {
                binded.Value = (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, ini[name]);
            }
            catch (Exception e)
            {
                binded.Value = defaultVal;
            }

            return binded;
        }

        /// <summary>
        /// Reads an Int32 to a BindableInt
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultVal"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="ini"></param>
        /// <returns></returns>
        static BindableInt ReadInt(string name, int defaultVal, int min, int max, KeyDataCollection ini)
        {
            var binded = new BindableInt(name, defaultVal, min, max);

            // Try to read the int.
            try
            {
                binded.Value = int.Parse(ini[name]);
            }
            catch (Exception e)
            {
                binded.Value = defaultVal;
            }

            return binded;
        }

        /// <summary>
        /// Reads a special configuration string type. These values need to be read and written in a
        /// certain way.
        /// </summary>
        /// <returns></returns>
        static Bindable<string> ReadSpecialConfigType(SpecialConfigType type, string name, string defaultVal, KeyDataCollection ini)
        {
            var binded = new Bindable<string>(name, defaultVal);

            try
            {
                // Get parsed config value.
                var parsedVal = ini[name];

                switch (type)
                {
                    case SpecialConfigType.Directory:
                        if (Directory.Exists(parsedVal))
                            binded.Value = parsedVal;
                        else
                        {
                            // Make sure the default directory is created.
                            Directory.CreateDirectory(defaultVal);
                            throw new ArgumentException();
                        }

                        break;
                    case SpecialConfigType.Path:
                        if (File.Exists(parsedVal))
                            binded.Value = parsedVal;
                        else
                            throw new ArgumentException();
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
            catch (Exception e)
            {
                binded.Value = defaultVal;
            }

            return binded;
        }

        /// <summary>
        /// Config Autosave functionality for Bindable<T>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="d"></param>
        static void AutoSaveConfiguration<T>(object sender, BindableValueChangedEventArgs<T> d)
        {
            // ReSharper disable once ArrangeMethodOrOperatorBody
            CommonTaskScheduler.Add(CommonTask.WriteConfig);
        }

        /// <summary>
        /// Takes all of the current values from the ConfigManager class and creates a file with them.
        /// This will automatically be called whenever a configuration value is changed in the code.
        /// </summary>
        internal static async Task WriteConfigFileAsync()
        {
            // Tracks the number of attempts to write the file it has made.
            var attempts = 0;

            // Don't do anything if the file isn't ready.
            while (!IsFileReady(GameDirectory + "/dsgame.cfg") && !FirstWrite)
            {
            }

            var sb = new StringBuilder();

            // Top file information
            // sb.AppendLine("; Drum Smasher Configuration File");
            sb.AppendLine("; Last Updated On: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
            sb.AppendLine("[Config]");
            sb.AppendLine("; Drum Smasher Configuration Values");

            // For every line we want to append "PropName = PropValue" to the string
            foreach (var prop in typeof(ConfigManager).GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (prop.Name == "FirstWrite" || prop.Name == "LastWrite")
                    continue;

                try
                {
                    sb.AppendLine(prop.Name + " = " + prop.GetValue(null));
                }
                catch (Exception e)
                {
                    sb.AppendLine(prop.Name + " = ");
                }
            }

            try
            {
                // Create a new stream
                var sw = new StreamWriter(GameDirectory + "/dsgame.cfg")
                {
                    AutoFlush = true
                };

                // Write to file and close it.;
                await sw.WriteLineAsync(sb.ToString());
                sw.Close();

                FirstWrite = false;
            }
            catch (Exception e)
            {
                // Try to write the file again 3 times.
                while (attempts != 2)
                {
                    attempts++;

                    // Create a new stream
                    var sw = new StreamWriter(GameDirectory + "/dsgame.cfg")
                    {
                        AutoFlush = true
                    };

                    // Write to file and close it.
                    await sw.WriteLineAsync(sb.ToString());
                    sw.Close();
                }

                // If too many attempts were made.
                if (attempts == 2)
                    Logger.Log("Too many write attempts to the config file have been made.", LogLevel.Error);
            }

            LastWrite = GameClient.TimeRunning;
        }

        /// <summary>
        /// Checks if the file is ready to be written to.
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public static bool IsFileReady(string sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (var inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return (inputStream.Length > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Enum containing special config types. We want to read and default these in
    /// a very particular way.
    /// </summary>
    internal enum SpecialConfigType
    {
        Directory,
        Path,
    }
}
