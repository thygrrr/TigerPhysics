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
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Jitter2.LinearMath;

public static class MathHelper
{
    /*

    // Given two lines return closest points on both lines
    // Line 1: p1 + (p2 - p1)*mua
    // Line 2: p3 + (p4 - p3)*mub

    public static bool LineLineIntersect(in JVector p1, in JVector p2, in JVector p3, in JVector P4,  out JVector Pa, out JVector Pb, out float mua, out float mub)
    {
        const float Epsilon = 1e-12f;

        Pa = Pb = JVector.Zero;
        mua = mub = 0;

        JVector p13 = p1 - p3;
        JVector p43 = P4 - p3;
        JVector p21 = p2 - p1;

        float d1343 = p13 * p43;
        float d4321 = p43 * p21;
        float d1321 = p13 * p21;
        float d4343 = p43 * p43;
        float d2121 = p21 * p21;

        float denom = d2121 * d4343 - d4321 * d4321;
        if (Math.Abs(denom) < Epsilon) return false;

        float numer = d1343 * d4321 - d1321 * d4343;

        mua = numer / denom;
        mub = (d1343 + d4321 * mua) / d4343;

        Pa = p1 + (p21 * mua);
        Pb = p3 + (p43 * mub);

        return true;
    }

    */

    public static bool IsRotationMatrix(in JMatrix matrix, float epsilon = 1e-06f)
    {
        if (!UnsafeIsZero((Matrix4x4) JMatrix.MultiplyTransposed(matrix, matrix) - Matrix4x4.Identity, epsilon))
        {
            return false;
        }

        return MathF.Abs(matrix.Determinant() - 1.0f) < epsilon;
    }

    public static void UnsafeDecomposeMatrix(in JMatrix matrix, out JMatrix orientation, out JVector scale)
    {
        orientation = matrix;

        scale.X = orientation.X.Length();
        scale.Y = orientation.Y.Length();
        scale.Z = orientation.Z.Length();

        orientation.X *= 1.0f / scale.X;
        orientation.Y *= 1.0f / scale.Y;
        orientation.Z *= 1.0f / scale.Z;
    }

    public static bool IsZero(in JVector vector, float epsilon = 1e-6f)
    {
        return Vector3.Dot(vector, vector) < epsilon;
    }

    public static bool UnsafeIsZero(in JMatrix matrix, float epsilon = 1e-6f)
    {
        if (!IsZero(matrix.X, epsilon)) return false;
        if (!IsZero(matrix.Y, epsilon)) return false;
        if (!IsZero(matrix.Z, epsilon)) return false;
        return true;
    }



    /// <summary>
    /// Calculates an orthonormal vector to the given vector.
    /// </summary>
    /// <param name="vec">The input vector, which does not need to be normalized.</param>
    /// <returns>An orthonormal vector to the input vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JVector CreateOrthonormal(in JVector vec)
    {
        JVector result = vec;

        Debug.Assert(!CloseToZero(vec));

        float xa = Math.Abs(vec.X);
        float ya = Math.Abs(vec.Y);
        float za = Math.Abs(vec.Z);

        if ((xa > ya && xa > za) || (ya > xa && ya > za))
        {
            result.X = vec.Y;
            result.Y = -vec.X;
            result.Z = 0;
        }
        else
        {
            result.Y = vec.Z;
            result.Z = -vec.Y;
            result.X = 0;
        }

        result.Normalize();

        Debug.Assert(MathF.Abs(JVector.Dot(result, vec)) < 1e-6f);

        return result;
    }


    /// <summary>
    /// Determines whether the length of the given vector is zero or close to zero.
    /// </summary>
    /// <param name="v">The vector to evaluate.</param>
    /// <param name="epsilonSq">A threshold value below which the squared magnitude of the vector
    /// is considered to be zero or close to zero.</param>
    /// <returns>True if the vector is close to zero; otherwise, false.</returns>
    public static bool CloseToZero(in JVector v, float epsilonSq = 1e-16f)
    {
        return v.LengthSquared() < epsilonSq;
    }
}