namespace FirstAPI.Application.ViewModel
{
    public class CarroViewModel
    {
        public string? Name { get; set; } // Adicione o '?'
        public int Age { get; set; }

        public IFormFile Photo { get; set; }
    }
}
