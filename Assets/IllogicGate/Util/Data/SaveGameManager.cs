using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace IllogicGate.Data
{
    // --- Interface Declaration --------------------------------------------------------------------
    /// <summary>
    /// Classes that can be saved or loaded must implement this interface
    /// </summary>
    public interface ISaveable
    {
        /// <summary> Unique id to identify the item inside the save file </summary>
        string id { get; }
        
        /// <summary> 
        /// Called when the user saves the game. 
        /// </summary>
        /// <returns> Save / Load data </returns>
        JSONObject OnSave();

        /// <summary>
        /// Called when the user loads the game
        /// </summary>
        /// <param name="json"> Save / Load data </param>
        void OnLoad(JSONObject json);
    }

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Class version of ISaveable that auto registers the object to be saved / loaded
    /// </summary>
    public abstract class Saveable : ISaveable
    {
        public string id { get; private set; }

        public Saveable(string id)
        {
            this.id = id;
            SaveGameManager.instance.Add(this);
        }

        public abstract void OnLoad(JSONObject json);
        public abstract JSONObject OnSave();
    }

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Used to save / load game data.
    /// To save and load, register your ISaveable objects with this class
    /// </summary>
    public class SaveGameManager : Singleton<SaveGameManager>
    {
        // --- Constants --------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> List of objects that can be saved / loaded </summary>
        private Dictionary<string, ISaveable> _list = new Dictionary<string, ISaveable>();

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Registers an object for saving / loading
        /// </summary>
        /// <param name="target">target to save laod</param>
        public void Add(ISaveable target)
        {
            _list.Add(target.id, target);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Unregisters an object for saving / loading
        /// </summary>
        /// <param name="target">target to save laod</param>
        public void Remove(ISaveable target)
        {
            if (_list.ContainsKey(target.id))
                _list.Remove(target.id);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Unregisters an object for saving / loading
        /// </summary>
        /// <param name="id">Unique id for the target</param>
        public void Remove(string id)
        {
            if (_list.ContainsKey(id))
                _list.Remove(id);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Creates a filename, with the format YYYYMMDD_HHMMSS
        /// </summary>
        /// <returns></returns>
        public string CreateFilename()
        {
            System.DateTime now = System.DateTime.Now;
            string str =  now.Year.ToString("0000")
                        + now.Month.ToString("00")
                        + now.Day.ToString("00")
                        + "_"
                        + now.Hour.ToString("00")
                        + now.Minute.ToString("00")
                        + now.Second.ToString("00");
            return str;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Returns an array with all the files saved in the save directory
        /// </summary>
        /// <returns></returns>
        public string[] GetSaveFileList()
        {
            return Directory.GetFiles(GetPath(null));
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Save every ISaveable object registered with Add()
        /// </summary>
        /// <param name="filename">Filename for the savefile</param>
        public void Save(string filename)
        {
            JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
            foreach(KeyValuePair<string, ISaveable> kvp in _list)
                json[kvp.Key] = kvp.Value.OnSave();

            EncryptedFile.WriteJSONObject(GetPath(filename), json);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Same as Load(string filename), but it loads the latest saved file
        /// </summary>
        /// <returns>True if the file was loaded, false if no savefile exists</returns>
        public bool Load()
        {
            List<string> files = new List<string>(Directory.GetFiles(GetPath(null)));
            if (files.Count == 0)
                return false;

            files.Sort((a, b) =>
            {
                System.TimeSpan diff = File.GetLastAccessTime(a).Subtract(File.GetLastAccessTime(b));
                return diff.TotalSeconds == 0 ? 0 : diff.TotalSeconds < 0 ? 1 : -1;
            });

            return Load(files[0]);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Load every ISaveable object registered with Add()
        /// </summary>
        /// <returns> true, if the file was loaded, false if it doesn't exist </returns>
        public bool Load(string filename)
        {
            if (!Exists(filename))
                return false;

            JSONObject json = EncryptedFile.ReadJSONObject(filename);

            foreach (KeyValuePair<string, ISaveable> kvp in _list)
            {
                if (json.HasField(kvp.Key))
                    kvp.Value.OnLoad(json[kvp.Key]);
#if !RELEASE
                else
                    Debug.LogWarning("Found id " + kvp.Key + " in save file, but no object to load it");
#endif
            }

            return true;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the save file exists
        /// </summary>
        /// <param name="filename"> file to check </param>
        /// <returns></returns>
        public bool Exists(string filename)
        {
            return File.Exists(GetPath(filename));
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Delete any saved data
        /// </summary>
        public void Delete(string filename)
        {
            if (Exists(filename))
                File.Delete(GetPath(filename));
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Returns the full save path for a given filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string GetPath(string filename)
        {
            string saveDirectory = Path.Combine(Application.persistentDataPath, "svgm");

            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            return filename == null ? saveDirectory : Path.Combine(saveDirectory, filename);
        }
    }
}