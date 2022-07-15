/*
// Practica 04
// Integrantes:
//      Elvis Toscano
//      Patricio Vaicilla
// Fecha de entrega: 17/06/2022
------------------------------------------
 Resultados Ejercicio 1: 
"   Uso de Timeouts y llamadas no bloqueantes"
   - Existe un tiempo límite de espera en el servidor cuando hay de por medio información que tiene que llegar
   - cuando este tiempo de espera llega a su límite entonces el envío de información no podrá realizarse
   -Además, existe la posibilidad que si se realiza modificaciones en los buffers, específicamente en las longitueds
   - ocasionará que los datos no se envien de manera correcta.
--------------------------------
 Resultados Ejercicio 2: "Framing y codificacion binaria textual"
 
   
   -Al realizar el cambio para que se envíe y se reciban los dos elementos no hay variación en la pérdida de paquetes
   -es decir no hay problema que los dos se envíen o reciban
   -Es importante definir que la forma de representar ya sea binaria o textual es la misma en bits. adicional para la realizacion de la 
   -codificacion y decodificacion es crucial ubicar todas las variables necesarias para su realización

-----------------------------------------------------------------------------------------------------------------------------------
 Conclusiones
  * Debido a que en el uso de timeouts y llamadas no bloqueantes existe un limite de espera, cuando la comunicación falla por algún motivo
  * existe un tiempo límite de espera lo que permite al sistema ser más efectivo y no desperdiciar recursos.
---------------------------------------------------------------------------------------------------------------------------------------------------
 Recomendaciones
  * Tener en cuenta que para la realización de la decodificación y codificación es crucial ubicar todas las variables que se requiera
  *

*/

using System;
using System.Net;
using System.Net.Sockets;
namespace ServidorEcoConTimeout
{
    class Program
    {
        //inicialización de constantes tipo privadas
        //para darle encapsulación y sean de caracter
        //global
        private const int TAM_BUFFER = 32;
        private const int TAM_COLA = 5;
        private const int LIMITE_ESPERA = 10000;
        static void Main(string[] args)
        {
            //Inicialización de puerto
            int puerto = 8080;
            //Inicialización de socket llamado servidor en null
            Socket servidor = null;
            //Tratamiento de errores mediante try
            try
            {
                //En caso de que exista una excepción
                //se inicializará este código.
                //Se crea un objeto del tipo socket en el cual se pasa
                //como parámetros direccion, tipo de socket y tipo de protocolo
                servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
                //Se tiene como parámetros de entrada en el terminal de un 
                //socket una dirección y un puerto
                servidor.Bind(new IPEndPoint(IPAddress.Any, puerto));
                //Socket se encuentra escuchando
                servidor.Listen(TAM_COLA);
            }
            catch (SocketException se)
            {
                //para verificar que se da una excepción se imprime
                //un mensaje de error
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                Environment.Exit(se.ErrorCode);
            }
            //Inicialización de variables usadas en RX
            byte[] buferRx = new byte[TAM_BUFFER];
            int cantBytesRecibidos;
            int totalBytesEnviados = 0;
            //Sentencia de control de tipo iteración 
            //sin salida
            for (; ; )
            {
                Socket cliente = null;
                try
                {

                    //Captura de excepción
                    //Creación de socket dirigido al servidor
                    cliente = servidor.Accept();
                    DateTime tiempoInicio = DateTime.Now;
                    //Método usado por el socket que define un 
                    //limite de espera
                    cliente.SetSocketOption(SocketOptionLevel.Socket,
                    SocketOptionName.ReceiveTimeout, LIMITE_ESPERA);
                    //Visualización extremo remoto del cliente
                    Console.Write("Gestionando al cliente: " + cliente.RemoteEndPoint
                    + " - ");
                    totalBytesEnviados = 0;
                    //sentencia de control iterativa sin asignar un
                    //límite de iteraciones
                    //relaciona cantidad de bytes recibidos frente a la cantidad de bytes
                    //que debería recibir el cliente 
                    while ((cantBytesRecibidos = cliente.Receive(buferRx, 0,
                    buferRx.Length, SocketFlags.None)) > 0)
                    {
                        //Preparación del servidor para la transferencia de datos
                        cliente.Send(buferRx, 0, cantBytesRecibidos,
                        SocketFlags.None);
                        totalBytesEnviados += cantBytesRecibidos;
                        TimeSpan tiempoTranscurrido = DateTime.Now - tiempoInicio;
                       //Sentencia de control condicional del tiempo límitede espera
                        if (LIMITE_ESPERA - tiempoTranscurrido.TotalMilliseconds <
                        0)
                        {  
                            //Tiempo finaliza con el cierre de la conexion
                            //con el cliente
                            Console.WriteLine("Terminando la conexión con el cliente debido al temporizador.Se han superado los "
                                + LIMITE_ESPERA + "ms; se han enviado " + totalBytesEnviados
                            + " bytes");
                            cliente.Close();
                            throw new SocketException(10060);
                        }
                        //Para un tipo de valor entero se realiza el establecimiento
                        //de un socket
                        cliente.SetSocketOption(SocketOptionLevel.Socket,
                        SocketOptionName.ReceiveTimeout, (int)(LIMITE_ESPERA -
                        tiempoTranscurrido.TotalMilliseconds));
                    }
                    //Impresión del número de bytes (8bits) a enviarse
                    Console.WriteLine("Se han enviado {0} bytes.",
                    totalBytesEnviados);
                    cliente.Close();
                }
                catch (SocketException se)
                {
                    //Sentencia de control condicional que permite controlar
                    //el total de información enviada
                    //En caso de suceder esto, se visualizará un sms de error
                    //Finalmente se acaba la conexión
                    if (se.ErrorCode == 10060)
                    {
                        //Terminación de la conexión
                        Console.WriteLine("Terminado la conexion debido al temporizador.Han transcurrido " 
                            + LIMITE_ESPERA + "ms; se han transmitido " + totalBytesEnviados + " bytes");
                    }
                    else
                    {
                        //Mensaje de error
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                    }
                    cliente.Close();
                }
            }
        }
    }
}