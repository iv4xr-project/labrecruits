/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

public class EnvironmentConfig
{
    public int seed = 1;

    public string level_path = "";
    public string level_name = "";

    public float agent_speed = 0.13f;
    public float npc_speed = 0.11f;
    public float fire_spread = 0.02f;
    public float jump_force = 0.18f;
    public float view_distance = 10f;
    public float light_intensity = 1f;

    public Tuple<string, string>[] add_links = new Tuple<string, string>[0];
    public Tuple<string, string>[] remove_links = new Tuple<string, string>[0];

    public override string ToString() => string.Format("{0} with {2} as seed, at location {1}.", level_name, level_path, seed);

    public static EnvironmentConfig DEFAULT => new EnvironmentConfig();
}
