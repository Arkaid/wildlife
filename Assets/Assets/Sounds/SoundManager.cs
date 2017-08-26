using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SoundManager : IllogicGate.SoundManager2D
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        static readonly string[] RoundClips = new string []
        {
            "Kurorak - Fulcrum",
        };
        // --- Static Properties ------------------------------------------------------------------------
        static public new SoundManager instance { get { return IllogicGate.SoundManager2D.instance as SoundManager; } }

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
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
            string randomClip = RoundClips[Random.Range(0, RoundClips.Length)];
            PlayBGM(randomClip);
        }
    }
}