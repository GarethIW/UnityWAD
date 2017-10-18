using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityWAD;

namespace UnityWAD
{
    public static class SpriteGenerator
    {
        public static Texture GenerateSprite(SpriteData spriteData)
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
                    var center = (spriteData.Width / 2)-3;

                    for(int x=0;x<columns.Count;x++)
                    {
                        Debug.Log(x+"/" + columns.Count);
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
                                var xy = (center - spriteData.XOffset) + x + (((spriteData.YOffset) - row - (i-1)) * spriteData.Width);
                                if (xy<textureData.Length && xy>=0)
                                    textureData[xy] = new Color32(0,0,0,255);
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
    }
}
