using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    int completedCount;
    int selectedCount;
    // Start is called before the first frame update
    void Start()
    {
        //Establish the Node Pool
        nodes = new List<Node>();
        selectedNodes = new List<Node>();
        GameControl.CrownCompletion.nodePooling();
        nodePool = GameControl.CrownCompletion.nodePool;
        GameControl.CrownCompletion.infoPopup = GameObject.FindWithTag("CrownInfo");
        GameControl.CrownCompletion.infoPopup.SetActive(false);
        NodeAssign();
        ToggleAssign();
        StartCoroutine(NodeMapping(nodes, 0f));
        selectedCount = GameControl.CrownCompletion.allCrowns.Count();
        completedCount = GameControl.CrownCompletion.totalDiscovered;
        GameControl.SaveData.newDiscoveries = 0;
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
        foreach (var crown in GameControl.CrownCompletion.allCrowns)
        {
            GameObject node = nodePool.GetComponent<ObjectPool>().GetPooledObject();
            node.SetActive(true);
            node.name = crown.Value.GetId();
            node.GetComponent<Node>().NodeAssignment(crown.Value);
            nodes.Add(node.GetComponent<Node>());
        }
    }
    IEnumerator FlowerSorting(List<string> types, List<bool> crownChoices, List<bool> discovery)
    {
        completedCount = 0;
        List<Node> selected = new List<Node>();
        List<Node> tossed = new List<Node>();
        bool wasSelected = false;
        foreach (var node in nodes)
        {
            Crown crown = node.crown;
            List<string> contained = crown.GetFlowers();
            /*foreach (string type in contained)
                Debug.Log(type);*/
            foreach (var type in types)
            {
                if (contained.Contains(type))
                {
                    //Debug.Log("passed the type check for:" + type);
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
            {
                //Debug.Log("Passed the selection test for discovery");
                wasSelected = true;
            }
            else
                wasSelected = false;
            stillSelected = false;
            //check crown types
            crownType currentStructure = crown.GetStructure();
            for (int i = 0; i < crownChoices.Count; i++)
            {
                if (!wasSelected || stillSelected)
                    break;
                if (crownChoices[i] == true)
                {
                    switch (i)
                    {
                        case 0: if (currentStructure == crownType.Basic)
                                stillSelected = true; break;
                        case 1: if (currentStructure == crownType.Advanced)
                                stillSelected = true; break;
                        case 2: if (currentStructure == crownType.Three)
                                stillSelected = true; break;
                        case 3: if (currentStructure == crownType.Complete)
                                stillSelected = true; break;
                        case 4: if (currentStructure == crownType.FullHouse)
                                stillSelected = true; break;
                        case 5: if (currentStructure == crownType.Four)
                                stillSelected = true; break;
                        case 6: if (currentStructure == crownType.Fiver)
                                stillSelected = true; break;
                    }
                }
            }
            if (stillSelected)
            {
                wasSelected = true;
                //Debug.Log("was selected overall");
            }
                
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
                if (crown.IsDiscovered())
                    completedCount++;
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
        Debug.Log("Post sorting count: " + selected.Count());
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
        Debug.Log("nodeGroup Last: " + nodeGroup.Last().gameObject.name);
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
            //finalNode = node;
            yield return new WaitForSeconds(delayTime);
        }
        Debug.Log("nodes mapped");
        finalNode = nodeGroup.Last();
        Camera.main.GetComponent<CompletionCamera>().NodeSet(finalNode);
    }

    IEnumerator assignLoc(Node node, int layer, int nodesPerLayer, int noInLayer)
    {
        float layerAngle = Mathf.PI * 2 / nodesPerLayer;
        float thisAngle = layerAngle * noInLayer;
        Vector2 direction = new Vector2(Mathf.Cos(thisAngle), Mathf.Sin(thisAngle));
        Vector3 newLocation = direction * layer * 2;
        if (node.firstTime)
            node.basePos = newLocation;
        node.newLocationLerp(newLocation);
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (discoveryToggles[2].isOn)
            ProgressText.text = completedCount + " of " + selectedCount + " Discovered - " + ((float)completedCount / selectedCount).ToString("0.00%");
        else
            ProgressText.text = selectedCount + " of " + GameControl.CrownCompletion.allCrowns.Count + " Selected";
    }

    public void SortCall()
    {
        Debug.Log("sorting now");
        //check flower types first
        List<string> flowerInputs = new List<string>();
        List<bool> crownInputs = new List<bool>();
        List<bool> discoveryInputs = new List<bool>();
        foreach (var toggle in flowerToggles)
        {
            if (toggle.isOn)
            {
                flowerInputs.Add(toggle.GetComponentInChildren<FlowerToggle>().type);
                Debug.Log(toggle.name);
            }
        }
        //check crown types
        foreach (var toggle in crownToggles)
        {
            crownInputs.Add(toggle.isOn);
        }
        //check discovery
        foreach (var toggle in discoveryToggles)
        {
            discoveryInputs.Add(toggle.isOn);
        }
        //get the selected bool
        showAll = !onlySelected.isOn;
        Debug.Log(flowerInputs.Count);
        StartCoroutine(FlowerSorting(flowerInputs, crownInputs, discoveryInputs));
    }
}
