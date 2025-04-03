using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Lab06;

public class CubeTexture
{
    private int Handle { get; set; }

    public CubeTexture(string path)
    {
        Handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        
        #if DEBUG
        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("Before params : "+error);
        }
        #endif
        
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        // GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        
        #if DEBUG
        error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("After params: "+error);
        }
        #endif
        
        StbImage.stbi_set_flip_vertically_on_load(0); 
        var image = ImageResult.FromStream(File.OpenRead(path+"posx.jpg"), ColorComponents.RedGreenBlue);
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
        image = ImageResult.FromStream(File.OpenRead(path+"posy.jpg"), ColorComponents.RedGreenBlue);
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveY, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
        image = ImageResult.FromStream(File.OpenRead(path+"posz.jpg"), ColorComponents.RedGreenBlue);
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveZ, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
        image = ImageResult.FromStream(File.OpenRead(path+"negx.jpg"), ColorComponents.RedGreenBlue);
        GL.TexImage2D(TextureTarget.TextureCubeMapNegativeX, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
        image = ImageResult.FromStream(File.OpenRead(path+"negy.jpg"), ColorComponents.RedGreenBlue);
        GL.TexImage2D(TextureTarget.TextureCubeMapNegativeY, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
        image = ImageResult.FromStream(File.OpenRead(path+"negz.jpg"), ColorComponents.RedGreenBlue);
        GL.TexImage2D(TextureTarget.TextureCubeMapNegativeZ, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
    }

    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
    }
}