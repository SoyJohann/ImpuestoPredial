using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services.SUser
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnection()
        {
            return _configuration.GetConnectionString("MasterConnection")
                ?? _configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> Add(Usuarios usuarios)
        {
            var connectionString = GetConnection();
            using var con = new NpgsqlConnection(connectionString);
            usuarios.clave = PasswordHash("123");
            usuarios.usuario_creo = 3;
            await con.OpenAsync();
            var query = "INSERT INTO usuarios(nombre,dir,tel,login,clave,nivel,correo,habilitado,municipio,mun) VALUES(@nombre,@dir,@tel,@login,@clave,@nivel,@correo,@habilitado,@municipio,@mun);";
            return await con.ExecuteAsync(query, usuarios);
        }

        public bool ValidatePasswordHash(string password, string dbPassword)
        {
            byte[] dbPasswordHashBytes = Convert.FromBase64String(dbPassword);
            byte[] salt = new byte[16];
            Array.Copy(dbPasswordHashBytes, 0, salt, 0, 16);

            var userPasswordBytes = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] userPasswordHash = userPasswordBytes.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (dbPasswordHashBytes[i + 16] != userPasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<int> DeleteUsuarioById(int id)
        {
            var connectionString = GetConnection();
            using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            var query = "DELETE FROM usuarios WHERE id = @id";
            return await con.ExecuteAsync(query, new { id });
        }

        public async Task<int> EditUsuarios(Usuarios usuarios)
        {
            var connectionString = GetConnection();
            using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            var query = "UPDATE usuarios SET nombre=@nombre,dir=@dir,tel=@tel,login=@login,nivel=@nivel,correo=@correo,habilitado=@habilitado,municipio=@municipio,mun=@mun WHERE id = @id";
            return await con.ExecuteAsync(query, usuarios);
        }

        public async Task<List<Usuarios>> GetAll()
        {
            var connectionString = GetConnection();
            using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            var query = "SELECT * FROM usuarios";
            return (await con.QueryAsync<Usuarios>(query)).ToList();
        }

        private string PasswordHash(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public async Task<int> Reset_key(int pid)
        {
            var connectionString = GetConnection();
            string pclave = PasswordHash("123");
            using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            var query = "UPDATE usuarios SET clave=@pclave WHERE id = @pid";
            return await con.ExecuteAsync(query, new { pclave, pid });
        }

        public async Task<int> Change_key(int pid, string skey)
        {
            var connectionString = GetConnection();
            string pclave = PasswordHash(skey);
            using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            var query = "UPDATE usuarios SET clave=@pclave WHERE id = @pid";
            return await con.ExecuteAsync(query, new { pclave, pid });
        }

        public async Task<Usuarios> GetUserById(int id)
        {
            var connectionString = GetConnection();
            using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            var query = "SELECT * FROM usuarios WHERE id = @id";
            return await con.QueryFirstOrDefaultAsync<Usuarios>(query, new { id });
        }

        public async Task<List<Usuarios>> GetUserByName(string nombre)
        {
            var connectionString = GetConnection();
            using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            var query = "SELECT * FROM \"usuarios\" WHERE nombre = @nombre";
            return (await con.QueryAsync<Usuarios>(query, new { nombre })).ToList();
        }
    }
}
