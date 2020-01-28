using System.Linq;
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
        Cursor.visible = false;
        var audioSources = GameObject.FindObjectsOfType<AudioSource>();
        Common.RotateSound = audioSources[0];
        Common.HitSound = audioSources[1];
        Common.LineRemovalSound = audioSources[2];
        Common.GameMusic = audioSources[3];
        Common.SetupOptions();
        Common.SetByHiddenLevel();
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
            var maxY = (from Transform child in current.transform select child.position.y).Max();
            Debug.Log(maxY);
            if (maxY > 20f)
            {
                var diff = maxY - 20f;
                current.transform.position = new Vector3(current.transform.position.x, current.transform.position.y - diff, current.transform.position.z);
            }
            else if (maxY < 20f)
            {
                var diff = 20f - maxY;
                current.transform.position = new Vector3(current.transform.position.x, diff + current.transform.position.y, current.transform.position.z);
            }

            current.GetComponent<MonoBehaviour>().enabled = true;
        }
        else
        {
            var current = this.nextFigure;
            current.transform.position = this.transform.position;
            var maxY = (from Transform child in current.transform select child.position.y).Max();
            Debug.Log(maxY);
            if (maxY > 20f)
            {
                var diff = maxY - 20f;
                current.transform.position = new Vector3(current.transform.position.x, current.transform.position.y - diff, current.transform.position.z);
            }
            else if (maxY < 20f)
            {
                var diff = 20f - maxY;
                current.transform.position = new Vector3(current.transform.position.x, diff + current.transform.position.y, current.transform.position.z);
            }

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
            new Vector3(17, 14, 1),
            Quaternion.identity);
        figure.GetComponent<MonoBehaviour>().enabled = false;
        var rotates = Random.Next(0, 4);
        for (int i = 0; i < rotates; i++)
        {
            figure.RotateF();
        }

        figure.CorrectPositionToBorders(
            t => t.position.y,
            (oldP, diff) => new Vector3(oldP.x, oldP.y + diff, oldP.z),
            (oldP, diff, max) => new Vector3(oldP.x, oldP.y - (diff - max), oldP.z),
            14f,
            18f);
        figure.CorrectPositionToBorders(
            t => t.position.x,
            (oldP, diff) => new Vector3(oldP.x + diff, oldP.y, oldP.z),
            (oldP, diff, max) => new Vector3(oldP.x - (diff - max), oldP.y, oldP.z),
            17f,
            21f);
        return figure;
    }
}
