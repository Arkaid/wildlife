using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Title
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Checks the server for updates and downloads new characters
    /// </summary>
    public class UpdateChecker : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public enum State
        {
            CheckingForUpdates,
            ApplicationUpdateRequired,
            DownloadingCharacters,
            Error,
            Done,
        }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Current state of the checker </summary>
        public State state { get; private set; }

        /// <summary> download progress </summary>
        public float progress { get; private set; }

        JSONObject charactersToDownload;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Checks for updates and download new character data
        /// </summary>
        public void CheckForUpdates()
        {
            state = State.CheckingForUpdates;
            StartCoroutine(CheckCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator CheckCoroutine()
        {
            yield return StartCoroutine(CheckUpdateRequired());
            if (state == State.Error || state == State.ApplicationUpdateRequired)
                yield break;

            // get a list of characters in the server that we haven't downloaded yet
            yield return StartCoroutine(GetCharacterList());
            if (state == State.Error)
                yield break;

            // do we need to download files?
            if (!charactersToDownload.IsNull)
            {
                progress = 0;
                state = State.DownloadingCharacters;
                yield return StartCoroutine(DownloadCharacterFiles());
            }
            state = State.Done;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Checks if there is a new version of the application that we need to download
        /// </summary>
        IEnumerator CheckUpdateRequired()
        {
            WWWForm form = new WWWForm();
            form.AddField("cmd", "chk_ver");
            form.AddField("ver", Application.version);

            WWW www = new WWW(Config.ServerURL, form.data);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                state = State.Error;
                Debug.Log(www.error);
                yield break;
            }

            // returns only 1 or 0
            if (www.text == "1")
                state = State.ApplicationUpdateRequired;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Gets a full character list from server, then compares with the already
        /// installed files and makes a list of the characters we need to download
        /// </summary>
        /// <returns></returns>
        IEnumerator GetCharacterList()
        {
            // request character list
            WWWForm form = new WWWForm();
            form.AddField("cmd", "get_list");
            WWW www = new WWW(Config.ServerURL, form.data);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                state = State.Error;
                Debug.Log(www.error);
                yield break;
            }

            // got the list from the server
            JSONObject remoteList = new JSONObject(www.text);

            // check the locally installed files
            JSONObject localList = CharacterFile.File.GetInstalledCharacterFileData();

            // download characters that do not exist
            // locally or which checksum is different
            charactersToDownload = new JSONObject();
            System.Func<JSONObject, JSONObject, bool> match = (a, b) =>
            {
                return
                    a["name"].str == b["name"].str &&         // name match
                    (long)a["size"].n == (long)b["size"].n && // size match
                    a["hash"].str == b["hash"].str;           // hash match
            };
            foreach (JSONObject item in remoteList.list)
            {
                // we did not find a match in the local list
                // so we need to download it
                if (localList.list.Find(j => match(j, item)) == null)
                    charactersToDownload.Add(item);
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Downloads one file at the time
        /// </summary>
        /// <returns></returns>
        IEnumerator DownloadCharacterFiles()
        {
            // calculate total size
            long totalSize = 0;
            foreach (JSONObject item in charactersToDownload.list)
                totalSize += (long)item["size"].n;

            // begin downloading one file at the time
            long downloadedSize = 0;
            foreach (JSONObject item in charactersToDownload.list)
            {
                string filename = item["name"].str;
                WWW www = new WWW(Config.CharactersURL + filename);
                while(!www.isDone)
                {
                    progress = (downloadedSize + www.bytesDownloaded) / (float)totalSize;
                    yield return null;
                }
                downloadedSize += www.bytesDownloaded;

                // oops!
                if (!string.IsNullOrEmpty(www.error))
                {
                    state = State.Error;
                    Debug.Log(www.error);
                    yield break;
                }

                // write file to disk
                string filepath = CharacterFile.File.dataPath + "/" + filename;
                if (System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);
                System.IO.File.WriteAllBytes(filepath, www.bytes);
            }
        }
    }
}