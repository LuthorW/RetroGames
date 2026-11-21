using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private const string NomeAdministradorInicial = "Administrador";

    private readonly IRepositorySet _repositorio;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> passwordHasher)
    {
        _repositorio = repositorio;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await _repositorio.Usuario.BuscarPorEmailAsync(email.Trim());

        // E-mail inexistente ou usuário inativo: null.
        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        // O PasswordHasher gera o hash da senha digitada, com o mesmo salt guardado
        // dentro de ds_senha_hash, e compara com o hash gravado. A senha nunca é guardada.
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string email, string senha)
    {
        // Subir a API de novo não cria um segundo administrador.
        if (await _repositorio.Usuario.ExisteAdministradorAsync())
        {
            return;
        }

        var administrador = new Usuario
        {
            Nome = NomeAdministradorInicial,
            Email = email.Trim(),
            Ativo = true,
            IdPerfil = PerfilIds.Administrador,
        };
        administrador.SenhaHash = _passwordHasher.HashPassword(administrador, senha);

        administrador.Id = await _repositorio.Usuario.InserirAsync(administrador);
    }
}
