using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace ClienteEcoNoBloqueante
{
    class Program
    {
        static void Main(string[] args)
        {
            //Inicialización de la clase tipo String
            // llamado servidor como localhost
            String servidor = "localhost";
            //Inicialización de buffer de transmisión
            //e inicialización de buffer de recepción
            byte[] buferTx = new byte[512];
            byte[] buferRx = new byte[512];
            //inicialización de puerto con el que se trabajará
            int puerto = 8080;
            //Inicialización de socket cliente en null
            Socket socketCliente = null;
            try
            {
                //En caso de que existe alguna excepción
                //Creo un objeto del tipo socket que paso como parametros
                //de entrada la direccion, tipo de socket y protocolo
                socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
                //Realizo la conexión del socket mediante una direccion 
                //y un puerto
                socketCliente.Connect(new IPEndPoint(Dns.GetHostEntry(servidor).AddressList[1],
                puerto));
            }
            catch (Exception e)
            {
                //Mensaje de error
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
            //Inicialización de contadores de tipo int
            //Que representan los bytes enviados y recibidos
            int totalBytesEnviados = 0;
            int totalBytesRecibidos = 0;
            //Almacenamiento en el buffer transmisión 
            //la cadena codificada en ascci

            buferTx = Encoding.ASCII.GetBytes("preparando: 5,3,2,1,0, GG well played!");

            //buferTx = Encoding.ASCII.GetBytes("ESTOY ENVIANDO TODO ESTOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO!");

            //Socket del cliente no se le permite bloquear
            socketCliente.Blocking = false;
            //sentencia de control while
            //Controla que los bytes rrecibidos sean menores que el tamaño del buffer
            while (totalBytesRecibidos < buferTx.Length)
            {
                //Sentencia condicional en caso que los bytes enviados sean
                //menores que el tamaño del bufer
                if (totalBytesEnviados < buferTx.Length)
                {
                    //Se procede a realizar control de errores
                    try
                    {
                        //Si existe incoherencia en el numero de
                        //bytes enviados
                        totalBytesEnviados += socketCliente.Send(buferTx, totalBytesEnviados,
                        buferTx.Length - totalBytesEnviados, SocketFlags.None);
                        Console.WriteLine("Se han enviado un total de {0} bytes al servidor...",
                        totalBytesEnviados);
                    }
                    catch (SocketException se)
                    {
                        //Aviso que se ha establecido una excepcion
                        if (se.ErrorCode == 10035)
                        {
                            //WSAEWOULDBLOCK: Recurso temporalmente no disponible
                            Console.WriteLine("Temporalmente no es posible enviar, se reintentará despues...");
                        }

                        else
                        {
                            Console.WriteLine(se.ErrorCode + ": " + se.Message);
                            socketCliente.Close();
                            Environment.Exit(se.ErrorCode);
                        }
                    }
                }
                try
                {
                    //En caso que se esté recibiendo el total de 
                    //bytes desde el servidor se procede a corregir
                    //el error
                    int bytesRecibidos = 0;
                    if ((bytesRecibidos = socketCliente.Receive(buferRx, totalBytesRecibidos,
                    buferRx.Length - totalBytesRecibidos, SocketFlags.None)) == 0)
                    {
                        Console.WriteLine("La conexion se cerro prematuramente...");
                        break;
                    }
                    totalBytesRecibidos += bytesRecibidos;
                }
                catch (SocketException se)
                {

                    //Excepción en caso de
                    //bytes recibidos
                    if (se.ErrorCode == 10035)
                        continue;
                    else
                    {
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                        break;
                    }
                }
                //Invocación del método realizar procesamiento
                RealizarProcesamiento();
            }
            //Codificación en asccii de los bytes recibidos
            //que se encuentran en el buffer de recepción
            Console.WriteLine("Se han recibido {0} bytes desde el servidor: {1}", totalBytesRecibidos,
            Encoding.ASCII.GetString(buferRx, 0, totalBytesRecibidos));
            //cerramos el socket
            socketCliente.Close();
            //Establecimiento de tiempo de comunicación entre el cliente y servidor
        }
        static void RealizarProcesamiento()
        {
            Console.WriteLine(".");
            Thread.Sleep(2000);
        }
    }
}
