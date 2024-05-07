/*
 * Copyright (c) Thorben Linneweber and others
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Numerics;

namespace Jitter2.LinearMath;

/// <summary>
/// Quaternion Q = W + Xi + Yj + Zk. Uses Hamilton's definition of ij=k.
/// </summary>
public struct JQuaternion
{
    public float X;
    public float Y;
    public float Z;
    public float W;

    public static JQuaternion Identity => new(0, 0, 0, 1);

    public JQuaternion(float x, float y, float z, float w)
    {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }

    public JQuaternion(float w, in JVector v)
    {
        W = w;
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }


    public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)
    {
        Add(in quaternion1, in quaternion2, out JQuaternion result);
        return result;
    }

    public static void Add(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        result.X = quaternion1.X + quaternion2.X;
        result.Y = quaternion1.Y + quaternion2.Y;
        result.Z = quaternion1.Z + quaternion2.Z;
        result.W = quaternion1.W + quaternion2.W;
    }

    public static JQuaternion Conjugate(in JQuaternion value)
    {
        return Quaternion.Conjugate(value);
    }

    public readonly JQuaternion Conj()
    {
        return Quaternion.Conjugate(this);
    }

    public readonly override string ToString()
    {
        return $"{W:F6} {X:F6} {Y:F6} {Z:F6}";
    }

    public static JQuaternion Subtract(in JQuaternion quaternion1, in JQuaternion quaternion2)
    {
        return Quaternion.Subtract(quaternion1, quaternion2);
    }

    public static void Subtract(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        result = Quaternion.Subtract(quaternion1, quaternion2);
    }

    public static JQuaternion Multiply(in JQuaternion quaternion1, in JQuaternion quaternion2)
    {
        return Quaternion.Multiply(quaternion1, quaternion2);
    }

    public static void Multiply(in JQuaternion quaternion1, in JQuaternion quaternion2, out JQuaternion result)
    {
        result = Quaternion.Multiply(quaternion1, quaternion2);
    }

    public static JQuaternion Multiply(in JQuaternion quaternion1, float scaleFactor)
    {
        return Quaternion.Multiply(quaternion1, scaleFactor);
    }

    public static void Multiply(in JQuaternion quaternion1, float scaleFactor, out JQuaternion result)
    {
        result = Quaternion.Multiply(quaternion1, scaleFactor);
    }

    public readonly float Length()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
    }

    public void Normalize()
    {
        var n = Quaternion.Normalize(this);
        X = n.X;
        Y = n.Y;
        Z = n.Z;
        W = n.W;
    }

    public static JQuaternion CreateFromMatrix(in JMatrix matrix)
    {
        return Quaternion.CreateFromRotationMatrix(matrix.value);
    }

    public static void CreateFromMatrix(in JMatrix matrix, out JQuaternion result)
    {
        result = Quaternion.CreateFromRotationMatrix(matrix.value);
    }

    public static JQuaternion operator *(in JQuaternion value1, in JQuaternion value2)
    {
        Multiply(value1, value2, out JQuaternion result);
        return result;
    }

    public static JQuaternion operator *(float value1, in JQuaternion value2)
    {
        Multiply(value2, value1, out JQuaternion result);
        return result;
    }

    public static JQuaternion operator *(in JQuaternion value1, float value2)
    {
        return Quaternion.Multiply(value1, value2);
    }

    public static JQuaternion operator +(in JQuaternion value1, in JQuaternion value2)
    {
        return Quaternion.Add(value1, value2);
    }

    public static JQuaternion operator -(in JQuaternion value1, in JQuaternion value2)
    {
        return Quaternion.Subtract(value1, value2);
    }

    #region Interop with System.Numerics
    
    public JQuaternion(System.Numerics.Quaternion quaternion)
    {
        X = quaternion.X;
        Y = quaternion.Y;
        Z = quaternion.Z;
        W = quaternion.W;
    }
    
    public static implicit operator System.Numerics.Quaternion(JQuaternion self)
    {
        return new System.Numerics.Quaternion(self.X, self.Y, self.Z, self.W);
    }
    
    public static implicit operator JQuaternion(System.Numerics.Quaternion quaternion)
    {
        return new JQuaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    }
    
    #endregion
}