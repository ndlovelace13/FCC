using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    Dictionary<string, int> actualAugs = new Dictionary<string, int>();
    [SerializeField] public Animator playerParticle;
    public int speedBuffs = 0;

    public bool inDanger = false;
    public bool repellentMoment = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RepellentChecker());
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
            if (!otherParent.GetComponent<ProjectileBehavior>().single)
            {
                bool enemyProj = otherParent.GetComponent<ProjectileBehavior>().enemyProj;
                actualAugs = otherParent.GetComponent<ProjectileBehavior>().getActualAugs();
                AugmentApplication(actualAugs, enemyProj);
                otherParent.GetComponent<ProjectileBehavior>().ObjectDeactivate();
            }
        }
        else if (other.gameObject.tag == "aoe")
        {
            GameObject otherParent = other.gameObject.transform.parent.gameObject;
            actualAugs = otherParent.GetComponent<AoeBehavior>().getActualAugs();
            AugmentApplication(actualAugs, false);
            otherParent.SetActive(false);
        }
    }

    public void AugmentApplication(Dictionary<string, int> actualAugs, bool enemy)
    {
        //execute flower buffs if the proj came from an enemy
        if (!enemy)
        {
            foreach (var aug in actualAugs)
            {
                GameControl.PlayerData.flowerStatsDict[aug.Key].OnPlayerCollision(gameObject, aug.Value);
            }
        }
        //execute player debuffs if the proj came from an enemy
        else
        {
            foreach (var aug in actualAugs)
            {
                StartCoroutine(DebuffHandler(aug.Key));
            }
        }  
        
    }

    IEnumerator DebuffHandler(string augName)
    {
        switch (augName)
        {
            case "bully": StartCoroutine(Insult()); break;
            default: Debug.Log("You haven't handled this yet bozo"); break;
        }
        yield return null;
    }

    //Insult handler for boss projectiiles
    IEnumerator Insult()
    {
        //set speedBuff to false
        speedBuffs = 0;
        BullyStats bully = (BullyStats)GameControl.PlayerData.enemyStatsDict["bully"];
        

        PlayerMovement player = GetComponentInChildren<PlayerMovement>();
        player.speed *= bully.insultDebuff;
        playerParticle.SetInteger("augment", 7);

        float currentTime = 0f;
        while (currentTime < bully.insultLength)
        {
            currentTime += Time.deltaTime;
            if (speedBuffs > 0)
                break;
            yield return new WaitForEndOfFrame();
        }

        player.speed /= bully.insultDebuff;
        playerParticle.SetInteger("augment", 0);
    }

    IEnumerator RepellentChecker()
    {
        while (true)
        {
            if (!GameControl.PlayerData.loading && !GameControl.PlayerData.gameOver && !GameControl.PlayerData.gamePaused)
            {
                if (inDanger)
                    Debug.Log("Among us balls");
                //Debug.Log(GameControl.PlayerData.remainingRepellent);
                if (inDanger && !repellentMoment && GameControl.PlayerData.remainingRepellent > 0)
                {
                    repellentMoment = true;
                    StartCoroutine(RepellentBegin());
                }
                if (!inDanger && repellentMoment)
                {
                    repellentMoment = false;
                    StartCoroutine(RepellentEnd());
                }
            }
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator RepellentBegin()
    {
        Debug.Log("we do a little repelling");
        yield return null;
    }

    IEnumerator RepellentEnd()
    {
        Debug.Log("no more repelling");
        yield return null;
    }

    public void DangerOn()
    {
        inDanger = true;
    }

    public void DangerOff()
    {
        inDanger = false;
    }    
}
