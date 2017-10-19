using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityWAD;
using UnityEditor;

public class SpriteGenTester : MonoBehaviour
{

    public Texture ResultTexture;

    public string WADFilename;

    public string[] AvailableSprites;
    public List<WADEntry> SpriteEntries = new List<WADEntry>();

    public int TestSpriteIndex;

    [ExecuteInEditMode]
    public void Awake()
    {
        using (var fs = new FileStream(WADFilename, FileMode.Open))
        {
            var wad = WADReader.GetInfo(fs);
            bool adding = false;
            foreach (var e in wad.Entries)
            {
                if (e.Name == "S_START")
                {
                    adding = true;
                    continue;
                }
                if (e.Name == "S_END")
                {
                    break;
                }

                if (!adding) continue;
                SpriteEntries.Add(e);
            }

            AvailableSprites = new string[SpriteEntries.Count];
            for (int i = 0; i < AvailableSprites.Length; i++)
                AvailableSprites[i] = SpriteEntries[i].Name;

        }
    }

    public void Generate()
    {
        using (var fs = new FileStream(WADFilename, FileMode.Open))
        {
            var pal = WADReader.GetPalette(fs, 0);
            var sd = WADReader.GetPictureSprite(fs, SpriteEntries[TestSpriteIndex]);
            ResultTexture = SpriteGenerator.GenerateSprite(sd, pal);
        }
    }
}
