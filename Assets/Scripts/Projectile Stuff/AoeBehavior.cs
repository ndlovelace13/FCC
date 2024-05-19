using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AoeBehavior : MonoBehaviour
{
    int[] augments;
    float activeTime = 2;
    List<GameObject> particles;
    int particleIgnore = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(int[] augs, float time, int partIgnore)
    {
        augments = augs;
        activeTime = time;
        particleIgnore = partIgnore;
        if (particles == null)
            getParticles();
        setParticles(augments);
        StartCoroutine(Deactivate());
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

    public void setParticles(int[] augs)
    {
        for (int i = 0; i < augs.Length; i++)
        {
            //Debug.Log("current aug: " + augs[i]);
            if (augs[i] != particleIgnore)
                particles[i].GetComponent<Animator>().SetInteger("augment", augs[i]);
            else
                particles[i].GetComponent<Animator>().SetInteger("augment", 0);
        }
    }

    public int[] getAugments() { return augments; }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(activeTime);
        particleIgnore = -1;
        gameObject.SetActive(false);
    }
}
