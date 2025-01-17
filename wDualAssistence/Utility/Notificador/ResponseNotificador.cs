namespace wDualAssistence.Utility.Notificador
{
    public class ResponseNotificador
    {
        public bool transaccion { get; set; }
        public long id_campana { get; set; }
        public string mensaje { get; set; }
        public string detalle { get; set; }
    }
}