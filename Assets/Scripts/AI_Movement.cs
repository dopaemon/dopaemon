using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class AI_Movement : MonoBehaviour
{
 
    Animator animator;
 
    public float moveSpeed = 0.2f;
    public float groundOffset = 0.03f;
 
    Vector3 stopPosition;
 
    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;
 
    int WalkDirection;
 
    public bool isWalking;
 
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
 
        //So that all the prefabs don't move/stop at the same time
        walkTime = Random.Range(3,6);
        waitTime = Random.Range(5,7);
 
 
        waitCounter = waitTime;
        walkCounter = walkTime;

        SnapToGround();
        ChooseDirection();
    }
 
    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
 
            walkCounter -= Time.deltaTime;
 
            switch (WalkDirection)
            {
                case  0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case  1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case  2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case  3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position -= transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }
 
            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;
                //stop movement
                transform.position = stopPosition;
                PlayAnimation(0);
                //reset the waitCounter
                waitCounter = waitTime;
            }
 
 
        }
        else
        {
 
            waitCounter -= Time.deltaTime;
 
            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }

    }

    void LateUpdate()
    {
        SnapToGround();
    }
 
 
    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
 
        isWalking = true;
        walkCounter = walkTime;
        PlayAnimation(1);
    }

    void SnapToGround()
    {
        Vector3 position = transform.position;

        foreach (Terrain terrain in Terrain.activeTerrains)
        {
            Vector3 terrainPosition = terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;

            if (position.x < terrainPosition.x || position.x > terrainPosition.x + terrainSize.x ||
                position.z < terrainPosition.z || position.z > terrainPosition.z + terrainSize.z)
            {
                continue;
            }

            position.y = terrain.SampleHeight(position) + terrainPosition.y + groundOffset;
            transform.position = position;
            return;
        }

        Vector3 rayOrigin = position + Vector3.up * 100f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 200f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            position.y = hit.point.y + groundOffset;
            transform.position = position;
        }
    }


void PlayAnimation(int animationIndex)
    {
        animator.SetInteger("AnimIndex", animationIndex);
        animator.SetTrigger("Next");
    }
}
