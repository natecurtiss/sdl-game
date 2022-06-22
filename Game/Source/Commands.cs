namespace Game;

static class Commands
{
    public static readonly Action<Input, Character, float, float> PlayerMovement = 
        (input, me, dt, speed) => me.Position += input.Axis().Normalized() * dt * speed; 
}