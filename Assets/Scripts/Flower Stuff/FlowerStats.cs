using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlowerStats : MonoBehaviour
{
    [SerializeField] public int rarity;
    [SerializeField] public string type;
    public int id;

    [SerializeField] protected int basePoints;
    [SerializeField] protected int damage;
    [SerializeField] protected int range;
    [SerializeField] protected int projCount;
    [SerializeField] protected int projRange;

    public Sprite headSprite;
    public Sprite projSprite;
    public Sprite stemSprite;
    public GameObject pool;

    [SerializeField]
    public int[] pointsTiers = new int[4];
    [SerializeField]
    public int[] damageTiers = new int[4];
    [SerializeField]
    public int[] rangeTiers = new int[4];
    [SerializeField]
    public int[] projTiers = new int[4];

    [SerializeField] public int aug;

    [SerializeField] public string primaryText, insideText, outsideText, fourText, fiveText;
    //associated projectile script
    //associated augment script
    // Start is called before the first frame update
    void Start()
    {
        UpdateAffinity(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual Sprite GetHeadSprite(int tier)
    {
        return headSprite;
    }

    public virtual void SetStem(GameObject head, GameObject stem)
    {
        head.transform.localPosition = new Vector3(-0.061f, 0.44f);
        stem.GetComponent<SpriteRenderer>().sprite = stemSprite;
    }

    public virtual int GetProjRange(int tier)
    {
        return projRange;
    }
    
    public void SetProjRange(int projRange)
    {
        this.projRange = projRange;
    }

    public virtual int GetRange(int tier)
    {
        return range;
    }

    public void SetRange(int range)
    {
        this.range = range;
    }

    public virtual int GetDamage(int tier)
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public virtual int GetPoints(int tier)
    {
        return basePoints;
    }

    public void SetPoints(int points)
    {
        this.basePoints = points;
    }

    public virtual int GetProjCount(int tier)
    {
        return projCount;
    }

    public void SetProjCount(int projCount)
    {
        this.projCount = projCount;
    }


    public void UpdateAffinity(int tier)
    {
        basePoints = pointsTiers[tier];
        damage = damageTiers[tier];
        range = rangeTiers[tier];
        projCount = projTiers[tier];
    }

    public virtual void OnProjArrival(GameObject proj)
    {
        Debug.Log("on arrival called for " + type);
        //proj.GetComponent<ProjectileBehavior>().ObjectDeactivate();
    }

    public virtual void OnEnemyCollision(GameObject enemy, int tier)
    {

    }

    public virtual void OnProjTravel(GameObject proj)
    {

    }

    public virtual void OnHitboxEnter(GameObject flower)
    {
        Debug.Log("Hitbox enter called for " + type);
    }

    protected IEnumerator SlowApply(float slowEffect, float slowTime, int particle, GameObject enemy)
    {
        GameObject part = enemy.GetComponent<EnemyBehavior>().nextParticle();
        enemy.GetComponent<EnemyBehavior>().setParticle(part, particle);
        enemy.GetComponent<EnemyBehavior>().isSlowed = true;
        /*if (isFrozen)
        {
            backupSpeed = backupSpeed * slowEffect;
            backupUsed = true;
        }
        else
            moveSpeed = moveSpeed * slowEffect;*/
        enemy.GetComponent<EnemyBehavior>().SpeedDown(slowEffect);
        yield return new WaitForSeconds(slowTime);
        //this might be fucked;
        /*if (isFrozen || backupUsed)
        {
            Debug.Log("BACKUP");
            backupSpeed = backupSpeed / slowEffect;
        }
        else
            moveSpeed = moveSpeed / slowEffect;*/
        if (!enemy.GetComponent<EnemyBehavior>().wasKilled)
        {
            enemy.GetComponent<EnemyBehavior>().SpeedUp(slowEffect);
            enemy.GetComponent<EnemyBehavior>().isSlowed = false;
            enemy.GetComponent<SpriteRenderer>().color = Color.white;
            enemy.GetComponent<EnemyBehavior>().setParticle(part, 0);
            if (particle == 4)
                enemy.GetComponent<EnemyBehavior>().isElectrified = false;
        }
        /*if (backupUsed)
        {
            moveSpeed = backupSpeed;
            backupUsed = false;
        }*/
    }
}
