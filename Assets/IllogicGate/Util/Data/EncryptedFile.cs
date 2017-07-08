
using System.IO;

namespace IllogicGate.Data
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Saves / Load data into an encrypted file
    /// </summary>
    public static class EncryptedFile
    {
        // --- Class Declaration ------------------------------------------------------------------------
        /// <summary> Default scrambled key to use when none given (Use Util.RestoreKey, Util.ShuffleKey) </summary>
        const string DefaultShuffledKey = @"391b7cedec6ed11be7248f0921e183e8";

        // --- Class Declaration ------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Reads bytes from an encrypted file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="shuffledKey">Optional encryption key (shuffled with Util.ShuffleKey)</param>
        /// <returns>A byte array with the decrypted data</returns>
        public static byte[] ReadBytes(string path, string shuffledKey = DefaultShuffledKey)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            byte [] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            stream.Close();

            BlowFishCS.BlowFish blowfish = new BlowFishCS.BlowFish(RestoreKey(shuffledKey));
            return blowfish.Decrypt_ECB(data);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Reads a string from an encrypted file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="shuffledKey">Optional encryption key (shuffled with Util.ShuffleKey)</param>
        /// <returns>A string with the decrypted data</returns>
        public static string ReadString(string path, string shuffledKey = DefaultShuffledKey)
        {
            return System.Text.Encoding.UTF8.GetString(ReadBytes(path, shuffledKey));
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Reads a JSON object from an encrypted file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="shuffledKey">Optional encryption key (shuffled with Util.ShuffleKey)</param>
        /// <returns>A JSON object with the decrypted data</returns>
        public static JSONObject ReadJSONObject(string path, string shuffledKey = DefaultShuffledKey)
        {
            return new JSONObject(ReadString(path, shuffledKey));
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Writes bytes to an encrypted file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="data">Binary data to save</param>
        /// <param name="shuffledKey">Optional encryption key (shuffled with Util.ShuffleKey)</param>
        public static void WriteBytes(string path, byte[] data, string shuffledKey = DefaultShuffledKey)
        {
            BlowFishCS.BlowFish blowfish = new BlowFishCS.BlowFish(RestoreKey(shuffledKey));
            data = blowfish.Encrypt_ECB(data);

            FileStream stream = new FileStream(path, FileMode.Create);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Writes a string to an encrypted file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="data">String data to save</param>
        /// <param name="shuffledKey">Optional encryption key (shuffled with ShuffleKey)</param>
        public static void WriteString(string path, string data, string shuffledKey = DefaultShuffledKey)
        {
            WriteBytes(path, System.Text.Encoding.UTF8.GetBytes(data), shuffledKey);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Writes a JSON object to an encrypted file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="data">JSON object data to save</param>
        /// <param name="shuffledKey">Optional encryption key (shuffled with ShuffleKey)</param>
        public static void WriteJSONObject(string path, JSONObject data, string shuffledKey = DefaultShuffledKey)
        {
            WriteString(path, data.ToString(false), shuffledKey);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Shuffles an encryption key for storing in a constant
        /// </summary>
        /// <param name="key">Key to shuffle</param>
        /// <returns></returns>
        public static string ShuffleKey(string key)
        {
            char[] ary = key.ToCharArray();
            for (int i = 0; i < ary.Length; ++i)
            {
                if ((ary[i] >= 'a' && ary[i] <= 'z') || (ary[i] >= 'A' && ary[i] <= 'Z') || (ary[i] >= '0' && ary[i] <= '9'))
                    ary[i] += (char)((i % 3) + 1);
            }
            return new string(ary);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Restores an encryption key from a shuffled version
        /// </summary>
        /// <param name="key">Key to restore</param>
        /// <returns></returns>
        public static string RestoreKey(string key)
        {
            char[] ary = key.ToCharArray();
            for (int i = 0; i < ary.Length; ++i)
            {
                if ((ary[i] >= 'a' && ary[i] <= 'z' + (char)6) || (ary[i] >= 'A' && ary[i] <= 'Z' + (char)6) || (ary[i] >= '0' && ary[i] <= '9' + (char)6))
                    ary[i] -= (char)((i % 3) + 1);
            }
            return new string(ary);
        }
    }
}