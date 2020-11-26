/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
/// <summary>
/// AgentController turns agent commands into some action for the character.
/// </summary>
public class AgentController {
    public Character _Character;
    public Observation observation;

    /// <summary>
    /// Contstructor method for AgentController.
    /// </summary>
    public AgentController(Character character, Observation o)
    {
        _Character = character;
        observation = o;
    }

    /// <summary>
    /// Handle movement.
    /// </summary>
    public void Move(Vector3 direction) {
        _Character.Move(direction);
    }

    /// <summary>
    /// Handle the move toward command
    /// </summary>
    public void MoveToward(Vector3 direction, bool jump)
    {
        _Character.SetWantedTarget(direction);
        if (jump) _Character.MoveJump();
    }

    /// <summary>
    /// Apply an agent command to the character
    /// </summary>
    /// <returns>An observation after the action has been applied</returns>
    public Observation ProcessCommand(Command c, int gameTick, NavMeshContainer nav)
    {
        if (gameTick % 199 == 0)
        {
            _Character.SwitchCircleDirection();
        }

        switch (c.cmd)
        {
            case AgentCommandType.DONOTHING:
                break;
            case AgentCommandType.MOVETOWARD:
                AgentCommand<Tuple<Vector3, bool>> move = c as AgentCommand<Tuple<Vector3, bool>>;
                MoveToward(move.arg.object1, move.arg.object2);
                break;
            case AgentCommandType.INTERACT:
                AgentCommand interact = c as AgentCommand;
                string interactWith = interact.targetId;

                // only interact if: (1) the target can be interacted to, and (2) the character is
                // within the target colliding's bound:
                GameObject interactable = GameObject.Find(interactWith);
                if (interactable != null)
                {
                    // some game-objects like doors may have their collider(s) burried in a
                    // sub-component, so we should perhaps traverse the sub-components too.
                    // But since right now only switches can be interacted, and they have
                    // their collider at the top, we will just do this for now:
                    Collider targetCollider = interactable.GetComponent<Collider>();
                    if (targetCollider != null && _Character.GetComponent<CharacterController>().bounds.Intersects(targetCollider.bounds))
                    {
                        interactable.GetComponent<Interactable>()?.Interact(_Character);
                    }
                }
                break;
        }

        observation.Observe(_Character, gameTick, nav, c.cmd);
        return observation;
    }
}