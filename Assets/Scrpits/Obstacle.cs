using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum Type
    {
        Null,
        Ladder,
        Bridge
    }

    [Header("Choose a obstacle type")]
    public Type ObstacleType;

    [Header("Assign how many mans are needed to pass this obstacle")]
    public int ManCount;

    [HideInInspector]
    //Bridge veya Ladder bu olcak
    public GameObject MemberPass;

    [HideInInspector]
    public List<Transform> ObstacleMembers;

    [HideInInspector]
    //Ladder, bridge etc nin ilk basta kuruldugu pozisyon
    public Vector3 passStartMember;
    [HideInInspector]
    //Ladder, bridge etc nin en son memberin pozisyonu 
    public Vector3 passEndMember;

    private void Start()
    {
        if(ManCount == 0)
        {
            Debug.LogError("Man count of obstacle " + this.name + " is not setted.");
        }

        if(ObstacleType == Type.Null)
        {
            Debug.LogError("Object type could not be null please assign a type in editor");
        }
        else if(ObstacleType == Type.Ladder)
        {
            this.tag = "LadderObstacle";
        }
        else if(ObstacleType == Type.Bridge)
        {
            this.tag = "BridgeObstacle";
        }
    }

    public void CreateObstacleMembers(List<MotherGang.GangMember> usedMembers, Vector3 passStartPosition, Vector3 passEndPosition)
    {
        //Obje pass edildikten sonra memberlarin olusturdugu path
        MemberPass = new GameObject("ObstaclePass");

        string objTag = "";

        if(ObstacleType == Type.Ladder)
        {
            objTag = "MemberLadder";
        }
        else if(ObstacleType == Type.Bridge)
        {
            objTag = "MemberBridge";
            MemberPass.layer = LayerMask.NameToLayer("MemberBridge");
        }

        MemberPass.transform.parent = this.transform;

        MemberPass.tag = objTag;
        MemberPass.layer = LayerMask.NameToLayer(objTag);

        this.tag = objTag;
        this.gameObject.layer = LayerMask.NameToLayer(objTag);

        foreach (MotherGang.GangMember usedMember in usedMembers)
        {
            usedMember.transform.parent = MemberPass.transform;
        }

        passStartMember = passStartPosition;
        passEndMember = passEndPosition;
    }

    //eger gang pass in oldugu pozisyona yakinsa ve usedObstacle a carpiyorsa true don
    public bool isCloseToPassPoint(Vector3 gangPosition)
    {
        if (Mathf.Abs(Vector3.Distance(gangPosition, passStartMember)) <= 10f || Mathf.Abs(Vector3.Distance(gangPosition, passEndMember)) <= 10f)
            return true;

        return false;
    }
}
