using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterData
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        const byte Version1 = 1;

        struct Header
        {
            public byte version;
            public int round1Base;
            public int round1Shadow;
            public int round2Base;
            public int round2Shadow;
            public int round3Base;
            public int round3Shadow;

            public void Load(BinaryReader br)
            {
                version = br.ReadByte();

            }
        }

        public struct RoundData
        {
            public Texture2D baseImage;
            public Texture2D shadowImage;

            public RoundData(byte[] pngBase, byte[] pngShadow)
            {
                baseImage = new Texture2D(PlayArea.ImageWidth, PlayArea.ImageHeight, TextureFormat.ARGB32, false);
                baseImage.LoadImage(pngBase);
                baseImage.filterMode = FilterMode.Point;

                shadowImage = new Texture2D(PlayArea.ImageWidth, PlayArea.ImageHeight, TextureFormat.Alpha8, false);
                shadowImage.LoadImage(pngShadow);
                shadowImage.filterMode = FilterMode.Point;
                shadowImage.alphaIsTransparency = true;
            }
        }

        public struct CharacterSheet
        {
            public Sprite name;
            public Sprite avatarA;
            public Sprite avatarB;
            public Sprite icon;
            public Sprite[] roundIcons;

            Texture2D source;

            public CharacterSheet(byte [] pngData)
            {
                source = new Texture2D(1200, 950, TextureFormat.ARGB32, false);
                source.LoadImage(pngData);
                source.filterMode = FilterMode.Point;
                source.alphaIsTransparency = true;
                

                name = Sprite.Create(source, new Rect(540, 83, 600, 67), new Vector2(300, 33.5f), 100);
                avatarA = Sprite.Create(source, new Rect(0, 150, 600, 800), new Vector2(300, 400), 100);
                avatarB = Sprite.Create(source, new Rect(600, 150, 600, 800), new Vector2(300, 400), 100);
                icon = Sprite.Create(source, new Rect(0, 0, 150, 150), new Vector2(75, 75), 100);

                roundIcons = new Sprite[3];
                roundIcons[0] = Sprite.Create(source, new Rect(150, 20, 130, 130), new Vector2(65, 65), 100);
                roundIcons[1] = Sprite.Create(source, new Rect(280, 20, 130, 130), new Vector2(65, 65), 100);
                roundIcons[2] = Sprite.Create(source, new Rect(410, 20, 130, 130), new Vector2(65, 65), 100);
            }
        }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        CharacterSheet characterSheet;
        
        public Texture2D[] roundImage;
        public Texture2D[] shadowImage;

        Header header;

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        

        // -----------------------------------------------------------------------------------	
        public void LoadCore(string filename)
        {
            byte[] data = IllogicGate.Data.EncryptedFile.ReadBytes(filename);
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            // load header
            header = new Header();
            header.Load(br);

            //selectionMenu.name = DeserializeTexture(br);
        }

        // -----------------------------------------------------------------------------------	
        public void LoadRound(string filename, int round)
        {
            byte[] data = IllogicGate.Data.EncryptedFile.ReadBytes(filename);
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            Header header = new Header();
            header.Load(br);
        }

        // -----------------------------------------------------------------------------------	
        void SerializeTexture(Texture2D tex, BinaryWriter bw)
        {
            bw.Write(tex.width);
            bw.Write(tex.height);
            bw.Write((int)tex.format);
            byte[] data = tex.EncodeToPNG();
            bw.Write(data.Length);
            bw.Write(data);
        }

        // -----------------------------------------------------------------------------------	
        Texture2D DeserializeTexture( BinaryReader br)
        {
            int w = br.ReadInt32();
            int h = br.ReadInt32();
            TextureFormat format = (TextureFormat)br.ReadInt32();
            byte [] data = br.ReadBytes(br.ReadInt32());

            Texture2D tex = new Texture2D(w, h, format, false);
            tex.LoadImage(data);

            // we assume these values to be correct
            tex.filterMode = FilterMode.Point;
            tex.alphaIsTransparency = true;

            return tex;
        }

    }
}