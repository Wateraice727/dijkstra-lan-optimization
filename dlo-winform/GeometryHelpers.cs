namespace dlo_winform;

public static class GeometryHelpers
{
    public static bool CirclesOverlap(PointF a, float radiusA, PointF b, float radiusB)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        float distSq = dx * dx + dy * dy;
        float radiusSum = radiusA + radiusB;
        return distSq < radiusSum * radiusSum;
    }
    private static float Orientation(PointF p, PointF q, PointF r)
    {
        return (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
    }
    public static bool SegmentsIntersect(PointF a1, PointF a2, PointF b1, PointF b2)
    { 

        float o1 = Orientation(a1, a2, b1);
        float o2 = Orientation(a1, a2, b2);
        float o3 = Orientation(b1, b2, a1);
        float o4 = Orientation(b1, b2, a2);

        if (o1 == 0 && IsOnSegment(a1, a2, b1)) return true;
        if (o2 == 0 && IsOnSegment(a1, a2, b2)) return true;
        if (o3 == 0 && IsOnSegment(b1, b2, a1)) return true;
        if (o4 == 0 && IsOnSegment(b1, b2, a2)) return true;

        return (o1 > 0) != (o2 > 0) && (o3 > 0) != (o4 > 0);
    }

    private static bool IsOnSegment(PointF a, PointF b, PointF c)
    {
        return c.X >= (a.X < b.X ? a.X : b.X) && c.X <= (a.X > b.X ? a.X : b.X) &&
               c.Y >= (a.Y < b.Y ? a.Y : b.Y) && c.Y <= (a.Y > b.Y ? a.Y : b.Y);
    }

    public static float Distance(PointF a, PointF b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    public static float DistanceToSegmentSquared(PointF p, PointF a, PointF b)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;
        float lengthSq = dx * dx + dy * dy;
        if (lengthSq == 0f)
        {
            float px = p.X - a.X;
            float py = p.Y - a.Y;
            return px * px + py * py;
        }

        float t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lengthSq;
        if (t < 0f) t = 0f;
        else if (t > 1f) t = 1f;

        float projX = a.X + t * dx;
        float projY = a.Y + t * dy;
        float projDx = p.X - projX;
        float projDy = p.Y - projY;
        return Distance(p, new PointF(projX, projY));
    }
    public static float DistanceToSegment(PointF p, PointF a, PointF b)
    {
        return MathF.Sqrt(DistanceToSegmentSquared(p, a, b));
    }
    
    public static bool EdgePassesThroughNode(NetworkEdge edge, NetworkNode node)
    {
        float distSq = DistanceToSegmentSquared(node.Position, edge.StartNode.Position, edge.EndNode.Position);
        return distSq < (node.Radius * node.Radius);
    }
}
