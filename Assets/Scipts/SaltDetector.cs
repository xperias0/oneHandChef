using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltDetector : MonoBehaviour
{
    
    public int maxScore = 10;
    public int prefectAmount = 10;
    public int minAmount = 2;
    
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetScore()
    {
        int preScore = maxScore / (prefectAmount - minAmount);
        if (count - minAmount < 0) return 0;
        
        int diff = count - minAmount - prefectAmount;
        if (diff < 0)
        {
            return (count - minAmount) * preScore;
        } else if (diff == 0)
        {
            return maxScore;
        }
        return maxScore - (diff * preScore);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        count++;
        Debug.Log("Collision detected");
    }
}
