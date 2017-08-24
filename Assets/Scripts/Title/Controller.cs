using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Jintori.Title
{
    using Common.UI;

    // --- Class Declaration ------------------------------------------------------------------------
    public class Controller : MonoBehaviour
    {
        [System.Serializable]
        struct ProgressBar
        {
            [SerializeField]
            GameObject root;

            [SerializeField]
            Text valueText;

            [SerializeField]
            RectTransform fillImage;

            public float value { set { SetValue(value); } }

            public void Show()
            {
                root.gameObject.SetActive(true);
            }
            public void Hide()
            {
                root.gameObject.SetActive(false);
            }
            void SetValue(float value)
            {
                valueText.text = Mathf.RoundToInt(value * 100f).ToString() + "%";
                fillImage.anchorMax = new Vector2(value, 1);
            }
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        const string NewVersionAvailable = "NEW VERSION\nAVAILABLE!";

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Text messageText = null;

        [SerializeField]
        ProgressBar progress;

        // --- Properties -------------------------------------------------------------------------------
        Animator animator;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator Start()
        {
            if (Config.instance.debugOn)
                IllogicGate.LogToFile.Activate();

            Debug.Log("Application started. Client version: " + Application.version);

            SoundManager.instance.PlayBGM("outgame");

            animator = GetComponent<Animator>();
            
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Updating Content"))
                yield return null;

            progress.Hide();
            messageText.text = "CHECKING FOR UPDATES";

            UpdateChecker checker = gameObject.AddComponent<UpdateChecker>();
            checker.CheckForUpdates();
            while (checker.state == UpdateChecker.State.CheckingForUpdates)
                yield return null;

            // application update required. Show popup, then continue
            if (checker.state == UpdateChecker.State.ApplicationUpdateRequired)
            {
                Overlay.instance.messagePopup.Show(NewVersionAvailable, "NEWS");
                while (Overlay.instance.messagePopup.isVisible)
                    yield return null;
                StartCoroutine(PressStart());
                yield break;
            }

            // something happened (server down / no internet connection)
            else if (checker.state == UpdateChecker.State.Error)
            {
                StartCoroutine(PressStart());
                yield break;
            }

            // we should be downloading characters now!
            messageText.text = "UPDATING CHARACTERS";
            progress.Show();
            while (checker.state == UpdateChecker.State.DownloadingCharacters)
            {
                progress.value = checker.progress;
                yield return null;
            }
            progress.Hide();
            StartCoroutine(PressStart());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator PressStart()
        {
            animator.SetBool("update_done", true);
            messageText.text = "PRESS ANY KEY";
            while (!Input.anyKeyDown)
                yield return null;

            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Select Menu");
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
    }
}