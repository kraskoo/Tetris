using UnityEngine;

using Random = System.Random;

public class Spawner : MonoBehaviour
{
    // ReSharper disable once StyleCop.SA1401
    public GameObject[] Groups;

    private static readonly Random Random = new Random();

    private GameObject nextFigure;

    // Start is called before the first frame update
    public void Start()
    {
        var audioSources = GameObject.FindObjectsOfType<AudioSource>();
        Common.RotateSound = audioSources[0];
        Common.HitSound = audioSources[1];
        Common.LineRemovalSound = audioSources[2];
        Common.GameMusic = audioSources[3];
        Common.SetupOptions();
        Common.IncreaseByHiddenLevel();
        if (!Common.IsRunning)
        {
            Common.IsRunning = !Common.IsRunning;
        }

        this.SpawnNext();
    }

    public void SpawnNext()
    {
        if (this.nextFigure == null)
        {
            this.SetNextFigure();
            var current = this.InnerSpawn();
            current.transform.position = this.transform.position;
            current.GetComponent<MonoBehaviour>().enabled = true;
        }
        else
        {
            var current = this.nextFigure;
            current.transform.position = this.transform.position;
            current.GetComponent<MonoBehaviour>().enabled = true;
            this.SetNextFigure();
        }
    }

    private void SetNextFigure()
    {
        this.nextFigure = this.InnerSpawn();
    }

    private GameObject InnerSpawn()
    {
        var figure = Behaviour.Instantiate(
            this.Groups[Random.Next(0, this.Groups.Length)],
            new Vector3(14, 11, 1),
            Quaternion.identity);
        figure.GetComponent<MonoBehaviour>().enabled = false;
        var rotates = Random.Next(0, 4);
        for (int i = 0; i < rotates; i++)
        {
            figure.RotateF();
        }

        return figure;
    }
}
