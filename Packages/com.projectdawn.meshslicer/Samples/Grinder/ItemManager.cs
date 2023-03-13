using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Item> Prefabs;
    List<Item> m_Items = new List<Item>();

    public List<Item> Items => m_Items;

    public Item CreateItem(int prefabIndex)
    {
        var gameObject = Instantiate(Prefabs[prefabIndex]);
        var item = gameObject.GetComponent<Item>();
        m_Items.Add(item);
        return item;
    }

    public void DestroyItem(Item item)
    {
        m_Items.Remove(item);
        Destroy(item);
    }

    void FixedUpdate()
    {
        foreach (var item in m_Items)
        {
            // Apply gravity
            float3 gravity = new float3(0, -10, 0);
            item.Velocity += gravity * Time.fixedDeltaTime;

            // Apply drag
            item.Velocity *= (1 - item.Drag * Time.fixedDeltaTime);

            // Apply velocity
            transform.position += (Vector3)(item.Velocity * Time.fixedDeltaTime);
        }
    }
}
