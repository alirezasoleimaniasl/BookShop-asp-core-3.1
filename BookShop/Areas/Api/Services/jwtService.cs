using BookShop.Areas.Admin.Data;
using BookShop.Areas.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Areas.Api.Services
{
    public class jwtService:IjwtService
    {
        public readonly IApplicationUserManager _userManager;
        public readonly IApplicationRoleManager _roleManager;
        public jwtService(IApplicationUserManager userManager, IApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser User)
        {
            var secretKey = Encoding.UTF8.GetBytes("1234567890asdfgh");
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionKey = Encoding.UTF8.GetBytes("1234567890asdfgh");//Feed wih 16 character or more
            
            //Encrypting the payload(Whole content)
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionKey),SecurityAlgorithms.Aes128KW,SecurityAlgorithms.Aes128CbcHmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "Mizfa.com",
                Audience = "Mizfa.com",
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(await GetClaimsAsync(User)),
                EncryptingCredentials = encryptingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }


        public async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser User)
        {
            var Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,User.UserName),
                new Claim(ClaimTypes.NameIdentifier,User.Id),
                new Claim(ClaimTypes.MobilePhone,User.PhoneNumber),
                new Claim("SecurityStampClaimTypes",User.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var Roles = _roleManager.Roles.ToList();
            foreach (var item in Roles)
            {
                var RoleClaims = await _roleManager.FindClaimsInRole(item.Id);
                foreach (var claim in RoleClaims.Claims)
                {
                    Claims.Add(new Claim(ConstantPolicies.DynamicPermissionClaimType, claim.ClaimValue));
                }
            }

            foreach (var item in Roles)
                Claims.Add(new Claim(ClaimTypes.Role, item.Name));

            return Claims;
        }
    }
}
