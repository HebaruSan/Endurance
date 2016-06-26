/**
 * ENExtensions.cs
 *
 * Endurance part modules.
 * (C) Copyright 2016, Jamie Leighton
 * The original code is courtesy of SSTU mod by shadowmage who gave permission to reuse his code.
 * As such this code continues to be covered by the same license: GNU GPL license.
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
*/
using System;
using UnityEngine;
using System.Collections.Generic;
namespace Endurance
{
    public static class ENExtensions
    {
        #region ConfigNode extension methods

        public static String[] GetStringValues(this ConfigNode node, String name)
        {
            String[] values = node.GetValues(name);
            return values == null ? new String[0] : values;
        }

        public static string GetStringValue(this ConfigNode node, String name, String defaultValue)
        {
            String value = node.GetValue(name);
            return value == null ? defaultValue : value;
        }

        public static string GetStringValue(this ConfigNode node, String name)
        {
            return GetStringValue(node, name, "");
        }

        public static bool[] GetBoolValues(this ConfigNode node, String name)
        {
            String[] values = node.GetValues(name);
            int len = values.Length;
            bool[] vals = new bool[len];            
            for (int i = 0; i < len; i++)
            {
                vals[i] = ENUtils.safeParseBool(values[i]);
            }
            return vals;
        }

        public static bool GetBoolValue(this ConfigNode node, String name, bool defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return bool.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static bool GetBoolValue(this ConfigNode node, String name)
        {
            return GetBoolValue(node, name, false);
        }

        public static float[] GetFloatValues(this ConfigNode node, String name)
        {
            String baseVal = node.GetStringValue(name);
            if (!String.IsNullOrEmpty(baseVal))
            {                
                String[] split = baseVal.Split(new char[] { ',' });
                float[] vals = new float[split.Length];
                for (int i = 0; i < split.Length; i++) { vals[i] = ENUtils.safeParseFloat(split[i]); }
                return vals;
            }
            return new float[0];
        }

        public static float GetFloatValue(this ConfigNode node, String name, float defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return float.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static float GetFloatValue(this ConfigNode node, String name)
        {
            return GetFloatValue(node, name, 0);
        }

        public static double GetDoubleValue(this ConfigNode node, String name, double defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return double.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static double GetDoubleValue(this ConfigNode node, String name)
        {
            return GetDoubleValue(node, name, 0);
        }

        public static int GetIntValue(this ConfigNode node, String name, int defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return int.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static int GetIntValue(this ConfigNode node, String name)
        {
            return GetIntValue(node, name, 0);
        }
        
        public static Vector3 GetVector3(this ConfigNode node, String name, Vector3 defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null)
            {
                return defaultValue;
            }
            String[] vals = value.Split(',');
            if (vals.Length < 3)
            {
                MonoBehaviour.print("ERROR parsing values for Vector3 from input: " + value + ". found less than 3 values, cannot create Vector3");
                return defaultValue;
            }
            return new Vector3((float)ENUtils.safeParseDouble(vals[0]), (float)ENUtils.safeParseDouble(vals[1]), (float)ENUtils.safeParseDouble(vals[2]));
        }

        public static Vector3 GetVector3(this ConfigNode node, String name)
        {
            String value = node.GetValue(name);
            if (value == null)
            {
                MonoBehaviour.print("No value for name: " + name + " found in config node: " + node);
                return Vector3.zero;
            }
            String[] vals = value.Split(',');
            if (vals.Length < 3)
            {
                MonoBehaviour.print("ERROR parsing values for Vector3 from input: " + value + ". found less than 3 values, cannot create Vector3");
                return Vector3.zero;
            }
            return new Vector3((float)ENUtils.safeParseDouble(vals[0]), (float)ENUtils.safeParseDouble(vals[1]), (float)ENUtils.safeParseDouble(vals[2]));
        }

        #endregion

        #region Transform extensionMethods

        public static Transform[] FindModels(this Transform transform, String modelName)
        {
            Transform[] trs = transform.FindChildren(modelName);
            Transform[] trs2 = transform.FindChildren(modelName + "(Clone)");
            Transform[] trs3 = new Transform[trs.Length + trs2.Length];
            int index = 0;
            for (int i = 0; i < trs.Length; i++, index++)
            {
                trs3[index] = trs[i];
            }
            for (int i = 0; i < trs2.Length; i++, index++)
            {
                trs3[index] = trs2[i];
            }
            return trs3;
        }

        public static Transform FindModel(this Transform transform, String modelName)
        {
            Transform tr = transform.FindRecursive(modelName);
            if (tr != null) { return tr; }
            return transform.FindRecursive(modelName + "(Clone)");
        }

        public static Transform[] FindChildren(this Transform transform, String name)
        {
            List<Transform> trs = new List<Transform>();
            if (transform.name == name) { trs.Add(transform); }
            locateTransformsRecursive(transform, name, trs);
            return trs.ToArray();
        }

        private static void locateTransformsRecursive(Transform tr, String name, List<Transform> output)
        {
            foreach (Transform child in tr)
            {
                if (child.name == name) { output.Add(child); }
                locateTransformsRecursive(child, name, output);
            }
        }

        public static Transform FindRecursive(this Transform transform, String name)
        {
            if (transform.name == name) { return transform; }//was the original input transform
            Transform tr = transform.Find(name);//found as a direct child
            if (tr != null) { return tr; }
            foreach(Transform child in transform)
            {
                tr = child.FindRecursive(name);
                if (tr != null) { return tr; }
            }
            return null;
        }

        public static Transform FindOrCreate(this Transform transform, String name)
        {
            Transform newTr = transform.FindRecursive(name);
            if (newTr != null)
            {
                //MonoBehaviour.print("found existing go for: " + name);
                return newTr;
            }
            //MonoBehaviour.print("creating new game object for: " + name);
            //ENUtils.recursePrintComponents(transform.gameObject, "");
            GameObject newGO = new GameObject(name);
            newGO.SetActive(true);
            newGO.name = newGO.transform.name = name;
            newGO.transform.NestToParent(transform);
            return newGO.transform;
        }

        #endregion

        #region Vector3 extensionMethods

        public static Vector3 CopyVector(this Vector3 input)
        {
            return new Vector3(input.x, input.y, input.z);
        }

        #endregion

    }
}

