using FirstAPI.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FirstAPI.Domain.Model;
using FirstAPI.Application.ViewModel;
using AutoMapper;
using FirstAPI.Domain.DTOs;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("api/v1/carro")]
    public class CarroController : ControllerBase
    {
        private readonly ICarroRepository _carroRepository;
        private readonly ILogger<CarroController> _logger;
        private readonly IMapper _mapper;

        public CarroController(ICarroRepository carroRepository, ILogger<CarroController> logger, IMapper mapper)
        {
            _carroRepository = carroRepository ?? throw new ArgumentNullException(nameof(carroRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public IActionResult add([FromForm] CarroViewModel carroView)
        {
            // --- INÍCIO DA CORREÇÃO ---

            var filePath = Path.Combine("Storage", carroView.Photo.FileName);

            // 1. Se carroView.Name for nulo, use string.Empty
            var nomeCarro = carroView.Name ?? string.Empty;

            using Stream fileStream = new FileStream(filePath, FileMode.Create);
            carroView.Photo.CopyTo(fileStream);

            // 2. Passe string.Empty para a foto em vez de null
            var carro = new Carro(nomeCarro, carroView.Age, filePath);

            // --- FIM DA CORREÇÃO ---

            _carroRepository.add(carro);

            return Ok();
        }
        [Authorize]
        [HttpPost]
        [Route("{id}/download")]
        public IActionResult DownloadPhoto(int id)
        {
            var carro = _carroRepository.Get(id);
            var dataBytes = System.IO.File.ReadAllBytes(carro.photo);
            return File(dataBytes, "image/png");
        }

        [HttpGet]
        public IActionResult Get(int pageNumber, int pageQuantity)
        {
            _logger.Log(LogLevel.Error, "Teve um erro");
            
            var carros = _carroRepository.Get(pageNumber, pageQuantity);

            _logger.LogInformation("teste");

            return Ok(carros);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Search(int id)
        {

            try
            {
                var carros = _carroRepository.Get(id);
                var carrosDTOs = _mapper.Map<CarroDTO>(carros);
                return Ok(carrosDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }
    }
}