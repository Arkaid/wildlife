using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Data
{
    using Game;

    // --- Class Declaration ------------------------------------------------------------------------
    public class Options : IllogicGate.Singleton<Options>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Location of the progress file </summary>
        string filePath { get { return Application.persistentDataPath + "/options.txt"; } }

        /// <summary> Keyword separator </summary>
        public static readonly string[] KeywordSeparator = new string[] { ", " };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        JSONObject json;

        /// <summary> Round image zoom </summary>
        public int zoom
        {
            get { return Mathf.Clamp((int)json["zoom"].i, 2, 4); }
            set { json.SetField("zoom", value); }
        }

        /// <summary> Game difficulty </summary>
        public Config.Difficulty difficulty
        {
            get { return (Config.Difficulty)System.Enum.Parse(typeof(Config.Difficulty), json["difficulty"].str); }
            set { json.SetField("difficulty", value.ToString()); }
        }

        /// <summary> Sound FX volume </summary>
        public int sfxVolume
        {
            get { return (int)json["sfx_volume"].i; }
            set { json.SetField("sfx_volume", value); }
        }

        /// <summary> Sound FX volume </summary>
        public int bgmVolume
        {
            get { return (int)json["bgm_volume"].i; }
            set { json.SetField("bgm_volume", value); }
        }

        // whether to play in fullscreen or not
        public bool fullScreen
        {
            get { return json["fullscreen"].b; }
            set { json.SetField("fullscreen", value); }
        }

        /// <summary> Lastly selected skill </summary>
        public Skill.Type skill
        {
            get { return (Skill.Type)System.Enum.Parse(typeof(Skill.Type), json["skill"].str); }
            set { json.SetField("skill", value.ToString()); }
        }

        // window resolution when playing in windowed mode
        public Vector2 resolution
        {
            get { return new Vector2(json["resolution_w"].f, json["resolution_h"].f); }
            set { json.SetField("resolution_w", value.x); json.SetField("resolution_h", value.y); }
        }

        /// <summary> This creates a debug log if turned on </summary>
        public bool debugOn { get { return json["debug"].b; } }

        /// <summary> List of keywords to show no matter what, comma separated </summary>
        public string showKeywords
        {
            get
            {
                string keywords = "";
                json.GetField(out keywords, "show_keywords", "");
                return keywords;
            }
            set { json.SetField("show_keywords", value); }
        }

        /// <summary> List of keywords to show no matter what </summary>
        public List<string> showKeywordsList {  get { return new List<string>(showKeywords.Split(KeywordSeparator, System.StringSplitOptions.RemoveEmptyEntries)); } }

        /// <summary> List of keywords to hide if not explicitly shown, comma separated </summary>
        public string hideKeywords
        {
            get
            {
                string keywords = "";
                json.GetField(out keywords, "hide_keywords", "");
                return keywords;
            }
            set { json.SetField("hide_keywords", value); }
        }

        /// <summary> List of keywords to hide if not explicitly shown </summary>
        public List<string> hideKeywordsList { get { return new List<string>(hideKeywords.Split(KeywordSeparator, System.StringSplitOptions.RemoveEmptyEntries)); } }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void OnInstanceCreated()
        {
            base.OnInstanceCreated();
            Load();
        }

        // -----------------------------------------------------------------------------------	
        public void Load()
        {
            if (!System.IO.File.Exists(filePath))
                CreateInitialFile();
            json = new JSONObject(System.IO.File.ReadAllText(filePath));
        }
        
        // -----------------------------------------------------------------------------------	
        public void Save()
        {
            System.IO.File.WriteAllText(filePath, json.ToString(true));
        }

        // -----------------------------------------------------------------------------------	
        void CreateInitialFile()
        {
            json = new JSONObject();
            json.AddField("difficulty", Config.Difficulty.Normal.ToString());
            json.AddField("skill", Game.Skill.Type.Shield.ToString());
            json.AddField("sfx_volume", 100);
            json.AddField("bgm_volume", 100);
            json.AddField("resolution_w", 1366);
            json.AddField("resolution_h", 768);
            json.AddField("fullscreen", false);
            json.AddField("zoom", 2);

            json.AddField("debug", false);

            Save();
        }
    }
}