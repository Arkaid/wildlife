﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Data
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class OptionsFile 
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Location of the progress file </summary>
        string filePath { get { return Application.persistentDataPath + "/options.txt"; } }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public JSONObject json { get; private set; }

        // --- Methods ----------------------------------------------------------------------------------
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
            System.IO.File.WriteAllText(filePath, json.ToString());
        }

        // -----------------------------------------------------------------------------------	
        void CreateInitialFile()
        {
            json = new JSONObject();
            json.AddField("difficulty", Config.Difficulty.Normal.ToString());
            json.AddField("sfx_volume", 100);
            json.AddField("bgm_volume", 100);
            json.AddField("resolution_w", Screen.currentResolution.width);
            json.AddField("resolution_h", Screen.currentResolution.height);
            json.AddField("fullscreen", true);

            json.AddField("debug", false);

            Save();
        }
    }
}