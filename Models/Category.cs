using System.ComponentModel.DataAnnotations.Schema;

namespace InforumBackend.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
