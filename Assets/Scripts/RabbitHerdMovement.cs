using UnityEngine;

public class RabbitHerdMovement : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float moveDuration = 4f;
    public float idleDuration = 3f;
    public float groundOffset = 0.03f;

    Animator[] animators;
    float timer;
    bool isMoving;

    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    void Start()
    {
        StartMoving();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (isMoving)
        {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }

        if (timer <= 0f)
        {
            if (isMoving)
            {
                StartIdle();
            }
            else
            {
                StartMoving();
            }
        }
    }

    void LateUpdate()
    {
        foreach (Animator rabbitAnimator in animators)
        {
            SnapToGround(rabbitAnimator.transform);
        }
    }

    void StartMoving()
    {
        transform.rotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);
        timer = moveDuration;
        isMoving = true;
        PlayAnimation(1);
    }

    void StartIdle()
    {
        timer = idleDuration;
        isMoving = false;
        PlayAnimation(0);
    }

    void PlayAnimation(int animationIndex)
    {
        foreach (Animator rabbitAnimator in animators)
        {
            rabbitAnimator.SetInteger("AnimIndex", animationIndex);
            rabbitAnimator.SetTrigger("Next");
        }
    }

    void SnapToGround(Transform rabbit)
    {
        Vector3 position = rabbit.position;

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
            rabbit.position = position;
            return;
        }
    }
}
