namespace Domain.Specification.Token;

public class TokenSpecification : SpecificationBase<Entities.Token>
{
    public TokenSpecification(string refreshToken)
    {
        Criteria = t => t.RefreshToken == refreshToken;
    }

    public TokenSpecification(string accessToken, string refreshToken)
    {
        Criteria = t => t.JwtToken == accessToken && t.RefreshToken == refreshToken;
        AddInclude(t => t.User);
    }
}