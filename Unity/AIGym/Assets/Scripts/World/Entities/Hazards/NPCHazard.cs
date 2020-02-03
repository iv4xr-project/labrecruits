/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHazard : Hazard
{
    float time = -1; //time used by patrol func
    Vector3 direction; //movement direction

    public int moveType;

    public override void UpdateHazard()
    {
        switch (moveType)
        {
            case 1:
                MoveRandom();
                break;

            case 2:
                MovePatrol(5);
                break;
        }
    }

    /// <summary>
    /// Move in a random horizontal direction
    /// </summary>
    void MoveRandom()
    {
        direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        Move(direction, 1);
    }

    /// <summary>
    /// Patrol in a random direction
    /// </summary>
    /// <param name="interval">Time spent moving in one direction</param>
    void MovePatrol(float interval)
    {
        if (time == -1)
        {
            while(direction == new Vector3())
            {
                direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            }
            time = 0;
        }
        else if (time >= interval)
        {
            direction = -direction;
            time = 0;
        }
        else
        {
            time += 0.1f; // TODO use env config here
        }

        Move(direction, 1);
    }
}
