using UnityEngine;

public class Group : MonoBehaviour
{
    private const int MaxHorizontalCount = 4;
    private const float MaxFallSpeed = 10f;

    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private bool isDownPressed = false;

    private int horizontalCounter = 0;

    private float fallSpeed = 1.5f;
    private float lastFallSpeed = 1.5f;
    private float lastFall = 0;

    public static AudioSource LineRemoval { get; private set; }

    public AudioSource Rotate { get; private set; }

    public AudioSource Hit { get; private set; }

    public void Start()
    {
        var audioSources = GameObject.FindObjectsOfType<AudioSource>();
        this.Rotate = audioSources[0];
        this.Hit = audioSources[1];
        LineRemoval = audioSources[2];

        if (!this.IsValidGridPos())
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
            if (this.horizontalCounter % MaxHorizontalCount == 0)
            {
                this.PressLeft();
                this.horizontalCounter = 0;
            }
        }
        
        if (this.isRightPressed)
        {
            this.horizontalCounter++;
            if (this.horizontalCounter % MaxHorizontalCount == 0)
            {
                this.PressRight();
                this.horizontalCounter = 0;
            }
        }
        
        if (this.isDownPressed)
        {
            this.lastFallSpeed = this.fallSpeed;
            this.fallSpeed = MaxFallSpeed;
            this.PressDown();
        }
        else
        {
            this.fallSpeed = this.lastFallSpeed;
        }
    }

    public bool IsValidGridPos()
    {
        foreach (Transform child in this.transform)
        {
            var v = Playfield.RoundVec2(child.position);

            // Not inside Border?
            if (!Playfield.InsideBorder(v))
            {
                return false;
            }

            // Block in grid cell (and not part of same group)?
            if (Playfield.Grid[(int)v.x, (int)v.y] != null &&
                Playfield.Grid[(int)v.x, (int)v.y].parent != this.transform)
            {
                return false;
            }
        }

        return true;
    }

    private void Restart()
    {
        int nulls = 0;
        for (int x = 0; x < Playfield.W; x++)
        {
            if (Playfield.Grid[x, 0] == null)
            {
                nulls++;
            }
        }

        // Check if no one field in grid of first row, then there will be no need to restart
        if (nulls == Playfield.W || this.IsValidGridPos())
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
    }

    private void PressDown()
    {
        // Modify position
        this.transform.position += new Vector3(0, -1, 0);

        // See if valid
        if (this.IsValidGridPos())
        {
            // It's valid. Update grid.
            this.UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            this.transform.position += new Vector3(0, 1, 0);
            this.Hit.Play();

            // Clear filled horizontal lines
            Playfield.DeleteFullRows();

            // Spawn next Group
            GameObject.FindObjectOfType<Spawner>().SpawnNext();

            // Disable script
            this.enabled = false;
        }

        this.lastFall = Time.time;
    }

    private void PressUp()
    {
        this.transform.Rotate(0, 0, -90);

        // See if valid
        if (this.IsValidGridPos())
        {
            // It's valid. Update grid.
            this.UpdateGrid();
            this.Rotate.Play();
        }
        else
        {
            // It's not valid. revert.
            this.transform.Rotate(0, 0, 90);
        }
    }

    private void PressRight()
    {
        // Modify position
        this.transform.position += new Vector3(1, 0, 0);

        // See if valid
        if (this.IsValidGridPos())
        {
            // It's valid. Update grid.
            this.UpdateGrid();
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
        if (this.IsValidGridPos())
        {
            // It's valid. Update grid.
            this.UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            this.transform.position += new Vector3(1, 0, 0);
        }
    }

    private void UpdateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < Playfield.H; ++y)
        {
            for (int x = 0; x < Playfield.W; ++x)
            {
                if (Playfield.Grid[x, y] != null)
                {
                    if (Playfield.Grid[x, y].parent == this.transform)
                    {
                        Playfield.Grid[x, y] = null;
                    }
                }
            }
        }

        // Add new children to grid
        foreach (Transform child in this.transform)
        {
            var v = Playfield.RoundVec2(child.position);
            Playfield.Grid[(int)v.x, (int)v.y] = child;
        }
    }
}