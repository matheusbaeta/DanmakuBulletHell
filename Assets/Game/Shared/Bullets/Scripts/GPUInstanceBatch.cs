using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class CPUInstanceBatch : MonoBehaviour
{
    public Mesh mesh; // geometric shape?
    public Material material;
    public int maxInstances = 100000;

    private ComputeBuffer argsBuffer;
    private int instanceCount;
    public void Initialize()
    {
        // Holds the Draw arguments for DrawMeshInstanceIndirect
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);

        uint[] args =
        {
            mesh.GetIndexCount(0),
            0,
            mesh.GetIndexStart(0),
            mesh.GetBaseVertex(0),
            0
        };

        argsBuffer.SetData(args);
        material.enableInstancing = true;
    }

    public void SetMatrixBuffer(ComputeBuffer buffer, int count)
    {
        instanceCount = count;
        material.SetBuffer("_PerInstanceData", buffer);
    }

    public void SetBulletBuffer(ComputeBuffer buffer)
    {
        material.SetBuffer("bullets", buffer);
    }
    public void Render()
    {
        if (instanceCount == 0 || mesh == null || material == null)
        {
            return;
        }

        uint[] args =
        {
            (uint)mesh.GetIndexCount(0),
            (uint)instanceCount, // INSTANCE COUNT CHANGES EACH FRAME
            (uint)mesh.GetIndexStart(0),
            (uint)mesh.GetBaseVertex(0),
            0
        };
        argsBuffer.SetData(args);

        Graphics.DrawMeshInstancedIndirect(
            mesh, 0, material, // mesh, submesh, material
            new Bounds(Vector3.zero, Vector3.one * 10000f), // Size of available regions to draw on GPU
            argsBuffer, // how many instances to draw
            0,
            null, // property block -> no extra per-call properties
            ShadowCastingMode.Off, // dont cast shadows
            false // disable shadow receiving -> good for performance
            );
    }
    private void OnDestroy()
    {
        argsBuffer?.Release();
    }
}
