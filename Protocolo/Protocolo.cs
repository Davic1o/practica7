using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Protocolo
{
    public class Protocolo
    {
        private readonly NetworkStream flujo;

        public Protocolo(NetworkStream flujo)
        {
            this.flujo = flujo ?? throw new ArgumentNullException(nameof(flujo));
        }

        // Envía un pedido y espera una respuesta
        public Respuesta ResolverPedido(Pedido pedido)
        {
            try
            {
                // Serializa el pedido y lo envía al servidor
                string mensaje = pedido.ToString();
                byte[] bufferTx = Encoding.UTF8.GetBytes(mensaje);
                flujo.Write(bufferTx, 0, bufferTx.Length);

                // Recibe la respuesta del servidor
                byte[] bufferRx = new byte[1024];
                int bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length);
                string respuestaTexto = Encoding.UTF8.GetString(bufferRx, 0, bytesRx);

                // Procesa la respuesta
                return Respuesta.Procesar(respuestaTexto);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("Error en la comunicación: " + ex.Message, ex);
            }
        }
    }

    public static class Respuesta
    {
        public static Respuesta Procesar(string mensaje)
        {
            var partes = mensaje.Split(' ', 2);
            return new Respuesta
            {
                Estado = partes[0],
                Mensaje = partes.Length > 1 ? partes[1] : string.Empty
            };
        }
    }
}
