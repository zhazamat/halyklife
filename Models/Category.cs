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
        public string Name { get; set; } = string.Empty;
        public ICollection<ThanksBoard> Messages { get; set; } = new List<ThanksBoard>();

    }
}
