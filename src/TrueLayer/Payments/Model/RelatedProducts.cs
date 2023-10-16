namespace TrueLayer.Payments.Model;

public record RelatedProducts
{
    /// <summary>
    /// Creates a Related products.
    /// </summary>
    /// <param name="signupPlus">Signup+</param>
    public RelatedProducts(SignupPlus? signupPlus = null)
    {
        SignupPlus = signupPlus;
    }
    /// <summary>
    /// Gets signup+ product.
    /// </summary>
    public SignupPlus? SignupPlus { get; }
}

/// <summary>
/// Creates a Signup+ products.
/// </summary>
/// <param name="signupPlus">Signup+</param>
public record SignupPlus
{
}
