using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Options : IllogicGate.SingletonBehaviour<Options>
    {
        [System.Serializable]
        struct Volume
        {
            public Slider slider;
            public Text text;

            int _value;
            public int value
            {
                get { return _value; }
                set
                {
                    _value = Mathf.Clamp(value, 0, 100);
                    text.text = _value.ToString();
                    slider.value = _value;
                }
            }
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Dropdown difficulty = null;

        [SerializeField]
        Volume sfx;

        [SerializeField]
        Volume bgm;

        [SerializeField]
        Dropdown zoom = null;

        [SerializeField]
        Toggle fullscreen = null;

        [SerializeField]
        Button accept = null;

        [SerializeField]
        Button cancel = null;

        [SerializeField]
        Button credits = null;

        [SerializeField]
        Button filterSet = null;

        [SerializeField]
        GameObject creditsScreen = null;

        [SerializeField]
        Filters filterScreen = null;

        // --- Properties -------------------------------------------------------------------------------
        public bool isVisible { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            sfx.slider.onValueChanged.AddListener((float value) => 
            {
                sfx.value = Mathf.RoundToInt(value);
                SoundManager sndMgr = SoundManager.instance;
                sndMgr.sfxVolume = sfx.value / 100f;
                sndMgr.PlaySFX("ui_hover");
            });

            bgm.slider.onValueChanged.AddListener((float value) => 
            {
                bgm.value = Mathf.RoundToInt(value);
                SoundManager.instance.bgmVolume = bgm.value / 100f;
            });

            accept.onClick.AddListener(OnAccept);
            cancel.onClick.AddListener(Close);
            credits.onClick.AddListener(() => { StartCoroutine(ShowCredits()); });
            filterSet.onClick.AddListener(() => { StartCoroutine(ShowFilters()); });
        }

        // -----------------------------------------------------------------------------------
        private void Update()
        {
            if (Input.GetButtonDown("Cancel") && !creditsScreen.activeInHierarchy)
                Close();
        }

        // --- Methods ----------------------------------------------------------------------------------


        // -----------------------------------------------------------------------------------
        void OnAccept()
        {
            Data.Options opts = Data.Options.instance;

            if (!Screen.fullScreen)
                opts.resolution = new Vector2(Screen.width, Screen.height);

            opts.difficulty = (Config.Difficulty)difficulty.value;
            opts.sfxVolume = sfx.value;
            opts.bgmVolume = bgm.value;
            opts.fullScreen = fullscreen.isOn;
            opts.zoom = zoom.value + 2;

            Util.SetResolutionFromConfig();

            // save and close
            opts.Save();
            Close();
        }

        // -----------------------------------------------------------------------------------
        IEnumerator ShowCredits()
        {
            yield return StartCoroutine(Transition.instance.Show(false));
            creditsScreen.SetActive(true);
            yield return StartCoroutine(Transition.instance.Hide());
            while (!(Input.GetButtonDown("Cut") || Input.GetButtonDown("Cancel")))
                yield return null;
            yield return StartCoroutine(Transition.instance.Show(false));
            creditsScreen.SetActive(false);
            yield return StartCoroutine(Transition.instance.Hide());
        }

        // -----------------------------------------------------------------------------------
        IEnumerator ShowFilters()
        {
            yield return StartCoroutine(Transition.instance.Show(false));
            filterScreen.Show();
            yield return StartCoroutine(Transition.instance.Hide());
            while (!filterScreen.done)
                yield return null;
            yield return StartCoroutine(Transition.instance.Show(false));
            filterScreen.Hide();
            yield return StartCoroutine(Transition.instance.Hide());
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Just set it to "is visible" false. It does not actually hide it, since
        /// we need to fade out first
        /// </summary>
        void Close()
        {
            isVisible = false;
        }

        // -----------------------------------------------------------------------------------
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------
        public void Show()
        {
            isVisible = true;
            gameObject.SetActive(true);

            Data.Options opts = Data.Options.instance;

            difficulty.value = (int)opts.difficulty;
            sfx.value = opts.sfxVolume;
            bgm.value = opts.bgmVolume;
            fullscreen.isOn = opts.fullScreen;
            zoom.value = opts.zoom - 2;


            difficulty.Select();
        }
    }
}