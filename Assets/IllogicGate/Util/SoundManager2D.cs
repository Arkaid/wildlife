using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SoundManager2D : SingletonBehaviour<SoundManager2D>
    {
        [System.Serializable]
        class ClipData
        {
            public AudioClip clip = null;
            [Range(0, 1)]
            public float volume = 1;
            public bool loop = false;
            public int priority = 128;
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

        float _sfxVolume = 1;
        public float sfxVolume { get { return _sfxVolume; } set { SetSFXVolume(value); } }

        float _bgmVolume = 1;
        public float bgmVolume { get { return _bgmVolume; } set { SetBGMVolume(value); } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            listener = gameObject.AddComponent<AudioListener>();
            clipsByName = new Dictionary<string, ClipData>();
            foreach (ClipData clip in clips)
                clipsByName[clip.clip.name] = clip;
        }

        // -----------------------------------------------------------------------------------	
        void SetSFXVolume(float value)
        {
            _sfxVolume = Mathf.Clamp01(value);
            foreach(AudioSource source in sfx)
                source.volume = _sfxVolume * clipsByName[source.clip.name].volume;
        }

        // -----------------------------------------------------------------------------------	
        void SetBGMVolume(float value)
        {
            _bgmVolume = Mathf.Clamp01(value);
            bgm.volume = _bgmVolume * clipsByName[bgm.clip.name].volume;
        }
        
        // -----------------------------------------------------------------------------------	
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
        public void CrossFadeBGM(string clip, float time = 2f)
        {
            StartCoroutine(CrossFadeBGM(clipsByName[clip], time));
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator CrossFadeBGM(ClipData data, float time)
        {
            AudioSource tmpSrc = gameObject.AddComponent<AudioSource>();
            tmpSrc.clip = data.clip;
            tmpSrc.loop = data.loop;
            tmpSrc.volume = 0;
            tmpSrc.priority = data.priority;
            tmpSrc.Play();

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

            Destroy(bgm);
            bgm = tmpSrc;
        }

        // -----------------------------------------------------------------------------------	
        public void PlaySFX(string clip)
        {
            StartCoroutine(PlaySFX(clipsByName[clip]));
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator PlaySFX(ClipData data)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = data.clip;
            source.loop = data.loop;
            source.volume = data.volume * sfxVolume;
            source.priority = data.priority;

            source.Play();
            sfx.Add(source);
            while (source.isPlaying)
                yield return null;
            sfx.Remove(source);
            Destroy(source);
        }
    }
}