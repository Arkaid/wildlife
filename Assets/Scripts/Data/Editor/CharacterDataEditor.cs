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

        /// <summary> loaded file, if any </summary>
        File loadedFile;

        /// <summary> Character sheet used for previewing puroses </summary>
        BaseSheet baseSheet;

        /// <summary> source PNG file for the character sheet </summary>
        string characterSheetFile;

        /// <summary> Round data for previewing purposes </summary>
        RoundImages[] rounds = new RoundImages[Config.Rounds];

        /// <summary> source PNG files for for each round (base, shadow) </summary>
        string[,] roundFiles = new string[Config.Rounds, 2];

        /// <summary> For scrolling </summary>
        Vector2 scroll;

        /// <summary> For each fold out </summary>
        bool[] foldout = new bool[4];

        // --- EditorWindow -----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
                Clear();
            if (GUILayout.Button("Load Character sheet"))
                LoadCharacterSheet();
            if (GUILayout.Button("Load Round 1 Images"))
                LoadRoundSet(0);
            if (GUILayout.Button("Load Round 2 Images"))
                LoadRoundSet(1);
            if (GUILayout.Button("Load Round 3 Images"))
                LoadRoundSet(2);
            if (GUILayout.Button("Save"))
                Save();
            if (GUILayout.Button("Load"))
                Load();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New ID", GUILayout.Width(200)))
                guid = System.Guid.NewGuid().ToString().ToUpper();
            guid = EditorGUILayout.TextField(guid);
            GUILayout.EndHorizontal();

            scroll = GUILayout.BeginScrollView(scroll);

            foldout[0] = EditorGUILayout.Foldout(foldout[0], "Character Sheet");
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

                GUILayout.BeginHorizontal();
                GUILayout.Label("Round 1 Icon", GUILayout.Width(75));
                DrawSprite(baseSheet.roundIcons[0]);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Round 2 Icon", GUILayout.Width(75));
                DrawSprite(baseSheet.roundIcons[1]);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Round 3 Icon", GUILayout.Width(75));
                DrawSprite(baseSheet.roundIcons[2]);
                GUILayout.EndHorizontal();
            }

            for (int i = 0; i < Config.Rounds; i++)
            {
                foldout[i + 1] = EditorGUILayout.Foldout(foldout[i + 1], string.Format("Round {0}", i + 1));
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
            baseSheet = null;
            rounds = new RoundImages[Config.Rounds];
            characterSheetFile = null;
            roundFiles = new string[Config.Rounds, 2];
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

            File.CreateFile(target, guid, characterSheetFile, roundFiles, loadedFile);
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

            for (int i = 0; i < Config.Rounds; i++)
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
            roundFiles[round, 0] = EditorUtility.OpenFilePanel("Load Base Image", "", "png");
            if (string.IsNullOrEmpty(roundFiles[round, 0]))
                return;

            roundFiles[round, 1] = EditorUtility.OpenFilePanel("Load Shadow Image", "", "png");
            if (string.IsNullOrEmpty(roundFiles[round, 1]))
                return;

            int w, h;
            byte[] pngBase = File.GetRawTextureData(roundFiles[round, 0]);
            byte [] pngShadow = File.GetRawTextureData(roundFiles[round, 1], out w, out h, true);
            rounds[round] = new RoundImages(pngBase, pngShadow, w == Game.PlayArea.LandscapeHeight);
            foldout[round + 1] = true;
        }
    }
}