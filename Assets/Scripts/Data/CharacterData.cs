using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Contains the character sheet info for the select menu
    /// This is a texture with a few sprites on them, including icon, name, avatar, etc
    /// </summary>
    public class CharacterSheet
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Width of the sprite sheet </summary>
        const int ImageWidth = 1200;

        /// <summary> Height of the sprite sheet </summary>
        const int ImageHeight = 950;

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Sprite with the character's name </summary>
        public Sprite name { get; private set; }

        /// <summary> Sprite with the character's avatar - selected </summary>
        public Sprite avatarA { get; private set; }

        /// <summary> Sprite with the character's avatar - confirmed </summary>
        public Sprite avatarB { get; private set; }

        /// <summary> Sprite with the character's icon for the selection grid </summary>
        public Sprite icon { get; private set; }

        /// <summary> Cleared round preview images </summary>
        public Sprite[] roundIcons { get; private set; }

        /// <summary> Source texture </summary>
        Texture2D source;

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public CharacterSheet(byte[] rawData)
        {
            source = new Texture2D(ImageWidth, ImageHeight, TextureFormat.RGBA32, false);
            source.filterMode = FilterMode.Point;
            source.alphaIsTransparency = true;
            source.wrapMode = TextureWrapMode.Clamp;
            source.LoadRawTextureData(rawData);
            source.Apply();

            // these values are taken directly from the sprite
            // sheet. If the sprite sheet layout changes, fix
            // these values too
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

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Holds the images for a given game round, both base image and shadow
    /// </summary>
    public class RoundData
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Base image </summary>
        public Texture2D baseImage { get; private set; }

        /// <summary> Shadow image </summary>
        public Texture2D shadowImage { get; private set; }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Creates the textures needed for the round
        /// </summary>
        public RoundData(byte[] rawBase, byte[] rawShadow)
        {
            baseImage = new Texture2D(PlayArea.ImageWidth, PlayArea.ImageHeight, TextureFormat.RGB24, false);
            baseImage.filterMode = FilterMode.Point;
            baseImage.alphaIsTransparency = true;
            baseImage.wrapMode = TextureWrapMode.Clamp;
            baseImage.LoadRawTextureData(rawBase);
            baseImage.Apply();

            shadowImage = new Texture2D(PlayArea.ImageWidth, PlayArea.ImageHeight, TextureFormat.Alpha8, false);
            baseImage.filterMode = FilterMode.Point;
            baseImage.alphaIsTransparency = true;
            baseImage.wrapMode = TextureWrapMode.Clamp;
            shadowImage.LoadRawTextureData(rawShadow);
            shadowImage.Apply();
        }
    }

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Saves and loads character datafiles.
    /// </summary>
    public class CharacterDataFile
    {
        // --- Constants --------------------------------------------------------------------------------
        // unshuffled key is "8f8c06bdfc41a468ad379e34791cbf78581295ed90abf6118ea30082b561"
        const string ShuffledKey = @"9h;d29cfid64b699cg49<f578;4ddi8:8935:7he;3bdi7349gd42394e684";

        /// <summary> File version 1 </summary>
        const byte Version1 = 1;

        /// <summary> A file entry in the data file </summary>
        struct Entry
        {
            /// <summary> offset from the start of the file </summary>
            public int offset;

            /// <summary> length in bytes </summary>
            public int length;

            public Entry(int offset = 0, int length = 0)
            {
                this.offset = offset;
                this.length = length;
            }

            public Entry(BinaryReader br)
            {
                offset = br.ReadInt32();
                length = br.ReadInt32();
            }
            public void Save(BinaryWriter bw)
            {
                bw.Write(offset);
                bw.Write(length);
            }
        }

        /// <summary> Data file header. This might change with versions </summary>
        struct Header
        {
            public byte version;
            public Entry characterSheet;
            public Entry[] roundBase;
            public Entry[] roundShadow;

            public Header(byte version)
            {
                this.version = version;
                characterSheet = new Entry();
                roundBase = new Entry[3];
                roundShadow = new Entry[3];
            }

            public Header(BinaryReader br)
            {
                version = br.ReadByte();
                characterSheet = new Entry(br);
                roundBase = new Entry[3];
                roundShadow = new Entry[3];

                for (int i = 0; i < roundBase.Length; i++)
                {
                    roundBase[i] = new Entry(br);
                    roundShadow[i] = new Entry(br);
                }
            }

            public void Save(BinaryWriter bw)
            {
                bw.Write(version);
                characterSheet.Save(bw);
                for (int i = 0; i < roundBase.Length; i++)
                {
                    roundBase[i].Save(bw);
                    roundShadow[i].Save(bw);
                }
            }
        }

        // --- Static Properties ------------------------------------------------------------------------
        public static string dataPath { get { return Application.dataPath + "/Characters"; } }

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
#if UNITY_EDITOR
        /// <summary>
        /// Creates a file with the character sheet and each round image
        /// Only available in editor mode
        /// encrypting them separately
        /// </summary>
        /// <param name="filename">File to save to</param>
        /// <param name="charSheetFile"> PNG file with the character sheet </param>
        /// <param name="roundFiles"> PNG files for each round (base, shadow) </param>
        public static void CreateFile(string filename, string charSheetFile, string [,] roundFiles)
        {
            BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create));
            BlowFishCS.BlowFish blowfish = new BlowFishCS.BlowFish(IllogicGate.Data.EncryptedFile.RestoreKey(ShuffledKey));
            
            // save an empty header to make space for it
            Header header = new Header(Version1);
            header.Save(bw);

            byte[] data;

            // encrypt and save the character sheet file
            //data = File.ReadAllBytes(charSheetFile);
            data = GetRawTextureData(charSheetFile);
            data = LZMAtools.CompressByteArrayToLZMAByteArray(data);
            data = blowfish.Encrypt_ECB(data);
            header.characterSheet = new Entry((int)bw.BaseStream.Position, data.Length);
            bw.Write(data);

            // encrypt and save round images
            for (int i = 0; i < 3; i++)
            {
                data = GetRawTextureData(roundFiles[i, 0]);
                data = LZMAtools.CompressByteArrayToLZMAByteArray(data);
                data = blowfish.Encrypt_ECB(data);
                header.roundBase[i] = new Entry((int)bw.BaseStream.Position, data.Length);
                bw.Write(data);

                data = GetRawTextureData(roundFiles[i, 1], true);
                data = LZMAtools.CompressByteArrayToLZMAByteArray(data);
                data = blowfish.Encrypt_ECB(data);
                header.roundShadow[i] = new Entry((int)bw.BaseStream.Position, data.Length);
                bw.Write(data);
            }

            // rewind and overwrite header
            bw.Seek(0, SeekOrigin.Begin);
            header.Save(bw);

            bw.Close();
        }

        // -----------------------------------------------------------------------------------	
        static public byte[] GetRawTextureData(string pngFile, bool isShadow = false)
        {
            const string TempFile = "Assets/_temp_.png";
            File.Copy(pngFile, Application.dataPath + "/_temp_.png", true);

            AssetDatabase.ImportAsset(TempFile);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(TempFile);
            importer.alphaIsTransparency = true;
            importer.alphaSource = isShadow ? 
                TextureImporterAlphaSource.FromGrayScale : 
                TextureImporterAlphaSource.FromInput;
            importer.anisoLevel = 0;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            //importer.textureCompression = TextureImporterCompression.CompressedHQ;
            //importer.crunchedCompression = true;
            importer.textureType = isShadow ?
                TextureImporterType.SingleChannel : 
                TextureImporterType.Default;
            importer.SaveAndReimport();

            Texture2D test = AssetDatabase.LoadAssetAtPath<Texture2D>(TempFile);
            byte [] rawData = test.GetRawTextureData();
            AssetDatabase.DeleteAsset(TempFile);

            return rawData;
        }
#endif

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads and decrypts a character sheet from file
        /// </summary>
        /// <param name="filename">File to load it from</param>
        public static CharacterSheet LoadCharacterSheet(string filename)
        {
            BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open));
            BlowFishCS.BlowFish blowfish = new BlowFishCS.BlowFish(IllogicGate.Data.EncryptedFile.RestoreKey(ShuffledKey));

            Header header = new Header(br);

            // read and decrypt the character sheet
            br.BaseStream.Seek(header.characterSheet.offset, SeekOrigin.Begin);
            byte[] rawData = br.ReadBytes(header.characterSheet.length);
            rawData = blowfish.Decrypt_ECB(rawData);
            rawData = LZMAtools.DecompressLZMAByteArrayToByteArray(rawData);
            br.Close();

            return new CharacterSheet(rawData);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads the images for a given round
        /// </summary>
        /// <param name="filename">File name to load the rounds from</param>
        /// <param name="round">round to load (0 to 2)</param>
        public static RoundData LoadRound(string filename, int round)
        {
            BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open));
            BlowFishCS.BlowFish blowfish = new BlowFishCS.BlowFish(IllogicGate.Data.EncryptedFile.RestoreKey(ShuffledKey));

            Header header = new Header(br);

            // read and decrypt the round image and shadow
            br.BaseStream.Seek(header.roundBase[round].offset, SeekOrigin.Begin);
            byte[] rawBase = br.ReadBytes(header.roundBase[round].length);
            rawBase = blowfish.Decrypt_ECB(rawBase);
            rawBase = LZMAtools.DecompressLZMAByteArrayToByteArray(rawBase);

            br.BaseStream.Seek(header.roundShadow[round].offset, SeekOrigin.Begin);
            byte[] rawShadow = br.ReadBytes(header.roundShadow[round].length);
            rawShadow = blowfish.Decrypt_ECB(rawShadow);
            rawShadow = LZMAtools.DecompressLZMAByteArrayToByteArray(rawShadow);

            br.Close();

            return new RoundData(rawBase, rawShadow);
        }


        // --- Properties -------------------------------------------------------------------------------
        // --- Methods ----------------------------------------------------------------------------------
    }
}