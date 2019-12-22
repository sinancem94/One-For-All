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

    BoxCollider collider;
    //oyun icinde bridge veya Ladder bu olcak
    GameObject MemberPass;

    [HideInInspector]
    //Ladder, bridge etc nin ilk basta kuruldugu pozisyon
    public Vector3 passStartPosition;
    [HideInInspector]
    //Ladder, bridge etc nin en son memberin pozisyonu 
    public Vector3 passEndPosition;

    private void Start()
    {
        collider = gameObject.GetComponent<BoxCollider>();

        if (collider.size.y <= 1)
        {
            Debug.LogWarning("Obstacle collider height is 1. It should be bigger for gang collision setting to 2.");
            collider.size = new Vector3(collider.size.x, 2, collider.size.z);
        }

        if (ManCount == 0)
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

    public void CreateObstacleMembers(List<MotherGang.GangMember> usedMembers, Vector3 passStartPos, Vector3 passEndPos)
    {
        //Obje pass edildikten sonra memberlarin olusturdugu path
        MemberPass = new GameObject("ObstaclePass");

        string objTag = "";

        if(ObstacleType == Type.Ladder)
        {
            objTag = "MemberLadder";
            MemberPass.layer = LayerMask.NameToLayer("MemberLadder");
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
            usedMember.transform.tag = objTag;
            usedMember.transform.gameObject.layer = LayerMask.NameToLayer(objTag);
        }

        this.passStartPosition = passStartPos;
        this.passEndPosition = passEndPos;

    }

    //eger gang pass in oldugu pozisyona yakinsa ve usedObstacle a carpiyorsa true don
    public bool isCloseToPassPoint(Vector3 gangPosition)
    {
        if (Mathf.Abs(Vector3.Distance(gangPosition, passStartPosition)) <= 5f || Mathf.Abs(Vector3.Distance(gangPosition, passEndPosition)) <= 5f)
            return true;

        return false;
    }
}
