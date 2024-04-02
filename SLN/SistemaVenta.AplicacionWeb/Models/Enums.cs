namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class Enums
    {
       public  enum StatusBooking
        {
            ReservaSinConfirmar = 1,
            ReservaConfirmada = 2,
            ReservaParcial  =3,
            Ingreso = 4,
            NoIngreso = 5,
            ReservaTerminada = 6,
        }

        public enum PayMethod
        {           
	        Efectivo = 1,
            Electronico = 2,
            Mixto = 3,
	        Credito = 4,
	        EntradaEfectivo = 5,
	        SalidaEfectivo = 6
        }

        public enum TipoDocMov
        {
            Recibo = 1,
            FacturaVenta = 2,
            PedidoProveedor = 3,
            Obsequio = 4,
            Credito = 5,
        }
    }
}
