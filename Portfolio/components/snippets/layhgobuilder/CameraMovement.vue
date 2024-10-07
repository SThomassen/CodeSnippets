<template>
	<v-container class="line-numbers">
<pre><code class="language-js line-numbers" style="white-space: pre-wrap; background-color: transparent;">
/// The Editor camera moves and rotates around a target point. 
/// It does not move lower than the surface, but it can still rotate upwards.
/// This will give it the same functionality to be able to look upwards a low object.
/// The camera behaviour is simular to the camera of Planet Coaster.
public class EditorCamera : LayhgoCamera
{
    private bool m_orthographic;
    private float m_distance, m_orthoDistance;
    private Vector3 m_position;
    private Quaternion m_rotation;

    [SerializeField] 
    private Bounds m_bounds = new Bounds();

    protected CameraTarget m_target = null;

    public void UpdateCameraMove(var a_delta)
    {
        if (m_currentState != ECameraState.Move)
		{
            return;
		}

        var velocity = m_target.Right * a_delta.x + m_target.Forward * a_delta.y;
        velocity.y = 0;

        MoveCamera(velocity * m_currentMoveSpeed * m_distance * .25f);
    }

    public void MoveCamera(var a_velocity)
    {
        var pos = m_target.Position + a_velocity;

        var width = m_bounds.center.x + m_bounds.extents.x;
        var depth = m_bounds.center.z + m_bounds.extents.z;

        var minHeight = m_bounds.center.y - m_bounds.extents.y;
        var maxHeight = m_bounds.center.y + m_bounds.extents.y;

        pos.x = Mathf.Clamp(pos.x, -width, width);
        pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
        pos.z = Mathf.Clamp(pos.z, -depth, depth);

        m_target.Position = pos;
    }

    public void RotateCamera(var a_euler)
    {
        var euler = m_target.Rotation.eulerAngles;
        euler -= a_euler;

        // Clamp x axis
        euler.x = (euler.x > 180) ? euler.x - 360 : euler.x;
        euler.x = Mathf.Clamp(euler.x, m_orthograpic ? 1 : m_rotateBounds.x, m_rotateBounds.y);

        m_target.Rotation = Quaternion.Euler(euler);
    }

    // Apply the transform changes
    private void TransformCamera()
    {
        var distance = m_targetDistance;

        if (m_smoothZoom)
		{
            m_distance = Mathf.Lerp(m_distance, m_orthograpic ? distance * m_orthoDistance : distance, Time.deltaTime * m_zoomTime);
		}
        else
		{
            m_distance = m_orthograpic ? distance * m_orthoDistance : distance;
		}

        if (m_smoothRotate)
		{
            m_rotation = Quaternion.Slerp(m_rotation, m_target.Rotation, Time.deltaTime * m_rotateTime);
		}
        else
		{
            m_rotation = m_target.Rotation;
		}

        if (m_smoothMove)
		{
            m_position = Vector3.Lerp(m_position, m_target.Position - m_target.Forward * m_distance, Time.deltaTime * m_currentMoveTime);
		}
        else
		{
            m_position = m_target.Position - transform.forward * m_distance;
		}

        m_position.y = CapHeight(m_position);
    }

    public override void Tick()
    {
        if (m_target == null)
		{
            return;
		}

        base.Tick();

        m_delta = m_mousePosition - Input.MousePosition;

        UpdateCameraMove(m_delta);
        UpdateCameraRotate(m_delta);
        UpdateCameraElevate(m_delta);

        UpdateCameraEdgeScrolling();
        TransformCamera();

        if (m_orthograpic)
        {
            var rotEuler = m_rotation.eulerAngles;
            rotEuler.x = m_rotateBounds.x;
            m_rotation = Quaternion.Euler(rotEuler);
        }

        transform.position = m_position;
        transform.rotation = m_rotation;

        m_mousePosition = Input.MousePosition;
    }
}
</code></pre>
</v-container>
</template>

<script>
import Prism from 'prismjs'

export default {
    mounted() {
        Prism.highlightAll();
    }
}
</script>