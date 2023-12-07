# Summary
|||
|:---|:---|
| Generated on: | 13/09/2023 - 16:52:44 |
| Coverage date: | 13/09/2023 - 16:50:49 |
| Parser: | Cobertura |
| Assemblies: | 4 |
| Classes: | 50 |
| Files: | 44 |
| **Line coverage:** | 84% (756 of 900) |
| Covered lines: | 756 |
| Uncovered lines: | 144 |
| Coverable lines: | 900 |
| Total lines: | 1639 |
| **Branch coverage:** | 72.8% (86 of 118) |
| Covered branches: | 86 |
| Total branches: | 118 |
| **Method coverage:** | [Feature is only available for sponsors](https://reportgenerator.io/pro) |

|**Name**|**Covered**|**Uncovered**|**Coverable**|**Total**|**Line coverage**|**Covered**|**Total**|**Branch coverage**|
|:---|---:|---:|---:|---:|---:|---:|---:|---:|
|**API**|**68**|**54**|**122**|**290**|**55.7%**|**0**|**8**|**0%**|
|API.Controllers.AdminController|16|0|16|42|100%|0|0||
|API.Controllers.CategoryController|16|0|16|43|100%|0|0||
|API.Controllers.ProductController|20|0|20|54|100%|0|0||
|API.Controllers.UserController|16|0|16|43|100%|0|0||
|API.Middleware.ExceptionMiddleware|0|19|19|37|0%|0|0||
|API.Middleware.LoggingMiddleware|0|9|9|20|0%|0|0||
|API.Middleware.TokenMiddleware|0|26|26|51|0%|0|8|0%|
|**Application**|**399**|**57**|**456**|**870**|**87.5%**|**51**|**54**|**94.4%**|
|Application.Dto.Auth.LoginRequest|2|0|2|20|100%|0|0||
|Application.Dto.Auth.LoginRequestValidation|0|8|8|20|0%|0|0||
|Application.Dto.Auth.LoginResponse|5|0|5|10|100%|0|0||
|Application.Dto.Auth.RefreshRequest|2|0|2|18|100%|0|0||
|Application.Dto.Auth.RefreshRequestValidation|0|5|5|18|0%|0|0||
|Application.Dto.Auth.RefreshResponse|3|0|3|8|100%|0|0||
|Application.Dto.Auth.RegisterRequest|2|0|2|22|100%|0|0||
|Application.Dto.Auth.RegisterRequestValidation|0|10|10|22|0%|0|0||
|Application.Dto.Category.CategoryRequest|1|0|1|15|100%|0|0||
|Application.Dto.Category.CategoryRequestValidation|0|4|4|15|0%|0|0||
|Application.Dto.Category.CategoryResponse|0|2|2|7|0%|0|0||
|Application.Dto.Product.ProductRequest|3|0|3|19|100%|0|0||
|Application.Dto.Product.ProductRequestValidation|0|6|6|19|0%|0|0||
|Application.Dto.Product.ProductResponse|0|6|6|11|0%|0|0||
|Application.Dto.User.UserRequest|2|0|2|22|100%|0|0||
|Application.Dto.User.UserRequestValidation|0|10|10|22|0%|0|0||
|Application.Dto.User.UserResponse|2|0|2|7|100%|0|0||
|Application.Services.CategoryService|39|0|39|67|100%|4|4|100%|
|Application.Services.ProductService|69|2|71|112|97.1%|9|10|90%|
|Application.Services.TokenService|137|4|141|205|97.1%|12|14|85.7%|
|Application.Services.UserService|132|0|132|211|100%|26|26|100%|
|**Domain**|**158**|**29**|**187**|**350**|**84.4%**|**17**|**38**|**44.7%**|
|Domain.Common.BaseEntity|1|0|1|6|100%|0|0||
|Domain.Entities.AppUser|1|1|2|9|50%|0|0||
|Domain.Entities.Category|1|1|2|9|50%|0|0||
|Domain.Entities.Product|6|0|6|13|100%|0|0||
|Domain.Entities.Token|6|0|6|13|100%|0|0||
|Domain.Exceptions.CategoryException|3|0|3|8|100%|0|0||
|Domain.Exceptions.ProductException|3|0|3|8|100%|0|0||
|Domain.Exceptions.UserException|3|0|3|8|100%|0|0||
|Domain.Specification.Product.ProductRequestParams|4|0|4|9|100%|0|0||
|Domain.Specification.Product.ProductSpecification|13|17|30|46|43.3%|1|14|7.1%|
|Domain.Specification.Product.ProductSpecificationForCount|11|0|11|17|100%|0|0||
|Domain.Specification.RequestParams|5|2|7|22|71.4%|0|4|0%|
|Domain.Specification.SpecificationBase`1|21|0|21|35|100%|0|0||
|Domain.Specification.SpecificationEvaluator`1|49|0|49|72|100%|13|14|92.8%|
|Domain.Specification.Token.TokenByJwtSpecification|0|4|4|9|0%|0|0||
|Domain.Specification.Token.TokenSpecification|9|0|9|15|100%|0|0||
|Domain.Specification.User.UserByRoleForCountSpecification|7|0|7|14|100%|0|0||
|Domain.Specification.User.UserByRoleSpecification|14|4|18|31|77.7%|3|6|50%|
|Domain.Specification.User.UserRequestParams|1|0|1|6|100%|0|0||
|**Infrastructure**|**131**|**4**|**135**|**245**|**97%**|**18**|**18**|**100%**|
|Infrastructure.Repositories.RepositoryBase`1|28|4|32|63|87.5%|0|0||
|Infrastructure.Repositories.UnitOfWork|33|0|33|61|100%|6|6|100%|
|Infrastructure.Repositories.UserRepository|70|0|70|121|100%|12|12|100%|
