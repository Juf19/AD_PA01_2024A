using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Protocolo
{
    // Código anterior
    public class Pedido
    {
        public string Comando { get; set; }
        public string[] Parametros { get; set; }

        public static Pedido Procesar(string mensaje)
        {
            var partes = mensaje.Split(' ');
            return new Pedido
            {
                Comando = partes[0].ToUpper(),
                Parametros = partes.Skip(1).ToArray()
            };
        }

        public override string ToString()
        {
            return $"{Comando} {string.Join(" ", Parametros)}";
        }
    }

    public class Respuesta
    {
        public string Estado { get; set; }
        public string Mensaje { get; set; }

        public override string ToString()
        {
            return $"{Estado} {Mensaje}";
        }
    }
    // Código nuevo
    public class Protocolos
    {
        public Pedido Pedido { get; set; }
        public Respuesta Respuesta { get; set; }

        public Protocolos(string mensajePedido, string mensajeRespuesta)
        {
            Pedido = Pedido.Procesar(mensajePedido);
            Respuesta = new Respuesta
            {
                Estado = mensajeRespuesta.Split(' ')[0],
                Mensaje = mensajeRespuesta.Substring(mensajeRespuesta.IndexOf(' ') + 1)
            };
        }

        public Protocolos(Pedido pedido)
        {
            Pedido = pedido;
        }

        public Respuesta ResolverPedido()
        {
            Respuesta respuesta = new Respuesta
            { Estado = "NOK", Mensaje = "Comando no reconocido" };

            switch (Pedido.Comando)
            {
                case "INGRESO":
                    if (Pedido.Parametros.Length == 2 &&
                        Pedido.Parametros[0] == "root" &&
                        Pedido.Parametros[1] == "admin20")
                    {
                        respuesta = new Random().Next(2) == 0
                            ? new Respuesta { Estado = "OK", Mensaje = "ACCESO_CONCEDIDO" }
                            : new Respuesta { Estado = "NOK", Mensaje = "ACCESO_NEGADO" };
                    }
                    else
                    {
                        respuesta.Mensaje = "ACCESO_NEGADO";
                    }
                    break;

                case "CALCULO":
                    if (Pedido.Parametros.Length == 3)
                    {
                        string placa = Pedido.Parametros[2];
                        if (ValidarPlaca(placa))
                        {
                            byte indicadorDia = ObtenerIndicadorDia(placa);
                            respuesta = new Respuesta
                            { Estado = "OK", Mensaje = $"{placa} {indicadorDia}" };
                        }
                        else
                        {
                            respuesta.Mensaje = "Placa no válida";
                        }
                    }
                    break;

                case "CONTADOR":
                    respuesta = new Respuesta
                    {
                        Estado = "OK",
                        Mensaje = "Contador de solicitudes: 5" // Implementar contador real
                    };
                    break;
            }

            return respuesta;
        }

        private bool ValidarPlaca(string placa)
        {
            return Regex.IsMatch(placa, @"^[A-Z]{3}[0-9]{4}$");
        }

        private byte ObtenerIndicadorDia(string placa)
        {
            int ultimoDigito = int.Parse(placa.Substring(6, 1));
            switch (ultimoDigito)
            {
                case 1: case 2: return 0b00100000; // Lunes
                case 3: case 4: return 0b00010000; // Martes
                case 5: case 6: return 0b00001000; // Miércoles
                case 7: case 8: return 0b00000100; // Jueves
                case 9: case 0: return 0b00000010; // Viernes
                default: return 0;
            }
        }
    }
}
