using UnityEngine;

public class DragSystem : MonoBehaviour, IDataPersistence
{
    Camera currentCamera;
    [Header("Objects", order = 0)]
    [Header("Screens", order = 1)]
    public GameObject popUpScreen;
    public GameObject quitScreen;
    [Space]
    public GameObject? currentlyDraging;
    public GameObject? currentlyHovering;
    [Space]
    public GameObject Placement;

    [Header("Teeth", order = 0)]
    public Teeth[] teeth;
    [Header("Placement", order = 0)]
    public Teeth[] placeholder;

    [Header("OffsetForMouse", order = 0)]
    public float offsetX = 0;
    public float offsetY = 0;
    public float OffsetZ;

    bool canMove = true;
    bool canHover = true;

    Vector3 rotState;
    Vector3 originalPos;
    [Space]
    public int solved = 0;

    private void Start()
    {
       /* for (int i = 0; i < teeth.Length; i++)
        {
            teeth[i].SetOriginalPosition();
            teeth[i].gameObject.transform.position = RandomPosition();
            teeth[i].gameObject.transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
        }*/
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 200f, ~(LayerMask.GetMask("Selected", "Invisible","Correct"))))
        {
            if (canHover)
            {
                if (hit.collider.gameObject != currentlyHovering && hit.collider.CompareTag("Selectable"))
                {
                    if (currentlyHovering != null)
                        currentlyHovering.layer = LayerMask.NameToLayer("Teeth");

                    currentlyHovering = hit.collider.gameObject;
                    currentlyHovering.layer = LayerMask.NameToLayer("Hover");
                }else if(!hit.collider.CompareTag("Selectable"))
                {
                    if (currentlyHovering != null)
                        currentlyHovering.layer = LayerMask.NameToLayer("Teeth");
                    currentlyHovering = null;
                }
            }
            else
            {
                if (hit.collider.gameObject != currentlyHovering && hit.collider.CompareTag("Position"))
                {
                    if (currentlyHovering != null)
                        currentlyHovering.layer = LayerMask.NameToLayer("Teeth");

                    currentlyHovering = hit.collider.gameObject;
                    currentlyHovering.layer = LayerMask.NameToLayer("PosHover");
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.CompareTag("Selectable"))
                {
                    currentlyDraging = hit.collider.gameObject;
                    currentlyDraging.transform.eulerAngles = new Vector3(0, 0, 0);
                    originalPos = currentlyDraging.transform.position;
                    currentlyDraging.layer = LayerMask.NameToLayer("Selected");
                    canHover = false;
                }
            }


            if (currentlyDraging != null && Input.GetMouseButtonUp(0))
            {

                switch(hit.collider.tag)
                {
                    case "Position":
                        if (CheckPos(currentlyDraging, hit.collider.gameObject))
                        {
                            SetPosition(currentlyDraging, hit.collider.gameObject.transform.position, new Vector3(0,0,0));
                            currentlyDraging.layer = currentlyHovering.layer = LayerMask.NameToLayer("Correct");
                            currentlyHovering = currentlyDraging =  null;
                            if (checkSolve())
                                PopUp(true);
                        }
                        else
                        {
                            SetPosition(currentlyDraging, originalPos);
                            currentlyDraging.layer = currentlyHovering.layer = LayerMask.NameToLayer("Teeth");
                            currentlyDraging = null;
                        }
                        canHover = true;
                        break;

                    case "Placement":
                        SetPosition(currentlyDraging, hit.point);
                        currentlyDraging.transform.rotation = Quaternion.FromToRotation(hit.transform.eulerAngles, hit.normal);
                        currentlyDraging.layer = LayerMask.NameToLayer("Teeth");
                        currentlyDraging = null;
                        canHover = true;
                        break;

                    default:
                        SetPosition(currentlyDraging, originalPos);
                       /* currentlyDraging.transform.position = originalPos;*/
                        currentlyDraging.layer = LayerMask.NameToLayer("Teeth");
                        currentlyDraging = null;
                        break;
                }

            }
        }
        else
        {
            if (currentlyHovering != null)
            {
                currentlyHovering.layer = LayerMask.NameToLayer("Teeth");
                currentlyHovering = null;
            }
        }

        if (currentlyDraging != null && canMove)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, currentCamera.WorldToScreenPoint(currentlyDraging.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

            RaycastHit hover;
            Ray rayh = currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayh, out hover, 200f, LayerMask.GetMask("Invisible")))
            {
                currentlyDraging.transform.position = new Vector3(worldPosition.x+ offsetX, worldPosition.y + offsetY, hover.point.z + OffsetZ);
            }
            else
            {
                currentlyDraging.transform.position = new Vector3(worldPosition.x + offsetX, worldPosition.y + offsetY, currentlyDraging.transform.position.z);
            }
        }


        if (Input.GetKey(KeyCode.R))
        {
            canMove = false;
            Cursor.lockState = CursorLockMode.Locked;
            currentlyDraging.transform.eulerAngles += 10f * new Vector3(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);

        }
        else if(Input.GetKeyUp(KeyCode.R))
        {
            Cursor.lockState = CursorLockMode.None;
            currentlyDraging.transform.eulerAngles = new Vector3(0, 0, 0);
            canMove = true;
        }



        if (Input.GetKey(KeyCode.Escape))
            OnQuit(true);
    }

    bool CheckPos(GameObject tooth, GameObject placement)
    {
        bool state = false;
        if(placement.gameObject.GetComponent<Teeth>() != null)
        {
            Teeth current = tooth.gameObject.GetComponent<Teeth>();
            Teeth place = placement.gameObject.GetComponent<Teeth>();

            if (current.side == place.side)
                if (current.orientation == place.orientation)
                    if (current.placement == place.placement)
                    {
                        current.isCorrect = true;
                        state = true;
                    }
        }
        return state;
    }
    Vector3 RandomPosition()
    {
        Vector3 m_Min, m_Max;
        Collider m_Collider = Placement.GetComponent<BoxCollider>();

        m_Min = m_Collider.bounds.min;
        m_Max = m_Collider.bounds.max;

        return new Vector3(Random.Range(m_Min.x, m_Max.x), Random.Range(m_Min.y, m_Max.y), m_Max.z);
    }


    void SetPosition(GameObject tooth, Vector3 Position, Vector3 Rotation)
    {
        tooth.transform.position = Position;
        tooth.transform.eulerAngles = Rotation;
    } 
    void SetPosition(GameObject tooth, Vector3 Position)
    {
        tooth.transform.position = Position;
    }

    public void Restart()
    {
        for (int i = 0; i < teeth.Length; i++)
        {
            if(teeth[i].isCorrect)
            {
                teeth[i].gameObject.transform.position = RandomPosition();
                teeth[i].isCorrect = placeholder[i].isCorrect = false;
                teeth[i].gameObject.layer = placeholder[i].gameObject.layer = LayerMask.NameToLayer("Teeth");
                teeth[i].gameObject.transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
               
            }
        }
        solved = 0;
    }

    public void Solve()
    {
        if (solved < teeth.Length)
        {
            if(teeth[solved].isCorrect)
                solved++;
            teeth[solved].isCorrect = placeholder[solved].isCorrect = true;
            teeth[solved].gameObject.layer = placeholder[solved].gameObject.layer = LayerMask.NameToLayer("Correct");
            teeth[solved].gameObject.transform.position = teeth[solved].pos;
            teeth[solved].gameObject.transform.eulerAngles = teeth[solved].rot;
            solved++;
        }
        else
        {
            PopUp(true);
        }
    }

    public bool checkSolve()
    {
        bool state = true;
        for (int i = 0; i < teeth.Length; i++)
        {
            if (!teeth[i].isCorrect)
            {
                state = false;
                break;
            }
        }
        return state;
    }

    public void PopUp(bool state)
    {
        popUpScreen.SetActive(state);
    }

    public void LoadData(GameData data)
    {
        solved = data.solvedCount;

        // Stupid check for first state
        // The tooth on last pos cant be in th right place

        if (data.positions[teeth.Length - 1] == Vector3.zero)
        {
            Restart();
        }
        else
        {
            for (int i = 0; i < teeth.Length; i++)
            {
                teeth[i].gameObject.transform.position = data.positions[i];
                if (data.teethState[i])
                {
                    teeth[i].gameObject.layer = placeholder[i].gameObject.layer = LayerMask.NameToLayer("Correct");
                }
                teeth[i].GetComponent<Teeth>().isCorrect = data.teethState[i];
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.solvedCount = solved;

        for (int i = 0; i < teeth.Length; i++)
        {
            data.positions[i] = teeth[i].gameObject.transform.position;
            data.teethState[i] = teeth[i].GetComponent<Teeth>().isCorrect;
        }
    }


    public void OnQuit(bool state)
    {
        quitScreen.SetActive(state);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
