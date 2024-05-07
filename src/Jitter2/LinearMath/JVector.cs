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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jitter2.LinearMath;

/// <summary>
/// Represents a three-dimensional vector using three floating-point numbers.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct JVector : IEquatable<JVector>, IEquatable<Vector3>
{
    internal static JVector InternalZero;
    internal static JVector Arbitrary;

    public float X;
    public float Y;
    public float Z;

    public static readonly JVector Zero;
    public static readonly JVector UnitX;
    public static readonly JVector UnitY;
    public static readonly JVector UnitZ;
    public static readonly JVector One;
    public static readonly JVector MinValue;
    public static readonly JVector MaxValue;

    static JVector()
    {
        One = new JVector(1, 1, 1);
        Zero = new JVector(0, 0, 0);
        UnitX = new JVector(1, 0, 0);
        UnitY = new JVector(0, 1, 0);
        UnitZ = new JVector(0, 0, 1);
        MinValue = new JVector(float.MinValue);
        MaxValue = new JVector(float.MaxValue);
        Arbitrary = new JVector(1, 1, 1);
        InternalZero = Zero;
    }

    public JVector(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public void Set(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public JVector(float xyz)
    {
        X = xyz;
        Y = xyz;
        Z = xyz;
    }

    public readonly override string ToString()
    {
        return $"{X:F6} {Y:F6} {Z:F6}";
    }

    public readonly bool Equals(JVector other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public readonly bool Equals(Vector3 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is JVector other && Equals(other)
               || obj is Vector3 v3 && Equals(v3);
    }

    public static bool operator ==(JVector value1, JVector value2)
    {
        return ((Vector3) value1).Equals(value2);
    }

    public static bool operator !=(JVector value1, JVector value2)
    {
        return !((Vector3) value1).Equals(value2);
    }

    public static JVector Min(in JVector value1, in JVector value2)
    {
        Min(value1, value2, out JVector result);
        return result;
    }

    public static void Min(in JVector value1, in JVector value2, out JVector result)
    {
        result.X = value1.X < value2.X ? value1.X : value2.X;
        result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
        result.Z = value1.Z < value2.Z ? value1.Z : value2.Z;
    }

    public static JVector Max(in JVector value1, in JVector value2)
    {
        Max(value1, value2, out JVector result);
        return result;
    }

    public static JVector Abs(in JVector value1)
    {
        return new JVector(MathF.Abs(value1.X), MathF.Abs(value1.Y), MathF.Abs(value1.Z));
    }

    public static float MaxAbs(in JVector value1)
    {
        JVector abs = Abs(value1);
        return MathF.Max(MathF.Max(abs.X, abs.Y), abs.Z);
    }

    public static void Max(in JVector value1, in JVector value2, out JVector result)
    {
        result.X = value1.X > value2.X ? value1.X : value2.X;
        result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
        result.Z = value1.Z > value2.Z ? value1.Z : value2.Z;
    }

    public void MakeZero()
    {
        X = 0.0f;
        Y = 0.0f;
        Z = 0.0f;
    }

    /// <summary>
    /// Calculates matrix \times vector, where vector is a column vector.
    /// </summary>
    public static JVector Transform(in JVector vector, in JMatrix matrix)
    {
        return Vector3.Transform(vector, matrix);
    }

    /// <summary>
    /// Calculates matrix^\mathrf{T} \times vector, where vector is a column vector.
    /// </summary>
    public static JVector TransposedTransform(in JVector vector, in JMatrix matrix)
    {
        return Vector3.Transform(vector, Matrix4x4.Transpose(matrix));
    }

    /// <summary>
    /// Calculates matrix \times vector, where vector is a column vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Transform(in JVector vector, in JMatrix matrix, out JVector result)
    {
        result = Vector3.Transform(vector, matrix);
    }

    /// <summary>
    /// Calculates matrix^\mathrf{T} \times vector, where vector is a column vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TransposedTransform(in JVector vector, in JMatrix matrix, out JVector result)
    {
        var transposed = Matrix4x4.Transpose(matrix);
        result = Vector3.Transform(vector, transposed);
    }

    /// <summary>
    /// Calculates the outer product.
    /// </summary>
    public static JMatrix Outer(in JVector u, in JVector v)
    {
        var result = new JMatrix
        {
            M11 = u.X * v.X,
            M12 = u.X * v.Y,
            M13 = u.X * v.Z,
            M21 = u.Y * v.X,
            M22 = u.Y * v.Y,
            M23 = u.Y * v.Z,
            M31 = u.Z * v.X,
            M32 = u.Z * v.Y,
            M33 = u.Z * v.Z,
        };
        return (result); //TODO: Maybe needs transposing?
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot(in JVector vector1, in JVector vector2)
    {
        return Vector3.Dot(vector1, vector2);
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(in JVector value1, in JVector value2, out JVector result)
    {
        result = value1 + value2;
    }

    public static JVector Subtract(JVector value1, JVector value2)
    {
        Subtract(value1, value2, out JVector result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Subtract(in JVector value1, in JVector value2, out JVector result)
    {
        result = (Vector3) value1 - (Vector3) value2;
    }

    public static JVector Cross(in JVector vector1, in JVector vector2)
    {
        return Vector3.Cross(vector1, vector2);;
    }

    public static void Cross(in JVector vector1, in JVector vector2, out JVector result)
    {
        result = Vector3.Cross(vector1, vector2);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public void Negate()
    {
        X = -X;
        Y = -Y;
        Z = -Z;
    }

    public static JVector Negate(in JVector value)
    {
        Negate(value, out JVector result);
        return result;
    }

    public static void Negate(in JVector value, out JVector result)
    {
        result = (Vector3) value * -1;
    }

    public static JVector Normalize(in JVector value)
    {
        Normalize(value, out JVector result);
        return result;
    }

    public void Normalize()
    {
        float num2 = X * X + Y * Y + Z * Z;
        float num = 1f / (float)Math.Sqrt(num2);
        X *= num;
        Y *= num;
        Z *= num;
    }

    public static void Normalize(in JVector value, out JVector result)
    {
        result = Vector3.Normalize(value);
    }

    public readonly float LengthSquared()
    {
        return ((Vector3) this).LengthSquared();
    }

    public readonly float Length()
    {
        return ((Vector3) this).Length();
    }

    public static void Swap(ref JVector vector1, ref JVector vector2)
    {
        (vector2, vector1) = (vector1, vector2);
    }

    public static JVector Multiply(in JVector value1, float scaleFactor)
    {
        return (Vector3) value1 * scaleFactor;
    }

    public static void Multiply(in JVector value1, float scaleFactor, out JVector result)
    {
        result = Multiply(value1, scaleFactor);
    }

    /// <summary>
    /// Calculates the cross product.
    /// </summary>
    public static JVector operator %(in JVector vector1, in JVector vector2)
    {
        return Vector3.Cross(vector1, vector2);
    }

    public static float operator *(in JVector vector1, in JVector vector2)
    {
        return Vector3.Dot(vector1, vector2);
    }

    public static JVector operator *(in JVector value1, float value2)
    {
        return Vector3.Multiply(value1, value2);
    }

    public static JVector operator *(float value1, in JVector value2)
    {
        return Vector3.Multiply(value1, value2);
    }

    public static JVector operator -(in JVector value1, in JVector value2)
    {
        return Vector3.Subtract(value1, value2);
    }

    public static JVector operator -(JVector left)
    {
        return Negate(left);
    }

    public static JVector operator +(in JVector value1, in JVector value2)
    {
        return Vector3.Add(value1, value2);
    }

    #region Interop with System.Numerics
    
    public JVector(System.Numerics.Vector3 vector)
    {
        X = vector.X;
        Y = vector.Y;
        Z = vector.Z;
    }

    private JVector(Vector4 vector)
    {
        X = vector.X;
        Y = vector.Y;
        Z = vector.Z;
    }

    public static implicit operator System.Numerics.Vector4(JVector self)
    {
        return new System.Numerics.Vector4(self.X, self.Y, self.Z, 0);
    }
    
    public static implicit operator System.Numerics.Vector3(JVector self)
    {
        return new System.Numerics.Vector3(self.X, self.Y, self.Z);
    }
    
    public static implicit operator JVector(System.Numerics.Vector3 self)
    {
        return new JVector(self);
    }
    
    public static implicit operator JVector(System.Numerics.Vector4 self)
    {
        return new JVector(self);
    }
    
    #endregion
}