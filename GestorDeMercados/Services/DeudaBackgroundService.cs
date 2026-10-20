namespace GestorDeMercados.Services
{
    // BackgroundService es una clase de .NET que permite ejecutar tareas en segundo plano
    // mientras la aplicación web está corriendo.
    public class DeudaBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DeudaBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Método principal que se ejecuta automáticamente al iniciar el servidor
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // El bucle se mantiene activo mientras el servidor web esté encendido
            while (!stoppingToken.IsCancellationRequested)
            {
                var ahora = DateTime.Now;

                // LÓGICA DE PROGRAMACIÓN:
                // Actualmente configurado para ejecutarse cada 1 minuto (para pruebas del profesor).
                // En producción, esto se cambiaría para ejecutarse una vez al día (a medianoche).
                var siguienteEjecucion = DateTime.Now.AddMinutes(1);
                var delay = siguienteEjecucion - ahora;

                // El servicio "duerme" hasta que llegue el momento de la siguiente ejecución
                await Task.Delay(delay, stoppingToken);

                // IMPORTANTE: Al ser un servicio de larga duración (Singleton), 
                // necesitamos crear un 'Scope' manual para usar servicios de base de datos (Scoped).
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Obtenemos la lógica de negocio de deudas
                    var deudaService = scope.ServiceProvider.GetRequiredService<DeudaService>();

                    // Ejecuta la generación automática de cobros (Luz, Agua, Alquiler, etc.)
                    deudaService.GenerarDeudasAutomaticas();
                }
            }
        }
    }
}