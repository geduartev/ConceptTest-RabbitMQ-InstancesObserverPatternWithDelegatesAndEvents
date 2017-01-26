using System;
using System.Threading;
using System.Threading.Tasks;

namespace ObserverPattern
{
    public class Program
    {
        private const int TimeoutCreateInstances = 2000;
        private const int TimeoutCloseSingleInstance = 1;

        private static RabbitInstances rabbitInstances = new RabbitInstances();

        static void Main(string[] args)
        {
            Console.WriteLine("\n\n****** INICIO PROGRAMA ******\n");

            rabbitInstances.WithoutInstances += new EventHandler(InstantlyCreateInstances);

            CancellationTokenSource cancellationTokenSource1 = new CancellationTokenSource();
            CancellationTokenSource cancellationTokenSource2 = new CancellationTokenSource();
            CancellationToken cancellationToken1 = cancellationTokenSource1.Token;
            CancellationToken cancellationToken2 = cancellationTokenSource2.Token;

            // Crear una tarea que cada 2000 milisegundos cree la totalidad de instancias.
            Task task1 = new Task(
                () =>
                {
                    CreateInstances(cancellationToken1);
                });

            // Crear una tarea que cada 500 milisegundos cierre una instancia.
            // Si al cerrar la instancia agota la totalidad de instancias disponibles
            // disparará un evento que creará la totalidad de instancias sin necesidad
            // de esperar a que la tarea encargada lo haga.
            // Emula un cliente que consume la instancia y la cierra.
            Task task2 = new Task(
                () =>
                {
                    CloseInstance(cancellationToken2);
                });

            // Iniciamos las tareas en paralelo.
            task1.Start();
            task2.Start();

            // Esperamos 10 segundos antes de finalizar las tareas y el programa.
            Thread.Sleep(TimeSpan.FromSeconds(10));
            cancellationTokenSource2.Cancel();
            cancellationTokenSource1.Cancel();

            Console.WriteLine("\n\n****** FIN PROGRAMA ******\n\n");
            // Emulando a un cliente que utiliza una instancia y la cierra.
            Console.ReadLine();
        }

        private static void CreateInstances(CancellationToken cancellationToken)
        {
            // Mientras la tarea no sea cancelada...
            while (cancellationToken.IsCancellationRequested == false)
            {
                // Crea las instancias necesarias y espera un tiempo antes de vovler a crearlas.
                rabbitInstances.CreateInstances();
                Thread.Sleep(TimeSpan.FromMilliseconds(TimeoutCreateInstances));
            }
        }

        private static void CloseInstance(CancellationToken cancellationToken)
        {
            // Mientras la tarea no sea cancelada...
            while (cancellationToken.IsCancellationRequested == false)
            {
                // Cerrará UNA instancia emulando que un cliente consume la instancia y la finaliza.
                // Esperará un tiempo antes de volver a cerrar otra.
                rabbitInstances.CloseSingleInstance();
                Thread.Sleep(TimeSpan.FromMilliseconds(TimeoutCloseSingleInstance));
            }
        }

        private static void InstantlyCreateInstances()
        {
            Console.WriteLine("¡Quedan muy pocas instancias! Creando inmediatamente nuevas...");
            rabbitInstances.CreateInstances();
        }
    }
}
