using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PHC.Utility
{
    public static class VectorExt
    {
        /// <summary>
        /// Rounds the Vector3 to the nearest whole numbers.
        /// </summary>
        public static Vector3 Round(this Vector3 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            vector.z = Mathf.Round(vector.z);
            return vector;
        }

        /// <summary>
        /// Rounds the Vector2 to the nearest whole numbers.
        /// </summary>
        public static Vector2 Round(this Vector2 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            return vector;
        }
    }
}
