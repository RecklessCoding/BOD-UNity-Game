using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHandler : MonoBehaviour
{
    public IAgent agent;

    public Material redHead, redArms, redBody;
    public Material blueHead, blueArms, blueBody;

    public MeshRenderer helmetMeshRenderer;
    public SkinnedMeshRenderer armsMeshRenderer, bodyMeshRenderer;

    public int teamALayer, teamBLayer;

    void Start()
    {
        agent = GetComponent<Agent>();
    }

    void Update()
    {
        UpdateMaterials();
    }


    public void UpdateMaterials()
    {
        if (agent.GetTeam().Equals(Team.Type.RED))
        {
            helmetMeshRenderer.material = redHead;
            armsMeshRenderer.material = redArms;
            bodyMeshRenderer.material = redBody;

            gameObject.layer = teamALayer;
        }
        else 
        {
            helmetMeshRenderer.material = blueHead;
            armsMeshRenderer.material = blueArms;
            bodyMeshRenderer.material = blueBody;

            gameObject.layer = teamBLayer;
        }
    } 
}
