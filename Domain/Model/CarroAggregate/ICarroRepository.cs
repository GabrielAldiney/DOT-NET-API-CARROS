using FirstAPI.Domain.DTOs;

namespace FirstAPI.Domain.Model.CarroAggregate
{
    public interface ICarroRepository
    {
        // Os métodos aqui devem ser idênticos aos métodos
        // públicos do seu CarroRepository
        void add(Carro carro);
        List<CarroDTO> Get(int pageNumber, int pageQuantity);

        Carro? Get(int id);
    }
}