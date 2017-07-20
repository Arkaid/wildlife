using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Jintori
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
            
        /// <summary> Character sheet used for previewing puroses </summary>
        CharacterSheet characterSheet;

        /// <summary> source PNG file for the character sheet </summary>
        string characterSheetFile;

        /// <summary> Round data for previewing purposes </summary>
        RoundData[] rounds = new RoundData[Config.Rounds];

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
            if (GUILayout.Button("Test"))
                Test();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New ID", GUILayout.Width(200)))
                guid = System.Guid.NewGuid().ToString().ToUpper();
            EditorGUILayout.SelectableLabel(guid);
            GUILayout.EndHorizontal();

            scroll = GUILayout.BeginScrollView(scroll);

            foldout[0] = EditorGUILayout.Foldout(foldout[0], "Character Sheet");
            if (foldout[0] && characterSheet != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(75));
                DrawSprite(characterSheet.name);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Icon", GUILayout.Width(75));
                DrawSprite(characterSheet.icon);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Avatar A", GUILayout.Width(75));
                DrawSprite(characterSheet.avatarA);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Avatar B", GUILayout.Width(75));
                DrawSprite(characterSheet.avatarB);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Round 1 Icon", GUILayout.Width(75));
                DrawSprite(characterSheet.roundIcons[0]);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Round 2 Icon", GUILayout.Width(75));
                DrawSprite(characterSheet.roundIcons[1]);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Round 3 Icon", GUILayout.Width(75));
                DrawSprite(characterSheet.roundIcons[2]);
                GUILayout.EndHorizontal();
            }

            for (int i = 0; i < Config.Rounds; i++)
            {
                foldout[i + 1] = EditorGUILayout.Foldout(foldout[i + 1], string.Format("Round {0}", i + 1));
                if (foldout[i + 1] && !string.IsNullOrEmpty(roundFiles[i, 0]) && !string.IsNullOrEmpty(roundFiles[0, 1]))
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
            guid = System.Guid.NewGuid().ToString().ToUpper();
            characterSheet = null;
            rounds = new RoundData[Config.Rounds];
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

            CharacterDataFile.CreateFile(target, guid, characterSheetFile, roundFiles);
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads a created character file to test if it was saved correctly
        /// </summary>
        void Test()
        {
            Clear();

            string target = EditorUtility.OpenFilePanel("Select file", "", "chr");
            if (string.IsNullOrEmpty(target))
                return;

            CharacterDataFile charFile = new CharacterDataFile(target);
            characterSheetFile = "_TEST_";
            characterSheet = charFile.characterSheet;

            guid = charFile.guid;

            for (int i = 0; i < Config.Rounds; i++)
            {
                roundFiles[i, 0] = "_TEST_";
                roundFiles[i, 1] = "_TEST_";
                rounds[i] = charFile.LoadRound(i);
            }
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
            characterSheet = new CharacterSheet(CharacterDataFile.GetRawTextureData(characterSheetFile));
            foldout[0] = true;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Load a PNG into preview and save the file name
        /// </summary>
        void LoadRoundSet(int round)
        {
            roundFiles[round, 0] = EditorUtility.OpenFilePanel("Load Base Image", "", "png");
            if (string.IsNullOrEmpty(roundFiles[round, 0]))
                return;

            roundFiles[round, 1] = EditorUtility.OpenFilePanel("Load Shadow Image", "", "png");
            if (string.IsNullOrEmpty(roundFiles[round, 1]))
                return;

            byte[] pngBase = CharacterDataFile.GetRawTextureData(roundFiles[round, 0]);
            byte [] pngShadow = CharacterDataFile.GetRawTextureData(roundFiles[round, 1], true);
            rounds[round] = new RoundData(pngBase, pngShadow);
            foldout[round + 1] = true;
        }
    }
}