using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterGrid : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        const float IconSize = 150;

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField, Tooltip("Layout element to control the height of the grid")]
        LayoutElement heightController = null;

        [SerializeField, Tooltip("Layout element to control the width of the grid")]
        LayoutElement widthController = null;

        [SerializeField]
        RectTransform pagesRoot = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Number of icons per page, in the x axis </summary>
        int iconsPerPageX;
        
        /// <summary> Number of icons per page, in the y axis </summary>
        int iconsPerPageY;

        /// <summary> number of available pages </summary>
        int pageCount; 

        /// <summary> Index of the current page </summary>
        int currentPage = 0;

        /// <summary> To avoid scrolling while scrolling </summary>
        bool isScrolling = false;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Called after a layout. We do the resizing of the inner grid here
        /// </summary>
        void OnRectTransformDimensionsChange()
        {
            StartCoroutine(Resize());
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator Resize()
        {
            // make the grid as tiny as possible as to make sure it fits inside the UI
            // but give it a really large flexible size, so it tries to fit as much
            // as the UI as it can
            heightController.minHeight = IconSize;
            heightController.preferredHeight = IconSize;
            heightController.flexibleHeight = 500000;
            widthController.minWidth = IconSize;
            widthController.preferredWidth = IconSize;
            widthController.flexibleWidth = 500000;

            // needed here, otherwise ForceRebuildLayoutImmediate doesn't work
            // for some goddamn reason
            yield return null;

            // force the rect to resize. This will give us the
            // actual amount of available space for the grid
            RectTransform rt = GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

            // amazingly, this is needed here too!
            yield return null;

            // okay, so now we have the maximum amount of available
            // space for the grid
            float w = rt.sizeDelta.x;
            float h = rt.sizeDelta.y;

            // make sure we make it nice and snug to fit an integer amount of icons
            iconsPerPageX = Mathf.FloorToInt(w / IconSize);
            iconsPerPageY = Mathf.FloorToInt(h / IconSize);
            w = (iconsPerPageX * IconSize) + 4; // 2px padding on each side
            h = (iconsPerPageY * IconSize) + 4; 

            // now resize it again
            heightController.minHeight = h;
            heightController.preferredHeight = h;
            heightController.flexibleHeight = 0;
            widthController.minWidth = w;
            widthController.preferredWidth = w;
            widthController.flexibleWidth = 0;

            Paginate();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// (Re)creates pages according to the number of characters / icons available
        /// </summary>
        void Paginate()
        {
            // first, deparent all icons
            CharacterIcon[] icons = GetComponentsInChildren<CharacterIcon>();
            foreach(CharacterIcon icon in icons)
                icon.transform.SetParent(null, true);

            // delete all the old pages
            while(pagesRoot.childCount > 0)
            {
                Transform page = pagesRoot.GetChild(0);
                page.SetParent(null);
                Destroy(page.gameObject);
            }

            // one icon space is for the "random" button
            int iconsPerPage = (iconsPerPageX * iconsPerPageY) - 1;
            pageCount = Mathf.CeilToInt(icons.Length / (float)iconsPerPage);

            // start creating pages
            int icon_idx = 0;
            float page_w = widthController.minWidth;
            for (int i = 0; i < pageCount; i++)
            {
                RectTransform page = new GameObject("Page " + (i + 1)).AddComponent<RectTransform>();
                page.SetParent(pagesRoot);
                page.anchoredPosition = new Vector2(i * page_w, 0);
                page.localScale = Vector3.one;
                page.anchorMax = Vector2.one;
                page.anchorMin = Vector2.zero;
                page.offsetMax = Vector2.one * -2; // 2px padding on each side
                page.offsetMin = Vector2.one * 2;

                // add icons to the page
                for (int j = 0; j < iconsPerPage && icon_idx < icons.Length; j++, icon_idx++)
                {
                    int x = j % iconsPerPageX;
                    int y = j / iconsPerPageX;

                    RectTransform tf = icons[icon_idx].GetComponent<RectTransform>();
                    tf.SetParent(page, true);
                    tf.anchoredPosition = new Vector2(x, -y) * IconSize;
                }
            }
        }

        // -----------------------------------------------------------------------------------	
        public void NextPage()
        {
            StartCoroutine(ScrollPage(1));
        }
        
        // -----------------------------------------------------------------------------------	
        public void PrevPage()
        {
            StartCoroutine(ScrollPage(-1));
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator ScrollPage(int offset)
        {
            const float ScrollTime = 0.45f;

            int next = currentPage + offset;
            if (next >= pageCount || next < 0 || isScrolling)
                yield break;

            isScrolling = true;

            RectTransform page_a = pagesRoot.GetChild(currentPage).GetComponent<RectTransform>();
            RectTransform page_b = pagesRoot.GetChild(next).GetComponent<RectTransform>();

            float dx = widthController.minWidth * offset;

            float elapsed = 0;
            while(elapsed < ScrollTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / ScrollTime);
                t = IllogicGate.Tweener.EaseOut(t);

                page_a.anchoredPosition = new Vector2(-dx * t, 0);
                page_b.anchoredPosition = new Vector2(dx * (1 - t), 0);
                yield return null;
            }

            currentPage = next;
            isScrolling = false;
        }


#if OLD
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Raised when the icon is switched </summary>
        public event System.Action<CharacterFile.File> selected;

        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Icon sampleIcon = null;

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            sampleIcon.gameObject.SetActive(false);
            Clear();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Clears the grid from all icons
        /// </summary>
        public void Clear()
        {
            while (transform.childCount > 1)
            {
                Transform child = transform.GetChild(0);
                if (child == sampleIcon.transform)
                    child = transform.GetChild(1);

                child.SetParent(null);
                DestroyObject(child.gameObject);
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Selects the first icon in the grid. You usually
        /// call this right after the menu screen opens
        /// </summary>
        public void SelectFirst()
        {
            Transform first = transform.GetChild(0);
            if (first == sampleIcon.transform)
                first = transform.GetChild(1);

            Toggle toggle = first.GetComponent<Toggle>();
            toggle.Select();
            toggle.isOn = true;
        }


        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Adds an icon to the grid
        /// </summary>
        public void Add(CharacterFile.File characterFile)
        {
            Icon newIcon = Instantiate(sampleIcon);
            newIcon.gameObject.SetActive(true);
            newIcon.Setup(characterFile);

            newIcon.selected += selected;

            newIcon.transform.SetParent(transform);
            newIcon.transform.localScale = Vector3.one;
        }
#endif
    }
}