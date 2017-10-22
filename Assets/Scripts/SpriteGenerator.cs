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
            Color32[] textureData;

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

                    textureData = new Color32[spriteData.Width * spriteData.Height];
                    var center = (spriteData.Width / 2)-1;

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
                                var xy = (center - spriteData.XOffset) + x + (((spriteData.YOffset) - row - (i-4)) * spriteData.Width);
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
                    // Raw sprites are always 64x64
                    spriteTexture = new Texture2D(spriteData.Width, spriteData.Height);
                    if (spriteData.Data.Length == 0)
                        return new Texture2D(spriteData.Width, spriteData.Height);

                    textureData = new Color32[spriteData.Width * spriteData.Height];
                    var dataPos = 0;
                    for (var x=0;x< spriteData.Width; x++)
                        for (var y = spriteData.Height-1; y >=0; y--)
                        {
                            var xy = x + (y * spriteData.Width);
                            textureData[xy] = paletteData.Colors[spriteData.Data[dataPos]];
                            dataPos++;
                        }

                    spriteTexture.SetPixels32(textureData);
                    spriteTexture.Apply();

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
                //for (int x = dl; x < dl + w; x++)
                 //   for (int y = dt; y < dt + h; y++)
                 //       if (x == dl || x == dl + w - 1 || y == dt || y == dt + h -1)
                //            spriteTexture.SetPixel(x, y, Color.red);
            }

            spriteTexture.Apply();

            return spriteTexture;
        }

        public static TileSheet GenerateWallTextureSheet(List<WallTextureData> textureList, Dictionary<string, SpriteData> patches, PaletteData paletteData, int padding)
        {
            var widest = 0;
            var tallest = 0;

            foreach (var t in textureList)
            {
                if (t.Width > widest) widest = t.Width;
                if (t.Height > tallest) tallest = t.Height;
            }

            var largest = widest > tallest ? widest : tallest;

            var rowCount = Mathf.CeilToInt(Mathf.Sqrt(textureList.Count));
            var w = rowCount * largest;
            var h = rowCount * largest;

            w += rowCount * padding * 2;
            h += rowCount * padding * 2;

            if (rowCount % 2 != 0) rowCount++;

            var spriteSheet = new TileSheet();
            spriteSheet.Rows = rowCount;
            spriteSheet.Columns = rowCount;
            spriteSheet.TileWidth = largest;
            spriteSheet.TileHeight = largest;
            spriteSheet.Padding = padding;
            spriteSheet.Texture = new Texture2D(w, h);

            int index = 0;
            foreach (var t in textureList)
            {
                var tex = GenerateWallTexture(t, patches, paletteData);

                var destX = (largest + padding*2) * (index % rowCount) + padding;
                var destY = (largest + padding*2) * (index / rowCount) + padding;

                var source = tex.GetPixels();
                spriteSheet.Texture.SetPixels(destX, destY, tex.width, tex.height, source);

                // Padding
                for(var i=1;i<=padding;i++)
                {
                    for(var x=0;x<tex.width;x++)
                    {
                        spriteSheet.Texture.SetPixel(destX+x, destY - i, tex.GetPixel(x, 0));
                        spriteSheet.Texture.SetPixel(destX+x, (destY + (tex.height-1)) + i, tex.GetPixel(x, tex.height-1));
                    }

                    for (var y =0; y < tex.height; y++)
                    {
                        spriteSheet.Texture.SetPixel(destX - i, (destY)+y, tex.GetPixel(0, y));
                        spriteSheet.Texture.SetPixel((destX + (tex.width-1)) + i, (destY)+y, tex.GetPixel(tex.width-1, y));
                    }
                }

                for(var x=0;x<padding;x++)
                    for(var y=0;y<padding;y++)
                    {
                        spriteSheet.Texture.SetPixel((destX-padding)+x, (destY-2)+y, tex.GetPixel(0, 0));
                        spriteSheet.Texture.SetPixel((destX+tex.width)+x, (destY-2)+y, tex.GetPixel(tex.width-1, 0));
                        spriteSheet.Texture.SetPixel((destX+tex.width)+x, (destY+tex.height)+y, tex.GetPixel(tex.width-1, tex.height-1));
                        spriteSheet.Texture.SetPixel((destX - padding) + x, (destY + tex.height) + y, tex.GetPixel(0, tex.height - 1));
                    }


                spriteSheet.LookupTable.Add(t.Name, new TileSheetSprite()
                {
                    TileNum = index,
                    Width = tex.width,
                    Height = tex.height
                });

                index++;
            }

            spriteSheet.Texture.Apply();

            return spriteSheet;
        }

        public static TileSheet GenerateTileSheet(List<SpriteData> spritesList, PaletteData paletteData, int padding)
        {
            var widest = 0;
            var tallest = 0;

            foreach (var s in spritesList)
            {
                if (s.Width > widest) widest = s.Width;
                if (s.Height > tallest) tallest = s.Height;
            }

            // Calculate number of rows and columns. We need this to be an even number for the shader to work
            var rowCount = Mathf.CeilToInt(Mathf.Sqrt(spritesList.Count));
            if (rowCount % 2 != 0) rowCount++;

            var w = rowCount * widest;
            var h = rowCount * tallest;

            w += rowCount * padding * 2;
            h += rowCount * padding * 2;

            var spriteSheet = new TileSheet();
            spriteSheet.Texture = new Texture2D(w, h);
            spriteSheet.Rows = rowCount;
            spriteSheet.Columns = rowCount;
            spriteSheet.TileWidth = widest;
            spriteSheet.TileHeight = tallest;
            spriteSheet.Padding = padding;

            int index = 0;
            foreach (var s in spritesList)
            {
                var tex = GenerateSprite(s, paletteData);

                var destX = (widest + padding*2) * (index % rowCount) + padding;
                var destY = (tallest+padding*2) * (index / rowCount) + padding;

                var source = tex.GetPixels();
                spriteSheet.Texture.SetPixels(destX,destY,tex.width,tex.height, source);

                // Padding
                for (var i = 1; i <= padding; i++)
                {
                    for (var x = 0; x < tex.width; x++)
                    {
                        spriteSheet.Texture.SetPixel(destX + x, destY - i, tex.GetPixel(x, 0));
                        spriteSheet.Texture.SetPixel(destX + x, (destY + (tex.height - 1)) + i, tex.GetPixel(x, tex.height - 1));
                    }

                    for (var y = 0; y < tex.height; y++)
                    {
                        spriteSheet.Texture.SetPixel(destX - i, (destY) + y, tex.GetPixel(0, y));
                        spriteSheet.Texture.SetPixel((destX + (tex.width - 1)) + i, (destY) + y, tex.GetPixel(tex.width - 1, y));
                    }
                }

                for (var x = 0; x < padding; x++)
                    for (var y = 0; y < padding; y++)
                    {
                        spriteSheet.Texture.SetPixel((destX - padding) + x, (destY - 2) + y, tex.GetPixel(0, 0));
                        spriteSheet.Texture.SetPixel((destX + tex.width) + x, (destY - 2) + y, tex.GetPixel(tex.width - 1, 0));
                        spriteSheet.Texture.SetPixel((destX + tex.width) + x, (destY + tex.height) + y, tex.GetPixel(tex.width - 1, tex.height - 1));
                        spriteSheet.Texture.SetPixel((destX - padding) + x, (destY + tex.height) + y, tex.GetPixel(0, tex.height - 1));
                    }

                spriteSheet.LookupTable.Add(s.Name, new TileSheetSprite()
                {
                    TileNum = index,
                    Width = tex.width,
                    Height = tex.height
                });

                index++;
            }

            spriteSheet.Texture.Apply();

            return spriteSheet;
        }
    }
}
