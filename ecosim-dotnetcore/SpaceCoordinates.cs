using System;
using System.Collections.Generic;
using System.Text;

namespace ecosim_dotnetcore
{
    class SpaceCoordinates
    {
        public double x;
        public double y;

        public SpaceCoordinates()
        {
            x = 0.0;
            y = 0.0;
        }
        public SpaceCoordinates(double paramXY)
        {
            x = paramXY;
            y = paramXY;
        }
        public SpaceCoordinates(double paramX, double paramY)
        {
            x = paramX;
            y = paramY;
        }
        public SpaceCoordinates(SpaceCoordinates coordinates)
        {
            x = coordinates.x;
            y = coordinates.y;
        }
        public void Set(double value)
        {
            x = value;
            y = value;
        }
        public void AddCoordinates(SpaceCoordinates coordinates)
        {
            x += coordinates.x;
            y += coordinates.y;
        }
        public void AddCoordinates(double factor)
        {
            x += factor;
            y += factor;
        }
        public void SubtractCoordinates(SpaceCoordinates coordinates)
        {
            x -= coordinates.x;
            y -= coordinates.y;
        }
        public void SubtractCoordinates(double factor)
        {
            x -= factor;
            y -= factor;
        }
        public void MultiplyCoordinates(SpaceCoordinates coordinates)
        {
            x *= coordinates.x;
            y *= coordinates.y;
        }
        public void MultiplyCoordinates(double factor)
        {
            x *= factor;
            y *= factor;
        }
        public void DevideCoordinates(SpaceCoordinates coordinates)
        {
            x /= coordinates.x;
            y /= coordinates.y;
        }
        public void DevideCoordinates(double factor)
        {
            x /= factor;
            y /= factor;
        }

        public double NormalizeFactor()
        {
            return Math.Sqrt((x * x) + (y * y));
        }

        public void Normalize()
        {
            double mag = Math.Sqrt((x * x) + (y * y));
            if(mag != 0)
            {
                x = x / mag;
                y = y / mag;
            }
        }
    }
}
