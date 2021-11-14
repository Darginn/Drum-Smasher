using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.IO
{
    public abstract class BaseFile
    {
        /// <summary>
        /// This stream is used for writing and saving,
        /// it will be always null except if <see cref="OnLoading"/> or <see cref="OnSaving"/> is running
        /// </summary>
        protected FileStream _stream;

        /// <summary>
        /// Loads the specified file
        /// </summary>
        /// <param name="file">File path</param>
        /// <returns>
        /// <para>True - File was found</para>
        /// <para>False - File was not found</para>
        /// </returns>
        public virtual bool Load(string file)
        {
            if (!File.Exists(file))
                return false;

            using (FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    _stream = fstream;
                    OnLoading();
                }
                finally
                {
                    _stream = null;
                }
            }

            return true;
        }

        /// <summary>
        /// Saves the current file
        /// </summary>
        /// <param name="file">File path</param>
        public virtual void Save(string file)
        {
            using (FileStream fstream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                try
                {
                    _stream = fstream;
                    OnSaving();
                }
                finally
                {
                    _stream = null;
                }
            }
        }

        #region Write
        protected abstract void Write(byte b);
        protected abstract void Write(params byte[] data);

        protected abstract void Write(short v);
        protected abstract void Write(int v);
        protected abstract void Write(long v);

        protected abstract void Write(float v);
        protected abstract void Write(double v);

        protected abstract void Write(string v);

        protected abstract void Write(TimeSpan v);
        protected abstract void Write(DateTime v);
        #endregion

        #region Read
        protected abstract byte ReadByte();
        protected abstract byte[] ReadBytes();

        protected abstract short ReadShort();
        protected abstract int ReadInt();
        protected abstract long ReadLong();

        protected abstract float ReadFloat();
        protected abstract double ReadDouble();

        protected abstract string ReadString();
        
        protected abstract TimeSpan ReadTimeSpan();
        protected abstract DateTime ReadDateTime();
        #endregion

        protected abstract void OnLoading();
        protected abstract void OnSaving();

        /// <summary>
        /// Ensures that we can read a specific size of bytes,
        /// if we want to read too much it will throw a <see cref="InvalidOperationException"/>
        /// </summary>
        /// <param name="size">bytes to read</param>
        protected void EnsureLength(int size)
        {
            if (_stream.Position + size >= _stream.Length)
                throw new InvalidOperationException("End of stream");
        }
    }
}
