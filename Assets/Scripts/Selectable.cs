using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// A simplification I'm using to coordinate controller and mouse input
    /// </summary>
    public class Selectable : UnityEngine.UI.Selectable, ISubmitHandler
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Called when you hover the mouse in, or move into the Selectable with the controller </summary>
        public event System.Action<Selectable> hoverIn;

        /// <summary> Called when you hover the mouse out, or move out of the Selectable with the controller </summary>
        public event System.Action<Selectable> hoverOut;

        /// <summary> Called when you actually select by pressing fire or clicking </summary>
        public event System.Action<Selectable> select;

        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public bool isHovering { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
       
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void OnSubmit(BaseEventData eventData)
        {
            if (isHovering && select != null)
                select(this);
        }

        // -----------------------------------------------------------------------------------	
        public override void OnPointerEnter(PointerEventData eventData)
        {
            Select();
        }

        // -----------------------------------------------------------------------------------	
        public override void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            if (hoverOut != null)
                hoverOut(this);
        }

        // -----------------------------------------------------------------------------------	
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (select != null)
                select(this);
        }

        // -----------------------------------------------------------------------------------	
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            isHovering = true;
            if (hoverIn != null)
                hoverIn(this);
        }

        // -----------------------------------------------------------------------------------	
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            isHovering = false;
            if (hoverOut != null)
                hoverOut(this);
        }


    }
}