using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ACS.Models;
using ACS.Data.Entities;
using ACS.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using ACS.Data.Entities.Tables;
using Org.BouncyCastle.Tsp;
using Renci.SshNet.Common;
using System.Net.Mail;

namespace ACS.Services
{
    
    public interface IUserService
    {
        public static string StudentPassword = "4ywHK0PQNzoL";
        public static string AdminEmail = "coran-sonna@gmail.com";
        public static string AdminPassword = "5Y%x6u2Wx@";
        public UserModel Authenticate(string username, string password);
        public UserModel Create(RegisterModel user);
        public int Delete(int id);
        public List<UserModel> GetAll();
        public bool ConfirmAccount(string username, string uuid);
    }
    public class UserService: IUserService
    {
        private const string name = ClaimTypes.Name;
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public UserModel Create(RegisterModel user)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new TspException("Password is required");

            if (ACS.Data.Access.Tables.UserAccess.GetByUsername(user.Username) != null)
                throw new ScpException("Username '" + user.Username + "' is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);
            var userEntity = user.ToEntity();
            userEntity.PasswordHash = passwordHash;
            userEntity.PasswordSalt = passwordSalt;

            var newId = ACS.Data.Access.Tables.UserAccess.Insert(userEntity);
            var newUser = ACS.Data.Access.Tables.UserAccess.Get(newId);

            var email = "";
            var pwd = "";

            /// sendEmail($"/User/confirm?unm={newUser.Username}&uuid={newUser.UUID}", newUser.Username, email, pwd);

            return new UserModel(newUser);
        }
        public UserModel Authenticate(string username, string password)
        {
            var user = GetUser(username, password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(GetName(), user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }
        public int Delete(int id)
        {
            return ACS.Data.Access.Tables.UserAccess.Delete(id);
        }
        public bool ConfirmAccount(string username, string uuid)
        {
            return ACS.Data.Access.Tables.UserAccess.GetByUsernameUuid(username, uuid) != null;
        }
        public List<UserModel> GetAll()
        {
            var users = ACS.Data.Access.Tables.UserAccess.Get();
            var u = new List<UserModel> { };
            if (users != null || users.Count > 0)
            {
                foreach (var item in users)
                {
                    u.Add(new UserModel(item));
                }
            }

            return u;
        }

        private static string GetName()
        {
            return name;
        }

        private static UserModel GetUser(string username, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = ACS.Data.Access.Tables.UserAccess.GetByUsernamePassword(username, passwordHash, passwordSalt);
            
            if (user == null)
                return null;

            return new UserModel(user);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            var salt = System.Text.Encoding.UTF8.GetBytes("my salt XXX SSSS WWW DD CCV VVVV");

            passwordSalt = System.Text.Encoding.UTF8.GetBytes(password);
            passwordHash = new System.Security.Cryptography.HMACSHA256(salt).ComputeHash(passwordSalt);
        }

        public static void sendEmail(string link, string emailTo, string emailFrom, string emailFromPwd)
        {
            if (!ACS.Helpers.RegexUtils.IsValidEmail(emailTo))
                return;

            var EmailSubject = "ACS Registration";
            DateTime eastern = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");

            string EmailHead = $"[Confirmation]";

            var EmailMessageBody = "Thank you for registering on ACS. Please click the link below ton confirm your registration."
                                    + "<br>"
                                    + $"{link}";


            var mail = new System.Net.Mail.MailMessage();
            mail.To.Add(emailTo);
            mail.From = new MailAddress(emailFrom, EmailHead, System.Text.Encoding.UTF8);
            mail.Subject = EmailSubject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = EmailMessageBody;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(emailFrom, emailFromPwd);
            client.Port = 587;
            client.Host = " smtp.gmail.com";
            client.EnableSsl = true;
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}