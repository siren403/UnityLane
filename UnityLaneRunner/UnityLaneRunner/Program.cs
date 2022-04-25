using UnityLaneRunner;
using static System.Console;
using static UnityLaneRunner.Runner;

Run<Lane>(args);
public class Lane : LaneBase
{
    [Lane]
    public void GooglePlay()
    {
        WriteLine("In GooglePlay");
    }
    
}