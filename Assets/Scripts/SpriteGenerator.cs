using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;
using UnityWAD;

namespace UnityWAD
{
    public static class SpriteGenerator
    {
        // Create a single sprite texture from given data
        public static Texture2D GenerateSprite(SpriteData spriteData, PaletteData paletteData)
        {
            Texture2D spriteTexture = null;

            switch (spriteData.Type)
            {
                case SpriteType.Picture:
                    List<int> columns = new List<int>();
                    int pos = 8;
                    for (int x = 0; x < spriteData.Width; x++)
                    {
                        columns.Add(BitConverter.ToInt32(spriteData.Data, pos));
                        pos += 4;
                    }

                    Color32[] textureData = new Color32[spriteData.Width * spriteData.Height];
                    var center = (spriteData.Width / 2);

                    for(int x=0;x<columns.Count;x++)
                    {
                        var postPos = columns[x];
                        while (spriteData.Data[postPos] != 255)
                        {
                            var row = (int)spriteData.Data[postPos];
                            postPos++;
                            var numPix = (int)spriteData.Data[postPos];
                            postPos +=2; // skip 1st byte of post
                            for (int i = 0; i < numPix; i++)
                            {
                                // TODO: Colours!
                                //Debug.Log(spriteData.XOffset + x + "," + ((spriteData.Height - spriteData.YOffset) + row + i) + "=" + (spriteData.XOffset + x + ((((spriteData.Height-1) - spriteData.YOffset) + row + i) * spriteData.Width)) + "/" + textureData.Length);
                                var xy = (center - spriteData.XOffset) + x + (((spriteData.YOffset) - row - (i-5)) * spriteData.Width);
                                if (xy < textureData.Length && xy >= 0)
                                    textureData[xy] = paletteData.Colors[spriteData.Data[postPos]];
                                postPos++;
                            }
                            postPos++; // skip last byte of post;
                        }
                    }

                    spriteTexture = new Texture2D(spriteData.Width, spriteData.Height, TextureFormat.ARGB32, true);
                    spriteTexture.SetPixels32(textureData);
                    spriteTexture.Apply();

                    break;
                case SpriteType.Raw:
                    break;
            }

            return spriteTexture;
        }

        public static Texture2D GenerateWallTexture(WallTextureData textureData, Dictionary<string, SpriteData> patches, PaletteData paletteData)
        {
            Texture2D spriteTexture = new Texture2D(textureData.Width, textureData.Height, TextureFormat.ARGB32, true);

            foreach (var patch in textureData.Patches)
            {
                var sprite = GenerateSprite(patches[patch.PatchName], paletteData);

                if (patch.XOffset >= textureData.Width || patch.YOffset >= textureData.Width ||
                    patch.XOffset+sprite.width<0 || patch.YOffset+sprite.height<0) continue;

                var dl = patch.XOffset;
                var dt = patch.YOffset;
                var w = sprite.width;
                var h = sprite.height;
                var sl = 0;
                var st = 0;
               
                if (patch.XOffset < 0)
                {
                    dl = 0;
                    sl = -patch.XOffset;
                    w = sprite.width + patch.XOffset;
                }
                if (dl + w >= textureData.Width)
                {
                    w = textureData.Width - dl;
                }
                if (patch.YOffset < 0)
                {
                    dt = 0;
                    st = -patch.YOffset;
                    h = sprite.height + patch.YOffset;
                }
                if (dt + h >= textureData.Height)
                {
                    h = textureData.Height - dt;
                }

                //Debug.Log(patch.XOffset + "," + patch.YOffset + "," + sprite.width + "," + sprite.height + "," + textureData.Width + "," + textureData.Height);
                //Debug.Log(sl + "," + st + "," + dl +"," + dt + "," + w + "," + h);
                var data = sprite.GetPixels(sl,st,w,h);
                spriteTexture.SetPixels(dl,dt,w,h,data);
            }

            spriteTexture.Apply();

            return spriteTexture;
        }

        public static SpriteSheet GenerateWallTextureSheet(List<WallTextureData> textureList, Dictionary<string, SpriteData> patches, PaletteData paletteData)
        {
            var widest = 0;
            var tallest = 0;

            foreach (var t in textureList)
            {
                if (t.Width > widest) widest = t.Width;
                if (t.Height > tallest) tallest = t.Height;
            }

            var rowCount = Mathf.CeilToInt(Mathf.Sqrt(textureList.Count));
            var w = rowCount * widest;
            var h = rowCount * tallest;

            Debug.Log(textureList.Count + "," + rowCount + "," + widest + "," + tallest);

            var spriteSheet = new SpriteSheet();
            spriteSheet.Texture = new Texture2D(w, h);

            int index = 0;
            foreach (var t in textureList)
            {
                var tex = GenerateWallTexture(t, patches, paletteData);

                var destX = widest * (index % rowCount);
                var desty = tallest * (index / rowCount);

                var source = tex.GetPixels();
                spriteSheet.Texture.SetPixels(destX, desty, tex.width, tex.height, source);

                index++;
            }

            spriteSheet.Texture.Apply();

            return spriteSheet;
        }

        public static SpriteSheet GenerateSpriteSheet(List<SpriteData> spritesList, PaletteData paletteData)
        {
            var widest = 0;
            var tallest = 0;

            foreach (var s in spritesList)
            {
                if (s.Width > widest) widest = s.Width;
                if (s.Height > tallest) tallest = s.Height;
            }

            var rowCount = Mathf.CeilToInt(Mathf.Sqrt(spritesList.Count));
            var w = rowCount * widest;
            var h = rowCount * tallest;

            var spriteSheet = new SpriteSheet();
            spriteSheet.Texture = new Texture2D(w, h);

            int index = 0;
            foreach (var s in spritesList)
            {
                var tex = GenerateSprite(s, paletteData);

                var destX = widest * (index % rowCount);
                var desty = tallest * (index / rowCount);

                var source = tex.GetPixels();
                spriteSheet.Texture.SetPixels(destX,desty,tex.width,tex.height, source);

                index++;
            }

            spriteSheet.Texture.Apply();

            return spriteSheet;
        }
    }
}
