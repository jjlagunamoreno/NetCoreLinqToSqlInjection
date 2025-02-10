namespace NetCoreLinqToSqlInjection.Models
{
    public class Coche: ICoche
    {
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Imagen { get; set; }
        public int Velocidad { get; set; }
        public int VelocidadMaxima { get; set; }
        public Coche()
        {
            Marca = "citroen";
            Modelo = "c15";
            Imagen = "c15.jpg";
            Velocidad = 0;
            VelocidadMaxima = 120;
        }
        public void Acelerar()
        {
            Velocidad += 10;
            if (Velocidad > VelocidadMaxima)
            {
                Velocidad = VelocidadMaxima;
            }
        }
        public void Frenar()
        {
            Velocidad -= 10;
            if (Velocidad < 0)
            {
                Velocidad = 0;
            }
        }
    }
}
