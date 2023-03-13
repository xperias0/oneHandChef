using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using static ProjectDawn.Mathematics.math2;
using static Unity.Mathematics.math;
using ProjectDawn.Mathematics;

public class FruitSpawner : MonoBehaviour
{
    public List<Fruit> Prefabs;
    public float Interval = 1.0f;
    public Vector2 ForceRange = new Vector2(1, 1);
    public float2 AngleRange = new float2();
    public float2 AngularForceRange = new float2(0, 1);

    Random Random;
    float Elapsed;

    void SpawnFruit()
    {
        var index = Random.NextInt(0, Prefabs.Count);

        var fruit = Instantiate(Prefabs[index]);
        fruit.transform.parent = GameObject.Find("Objs").transform;
        fruit.transform.gameObject.layer = 6;
        fruit.transform.position = transform.position;

        float force = Random.NextFloat(ForceRange.x, ForceRange.y);
        float angle = Random.NextFloat(AngleRange.x, AngleRange.y);
        float2 direction = math2.direction(radians(angle + 90));
        float angularForce = Random.NextFloat(AngularForceRange.x, AngularForceRange.y);

        // Update velocity
        var rigidBody = fruit.Rigidbody;
        rigidBody.velocity = normalize(new float3(direction.x, direction.y, 0)) * force;
        rigidBody.angularVelocity = new Vector3(0, 0, angularForce);

       // GetComponent<AudioSource>().Play();
    }

    private void Start()
    {
        Random = new Random(1);
    }

    void Update()
    {
        Elapsed += Time.deltaTime;

        if (Elapsed >= Interval)
        {
            SpawnFruit();
            Elapsed -= Interval;
        }

        if (GameObject.Find("Objs").transform.childCount > 15)
        {
            Destroy(GameObject.Find("Objs").transform.GetChild(0).gameObject);
        }

    }
}
