using UnityEngine;
using System.Collections.Generic;

public class BulletSystem : MonoBehaviour
{
    public static BulletSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public PlayerController player;
    public ComputeShader bulletCompute;
    public GPUInstanceBatch gpuInstanceBatch;
    public int maxBullets = 10000;
    public Vector2 maxDistance = new Vector2(10, 12);

    public struct BulletData
    {
        public Vector3 position;
        public Vector3 direction;
        public float radius;
        public float alive; // 1 = alive, 0 = dead
        public Vector2 uvOffset;
        public Vector2 uvSize;
        public Vector2 uvScale;
    }

    private ComputeBuffer bulletBuffer;
    private ComputeBuffer matrixBuffer;
    private ComputeBuffer playerHitBuffer;

    private BulletData[] cpuBullets; // keep track of CPU side state
    Queue<int> freeIndices = new Queue<int>();
    HashSet<int> freeIndicesSet = new HashSet<int>();

    private int kernel;
    private uint threadGroupSizeX;

    public int BulletCount => _bulletCount;
    [SerializeField] private int _bulletCount;

    public bool playerHit;
    private uint[] playerHitData = new uint[1];

    void Start()
    {
        gpuInstanceBatch.Initialize();

        bulletBuffer = new ComputeBuffer(maxBullets, sizeof(float) * 14, ComputeBufferType.Structured);
        matrixBuffer = new ComputeBuffer(maxBullets, 64, ComputeBufferType.Structured);
        playerHitBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.Structured);

        cpuBullets = new BulletData[maxBullets];
        freeIndices = new Queue<int>(maxBullets);

        kernel = bulletCompute.FindKernel("CSMain");
        bulletCompute.GetKernelThreadGroupSizes(kernel, out threadGroupSizeX, out _, out _);

        gpuInstanceBatch.SetMatrixBuffer(matrixBuffer, 0);
    }

    public void SpawnBullet(Vector3 position, Vector3 direction, Sprite sprite, float radius)
    {
        int indexToUse = -1;

        if (freeIndices.Count > 0)
        {
            indexToUse = freeIndices.Dequeue();
            freeIndicesSet.Remove(indexToUse);
        }
        else if (_bulletCount < maxBullets)
        {
            indexToUse = _bulletCount++;
        }

        if (indexToUse == -1) return;

        Texture tex = sprite.texture;
        Rect rect = sprite.textureRect;

        Vector2 uvOffset = new Vector2(rect.x / tex.width, rect.y / tex.height);
        Vector2 uvSize = new Vector2(rect.width / tex.width, rect.height / tex.height);

        cpuBullets[indexToUse] = new BulletData
        {
            position = position,
            direction = direction,
            radius = radius,
            alive = 1,
            uvSize = uvSize,
            uvOffset = uvOffset,
            uvScale = sprite.rect.size / sprite.pixelsPerUnit,
        };

        bulletBuffer.SetData(cpuBullets, indexToUse, indexToUse, 1);
    }

    void Update()
    {
        if (_bulletCount == 0) return;

        // Reset flag to 0 before running compute shader
        playerHitData[0] = 0;
        playerHitBuffer.SetData(playerHitData);

        // Set buffer
        bulletCompute.SetBuffer(kernel, "playerHitBuffer", playerHitBuffer);

        bulletCompute.SetVector("playerPosition", player.transform.position);
        bulletCompute.SetFloat("playerHitDistance", player.collisionDistance);

        bulletCompute.SetFloat("maxX", maxDistance.x);
        bulletCompute.SetFloat("maxY", maxDistance.y);
        bulletCompute.SetFloat("deltaTime", Time.deltaTime);
        bulletCompute.SetInt("bulletCount", _bulletCount);
        bulletCompute.SetBuffer(kernel, "bullets", bulletBuffer);
        bulletCompute.SetBuffer(kernel, "matrices", matrixBuffer);

        bulletCompute.Dispatch(kernel, Mathf.CeilToInt(_bulletCount / (float)threadGroupSizeX), 1, 1);

        // 🧩 After the compute shader runs, get alive status back periodically
        if (Time.frameCount % 10 == 0) // don't read every frame (expensive)
        {
            bulletBuffer.GetData(cpuBullets, 0, 0, _bulletCount);

            for (int i = 0; i < _bulletCount; i++)
            {
                if (cpuBullets[i].alive == 0 && freeIndicesSet.Add(i)) // Add returns false if already exists
                    freeIndices.Enqueue(i);
            }
        }

        playerHitBuffer.GetData(playerHitData);
        playerHit = playerHitData[0] == 1;

        gpuInstanceBatch.SetMatrixBuffer(matrixBuffer, _bulletCount);
        gpuInstanceBatch.SetBulletBuffer(bulletBuffer);
        gpuInstanceBatch.Render();
    }

    void OnDestroy()
    {
        bulletBuffer?.Release();
        matrixBuffer?.Release();
    }
}
