/* Big thanks and credits to Daniel Shiffman and all contributors from P5.js.*/
/* I needed some calculations for a project and for this purpose I started to transfrom methods to c# */
using System.Numerics;


namespace ODC.P5Sharp
{
    public static class Vectors
    {
        public static Vector2 SetMag(this Vector2 vector2, float length)
        {
            vector2 = Vector2.Normalize(vector2);
            vector2 = Vector2.Multiply(vector2, length);
            if (float.IsNaN(vector2.X))
            {
                return new Vector2(0, 0);
            }

            return vector2;

        }
        public static Vector2 Limit(this Vector2 vector2, float max)
        {
            var mSq = vector2.MagSq();
            if (mSq > max * max)
            {
                vector2 = Vector2.Divide(vector2, (float)Math.Sqrt(mSq));
                vector2 = Vector2.Multiply(max, vector2);
            }
            return vector2;
        }
        public static Vector2 MyAdd(this Vector2 vector2, float x, float y)
        {
            float mx = x;
            float my = y;
            vector2.X += mx;
            vector2.Y += my;

            return vector2;
        }
        public static float Heading(this Vector2 vector2)
        {
            //float radianToDegree = 180 / MathF.PI;
            float h = MathF.Atan2(vector2.Y, vector2.X);
            //h *= radianToDegree;
            return h;
        }

        public static float MagSq(this Vector2 vector2)
        {
            var x = vector2.X;
            var y = vector2.Y;

            return x * x + y * y;
        }
    }
}