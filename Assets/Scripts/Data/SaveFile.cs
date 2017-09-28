using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Data
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Records (Best time, High score, etc) that can
    /// be assigned to given playthrough. You can
    /// change these values then call SaveFile.Save()
    /// to save them to disk
    /// </summary>
    public class Records
    {
        /// <summary> Best time. -1 = N/A. - Seconds </summary>
        public float bestTime;

        /// <summary> High score. -1 = N/A </summary>
        public long highScore;

        /// <summary> Serializes to JSON </summary>
        public JSONObject ToJSON()
        {
            JSONObject json = new JSONObject();
            json.AddField("best_time", bestTime);
            json.AddField("high_score", highScore);
            return json;
        }

        /// <summary> Deserializes from JSON </summary>
        public Records(JSONObject json)
        {
            bestTime = json["best_time"].f;
            highScore = json["high_score"].i;
        }

        /// <summary> Creates a new entry, with N/A values </summary>
        public Records()
        {
            bestTime = -1;
            highScore = -1;
        }
    }

    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundData
    {
        /// <summary> True if this round has been cleared in any difficulty </summary>
        public bool cleared;

        /// <summary> Record socres for each difficulty </summary>
        public Dictionary<Config.Difficulty, Records> records { get; private set; }

        /// <summary> Serializes to JSON </summary>
        public JSONObject ToJSON()
        {
            JSONObject json = new JSONObject();

            json.AddField("cleared", cleared);
            foreach(KeyValuePair<Config.Difficulty, Records> kvp in records)
                json.AddField(kvp.Key.ToString(), kvp.Value.ToJSON());
            return json;
        }

        /// <summary> Deserializes from JSON </summary>
        public RoundData(JSONObject json)
        {
            cleared = json["cleared"].b;
            records = new Dictionary<Config.Difficulty, Records>();

            foreach (Config.Difficulty diff in System.Enum.GetValues(typeof(Config.Difficulty)))
                records.Add(diff, new Records(json[diff.ToString()]));
        }

        /// <summary> Creates an empty data entry </summary>
        public RoundData()
        {
            cleared = false;
            records = new Dictionary<Config.Difficulty, Records>();

            foreach (Config.Difficulty diff in System.Enum.GetValues(typeof(Config.Difficulty)))
                records.Add(diff, new Records());
        }
    }

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Holds the stats for a given character (which
    /// rounds are cleared, scores, etc)
    /// </summary>
    public class CharacterStats
    {
        /// <summary> GUID to identify the character. This matches CharacterDataFile.guid </summary>
        public string guid { get; private set; }

        /// <summary> true, if the character was played (regardless whehter it was beat or not) </summary>
        public bool played;

        /// <summary> Array of rounds, 0 to Config.Rounds - 1 </summary>
        public RoundData[] rounds { get; private set; }

        /// <summary> Serializes to JSON </summary>
        public JSONObject ToJSON()
        {
            JSONObject json = new JSONObject();
            json.AddField("guid", guid);
            json.AddField("played", played);

            JSONObject jsonList = new JSONObject();
            foreach (RoundData round in rounds)
                jsonList.Add(round.ToJSON());
            json.AddField("rounds", jsonList);

            return json;
        }

        /// <summary> Deserializes from JSON </summary>
        public CharacterStats(JSONObject json)
        {
            guid = json["guid"].str;
            played = json["played"].b;
            rounds = new RoundData[Config.Rounds];
            for (int i = 0; i < Config.Rounds; i++)
                rounds[i] = new RoundData(json["rounds"][i]);
        }

        /// <summary>
        /// Creates an empty data entry for a new character
        /// </summary>
        public CharacterStats(string guid)
        {
            this.guid = guid;
            played = false;
            rounds = new RoundData[Config.Rounds];
            for (int i = 0; i < Config.Rounds; i++)
                rounds[i] = new RoundData();
        }
    }

    // --- Class Declaration ------------------------------------------------------------------------
    public class SaveFile : IllogicGate.Singleton<SaveFile>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        const string CurrentVersion = SaveFileVer1;

        const string SaveFileVer1 = "VERSION_1";
        const string SaveFileVer2 = "VERSION_2";

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Location of the progress file </summary>
        string filePath { get { return Application.persistentDataPath + "/progress.sav"; } }

        /// <summary> Saved data for each character, by guid </summary>
        Dictionary<string, CharacterStats> characterDataByGUID = new Dictionary<string, CharacterStats>();

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void OnInstanceCreated()
        {
            Load();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Retreives stats for a character by guid. 
        /// Creates an empty one if it's the first time we play it
        /// </summary>
        public CharacterStats GetCharacterStats(string guid)
        {
            if (!characterDataByGUID.ContainsKey(guid))
                characterDataByGUID.Add(guid, new CharacterStats(guid));

            return characterDataByGUID[guid];
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Saves all changes to an encrypted save file
        /// </summary>
        public void Save()
        {
            JSONObject json = new JSONObject();
            json.AddField("version", SaveFileVer1);

            JSONObject jsonList = new JSONObject();
            foreach (CharacterStats data in characterDataByGUID.Values)
                jsonList.Add(data.ToJSON());
            json.AddField("characters", jsonList);

            IllogicGate.Data.EncryptedFile.WriteJSONObject(filePath, json);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads encrypted saved data
        /// </summary>
        public void Load()
        {
            if (!System.IO.File.Exists(filePath))
                return;

            JSONObject json = IllogicGate.Data.EncryptedFile.ReadJSONObject(filePath);

            // If necessary, add the missing stuff to the savefile for a valid JSON
            json = UpdateToCurrentVersion(json);

            characterDataByGUID = new Dictionary<string, CharacterStats>();
            foreach (JSONObject item in json["characters"].list)
            {
                CharacterStats data = new CharacterStats(item);
                characterDataByGUID.Add(data.guid, data);
            }
        }

        // -----------------------------------------------------------------------------------	
        JSONObject UpdateToCurrentVersion(JSONObject json)
        {
            switch (json["version"].str)
            {
                case SaveFileVer1:
                    // in version 2:
                    // added UNLOCK letter collection
                    break;
            }

            return json;
        }
    }
}