using UnityEngine;

/*
 * Logic for trigerring and updating score.
 */
[RequireComponent(typeof(Collider))]
public class Scoring : MonoBehaviour
{

    public Scoring() { }

    /*
     * This should be invoked when the game-object that owns this Scoring component
     * is interacted to by a Character. The method will then check if the game-object
     * is eligible for first-time turn-on bonus.
     */ 
    public void InteractionBonus(Character whoDidIt)
    {
        string oname = this.gameObject.name;
        if (this.gameObject.tag == "Switch" && !whoDidIt.hasReceivedTurnedOnBonus(oname))
        {
            Toggleable tg = this.gameObject.GetComponent<Toggleable>();
            // tg should not be null, but any way:
            if (tg != null && tg.isActive)
            {
                int scoreGained = 10; // first time visiting a turned-on door --> bonus!
                whoDidIt.registerVisitedTurnedOnSwitch(oname);
                string[] moods =
                    {
                       "Hmm...",
                       "What was that?",
                       "Snap!"
                    };
                whoDidIt.SetMood(moods[Random.Range(0, moods.Length)]);
                whoDidIt.Score += scoreGained;
                AudioSource sound = this.gameObject.GetComponent<AudioSource>();
                sound.Play(0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Character>(out var character))
        {
            string oname = this.gameObject.name;
            //Debug.Log(">>> objname: " + oname);
            int scoreGained = 0;
            switch (this.gameObject.tag)
            {
                case "Switch":
                    if (character.hasBeenVisited(oname)) return;
                    scoreGained = 1; // else first time visiting the door --> +1
                    break;
                case "Goal":
                    if (character.hasBeenVisited(oname)) return;

                    if (this.gameObject.name == "Finish")
                    {
                        scoreGained = 500;
                        character.SetMood("Finish!");
                    }
                    else
                    {
                        scoreGained = 100;
                        character.SetMood("YES!");
                    }

                    // heal the character too:
                    character.Health = character.maxHealth ;
                    AudioSource sound = this.gameObject.GetComponent<AudioSource>();
                    sound.Play(0);
                    break;
            }
            character.registerVisitedGameObject(oname);
            character.Score += scoreGained ; 
        }
    }
}
