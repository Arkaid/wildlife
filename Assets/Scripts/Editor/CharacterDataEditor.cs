using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
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
        }

        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        CharacterData.CharacterSheet characterSheet;
        string characterSheetFile;

        string[,] roundFiles = new string[3, 2];
        CharacterData.RoundData[] rounds = new CharacterData.RoundData[3];

        Vector2 scroll;

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

            GUILayout.EndHorizontal();

            scroll = GUILayout.BeginScrollView(scroll);

            foldout[0] = EditorGUILayout.Foldout(foldout[0], "Character Sheet");
            if (foldout[0] && !string.IsNullOrEmpty(characterSheetFile))
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

            for (int i = 0; i < 3; i++)
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
        void Clear()
        {
            characterSheetFile = null;
            roundFiles = new string[3, 2];
        }
        // -----------------------------------------------------------------------------------	
        void Save()
        {
            string target = EditorUtility.OpenFilePanel("Save", "", "chr");

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
        void LoadCharacterSheet()
        {
            characterSheetFile = EditorUtility.OpenFilePanel("Load Character Sheet", "", "png");
            if (string.IsNullOrEmpty(characterSheetFile))
                return;
            characterSheet = new CharacterData.CharacterSheet(System.IO.File.ReadAllBytes(characterSheetFile));
            foldout[0] = true;
        }

        // -----------------------------------------------------------------------------------	
        void LoadRoundSet(int round)
        {
            roundFiles[round, 0] = EditorUtility.OpenFilePanel("Load Round 1 Image", "", "png");
            if (string.IsNullOrEmpty(roundFiles[round, 0]))
                return;

            roundFiles[round, 1] = EditorUtility.OpenFilePanel("Load Round 1 Shadow", "", "png");
            if (string.IsNullOrEmpty(roundFiles[round, 1]))
                return;

            byte[] pngBase = System.IO.File.ReadAllBytes(roundFiles[round, 0]);
            byte [] pngShadow = System.IO.File.ReadAllBytes(roundFiles[round, 1]);
            rounds[round] = new CharacterData.RoundData(pngBase, pngShadow);
            foldout[round + 1] = true;
        }
    }
}