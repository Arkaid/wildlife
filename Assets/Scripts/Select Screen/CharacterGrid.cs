using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterGrid : Selectable
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        const float IconSize = 150;

        public enum SortCriteria
        {
            DateCreated,
            DateUpdated,
            Unplayed,
            Artist,
            CharacterName,
        }

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
        Selectable startButton = null;

        [SerializeField]
        Button nextPageButton = null;

        [SerializeField]
        Button prevPageButton = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Becomes true when it's ready to be used </summary>
        public bool isReady { get { return iconsPerPageX != 0; } }

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

        /// <summary> Sort criteria for the characters </summary>
        SortCriteria sortBy = SortCriteria.DateUpdated;

        /// <summary> If true, sort backwards </summary>
        public bool sortReversed = false;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Start()
        {
            nextPageButton.onClick.AddListener(NextPage);
            prevPageButton.onClick.AddListener(PrevPage);

            //sampleIcon.transform.SetParent(transform.parent, true);
            sampleIcon.gameObject.SetActive(false);

            StartCoroutine(ResizeCheck());

            base.Start();
        }

        // -----------------------------------------------------------------------------------	
        public override void OnSelect(BaseEventData eventData)
        {
            // if the grid gets selected, select the icon that was last hovered
            // you need a 1 frame delay, since you can't releselect while selecting
            base.OnSelect(eventData);
            StartCoroutine(DelayedSelect());
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public CharacterFile.File SelectRandomCharacter()
        {
            CharacterIcon[] availableCharacters = GetComponentsInChildren<CharacterIcon>(true);

            while (true)
            {
                int rnd = Random.Range(0, availableCharacters.Length);
                if (availableCharacters[rnd].characterFile != null) // avoid reselecting random
                    return availableCharacters[rnd].characterFile;
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Add an icon to the character grid
        /// </summary>
        public CharacterIcon AddCharacter(CharacterFile.File file)
        {
            CharacterIcon newIcon = Instantiate(sampleIcon, pagesRoot, true);
            newIcon.name = file.guid;
            newIcon.characterFile = file;
            newIcon.gameObject.SetActive(true);
            newIcon.selected += OnCharacterSelected;
            newIcon.highlighted += OnCharacterHighlighted;

            return newIcon;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Sorts the icons on the grid by the given criteria
        /// </summary>
        List<CharacterIcon> GetSortedCharacters()
        {
            // get a list of characters, removing "random" icon
            List<CharacterIcon> availableCharacters = new List<CharacterIcon>(GetComponentsInChildren<CharacterIcon>(true));
            availableCharacters.Remove(random);

            // now sort
            switch (sortBy)
            {
                case SortCriteria.DateCreated:
                    availableCharacters.Sort(SortFunc_ByDateCreated); break;
                case SortCriteria.DateUpdated:
                    availableCharacters.Sort(SortFunc_ByDateUpdated); break;
                case SortCriteria.Artist:
                    availableCharacters.Sort(SortFunc_ByArtist); break;
                case SortCriteria.CharacterName:
                    availableCharacters.Sort(SortFunc_ByCharacterName); break;
                case SortCriteria.Unplayed:
                    availableCharacters.Sort(SortFunc_ByUnplayed); break;
            }

            if (sortReversed)
                availableCharacters.Reverse();

            return availableCharacters;
        }

        // -----------------------------------------------------------------------------------	
        int SortFunc_ByDateCreated(CharacterIcon a, CharacterIcon b)
        {
            System.TimeSpan diff = a.characterFile.createdDate - b.characterFile.createdDate;
            if (diff.TotalSeconds > 0)
                return 1;
            else if (diff.TotalSeconds < 0)
                return -1;
            else
                return 0;
        }
        // -----------------------------------------------------------------------------------	
        int SortFunc_ByDateUpdated(CharacterIcon a, CharacterIcon b)
        {
            System.TimeSpan diff = a.characterFile.updatedDate - b.characterFile.updatedDate;
            if (diff.TotalSeconds > 0)
                return 1;
            else if (diff.TotalSeconds < 0)
                return -1;
            else
                return 0;
        }
        // -----------------------------------------------------------------------------------	
        int SortFunc_ByArtist(CharacterIcon a, CharacterIcon b)
        {
            return string.Compare(a.characterFile.artist, b.characterFile.artist);
        }
        // -----------------------------------------------------------------------------------	
        int SortFunc_ByCharacterName(CharacterIcon a, CharacterIcon b)
        {
            return string.Compare(a.characterFile.characterName, b.characterFile.characterName);
        }
        // -----------------------------------------------------------------------------------	
        int SortFunc_ByUnplayed(CharacterIcon a, CharacterIcon b)
        {
            Data.CharacterStats stats_a = Data.SaveFile.instance.GetCharacterStats(a.characterFile.guid);
            Data.CharacterStats stats_b = Data.SaveFile.instance.GetCharacterStats(b.characterFile.guid);

            // if both are either played, or unplayed; sort by last updated
            if (stats_a.played == stats_b.played)
                return SortFunc_ByDateUpdated(a, b);

            // else, unplayed comes first
            if (!stats_a.played)
                return 1;
            else
                return -1;
        }
        // -----------------------------------------------------------------------------------	
        IEnumerator DelayedSelect()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(lastHover.gameObject);
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterHighlighted(CharacterIcon sender)
        {
            lastHover = sender;
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSelected(CharacterIcon sender)
        {
            selected = sender;
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator ResizeCheck()
        {
            int lastW = -1;
            int lastH = -1;

            while(true)
            {
                yield return null;
                if (Screen.width == lastW && Screen.height == lastH)
                    continue;
                lastW = Screen.width;
                lastH = Screen.height;
                StartCoroutine(Resize());
            }
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
            RectTransform rt = widthController.GetComponent<RectTransform>();
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
        /// A version of paginate that also changes the sorting order
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="reverse"></param>
        public void Paginate(SortCriteria sortBy, bool reverse)
        {
            this.sortBy = sortBy;
            sortReversed = reverse;
            Paginate();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// (Re)creates pages according to the number of characters / icons available
        /// </summary>
        public void Paginate()
        {
            // first, deparent all icons
            List<CharacterIcon> icons = GetSortedCharacters();
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

                    navi.selectOnLeft = (lt >= 0 && lt < iconCount) ? page.GetChild(lt).GetComponent<Selectable>() : prevPageButton as UnityEngine.UI.Selectable;
                    navi.selectOnRight = (rt >= 0 && rt < iconCount) ? page.GetChild(rt).GetComponent<Selectable>() : nextPageButton as UnityEngine.UI.Selectable;
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

            // we need less pages now
            if (currentPage >= pageCount)
                currentPage = pageCount - 1;

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
        /// <summary>
        /// Scrolls the character icon pages to the left (-1) or right (+1)
        /// </summary>
        IEnumerator ScrollPage(int offset)
        {
            const float ScrollTime = 0.45f;

            // can't scroll 
            int next = currentPage + offset;
            if (next >= pageCount || next < 0 || isScrolling)
                yield break;

            isScrolling = true;

            // just move the current and next pages
            RectTransform oldPage = pagesRoot.GetChild(currentPage).GetComponent<RectTransform>();
            RectTransform newPage = pagesRoot.GetChild(next).GetComponent<RectTransform>();

            // by this much (page width). offset here gives us direction
            float dx = widthController.minWidth * offset;

            float elapsed = 0;
            while (elapsed < ScrollTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / ScrollTime);
                t = IllogicGate.Tweener.EaseOut(t);

                // move one page in and the other out
                oldPage.anchoredPosition = new Vector2(-dx * t, 0);
                newPage.anchoredPosition = new Vector2(dx * (1 - t), 0);
                yield return null;
            }

            // update the navigation for the random button
            SetRandomButtonNavigation(newPage);

            // "lastHover" is now in the old page.
            // reselect the same position (or the last one int the page, if unavailable)
            int idx = lastHover.transform.GetSiblingIndex();
            if (idx >= newPage.childCount)
                idx = newPage.childCount - 1;
            lastHover = newPage.GetChild(idx).GetComponent<CharacterIcon>();

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

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Selects the first icon in the grid. You usually
        /// call this right after the menu screen opens
        /// </summary>
        public void SelectFirst()
        {
            Transform firstPage = pagesRoot.GetChild(0);
            Transform first = firstPage.GetChild(0);
            CharacterIcon icon = first.GetComponent<CharacterIcon>();
            icon.Select();
            icon.OnSubmit(null);
        }
    }
}