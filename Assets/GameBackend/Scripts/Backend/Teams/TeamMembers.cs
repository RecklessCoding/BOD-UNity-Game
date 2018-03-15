using UnityEngine;

[System.Serializable]
public class TeamMembers : MonoBehaviour
{
    public GameObject[] teamMembers;

    public int TeamSize
    {
        get
        {
            return teamMembers.Length;
        }
    }
}