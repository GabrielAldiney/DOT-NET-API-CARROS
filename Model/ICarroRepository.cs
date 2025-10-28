using FirstAPI.Model; // Importante para ele saber o que é a classe 'Carro'

namespace FirstAPI.Infraestrutura
{
    public interface ICarroRepository
    {
        // Os métodos aqui devem ser idênticos aos métodos
        // públicos do seu CarroRepository
        void add(Carro carro);
        List<Carro> Get();

        Carro? Get(int id);
    }
}