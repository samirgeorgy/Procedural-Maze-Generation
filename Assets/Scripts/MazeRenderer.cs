using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeRenderer : MonoBehaviour
{
    #region Private Variables

    [SerializeField] private int _mazeWidth = 10;               //The width of the maze
    [SerializeField] private int _mazeHeight = 10;              //The height of the maze
    [SerializeField] private float _hallwaySize = 2;            //The size of the hallways of the maze
    [SerializeField] private GameObject _wallPrefab;            //The wall prefab
    [SerializeField] private GameObject _groundPrefab;          //The ground prefab
    
    private NavMeshSurface surface;                             //The navigation surface of the maze

    private WallState[,] _mazeData;                             //An array containing the maze's wall placement data

    #endregion

    #region Unity Functions

    // Start is called before the first frame update
    void Start()
    {
        //Generating the maze data
        _mazeData = MazeBuilder.BuildMaze(_mazeWidth, _mazeHeight);

        //Rendering the maze
        RenderMaze();

        //Generating the nav mesh for the maze
        surface = GameObject.FindGameObjectWithTag("Floor").GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    #endregion

    #region Supporting Functions

    /// <summary>
    /// This function renders the maze based on the data generated from the algorithm
    /// </summary>
    private void RenderMaze()
    {
        //Creating the floor and matching it with the maze size
        GameObject floor = Instantiate(_groundPrefab, this.transform);
        floor.transform.localScale = new Vector3(_mazeWidth/2, 0, _mazeHeight/2);

        //Start rendering the maze
        for (int i = 0; i < _mazeWidth; ++i)
        {
            for (int j = 0; j < _mazeHeight; ++j)
            {
                //The cell data
                WallState cell = _mazeData[i, j];

                //The cell's position in the maz
                Vector3 position = new Vector3(-_mazeWidth / 2 + i, 0, -_mazeHeight / 2 + j);

                //Here we check if the upper side of the wall is in the data
                if (cell.HasFlag(WallState.UP))
                {
                    //Instatiate the prefab
                    GameObject upperWall = Instantiate(_wallPrefab, this.transform);

                    //Here we place the upper wall in the upper position of the cell's center
                    upperWall.transform.position = position + new Vector3(0, 0, _hallwaySize / 2);

                    //Adjusting the scale of the wall to accomodate the maze's hallway size
                    upperWall.transform.localScale = new Vector3(_hallwaySize + 0.2f, upperWall.transform.localScale.y, upperWall.transform.localScale.z);
                }

                //We do the same for the left
                if (cell.HasFlag(WallState.LEFT))
                {
                    //Instatiate the prefab
                    GameObject leftWall = Instantiate(_wallPrefab, this.transform);

                    //Here we place the left wall in the left position of the cell's center
                    leftWall.transform.position = position + new Vector3(-_hallwaySize / 2, 0, 0);

                    //Adjusting the scale of the wall to accomodate the maze's hallway size
                    leftWall.transform.localScale = new Vector3(_hallwaySize + 0.2f, leftWall.transform.localScale.y, leftWall.transform.localScale.z);

                    //Rotating the wall 90 degrees
                    leftWall.transform.localEulerAngles = new Vector3(0, 90, 0);
                }

                //Handling the case for the far right side of the maze
                if (i == _mazeWidth - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        GameObject rightWall = Instantiate(_wallPrefab, this.transform);
                        rightWall.transform.position = position + new Vector3(_hallwaySize / 2, 0, 0);
                        rightWall.transform.localScale = new Vector3(_hallwaySize + 0.2f, rightWall.transform.localScale.y, rightWall.transform.localScale.z);
                        rightWall.transform.localEulerAngles = new Vector3(0, 90, 0);
                    }
                }

                //handing the lower side of the maze
                if (j == 0)
                {
                    //Instatiate the prefab
                    GameObject lowerWall = Instantiate(_wallPrefab, this.transform);

                    //Here we place the upper wall in the upper position of the cell's center
                    lowerWall.transform.position = position + new Vector3(0, 0, -_hallwaySize / 2);

                    //Adjusting the scale of the wall to accomodate the maze's hallway size
                    lowerWall.transform.localScale = new Vector3(_hallwaySize + 0.2f, lowerWall.transform.localScale.y, lowerWall.transform.localScale.z);
                }
            }
        }

        //Offsetting the mesh to start at point 0, 0, 0
        this.transform.position = new Vector3(_mazeWidth / 2, 0, _mazeHeight / 2);
    }

    #endregion
}
