<template>
	<v-container class="line-numbers">
<pre><code class="language-js line-numbers" style="white-space: pre-wrap; background-color: transparent;">
public class SiteObject : BasicObject, IDraggable
{
    /// Find the closest target within range and move it aside the target
    /// return true when there is a connection
    public virtual bool Connect()
    {
        ConnectState(false);

        m_snapRange = m_size.x * 0.5f;
        float closest = float.MaxValue;
        Collider[] colliders = Physics.OverlapSphere(m_movePosition, m_snapRange, LayerCollection.ConstructionLayer);
        SiteObject closestConnectable = null;

        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                // check if there is a connectable object
                SiteObject connectable = CanConnectToObject(col);
                if (connectable == null)
                    continue;

                float dist = Vector3.Distance(col.gameObject.transform.position, m_movePosition);
                if (dist < closest)
                {
                    closestConnectable = col.GetComponent< SiteObject>();
                    closest = dist;
                }
            }
            if (closestConnectable == null)
                return false;

            int side = CaculateSide(closestConnectable);
            ConnectSite(closestConnectable, side);
            ConnectState(true);

            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculate on which side the object is relative to the target
    /// returns a value of positive or negative 1.
    /// </summary>
    public int CaculateSide(SiteObject a_object)
    {
        Vector3 center = m_movePosition;
        Vector3 other = a_object.GetObject().GetCenter();
        Vector3 dir = (other - center).normalized;

        float dot = Vector3.Dot(dir, a_object.transform.right);
        int sign = (int)Mathf.Sign(dot);
        return sign;
    }

    /// <summary>
    /// Place the object at the side of the target
    /// </summary>
    protected virtual void ConnectSite(SiteObject a_target, int a_dir)
    {
        // Get the connect point
        Vector3 connectPosition = a_target.transform.position + (-a_dir * a_target.transform.right * m_size.x * 0.5f);

        // The object can be placed with a range of 180 degrees round the connection point
        float angleY = m_angles.y * GameManager.Instance.Editor.RotateInterval;
        int positions = Mathf.CeilToInt(180 / angleY) + 1;
        Vector3 closestPoint = transform.position;
        float closest = float.MaxValue;
        // Loop through possiblities and find closest point within range
        for (int i = 0; i < positions; i++)
        {
            Vector3 selfOffset = Quaternion.AngleAxis(90 + i * angleY, Vector3.up) * (transform.right * m_size.x * 0.5f * a_dir);
            float dist = Vector3.Distance(m_movePosition, connectPosition + selfOffset);
            if (dist < closest)
            {
                closest = dist;
                closestPoint = connectPosition + selfOffset;
            }
        }
        
        // Set position to closest point and rotate the object towards the connection point.
        SetPosition(closestPoint);
        m_direction = (new Vector3(connectPosition.x, 0, connectPosition.z) - new Vector3(transform.position.x, 0, transform.position.z)) * a_dir;
        transform.right = m_direction;
    }

    /// <summary>
    /// Calculate the placement points between start position and target position
    /// The line of points can be curved by changing the curveDist value
    /// </summary>
    public virtual Vector3[] GetDragPoints(Vector3 a_target)
    {
        Vector3 dir = a_target - m_startPosition;

        float length = dir.magnitude;
        float size = m_size.x;
        float dist = 0;
        float time = 0;

        int count = Mathf.CeilToInt(length / size);
        m_direction = SnapDirection(dir);
        m_endPosition = m_startPosition + (m_direction * size * count);

        Vector3 center = (m_startPosition + m_endPosition) * .5f;
        Vector3 cross = Vector3.Cross(m_direction, Vector3.up).normalized;
        m_curveTarget = center + (cross * m_curveDist);

        m_points.Clear();
        m_points.Add(m_startPosition);
        Vector3 previousPoint = m_startPosition;
        // loop through the points to move the points as close as possible to their position
        while (time <= 1)
        {
            time += 1f / count;
            // curve point based on the target curve
            Vector3 curvePoint = QuadraticCurve(m_startPosition, m_curveTarget, m_endPosition, time);
            dist += Vector3.Distance(previousPoint, curvePoint);
            while (dist >= m_size.x)
            {
                float overshot = dist - m_size.x;
                Vector3 spacedPoint = curvePoint + (previousPoint - curvePoint).normalized * overshot;
                // set the point height to the lowest surface point at its position
                float lowestSurface = GameManager.Instance.Terrain.GetSurfaceHeight(spacedPoint + Vector3.up * 20);
                spacedPoint.y = lowestSurface;

                if (spacedPoint != m_endPosition)
                    m_points.Add(spacedPoint);

                dist = overshot;
                previousPoint = spacedPoint;
            }
            previousPoint = curvePoint;
        }

        // remove the last point as this is the same as the target position.
        m_points.RemoveAt(m_points.Count - 1);
        return m_points.ToArray();
    }

    protected Vector3 QuadraticCurve(Vector3 a_start, Vector3 a_curve, Vector3 a_end, float a_time)
    {
        Vector3 p0 = Vector3.Lerp(a_start, a_curve, a_time);
        Vector3 p1 = Vector3.Lerp(a_curve, a_end, a_time);

        return Vector3.Lerp(p0, p1, a_time);
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