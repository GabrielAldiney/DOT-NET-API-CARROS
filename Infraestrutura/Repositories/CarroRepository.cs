using FirstAPI.Infrastructure;
using FirstAPI.Domain.Model;
using FirstAPI.Domain.DTOs;

namespace FirstAPI.Infraestrutura.Repositories
{
    public class CarroRepository : ICarroRepository
    {
        // 2. Apenas declare o campo, não crie a instância
        private readonly ConnectionContext _context;

        // 3. Crie um construtor que "recebe" o context
        public CarroRepository(ConnectionContext context)
        {
            _context = context; // O .NET vai injetar o context aqui
        }

        public void add(Carro carro)
        {
            _context.Carros.Add(carro);
            _context.SaveChanges();
        }

        public List<CarroDTO> Get(int pageNumber, int pageQuantity)
        {
            return _context.Carros.Skip(pageNumber * pageQuantity).Take(pageQuantity).Select(b => new CarroDTO { Id = b.id, NameCarro = b.name, Photo = b.photo }).ToList();
        }

        public Carro? Get(int id)
        {
            return _context.Carros.Find(id);
        }
    }
}