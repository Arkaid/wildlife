using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IllogicGate;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SoundManager : SoundManager2D
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        static readonly string[] RoundClips = new string []
        {
            "Kurorak - Fulcrum",
            "Kurorak - Gravibender",
            "Intersekt - Track 02",
        };
        // --- Static Properties ------------------------------------------------------------------------
        static public new SoundManager instance { get { return IllogicGate.SoundManager2D.instance as SoundManager; } }

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Randomized list of bgms </summary>
        List<string> bgmPlayList = new List<string>();

        /// <summary> Name of the BGM currently playing </summary>
        public string currentBGM { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Awake()
        {
            base.Awake();
            bgmVolume = Config.instance.bgmVolume / 100f;
            sfxVolume = Config.instance.sfxVolume / 100f;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void PlayRandomRoundClip()
        {
            if (bgmPlayList.Count == 0)
            {
                bgmPlayList = new List<string>(RoundClips);
                bgmPlayList.Shuffle();
            }

            currentBGM = bgmPlayList[0];
            bgmPlayList.RemoveAt(0);
            PlayBGM(currentBGM);
        }
    }
}