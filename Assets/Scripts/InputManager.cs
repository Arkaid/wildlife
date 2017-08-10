using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Simple input manager wrapper class to map inputs to abstract names,
    /// since Unity's InputManager is inaccessible
    /// NOTE: this breaks UI navigation D:
    /// </summary>
    public static class InputManager
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary>
        /// Joysticks axis. These must be mapped in Unity's InputManager
        /// </summary>
        public enum JoystickAxis
        {
            JoystickAxis0,
            JoystickAxis1,
            JoystickAxis2,
            JoystickAxis3,
            JoystickAxis4,
            JoystickAxis5,
            JoystickAxis6,
            JoystickAxis7,
            JoystickAxis8,
            JoystickAxis9,
        }

        // --- Static Properties ------------------------------------------------------------------------
        /// <summary> Maps names to buttons / keys </summary>
        static Dictionary<string, List<string>> buttonMapping = new Dictionary<string, List<string>>();

        /// <summary> Maps names to joystick axis (up to 10) </summary>
        static Dictionary<string, List<string>> axisMapping = new Dictionary<string, List<string>>();

        /// <summary> Maps names to buttons (positive / negative) </summary>
        static Dictionary<string, List<string[]>> axisButtonMapping = new Dictionary<string, List<string[]>>();

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Map an action to a button input
        /// </summary>
        public static void AddButtonMapping(string action, string input)
        {
            if (!buttonMapping.ContainsKey(action))
                buttonMapping[action] = new List<string>();
            buttonMapping[action].Add(input);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Replace an action's button mapping
        /// </summary>
        public static void ReplaceButtonMapping(string action, string oldInput, string newInput)
        {
            int idx = buttonMapping[action].IndexOf(oldInput);
            buttonMapping[action][idx] = newInput;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Remove an actions button mapping
        /// </summary>
        public static void RemoveButtonMapping(string action, string input)
        {
            buttonMapping[action].Remove(input);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Add a joystick axis mapping
        /// </summary>
        public static void AddAxisMapping(string action, JoystickAxis input)
        {
            if (!axisMapping.ContainsKey(action))
                axisMapping[action] = new List<string>();
            
            // NOTE: you must set the axis names in InputManager to match the enum
            axisMapping[action].Add(input.ToString());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Add a positive/negative button axis mapping
        /// </summary>
        public static void AddAxisMapping(string action, string positive, string negative)
        {
            if (!axisButtonMapping.ContainsKey(action))
                axisButtonMapping[action] = new List<string[]>();
            axisButtonMapping[action].Add(new string[] { positive, negative });
        }

        // -----------------------------------------------------------------------------------	
        public static float GetAxisRaw(string action)
        {
            float result = 0;
            // do we have a joystick axis mapping to that action?
            if (axisMapping.ContainsKey(action))
            {
                // get the first non-zero axis
                List<string> mappings = axisMapping[action];
                for (int i = 0; i < mappings.Count && Mathf.Approximately(result, 0); i++)
                    result = Input.GetAxisRaw(mappings[i]);
            }

            // if we haven't found a joystick mapping, try keys
            if (result == 0 && axisButtonMapping.ContainsKey(action))
            {
                // find the first non-zero mapping
                List<string[]> mappings = axisButtonMapping[action];
                for (int i = 0; i < mappings.Count && Mathf.Approximately(result, 0); i++)
                {
                    if (Input.GetKey(mappings[i][0]))
                        result++;
                    if (Input.GetKey(mappings[i][1]))
                        result--;
                }
            }

            return result;
        }

        // -----------------------------------------------------------------------------------	
        public static bool GetButton(string action)
        {
            bool pressed = false;
            List<string> mappings = buttonMapping[action];

            for (int i = 0; i < mappings.Count && !pressed; i++)
                pressed = Input.GetKey(mappings[i]);

            return pressed;
        }

        // -----------------------------------------------------------------------------------	
        public static bool GetButtonUp(string action)
        {
            bool pressed = false;
            List<string> mappings = buttonMapping[action];

            for (int i = 0; i < mappings.Count && !pressed; i++)
                pressed = Input.GetKeyUp(mappings[i]);

            return pressed;
        }

        // -----------------------------------------------------------------------------------	
        public static bool GetButtonDown(string action)
        {
            bool pressed = false;
            List<string> mappings = buttonMapping[action];

            for (int i = 0; i < mappings.Count && !pressed; i++)
                pressed = Input.GetKeyDown(mappings[i]);

            return pressed;
        }
        
        // -----------------------------------------------------------------------------------	
        public static JSONObject Serialize()
        {
            JSONObject json = new JSONObject();

            // button mapping
            JSONObject map = new JSONObject();
            foreach(KeyValuePair<string, List<string>> kvp in buttonMapping)
            {
                JSONObject list = new JSONObject();
                foreach (string btn in kvp.Value)
                    list.Add(btn);
                map.AddField(kvp.Key, list);
            }
            json.AddField("button_mapping", map);

            // axis mapping
            map = new JSONObject();
            foreach (KeyValuePair<string, List<string>> kvp in axisMapping)
            {
                JSONObject list = new JSONObject();
                foreach (string btn in kvp.Value)
                    list.Add(btn);
                map.AddField(kvp.Key, list);
            }
            json.AddField("axis_mapping", map);

            // axis/button mapping
            map = new JSONObject();
            foreach (KeyValuePair<string, List<string[]>> kvp in axisButtonMapping)
            {
                JSONObject list = new JSONObject();
                foreach (string[] btn in kvp.Value)
                {
                    string str = string.Format("[\"{0}\",\"{1}\"]", btn[0], btn[1]);
                    list.Add(new JSONObject(str));
                }
                map.AddField(kvp.Key, list);
            }
            json.AddField("axis_button_mapping", map);

            return json;
        }

        // -----------------------------------------------------------------------------------	
        public static void Deserialize(JSONObject json)
        {
            // button mapping
            buttonMapping = new Dictionary<string, List<string>>();
            JSONObject map = json["button_mapping"];
            foreach (string action in map.keys)
            {
                List<string> buttons = new List<string>();
                foreach (JSONObject item in map[action].list)
                    buttons.Add(item.str);
                buttonMapping.Add(action, buttons);
            }

            // axis mapping
            axisMapping = new Dictionary<string, List<string>>();
            map = json["axis_mapping"];
            foreach (string action in map.keys)
            {
                List<string> axis = new List<string>();
                foreach (JSONObject item in map[action].list)
                    axis.Add(item.str);
                axisMapping.Add(action, axis);
            }

            // axis-button mapping
            axisButtonMapping = new Dictionary<string, List<string[]>>();
            map = json["axis_button_mapping"];
            foreach (string action in map.keys)
            {
                List<string[]> buttons = new List<string[]>();
                foreach (JSONObject item in map[action].list)
                    buttons.Add(new string[] { item.list[0].str, item.list[1].str });
                axisButtonMapping.Add(action, buttons);
            }
        }

        // --- Properties -------------------------------------------------------------------------------

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}