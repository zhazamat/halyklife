using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hbk.Models
{
    [Table("categories")]
    public class Category
    {
        internal object ThanksBoards;

        public Category()
        {
            ThanksBoards = new HashSet<ThanksBoard>();
        }

            [Key, Required]
        public int Id { get; set; }
        [DisplayName("Название категории")]
        public string CategoryName { get; set; } = string.Empty;
        [NotMapped]
        public string CategoryImg { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
     
        public ICollection<ThanksBoard> Messages { get; set; } = new List<ThanksBoard>();
       
    }
}
