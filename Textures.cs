using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace rurik;

public class Textures
{
    public Dictionary<string, Texture2D> TextureMap { get; private set; } = new Dictionary<string, Texture2D>();

    public void LoadContent(ContentManager content)
    {
        // Get the executable directory
        string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        // The Content directory is typically in the same directory as the executable
        string contentDirectory = Path.Combine(executableDirectory, "Content");
        
        Globals.Log("Textures.LoadContent(): contentDirectory=" + contentDirectory);
        
        if (!Directory.Exists(contentDirectory))
        {
            Globals.Log("Textures.LoadContent(): Content directory does not exist!");
            return;
        }
        
        int loadedCount = 0;
        foreach (string filePath in Directory.GetFiles(contentDirectory, "*.xnb"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            // Only load textures that match our action pattern (muster, move, attack, tax, build, scheme)
            ///if (fileName.StartsWith("muster-") || fileName.StartsWith("move-") ||
            //   fileName.StartsWith("attack-") || fileName.StartsWith("tax-") ||
            //    fileName.StartsWith("build-") || fileName.StartsWith("scheme-"))
            //{
                try
                {
                    TextureMap[fileName] = content.Load<Texture2D>(fileName);
                    Globals.Log("Textures.LoadContent(): Loaded " + fileName);
                    loadedCount++;
                }
                catch (Exception ex)
                {
                    Globals.Log("Textures.LoadContent(): Failed to load " + fileName + ": " + ex.Message);
                }
            //}
        }
        
        Globals.Log("Textures.LoadContent(): Loaded " + loadedCount + " action textures");
        
        // Add commonly used textures here
        TextureMap["Map2"] = content.Load<Texture2D>("Map2");
        TextureMap["map"] = TextureMap["Map2"];
        TextureMap["tradeBoom"] = TextureMap["tradeboom"];
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
