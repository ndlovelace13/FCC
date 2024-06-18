using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletionNodes : MonoBehaviour
{

    GameObject nodePool;
    List<Node> nodes;
    List<Node> selectedNodes;
    public Node finalNode;
    [SerializeField] TMP_Text ProgressText;

    //toggles
    [SerializeField] FilterDrop flowerTab;
    [SerializeField] FilterDrop crownTab;
    [SerializeField] FilterDrop discoveryTab;

    List<Toggle> flowerToggles;
    List<Toggle> crownToggles;
    List<Toggle> discoveryToggles;


    [SerializeField] Toggle onlySelected;
    bool showAll;
    int selectedCount;
    // Start is called before the first frame update
    void Start()
    {
        //Establish the Node Pool
        nodes = new List<Node>();
        selectedNodes = new List<Node>();
        CrownCompletionism.completionTracker.nodePooling();
        nodePool = CrownCompletionism.completionTracker.nodePool;
        NodeAssign();
        ToggleAssign();
        StartCoroutine(NodeMapping(nodes, 0.005f));
        selectedCount = CrownCompletionism.completionTracker.crowns.Count();
    }

    private void ToggleAssign()
    {
        //flowers
        flowerToggles = flowerTab.GetOptions();
        //crowns
        crownToggles = crownTab.GetOptions();
        //discovery
        discoveryToggles = discoveryTab.GetOptions();
    }

    private void NodeAssign()
    {
        foreach (var crown in CrownCompletionism.completionTracker.crowns)
        {
            GameObject node = nodePool.GetComponent<ObjectPool>().GetPooledObject();
            node.SetActive(true);
            node.name = crown.GetId();
            node.GetComponent<Node>().NodeAssignment(crown);
            nodes.Add(node.GetComponent<Node>());
        }
    }
    IEnumerator FlowerSorting(List<string> types, List<bool> discovery)
    {
        List<Node> selected = new List<Node>();
        List<Node> tossed = new List<Node>();
        bool wasSelected = false;
        foreach (var node in nodes)
        {
            Crown crown = node.crown;
            List<string> contained = crown.GetFlowers();
            foreach (var type in types)
            {
                if (contained.Contains(type))
                {
                    wasSelected = true;
                    break;
                }
            }
            bool stillSelected = false;
            for (int i = 0; i < discovery.Count; i++)
            {
            
                if (!wasSelected || stillSelected)
                    break;
                if (discovery[i] == true)
                {
                    switch (i)
                    {
                        case 0:
                            if (!crown.IsDiscovered() && !crown.Discoverable())
                                stillSelected = true;
                            break;
                        case 1:
                            if (crown.Discoverable())
                                stillSelected = true;
                            break;
                        case 2:
                            if (crown.IsDiscovered())
                                stillSelected = true;
                            break;
                    }
                }
                
            }
            if (stillSelected)
                wasSelected = true;
            else
                wasSelected = false;
            if (!showAll)
            {
                node.SetVisible(wasSelected);
            }
            else
            {
                node.SetVisible(showAll);
            }
            if (!wasSelected)
            {
                tossed.Add(node);
            }
            else
            {
                selected.Add(node);
            }
            wasSelected = false;
        }
        List<Node> all = new List<Node>();
        all = selected.Union(tossed).ToList();
        if (!showAll)
            selectedCount = selected.Count();
        else
            selectedCount = all.Count();
        Debug.Log("Post sorting count: " + all.Count());
        if (selectedNodes != selected)
        {
            StartCoroutine(NodeMapping(all, 0f));
            Debug.Log("mapping starts now");
        }
            
        selectedNodes = selected;
        yield return null;
    }

    IEnumerator NodeMapping(List<Node> nodeGroup, float delayTime)
    {
        int layer = 0;
        int nodesPerLayer = 1;
        int noInLayer = 0;
        foreach (var node in nodeGroup)
        {
            //if the layer is full, increase the layer and adjust to the new layer stats accordingly
            if (nodesPerLayer == noInLayer)
            {
                layer++;
                noInLayer = 0;
                nodesPerLayer = (int) (layer * 2 * Mathf.PI);
            }
            StartCoroutine(assignLoc(node, layer, nodesPerLayer, noInLayer));
            noInLayer++;
            finalNode = node;
            yield return new WaitForSeconds(delayTime);
        }
        Debug.Log("nodes mapped");
        Camera.main.GetComponent<CompletionCamera>().NodeSet(finalNode);
    }

    IEnumerator assignLoc(Node node, int layer, int nodesPerLayer, int noInLayer)
    {
        float layerAngle = Mathf.PI * 2 / nodesPerLayer;
        float thisAngle = layerAngle * noInLayer;
        Vector2 direction = new Vector2(Mathf.Cos(thisAngle), Mathf.Sin(thisAngle));
        Vector3 newLocation = direction * layer * 2;
        if (node.firstTime)
        {
            node.InitialNodePlace(newLocation);
        }
        else
        {
            node.newLocationLerp(newLocation);
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        ProgressText.text = CrownCompletionism.completionTracker.totalDiscovered + " of " + selectedCount + " Discovered";
    }

    public void SortCall()
    {
        Debug.Log("sorting now");
        //check flower types first
        List<string> flowerInputs = new List<string>();
        List<bool> discoveryInputs = new List<bool>();
        foreach (var toggle in flowerToggles)
        {
            if (toggle.isOn)
            {
                flowerInputs.Add(toggle.GetComponentInChildren<TMP_Text>().text);
                Debug.Log(toggle.name);
            }
        }
        //check discovery
        foreach (var toggle in discoveryToggles)
        {
            discoveryInputs.Add(toggle.isOn);
        }
        //get the selected bool
        showAll = !onlySelected.isOn;
        Debug.Log(flowerInputs.Count);
        StartCoroutine(FlowerSorting(flowerInputs, discoveryInputs));
    }
}
