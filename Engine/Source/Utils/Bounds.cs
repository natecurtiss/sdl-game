namespace Engine;

public struct Bounds
{
    public readonly Vector2 Center;
    public readonly Vector2 Size;
    public Vector2 Extents => Size / 2f;
    public Vector2 Left => Center with {X = Center.X - Extents.X};
    public Vector2 Right => Center with {X = Center.X + Extents.X};
    public Vector2 Bottom => Center with {Y = Center.Y - Extents.Y};
    public Vector2 Top => Center with {Y = Center.Y + Extents.Y};
    
    public Bounds(Vector2 center, Vector2 size)
    {
        Center = center;
        Size = size;
    }
    
    public bool Overlaps(Vector2 point) =>
        point.X >= Left.X &&
        point.X <= Right.X &&
        point.Y >= Bottom.Y &&
        point.Y <= Top.Y;
    
    public bool Overlaps(Bounds other)
    {
        var bottomLeft1 = new Vector2(Left.X, Bottom.Y);
        var topRight1 = new Vector2(Right.X, Top.Y);
        var bottomLeft2 = new Vector2(other.Left.X, other.Bottom.Y);
        var topRight2 = new Vector2(other.Right.X, other.Top.Y);

        var oneIsALine = bottomLeft1.X == topRight1.X || 
                         bottomLeft1.Y == topRight1.Y || 
                         bottomLeft2.X == topRight2.X ||
                         bottomLeft2.Y == topRight2.Y;
        if (oneIsALine)
            return false;

        var oneIsToTheLeft = bottomLeft1.X >= topRight2.X || bottomLeft2.X >= topRight1.X;
        if (oneIsToTheLeft)
            return false;

        var oneIsOnTop = topRight1.Y <= bottomLeft2.Y || topRight2.Y <= bottomLeft1.Y;
        return !oneIsOnTop;
    }
    public bool IsAbove(Bounds other) => Top.Y >= other.Top.Y;
    public bool IsBelow(Bounds other) => Bottom.Y <= other.Bottom.Y;
    public bool IsRightOf(Bounds other) => Right.X >= other.Right.X;
    public bool IsLeftOf(Bounds other) => Left.X <= other.Left.X;
    public bool IsCompletelyAbove(Bounds other) => Bottom.Y >= other.Top.Y;
    public bool IsCompletelyBelow(Bounds other) => other.IsCompletelyAbove(this);
    public bool IsCompletelyRightOf(Bounds other) => Left.X >= other.Right.X;
    public bool IsCompletelyLeftOf(Bounds other) => other.IsCompletelyRightOf(this);
    
    public void Encapsulate(Vector2 point)
    {
        if (Overlaps(point)) return;
        var left = Left.X;
        var right = Right.X;
        var bottom = Bottom.Y;
        var top = Top.Y;
        if (point.X < Left.X)
            left = point.X;
        else if (point.X > Right.X)
            right = point.X;
        if (point.Y < Bottom.Y)
            bottom = point.Y;
        else if (point.Y > Top.Y)
            top = point.Y;
        var bottomLeft = new Vector2(left, bottom);
        var topRight = new Vector2(right, top);
        var size = topRight - bottomLeft;
        var center = bottomLeft + size / 2f;
        this = new(center, size);
    }
}