using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BDeshi.Utility.Extensions
{
    public static class Unity
    {
        public static void allignToDir(this Transform transform, Vector2 dir)
        {
            float angle = get2dAngle(dir);
            transform.set2dRotation(angle);
        }

        public static void allignToDir(this Transform transform, Vector2 dir, float angleOffsetInDegrees)
        {
            float angle = get2dAngle(dir) + angleOffsetInDegrees;
            transform.set2dRotation(angle);
        }

        //requires T to be a class and NOT a struct FIX: use.equals which may be slower...

        public static void allignToDir(this Rigidbody2D rb2D, Vector2 dir)
        {
            rb2D.rotation = get2dAngle(dir);
        }

        public static bool exceedsDist(this Vector3 vec, float dist)
        {
            return vec.sqrMagnitude > (dist * dist);
        }
        
        /// <summary>
        /// Imagine a cone with the "with" vec as forward, is the "dir" within a given angle with "with" 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="with"></param>
        /// <param name="halfAngle"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static  bool isWithinAngleWith(this Vector3 dir, Vector3 with, float halfAngle, out float angle)
        {
            angle = Vector2.Angle(with, dir);

            return angle <= halfAngle;
        }
        public static bool withinRange(this Vector3 vec, float minDist, float maxDist)
        {
            var d = vec.sqrMagnitude;
            return d >= (minDist * minDist) && d <= (maxDist * maxDist);
        }

        public static void set2dRotation(this Transform transform, float angle)
        {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public static void addAngleOffset(this Transform transform, float angleOffset)
        {
            transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.z + angleOffset, Vector3.forward);
        }
        
        /// <summary>
        /// Get 2d angle of this vector
        /// </summary>
        /// <param name="normalizedDir">FOR THE LOVE OF GOD NORMALIZE THIS</param>
        /// <returns></returns>
        public static float get2dAngle(this Vector3 normalizedDir)
        {
            return Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;
        }

        public static float get2dAngle(this Vector2 dir)
        {
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        public static float get2dAngle(this Transform t)
        {
            return get2dAngle(t.up);
        }
        public static void lookAlongTopDown(this Transform transform, Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        public static bool withinRange(this Vector2 range, float value)
        {
            return value >= range.x && value <= range.y;
        }

        public static Vector3 toTopDown(this Vector2 dir)
        {
            return new Vector3(dir.x, 0, dir.y);
        }

        public static void Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
        }
        
        public static Vector2 Rotated(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            return new Vector2((cos * tx) - (sin * ty),(sin * tx) + (cos * ty));
        }

        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        public static bool Contains(this LayerMask mask, GameObject obj)
        {
            return mask == (mask | (1 << obj.layer));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="hit">ASSUME THAT the raycast has HIT SOMETHING</param>
        /// <returns></returns>
        public static bool Contains(this LayerMask mask, RaycastHit2D hit)
        {
            return mask == (mask | (1 << hit.collider.gameObject.layer));
        }
        public static bool Contains(this LayerMask mask, RaycastHit hit)
        {
            return mask == (mask | (1 << hit.collider.gameObject.layer));
        }

        public static void reparentAndReset(this Transform transform, Transform parent)
        {
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        
        public static float distanceBetween(this Transform transform, Transform t)
        {
            return (transform.position - t.position).magnitude;
        }

        public static RaycastHit2D raycastFromInsideCollider2D(Vector2 origin, Vector2 direction, float length, LayerMask layer)
        {
            bool usedToHit = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = true;
            var result = Physics2D.Raycast(origin, direction, length, layer);
            Physics2D.queriesStartInColliders = usedToHit;

            return result;
        }

        public static Vector2 getRaycastEndpoint2D(Vector2 origin, Vector2 dir, float length, LayerMask layer, out RaycastHit2D hit)
        {
            dir.Normalize();
            hit = raycastFromInsideCollider2D(origin, dir, length, layer);
            return hit ? (hit.point) : (origin + dir * length);
        }


        public static Vector2 multiplyDimensions(this Vector2 v, Vector2 other)
        {
            return new Vector2(v.x * other.x, v.y * other.y);
        }

        public static Vector3 multiplyDimensions(this Vector3 v, Vector3 other)
        {
            return new Vector3(v.x * other.x, v.y * other.y, v.z * v.z);
        }
        public static void Shuffle<T>(this IList<T> list)  
        {  
            for (int i = list.Count -1 ; i < list.Count; i--)
            {
                int k = Random.Range(0, i+1); 
                (list[k], list[i]) = (list[i], list[k]);
            }

        }

#if UNITY_EDITOR
        public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        {
            if (_color != default(Color))
                Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (_height - (_radius * 2)) / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

            }
        }

        public static void DrawPathGizmos(List<Vector3> path)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Gizmos.DrawLine(path[i-1], path[i ]);
            }
        }
#endif
    }
}