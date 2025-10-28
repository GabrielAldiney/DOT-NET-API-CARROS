using FirstAPI.Model;
using FirstAPI.Infrastructure; // <-- 1. ADICIONE ESTE USING

namespace FirstAPI.Infraestrutura
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

        public List<Carro> Get()
        {
            return _context.Carros.ToList();
        }

        public Carro? Get(int id)
        {
            return _context.Carros.Find(id);
        }
    }
}