using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomerAPI.Models;
using CustomerAPI.Data;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using CustomerAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using CustomerAPI.Services;

namespace CustomerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CustomerAPIContext _context;
        private readonly JWTSetting setting;
        private readonly IRefreshTokenGenerator tokenGenerator;

        public UserController(CustomerAPIContext CustomerAPI_DB, IOptions<JWTSetting> options, IRefreshTokenGenerator _refreshToken,
            ) {
            _context = CustomerAPI_DB;
            setting =  options.Value;
            tokenGenerator = _refreshToken;        }


        [NonAction]
        public TokenResponse Authenticate(string username, Claim[] claims)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var tokenkey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenhandler = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                 signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)

                );
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(username);

            return tokenResponse;
        }

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCred user)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var _user = _context.TblUser.FirstOrDefault(o=> o.Userid == user.UserName && o.Password == user.Password);
            if(_user == null)
            {
                return Unauthorized();
            }

            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _user.Userid),
                        new Claim(ClaimTypes.Role, _user.Role)

                    }
                ),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenhandler.WriteToken(token);

            tokenResponse.JWTToken = finaltoken;
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(user.UserName);

            return Ok(tokenResponse);
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token.JWTToken, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.securitykey)),
                ValidateIssuer = false,
                ValidateAudience = false,

            }, out securityToken);

            var _token = securityToken as JwtSecurityToken;
            if (_token != null && !_token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)) {
                return Unauthorized();
            }
            var username = principal.Identity.Name;
            var _reftable = _context.TblRefreshtoken.FirstOrDefault(o => o.UserId == username && o.RefreshToken == token.RefreshToken);
            if (_reftable == null)
            {
                return Unauthorized();
            }

            TokenResponse _result = Authenticate(username, principal.Claims.ToArray());

            return Ok(_result);
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            //var response = await _authenticationService.Register(request);

            return Ok(response);
        }

    }

    
}
