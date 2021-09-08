using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines which wall side is to be displayed for the maze
/// </summary>
[Flags]
public enum WallState
{
    /// <summary>
    /// Binary representation of 0001 to indicate that the Upper wall should be displayed
    /// </summary>
    UP = 1,

    /// <summary>
    /// Binary representation of 0010 to indicate that the Lower wall should be displayed
    /// </summary>
    DOWN = 2,

    /// <summary>
    /// Binary representation of 0100 to indicate that the left wall should be displayed
    /// </summary>
    LEFT = 4,

    /// <summary>
    /// Binary representation of 1000 to indicate that the right wall should be displayed
    /// </summary>
    RIGHT = 8,

    /// <summary>
    /// Binary representation of 1000 0000 this way the wall visited status of the cell
    /// </summary>
    VISITED = 128           //This will not affect the wall side flags but I can use it to mark a cell side as visited

}

/// <summary>
/// Defines the position of the cell in the maze
/// </summary>
public struct CellPosition
{
    public int x;
    public int y;
}

/// <summary>
/// This degfines the neighbor as its position and the shared wall it has with the current cell
/// </summary>
public struct CellNeighbor
{
    public CellPosition position;
    public WallState sharedWall;
}

public static class MazeBuilder
{
    #region Supporting Functions

    /// <summary>
    /// Builds the maze using the recursive backtracking algorithm
    /// </summary>
    /// <param name="width">The width of the maze</param>
    /// <param name="height">The height of the maze</param>
    /// <returns>The maze data</returns>
    public static WallState[,] BuildMaze(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];

        //initialize the maze with all 1s to build all the walls
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                //This line ORs all the binary representations of the four sides to result 1111 to indicate that all walls should be displayed
                maze[i, j] = WallState.UP | WallState.DOWN | WallState.LEFT | WallState.RIGHT;
            }
        }

        //After initializeing the maze, we need to pick a random cell in the maze to start backtracking from
        var randomCell = new System.Random();
        CellPosition pos = new CellPosition { x = randomCell.Next(0, width), y = randomCell.Next(0, height) };

        //Creating a stack that will hold the visited cells
        Stack<CellPosition> cellPosStack = new Stack<CellPosition>();

        //We need to keep track of the visited cells and mark them as we traverse through the cells....
        //Marking the first cell as visited
        maze[pos.x, pos.y] |= WallState.VISITED;

        //Adding the visited position to the stack
        cellPosStack.Push(pos);

        //Here we start the backtracking and keep selecting random neighbors of the current cell
        //if the random neighbor was not visited before, the shared wall between the current cell and the neighbor cell is removed, then the position of the neighbor is added to the stack and we loop again on the selected neighbor
        while (cellPosStack.Count > 0)
        {
            //Poping the cell on top of the stack
            CellPosition currentCell = cellPosStack.Pop();

            //We need to get a list of all the surrounding neighbor cells to the current cell
            List<CellNeighbor> neighbors = GetAllNeighborCells(currentCell, maze, width, height);

            //If we find unvisited neighbors of the current cell
            if (neighbors.Count > 0)
            {
                //Return the current cell to the stack
                cellPosStack.Push(currentCell);

                //We select a random neighbor to go to from the list
                var index = randomCell.Next(0, neighbors.Count);
                CellNeighbor randomNeighbor = neighbors[index];

                //We need to mark the position of that neighbor as visited and remove the shared wall between the neighbor and the current cell;
                CellPosition neighborPos = randomNeighbor.position;
                maze[currentCell.x, currentCell.y] &= ~randomNeighbor.sharedWall;
                maze[neighborPos.x, neighborPos.y] |= WallState.VISITED;

                //We also need to remove the neighbor's shared wall with respect to the current cell.
                //The neighbot's wall that should be removed will always be the opposite wall to the shared wall between the current and the neighbor
                maze[neighborPos.x, neighborPos.y] &= ~GetOppositeWall(randomNeighbor.sharedWall);

                //Then we push the random neighbor's position into the stack
                cellPosStack.Push(neighborPos);
            }
        }

        return maze;
    }

    /// <summary>
    /// Gets the opposite wall of the current wall
    /// </summary>
    /// <param name="wall">The current wall</param>
    /// <returns>The opposite wall to the current wall</returns>
    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.DOWN:    return WallState.UP;
            case WallState.UP:      return WallState.DOWN;
            case WallState.LEFT:    return WallState.RIGHT;
            case WallState.RIGHT:   return WallState.LEFT;
            default:                return WallState.LEFT;
        }
    }

    /// <summary>
    /// Returns a list of all the neighbor cells to a certain cell that are not visited before
    /// </summary>
    /// <param name="cell">The cell to fine its neighbors</param>
    /// <param name="maze">The big maze</param>
    /// <param name="width">The width of the maze</param>
    /// <param name="height">The height of the maze</param>
    /// <returns>A list containing all the neighbor cells to the current cell.</returns>
    private static List<CellNeighbor> GetAllNeighborCells(CellPosition cell, WallState[,] maze, int width, int height)
    {
        List<CellNeighbor> temp = new List<CellNeighbor>();

        //Check for the left
        if (cell.x > 0)
        {
            //We only add the cells that are NOT visited before
            if (!maze[cell.x -1, cell.y].HasFlag(WallState.VISITED))
            {
                CellNeighbor n = new CellNeighbor
                {
                    position = new CellPosition { x = cell.x - 1, y = cell.y },
                    sharedWall = WallState.LEFT
                };


                temp.Add(n);
            }
        }

        //Check for the right
        if (cell.x < width - 1)
        {
            //We only add the cells that are NOT visited before
            if (!maze[cell.x + 1, cell.y].HasFlag(WallState.VISITED))
            {
                CellNeighbor n = new CellNeighbor
                {
                    position = new CellPosition { x = cell.x + 1, y = cell.y },
                    sharedWall = WallState.RIGHT
                };

                temp.Add(n);
            }
        }

        //Check for the up
        if (cell.y < height - 1)
        {
            //We only add the cells that are NOT visited before
            if (!maze[cell.x, cell.y + 1].HasFlag(WallState.VISITED))
            {
                CellNeighbor n = new CellNeighbor
                {
                    position = new CellPosition { x = cell.x, y = cell.y + 1 },
                    sharedWall = WallState.UP
                };


                temp.Add(n);
            }
        }

        //Check for the down
        if (cell.y > 0)
        {
            //We only add the cells that are NOT visited before
            if (!maze[cell.x, cell.y - 1].HasFlag(WallState.VISITED))
            {
                CellNeighbor n = new CellNeighbor
                {
                    position = new CellPosition { x = cell.x, y = cell.y - 1 },
                    sharedWall = WallState.DOWN
                };


                temp.Add(n);
            }
        }

        return temp;
    }

    #endregion
}
