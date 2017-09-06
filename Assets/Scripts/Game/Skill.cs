using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Skill : IllogicGate.SingletonBehaviour<Skill>
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Called when the skill is triggered </summary>
        public event System.Action skillTriggered;
        
        /// <summary> Called when the skill is released </summary>
        public event System.Action skillReleased;

        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Available skill moves</summary>
        public enum Type
        {
            Shield,
            Speed,
            Freeze,
            COUNT
        }

        // --- Static Properties ------------------------------------------------------------------------
        /// <summary> shorthand </summary>
        Type type { get { return Config.instance.skill; } }

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Skill remaining time </summary>
        public float remainingTime { get; private set; }

        /// <summary> Maximum available time. Can't recharge past this </summary>
        public float maxTime { get; private set; }

        /// <summary> true if the skill is being used </summary>
        public bool isActive { get; private set; }

        /// <summary> Speed skill multiplier. 1 if inactive or skill is not Speed </summary>
        public float speedMultiplier { get; private set; }

        /// <summary> True if skill is shield and active </summary>
        public bool isShieldActive { get { return isActive && type == Type.Shield;  } }

        /// <summary> True if skill is freeze and active </summary>
        public bool isFreezeActive { get { return isActive && type == Type.Freeze; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnDisable()
        {
            Deactivate();
        }

        // -----------------------------------------------------------------------------------	
        void Update()
        {
            if (Controller.instance.isPaused)
                return;
                
            speedMultiplier = type == Type.Speed && isActive ?
                Config.instance.speedSkillMultiplier : 1;

            // can we activate the skill?
            if (!isActive && remainingTime > 0 && Input.GetButton("Skill"))
            {
                string clip = "game_skill_" + type.ToString().ToLower();
                SoundManager.instance.PlaySFX(clip);
                isActive = true;
                if (skillTriggered != null)
                    skillTriggered();
            }

            // do we have to deactivate the skill
            else if (isActive && (remainingTime <= 0 || !Input.GetButton("Skill")))
                Deactivate();

            if (isActive)
            {
                remainingTime -= Time.deltaTime;
                UI.instance.skillBar.remainingTime = remainingTime;
            }
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Initialize()
        {
            maxTime = Config.instance.GetSkillTime(type);
            remainingTime = maxTime;
            speedMultiplier = 1;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary> Resets the skill to its inactive state </summary>
        public void Deactivate()
        {
            if (SoundManager.instance != null)
            {
                string clip = "game_skill_" + type.ToString().ToLower();
                SoundManager.instance.StopSFX(clip);
            }

            isActive = false;
            if (skillReleased != null)
                skillReleased();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Recharges a bit of the skill, based on percentage cleared
        /// </summary>
        public void Recharge(float percentage)
        {
            remainingTime += Config.instance.CalculateSkillCharge(type, percentage);
            remainingTime = Mathf.Min(remainingTime, maxTime);
            UI.instance.skillBar.remainingTime = remainingTime;
        }
    }
}