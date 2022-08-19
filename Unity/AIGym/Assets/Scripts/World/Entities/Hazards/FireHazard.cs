/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/
using Newtonsoft.Json;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[JsonObject(MemberSerialization.OptIn)]
public class FireHazard : Hazard
{
    public AudioClip firesizzle ;

    
    public static float SPREAD_SPEED;
    //Vars to track when to spread and the time before a spread
    private float spreadTimer;

    // Start is called before the first frame update
    public override void Start()
    {
        Reset();
        base.Start();
    }

    public void Reset()
    {
        spreadTimer = 1;
    }

    public override void UpdateHazard()
    {
        //When the spread time is hit call Spread for all surrounding spaces 
        ClearDouble();
        if (spreadTimer > 0)
            spreadTimer -= SPREAD_SPEED;
        else //if (!hasSpread)
        {
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            continue;
                        else
                        {
                            Spread(new Vector3(x, y, z));
                        }
                    }
            Reset();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Character>(out var character))
        {
            character.Health -= 5;
            AudioSource sound = this.gameObject.GetComponent<AudioSource>();
            sound.clip = null;
            if (character.Health > 30)
            {
                sound.clip = firesizzle ;
            }
            if (sound.clip != null) sound.Play(0);
        }
    }

    /// <summary>
    /// Spawn a fire if the given position is valid
    /// </summary>
    /// <param name="direction"></param>
    void Spread(Vector3 direction)
    {
        Vector3 target = transform.localPosition + direction + new Vector3(0,0.5f,0);
        Vector3 belowTarget = target - new Vector3(0, 0.5f, 0);
        Collider[] hitColliders;

        hitColliders = Physics.OverlapBox(belowTarget, transform.localScale / 2, Quaternion.identity);
        foreach (Collider c in hitColliders)
        {
            if (Flammable(c.gameObject.tag))
            {
                hitColliders = Physics.OverlapBox(target, transform.localScale / 2, Quaternion.identity);
                bool open = false;
                if (hitColliders.Length == 0)
                    open = true;
                else
                {
                    foreach (Collider cp in hitColliders)
                    {
                        if (Available(cp.gameObject.tag))
                        {
                            open = true;
                        }
                    }
                }
                if (open)
                {
                    GameObject newFire = Duplicate();
                    newFire.transform.localPosition += direction;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Check if valid object to spawn fire on
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Flammable(string tag)
    {
        if (tag == "Floor" || tag == "Wall")
            return true;

        return false;
    }

    /// <summary>
    /// Check if object doesnt obstruct fire spreading
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Available(string tag)
    {
        if (tag == "Player" || tag == "Decoration")
            return true;

        return false;
    }

    /// <summary>
    /// Clear overlapping fire
    /// </summary>
    void ClearDouble()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.localPosition, transform.localScale / 2, Quaternion.identity);
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject != this.gameObject && c.gameObject.GetComponent<FireHazard>() != null)
            {
                Destroy(c.gameObject);
            }
        }
    }
}
