using System;
using System.Collections.Generic;
using System.Linq;
using Jitter2;
using Jitter2.Collision;
using Jitter2.Collision.Shapes;
using Jitter2.LinearMath;
using JitterDemo.Renderer;
using JitterDemo.Renderer.OpenGL;

namespace JitterDemo;

public class Dragon : Renderer.TriangleMesh
{
    public Dragon() : base("dragon.obj.zip", 5)
    {
    }

    public override void LightPass(PhongShader shader)
    {
        shader.MaterialProperties.SetDefaultMaterial();
        shader.MaterialProperties.ColorMixing.Set(1.2f, 0, 0.5f);
        base.LightPass(shader);
    }
}

public class CollisionTriangle : ISupportMap
{
    public JVector A, B, C;
    public void SupportMap(in JVector direction, out JVector result)
    {
        float min = JVector.Dot(A, direction);
        float dot = JVector.Dot(B, direction);

        result = A;
        if (dot > min) { min = dot; result = B; }
        dot = JVector.Dot(C, direction);
        if (dot > min) { result = C; }
    }

    public JVector GeometricCenter
    {
        get => (1.0f / 3.0f) * (A + B + C);
    }
}

public class CustomCollisionDetection : IBroadPhaseFilter
{
    private readonly World world;
    private readonly Shape shape;
    private readonly Octree octree;
    private readonly ulong minIndex;

    public CustomCollisionDetection(World world, Shape shape, Octree octree)
    {
        this.shape = shape;
        this.octree = octree;
        this.world = world;

        (minIndex, _) = World.RequestId(octree.Indices.Length);
    }

    [ThreadStatic] private static CollisionTriangle? ts;
    [ThreadStatic] private static Stack<uint>? candidates;

    public bool Filter(Shape shapeA, Shape shapeB)
    {
        if (shapeA != shape && shapeB != shape) return true;

        var collider = shapeA == shape ? shapeB : shapeA;

        if (collider.RigidBody == null || collider.RigidBody.Data.IsStaticOrInactive) return false;

        candidates ??= new Stack<uint>();
        ts ??= new CollisionTriangle();

        octree.Query(candidates, collider.WorldBoundingBox);

        while (candidates.Count > 0)
        {
            uint index = candidates.Pop();
            ts.A = octree.Vertices[octree.Indices[index].IndexA];
            ts.B = octree.Vertices[octree.Indices[index].IndexB];
            ts.C = octree.Vertices[octree.Indices[index].IndexC];

            bool hit = NarrowPhase.MPREPA(ts, collider, collider.RigidBody!.Orientation, collider.RigidBody!.Position,
                out JVector pointA, out JVector pointB, out JVector normal, out float penetration);

            if (hit)
            {
                world.RegisterContact(collider.ShapeId, minIndex + (ulong)index, world.NullBody, collider.RigidBody,
                    pointA, pointB, normal, penetration, false);
            }
        }

        return false;
    }
}

public class Demo20 : IDemo
{
    public string Name => "Custom Collision (Octree)";

    private Playground pg = null!;
    private World world = null!;

    public void Build()
    {
        pg = (Playground)RenderWindow.Instance;
        world = pg.World;

        pg.ResetScene(true);

        var tm = RenderWindow.Instance.CSMRenderer.GetInstance<Dragon>();

        var indices = tm.mesh.Indices.Select((i) 
            => new Octree.TriangleIndices(i.T1, i.T2, i.T3)).ToArray();

        var vertices = tm.mesh.Vertices.Select(v
            => Conversion.ToJitterVector(v.Position)).ToArray();

        // Build the octree
        var octree = new Octree(indices, vertices);

        // Add a "test" shape to the jitter world. We will filter out broad phase collision
        // events generated by Jitter and add our own collision handling. Make the test shape
        // the dimensions of the octree.
        var testShape = new TransformedShape(new BoxShape(octree.Dimensions.Max - octree.Dimensions.Min), octree.Dimensions.Center);
        world.AddShape(testShape);

        world.BroadPhaseFilter = new CustomCollisionDetection(world, testShape, octree);
    }

    public void Draw()
    {
        var tm = RenderWindow.Instance.CSMRenderer.GetInstance<Dragon>();
        tm.PushMatrix(Matrix4.Identity, new Vector3(0.35f, 0.35f, 0.35f));
    }
}