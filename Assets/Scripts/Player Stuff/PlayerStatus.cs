using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    Dictionary<string, int> actualAugs = new Dictionary<string, int>();
    [SerializeField] public Animator playerParticle;
    [SerializeField] RepellentRange repellentRange;
    public int speedBuffs = 0;

    //repellent stuff
    public bool inDanger = false;
    public bool repellentMoment = false;
    public float repellentLerpTime = 0.25f;
    public float repellentModeTimeScale = 0.2f;
    public float repellentModeZoom = 0.5f;
    [SerializeField] GameObject repellentObj;

    public float repMaxZoom;
    public float ogZoom;

    public float ogTimeScale;
    public float repTimeScale;
    // Start is called before the first frame update
    void Start()
    {
        repMaxZoom = Camera.main.orthographicSize * repellentModeZoom;
        ogZoom = Camera.main.orthographicSize;

        ogTimeScale = Time.timeScale;
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
                //Debug.Log("Running");
                if (repellentRange.enemiesInRange > 0)
                    inDanger = true;
                else
                    inDanger = false;
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
                //add a check here for using repellent - or start a coroutine after repellent begin
            }
            yield return new WaitForEndOfFrame();
        }

    }

    //lerp in to the repellent option
    IEnumerator RepellentBegin()
    {
        Debug.Log("we do a little repelling");
        GameControl.PlayerData.repellentMode = true;
        repellentObj.SetActive(true);
        
        //get the current values of everything
        float currentTimeScale = Time.timeScale;
        float currentCamZoom = Camera.main.orthographicSize;
        float currentTime = 0f;
        while (currentTime < repellentLerpTime)
        {
            Time.timeScale = Mathf.Lerp(currentTimeScale, repellentModeTimeScale, currentTime / repellentLerpTime);
            Camera.main.orthographicSize = Mathf.Lerp(currentCamZoom, repMaxZoom, currentTime / repellentLerpTime);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (!inDanger)
            {
                yield break;
            }
        }
        Camera.main.orthographicSize = repMaxZoom;
        GameControl.PlayerData.crosshairActive = false;

        yield return null;
    }

    //lerp out of the repellent option
    IEnumerator RepellentEnd()
    {
        Debug.Log("no more repelling");

        //get the current values of everything
        float currentTimeScale = Time.timeScale;
        float currentCamZoom = Camera.main.orthographicSize;
        float currentTime = 0f;
        while (currentTime < repellentLerpTime)
        {
            Time.timeScale = Mathf.Lerp(currentTimeScale, ogTimeScale, currentTime / repellentLerpTime);
            Camera.main.orthographicSize = Mathf.Lerp(currentCamZoom, ogZoom, currentTime / repellentLerpTime);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (inDanger)
            {
                yield break;
            }
        }
        Camera.main.orthographicSize = ogZoom;
        repellentObj.SetActive(false);
        GameControl.PlayerData.repellentMode = false;
        GameControl.PlayerData.crosshairActive = true;

        yield return null;
    }

    public void DangerOn()
    {
        inDanger = true;
        //Debug.Log("Danger on");
    }

    public void DangerOff()
    {
        inDanger = false;
        //Debug.Log("Danger off");
    }    
}
