using GestorDeMercados.Models;

namespace GestorDeMercados.Repositories.Interfaces
{
    public interface IDeuda
    {
        IEnumerable<Deuda> Listar();
        Deuda? Buscar(int id);
        void Registrar(Deuda deuda);
        void Actualizar(Deuda deuda);
        void Eliminar(int id);
        bool ExisteDeuda(int tiendaId, int servicioId, DateTime fechaFacturacion);

        IEnumerable<Deuda> ObtenerPendientesPorDueno(int duenoId);
    }
}
