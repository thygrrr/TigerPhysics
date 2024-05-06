using System.Numerics;

namespace JitterTests;

public class NumericsInteropTests
{
    [TestCase]
    public void Can_Cast_Matrix()
    {
        var matrix = new Matrix4x4
        {
            M11 = 2,
            M12 = 3,
            M13 = 4,
            M14 = 0,
            M21 = 5,
            M22 = 6,
            M23 = 7,
            M24 = 0,
            M31 = 8,
            M32 = 9,
            M33 = 10,
            M34 = 0,
            M41 = 0,
            M42 = 0,
            M43 = 0,
            M44 = 1,
        };

        var jitterMatrix = (JMatrix) matrix;
        var convertedMatrix = (Matrix4x4) jitterMatrix;
        
        Assert.That(convertedMatrix, Is.EqualTo(matrix));
        Assert.That((JMatrix) convertedMatrix, Is.EqualTo(jitterMatrix));
    }

    [TestCase]
    public void Matrix_Transforms_Equivalent()
    {
        var matrix = new Matrix4x4
        {
            M11 = 2,
            M12 = 3,
            M13 = 4,
            M21 = 5,
            M22 = 6,
            M23 = 7,
            M31 = 8,
            M32 = 9,
            M33 = 10,
            M44 = 0,
        };

        var jitterMatrix = new JMatrix
        {
            value = matrix,
        };
        
        var vector = new Vector3(1, 2, 3);
        var jitterVector = new JVector(1, 2, 3);
        
        var transformedVector = Vector3.Transform(vector, matrix);
        var jitterTransformedVector = JVector.Transform(jitterVector, jitterMatrix);

        var rotation = Matrix4x4.CreateRotationY(10);
        var jitterRotation = JMatrix.CreateRotationY(10);
        
        Assert.That(jitterTransformedVector.X, Is.EqualTo(transformedVector.X).Within(float.Epsilon));
    }
}