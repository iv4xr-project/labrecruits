/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireBuilder : MonoBehaviour
{
    public GameObject wireParent;
    public GameObject wirePrefab;
    public GameObject wireBlockPrefab;
    public Color baseColor;
    public Color litColor;

    //How long does it take for a lit part to loop through the whole wire.
    public float flowTime;
    
    //How long is one part lit.
    private float litTime;
    
    IEnumerator LitWire(List<GameObject> wireParts, int index)
    {
        GameObject wire = wireParts[index];
        wire.GetComponent<Renderer>().material.color = litColor;
        yield return new WaitForSeconds(litTime);
        wire.GetComponent<Renderer>().material.color = baseColor;

        yield return LitWire(wireParts, (index + 1) % wireParts.Count);
    }
    
    /// <param name="start">Starting point of the wire</param>
    /// <param name="end">Ending point of the wire</param>
    /// <param name="radius">Radius of the wire</param>
    /// <param name="stepLength">Length of each wire part in the wire</param>
    /// <param name="connected">Is the wire connected or broken?</param>
    public GameObject CreateWire(Transform start, Transform end, float radius, float stepLength, bool broken)
    {
        //Creating a bit of spacing for the wire between the links.
        Vector3 spacing = new Vector3(0, radius * 0.5f, 0);
        var s = start.transform.position + spacing;
        var e = end.transform.position + spacing;
        Vector3 dir = Vector3.Normalize(e - s);
        s += dir;
        e -= dir;
        
        float length = Vector3.Distance(s, e);
        List<GameObject> wireParts = new List<GameObject>();
        
        Wire newWire = Instantiate(wireParent, transform).GetComponent<Wire>();
        newWire.name = "Wire";
        newWire.broken = broken;
        newWire.start = start;
        newWire.end = end;
        
        Instantiate(wireBlockPrefab, s, Quaternion.identity, newWire.transform);
        Instantiate(wireBlockPrefab, e, Quaternion.identity, newWire.transform);
        float cable = 0f;
        
        while (cable < length)
        {
            GameObject part = Instantiate(wirePrefab, Vector3.Lerp(s, e, cable / length), Quaternion.Euler(90,-90,0), newWire.transform);
            part.transform.localScale = new Vector3(radius, stepLength/2, radius);
            part.name = "CablePart " + cable;
            var partRB = part.GetComponent<Rigidbody>();

            if (wireParts.Count > 0)
            {
                var oldPart = wireParts[wireParts.Count - 1];
                var oldHinge = oldPart.GetComponent<HingeJoint>();

                // Apply physics
                if (oldHinge && partRB)
                {
                    oldHinge.connectedBody = partRB;
                    partRB.isKinematic = false;
                }

                oldPart.transform.localRotation =
                    Quaternion.LookRotation(part.transform.position - oldPart.transform.position, Vector3.up);
                oldPart.transform.Rotate(new Vector3(-90,0,0));
            }
            else
            {
                if (partRB)
                    partRB.isKinematic = true;
            }
            
            wireParts.Add(part);
            cable += stepLength*0.95f;
        }

        //Remove last HingeJoint when we have more than one wire part.
        if (wireParts.Count > 1)
        {
            var last2 = wireParts[wireParts.Count - 2]; //.GetComponent<HingeJoint>();
            var last = wireParts[wireParts.Count - 1]; //.GetComponent<HingeJoint>();

            var lastRB = last.GetComponent<Rigidbody>();
            if (lastRB)
                lastRB.isKinematic = true;

            last.transform.localRotation =
                Quaternion.LookRotation(last.transform.position - last2.transform.position, Vector3.up);
            last.transform.Rotate(new Vector3(-90,0,0));
            Destroy(last);
        }

        //Lit the wire when we have at least one wire part.

        bool AnimateWires = false;
        if (wireParts.Count > 0 && AnimateWires)
        {
            litTime = flowTime / wireParts.Count;
            for (int i = 0; i < wireParts.Count; i++)
            {
                if(!broken && i % 3 == 0)
                    StartCoroutine(LitWire(wireParts, i));
            
                if(broken)
                    wireParts[i].GetComponent<Renderer>().material.color = Color.gray;
            }
        }
        
        return newWire.gameObject;
    }
}
