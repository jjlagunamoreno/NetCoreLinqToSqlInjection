namespace NetCoreLinqToSqlInjection.Models
{
    public class Deportivo: ICoche
    {
        public Deportivo()
        {
            Marca = "Fiat";
            Modelo = "Punto";
            Imagen = "fiat-punto.jpg";
            VelocidadMaxima = 320;
        }

        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Imagen { get; set; }
        public int Velocidad { get; set; }
        public int VelocidadMaxima { get; set; }

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
