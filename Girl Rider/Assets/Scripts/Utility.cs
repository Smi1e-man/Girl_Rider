using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float FLOAT_TO_INT_MULTIPLIER = 100f;

    public class LineSegment
    {
        public Point p1;
        public Point p2;

        public Color color;

        public bool hasIntersection;

        public int pIndex1;
        public int pIndex2;

        public LineSegment(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.color = Color.black;
        }

        public LineSegment(Vector3 v1, Vector3 v2, int pIndex1 = 0, int pIndex2 = 0) : this(new Point(v1), new Point(v2))
        {
            this.pIndex1 = pIndex1;
            this.pIndex2 = pIndex2;

            p1.pIndex = pIndex1;
            p2.pIndex = pIndex2;
        }

        public Vector3 Center => (p1.GetVector() + p2.GetVector()) * 0.5f;

    }

    public class Point
    {
        public int x;
        public int y;

        public int pIndex;

        private float _thirdCoord;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
            this._thirdCoord = 0;
        }

        public Point(Vector3 v) : this(Mathf.RoundToInt(v.x * FLOAT_TO_INT_MULTIPLIER), Mathf.RoundToInt(v.z * FLOAT_TO_INT_MULTIPLIER))
        {
            _thirdCoord = v.y;
        }

        public Vector3 GetVector()
        {
            return new Vector3(x / FLOAT_TO_INT_MULTIPLIER, _thirdCoord, y / FLOAT_TO_INT_MULTIPLIER);
        }

    };



    public static Color MultiplyHSV(Color c, float h, float s, float v)
    {
        Color.RGBToHSV(c, out float H, out float S, out float V);

        H = Mathf.Clamp01(H * h);
        S = Mathf.Clamp01(S * s);
        V = Mathf.Clamp01(V * v);

        return Color.HSVToRGB(H, S, V);
    }

    public static Color AddHSV(Color c, float h, float s, float v)
    {
        Color.RGBToHSV(c, out float H, out float S, out float V);

        H = Mathf.Clamp01(H + h);
        S = Mathf.Clamp01(S + s);
        V = Mathf.Clamp01(V + v);

        return Color.HSVToRGB(H, S, V);
    }

    public static List<Point> GetIdealPolygon(List<Point> polygon, float dudeRadius, int dudeCount, out float targetRadius, out Vector3 centerP, out float deltaRadius)
    {
        List<Point> idealPolygon = new List<Point>();

        centerP = Vector3.zero;
        for (int i = 0; i < polygon.Count; i++)
        {
            centerP += polygon[i].GetVector();
        }
        centerP /= polygon.Count;
        centerP.y = 0;

        Vector3 initialP = polygon[0].GetVector();
        initialP.y = 0;
        float radius = (centerP - initialP).magnitude;

        /*if (dudeCount == 0) */dudeCount++;
        targetRadius = Mathf.Sqrt(dudeCount * (4 * dudeRadius * dudeRadius) / Mathf.PI);

        Vector3 initialDir = (initialP - centerP).normalized;
        centerP += initialDir * (radius - targetRadius);

        deltaRadius = Mathf.Clamp(radius - targetRadius, 0, 100f);
        //Debug.Log(deltaRadius);

        radius = targetRadius;

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector3 pos = centerP + Quaternion.Euler(0, 360f / polygon.Count * i, 0) * initialDir * radius;
            pos.y = polygon[0].GetVector().y;
            idealPolygon.Add(new Point(pos) { pIndex = polygon[i].pIndex});
        }

        return idealPolygon;
    }

    public static string IntToString(int a, int charCount)
    {
        string s = a.ToString();

        int countToAdd = charCount - s.Length;
        for (int i = 0; i < countToAdd; i++)
        {
            s = "0" + s;
        }

        return s;
    }



    static int INF = 10000;


    // Given three colinear points p, q, r, the function checks if 
    // point q lies on line segment 'pr' 
    public static bool OnSegment(Point p, Point q, Point r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;

        return false;
    }

    // To find orientation of ordered triplet (p, q, r). 
    // The function returns following values 
    // 0 --> p, q and r are colinear 
    // 1 --> Clockwise 
    // 2 --> Counterclockwise 
    public static int GetOrientation(Point p, Point q, Point r)
    {
      
        int val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0; // colinear 

        return (val > 0) ? 1 : 2; // clock or counterclock wise 
    }

    public static bool DoLinesIntersect(LineSegment l1, LineSegment l2)
    {
        return DoLinesIntersect(l1.p1, l1.p2, l2.p1, l2.p2);
    }


    public static bool DoLinesIntersect(Point p1, Point q1, Point p2, Point q2)
    {
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = GetOrientation(p1, q1, p2);
        int o2 = GetOrientation(p1, q1, q2);
        int o3 = GetOrientation(p2, q2, p1);
        int o4 = GetOrientation(p2, q2, q1);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
        if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

        // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases 
    }

    public static bool IsPointInside(Point p, Point[] polygon)
    {
        int n = polygon.Length;

        // There must be at least 3 vertices in polygon[] 
        if (n < 3)
        {
            return false;
        }

        // Create a point for line segment from p to infinite 
        Point extreme = new Point(INF, p.y);

        // Count intersections of the above line  
        // with sides of polygon 
        int count = 0, i = 0;
        do
        {
            int next = (i + 1) % n;

            // Check if the line segment from 'p' to  
            // 'extreme' intersects with the line  
            // segment from 'polygon[i]' to 'polygon[next]' 
            if (DoLinesIntersect(polygon[i],
                            polygon[next], p, extreme))
            {
                // If the point 'p' is colinear with line  
                // segment 'i-next', then check if it lies  
                // on segment. If it lies, return true, otherwise false 
                if (GetOrientation(polygon[i], p, polygon[next]) == 0)
                {
                    return OnSegment(polygon[i], p,
                                     polygon[next]);
                }
                count++;
            }
            i = next;
        } while (i != 0);

        // Return true if count is odd, false otherwise 
        return (count % 2 == 1); // Same as (count%2 == 1) 
    }


    public static bool IsInside(Point p, Point[] polygon)
    {
        int n = polygon.Length;

        bool isInside = false;

        int i, j = 0;
        for (i = 0, j = n - 1; i < n; j = i++)
        {
            if ((((polygon[i].y <= p.y) && (p.y < polygon[j].y)) ||
                 ((polygon[j].y <= p.y) && (p.y < polygon[i].y))) &&
                (p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                isInside = !isInside;
        }
        return isInside;
    }



    // Convex hull

    public static IList<Vector2> ComputeConvexHull(List<Vector2> points, bool sortInPlace = false)
    {
        if (!sortInPlace)
            points = new List<Vector2>(points);
        points.Sort((a, b) =>
            a.x == b.x ? a.y.CompareTo(b.y) : (a.x > b.x ? 1 : -1));

        // Importantly, DList provides O(1) insertion at beginning and end
        CircularList<Vector2> hull = new CircularList<Vector2>();
        int L = 0, U = 0; // size of lower and upper hulls

        // Builds a hull such that the output polygon starts at the leftmost Vector2.
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Vector2 p = points[i], p1;

            // build lower hull (at end of output list)
            while (L >= 2 && (p1 = hull.Last).Sub(hull[hull.Count - 2]).Cross(p.Sub(p1)) >= 0)
            {
                hull.PopLast();
                L--;
            }
            hull.PushLast(p);
            L++;

            // build upper hull (at beginning of output list)
            while (U >= 2 && (p1 = hull.First).Sub(hull[1]).Cross(p.Sub(p1)) <= 0)
            {
                hull.PopFirst();
                U--;
            }
            if (U != 0) // when U=0, share the Vector2 added above
                hull.PushFirst(p);
            U++;
            Debug.Assert(U + L == hull.Count + 1);
        }
        hull.PopLast();
        return hull;
    }

    private static Vector2 Sub(this Vector2 a, Vector2 b)
    {
        return a - b;
    }

    private static float Cross(this Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    private class CircularList<T> : List<T>
    {
        public T Last
        {
            get
            {
                return this[this.Count - 1];
            }
            set
            {
                this[this.Count - 1] = value;
            }
        }

        public T First
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }

        public void PushLast(T obj)
        {
            this.Add(obj);
        }

        public T PopLast()
        {
            T retVal = this[this.Count - 1];
            this.RemoveAt(this.Count - 1);
            return retVal;
        }

        public void PushFirst(T obj)
        {
            this.Insert(0, obj);
        }

        public T PopFirst()
        {
            T retVal = this[0];
            this.RemoveAt(0);
            return retVal;
        }
    }
}
