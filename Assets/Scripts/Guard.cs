using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardSpottedPlayer;

    public Transform pathHolder;
    public Light spotLight;
    public float viewDistance;
    public LayerMask viewMask;
    private float viewAngle;

    public float guardingSpeed = 5f;
    public float waitTime = .3f;
    public float turnSpeed = 90f;
    public float timeToSpotPlayer = .5f;


    private Transform player;
    private Color originalSpotLightColor;
    private float playerVisibleTimer;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotLight.spotAngle;

        originalSpotLightColor = spotLight.color;

        Vector3[] wayPoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = pathHolder.GetChild(i).position;
            wayPoints[i].y = transform.position.y;
        }
        StartCoroutine(FollowPath(wayPoints));
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= viewDistance)
        {
            Vector3 dir = player.position - transform.position;
            Vector2 dir2D = new Vector2(dir.x, dir.z);
            Vector2 dirGuard2D = new Vector2(transform.forward.x, transform.forward.z);
            float angle = Vector2.Angle(dirGuard2D, dir2D);

            if (Mathf.Abs(angle) <= viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask)) // if there is obstacles
                {
                    return true;
                }
                
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] wayPoints)
    {
        transform.position = wayPoints[0];

        int targetWayPointIndex = 1;
        Vector3 targetWayPoint = wayPoints[targetWayPointIndex];

        transform.LookAt(targetWayPoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint, guardingSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWayPoint) < 0.1)
            {
                targetWayPointIndex = (targetWayPointIndex + 1) % wayPoints.Length;
                targetWayPoint = wayPoints[targetWayPointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWayPoint));
            }
            yield return null; // yield one frame
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 directionToTarget = (lookTarget - transform.position).normalized;

        float targetAngle = 90 - Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        } else
        {
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotLight.color = Color.Lerp(originalSpotLightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardSpottedPlayer != null)
            {
                OnGuardSpottedPlayer();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 prevPosition = startPosition;

        foreach (Transform wayPoint in pathHolder)
        {
            Gizmos.DrawSphere(wayPoint.position, .2f);
            Gizmos.DrawLine(prevPosition, wayPoint.position);
            prevPosition = wayPoint.position;
        }
        Gizmos.DrawLine(prevPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
