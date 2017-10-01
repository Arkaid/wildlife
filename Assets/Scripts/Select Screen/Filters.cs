using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Filters : MonoBehaviour
    {
        [System.Serializable]
        struct Filter
        {
            public string keyword;
            public Troggle troggle;
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        static readonly char[] trimEnd = new char[] { ',', ' ' };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        List<Filter> filters = null;

        [SerializeField]
        Button okButton = null;

        [SerializeField]
        Button cancelButton = null;

        [SerializeField]
        CharacterGrid characterGrid = null;

        // --- Properties -------------------------------------------------------------------------------
        public bool done { get; private set; }
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            okButton.onClick.AddListener(OnOK);
            cancelButton.onClick.AddListener(OnCancel);
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Show()
        {
            done = false;
            gameObject.SetActive(true);
            UpdateTroggles();
        }

        // -----------------------------------------------------------------------------------	
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        void UpdateTroggles()
        {
            List<string> showList = Data.Options.instance.showKeywordsList;
            List<string> hideList = Data.Options.instance.hideKeywordsList;

            foreach (Filter filter in filters)
            {
                string[] keyword_arr = filter.keyword.Split(Data.Options.KeywordSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                filter.troggle.value = Troggle.Value.None;
                foreach(string keyword in keyword_arr)
                {
                    if (showList.Contains(keyword))
                        filter.troggle.value = Troggle.Value.Yes;
                    else if (hideList.Contains(keyword))
                        filter.troggle.value = Troggle.Value.No;
                }
            }
        }

        // -----------------------------------------------------------------------------------	
        void OnOK()
        {

            string show = "";
            string hide = "";

            foreach (Filter filter in filters)
            {
                if (filter.troggle.value == Troggle.Value.Yes)
                    show += filter.keyword + ", ";
                else if (filter.troggle.value == Troggle.Value.No)
                    hide += filter.keyword + ", ";
            }

            show = show.TrimEnd(trimEnd);
            hide = hide.TrimEnd(trimEnd);

            Data.Options.instance.showKeywords = show;
            Data.Options.instance.hideKeywords = hide;
            Data.Options.instance.Save();

            characterGrid.Paginate();

            OnCancel();
        }

        // -----------------------------------------------------------------------------------	
        void OnCancel()
        {
            done = true;
        }
    }
}