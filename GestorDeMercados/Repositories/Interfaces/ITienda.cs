using GestorDeMercados.Models;

namespace GestorDeMercados.Repositories.Interfaces
{
    public interface ITienda
    {
        IEnumerable<Tienda> Listar();
        Tienda? Buscar(int id);
        void Registrar(Tienda tienda);
        void Actualizar(Tienda tienda);
        void Eliminar(int id);
    }
}
