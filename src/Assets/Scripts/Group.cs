using UnityEngine;

public class Group : MonoBehaviour
{
    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private bool isDownPressed = false;

    private int horizontalCounter = 0;

    private float fallSpeed = Constants.InitialFallSpeed;
    private float lastFallSpeed = Constants.InitialFallSpeed;
    private float lastFall = 0;

    public static AudioSource LineRemoval { get; private set; }

    public static AudioSource Rotate { get; private set; }

    public AudioSource Hit { get; private set; }

    public void Start()
    {
        var audioSources = GameObject.FindObjectsOfType<AudioSource>();
        Rotate = audioSources[0];
        this.Hit = audioSources[1];
        LineRemoval = audioSources[2];

        if (!this.gameObject.IsValidGridPos())
        {
            Debug.Log("GAME OVER");
            Spawner.IsRunning = false;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Spawner.IsRunning = !Spawner.IsRunning;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit(0);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.R))
        {
            this.Restart();
        }

        if (!Spawner.IsRunning)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.isLeftPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            this.isLeftPressed = false;
            this.horizontalCounter = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.isRightPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            this.isRightPressed = false;
            this.horizontalCounter = 0;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.PressUp();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.isDownPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            this.isDownPressed = false;
        }

        this.fallSpeed = this.lastFallSpeed = Constants.InitialFallSpeed + (0.25f * Results.Level);
        if (Time.time - this.lastFall >= 1 / this.fallSpeed)
        {
            this.PressDown();
        }
    }

    public void FixedUpdate()
    {
        if (!Spawner.IsRunning)
        {
            return;
        }

        if (this.isLeftPressed)
        {
            this.horizontalCounter++;
            if (this.horizontalCounter % Constants.MaxHorizontalCount == 0)
            {
                this.PressLeft();
                this.horizontalCounter = 0;
            }
        }
        
        if (this.isRightPressed)
        {
            this.horizontalCounter++;
            if (this.horizontalCounter % Constants.MaxHorizontalCount == 0)
            {
                this.PressRight();
                this.horizontalCounter = 0;
            }
        }
        
        if (this.isDownPressed)
        {
            this.lastFallSpeed = this.fallSpeed;
            this.fallSpeed = Constants.MaxFallSpeed;
            this.PressDown();
        }
        else
        {
            this.fallSpeed = this.lastFallSpeed;
        }
    }

    private void Restart()
    {
        if (this.gameObject.IsValidGridPos())
        {
            return;
        }

        Spawner.IsRunning = false;
        for (int x = 0; x < Playfield.W; x++)
        {
            for (int y = 0; y < Playfield.H; y++)
            {
                if (Playfield.Grid[x, y] != null)
                {
                    GameObject.Destroy(Playfield.Grid[x, y].gameObject);
                    Playfield.Grid[x, y] = null;
                }
            }
        }

        Spawner.IsRunning = true;
        Results.Level = 1;
        Results.Score = 0;
    }

    private void PressDown()
    {
        // Modify position
        this.transform.position += new Vector3(0, -1, 0);

        // See if valid
        if (this.gameObject.IsValidGridPos())
        {
            // It's valid. Update grid.
            this.gameObject.UpdateGrid();
        }
        else
        {
            var lastY = this.transform.localPosition.y;
            if (lastY > 0)
            {
                lastY = 0;
            }

            // It's not valid. revert.
            this.transform.position += new Vector3(0, 1, 0);
            this.Hit.Play();
            Results.Score += (int)(Constants.MaxAdditionalPoints - lastY);

            // Clear filled horizontal lines
            Playfield.DeleteFullRows();

            // Spawn next Group
            GameObject.FindObjectOfType<Spawner>().SpawnNext();

            // Disable script
            this.enabled = false;
        }

        this.lastFall = Time.time;
    }

    private void PressUp() => this.gameObject.RotateFigure();

    private void PressRight()
    {
        // Modify position
        this.transform.position += new Vector3(1, 0, 0);

        // See if valid
        if (this.gameObject.IsValidGridPos())
        {
            // It's valid. Update grid.
            this.gameObject.UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            this.transform.position += new Vector3(-1, 0, 0);
        }
    }

    private void PressLeft()
    {
        // Modify position
        this.transform.position += new Vector3(-1, 0, 0);

        // See if valid
        if (this.gameObject.IsValidGridPos())
        {
            // It's valid. Update grid.
            this.gameObject.UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            this.transform.position += new Vector3(1, 0, 0);
        }
    }
}