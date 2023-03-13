using ProjectDawn.Geometry3D;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ItemSpawner : MonoBehaviour
{
    public Item Prefab;
    public float Interval = 1.0f;
    public Box SpawnVolume = new Box(-0.5f, 1);

    Random Random = new Random(1);
    float Elapsed;
    Stack<Item> m_FreeItems = new Stack<Item>();

    void SpawnItem()
    {
        var item = Instantiate(Prefab);
        item.transform.position = (Vector3)Random.NextFloat3(SpawnVolume.Min, SpawnVolume.Max) + transform.position;
    }

    void Update()
    {
        Elapsed += Time.deltaTime;

        if (Elapsed >= Interval)
        {
            SpawnItem();
            Elapsed -= Interval;
        }
    }

    private void OnDrawGizmos()
    {
        Box volume = SpawnVolume;
        volume.Position += (float3)transform.position;
        ShapeGizmos.DrawSolidBox(volume, new Color(0, 1, 0, 0.2f));
        ShapeGizmos.DrawWireBox(volume, new Color(0, 1, 0, 1));
    }
}
