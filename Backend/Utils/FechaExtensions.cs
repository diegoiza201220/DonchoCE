namespace Utils
{
    public static class FechaExtensions
    {
        public static int ToIntFecha(this DateTime fecha)
            => int.Parse(fecha.ToString("yyyyMMdd"));
    }
}
