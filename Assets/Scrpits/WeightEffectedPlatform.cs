using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightEffectedPlatform : MonoBehaviour, Interactable
{
    [Header("Maximum Member Count For Access")]
    public int MaximumMemberCountForAccess;
    [Header("Minimum Member Count To Touch Ground")]
    public int MinimumMemberCountToTouchGround;

    Transform ground;

    bool isGoingDown;
    float effectingWeight;
    bool isOpen;

    Vector3 IdlePos;
    Vector3 maximumMovableLength;

    //eger ustunde bir weight varsa onu goToPos setleniyor ve asagi dogru iniyor.
    Vector3 goToPosition;

    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("Ground").transform;
        Vector3 groundPos = ground.position;
        groundPos.y += ground.localScale.y / 2f;

        IdlePos = this.transform.position;
        maximumMovableLength = new Vector3(IdlePos.x, IdlePos.y - groundPos.y - (transform.localScale.y), IdlePos.z);

        goToPosition = IdlePos;

        effectingWeight = 0f;

        isGoingDown = false;
        isOpen = true;
    }

    void Update()
    { 
            GetEffectedByWeight();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Base"))
        {
            RaycastHit hit;

            // base bu objenin ustundeyse
            if (collision.transform.position.y > this.transform.position.y && !isGoingDown)
            {
                isGoingDown = true;

                effectingWeight = DataManager.instance.GetGang().GangWheight;

                if (effectingWeight <= MaximumMemberCountForAccess)
                    effectingWeight = 0;
                else if (effectingWeight > MinimumMemberCountToTouchGround)
                    effectingWeight = MinimumMemberCountToTouchGround;

                //weight ve maximum movable lengthe gore bi uzunluk belirleniyor. Obje bu uzunluk kadar asagi incek
                Vector3 currentMovableLength;

                currentMovableLength = (effectingWeight / MinimumMemberCountToTouchGround) * maximumMovableLength;

                goToPosition = new Vector3(IdlePos.x, IdlePos.y - currentMovableLength.y, IdlePos.z);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Base"))
        {
            Collider thisCollider = GetComponent<Collider>();

            // base bu objenin ustundeyse
            if (isGoingDown && !thisCollider.bounds.Contains(new Vector3(collision.gameObject.transform.position.x, this.transform.position.y, collision.gameObject.transform.position.z))) 
            {
                isGoingDown = false;
                effectingWeight = 0f;

                goToPosition = IdlePos;
            }
        }
    }

    void GetEffectedByWeight()
    {
        transform.position = Vector3.MoveTowards(transform.position, goToPosition, 0.2f);

        if (isOpen == false && Vector3.Distance(transform.position, goToPosition) <= 0.2f)
            this.enabled = false;
    }

    public void Interacted()
    {
        isOpen = false;

        effectingWeight = 0f;

        goToPosition = IdlePos;

        if (isGoingDown)
            isGoingDown = false;

        this.transform.GetChild(1).gameObject.SetActive(true);
        this.transform.GetChild(2).gameObject.SetActive(true);
    }
}

