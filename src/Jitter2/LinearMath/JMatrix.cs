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
/// 3x3 matrix of 32 bit float values in column major format.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 36)]
public struct JMatrix
{
    [FieldOffset(0)] public Vector4 X;
    [FieldOffset(16)] public Vector4 Y;
    [FieldOffset(32)] public Vector4 Z;
    [FieldOffset(48)] public Vector4 W;
    
    [FieldOffset(0)] public Matrix4x4 value;

    public float M11 { get => value.M11; set => this.value.M11 = value; }
    public float M12 { get => value.M12; set => this.value.M12 = value; }
    public float M13 { get => value.M13; set => this.value.M13 = value; }
    public float M21 { get => value.M21; set => this.value.M21 = value; }
    public float M22 { get => value.M22; set => this.value.M22 = value; }
    public float M23 { get => value.M23; set => this.value.M23 = value; }
    public float M31 { get => value.M31; set => this.value.M31 = value; }
    public float M32 { get => value.M32; set => this.value.M32 = value; }
    public float M33 { get => value.M33; set => this.value.M33 = value; }
    
    
    
    public static readonly JMatrix Identity;
    public static readonly JMatrix Zero;

    static JMatrix()
    {
        Zero = default;
        Identity = Matrix4x4.Identity;
    }

    public JMatrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
    {
        //TODO: There's an annotation that lets us skip this double initialization.
        X = Y = Z = W = default;
        
        value = new Matrix4x4()
        {
            M11 = m11,
            M12 = m12,
            M13 = m13,
            M21 = m21,
            M22 = m22,
            M23 = m23,
            M31 = m31,
            M32 = m32,
            M33 = m33,
            M44 = 1,
        };
    }

    public JVector UnsafeGet(int index)
    {
        return index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => JVector.Zero,
        };
    }

    public readonly JVector GetColumn(int index)
    {
        return index switch
        {
            0 => new JVector(value.M11, value.M21, value.M31),
            1 => new JVector(value.M12, value.M22, value.M32),
            2 => new JVector(value.M13, value.M23, value.M33),
            _ => JVector.Zero,
        };
    }

    public static JMatrix Multiply(in JMatrix matrix1, in JMatrix matrix2)
    {
        Multiply(matrix1, matrix2, out JMatrix result);
        return result;
    }

    public static JMatrix MultiplyTransposed(in JMatrix matrix1, in JMatrix matrix2)
    {
        MultiplyTransposed(matrix1, matrix2, out JMatrix result);
        return result;
    }

    public static JMatrix TransposedMultiply(in JMatrix matrix1, in JMatrix matrix2)
    {
        TransposedMultiply(matrix1, matrix2, out JMatrix result);
        return result;
    }

    public static JMatrix CreateRotationMatrix(JVector axis, float angle)
    {
        float c = MathF.Cos(angle / 2.0f);
        float s = MathF.Sin(angle / 2.0f);
        axis *= s;
        JQuaternion jq = new(axis.X, axis.Y, axis.Z, c);
        CreateFromQuaternion(in jq, out JMatrix result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(in JMatrix matrix1, in JMatrix matrix2, out JMatrix result)
    {
        result = Matrix4x4.Multiply(matrix1, matrix2);
    }

    public static JMatrix Add(JMatrix matrix1, JMatrix matrix2)
    {
        Add(matrix1, matrix2, out JMatrix result);
        return result;
    }

    /// <summary>
    /// Calculates matrix1 \times matrix2^\mathrm{T}.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MultiplyTransposed(in JMatrix matrix1, in JMatrix matrix2, out JMatrix result)
    {
        result = Matrix4x4.Multiply(matrix1, Matrix4x4.Transpose(matrix2));
    }

    public static JMatrix CreateRotationX(float radians)
    {
        return Matrix4x4.CreateRotationX(radians);
    }

    public static JMatrix CreateRotationY(float radians)
    {
        return Matrix4x4.CreateRotationY(radians);    
    }

    public static JMatrix CreateRotationZ(float radians)
    {
        return Matrix4x4.CreateRotationZ(radians);
    }

    /// <summary>
    /// Create a scaling matrix.
    /// </summary>
    /// <returns></returns>
    public static JMatrix CreateScale(in JVector scale)
    {
        return Matrix4x4.CreateScale(scale);
    }

    /// <summary>
    /// Create a scaling matrix.
    /// </summary>
    /// <returns></returns>
    public static JMatrix CreateScale(float x, float y, float z)
    {
        return Matrix4x4.CreateScale(x, y, z);
    }

    /// <summary>
    /// Calculates matrix1^\mathrm{T} \times matrix2.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TransposedMultiply(in JMatrix matrix1, in JMatrix matrix2, out JMatrix result)
    {
        result = Matrix4x4.Multiply(Matrix4x4.Transpose(matrix1), matrix2);
    }

    public static void Add(in JMatrix matrix1, in JMatrix matrix2, out JMatrix result)
    {
        result = matrix1.value + matrix2.value;
    }

    public static void Subtract(in JMatrix matrix1, in JMatrix matrix2, out JMatrix result)
    {
        result = (matrix1.value - matrix2.value);
    }

    public readonly float Determinant()
    {
        return value.GetDeterminant();
    }

    public static bool Inverse(in JMatrix matrix, out JMatrix result)
    {
        var success = Matrix4x4.Invert(matrix.value, out var res);
        result = res;
        return success;
    }

    public static JMatrix Multiply(JMatrix matrix1, float scaleFactor)
    {
        return matrix1.value * scaleFactor;
    }

    public static void Multiply(in JMatrix matrix1, float scaleFactor, out JMatrix result)
    {
        result = matrix1 * scaleFactor;
    }

    public static JMatrix CreateFromQuaternion(JQuaternion quaternion)
    {
        return Matrix4x4.CreateFromQuaternion(quaternion);
    }

    public static void Absolute(in JMatrix matrix, out JMatrix result)
    {
        result = Matrix4x4.Identity;
        result.M11 = MathF.Abs(matrix.value.M11);
        result.M12 = MathF.Abs(matrix.value.M12);
        result.M13 = MathF.Abs(matrix.value.M13);
        result.M21 = MathF.Abs(matrix.value.M21);
        result.M22 = MathF.Abs(matrix.value.M22);
        result.M23 = MathF.Abs(matrix.value.M23);
        result.M31 = MathF.Abs(matrix.value.M31);
        result.M32 = MathF.Abs(matrix.value.M32);
        result.M33 = MathF.Abs(matrix.value.M33);
    }

    public static void CreateFromQuaternion(in JQuaternion quaternion, out JMatrix result)
    {
        result = Matrix4x4.CreateFromQuaternion(quaternion);
    }

    public static JMatrix Transpose(in JMatrix matrix)
    {
        Transpose(in matrix, out JMatrix result);
        return result;
    }

    /// <summary>
    /// Returns JMatrix(0, -vec.Z, vec.Y, vec.Z, 0, -vec.X, -vec.Y, vec.X, 0)-
    /// </summary>
    public static JMatrix CreateCrossProduct(in JVector vec)
    {
        return new JMatrix(0, -vec.Z, vec.Y, vec.Z, 0, -vec.X, -vec.Y, vec.X, 0);
    }

    private static void Transpose(in JMatrix matrix, out JMatrix result)
    {
        result = Matrix4x4.Transpose(matrix);
    }

    public static JMatrix operator *(in JMatrix matrix1, in JMatrix matrix2)
    {
        return Matrix4x4.Multiply(matrix1, matrix2);
    }

    public float Trace()
    {
        return value.M11 + value.M22 + value.M33;
    }

    public static JMatrix operator *(float factor, in JMatrix matrix)
    {
        Multiply(matrix, factor, out JMatrix result);
        return result;
    }

    public static JMatrix operator *(in JMatrix matrix, float factor)
    {
        return matrix.value * factor;
    }

    public static JMatrix operator +(in JMatrix value1, in JMatrix value2)
    {
        Add(value1, value2, out JMatrix result);
        return result;
    }

    public static JMatrix operator -(in JMatrix value1, in JMatrix value2)
    {
        Subtract(value1, value2, out JMatrix result);
        return result;
    }

    #region Interop with System.Numerics
    
    public static implicit operator System.Numerics.Matrix4x4(JMatrix matrix)
    {
        return matrix.value;
    }
    
    public static implicit operator JMatrix(System.Numerics.Matrix4x4 matrix)
    {
        return new JMatrix
        {
            value = matrix,
        };
        /*
        (
            matrix.M11, matrix.M12, matrix.M13,
            matrix.M21, matrix.M22, matrix.M23,
            matrix.M31, matrix.M32, matrix.M33); //TODO: Maybe just set value.
            */
    }
    
    public static explicit operator System.Numerics.Quaternion(JMatrix matrix)
    {
        return System.Numerics.Quaternion.CreateFromRotationMatrix(matrix);
    }
    
    public static implicit operator JMatrix(System.Numerics.Quaternion quaternion)
    {
        CreateFromQuaternion(new JQuaternion(quaternion), out var result);
        return result;
    }
    
    #endregion
}