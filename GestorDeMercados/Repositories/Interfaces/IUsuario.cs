using GestorDeMercados.Models;

namespace GestorDeMercados.Repositories.Interfaces
{
    public interface IUsuario
    {
        IEnumerable<Usuario> Listar();
        Usuario? Buscar(int id);
        void Registrar(Usuario usuario);
        void Actualizar(Usuario usuario);
        void Eliminar(int id);
        Usuario? ObtenerPorUsuario(string usuario);
    }
}
