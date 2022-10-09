using UnityEngine;

namespace ProjectEdit.LevelsEditor
{
    public class EditorCamera : MonoBehaviour
    {
		private static Vector2 MousePosition => InputSystem.Editor.MousePosition.ReadValue<Vector2>();

		[SerializeField] private Camera m_SelectionCamera;
		
		private Plane m_Plane = new(Vector3.back, Vector3.zero);

		private Vector3 m_DragStartPosition;
		private Vector3 m_DragCurrentPosition;

		private bool m_IsAlt;
		private bool m_IsLPressing;
		private bool m_IsRPressing;

		private Camera m_Camera;

		private Transform m_CameraTransform;
		private Transform m_SelectionTransform;

        private void Start()
        {
            m_Camera = GetComponent<Camera>();

			InputSystem.Editor.Enable();

			// Alt Key
			InputSystem.Editor.Alt.performed += _ => m_IsAlt = true; 
			InputSystem.Editor.Alt.canceled += _ => m_IsAlt = false;

			// Left Mouse Button
			InputSystem.Editor.Perform.performed += _ =>
			{
				m_IsLPressing = true;

				Ray ray = m_Camera.ScreenPointToRay(MousePosition);

				if (m_Plane.Raycast(ray, out float entry))
					m_DragStartPosition = ray.GetPoint(entry);
			};

			InputSystem.Editor.Perform.canceled += _ => m_IsLPressing = false;

			// Right Mouse Button
			InputSystem.Editor.RMB.performed += _ => m_IsRPressing = true; 
			InputSystem.Editor.RMB.canceled += _ => m_IsRPressing = false; 

			// Mouse Scroll
			InputSystem.Editor.ScrollDelta.performed += c => MouseScroll(c.ReadValue<float>());
			
			UpdateSelectionCamera();
        }

		private void Update()
		{
			if (m_IsLPressing && m_IsAlt)
            {
				Ray ray = m_Camera.ScreenPointToRay(MousePosition);

                if (m_Plane.Raycast(ray, out float entry))
                {
					m_DragCurrentPosition = ray.GetPoint(entry);
					transform.position += m_DragStartPosition - m_DragCurrentPosition;
                }
            }

			if (m_IsRPressing && m_IsAlt)
				MouseScroll(InputSystem.Editor.MouseDelta.ReadValue<float>());

			m_SelectionTransform.position = m_CameraTransform.position;
		}

		private void MouseScroll(float delta)
        {
	        float orthographicSize = m_Camera.orthographicSize;
	        orthographicSize -= delta;
	        m_Camera.orthographicSize = orthographicSize;
	        m_Camera.orthographicSize = Mathf.Clamp(orthographicSize, 1.0f, 12.0f);
	        m_SelectionCamera.orthographicSize = m_Camera.orthographicSize;
        }

		private void UpdateSelectionCamera()
		{
			RenderTexture rt = m_SelectionCamera.targetTexture;
			
			rt.Release();
			rt.width = Screen.width;
			rt.height = Screen.height;
			rt.Create();
			
			m_SelectionCamera.aspect = m_Camera.aspect;

			m_CameraTransform = transform;
			m_SelectionTransform = m_SelectionCamera.transform;
		}
    }
}
