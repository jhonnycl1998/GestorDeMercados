using GestorDeMercados.Models;

namespace GestorDeMercados.Repositories.Interfaces
{
    public interface IRol
    {
        IEnumerable<Rol> Listar();
        Rol? Buscar(int id);
        void Registrar(Rol rol);
        void Actualizar(Rol rol);
        void Eliminar(int id);
    }
}
