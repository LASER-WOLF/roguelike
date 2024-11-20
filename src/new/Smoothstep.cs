namespace Core;

/// <summary>
/// Smoothstep functions.
/// From: https://iquilezles.org/articles/smoothsteps/
/// </summary>
public static class Smoothstep
{

    public static float Run(float x, bool inverse = false, float n = 1.0f, int index = 0)
    {
        switch (index)
        {
            case 1:
                return QuarticPolynomial(x, inverse);
            case 2:
                return QuinticPolynomial(x);
            case 3:
                return QuadraticRational(x, inverse);
            case 4:
                return CubicRational(x, inverse);
            case 5:
                return Rational(x, inverse, n);
            case 6:
                return PiecewiseQuadratic(x, inverse);
            case 7:
                return PiecewisePolynomial(x, inverse, n);
            case 8:
                return Trigonometric(x, inverse);
        }
        return CubicPolynomial(x, inverse);
    }
    
    // #0
    // Cubic Polynomial
    // Continuity: C1
    public static float CubicPolynomial(float x, bool inverse = false)
    {
        if (inverse) { return 0.5f - MathF.Sin( MathF.Asin( 1.0f - 2.0f * x ) / 3.0f ); }
        return x * x * ( 3.0f - 2.0f * x );                                
    }

    // #1
    // Quartic Polynomial
    // Continuity: C1
    public static float QuarticPolynomial(float x, bool inverse = false)
    {
        if (inverse) { return MathF.Sqrt( 1.0f - MathF.Sqrt( 1.0f - x ) ); }
        return x * x * ( 2.0f - x * x );                                   
    }

    // #2
    // Quintic Polynomial
    // Continuity: C2
    public static float QuinticPolynomial(float x)
    {
        return x * x * x * ( x * ( x * 6.0f - 15.0f ) + 10.0f );
    }

    // #3
    // Quadratic Rational
    // Continuity: C1
    public static float QuadraticRational(float x, bool inverse = false)
    {
        if (inverse) { return ( x - MathF.Sqrt( x * ( 1.0f - x ) ) ) / ( 2.0f * x - 1.0f ); }
        return x * x / ( 2.0f * x * x - 2.0f * x + 1.0f );
    }
    
    // #4
    // Cubic Rational
    // Continuity: C2
    public static float CubicRational(float x, bool inverse = false)
    {
        if (inverse)
        { 
            float a = MathF.Pow(        x, 1.0f / 3.0f );
            float b = MathF.Pow( 1.0f - x, 1.0f / 3.0f );
            return a / ( a + b ); 
        }
        return x * x * x / ( 3.0f * x * x - 3.0f * x + 1.0f );
    }

    // #5
    // Rational
    // Continuity: C(n-1)
    public static float Rational(float x, bool inverse = false, float n = 1.0f)
    {
        if (inverse) { n = 1.0f / n; }
        return MathF.Pow( x, n ) / ( MathF.Pow( x, n ) + MathF.Pow( 1.0f - x, n ));
    }
    
    // #6
    // Piecewise Polynomial
    // Continuity: C1
    public static float PiecewiseQuadratic(float x, bool inverse = false)
    {
        if (inverse) { return ( x < 0.5f ) ? MathF.Sqrt( 0.5f * x ) : 1.0f - MathF.Sqrt( 0.5f - 0.5f * x ); }
        return ( x < 0.5f ) ? 2.0f * x * x : 2.0f * x * ( 2.0f - x ) - 1.0f;
    }
    
    // #7
    // Piecewise Polynomial
    // Continuity: C(n-1)
    public static float PiecewisePolynomial(float x, bool inverse = false, float n = 1.0f)
    {
        if (inverse) { n = 1.0f / n; }
        return ( x < 0.5f) ? 0.5f * MathF.Pow( 2.0f * x, n) : 1.0f - 0.5f * MathF.Pow( 2.0f * ( 1.0f - x ), n);
    }
    
    // #8
    // Trigonometric
    // Continuity: C1
    public static float Trigonometric(float x, bool inverse = false)
    {
        if (inverse) { return MathF.Acos( 1.0f - 2.0f * x ) / MathF.PI; }
        return 0.5f - 0.5f * MathF.Cos( MathF.PI * x );
    }

}
