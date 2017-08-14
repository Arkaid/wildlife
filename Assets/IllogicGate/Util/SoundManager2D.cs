using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Class for handling BGM and SFX for a 2D game (non-spatial sounds)
    /// </summary>
    public class SoundManager2D : SingletonBehaviour<SoundManager2D>
    {
        /// <summary> Data for one audio clip </summary>
        [System.Serializable]
        class ClipData
        {
            public AudioClip clip = null;   // clip
            [Range(0, 1)]
            public float volume = 1;        // volume to play at
            public bool loop = false;       // loops?
            public int priority = 128;      // priority
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        ClipData [] clips;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Listener </summary>
        AudioListener listener;

        /// <summary> background music </summary>
        AudioSource bgm;

        /// <summary> list of sources for each SFX </summary>
        List<AudioSource> sfx = new List<AudioSource>();

        /// <summary> Arrange clips by name </summary>
        Dictionary<string, ClipData> clipsByName;

        /// <summary> Volume for sound effects </summary>
        public float sfxVolume { get { return _sfxVolume; } set { SetSFXVolume(value); } }
        float _sfxVolume = 1;

        /// <summary> Volume for background music </summary>
        public float bgmVolume { get { return _bgmVolume; } set { SetBGMVolume(value); } }
        float _bgmVolume = 1;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Awake()
        {
            base.Awake();
            //DontDestroyOnLoad(gameObject);

            // creates an audio listener, so remove anything 
            // that cameras may have!
            listener = gameObject.AddComponent<AudioListener>();

            // make clips easier to find
            clipsByName = new Dictionary<string, ClipData>();
            foreach (ClipData clip in clips)
                clipsByName[clip.clip.name] = clip;
        }

        // -----------------------------------------------------------------------------------	
        void SetSFXVolume(float value)
        {
            // sets the new volume and re-volumes clips already playing
            _sfxVolume = Mathf.Clamp01(value);
            foreach(AudioSource source in sfx)
                source.volume = _sfxVolume * clipsByName[source.clip.name].volume;
        }

        // -----------------------------------------------------------------------------------	
        void SetBGMVolume(float value)
        {
            // sets the new volume and re-volumes clips already playing
            _bgmVolume = Mathf.Clamp01(value);
            bgm.volume = _bgmVolume * clipsByName[bgm.clip.name].volume;
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Start playing a background music clip immediately
        /// </summary>
        public void PlayBGM(string clip)
        {
            ClipData data = clipsByName[clip];

            if (bgm == null)
                bgm = gameObject.AddComponent<AudioSource>();
            bgm.Stop();
            bgm.clip = data.clip;
            bgm.loop = data.loop;
            bgm.volume = data.volume * bgmVolume;
            bgm.priority = data.priority;
            bgm.Play();
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Crossfades the BGM clip to a new one, in the specified time
        /// </summary>
        public void CrossFadeBGM(string clip, float time = 2f)
        {
            StartCoroutine(CrossFadeBGM(clipsByName[clip], time));
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator CrossFadeBGM(ClipData data, float time)
        {
            // create a temporary audio source to fade
            AudioSource tmpSrc = gameObject.AddComponent<AudioSource>();
            tmpSrc.clip = data.clip;
            tmpSrc.loop = data.loop;
            tmpSrc.volume = 0;
            tmpSrc.priority = data.priority;
            tmpSrc.Play();

            // fade from volume 1 to volume 2
            float elapsed = 0;
            float vol_1 = clipsByName[bgm.clip.name].volume * bgmVolume;
            float vol_2 = data.volume * bgmVolume;
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / time);

                bgm.volume = Mathf.Lerp(vol_1, 0, t);
                tmpSrc.volume = Mathf.Lerp(0, vol_2, t);

                yield return null;
            }

            // destroy the old audiosource and replace with temporary
            Destroy(bgm);
            bgm = tmpSrc;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Plays a sound effect
        /// </summary>
        public void PlaySFX(string clip)
        {
            StartCoroutine(PlaySFX(clipsByName[clip]));
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator PlaySFX(ClipData data)
        {
            // create a new audio source for the clip
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = data.clip;
            source.loop = data.loop;
            source.volume = data.volume * sfxVolume;
            source.priority = data.priority;

            // play it and add it to the list
            source.Play();
            sfx.Add(source);
            while (source.isPlaying)
                yield return null;

            // remove it from list and destroy source
            sfx.Remove(source);
            Destroy(source);
        }
    }
}