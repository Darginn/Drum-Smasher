using OpenToolkit.Graphics.ES30;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DSLauncher.UI
{
    public class MainWindow : GameWindow
    {
        private static MainWindow _default;

        public static MainWindow Default
        {
            get
            {
                if (_default == null)
                {
                    GameWindowSettings gws = new GameWindowSettings()
                    {
                        IsMultiThreaded = true,
                        RenderFrequency = 60,
                        UpdateFrequency = 60
                    };
                    NativeWindowSettings nws = new NativeWindowSettings()
                    {
                        StartFocused = true,
                        StartVisible = true,
                        WindowBorder = WindowBorder.Hidden,
                        WindowState = WindowState.Normal,
                        Title = "Test window",
                        Location = new OpenToolkit.Mathematics.Vector2i(600, 450),
                        Profile = ContextProfile.Core
                    };
                    _default = new MainWindow(gws, nws);
                }

                return _default;
            }
        }

        public int VertexBufferObject { get; private set; }
        public int VertexArrayObject { get; private set; }

        private Shader _shader;

        public MainWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
        {
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            
            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenBuffer();

            _shader = new Shader("shader.vert", "shader.frag");
            
            base.OnLoad();
        }

        private float[] _vertices = {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f,  0.5f, 0.0f  //Top vertex
        };

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            SwapBuffers();

            Draw(args);
            base.OnUpdateFrame(args);
        }

        private void Draw(FrameEventArgs args)
        {
            // ..:: Initialization code (done once (unless your object frequently changes)) :: ..
            // 1. bind Vertex Array Object
            GL.BindVertexArray(VertexArrayObject);

            // 2. copy our vertices array in a buffer for OpenGL to use
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            // 3. then set our vertex attributes pointers
            _shader.Use();
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        protected override void OnRefresh()
        {
            Task.Run(() => Console.WriteLine("Refreshed"));

            base.OnRefresh();
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);

            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);

            base.OnResize(e);
        }
    }

    public class Shader : IDisposable
    {
        public int Handle;

        public string VertexShaderSource;
        public string FragmentShaderSource;

        public int VertexShader;
        public int FragmentShader;
        private bool disposedValue = false;


        public Shader(string vertexPath, string fragmentPath)
        {
            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }


            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource); 


            GL.CompileShader(VertexShader);

            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != System.String.Empty)
                Console.WriteLine(infoLogVert);

            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);

            if (infoLogFrag != System.String.Empty)
                Console.WriteLine(infoLogFrag);


            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);


            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
