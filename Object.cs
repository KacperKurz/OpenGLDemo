using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Lab06;

public class Object
{
    public Object(ref Shader shader, Matrix4 model, Mesh mesh, ref Camera camera)
    {
        #if DEBUG
        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Before object init: "+error);
        }
        #endif
        Shader = shader;
        Model = model;
        Mesh = mesh;
        Camera = camera;
        ProjectionLocation = GL.GetUniformLocation(Shader.Handle, "projection");
        ViewLocation = GL.GetUniformLocation(Shader.Handle, "view");
        ModelLocation = GL.GetUniformLocation(Shader.Handle, "model");
        TextureLocation = GL.GetUniformLocation(Shader.Handle, "texture0");
        NormalMapLocation = GL.GetUniformLocation(Shader.Handle, "texture1");
        SpecularMapLocation = GL.GetUniformLocation(Shader.Handle, "texture2");
        LightLocation = GL.GetUniformLocation(Shader.Handle, "lightPos");
        ViewPositionLocation = GL.GetUniformLocation(Shader.Handle, "viewPos");
        UseTextureLocation = GL.GetUniformLocation(Shader.Handle, "useTexture");
        UseNormalMapLocation = GL.GetUniformLocation(Shader.Handle, "useNormalMap");
        UseSpecularMapLocation = GL.GetUniformLocation(Shader.Handle, "useSpecularMap");
        UseShadingLocation = GL.GetUniformLocation(Shader.Handle, "useShading");
        DebugModeLocation = GL.GetUniformLocation(Shader.Handle, "debugMode");
        TimeLocation = GL.GetUniformLocation(Shader.Handle, "time");
        VertexArray = GL.GenVertexArray();
        GL.BindVertexArray(VertexArray);
        VertexBuffer = GL.GenBuffer();
        TextureUvBuffer = GL.GenBuffer();
        NormalBuffer = GL.GenBuffer();
        TangentBuffer = GL.GenBuffer();
        BitangentBuffer = GL.GenBuffer();
        
        GL.BindBuffer(BufferTarget.ArrayBuffer,VertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Mesh.Vertices.Count * sizeof(float) * 4, Mesh.Vertices.ToArray(), BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, TextureUvBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Mesh.TextureVertices.Count * sizeof(float) * 3, Mesh.TextureVertices.ToArray(), BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(1);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Mesh.Normals.Count * sizeof(float) * 3, Mesh.Normals.ToArray(), BufferUsageHint.StaticDraw );
        GL.VertexAttribPointer(2,3,VertexAttribPointerType.Float, false, 3 * sizeof(float),0);
        GL.EnableVertexAttribArray(2);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, TangentBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Mesh.Tangents.Count * sizeof(float) * 3, Mesh.Tangents.ToArray(), BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(3,3,VertexAttribPointerType.Float, false, 3* sizeof(float), 0);
        GL.EnableVertexAttribArray(3);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, BitangentBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Mesh.Bitangents.Count * sizeof(float) * 3, Mesh.Bitangents.ToArray(), BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(4,3,VertexAttribPointerType.Float, false, 3* sizeof(float), 0);
        GL.EnableVertexAttribArray(4);
        
        
        
        GL.BindVertexArray(0);
        
        #if DEBUG
        error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Object init: "+error);
        }
        #endif
    }

    public Shader Shader { get; set; }
    private Matrix4 Model { get; set; }
    private Mesh Mesh { get; set; }
    private int ProjectionLocation { get; set; }
    private int ViewLocation { get; set; }
    private int ModelLocation { get; set; }
    private int TextureLocation { get; set; }
    private int NormalMapLocation { get; set; }
    private int SpecularMapLocation { get; set; }
    private int LightLocation { get; set; }
    private int ViewPositionLocation { get; set; }
    private int UseTextureLocation { get; set; }
    private int UseNormalMapLocation { get; set; }
    private int UseSpecularMapLocation { get; set; }
    private int UseShadingLocation { get; set; }
    private int DebugModeLocation { get; set; }
    private Camera Camera { get; set; }
    private int VertexBuffer { get; set; }
    private int TextureUvBuffer { get; set; }
    private int NormalBuffer { get; set; }
    public int TangentBuffer { get; set; }
    public int BitangentBuffer { get; set; }
    private int VertexArray { get; set; }
    private int TimeLocation { get; set; }

    public void Draw(double time, bool useTextures, bool useNormalMaps, bool useSpecularMaps, bool useShading, int debugMode = 0)
    {
        GL.BindVertexArray(VertexArray);
        
        var projection = Camera.GetProjectionMatrix();
        var view = Camera.GetViewMatrix();
        var model = Model;
        Shader.Use();
        
        #if DEBUG
        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Before assign texture: "+error);
        }
        #endif

        if (Mesh.Texture == null || useTextures==false)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Uniform1(UseTextureLocation, 0);
        }
        else
        {
            Mesh.Texture.Use();
            GL.Uniform1(UseTextureLocation, 1);
        }
        
        #if DEBUG
        error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Before assign normalMap: "+error);
        }
        #endif

        if (Mesh.NormalMap == null || useNormalMaps==false)
        {
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Uniform1(UseNormalMapLocation, 0);
        }
        else
        {
            Mesh.NormalMap.Use(TextureUnit.Texture1);
            GL.Uniform1(UseNormalMapLocation, 1);
        }
        
        #if DEBUG
        error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("after assign normalMap: "+error);
        }
        #endif
        
        if (Mesh.SpecularMap == null || useSpecularMaps==false)
        {
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Uniform1(UseSpecularMapLocation, 0);
        }
        else
        {
            Mesh.SpecularMap.Use(TextureUnit.Texture2);
            GL.Uniform1(UseSpecularMapLocation, 1);
        }
        
        GL.Uniform1(UseShadingLocation, useShading?1:0);
        GL.Uniform1(DebugModeLocation, debugMode);
        GL.Uniform1(TextureLocation, 0);
        GL.Uniform1(NormalMapLocation, 1);
        GL.Uniform1(SpecularMapLocation, 2);
        GL.Uniform1(TimeLocation, (float)time);
        GL.Uniform3(LightLocation,new Vector3(1.0f,1.0f,0.0f));
        GL.Uniform3(ViewPositionLocation,Camera.Position);
        GL.UniformMatrix4(ProjectionLocation,false,ref projection);
        GL.UniformMatrix4(ViewLocation,false,ref view);
        GL.UniformMatrix4(ModelLocation,false,ref model);
        GL.DrawArrays(PrimitiveType.Triangles, 0,Mesh.Vertices.Count);
        GL.BindVertexArray(0);
        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.DisableVertexAttribArray(2);
        GL.DisableVertexAttribArray(3);
        GL.DisableVertexAttribArray(4);
        
        #if DEBUG
        error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Object draw: "+error);
        }
        #endif
    }
}