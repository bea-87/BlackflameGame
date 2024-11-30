using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHolderObjectParent {

    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedObjectChangedEventArgs> OnSelectedObjectChanged;
    public class OnSelectedObjectChangedEventArgs : EventArgs {
        public ISelectableObject selectedObject;
    }

    public event EventHandler OnStateChanged;
    public event EventHandler<OnTrialActivatedEventArgs> OnTrialActivated;
    public class OnTrialActivatedEventArgs : EventArgs {
        public bool active;
    }
    


    [SerializeField] private GameInput gameInput;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private Transform holderObjectHoldPoint;
    [SerializeField] private LayerMask objectsLayerMask;
    [SerializeField] private Trial trial;




    private bool isWalkingForward;
    private bool isSideStepping;
    private float yRotation = 0f;
    private Vector3 previousPosition;
    private HolderObject holderObject;
    private Vector3 lastInteractDir;
    private ISelectableObject selectedObject;



    private int playerState = 0;
    private float playerEnergy = 100;
    private float playerMaxEnergy = 100;


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        previousPosition = transform.position;
    }

    private void Start() {
        gameInput.OnPickUpAction += GameInput_OnPickUpAction;
        gameInput.OnEnforceAction += GameInput_OnEnforceAction;
        gameInput.OnPunchAction += GameInput_OnPunchAction;
    }

    private void GameInput_OnEnforceAction(object sender, System.EventArgs e) { 
        if (playerState == 1) { 
            playerState = 0;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
            if (trial.trialActive) {
                trial.trialActive = false;
                Debug.Log("inactive");
                OnTrialActivated?.Invoke(this, new OnTrialActivatedEventArgs {
                    active = false
                });
            }
        } else {
            playerState = 1;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
            if (holderObject != null && holderObject.gameObject.name == "Ball") {
                trial.trialActive = true;
                OnTrialActivated?.Invoke(this, new OnTrialActivatedEventArgs {
                    active = true
                });
                Debug.Log("Active");
            }
        }
    }

    private void Update()
    {
        HandleStates();
    }

    public bool IsWalkingForward()
    {
        return isWalkingForward;
    }

    public bool IsSideStepping()
    {
        return isSideStepping;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Get the forward and right vectors based on the player's current orientation
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Modify inputVector with the player's facing direction
        Vector3 moveDir = (forward * inputVector.y + right * inputVector.x);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;

        bool canMove = true;

        // Check for forward/backward movement
        if (Mathf.Abs(inputVector.y) > 0.1f)
        {
            Vector3 moveDirZ = forward * inputVector.y;

            canMove = !Physics.CapsuleCast(transform.position, 
                transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance,
                ~(1 << LayerMask.NameToLayer("PlayerHeldObjects")));

            if (canMove)
            {
                transform.position += moveDirZ.normalized * moveDistance; // Move forward or backward
            }
        }

        // Check for left/right movement
        if (Mathf.Abs(inputVector.x) > 0.1f)
        {
            Vector3 moveDirX = right * inputVector.x;

            canMove = !Physics.CapsuleCast(transform.position,
                transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance,
                ~(1 << LayerMask.NameToLayer("PlayerHeldObjects")));

            if (canMove)
            {
                transform.position += moveDirX.normalized * moveDistance; // Move left or right
            }
        }

        // Set isWalkingForward and isSideStepping based on movement direction
        isWalkingForward = Mathf.Abs(inputVector.y) > 0.1f;
        isSideStepping = Mathf.Abs(inputVector.x) > 0.1f;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * rotationSpeed;

        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f); // Rotate the player around the Y-axis
    }

    private void HandleStates() {
        if (playerState == 0) {
            //State is just normal walking
            HandleMovement();
            HandleRotation();
            HandleSelected();
            if (playerEnergy < playerMaxEnergy) {
                playerEnergy += Time.deltaTime;
            }

        } else if (playerState == 1) {
            //state is enforced
            HandleMovement();
            HandleRotation();
            HandleSelected();
            playerEnergy -= Time.deltaTime;
            playerMaxEnergy += Time.deltaTime / 100;
            if (playerEnergy <= 0) {
                playerState = 0;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                OnTrialActivated?.Invoke(this, new OnTrialActivatedEventArgs {
                    active = false
                });
            }

        } else if(playerState == 2) {
            if (playerEnergy < playerMaxEnergy) {
                playerEnergy += Time.deltaTime;
            }
            playerMaxEnergy += Time.deltaTime/100;
            //state is recovering
        } else {
            Debug.LogError("There is an unknown state");
        }
    }


    private void HandleSelected() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }
        float interactDistance = 20f;
        float raycastHeightOffset = 0.2f; 

        // Calculate the position to start the raycast from
        Vector3 raycastOrigin = transform.position + Vector3.up * raycastHeightOffset;

        
        if (Physics.Raycast(raycastOrigin, transform.forward, out RaycastHit raycastHit, interactDistance, objectsLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out ISelectableObject selectableObject)) {
                if (selectableObject != selectedObject) {
                    SetSelectedObject(selectableObject);
                }
            } else {
                SetSelectedObject(null);
            }
        } else {
            SetSelectedObject(null);
        }
    }

    public int returnPlayerState() {
        return playerState;
    }

    public float returnPlayerEnergy() {
        return playerEnergy;
    }

    public float returnPlayerMaxEnergy() {
        return playerMaxEnergy;
    }

    public Transform GetHolderObjectFollowTransform() {
        return holderObjectHoldPoint;
    }

    public void SetHolderObject(HolderObject holderObject) {
        this.holderObject = holderObject;
    }

    public HolderObject GetHolderObject() {
        return holderObject;
    }

    public void ClearHolderObject() {
        holderObject = null;
    }

    public bool HasHolderObject() {
        return holderObject != null;
    }

    private void SetSelectedObject(ISelectableObject selectedObject) {
        this.selectedObject = selectedObject;

        OnSelectedObjectChanged?.Invoke(this, new OnSelectedObjectChangedEventArgs {
            selectedObject = selectedObject
        });
    }

    private void GameInput_OnPickUpAction(object sender, System.EventArgs e) {
        Debug.Log("Picking up");
        if (selectedObject != null) {
            selectedObject.Interact(this);
            var obj = GetHolderObject().gameObject;
            obj.layer = LayerMask.NameToLayer("PlayerHeldObjects");

            if (holderObject.gameObject.name == "Ball" && playerState == 1) {
                trial.trialActive = true;
                OnTrialActivated?.Invoke(this, new OnTrialActivatedEventArgs {
                    active = true
                });
                Debug.Log("Active");
            }
        } else if (HasHolderObject()) {
            Debug.Log("PuttingDown");
            var obj = GetHolderObject().gameObject;
            obj.layer = LayerMask.NameToLayer("Default");
            GetHolderObject().Interact(this);
            if (trial.trialActive) {
                trial.trialActive = false;
                OnTrialActivated?.Invoke(this, new OnTrialActivatedEventArgs {
                    active = false
                });
                Debug.Log("inactive");
            }
        }
    }

    private void GameInput_OnPunchAction(object sender, System.EventArgs e) {
        if (playerState == 1) {
            // If in enforced state, reduce energy instead of punching
            if (selectedObject != null && selectedObject.gameObject.CompareTag("Enemy")) {
                selectedObject.Interact(this);
                playerEnergy -= 2;
            }
        }
    }

    public void ResetAttributes() { 
        playerState = 0;
        playerEnergy = 100;
        playerMaxEnergy = 100;
    }

    public void ResetPos() {
        transform.position = new Vector3(18f, 3.35f, 37.5f);
    }
}




