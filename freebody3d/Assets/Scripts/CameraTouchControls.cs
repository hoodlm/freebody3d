using UnityEngine;

public class CameraTouchControls : MonoBehaviour {

	/// The current pivot position for horizontal rotation.
	public Vector3 horizontalPivotPosition = Vector3.zero;

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
	 */
	private void MoveWithTouchDelta (Vector2 delta) {
		// Apply a horizontal rotation based on the X vector.
		this.transform.RotateAround(horizontalPivotPosition, Vector3.up, horizontalRotateRate * delta.x);

		// Vertical motion depends on the current height of the camera. If the height position is
		// within a certain threshold, then we simply translate the camera up and down. Otherwise,
		// we should rotate the camera to look over the "poles" of a capsule.
		float verticalDisplacement = this.transform.position.y;
		bool yShouldTranslate = Mathf.Abs(verticalDisplacement) < maxVerticalTranslation;

		if (yShouldTranslate) {
			Vector3 translationVector = delta.y * verticalTranslateRate * Vector3.down;
			this.transform.Translate(translationVector, Space.World);
			this.horizontalPivotPosition += translationVector;
		} else {
			// We also want to make sure not to rotate too high up.
			float angle = Vector3.Angle(Vector3.up, transform.forward);

			float maxTopAngle    = 90 + maxVerticalRotationAngle;
			float maxBottomAngle = 90 - maxVerticalRotationAngle;

			bool yCanRotateDown  = (angle > maxBottomAngle);
			bool yCanRotateUp    = (angle < maxTopAngle);

			bool yIsRotatingDown = delta.y >= 0;
			bool yIsRotatingUp   = delta.y <= 0;

			bool yCanRotate = (yCanRotateDown && yIsRotatingDown) || (yCanRotateUp && yIsRotatingUp);

			if (yCanRotate) {
				this.transform.RotateAround(horizontalPivotPosition, transform.right, verticalRotateRate * -delta.y);
			}
		}
	}
}
