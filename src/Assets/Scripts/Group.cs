using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Group : MonoBehaviour
{
    private bool isDownPressed = false;
    private float fallSpeed = Constants.InitialFallSpeed;
    private float lastFallSpeed = Constants.InitialFallSpeed;
    private float lastFall = 0;
    private float sideMovePeriodTime = 0f;
    private float periodDistance = 0.300f;
    private float maxAccelerationDistance = 0.16f;
    private float accelerationDistance = 0f;

    public void Start()
    {
        this.sideMovePeriodTime = Time.time + (this.periodDistance - (((36f + Results.Level) / 100) * this.periodDistance) - this.accelerationDistance);
        if (!this.gameObject.IsValidGridPos())
        {
            Debug.Log("GAME OVER");
            Common.IsRunning = false;
            Common.IsGameStarted = false;
            SceneManager.LoadScene(3);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Common.IsLeftPressed = false;
            Common.IsRightPressed = false;
            Common.IsRunning = false;
            FindObjectsOfType<GameObject>().ToList().ForEach(go => go.SetActive(false));
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Common.IsLeftPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Common.IsLeftPressed = false;
            this.accelerationDistance = 0f;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Common.IsRightPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            Common.IsRightPressed = false;
            this.accelerationDistance = 0f;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.PressUp();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.isDownPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            this.isDownPressed = false;
        }
        
        this.fallSpeed = this.lastFallSpeed = Constants.InitialFallSpeed + (Constants.IncreasePerLevel * Results.Level);
        if (Time.time - this.lastFall >= 1 / this.fallSpeed)
        {
            this.PressDown();
        }
    }

    public void FixedUpdate()
    {
        if (!Common.IsRunning)
        {
            return;
        }

        if (Common.IsLeftPressed && !Common.IsRightPressed)
        {
            if (Time.time >= this.sideMovePeriodTime)
            {
                this.PressLeft();
                this.sideMovePeriodTime = Time.time + (this.periodDistance - (((36f + Results.Level) / 100) * this.periodDistance) - this.accelerationDistance);
                if (this.accelerationDistance < this.maxAccelerationDistance)
                {
                    this.accelerationDistance += 0.04f;
                }
            }
        }
        
        if (Common.IsRightPressed && !Common.IsLeftPressed)
        {
            if (Time.time >= this.sideMovePeriodTime)
            {
                this.PressRight();
                this.sideMovePeriodTime = Time.time + (this.periodDistance - (((36f + Results.Level) / 100) * this.periodDistance) - this.accelerationDistance);
                if (this.accelerationDistance < this.maxAccelerationDistance)
                {
                    this.accelerationDistance += 0.04f;
                }
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
            var lowestY =
                (from Transform child in this.gameObject.transform
                 select child.transform.position.y).Min() + 2f;
            Common.HitSound.Play();
            Results.Score += (int)(Constants.MaxAdditionalPoints - Mathf.Round(lowestY));

            // It's not valid. revert.
            this.transform.position += new Vector3(0, 1, 0);

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