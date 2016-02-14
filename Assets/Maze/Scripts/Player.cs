using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    private MazeCell currentCell;
    private MazeDirection faceDirection;

    public void SetLocation(MazeCell cell)
    {
        currentCell = cell;
        cell.room.Show();
        transform.localPosition = cell.transform.localPosition;
    }

    private void Move(MazeDirection direction)
    {
        MazeCellEdge edge = currentCell.GetEdge(direction);
        if(edge is MazePassage)
        {
            SetLocation(edge.otherCell);
        }
    }
    
    private void Update()
    {
        //Camera.main.transform.SetParent(transform);
        Vector3 camPos = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
        Camera.main.transform.position = camPos;
    }

    public float pushPower = 2.0f;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        MazeCell cell = hit.gameObject.GetComponentInParent<MazeCell>();
        if (cell == null)
            return;
        
        if(cell != currentCell)
        {
            if(currentCell != null)
                currentCell.OnPlayerExit();

            cell.OnPlayerEnter();
        }

        currentCell = cell;
        Vector3 transformDirection = new Vector3(Mathf.Round(transform.forward.x), 0f, Mathf.Round(transform.forward.z));
        IntVector2 direction = new IntVector2((int) transformDirection.x, (int) transformDirection.z);

        if(direction.x != 0 && direction.z != 0)
        {
            if (Random.Range(0, 1) == 1)
                direction.x = 0;
            else
                direction.z = 0;
        }

        MazeDirection mazeDirection = MazeDirections.FromIntVector2(direction);
        faceDirection = mazeDirection;
        
    }
}
