using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Lab06;

public class SkyBox
{
    private readonly int _vao;
    private readonly Shader _shader;
    private readonly CubeTexture _texture;
    private readonly Camera _camera;
    private readonly int _pvmLocation;
    private readonly int _textureLocation;
    private readonly Mesh _mesh;

    public SkyBox(ref CubeTexture texture, ref Camera camera)
    {
        _shader = new Shader("./Shaders/Vertex/SkyBox.vert", "./Shaders/Fragment/SkyBox.frag");
        _mesh = ObjLoader.Load("./models/cube.obj", new Dictionary<string, Texture>(),
            new Dictionary<string, Texture>(), new Dictionary<string, Texture>())[0];
        _texture = texture;
        _camera = camera;
        _pvmLocation = GL.GetUniformLocation(_shader.Handle, "matPVM");
        _textureLocation = GL.GetUniformLocation(_shader.Handle, "texture0");
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        var vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, _mesh.Vertices.Count * sizeof(float) * 4, _mesh.Vertices.ToArray(),
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindVertexArray(0);
        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Init: " + error);
        }
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);


        _texture.Use();


        var pvm = Matrix4.CreateScale(200f, 200f, 200f) * Matrix4.CreateTranslation(_camera.Position) *
                  _camera.GetViewMatrix() * _camera.GetProjectionMatrix();

        _shader.Use();

        GL.UniformMatrix4(_pvmLocation, false, ref pvm);
        GL.Uniform1(_textureLocation, 0);
        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Before: " + error);
        }

        GL.DrawArrays(PrimitiveType.Triangles, 0, _mesh.Vertices.Count);
        error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("After: " + error);
        }

        GL.BindVertexArray(0);
        GL.DisableVertexAttribArray(0);
    }
}