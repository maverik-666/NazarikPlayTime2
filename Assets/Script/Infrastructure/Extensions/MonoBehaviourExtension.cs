namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class Extensions
    {
        public static T Resolve<T>(this MonoBehaviour monoBeh)
        {
            return monoBeh.GetComponentInParent<Context>().Resolve<T>();
        }

        public static T GetParentRecursive<T>(this MonoBehaviour monoBeh)
        {
            Transform parent = monoBeh.transform.parent;
            while (parent != null)
            {
                if (parent.TryGetComponent(out T c))
                {
                    return c;
                }

                parent = parent.parent;
            }

            return default;
        }
        
        public static float RoundToStep(this float value, float step)
        {
            return Mathf.Round(value / step) * step;
        }

        public static Vector3 RoundToStep(this Vector3 value, float step)
        {
            return new Vector3(
                value.x.RoundToStep(step),
                value.y.RoundToStep(step),
                value.z.RoundToStep(step)
            );
        } 
        
        public static void Shuffle<T>(this IList<T> ts) {
            int count = ts.Count;
            int last = count - 1;
            for (var i = 0; i < last; ++i) {
                int r = Random.Range(i, count);
                (ts[i], ts[r]) = (ts[r], ts[i]);
            }
        }
        
        public static Vector2 Rotate(this Vector2 v, float degrees) 
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        
            float tx = v.x;
            float ty = v.y;
        
            return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        }

        public static float CalcRotation(this Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }
    }
}