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

    BoxCollider coll;
    //oyun icinde bridge veya Ladder bu olcak
    GameObject ObstaclePass;

    [HideInInspector]
    //Ladder, bridge etc nin ilk basta kuruldugu pozisyon
    public Vector3 passStartPosition;
    [HideInInspector]
    //Ladder, bridge etc nin en son memberin pozisyonu 
    public Vector3 passEndPosition;

    //asagi yukari ya da ileri geri pass den cikarken start ve end pozisyonlari degistirebilmek icin. transform ortasi yerine bu degeri kullanmak lazim.
    Vector3 passMiddlePosition;

    private void Start()
    {
        coll = gameObject.GetComponent<BoxCollider>();

        if (coll.size.y <= 1)
        {
            Debug.LogWarning("Obstacle collider height is 1. It should be bigger for gang collision setting to 2.");
            coll.size = new Vector3(coll.size.x, 2, coll.size.z);
        }

        if (ManCount == 0)
        {
            Debug.LogError("Man count of obstacle " + this.gameObject.name + " is not setted.");
        }

        if(ObstacleType == Type.Null)
        {
            Debug.LogError("Object type could not be null please assign a type in editor");
        }
        else 
        {
            this.tag = "Obstacle";
        }
    }

    public void SetAsPassableObstacle(List<MotherGang.GangMember> usedMembers, Vector3 passStartPos, Vector3 passEndPos)
    {
        //Obje pass edildikten sonra memberlarin olusturdugu path
        ObstaclePass = new GameObject("ObstaclePass");

        string objTag = "PassableObstacle";

        ObstaclePass.transform.parent = this.transform;

        ObstaclePass.tag = objTag;
        ObstaclePass.layer = LayerMask.NameToLayer(objTag);

        this.tag = objTag;
        this.gameObject.layer = LayerMask.NameToLayer(objTag);

        foreach (MotherGang.GangMember usedMember in usedMembers)
        {
            usedMember.transform.parent = ObstaclePass.transform;
            usedMember.transform.tag = objTag;
            usedMember.transform.gameObject.layer = LayerMask.NameToLayer(objTag);
        }

        this.passStartPosition = passStartPos;
        this.passEndPosition = passEndPos;

        passMiddlePosition = (passEndPos + passStartPos) / 2f;

    }

    //eger gang pass in oldugu pozisyona yakinsa ve usedObstacle a carpiyorsa true don
    public bool isCloseToPassPoint(Vector3 gangPosition)
    {
        if (Mathf.Abs(Vector3.Distance(gangPosition, passStartPosition)) <= 5f || Mathf.Abs(Vector3.Distance(gangPosition, passEndPosition)) <= 5f)
            return true;

        return false;
    }

    public void CloseColliders()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }
    public void OpenColliders()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
    }

    //her bir kopruden/merdivenden geciste degisiyor
    public int SetPassStartAndEndPositions()
    {
        int direction = 0;
        //calculate starting position and ending position if obstacle is a bridge look at z positions
        if (ObstacleType == Obstacle.Type.Bridge)
        {
            if (DataManager.instance.GetGang().Base.position.z < passMiddlePosition.z)
                direction = 1;
            else
                direction = -1;

            if ((passStartPosition.z > passEndPosition.z && direction == 1) || (passStartPosition.z < passEndPosition.z && direction != 1))
            {
                SwitchStartEndPositions();
            }
        }
        //calculate starting position and ending position if obstacle is a ladder look at y positions
        else if (ObstacleType == Obstacle.Type.Ladder)
        {

            if (DataManager.instance.GetGang().Base.position.y < passMiddlePosition.y)
                direction = 1;
            else
                direction = -1;

            if ((passStartPosition.y > passEndPosition.y && direction == 1) || (passStartPosition.y < passEndPosition.y && direction != 1))
            {
                SwitchStartEndPositions();
            }
        }

        return direction;
    }

    void SwitchStartEndPositions()
    {
        Vector3 tmpPos = passStartPosition;
        passStartPosition = passEndPosition;
        passEndPosition = tmpPos;
    }
}
