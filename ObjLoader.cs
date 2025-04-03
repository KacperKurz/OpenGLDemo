using System.Globalization;
using OpenTK.Mathematics;

namespace Lab06;

public static class ObjLoader
{
    public static List<Mesh> Load(string path, Dictionary<string, Texture> textures,
        Dictionary<string, Texture> normalMaps, Dictionary<string, Texture> specularMaps)
    {
        var textureIndex = -1;
        var vertices = new List<Vector4>();
        var textureVertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var indicies = new List<KeyValuePair<string, List<List<uint>>>>();

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Unable to open \"" + path + "\", does not exist.");
        }

        using (var streamReader = new StreamReader(path))
        {
            while (!streamReader.EndOfStream)
            {
                var words = new List<string>(streamReader.ReadLine().ToLower().Split(' '));
                words.RemoveAll(s => s == string.Empty);

                if (words.Count == 0)
                    continue;

                var type = words[0];
                words.RemoveAt(0);

                switch (type)
                {
                    // vertex
                    case "v":
                        vertices.Add(new Vector4(float.Parse(words[0],CultureInfo.InvariantCulture), float.Parse(words[1],CultureInfo.InvariantCulture),
                            float.Parse(words[2],CultureInfo.InvariantCulture), words.Count < 4 ? 1 : float.Parse(words[3],CultureInfo.InvariantCulture)));
                        break;

                    case "vt":
                        textureVertices.Add(new Vector3(float.Parse(words[0],CultureInfo.InvariantCulture), float.Parse(words[1],CultureInfo.InvariantCulture),
                            words.Count < 3 ? 0 : float.Parse(words[2],CultureInfo.InvariantCulture)));
                        break;

                    case "vn":
                        normals.Add(new Vector3(float.Parse(words[0],CultureInfo.InvariantCulture), float.Parse(words[1],CultureInfo.InvariantCulture), float.Parse(words[2],CultureInfo.InvariantCulture)));
                        break;

                    // face
                    case "f":
                        foreach (var comps in from w in words where w.Length != 0 select w.Split('/'))
                        {
                            // subtract 1: indices start from 1, not 0
                            indicies[textureIndex].Value[0].Add(uint.Parse(comps[0],CultureInfo.InvariantCulture) - 1);

                            if (comps.Length > 1 && comps[1].Length != 0)
                                indicies[textureIndex].Value[1].Add(uint.Parse(comps[1],CultureInfo.InvariantCulture) - 1);

                            if (comps.Length > 2)
                                indicies[textureIndex].Value[2].Add(uint.Parse(comps[2],CultureInfo.InvariantCulture) - 1);
                        }

                        break;
                    case "usemtl":
                        textureIndex += 1;
                        indicies.Add(new KeyValuePair<string, List<List<uint>>>(words[0].Split('.')[0].ToLower(),new List<List<uint>>{new(), new(), new()}));
                        break;
                }
            }
        }

        var result = new List<Mesh>();

        for (var i = 0; i <= textureIndex; ++i)
        {
            var mesh = new Mesh(indicies[i].Value[0].Select(x => vertices[(int)x]).ToList(),
                indicies[i].Value[1].Select(x => textureVertices[(int)x]).ToList(),
                indicies[i].Value[2].Select(x => normals[(int)x]).ToList(),
                textures.ContainsKey(indicies[i].Key) ? textures[indicies[i].Key] : null,
                normalMaps.ContainsKey(indicies[i].Key) ? normalMaps[indicies[i].Key] : null,
                specularMaps.ContainsKey(indicies[i].Key) ? specularMaps[indicies[i].Key] : null
                );

            #region tangents

            for (var j = 0; j < mesh.Vertices.Count; j += 3)
            {
            	Vector3 edge1 = (mesh.Vertices[j + 1] - mesh.Vertices[j]).Xyz;
            	Vector3 edge2 = (mesh.Vertices[j + 2] - mesh.Vertices[j]).Xyz;
            	Vector2 deltaUV1 = (mesh.TextureVertices[j + 1] - mesh.TextureVertices[j]).Xy;
            	Vector2 deltaUV2 = (mesh.TextureVertices[j + 2] - mesh.TextureVertices[j]).Xy;
            
            	float f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);
            
            	var tangent = new Vector3(
            		f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X),
            		f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y),
            		f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z)
            	);
            
            	var bitangent = new Vector3(
            		f * (-deltaUV2.X * edge1.X + deltaUV1.X * edge2.X),
            		f * (-deltaUV2.X * edge1.Y + deltaUV1.X * edge2.Y),
            		f * (-deltaUV2.X * edge1.Z + deltaUV1.X * edge2.Z)
            	);
            	
            	mesh.Tangents.Add(tangent);
                mesh.Tangents.Add(tangent);
                mesh.Tangents.Add(tangent);
            	mesh.Bitangents.Add(bitangent);
                mesh.Bitangents.Add(bitangent);
                mesh.Bitangents.Add(bitangent);
            }

            #endregion

            result.Add(mesh);
        }


        return result;
    }
}