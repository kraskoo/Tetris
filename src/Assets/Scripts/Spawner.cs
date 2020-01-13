using UnityEngine;

public class Spawner : MonoBehaviour
{
    // ReSharper disable once StyleCop.SA1401
    public static bool IsRunning = true;

    // ReSharper disable once StyleCop.SA1307
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once StyleCop.SA1401
    public GameObject[] groups;

    private static readonly System.Random Random = new System.Random();

    // Start is called before the first frame update
    public void Start()
    {
        this.SpawnNext();
    }

    public void SpawnNext()
    {
        var figure = GameObject.Instantiate(
            this.groups[Random.Next(0, this.groups.Length)],
            this.transform.position,
            Quaternion.identity);
        var rotates = Random.Next(0, 4);
        for (int i = 0; i < rotates; i++)
        {
            figure.transform.Rotate(0, 0, -90);
        }
    }
}
