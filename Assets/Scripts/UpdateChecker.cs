using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class UpdateChecker : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        bool updateRequired;

        JSONObject charactersToDownload;

        string error;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void CheckForUpdates()
        {
            StartCoroutine(CheckCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator CheckCoroutine()
        {
            yield return StartCoroutine(CheckUpdateRequired());

            yield return StartCoroutine(GetCharacterList());

            print(error);
            print(charactersToDownload);

            // do we need to download files?
            if (!charactersToDownload.IsNull)
                yield return StartCoroutine(DownloadCharacterFiles());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator CheckUpdateRequired()
        {
            WWWForm form = new WWWForm();
            form.AddField("cmd", "chk_ver");
            form.AddField("ver", Application.version);

            WWW www = new WWW(Config.ServerURL, form.data);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                error = www.error;
                yield break;
            }

            // returns only 1 or 0
            updateRequired = www.text == "1";
            print(updateRequired);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator GetCharacterList()
        {
            // request character list
            WWWForm form = new WWWForm();
            form.AddField("cmd", "get_list");
            WWW www = new WWW(Config.ServerURL, form.data);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                error = www.error;
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
                    float progress = (downloadedSize + www.bytesDownloaded) / (float)totalSize;
                    print(progress);
                    yield return null;
                }

                // oops!
                if (!string.IsNullOrEmpty(www.error))
                {
                    error = www.error;
                    yield break;
                }

                // write file to disk
                System.IO.File.WriteAllBytes(CharacterFile.File.dataPath + "/" + filename, www.bytes);
            }
        }
    }
}