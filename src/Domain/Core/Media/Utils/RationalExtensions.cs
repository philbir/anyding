using SixLabors.ImageSharp;

namespace Anyding.Media;

public static class RationalExtensions
{
    public static double GetValue(this Rational rational)
    {
        return rational.Numerator / rational.Denominator;
    }
}
