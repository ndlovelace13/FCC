using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AoeBehavior : MonoBehaviour
{
    string[] augments;
    Dictionary<string, int> actualAugs;
    float activeTime = 2;
    List<GameObject> particles;
    //what does this do????
    string particleIgnore = "";
    int tier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(Dictionary<string, int> augPass, float time, string partIgnore)
    {
        //augments = augs;
        actualAugs = augPass;
        activeTime = time;
        particleIgnore = partIgnore;
        if (particles == null)
            getParticles();
        //this.tier = tier;
        setParticles();
        StartCoroutine(Deactivate());
    }

    public int GetTier()
    {
        return tier;
    }

    private void getParticles()
    {
        //retrieving all particles
        particles = new List<GameObject>();
        Transform temp = transform.GetChild(0);
        int childCount = temp.childCount;
        Debug.Log("child count" + childCount);
        for (int i = 0; i < childCount; i++)
        {
            particles.Add(temp.GetChild(i).gameObject);
        }
    }

    public void setParticles(string[] augs)
    {
        for (int i = 0; i < augs.Length; i++)
        {
            //Debug.Log("current aug: " + augs[i]);
            if (augs[i] != "" && augs[i] != particleIgnore && augs[i] != null)
            {
                FlowerStats currentStats = GameControl.PlayerData.flowerStatsDict[augs[i]];
                particles[i].GetComponent<Animator>().SetInteger("augment", currentStats.aug);
            }
            else
            {
                particles[i].GetComponent<Animator>().SetInteger("augment", 0);
            }
        }
    }

    public void setParticles()
    {
        /*for (int i = 0; i < augs.Length; i++)
        {
            //Debug.Log("current aug: " + augs[i]);
            if (augs[i] != "" && augs[i] != null)
            {
                FlowerStats currentStats = GameControl.PlayerData.flowerStatsDict[augs[i]];
                particles[i].GetComponent<Animator>().SetInteger("augment", currentStats.aug);
            }
            else
            {
                particles[i].GetComponent<Animator>().SetInteger("augment", 0);
            }
        }*/
        for (int i = 0; i < actualAugs.Count; i++)
        {
            if (actualAugs.ElementAt(i).Key != particleIgnore)
            {
                FlowerStats currentStats = GameControl.PlayerData.flowerStatsDict[actualAugs.ElementAt(i).Key];
                particles[i].GetComponent<Animator>().SetInteger("augment", currentStats.aug);
            }
            
        }
    }

    public string[] getAugments() { return augments; }

    public Dictionary<string, int> getActualAugs()
    { return actualAugs; }

    public 

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(activeTime);
        particleIgnore = "";
        gameObject.SetActive(false);
    }
}
