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

        [SerializeField]
        CharacterIcon sampleIcon;

        [SerializeField]
        RoundIcon round4 = null;

        [SerializeField]
        CharacterIcon random = null;

        [SerializeField]
        UnityEngine.UI.Selectable startButton = null;

        [SerializeField]
        Button nextPageButton = null;

        [SerializeField]
        Button prevPageButton = null;

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

        /// <summary> Currently selected icon </summary>
        public CharacterIcon selected { get; private set; }

        /// <summary> Icon we last hovered on </summary>
        public CharacterIcon lastHover { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            nextPageButton.onClick.AddListener(NextPage);
            prevPageButton.onClick.AddListener(PrevPage);

            sampleIcon.transform.SetParent(null, true);
            sampleIcon.gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        private void Update()
        {
            // check if we need to move to the next / prev page when using controllers
            int idx = lastHover.transform.GetSiblingIndex();
            int x = idx % iconsPerPageX;

            int dx = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            if (x == 0 && dx == -1)
                PrevPage();
            else if (x == iconsPerPageX - 1 && dx == 1)
                NextPage();
        }

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
        public void AddCharacter()
        {
            CharacterIcon newIcon = Instantiate(sampleIcon, pagesRoot, true);
            newIcon.gameObject.SetActive(true);
            newIcon.select += OnCharacterSelected;
            newIcon.hoverIn += OnCharacterHoveredIn;
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterHoveredIn(Selectable sender)
        {
            lastHover = sender as CharacterIcon;

            // let round4 where to navigate upon returning
            Navigation navi = round4.navigation;
            navi.selectOnRight = lastHover;
            navi.selectOnDown = lastHover;
            round4.navigation = navi;
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSelected(Selectable sender)
        {
            selected = sender as CharacterIcon;
        }

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
        public void Paginate()
        {
            // first, deparent all icons
            List<CharacterIcon> icons = new List<CharacterIcon>(GetComponentsInChildren<CharacterIcon>());
            icons.Remove(random);
            foreach (CharacterIcon icon in icons)
                icon.transform.SetParent(null, true);

            // haven't added any characters yet
            if (icons.Count == 0)
                return;

            // delete all the old pages
            while (pagesRoot.childCount > 0)
            {
                Transform page = pagesRoot.GetChild(0);
                page.SetParent(null);
                Destroy(page.gameObject);
            }

            // one icon space is for the "random" button
            int iconsPerPage = (iconsPerPageX * iconsPerPageY) - 1;
            pageCount = Mathf.CeilToInt(icons.Count / (float)iconsPerPage);

            // start creating pages
            int icon_idx = 0;
            float page_w = widthController.minWidth;
            for (int i = 0; i < pageCount; i++)
            {
                RectTransform page = new GameObject("Page " + (i + 1)).AddComponent<RectTransform>();
                page.SetParent(pagesRoot);
                page.localScale = Vector3.one;
                page.anchorMax = Vector2.one;
                page.anchorMin = Vector2.zero;
                page.offsetMax = Vector2.one * -2; // 2px padding on each side
                page.offsetMin = Vector2.one * 2;
                page.anchoredPosition = new Vector2(i * page_w, 0);

                // add icons to the page
                for (int j = 0; j < iconsPerPage && icon_idx < icons.Count; j++, icon_idx++)
                {
                    int x = j % iconsPerPageX;
                    int y = j / iconsPerPageX;

                    RectTransform tf = icons[icon_idx].GetComponent<RectTransform>();
                    tf.SetParent(page, true);
                    tf.anchoredPosition = new Vector2(x, -y) * IconSize;
                }

                // fix navigation for all icons in one page
                int iconCount = page.childCount;
                for (int j = 0; j < iconCount; j++)
                {
                    // indices for left, up, down and right elements in the page
                    int x = j % iconsPerPageX;
                    int y = j / iconsPerPageX;
                    int lt = x == 0 ? -1 : (x - 1) + y * iconsPerPageX;
                    int rt = x == iconsPerPageX - 1 ? -1 : (x + 1) + y * iconsPerPageX;
                    int up = y == 0 ? -1 : x + (y - 1) * iconsPerPageX;
                    int dw = y == iconsPerPageY - 1 ? -1 : x + (y + 1) * iconsPerPageX;

                    // now connect the navigation
                    Selectable icon = page.GetChild(j).GetComponent<Selectable>();
                    Navigation navi = icon.navigation;
                    navi.mode = Navigation.Mode.Explicit;

                    navi.selectOnLeft = (lt >= 0 && lt < iconCount) ? page.GetChild(lt).GetComponent<Selectable>() : null;
                    navi.selectOnRight = (rt >= 0 && rt < iconCount) ? page.GetChild(rt).GetComponent<Selectable>() : null;
                    navi.selectOnUp = (up >= 0 && up < iconCount) ? page.GetChild(up).GetComponent<Selectable>() : round4;
                    navi.selectOnDown = (dw >= 0 && dw < iconCount) ? page.GetChild(dw).GetComponent<Selectable>() : startButton;

                    // special cases for the "random" button
                    if (j == iconCount - 1) // last button on the page
                        navi.selectOnRight = random;
                    if (x == iconsPerPageX - 1 && y == iconsPerPageY - 2) // the one on top of the random button
                        navi.selectOnDown = random;

                    icon.navigation = navi;
                }
            }
            SetRandomButtonNavigation(pagesRoot.GetChild(currentPage));
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

            // can't scroll 
            int next = currentPage + offset;
            if (next >= pageCount || next < 0 || isScrolling)
                yield break;

            isScrolling = true;

            // just move the current and next pages
            RectTransform page_a = pagesRoot.GetChild(currentPage).GetComponent<RectTransform>();
            RectTransform page_b = pagesRoot.GetChild(next).GetComponent<RectTransform>();

            // by this much (page width). offset here gives us direction
            float dx = widthController.minWidth * offset;

            float elapsed = 0;
            while (elapsed < ScrollTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / ScrollTime);
                t = IllogicGate.Tweener.EaseOut(t);

                // move one page in and the other out
                page_a.anchoredPosition = new Vector2(-dx * t, 0);
                page_b.anchoredPosition = new Vector2(dx * (1 - t), 0);
                yield return null;
            }

            SetRandomButtonNavigation(page_b);

            currentPage = next;
            isScrolling = false;
        }

        // -----------------------------------------------------------------------------------	
        void SetRandomButtonNavigation(Transform page)
        {
            Navigation navi = random.navigation;
            navi.mode = Navigation.Mode.Explicit;
            navi.selectOnDown = startButton;

            // set up the random button navigation for this page
            // 1) the one on top of the random button
            int idx = (iconsPerPageX - 1) + (iconsPerPageY - 2) * iconsPerPageX;
            if (idx < page.childCount)
                navi.selectOnUp = page.GetChild(idx).GetComponent<Selectable>();
            // 2) the last one
            idx = page.childCount - 1;
            navi.selectOnLeft = page.GetChild(idx).GetComponent<Selectable>();

            random.navigation = navi;
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