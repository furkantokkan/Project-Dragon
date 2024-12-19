using RPG.Control;

public interface IRaycastable 
{
    CursorType GetCursorType();
    bool HandleRaycast(PlayerController sender);    
}
