namespace Domain.Specification.Token;

public class TokenByJwtSpecification : SpecificationBase<Entities.Token>
{
    public TokenByJwtSpecification(string accessToken)
    {
        Criteria = t => t.JwtToken == accessToken;
    }
}