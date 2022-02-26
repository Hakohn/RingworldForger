//#define BOLT_EXISTS
#if BOLT_EXISTS
using Bolt;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChironPE
{
    #region Enums
    public enum RandomizationMethod
    {
        /// <summary> The default randomization method. Completely random. </summary>
        Default,
        /// <summary> All the elements will be selected randomly with no repetitions until they've run out. </summary>
        RoundRobin
    };
    #endregion

    #region Extensions
    public static class StringExtensions
    {
        /// <summary> Convert the current string into how it is should be shown by Unity's GUI. </summary>
        public static string ToGUIName(this string variableName)
        {
            string ans = $"{char.ToUpper(variableName[0])}";
            for (int i = 1; i < variableName.Length; i++)
            {
                if (variableName[i] == '_')
                {
                    continue;
                }
                if (char.IsUpper(variableName[i]) && (i + 1 >= variableName.Length || !char.IsUpper(variableName[i + 1])))
                {
                    ans += " ";
                }
                ans += variableName[i];
            }
            return ans;
        }

        /// <summary> Turns the given sentence into a variable name, in the camel style. </summary>
        public static string ToVariableName(this string sentence)
        {
            string res = $"{char.ToLower(sentence[0])}";
            for (int i = 1; i < sentence.Length; i++)
            {
                if (char.IsLetterOrDigit(sentence[i]))
                {
                    res += char.ToLower(sentence[i]);
                }
                else if (sentence[i] == ' ' || sentence[i] == '_')
                {
                    if (i + 1 < sentence.Length && char.IsLetterOrDigit(sentence[i + 1]))
                    {
                        i++;
                        res += char.ToUpper(sentence[i]);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Assuming that the given string is a path, the file name from the end of the path will be extracted and returned as a string.
        /// </summary>
        public static string ExtractFileNameFromPath(this string path, bool keepExtension = false)
        {
            int startIndex = -1, endIndex = -1;
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '.' && endIndex == -1)
                {
                    endIndex = i;
                }
                else if (path[i] == '/' && startIndex == -1)
                {
                    startIndex = i + 1;
                    if (!keepExtension)
                        return path.Substring(startIndex, endIndex - startIndex);
                    else
                        return path.Substring(startIndex);
                }
            }
            return path;
        }
    }
    
    public static class NumberExtensions
    {
        /// <summary> Remaps the current value from the first interval to the second interval. </summary>
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        /// <summary> Check if the value is found between the two given values. [min, max) </summary>
        public static bool InRange<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(value) <= 0 && value.CompareTo(max) < 0)
                return true;
            return false;
        }
        /// <summary> Check if the value is found between the two given values. [min, max) </summary>
        public static bool InRange<T>(this T value, (T, T) range) where T : IComparable<T>
        {
            if (range.Item1.CompareTo(value) <= 0 && value.CompareTo(range.Item2) < 0)
                return true;
            return false;
        }

        /// <summary> Check if the value is found between the two given values. This includes the maximum in the range. [min, max] </summary>
        public static bool InRange2<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(value) <= 0 && value.CompareTo(max) <= 0)
                return true;
            return false;
        }
        /// <summary> Check if the value is found between the two given values. This includes the maximum in the range. [min, max] </summary>
        public static bool InRange2<T>(this T value, (T, T) range) where T : IComparable<T>
        {
            if (range.Item1.CompareTo(value) <= 0 && value.CompareTo(range.Item2) <= 0)
                return true;
            return false;
        }

        /// <summary> Get the fractional part of the number. [0f, 1f) </summary>
		public static float Fractional(this float value)
        {
            return Mathf.Abs(value - Mathf.FloorToInt(value));
        }

        /// <summary>
        /// Rounds the given value to the closest multiple of the given m.
        /// </summary>
        /// <param name="value"> The value that we want to be rounded. </param>
        /// <param name="m"> The number that we will want to find a multiple of. </param>
        /// <returns> The rounded (either up or down; the closest of the two) value. </returns>
        public static int RoundToMultiple(this float value, int m)
        {
            return RoundToMultiple((int)value, m);
        }
        public static int RoundToMultiple(this int value, int m)
        {
            int roundDown = value / m * m;
            int roundUp = value / m * m + m;

            if (Mathf.Abs(value - roundDown) < Mathf.Abs(value - roundUp))
                return roundDown;
            return roundUp;
        }
    }
    
    public static class VectorExtensions
    {
        /// <summary> Remaps the current value on X, Y, Z from the first interval to the second interval. </summary>
        public static Vector3 Remap(this Vector3 value, Vector3 from1, Vector3 to1, Vector3 from2, Vector3 to2)
        {
            value.x = value.x.Remap(from1.x, to1.x, from2.x, to2.x);
            value.y = value.y.Remap(from1.y, to1.y, from2.y, to2.y);
            value.z = value.z.Remap(from1.z, to1.z, from2.z, to2.z);

            return value;
        }

        /// <summary> Convert from Vector2 to a horizontal Vector3 (x to x, and y to z). </summary>
        public static Vector3 ToHorizontalV3(this Vector2 vect)
        {
            return new Vector3(vect.x, 0, vect.y);
        }

        /// <summary> Convert from Vector3 to Vector2 (and thus, the z value will be removed) </summary>
        public static Vector2 ToVector2(this Vector3 vect)
        {
            return new Vector2(vect.x, vect.y);
        }

        /// <summary> Flip the Vector2, by turning the x into y and y into x. </summary>
        public static Vector2 Flipped(this Vector2 vect)
        {
            return new Vector2(vect.y, vect.x);
        }

        /// <summary> Get only the horizontal part of the vector (only the XZ, with a Y value of 0). </summary>
        public static Vector3 Horizontal(this Vector3 vect)
        {
            return new Vector3(vect.x, 0, vect.z);
        }
        /// <summary> Get only the vertical part of the vector (only the Y, with a X and Z value of 0). </summary>
        public static Vector3 Vertical(this Vector3 vect)
        {
            return new Vector3(0, vect.y, 0);
        }

        /// <summary> Returns the direction opposite to the given vector. </summary>
        public static Vector3 OppositeDirection(this Vector3 vect)
        {
            return -vect.normalized;
        }
    }
    
    public static class PhysicsExtensions
    {
        /// <summary> 
        /// Enables or disables the Rigidbody. If it is set to disable,
        /// the rigidbody will be set to kinematic and no longer detect collisions. 
        /// Otherwise, in case it is set to enabled, it will no longer be kinematic and
        /// will be able to detect collisions again. 
        /// </summary>
        public static void SetEnabled(this Rigidbody rb, bool enabled)
        {
            if (enabled)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
            }
            else
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }
    }
    
    public static class ColorExtensions
    {
        /// <summary> Returns the given color, but with different alpha, given through argument. </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary> Converts from an ordinary color to an HDR color, with a given intensity. Designed for HDRP / URP materials. </summary>
        public static Color ToHDRColor(this Color color, float intensity)
        {
            return color * Mathf.Pow(2, intensity);
        }

        /// <summary> Get the intensity of the HDR color. Designed for HDRP / URP materials. </summary>
        public static float HDRIntensity(this Color color)
        {
            float maxVal = Mathf.Max(color.linear.r, color.linear.g, color.linear.b);
            float intensityPow2 = maxVal / 255f;
            if (intensityPow2 > 1)
                return Mathf.Log(intensityPow2, 2);
            else return 0;
        }

        public static Color WithIntensity(this Color color, float intensity)
        {
            float currentIntensity = color.HDRIntensity();
            Color nonHDRColor = color / Mathf.Pow(2, currentIntensity);

            return ToHDRColor(nonHDRColor, intensity);
        }
    }
    
    public static class ContainerExtensions
    {
        /// <summary> Get a random element from the collection. It will throw an error in case the collection is empty or null. </summary>
        public static T RandomElement<T>(this IList<T> collection)
        {
            return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count));
        }

        /// <summary> Get a shuffled array of the indexes of the given collection. </summary>
        public static int[] ShuffledIndexes<T>(IList<T> collection)
        {
            // generating a random array
            System.Random rnd = new System.Random();
            int[] res =
                Enumerable.Range(0, collection.Count)
                .OrderBy(c => rnd.Next()).ToArray();

            return res;
        }

        /// <summary> 
        /// Get the index position within the array. Not to be confused with Array.FindIndex.
        /// Long story short: returns the position ("space") in which the element fits into the array.
        /// </summary>
        public static int PositionInRangeArray<T>(this T value, IList<T> collection) where T : IComparable<T>
        {
            int index = collection.Count;
            for (int i = 0; i < collection.Count; i++)
            {
                if (value.CompareTo(collection[i]) < 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary> Get a shuffled array of the indexes of the given array. </summary>
        public static int[] ShuffledIndexes<T>(this T[] arr)
        {
            // generating a random array
            System.Random rnd = new System.Random();
            int[] res =
                Enumerable.Range(0, arr.Length)
                .OrderBy(c => rnd.Next()).ToArray();

            return res;
        }
        /// <summary> Get a shuffled array of the indexes of the given list. </summary>
        public static int[] ShuffledIndexes<T>(List<T> list)
        {
            // generating a random array
            System.Random rnd = new System.Random();
            int[] res =
                Enumerable.Range(0, list.Count)
                .OrderBy(c => rnd.Next()).ToArray();

            return res;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> List)
        {
            return (List == null || List.Count < 1);
        }

        public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> Dictionary)
        {
            return (Dictionary == null || Dictionary.Count < 1);
        }

        public static int RotativeIncrement(this int index, int containerLength)
        {
            index++;
            if (index >= containerLength) return 0;
            return index;
        }
        public static int RotativeDecrement(this int index, int containerLength)
        {
            index--;
            if (index < 0) return containerLength - 1;
            return index;
        }
    }
    
    public static class BitmaskExtensions
    {
        /// <summary> Checks if the bitmask includes the given flag. </summary>
        public static bool IncludesFlag<T>(this T bitmask, T flag) where T : Enum
        {
            return IncludesFlag((int)(object)bitmask,
                                (int)(object)flag);
        }
        public static bool IncludesFlag(this int bitmask, int flag)
        {
            return (bitmask & flag) != 0;
        }

        #region Layer Bitmasks
        public static bool IsInLayerMask(this int layer, LayerMask layerMask)
        {
            return (layerMask == (layerMask | (1 << layer)));
        }
        public static bool IncludesLayer(this LayerMask layerMask, int layer)
        {
            return layer.IsInLayerMask(layerMask);
        }
        #endregion

        /// <summary>
        /// Sets a new flag in the given bitmask. Just like with the string modifiers,
        /// this will return a new bitmask; not modify the given one.
        /// </summary>
        public static T AddFlag<T>(this T bitmask, T flag) where T : Enum
        {
            return (T)(object)AddFlag((int)(object)bitmask,
                                      (int)(object)flag);
        }
        public static int AddFlag(this int bitmask, int flag)
        {
            return bitmask | flag;
        }

        /// <summary>
        /// Removes a flag from the given bitmask. Just like with the string modifiers,
        /// this will return a new bitmask; not modify the given one.
        /// </summary>
        public static T RemoveFlag<T>(this T bitmask, T flag) where T : Enum
        {
            return (T)(object)RemoveFlag((int)(object)bitmask,
                                         (int)(object)flag);
        }
        public static int RemoveFlag(this int bitmask, int flag)
        {
            return bitmask & (~flag);
        }
    }
    
    public static class TransformExtensions
    {
        public static Transform GetRootParent(this Transform transform)
        {
            if (transform.parent == null) return null;

            Transform rootParent = transform;
            while(rootParent.parent != null)
            {
                rootParent = rootParent.parent;
            }

            return rootParent;
        }

        public static Vector3[] GetPositions(this IList<Transform> transformCollection)
        {
            Vector3[] arr = new Vector3[transformCollection.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = transformCollection[i].position;
            }

            return arr;
        }
    }

    public static class ScriptableObjectExtensions
    {
        /// <summary>
        /// Creates and returns a clone of any given scriptable object.
        /// </summary>
        public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
        {
            if (scriptableObject == null)
            {
                Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
                return (T)ScriptableObject.CreateInstance(typeof(T));
            }

            T instance = UnityEngine.Object.Instantiate(scriptableObject);
            //instance.name = scriptableObject.name; // remove (Clone) from name
            return instance;
        }
    }

#if BOLT_EXISTS
    public static class BoltExtensions
    { 
        /// <summary>
        /// Attempts to get the flow machine object variable, and out its value.
        /// </summary>
        /// <typeparam name="T"> The expected type of the variable. </typeparam>
        /// <param name="flowMachine"> The flow machine using the variable. </param>
        /// <param name="variable"> The variable name. </param>
        /// <param name="value"> The out value. </param>
        /// <returns> True if the variable was found and its value was put into the out argument; false otherwise. </returns>
        public static bool TryGetVariable<T>(this FlowMachine flowMachine, string variable, out T value)
        {
            VariableDeclarations declarations = Variables.Object(flowMachine);
            if (declarations.IsDefined(variable))
            {
                value = declarations.Get<T>(variable);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to set the flow machine object variable to the given value.
        /// </summary>
        /// <typeparam name="T"> The expected type of the variable. </typeparam>
        /// <param name="flowMachine"> The flow machine using the variable. </param>
        /// <param name="variable"> The variable name. </param>
        /// <param name="value"> The value to be set. </param>
        /// <returns> True if the variable was found and its value was set; false otherwise. </returns>
        public static bool TrySetVariable<T>(this FlowMachine flowMachine, string variable, T value)
        {
            VariableDeclarations declarations = Variables.Object(flowMachine);
            if (declarations.IsDefined(variable))
            {
                declarations.Set(variable, value);
                return true;
            }

            return false;
        }
    }
#endif
#endregion
}