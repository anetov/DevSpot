using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DevSpot.Models
{
    public class JobPosting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Location { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; }

        // Ссылка на Id пользователя, который создал JobPosting
        [Required]
        public string UserId { get; set; }

        // Дает возможность на ссылание на объект пользователя(IdentityUser)
        // Если не было бы ForeignKey, то пользователя получали бы с помощью query, ссылаясь по Id
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
