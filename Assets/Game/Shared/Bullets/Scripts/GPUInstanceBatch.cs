using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

public class GPUInstanceBatch : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int maxInstances = 100000;

    private ComputeBuffer argsBuffer;
    private int instanceCount;

    public void Initialize()
    {
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);

        uint[] args = {
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
            return;

        uint[] args = {
            (uint)mesh.GetIndexCount(0),
            (uint)instanceCount,
            (uint)mesh.GetIndexStart(0),
            (uint)mesh.GetBaseVertex(0),
            0
        };
        argsBuffer.SetData(args);

        Graphics.DrawMeshInstancedIndirect(
            mesh, 0, material,
            new Bounds(Vector3.zero, Vector3.one * 10000f),
            argsBuffer,
            0,
            null,
            ShadowCastingMode.Off,
            false
        );
    }

    private void OnDestroy()
    {
        argsBuffer?.Release();
    }
}
