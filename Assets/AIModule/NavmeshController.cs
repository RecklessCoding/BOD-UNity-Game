using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public Vector3 targetPosition;

    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set
        {
            targetPosition = value;
            GeneratePath(targetPosition);
        }
    }

    public NavMeshPath Path
    {
        get
        {
            return path;
        }

        set
        {
            path = value;
        }
    }

    public List<Vector3> pathGenerated = new List<Vector3>();
    public float distanceNeeded = 0.2f;

    public float offset;

    public bool moveToTarget = true;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetPosition = transform.position;
    }

    void Update()
    {
        CheckTarget();
    }

    void CheckTarget()
    {
        if (!moveToTarget)
            return;

        if (pathGenerated.Count <= 0 || pathGenerated[0] == null)
            return;

        if ((transform.position - pathGenerated[0]).magnitude <= distanceNeeded)
        {
            pathGenerated.RemoveAt(0);
            return;
        }
    }

    private NavMeshPath path;

    public void GeneratePath(Vector3 targetPosition)
    {
        if (targetPosition == null)
            return;

        path = new NavMeshPath();
        navMeshAgent.CalculatePath(targetPosition, path);
        pathGenerated = path.corners.ToList();
    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < pathGenerated.Count; i++)
        {
            if (i == 0)
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(new Vector3(pathGenerated[i].x, 2, pathGenerated[i].z), Vector3.one);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 2, Vector3.one);
    }
}
