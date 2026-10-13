using GestorDeMercados.Models;

namespace GestorDeMercados.Repositories.Interfaces
{
    public interface IServicio
    {
        IEnumerable<Servicio> Listar();
        Servicio? Buscar(int id);
        void Registrar(Servicio servicio);
        void Actualizar(Servicio servicio);
        void Eliminar(int id);
    }
}
