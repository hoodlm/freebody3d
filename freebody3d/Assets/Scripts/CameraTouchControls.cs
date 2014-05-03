using UnityEngine;

public class CameraTouchControls : MonoBehaviour {

	/// The current pivot position for the camera to rotate around.
	public Vector3 pivotPosition = Vector3.zero;

	/// The distance (in world space) that the camera can translate vertically before vertical rotation begins.
	public float maxVerticalTranslation = 1.0f;

	/// When the camera is vertically rotating, don't allow it to rotate beyond this angle from horizonal.
	/// This value should be less than (but close to) 90 degrees.
	public float maxVerticalRotationAngle = 80f;

	/// Scalar to affect the (angular) rate of horizontal rotation.
	public float horizontalRotateRate = 0.5f;

	/// Scalar to affect the (angular) rate of vertical rotation.
	public float verticalRotateRate = 0.5f;

	/// Scalar to affect the linear rate of vertical translation.
	public float verticalTranslateRate = 0.01f;
	
	/// Whether a dragging event is currently happening (that is, if a touch event was detected last frame).
	private bool isDragging;

	/// During a drag event, this contains the touch position of the previous frame.
	private Vector2 previousTouchPosition;

	// Use this for initialization
	void Start () {
		previousTouchPosition = Vector2.zero;
		isDragging = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0)) {
			Vector2 touchPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			Vector2 delta =  touchPosition - previousTouchPosition;

			if (isDragging) {
				MoveWithTouchDelta(delta);
			} else {
				isDragging = true;
			}

			previousTouchPosition = touchPosition;
		} else {
			isDragging = false;
		}
	}

	/**
	 * 	Moves the object based on a 2-dimensional translation (e.g. from a mouse or touch drag).
	 * 	The motion follows the surface of a capsule shape, with the "caps" of the capsule defined by
	 * 	the "maxVerticalTranslation" variable.
	 * 
	 * 	@param delta The change in user input since the previous frame, in pixel coordinates.
	 */
	private void MoveWithTouchDelta (Vector2 delta) {
		HandleHorizontalMovement(delta.x);
		// Since the input is in pixel or screen coordinates, "up" yields a negative y component.
		// In order to make the code more readable in the sections below, we'll flip the sign on delta.y,
		// so that a positive deltaY can be interpreted as "up" and a negative deltaY is interpreted as "down".
		HandleVerticalMovement(-delta.y);
	}

	/**
	 *	Rotates the camera around the pivotPosition based on the horizontal component of the user input.
	 *	This rotation is performed strictly in the XZ plane.
	 *	@param deltaX The X component of the user input.
	 */
	private void HandleHorizontalMovement(float deltaX) {
		float yBefore = transform.position.y;
		this.transform.RotateAround(pivotPosition, Vector3.up, horizontalRotateRate * deltaX);
		DebugUtils.Assert(yBefore.Equals(transform.position.y),
		                  "Y position wasn't invariant during horizonal camera movement.", this);
	}

	/**
	 * 	Moves the camera based on the vertical component of the user input. The type of movement is
	 * 	dependent on the current position of the camera. If the camera's Y displacement is within the
	 * 	threshold defined by +- maxVerticalTranslation, then the camera is translated up and down.
	 * 	If it is outside of that threshold, then the camera rotates vertically, up to a maximum angle
	 * 	defined by maxVerticalRotationAngle.
	 * 	@param deltaY The Y component of the user input.
	 */
	private void HandleVerticalMovement(float deltaY) {
		float verticalDisplacement = this.transform.position.y;
		bool yShouldTranslate = Mathf.Abs(verticalDisplacement) < maxVerticalTranslation;
		
		if (yShouldTranslate) {
			PerformVerticalTranslation(deltaY);
		} else {
			PerformVerticalRotation(deltaY);
		}


	}

	/**
	 * 	Translates the camera and the pivot position along the Y Axis.
	 * 	@param deltaY A scalar for the translation.
	 */
	private void PerformVerticalTranslation(float deltaY) {
		Vector3 positionBefore = transform.position;

		Vector3 translationVector = deltaY * verticalTranslateRate * Vector3.up;
		this.transform.Translate(translationVector, Space.World);
		this.pivotPosition += translationVector;

		DebugUtils.Assert(positionBefore.x.Equals(transform.position.x),
		                  "X position wasn't invariant during vertical camera translation.", this);
		DebugUtils.Assert(positionBefore.z.Equals(transform.position.z),
		                  "Z position wasn't invariant during vertical camera translation.", this);
	}

	/**
	 * 	Rotates the camera vertically - specifically, performs a rotation around the "Left" axis of the camera
	 * 	through the pivotPoint.
	 * 	@param deltaY A scalar for the rotation.
	 */
	private void PerformVerticalRotation(float deltaY) {
		// We make sure that we don't rotate past the maximum angle.
		float angle = Vector3.Angle(Vector3.up, transform.forward);
		float maxTopAngle    = 90 + maxVerticalRotationAngle;
		float maxBottomAngle = 90 - maxVerticalRotationAngle;

		bool yCanRotateDown  = (angle > maxBottomAngle);
		bool yCanRotateUp    = (angle < maxTopAngle);
		DebugUtils.Assert(yCanRotateDown || yCanRotateUp,
		                  "Y is unable to rotate up OR down.", this);

		bool yIsRotatingDown = deltaY <= 0;
		bool yIsRotatingUp   = deltaY >= 0;
		bool yCanRotate = (yCanRotateDown && yIsRotatingDown) || (yCanRotateUp && yIsRotatingUp);
		
		if (yCanRotate) {
			this.transform.RotateAround(pivotPosition, transform.right, verticalRotateRate * deltaY);
		}
	}
}
