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
                GameObject.Find(interactWith).GetComponent<Interactable>().Interact();
                break;
        }

        observation.Observe(_Character, gameTick, nav, c.cmd);
        return observation;
    }
}