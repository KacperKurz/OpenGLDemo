using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lab06
{
    public class Window : GameWindow
    {
        private int _height;
        private int _width;
        private float _speed;
        private Vector2 _lastPos;
        private bool _firstMove = true;
        private Camera _camera;
        private readonly float _sensivity;
        private readonly List<Object> _scene = new();
        private double _time;
        private SkyBox _skyBox;
        private bool _useTextures = true;
        private bool _useNormalMaps = true;
        private bool _useSpecularMaps = true;
        private bool _useSkyBox = true;
        private bool _useShading = true;
        
        public Window(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings
                { ClientSize = (width, height), Title = title })
        {
            _width = width;
            _height = height;
            _speed = 1.5f;

            _sensivity = 1f;
        }
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (!IsFocused || CursorState!=CursorState.Grabbed) // check to see if the window is focused
            {
                return;
            }


            if (KeyboardState.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * _speed * (float)e.Time; //Forward 
            }
        
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * _speed * (float)e.Time; //Backwards
            }
        
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                _camera.Position -= Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * _speed * (float)e.Time; //Left
            }
        
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                _camera.Position += Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * _speed * (float)e.Time; //Right
            }
        
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * _speed * (float)e.Time; //Up 
            }
        
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _camera.Position -= _camera.Up * _speed * (float)e.Time; //Down
            }
        
            var mouse = MouseState;
            
            
            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);
        
                _camera.Yaw += deltaX * _sensivity;
                switch (_camera.Pitch)
                {
                    case > 89.0f:
                        _camera.Pitch = 89.0f;
                        break;
                    case < -89.0f:
                        _camera.Pitch = -89.0f;
                        break;
                    default:
                        _camera.Pitch -= deltaY * _sensivity;
                        break;
                }
            }
            _time += e.Time;
        }
        
        
        protected override void OnLoad()
        {
            base.OnLoad();
            CursorState = CursorState.Grabbed;
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            
            _camera = new Camera(new Vector3(-10,5,0),(float)_width/_height );
            
            var textureFiles = new DirectoryInfo("./Textures/").GetFiles().Where(x=>x.FullName.Contains("BaseColor")).Where(x=>x.FullName.Contains(".png"));

            var textures = textureFiles.ToDictionary(file => file.Name.Split("_BaseColor")[0].ToLower(), file => new Texture(file.FullName));

            Console.WriteLine("Successfully loaded "+textures.Count+" textures.");
            
            var normalMapFiles = new DirectoryInfo("./Textures/").GetFiles().Where(x=>x.FullName.Contains("Normal")).Where(x=>x.FullName.Contains(".png"));
            
            var normalMaps = normalMapFiles.ToDictionary(file => file.Name.Split("_Normal")[0].ToLower(), file => new Texture(file.FullName));
            
            Console.WriteLine("Successfully loaded "+textures.Count+" normal maps.");
            
            var specularMapFiles = new DirectoryInfo("./Textures/").GetFiles().Where(x=>x.FullName.Contains("Specular")).Where(x=>x.FullName.Contains(".png"));
            
            var specularMaps = specularMapFiles.ToDictionary(file => file.Name.Split("_Specular")[0].ToLower(), file => new Texture(file.FullName));
            
            Console.WriteLine("Successfully loaded "+textures.Count+" specular maps.");
            
            var shader = new Shader("./Shaders/Vertex/Shader.vert",
                "./Shaders/Fragment/Shader.frag");

            var scene = ObjLoader.Load("./models/bistro.obj", textures, normalMaps, specularMaps);
            
            Console.WriteLine("Successfully loaded "+scene.Count+" meshes.");

            foreach (var mesh in scene)
            {
                _scene.Add(
                    new Object(
                        ref shader,
                        Matrix4.Identity, 
                        mesh,
                        ref _camera
                    )
                );
            }
            
            var texture = new CubeTexture("./Textures/skybox/skybox5/");
            #if DEBUG
            var error = GL.GetError();
            if (error != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("Texture: "+error);
            }
            #endif
            _skyBox = new SkyBox(ref texture, ref _camera);

        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (var @object in _scene)
            {
                @object.Draw(_time, _useTextures,_useNormalMaps,_useSpecularMaps, _useShading);
            }
            if (_useSkyBox) 
            {
                GL.Disable(EnableCap.CullFace);
                _skyBox.Draw();
                GL.Enable(EnableCap.CullFace);
            }
            SwapBuffers();
        }
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            _height = e.Height;
            _width = e.Width;
            _camera.AspectRatio = (float)_width / _height;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Keys.Tab:
                    CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
                    break;
                case Keys.LeftShift:
                    _speed = 10f;
                    break;
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            switch (e.Key)
            {
                case Keys.LeftShift:
                    _speed = 1.5f;
                    break;
                case Keys.T:
                    _useTextures = !_useTextures;
                    break;
                case Keys.N:
                    _useNormalMaps = !_useNormalMaps;
                    break;
                case Keys.M:
                    _useSpecularMaps = !_useSpecularMaps;
                    break;
                case Keys.B:
                    _useSkyBox = !_useSkyBox;
                    break;
                case Keys.L:
                    _useShading = !_useShading;
                    break;
            }
        }

        protected override void OnUnload()
        {
            foreach (var @object in _scene)
            {
                @object.Shader.Dispose();
            }
        }
        
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }
        
    }
}