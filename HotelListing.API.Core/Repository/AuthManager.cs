using AutoMapper;

using  HotelListing.API.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelListing.API.Core.Abstract;
using HotelListing.API.Core.Models.Users;

namespace  HotelListing.API.Core.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;
        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";

        public AuthManager(IMapper mapper,UserManager<ApiUser> user,IConfiguration configuration)
        {
            this._mapper = mapper;
            this._userManager = user;
            this._configuration = configuration;
        }
        public async Task<IEnumerable<IdentityError>> RegisterAsync(ApiUserDTO userDTO)
        {
            _user= _mapper.Map<ApiUser>(userDTO);
            _user.UserName = userDTO.Email;
            var result = await _userManager.CreateAsync(_user, userDTO.PassWord);
            if (result.Succeeded)
                result = await _userManager.AddToRoleAsync(_user, "User");
            return result.Errors;

        }
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO model)
        {
           
            _user = await _userManager.FindByEmailAsync(model.Email);
            if (_user is null)
                return default;
            bool isValidCredentials = await _userManager.CheckPasswordAsync(_user, model.PassWord);
            if (isValidCredentials)
                    return default;
            var token = await GenerateTokenAsync();
            var refreshToken = await CreateRefreshTokenAsync();
            return new AuthResponseDTO
            {
                Token = token,
                UserID = _user.Id,
                RefreshToken=refreshToken
            };
            
        }

        private async Task<string> GenerateTokenAsync()
        {
            var securityKey =new  SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaim = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            //claims sisteme giriş yapan kullanıcıların ek bilgilerinin tutulduğu yapıdır.
            var userClaims=await _userManager.GetClaimsAsync(_user);
            //token içerisindeki gizli bilgileri tutucaz
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub,_user.Email),
                //tokena erişmek isteyenlere sahte token verir
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,_user.Email),
                new Claim("uid",_user.Id)
            }
            .Union(userClaims).Union(roleClaim);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials:credentials
                
                
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshTokenAsync()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);
            return newRefreshToken;
        }

        public async Task<AuthResponseDTO> VerifyRefreshTokenAsync(AuthResponseDTO request)
        {
            var jwtSecurityTokenHandler=new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var userName = tokenContent.Claims.ToList().FirstOrDefault(i=>i.Type==JwtRegisteredClaimNames.Email).Value;
            _user = await _userManager.FindByNameAsync(userName);
            if(_user==null || _user.Id!=request.UserID)
                    return null;
            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);
            if (isValidRefreshToken)
            {
                var token = await GenerateTokenAsync();
                return new AuthResponseDTO
                {
                    Token = token,
                    UserID = _user.Id,
                    RefreshToken = await CreateRefreshTokenAsync(),

                };
            }
            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }
    }
}
