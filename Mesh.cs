using System.Net.Mime;
using OpenTK.Mathematics;

namespace Lab06;

public class Mesh
{
    public List<Vector4> Vertices { get; set; }
    public List<Vector3> TextureVertices { get; set; }
    public List<Vector3> Normals { get; set; }
    public List<Vector3> Tangents { get; set; }
    public List<Vector3> Bitangents { get; set; }
    public Texture? Texture { get; set; }
    public Texture? NormalMap { get; set; }
    
    public Texture? SpecularMap { get; set; }

    public Mesh(List<Vector4> vertices, List<Vector3> textureVertices, List<Vector3> normals, Texture? texture=null, Texture? normalMap=null, Texture? specularMap=null)
    {
        Vertices = vertices;
        TextureVertices = textureVertices;
        Normals = normals;
        Texture = texture;
        NormalMap = normalMap;
        SpecularMap = specularMap;
        Tangents = new List<Vector3>();
        Bitangents = new List<Vector3>();
    }
}