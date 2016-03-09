/**
 * ENUtils.cs
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
using System.Collections.Generic;
using UnityEngine;

namespace Endurance
{
    public class ENUtils
    {
        public static void updatePartHighlighting(Part part)
        {
            //if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight) { return; }//no need for highlighting in prefab construction...
            //part.highlighter.ReinitMaterials();
        }

        public static GameObject createJettisonedObject(GameObject toJettison, Vector3 velocity, Vector3 force, float mass)
        {
            GameObject jettisonedObject = new GameObject("JettisonedDebris");
            Transform parent = toJettison.transform.parent;
            if (parent != null)
            {
                jettisonedObject.transform.position = parent.position;
                jettisonedObject.transform.rotation = parent.rotation;
            }
            toJettison.transform.NestToParent(jettisonedObject.transform);
            jettisonedObject.AddComponent<physicalObject>();
            Rigidbody rb = jettisonedObject.AddComponent<Rigidbody>();
            rb.velocity = velocity;
            rb.mass = mass;            
            rb.AddForce(force);
            rb.useGravity = false;
            return jettisonedObject;
        }

        public static bool isTechUnlocked(String techName)
        {
            if (HighLogic.CurrentGame == null) { return true; }
            else if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX)
            {
                if (ResearchAndDevelopment.Instance == null) { return true; }
                RDTech.State techState = ResearchAndDevelopment.GetTechnologyState(techName);
                return techState == RDTech.State.Available;
            }
            return false;
        }

        public static bool isResearchGame()
        {
            if (HighLogic.CurrentGame == null) { return false; }
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX) { return true; }
            return false;
        }

        //retrieve an array of Components that implement <T>/ extend <T>;
        //<T> may be an interface or class
        public static T[] getComponentsImplementing<T>(GameObject obj) where T : class
        {
            List<T> interfacesList = new List<T>();
            Component[] comps = obj.GetComponents<MonoBehaviour>();
            T t;
            foreach (Component c in comps)
            {
                t = c as T;
                if (t != null)
                {
                    interfacesList.Add(t);
                }
            }
            return interfacesList.ToArray();
        }

        public static T findNext<T>(T[] array, System.Predicate<T> alg, bool iterateBackwards)
        {
            int index = findIndex<T>(array, alg);
            int len = array.Length;
            if (index < 0 || index >= len)
            {
                return default(T);//invalid
            }
            int iter = iterateBackwards ? -1 : 1;
            index += iter;
            if (index < 0) { index = len - 1; }
            if (index >= len) { index = 0; }
            return array[index];
        }

        public static T findNext<T>(List<T> list, System.Predicate<T> alg, bool iterateBackwards)
        {
            int index = findIndex<T>(list, alg);
            int len = list.Count;
            if (index < 0 || index >= len)
            {
                return default(T);//invalid
            }
            int iter = iterateBackwards ? -1 : 1;
            index += iter;
            if (index < 0) { index = len - 1; }
            if (index >= len) { index = 0; }
            return list[index];
        }

        public static T findNextEligible<T>(List<T> list, System.Predicate<T> matchCurrent, System.Predicate<T> matchEligible, bool iterateBackwards)
        {

            int iter = iterateBackwards ? -1 : 1;
            int startIndex = findIndex(list, matchCurrent);
            int length = list.Count;
            int index;
            for (int i = 1; i <= length; i++)//will always loop around to catch the start index...
            {
                index = startIndex + iter * i;
                while (index >= length) { index -= length; }
                while (index < 0) { index += length; }

                if (matchEligible.Invoke(list[index]))
                {
                    return list[index];
                }
            }
            return default(T);
        }

        public static T findNextEligible<T>(T[] list, System.Predicate<T> matchCurrent, System.Predicate<T> matchEligible, bool iterateBackwards)
        {

            int iter = iterateBackwards ? -1 : 1;
            int startIndex = findIndex(list, matchCurrent);
            int length = list.Length;
            int index;
            for (int i = 1; i <= length; i++)//will always loop around to catch the start index...
            {
                index = startIndex + iter * i;
                while (index >= length) { index -= length; }
                while (index < 0) { index += length; }

                if (matchEligible.Invoke(list[index]))
                {
                    return list[index];
                }
            }
            return default(T);
        }

        public static int findIndex<T>(T[] array, System.Predicate<T> alg)
        {            
            return Array.FindIndex<T>(array, alg);
        }

        public static int findIndex<T>(List<T> list, System.Predicate<T> alg)
        {
            return list.FindIndex(alg);
        }

        public static double safeParseDouble(String val)
        {
            double returnVal = 0;
            try
            {
                returnVal = double.Parse(val);
            }
            catch (Exception e)
            {
                MonoBehaviour.print("could not parse double value from: '" + val + "'\n" + e.Message);
            }
            return returnVal;
        }

        internal static bool safeParseBool(string v)
        {
            bool value = false;
            try
            {
                value = Boolean.Parse(v);
            }
            catch (Exception e)
            {
                MonoBehaviour.print("could not parse bool value from : " + v+"\n" + e.Message);
            }
            return value;
        }

        public static float safeParseFloat(String val)
        {
            float returnVal = 0;
            try
            {
                returnVal = float.Parse(val);
            }
            catch (Exception e)
            {
                MonoBehaviour.print("could not parse float value from: '" + val + "'\n" + e.Message);
            }
            return returnVal;
        }

        public static int safeParseInt(String val)
        {
            int returnVal = 0;
            try
            {
                returnVal = int.Parse(val);
            }
            catch (Exception e)
            {
                MonoBehaviour.print("could not parse int value from: '" + val + "'\n" + e.Message);
            }
            return returnVal;
        }

        public static String[] parseCSV(String input)
        {
            return parseCSV(input, ",");
        }

        public static String[] parseCSV(String input, String split)
        {
            String[] vals = input.Split(new String[] { split }, StringSplitOptions.None);
            int len = vals.Length;
            for (int i = 0; i < len; i++)
            {
                vals[i] = vals[i].Trim();
            }
            return vals;
        }

        public static String concatArray(float[] array)
        {
            String val = "";
            if (array != null)
            {
                foreach (float f in array) { val = val + f + ","; }
            }
            return val;
        }

        public static String concatArray(String[] array)
        {
            String val = "";
            if (array != null)
            {
                foreach (String f in array) { val = val + f + ","; }
            }
            return val;
        }

        public static String printList<T>(List<T> list, String separator)
        {
            String str = "";
            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                str = str + list[i].ToString();
                if (i < len - 1) { str = str + separator; }
            }
            return str;
        }

        public static String printArray<T>(T[] array, String separator)
        {
            String str = "";
            if (array != null)
            {
                int len = array.Length;
                for (int i = 0; i < len; i++)
                {
                    str = str + array[i].ToString();
                    if (i < len - 1) { str = str + separator; }
                }
            }
            return str;
        }

        public static void destroyChildren(Transform tr)
        {
            if (tr == null || tr.childCount<=0) { return; }

            foreach (Transform child in tr)
            {
                if (child == null) { continue; }
                MonoBehaviour.print("Destroying game object: " + child);
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void destroyChildrenImmediate(Transform tr)
        {
            if (tr == null || tr.childCount <= 0) { return; }
            
            foreach (Transform child in tr)
            {
                if (child == null)
                {
                    continue;
                }
                MonoBehaviour.print("Destroying game object (IMMEDIATE): " + child);
                GameObject.DestroyImmediate(child.gameObject);
            }
        }

        public static void recursePrintMaterialData(Transform tr)
        {
            if (tr.renderer != null)
            {
                Material m = tr.renderer.material;
                Texture t = m.mainTexture;
                Shader s = m.shader;
                MonoBehaviour.print("mat: " + m + " : tex: " + t + " : shad: " + s);
            }

            foreach (Transform child in tr)
            {
                recursePrintMaterialData(child);
            }
        }

        public static void recursePrintChildTransforms(Transform tr, String prefix)
        {
            MonoBehaviour.print("Transform found: " + prefix + tr.name);
            for (int i = 0; i < tr.childCount; i++)
            {
                recursePrintChildTransforms(tr.GetChild(i), prefix + "  ");
            }
        }

        public static void recursePrintComponents(GameObject go, String prefix)
        {
            MonoBehaviour.print("Found gameObject: " + prefix + go.name+" enabled: "+go.activeSelf+ " :: " +go.activeInHierarchy);
            int childCount = go.transform.childCount;
            Component[] comps = go.GetComponents<Component>();
            foreach (Component comp in comps)
            {
                MonoBehaviour.print("Found Component : " + prefix + "* " + comp.GetType());
            }

            for (int i = 0; i < childCount; i++)
            {
                recursePrintComponents(go.transform.GetChild(i).gameObject, prefix + "  ");
            }
        }

        public static void enableMeshColliderRecursive(Transform tr, bool enabled, bool convex)
        {
            MeshCollider mc = tr.GetComponent<MeshCollider>();
            if (mc != null)
            {
                mc.enabled = enabled;
                mc.convex = convex;
            }
            int len = tr.childCount;
            for (int i = 0; i < len; i++)
            {
                enableMeshColliderRecursive(tr.GetChild(i), enabled, convex);
            }
        }

        public static void addMeshCollidersRecursive(Transform tr, bool enabled, bool convex)
        {
            MeshCollider mc = tr.GetComponent<MeshCollider>();
            if (mc == null)
            {
                MeshFilter mf = tr.GetComponent<MeshFilter>();
                if (mf != null && mf.mesh != null)
                {
                    mc = tr.gameObject.AddComponent<MeshCollider>();
                }
            }
            if (mc != null)
            {
                mc.enabled = enabled;
                mc.convex = convex;
            }
            int len = tr.childCount;
            for (int i = 0; i < len; i++)
            {
                addMeshCollidersRecursive(tr.GetChild(i), enabled, convex);
            }
        }        

        public static void enableRenderRecursive(Transform tr, bool val)
        {
            if (tr.renderer != null)
            {
                tr.renderer.enabled = val;
            }
            for (int i = 0; i < tr.childCount; i++)
            {
                enableRenderRecursive(tr.GetChild(i), val);
            }
        }

        public static void enableColliderRecursive(Transform tr, bool val)
        {
            foreach (Collider collider in tr.gameObject.GetComponents<Collider>())
            {
                collider.enabled = val;
            }
            for (int i = 0; i < tr.childCount; i++)
            {
                enableColliderRecursive(tr.GetChild(i), val);
            }
        }

        public static Texture findTexture(String textureName, bool normal)
        {
            return GameDatabase.Instance.GetTexture(textureName, normal);
        }

        public static float distanceFromLine(Ray ray, Vector3 point)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        public static Material loadMaterial(String diffuse, String normal)
        {
            return loadMaterial(diffuse, normal, string.Empty, "KSP/Bumped Specular");
        }

        public static Material loadMaterial(String diffuse, String normal, String shader)
        {
            return loadMaterial(diffuse, normal, String.Empty, shader);
        }

        public static Material loadMaterial(String diffuse, String normal, String emissive, String shader)
        {
            Material material;
            Texture diffuseTexture = ENUtils.findTexture(diffuse, false);
            Texture normalTexture = String.IsNullOrEmpty(normal) ? null : ENUtils.findTexture(normal, true);
            Texture emissiveTexture = String.IsNullOrEmpty(emissive) ? null : ENUtils.findTexture(emissive, false);
            material = new Material(Shader.Find(shader));
            material.SetTexture("_MainTex", diffuseTexture);
            if (normalTexture != null)
            {
                material.SetTexture("_BumpMap", normalTexture);
            }
            if (emissiveTexture != null)
            {
                material.SetTexture("_Emissive", emissiveTexture);
            }
            return material;
        }

        public static void setMainTextureRecursive(Transform tr, Texture tex)
        {
            setTextureRecursive(tr, tex, "_MainTex");
        }

        public static void setTextureRecursive(Transform tr, Texture tex, String texID)
        {
            int id = Shader.PropertyToID(texID);
            setTextureRecursive(tr, tex, id);
        }

        public static void setTextureRecursive(Transform tr, Texture tex, int id)
        {
            if (tr != null && tr.renderer != null && tr.renderer.material != null)
            {
                tr.renderer.material.SetTexture(id, tex);
            }
            foreach (Transform tr1 in tr)
            {
                setTextureRecursive(tr1, tex, id);
            }
        }

        public static void setMaterialRecursive(Transform tr, Material mat)
        {
            if (tr.gameObject.renderer != null) { tr.gameObject.renderer.material = mat; }
            int len = tr.childCount;
            for (int i = 0; i < len; i++)
            {
                setMaterialRecursive(tr.GetChild(i), mat);
            }
        }

        public static void setOpacityRecursive(Transform tr, float opacity)
        {
            if (tr.renderer != null && tr.renderer.material != null)
            {
                tr.renderer.material.SetFloat("_Opacity", opacity);
                tr.renderer.material.renderQueue = opacity >= 1f ? 2000 : 3000;
            }
            foreach (Transform child in tr) { setOpacityRecursive(child, opacity); }
        }

        public static Bounds getRendererBoundsRecursive(GameObject gameObject)
        {
            Renderer[] childRenders = gameObject.GetComponentsInChildren<Renderer>(false);
            Renderer parentRender = gameObject.GetComponent<Renderer>();

            Bounds combinedBounds = default(Bounds);

            bool initializedBounds = false;

            if (parentRender != null && parentRender.enabled)
            {
                combinedBounds = parentRender.bounds;
                initializedBounds = true;
            }
            int len = childRenders.Length;
            for (int i = 0; i < len; i++)
            {
                if (initializedBounds)
                {
                    combinedBounds.Encapsulate(childRenders[i].bounds);
                }
                else
                {
                    combinedBounds = childRenders[i].bounds;
                    initializedBounds = true;
                }
            }
            return combinedBounds;
        }

        public static void findShieldedPartsCylinder(Part basePart, Bounds fairingRenderBounds, List<Part> shieldedParts, float topY, float bottomY, float topRadius, float bottomRadius)
        {
            float height = topY - bottomY;
            float largestRadius = topRadius > bottomRadius ? topRadius : bottomRadius;

            Vector3 lookupCenterLocal = new Vector3(0, bottomY + (height * 0.5f), 0);
            Vector3 lookupTopLocal = new Vector3(0, topY, 0);
            Vector3 lookupBottomLocal = new Vector3(0, bottomY, 0);
            Vector3 lookupCenterGlobal = basePart.transform.TransformPoint(lookupCenterLocal);

            Ray lookupRay = new Ray(lookupBottomLocal, new Vector3(0, 1, 0));

            List<Part> partsFound = new List<Part>();
            Collider[] foundColliders = Physics.OverlapSphere(lookupCenterGlobal, height * 1.5f, 1);
            foreach (Collider col in foundColliders)
            {
                Part pt = col.gameObject.GetComponentUpwards<Part>();
                if (pt != null && pt != basePart && pt.vessel == basePart.vessel && !partsFound.Contains(pt))
                {
                    partsFound.Add(pt);
                }
            }

            Bounds[] otherPartBounds;
            Vector3 otherPartCenterLocal;

            float partYPos;
            float partYPercent;
            float partYRadius;
            float radiusOffset = topRadius - bottomRadius;

            foreach (Part pt in partsFound)
            {
                //check basic render bounds for containment

                //TODO this check misses the case where the fairing is long/tall, containing a wide part; it will report that the wide part can fit inside
                //of the fairing, due to the relative size of their colliders
                otherPartBounds = pt.GetRendererBounds();
                if (PartGeometryUtil.MergeBounds(otherPartBounds, pt.transform).size.sqrMagnitude > fairingRenderBounds.size.sqrMagnitude)
                {
                    continue;
                }

                Vector3 otherPartCenter = pt.partTransform.TransformPoint(PartGeometryUtil.FindBoundsCentroid(otherPartBounds, pt.transform));
                if (!fairingRenderBounds.Contains(otherPartCenter))
                {
                    continue;
                }

                //check part bounds center point against conic projection of the fairing
                otherPartCenterLocal = basePart.transform.InverseTransformPoint(otherPartCenter);

                //check vs top and bottom of the shielded area                
                if (otherPartCenterLocal.y > lookupTopLocal.y || otherPartCenterLocal.y < lookupBottomLocal.y)
                {
                    continue;
                }

                //quick check vs cylinder radius
                float distFromLine = ENUtils.distanceFromLine(lookupRay, otherPartCenterLocal);
                if (distFromLine > largestRadius)
                {
                    continue;
                }

                //more precise check vs radius of the cone at that Y position
                partYPos = otherPartCenterLocal.y - lookupBottomLocal.y;
                partYPercent = partYPos / height;
                partYRadius = partYPercent * radiusOffset;
                if (distFromLine > (partYRadius + bottomRadius))
                {
                    continue;
                }
                shieldedParts.Add(pt);
            }
        }


        public static void removeTransforms(Part part, String[] transformNames)
        {
            Transform[] trs;
            foreach (String name in transformNames)
            {
                trs = part.transform.FindChildren(name.Trim());
                foreach (Transform tr in trs)
                {
                    GameObject.Destroy(tr.gameObject);
                }
            }
        }

        public static GameObject cloneModel(String modelURL)
        {
            GameObject clonedModel = null;
            GameObject prefabModel = GameDatabase.Instance.GetModelPrefab(modelURL);
            if (prefabModel != null)
            {
                clonedModel = (GameObject)GameObject.Instantiate(prefabModel);
                clonedModel.name = modelURL;
                clonedModel.transform.name = modelURL;
                clonedModel.SetActive(true); 
            }
            else
            {
                MonoBehaviour.print("Could not clone model by name: " + modelURL);
            }
            return clonedModel;
        }

        public static float calcTerminalVelocity(float kilograms, float rho, float cD, float area)
        {
           return Mathf.Sqrt((2f * kilograms * 9.81f) / (rho * area * cD));
        }

        public static float calcDragKN(float rho, float cD, float velocity, float area)
        {
            return calcDynamicPressure(rho, velocity) * area * cD * 0.001f;
        }

        public static float calcDynamicPressure(float rho, float velocity)
        {
            return 0.5f * rho * velocity * velocity;
        }
        
        public static double toRadians(double val)
        {
            return (Math.PI / 180d) * val;
        }

        public static double toDegrees(double val)
        {
            return val * (180d / Math.PI);
        }

    }
}

