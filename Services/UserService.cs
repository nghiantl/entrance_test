using WebApi.Entities;
using entrance_test.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace WebApi.Services
{
    public interface IUserService
    {
        UserResponse SaveUser(UserRequest request);

        User GetByEmailAndPassword(string email, string password);

        User GetByEmail(string email);

        User GetById(int id);

        SignInResponse SignIn(SignInRequest signInRequest);

        TokenResponse RefreshToken(TokenRequest tokenRequest);

        Tokens GetByRefreshToken(string refreshToken);
    }
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration appSettings)
        {
            _configuration = appSettings;
        }

        public User GetByEmail(string email)
        {
            User user = null;
            using (var db = new EntranceTestContext())
            {
                user = db.Users.First(s => s.email == email);
            }
            return user;
        }

        public User GetByEmailAndPassword(string email, string password)
        {
            User user = null;
            using (var db = new EntranceTestContext())
            {
                user = db.Users.First(s => s.email == email && s.passwrord == password);
            }
            return user;
        }

        public UserResponse SaveUser(UserRequest request)
        {
            UserResponse response = null;
            using (var db = new EntranceTestContext())
            {
                User user = new User
                {
                    firstName = request.firstName,
                    lastName = request.lastName,
                    email = request.email,
                    passwrord = BCrypt.Net.BCrypt.HashPassword(request.passwrord)
                };
                db.Users.Add(user);
                db.SaveChanges();

                User userRes = GetByEmailAndPassword(request.email, BCrypt.Net.BCrypt.HashPassword(request.passwrord));
                if(userRes != null)
                {
                    response = new UserResponse
                    {
                        Id = userRes.Id,
                        firstName = userRes.firstName,
                        lastName = userRes.lastName,
                        email = userRes.email
                    };    
                }    
            }
            return response;
        }

        public SignInResponse SignIn(SignInRequest signInRequest)
        {
            SignInResponse signInResponse = null;
            using(var db = new EntranceTestContext())
            {
                var user = db.Users.SingleOrDefault(s => s.email == signInRequest.email && s.passwrord == BCrypt.Net.BCrypt.HashPassword(signInRequest.password));

                var token = generateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                var tokens = new Tokens();
                tokens.userId = user.Id;
                tokens.refreshToken = refreshToken;
                tokens.expiresIn = refreshTokenValidityInDays.ToString();

                db.Tokens.Add(tokens);

                db.SaveChanges();

                signInResponse = new SignInResponse
                {
                    user = new UserResponse
                    {
                        firstName = user.firstName,
                        lastName = user.lastName,
                        email = user.email,
                        Id = user.Id
                    },
                    token = token,
                    refreshToken = refreshToken
                };
            }
            return signInResponse;
        }

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 1h
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public TokenResponse RefreshToken(TokenRequest tokenRequest)
        {
            TokenResponse tokenResponse = null;
            Tokens tokens = GetByRefreshToken(tokenRequest.refreshToken);
            if(tokens != null)
            {
                User user = GetById(tokens.userId);

                var newToken = generateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                tokens.refreshToken = newRefreshToken;
                using(var db = new EntranceTestContext())
                {
                    db.Tokens.Update(tokens);
                }

                tokenResponse = new TokenResponse
                {
                    token = newToken,
                    refreshToken = newRefreshToken
                };    
            }
            return tokenResponse;
        }

        public Tokens GetByRefreshToken(string refreshToken)
        {
            Tokens tokens = null;
            using (var db = new EntranceTestContext())
            {
                tokens = db.Tokens.SingleOrDefault(s => s.refreshToken == refreshToken);
            }
            return tokens;
        }

        public User GetById(int id)
        {
            User user = null;
            using (var db = new EntranceTestContext())
            {
                user = db.Users.SingleOrDefault(s => s.Id == id);
            }
            return user;
        }
    }
}
