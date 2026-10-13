using GestorDeMercados.Models;

namespace GestorDeMercados.Repositories.Interfaces
{
    public interface IPago
    {
        IEnumerable<Pago> Listar();
        Pago? ObtenerPorId(int id);
        Pago? ObtenerPorDeudaId(int deudaId);
        Pago RegistrarPago(Pago pago);
        bool Eliminar(int id);
        IEnumerable<Pago> ObtenerPagosDeHoy();

    }
}
