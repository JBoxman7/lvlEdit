using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Utility {

    public static T[] ShuffleArray<T>(T[] array, int seed) {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++) {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }

    public static void Rotate<T>(this T[] array, int count) {
        if (array == null || array.Length < 2) return;
        count %= array.Length;
        if (count == 0) return;
        int left = count < 0 ? -count : array.Length + count;
        int right = count > 0 ? count : array.Length - count;
        if (left <= right) {
            for (int i = 0; i < left; i++) {
                var temp = array[0];
                Array.Copy(array, 1, array, 0, array.Length - 1);
                array[array.Length - 1] = temp;
            }
        }
        else {
            for (int i = 0; i < right; i++) {
                var temp = array[array.Length - 1];
                Array.Copy(array, 0, array, 1, array.Length - 1);
                array[0] = temp;
            }
        }
    }

    public static T CreateJaggedArray<T>(params int[] lengths) {
        return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths);
    }

    public static object InitializeJaggedArray(Type type, int index, int[] lengths) {
        Array array = Array.CreateInstance(type, lengths[index]);
        Type elementType = type.GetElementType();
        if (elementType != null) {
            for (int i = 0; i < lengths[index]; i++) {
                array.SetValue(
                    InitializeJaggedArray(elementType, index + 1, lengths), i);
            }
        }
        return array;
    }

    //public static float ClampAngle(float angle, float min, float max) {
    //    if (angle < -360) { angle += 360; }
    //    if (angle > 360) { angle -= 360; }
    //    return Mathf.Clamp(angle, min, max);
    //}

    public struct Coord {
        public int i;
        public int j;
        public int k;

        public Coord(int i, int j, int k) {
            this.i = i;
            this.j = j;
            this.k = k;
        }

        public static Vector3[] GetDirectionalOffsets() {
            Vector3[] offsets = {
                Vector3.up,
                Vector3.down,
                Vector3.right,
                Vector3.left,
                Vector3.forward,
                Vector3.back
            };
            return offsets;
        }
    }
}
