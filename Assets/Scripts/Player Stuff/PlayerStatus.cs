using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    Dictionary<string, int> actualAugs = new Dictionary<string, int>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollisionCheck(Collider2D other)
    {
        //Debug.Log("collision happening");
        if (other.gameObject.tag == "projectile")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            actualAugs = otherParent.GetComponent<ProjectileBehavior>().getActualAugs();
            AugmentApplication(actualAugs);
            otherParent.GetComponent<ProjectileBehavior>().ObjectDeactivate();
        }
        else if (other.gameObject.tag == "aoe")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            actualAugs = otherParent.GetComponent<AoeBehavior>().getActualAugs();
            AugmentApplication(actualAugs);
            otherParent.SetActive(false);
        }
    }

    public void AugmentApplication(Dictionary<string, int> actualAugs)
    {
        foreach (var aug in actualAugs)
        {
            GameControl.PlayerData.flowerStatsDict[aug.Key].OnPlayerCollision(gameObject, aug.Value);
        }
    }
}
