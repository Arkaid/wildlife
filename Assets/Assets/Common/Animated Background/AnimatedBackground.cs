using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Image))]
    public class AnimatedBackground : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Vector2 speed;

        // --- Properties -------------------------------------------------------------------------------
        Material material;
        Vector2 offset = Vector2.zero;
        Image image;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            image = GetComponent<Image>();
            material = image.material;
        }
        // -----------------------------------------------------------------------------------	
        private void Update()
        {
            Vector2 size = image.rectTransform.rect.size;
            float scale_w = size.x / material.mainTexture.width;
            float scale_h = size.y / material.mainTexture.height;
            material.mainTextureScale = new Vector2(scale_w, scale_h);

            offset += speed * Time.deltaTime;
            material.mainTextureOffset = offset;
        }
    }
}