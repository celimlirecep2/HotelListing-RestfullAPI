using HotelListing.API.Core.Models.Users;

using Microsoft.AspNetCore.Identity;

namespace  HotelListing.API.Core.Abstract
{
    public interface IAuthManager 
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO model);
        Task<IEnumerable<IdentityError>> RegisterAsync(ApiUserDTO userDTO);
        Task<string> CreateRefreshTokenAsync();
        Task<AuthResponseDTO> VerifyRefreshTokenAsync(AuthResponseDTO request);
    }
}
