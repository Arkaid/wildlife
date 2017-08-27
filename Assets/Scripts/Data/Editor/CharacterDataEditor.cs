using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Jintori.CharacterFile
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Helps create and test character data files
    /// </summary>
    public class CharacterDataEditor : EditorWindow
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        [MenuItem("Jintori/Character Data")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            CharacterDataEditor window = GetWindow<CharacterDataEditor>();
            window.Show();
            window.Clear();
        }

        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Unique id to identify the character file. Used for saving / loading progress </summary>
        string guid;

        /// <summary> comma separated tags </summary>
        string tags;

        /// <summary> artist name </summary>
        string artist;

        /// <summary> character's name </summary>
        string characterName;

        /// <summary> loaded file, if any </summary>
        File loadedFile;

        /// <summary> Character sheet used for previewing puroses </summary>
        BaseSheet baseSheet;

        /// <summary> source PNG file for the character sheet </summary>
        string characterSheetFile;

        /// <summary> Round data for previewing purposes </summary>
        RoundImages[] rounds = new RoundImages[Config.Rounds];

        /// <summary> source PNG files for for each round (base, shadow) </summary>
        List<string[]> roundFiles = new List<string[]>();

        /// <summary> For scrolling </summary>
        Vector2 scroll;

        /// <summary> For each fold out </summary>
        bool[] foldout = new bool[5];

        // --- EditorWindow -----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear", GUILayout.Width(100)))
                Clear();
            if (GUILayout.Button("Load", GUILayout.Width(100)))
                Load();
            if (GUILayout.Button("Save", GUILayout.Width(100)))
                Save();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New ID", GUILayout.Width(120)))
                guid = System.Guid.NewGuid().ToString().ToUpper();
            guid = EditorGUILayout.TextField(guid);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Character", GUILayout.Width(120));
            characterName = EditorGUILayout.TextField(characterName);
            GUILayout.Label("Artist", GUILayout.Width(120));
            artist = EditorGUILayout.TextField(artist);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Created:", GUILayout.Width(120));
            EditorGUILayout.LabelField(loadedFile != null ? loadedFile.createdDate.ToString() : "NEW");
            GUILayout.Label("Updated", GUILayout.Width(120));
            EditorGUILayout.LabelField(loadedFile != null ? loadedFile.updatedDate.ToString() : "NEW");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tags (IB style)", GUILayout.Width(120));
            tags = EditorGUILayout.TextField(tags);
            GUILayout.EndHorizontal();

            scroll = GUILayout.BeginScrollView(scroll);

            foldout[0] = EditorGUILayout.Foldout(foldout[0], "Character Sheet");
            if (GUILayout.Button("Import image", GUILayout.Width(100)))
                LoadCharacterSheet();
            if (foldout[0] && baseSheet != null)
            {

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(75));
                DrawSprite(baseSheet.name);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Icon", GUILayout.Width(75));
                DrawSprite(baseSheet.icon);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Avatar A", GUILayout.Width(75));
                DrawSprite(baseSheet.avatarA);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Avatar B", GUILayout.Width(75));
                DrawSprite(baseSheet.avatarB);
                GUILayout.EndHorizontal();

                for (int i = 0; i < Config.Rounds; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Round " + (i + 1) + " Icon", GUILayout.Width(75));
                    DrawSprite(baseSheet.roundIcons[i]);
                    GUILayout.EndHorizontal();
                }
            }

            for (int i = 0; i < Config.Rounds; i++)
            {
                foldout[i + 1] = EditorGUILayout.Foldout(foldout[i + 1], string.Format("Round {0}", i + 1));
                if (GUILayout.Button("Import images", GUILayout.Width(100)))
                    LoadRoundSet(i);
                if (foldout[i + 1] && rounds[i] != null)
                {
                    GUILayout.BeginHorizontal();
                    DrawTexture(rounds[i].baseImage);
                    DrawTexture(rounds[i].shadowImage);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Clears the previews
        /// </summary>
        void Clear()
        {
            loadedFile = null;
            guid = System.Guid.NewGuid().ToString().ToUpper();
            tags = "";
            artist = "";
            characterName = "";
            baseSheet = null;
            rounds = new RoundImages[Config.Rounds];
            characterSheetFile = null;
            roundFiles = new List<string[]>(new string[Config.Rounds][]);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Saves and encrypts the charcter file to disk
        /// </summary>
        void Save()
        {
            string target = Path.GetFileNameWithoutExtension(characterSheetFile) + ".chr";
            target = EditorUtility.SaveFilePanel("Save", "", target, "chr");

            if (string.IsNullOrEmpty(target))
                return;

            File.CreateFile(target, guid, characterName, artist, tags, characterSheetFile, roundFiles, loadedFile);
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads a created character file to test if it was saved correctly
        /// </summary>
        void Load()
        {
            Clear();

            string target = EditorUtility.OpenFilePanel("Select file", "", "chr");
            if (string.IsNullOrEmpty(target))
                return;

            loadedFile = new File(target);
            baseSheet = loadedFile.baseSheet;

            guid = loadedFile.guid;
            artist = loadedFile.artist;
            characterName = loadedFile.characterName;
            tags = loadedFile.tags;

            for (int i = 0; i < loadedFile.availableRounds; i++)
                rounds[i] = loadedFile.LoadRound(i);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Draws a sprite as a GUILayout control
        /// </summary>
        /// <param name="sprite"></param>
        void DrawSprite(Sprite sprite)
        {
            if (sprite == null)
                return;

            Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
                                       sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            
            GUILayout.Box(GUIContent.none, GUILayout.Width(sprite.rect.width + 4), GUILayout.Height(sprite.rect.height + 4));
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x = rect.x + 2;
            rect.y = rect.y + 2;
            rect.width = rect.width - 2;
            rect.height = rect.height - 2;
            GUI.DrawTextureWithTexCoords(rect, sprite.texture, spriteRect);
        }

        // -----------------------------------------------------------------------------------	
        void DrawTexture(Texture2D tex)
        {
            GUILayout.Box(GUIContent.none, GUILayout.Width(tex.width + 4), GUILayout.Height(tex.height + 4));
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x = rect.x + 2;
            rect.y = rect.y + 2;
            rect.width = rect.width - 2;
            rect.height = rect.height - 2;
            GUI.DrawTexture(rect, tex);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Load a PNG into preview and save the file name
        /// </summary>
        void LoadCharacterSheet()
        {
            characterSheetFile = EditorUtility.OpenFilePanel("Load Character Sheet", "", "png");
            if (string.IsNullOrEmpty(characterSheetFile))
                return;
            baseSheet = new BaseSheet(File.GetRawTextureData(characterSheetFile));
            foldout[0] = true;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads a pair of images for a round, and previews them
        /// </summary>
        void LoadRoundSet(int round)
        {
            string[] files = new string[2];
            files[0] = EditorUtility.OpenFilePanel("Load Base Image", "", "png");
            if (string.IsNullOrEmpty(files[0]))
                return;

            files[1] = EditorUtility.OpenFilePanel("Load Shadow Image", "", "png");
            if (string.IsNullOrEmpty(files[1]))
                return;

            roundFiles[round] = files;

            // for previewing
            int w, h;
            byte[] pngBase = File.GetRawTextureData(files[0]);
            byte [] pngShadow = File.GetRawTextureData(files[1], out w, out h, true);
            rounds[round] = new RoundImages(pngBase, pngShadow, w == Game.PlayArea.LandscapeHeight);
            foldout[round + 1] = true;
        }
    }
}