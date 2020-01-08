using UnityEngine;

public class Spawner : MonoBehaviour
{
    // ReSharper disable once StyleCop.SA1401
    public static bool IsRunning = true;

    // ReSharper disable once StyleCop.SA1307
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once StyleCop.SA1401
    public GameObject[] groups;

    // Start is called before the first frame update
    public void Start()
    {
        this.SpawnNext();
    }

    public void SpawnNext()
    {
        GameObject.Instantiate(
            this.groups[Random.Range(0, this.groups.Length)],
            this.transform.position,
            Quaternion.identity);
    }
}
