using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    private static string MAIN_KERNEL = "CSMain";
    public struct BulletData
    {
        public Vector3 position;
        public Vector3 direction;
        public float radius;
        public float alive;
        public Vector2 uvOffset;
        public Vector2 uvSize;
        public Vector2 uvScale;
    }

    public static BulletSystem Instance;

    public PlayerController player;
    public ComputeShader bulletCompute; // Runs bullet physics on GPU
    public CPUInstanceBatch gpuInstanceBatch; // Handles drawing bullets on screen

    public int maxBullets = 10000;
    public Vector2 maxDistance = new Vector2(10, 12);

    private ComputeBuffer bulletBuffer; // stores all bullet data
    private ComputeBuffer matrixBuffer; // stores transformation data for rendering
    private ComputeBuffer playerHitBuffer; 

    private BulletData[] cpuBullets;
    Queue<int> freeIndices = new Queue<int>(); 
    HashSet<int> freeIndicesSet = new HashSet<int>();

    private int kernel;
    private uint threadGroupSizeX;

    // Read-only variable (get _bulletCount value)
    public int BulletCount => _bulletCount;
    [SerializeField] private int _bulletCount;

    public bool playerHit;
    private uint[] playerHitData = new uint[1];

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gpuInstanceBatch.Initialize();

        var bulletBufferStride = sizeof(float) * 14; // Struct BulletData on BulletCompute are 14 floats
        bulletBuffer = new ComputeBuffer(maxBullets, bulletBufferStride, ComputeBufferType.Structured);
        matrixBuffer = new ComputeBuffer(maxBullets, 64, ComputeBufferType.Structured);
        playerHitBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.Structured);

        cpuBullets = new BulletData[maxBullets];
        freeIndices = new Queue<int>(maxBullets);

        kernel = bulletCompute.FindKernel(MAIN_KERNEL);

        /*
         Tells us how to divide the work efficiently across the GPU's parallel processors.
         Get How many threads can run together.
        */

        bulletCompute.GetKernelThreadGroupSizes(kernel, out threadGroupSizeX, out _, out _);
        // arguments(buffer, count)
        gpuInstanceBatch.SetMatrixBuffer(matrixBuffer, 0);
    }

    public void SpawnBullet(Vector3 position, Vector3 direction, Sprite sprite, float radius)
    {
        int indexToUse = -1;

        if(freeIndices.Count > 0)
        {
            indexToUse = freeIndices.Dequeue();
            freeIndicesSet.Remove(indexToUse);
        }
        else if(_bulletCount < maxBullets)
        {
            indexToUse = _bulletCount++;
        }

        // If index can't be updated return
        if (indexToUse == -1) return;

        Texture tex = sprite.texture;
        Rect rect = sprite.textureRect; // Get a 2D rectangle out of sprite texture

        Vector2 uvOffset = new Vector2(rect.x / tex.width, rect.y / tex.height); // Where the sprite STARTS in the big texture
        Vector2 uvSize = new Vector2(rect.width / tex.width, rect.height / tex.height); // How much of the texture this sprite uses

        cpuBullets[indexToUse] = new BulletData
        {
            position = position,
            direction = direction,
            radius = radius,
            alive = 1,
            uvSize = uvSize,
            uvOffset = uvOffset,
            uvScale = sprite.rect.size / sprite.pixelsPerUnit // sprite size in world units.
        };

        bulletBuffer.SetData(cpuBullets, indexToUse, indexToUse, 1);
    }

    private void Update()
    {
        if (_bulletCount == 0) return;

        // RESET flag to 0 before running compute shader
        playerHitData[0] = 0;
        playerHitBuffer.SetData(playerHitData);

        // Set Buffer
        bulletCompute.SetBuffer(kernel, "playerHitBuffer", playerHitBuffer);

        bulletCompute.SetVector("playerPosition", player.transform.position);
        bulletCompute.SetFloat("playerHitDistance", player.collisionDistance);

        bulletCompute.SetFloat("maxX", maxDistance.x);
        bulletCompute.SetFloat("maxY", maxDistance.y);
        bulletCompute.SetFloat("deltaTime", Time.deltaTime);
        bulletCompute.SetInt("bulletCount", _bulletCount);
        bulletCompute.SetBuffer(kernel, "bullets", bulletBuffer);
        bulletCompute.SetBuffer(kernel, "matrices", bulletBuffer);

        bulletCompute.Dispatch(kernel, Mathf.CeilToInt(_bulletCount / (float)threadGroupSizeX), 1, 1);

        // 🧩 After the compute shader runs, get alive status back periodically
        if(Time.frameCount % 10 == 0) // Don't read every frame too expensive
        {
            bulletBuffer.GetData(cpuBullets, 0, 0, _bulletCount);

            for(int i = 0; i < _bulletCount; i++)
            {
                if (cpuBullets[i].alive == 0 && freeIndicesSet.Add(i)) // Add returns false if already exists
                {
                    freeIndices.Enqueue(i);
                }
            }
        }

        playerHitBuffer.GetData(playerHitData);
        playerHit = playerHitData[0] == 1;

        gpuInstanceBatch.SetMatrixBuffer(matrixBuffer, _bulletCount);
        gpuInstanceBatch.SetBulletBuffer(bulletBuffer);
        gpuInstanceBatch.Render();
    }

    private void OnDestroy()
    {
        bulletBuffer?.Release();
        matrixBuffer?.Release();
    }
}
