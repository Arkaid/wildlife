using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class BonusEffect : MonoBehaviour
    {
        [System.Serializable]
        struct Settings
        {
            public Type type;
            public Sprite sprite;
            public Transform target;
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public enum Type
        {
            ExtraLife,
            ExtraTime,
            SkillRecharge,
            Letter_U,
            Letter_N,
            Letter_L,
            Letter_O,
            Letter_C,
            Letter_K,
        }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        List<Settings> settings;

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Plays the effect
        /// </summary>
        /// <param name="from">Where it starts, in canvas units</param>
        /// <returns></returns>
        public IEnumerator Play(Type type, Vector3 from)
        {
            const float AnimationTime = 0.75f;

            Settings set = settings.Find(s => s.type == type);

            Image image = GetComponent<Image>();
            image.sprite = set.sprite;

            // convert play area position to UI position
            Vector3 start = transform.parent.InverseTransformPoint(from);
            Vector3 finsh = transform.parent.InverseTransformPoint(set.target.position);

            finsh.z = start.z;

            float elapsed = 0;
            while(elapsed <= AnimationTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / AnimationTime);
                t = IllogicGate.Tweener.EaseOut(t, IllogicGate.Tweener.Mode.Sine);
                transform.localPosition = Vector3.Lerp(start, finsh, t);
                transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.5f, t);
                yield return null;
            }

            image.enabled = false;

            Destroy(gameObject, 3);
        }
    }
}