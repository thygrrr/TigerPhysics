using System.Numerics;
using System.Runtime.CompilerServices;
using Jitter2.Dynamics;
using Jitter2.LinearMath;
using JitterDemo.Renderer.OpenGL;
using Vector3 = JitterDemo.Renderer.OpenGL.Vector3;

namespace JitterDemo;

public class Conversion
{
    public static JMatrix ToJitterMatrix(Matrix4 im)
    {
        var mat = new JMatrix
        {
            M11 = im.M11,
            M12 = im.M12,
            M13 = im.M13,
            M21 = im.M21,
            M22 = im.M22,
            M23 = im.M23,
            M31 = im.M31,
            M32 = im.M32,
            M33 = im.M33,
        };

        return mat;
    }

    public static JVector ToJitterVector(Vector3 im)
    {
        return new JVector(im.X, im.Y, im.Z);
    }

    public static Vector3 FromJitter(JVector vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }

    public static Matrix4 FromJitter(JMatrix jmat)
    {
        jmat = Matrix4x4.Transpose(jmat);
        Matrix4 mat = new Matrix4
        {
            M11 = jmat.M11,
            M12 = jmat.M12,
            M13 = jmat.M13,
            M14 = 0,
            M21 = jmat.M21,
            M22 = jmat.M22,
            M23 = jmat.M23,
            M24 = 0,
            M31 = jmat.M31,
            M32 = jmat.M32,
            M33 = jmat.M33,
            M34 = 0,
            M41 = 0,
            M42 = 0,
            M43 = 0,
            M44 = 1,
        };

        return mat;
    }

    public static unsafe void FromJitterOpt(RigidBody body, out Matrix4 mat)
    {
        mat = FromJitter(body.Data.Orientation);
        mat.M14 = body.Data.Position.X;
        mat.M24 = body.Data.Position.Y;
        mat.M34 = body.Data.Position.Z;
        mat.M44 = 1; 


        /*
        Unsafe.CopyBlock(Unsafe.AsPointer(ref mat.M11), Unsafe.AsPointer(ref body.Data.Orientation.value.M11), 12);
        Unsafe.CopyBlock(Unsafe.AsPointer(ref mat.M12), Unsafe.AsPointer(ref body.Data.Orientation.value.M11), 12);
        Unsafe.CopyBlock(Unsafe.AsPointer(ref mat.M13), Unsafe.AsPointer(ref body.Data.Orientation.value.M11), 12);
        Unsafe.CopyBlock(Unsafe.AsPointer(ref mat.M14), Unsafe.AsPointer(ref body.Data.Position.X), 12);
        */
    }

    public static Matrix4 FromJitter(RigidBody body)
    {
        FromJitterOpt(body, out Matrix4 mat);
        return mat;
    }
}