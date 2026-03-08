using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace rurik;

public class Textures
{
    public Dictionary<string, Texture2D> TextureMap { get; private set; } = new Dictionary<string, Texture2D>();

    public void LoadContent(ContentManager content)
    {
        // Add commonly used textures here
        TextureMap["Map2"] = content.Load<Texture2D>("Map2");
        TextureMap["map"] = TextureMap["Map2"];
    }

    public Texture2D? GetTexture(string name)
    {
        if (TextureMap.TryGetValue(name, out var texture))
        {
            return texture;
        }
        return null;
    }
}
