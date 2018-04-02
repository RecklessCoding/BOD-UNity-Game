using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour, IGrabable
{
    public SkinnedMeshRenderer meshRenderer;
    public Material teamAMaterial;
    public Material teamBMaterial;

    public Team.Type teamFlag;

    private Transform originalHolder;

    private BoxCollider _boxCollider;

    private bool isGrabbed;
    private IAgent flagHolder;

    public bool IsGrabbed
    {
        get { return isGrabbed; }
        set
        {
            if (isGrabbed != value)
            {
                isGrabbed = value;
                TeamManager.Instance.SendFlagMessage(this);
            }
        }
    }

    public bool Grabbed()
    {
        return isGrabbed;
    }

    void Awake()
    {
        originalHolder = transform.parent;
    }

    void Update()
    {
        UpdateMaterials();
    }

    void UpdateMaterials()
    {
        if (teamFlag == Team.Type.RED)
            meshRenderer.material = teamAMaterial;
        if (teamFlag == Team.Type.BLUE)
            meshRenderer.material = teamBMaterial;
    }

    internal void ResetFlag()
    {
        IsGrabbed = false;
        if (flagHolder!=null)
        {
            flagHolder.DropFlag();
            flagHolder = null;
        }
        transform.SetParent(originalHolder);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        GetComponent<BoxCollider>().enabled = true;
    }

    void OnDrawGizmos()
    {
        if (_boxCollider == null)
            _boxCollider = GetComponent<BoxCollider>();

        if (teamFlag == Team.Type.RED)
            Gizmos.color = Color.red;
        else if (teamFlag == Team.Type.BLUE)
            Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(_boxCollider.center + transform.position, _boxCollider.size / 2);
    }

    public Team.Type GetTeam()
    {
        return teamFlag;
    }

    public Vector3 GetLocation()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetGrabbed(bool grabbed, IAgent newHolder)
    {
        this.flagHolder = newHolder;
        this.IsGrabbed = grabbed;
    }
}
