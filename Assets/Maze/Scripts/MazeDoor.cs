using UnityEngine;
using System.Collections;

public class MazeDoor : MazePassage {
    public Transform hinge;
    private int state = 0;
    private static Quaternion normalRotation = Quaternion.Euler(0f, 180f, 0f),
                              mirroredRotation = Quaternion.Euler(0f, -180f, 0f);
    private bool isMirrored = false;

    private MazeDoor OtherSideOfDoor
    {
        get
        {
            return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor;
        }
    }
    
    public override void Initialize(MazeCell primary, MazeCell other, MazeDirection direction)
    {
        base.Initialize(primary, other, direction);
        if(OtherSideOfDoor != null)
        {
            isMirrored = true;
            hinge.localScale = new Vector3(-1f, 1f, 1f);
            Vector3 p = hinge.localPosition;
            p.x = -p.x;
            hinge.localPosition = p;
        }

        for(int i=0; i<transform.childCount;i++)
        {
            Transform child = transform.GetChild(i);
            if(child != hinge)
            {
                child.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
            }
        }
    }

    public override void OnPlayerEnter()
    {
        //OtherSideOfDoor.hinge.localRotation = hinge.localRotation = Quaternion.Lerp(hinge.localRotation, Quaternion.Euler(0f, -90f, 0f), Time.deltaTime * 5f);
        state = 1;
        OtherSideOfDoor.cell.room.Show();
    }

    public override void OnPlayerExit()
    {
        //OtherSideOfDoor.hinge.localRotation = hinge.localRotation = Quaternion.identity;
        state = 0;
        OtherSideOfDoor.cell.room.Hide();
    }

    public void Update()
    {
        if(state == 1)
        {
            OtherSideOfDoor.hinge.localRotation = hinge.localRotation = Quaternion.Lerp(hinge.localRotation, (isMirrored?mirroredRotation:normalRotation), Time.deltaTime * 5f);
        } else
        {
            OtherSideOfDoor.hinge.localRotation = hinge.localRotation = Quaternion.Lerp(hinge.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }
}
