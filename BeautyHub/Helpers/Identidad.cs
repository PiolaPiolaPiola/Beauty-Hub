using System.Globalization;
using System.Security.Claims;
using BeautyHub.Models;

namespace BeautyHub.Helpers
{
    public static class Identidad
    {
        public const string ClaimIdCliente = "beautyhub:id_cliente";
        public const string ClaimIdProfesional = "beautyhub:id_profesional";

        public static List<Claim> Crear(string nombreUsuario, string rol, int? idCliente, int? idProfesional)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, nombreUsuario),
                new(ClaimTypes.Role, rol)
            };

            if (idCliente.HasValue)
            {
                claims.Add(new Claim(ClaimIdCliente, idCliente.Value.ToString()));
            }

            if (idProfesional.HasValue)
            {
                claims.Add(new Claim(ClaimIdProfesional, idProfesional.Value.ToString()));
            }

            return claims;
        }

        public static int? IdCliente(ClaimsPrincipal usuario) => LeerEntero(usuario, ClaimIdCliente);

        public static int? IdProfesional(ClaimsPrincipal usuario) => LeerEntero(usuario, ClaimIdProfesional);

        public static string FormatearPrecio(decimal precio)
        {
            return "$" + precio.ToString("N0", new CultureInfo("es-CO"));
        }

        private static int? LeerEntero(ClaimsPrincipal usuario, string tipo)
        {
            var valor = usuario.FindFirst(tipo)?.Value;
            return int.TryParse(valor, out var id) ? id : null;
        }
    }
}
