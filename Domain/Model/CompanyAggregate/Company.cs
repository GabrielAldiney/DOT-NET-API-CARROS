using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Domain.Model.CompanyAggregate
{
    public class Company
    {

        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }   
    }
}
