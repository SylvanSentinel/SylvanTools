using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public GameObject whoShot;

    [Header("Bullet Stats")]
    [SerializeField] float bulletDamage = 1;
    [SerializeField] float speed = 20f;
    [SerializeField] float destroyTime = 5f;
    [SerializeField] GameObject explosion;
    [SerializeField] bool doExplosionDamage = false;
    [SerializeField] GameObject decal;
    [SerializeField] ProjectileType pType;
    [SerializeField] float alignSpeed = 5f;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] Transform audioHolder;

    [Header("Unique Effects")]
    [SerializeField] bool doesBounce = false;
    [SerializeField] int bounceCount = 3;

    [Header("Linger Objects")]
    [SerializeField] float lingerTime = 3f;
    [SerializeField] GameObject[] lingeringChildren;
    List<GameObject> heldObjects = new List<GameObject>();

    [Header("Collider Info")]
    private Vector3 origin = Vector3.zero;
    [SerializeField] float radius = 1;
    [SerializeField] float range = 1;


    Vector3 previousPosition;
    private void Awake()
    {
        //ProjectileAwake();
    }

    public void ProjectileAwake()
    {
        for (int i = 0; i < lingeringChildren.Length; i++)
        {
            heldObjects.Add(lingeringChildren[i]); //c
        }
        gameObject.SetActive(true);
        Destroy(gameObject, destroyTime);
        previousPosition = transform.position;// rb.position;
        didEnd = false;
    }
    public void ProjectileAwake(GameObject target)
    {
        this.target = target;
        for (int i = 0; i < lingeringChildren.Length; i++)
        {
            heldObjects.Add(lingeringChildren[i]);
        }
        gameObject.SetActive(true);
        Destroy(gameObject, destroyTime);
        previousPosition = transform.position;// rb.position;
        didEnd = false;
    }
    private GameObject target;
    private Vector3 targetLastPos;
    Vector3 pos;
    bool doExplosion = true;
    Vector3 currentPositionRayTo;
    private void FixedUpdate()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);

        if (target != null)
        {
            pos = target.transform.position;
            switch (pType)
            {
                case ProjectileType.Follow:
                    transform.LookAt(pos);
                    break;
                case ProjectileType.Guided:
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                                Quaternion.LookRotation(pos - transform.position), Time.deltaTime * alignSpeed);
                    break;
                case ProjectileType.Predict:
                    var hitPos = Predict(transform.position, pos, targetLastPos,
                                speed);
                    targetLastPos = pos;

                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(hitPos - transform.position), Time.deltaTime * alignSpeed);
                    break;
                default:
                    if (transform == null || target.transform == null) break;
                    transform.LookAt(pos);
                    break;

            }
        }



        currentPositionRayTo = transform.position - previousPosition;
        RaycastHit hit;
        if (Physics.SphereCast(previousPosition, radius, currentPositionRayTo, out hit, Vector3.Distance(transform.position, previousPosition)))
        {
            //HIT SOMETHING
            transform.position = hit.point; //THIS is important
            Collider other = hit.collider;

            if (other.gameObject != whoShot && !other.CompareTag("TAGS YOU WANT TO IGNORE"))
            {


                if (decal && !other.CompareTag("Player") && !other.CompareTag("Enemy"))
                {

                    //leave decal on bounce
                    if (doesBounce == false)
                    {
                        Decal(hit);
                    }

                }
                if (other.CompareTag("Player"))
                {
                    //other.dodamage

                }
                if (other.CompareTag("Enemy"))
                {
                    //do damage

                }

                Detonate(hit);
            }
        }
        previousPosition = transform.position;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = new Vector3(transform.position.x + origin.x, transform.position.y + origin.y, transform.position.z + origin.z + range);
        Gizmos.DrawWireSphere(pos, radius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }


    void Decal(RaycastHit hit)
    {
        GameObject d = Instantiate(decal, transform.position, Quaternion.identity);
        d.transform.up = hit.normal;
        d.transform.parent = hit.collider.gameObject.transform;
        Destroy(d, 5);
    }

    bool didEnd = false;
    void Detonate(RaycastHit hit)
    {
        if (didEnd)
        {
            return;
        }

        if (doesBounce && bounceCount > 0)
        {
            bounceCount--;
            Vector3 direction = Vector3.Reflect(transform.forward, hit.normal);
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
            return;
        }


        if (hitSFX != null && audioHolder != null)
        {
            audioHolder.GetComponent<AudioSource>().PlayOneShot(hitSFX);
        }


        didEnd = true;
        if (doExplosion)
        {

            if (explosion != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }

            if (doExplosionDamage)
            {
                Explode();
            }
        }

        for (int i = 0; i < heldObjects.Count; i++)
        {
            heldObjects[i].transform.parent = null;
            Destroy(heldObjects[i], lingerTime);
        }

        Destroy(gameObject);
    }

    private bool didExplode = false;
    private void Explode()
    {
        if (didExplode) return;
        didExplode = true;

        List<GameObject> hitList = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4);
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject other = hitCollider.gameObject;

            if (hitList.Contains(other.transform.root.gameObject)) return;

            if (other.CompareTag("Enemy") || other.CompareTag("Player"))
            {
                //see if can damage then do so
                /*
                if (other.GetComponentInParent<IDamageable>() != null)
                {
                    other.GetComponentInParent<IDamageable>().Damage(bulletDamage, whoShot);
                    hitList.Add(other.transform.root.gameObject);
                }
                */
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(5, transform.position, 5f, 3f, ForceMode.VelocityChange);
            }
        }
    }


    public Vector3 Predict(Vector3 sPos, Vector3 tPos, Vector3 tLastPos, float pSpeed)
    {
        // Target velocity
        var tVel = (tPos - tLastPos) / Time.deltaTime;

        // Time to reach the target
        var flyTime = GetProjFlightTime(tPos - sPos, tVel, pSpeed);

        if (flyTime > 0)
            return tPos + flyTime * tVel;
        return tPos;
    }

    private float GetProjFlightTime(Vector3 dist, Vector3 tVel, float pSpeed)
    {
        var a = Vector3.Dot(tVel, tVel) - pSpeed * pSpeed;
        var b = 2.0f * Vector3.Dot(tVel, dist);
        var c = Vector3.Dot(dist, dist);

        var det = b * b - 4 * a * c;

        if (det > 0)
            return 2 * c / (Mathf.Sqrt(det) - b);
        return -1;
    }

}

public enum ProjectileType
{
    Follow, Guided, Predict
}
