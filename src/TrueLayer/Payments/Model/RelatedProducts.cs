namespace TrueLayer.Payments.Model;

/// <summary>
/// Represents related products that can be associated with a payment.
/// </summary>
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
    /// Gets Signup+ product.
    /// </summary>
    public SignupPlus? SignupPlus { get; }
}

/// <summary>
/// Represents a Signup+ product.
/// </summary>
public record SignupPlus
{
}
