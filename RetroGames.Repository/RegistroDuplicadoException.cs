namespace RetroGames.Repository;

/// <summary>
/// Lançada quando o MySQL recusa um valor repetido numa coluna UNIQUE (erro 1062).
/// O Service captura esta exceção e a converte no erro de negócio correspondente
/// (por exemplo, PlataformaNomeDuplicado).
/// </summary>
public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
