
namespace CGA
{
    public struct Vector2D
    {
        public float X { get; private set; }
        public float Y { get; private set; }


        public Vector2D(float x, float y)
        {
            X = x; Y = y;
        }


        public void Change(float x, float y)
        {
            X = x; Y = y; 
        }
    }
}
