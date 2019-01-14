namespace Tweeter.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Person")]
    public partial class Person
    {
        public Person()
        {
            Tweets = new List<Tweet>();
            Following = new List<Person>();
            Followers = new List<Person>();
        }

        [Key]
        [StringLength(25)]
        [Required]
        [Display(Name ="User Id")]
        public string user_id { get; set; }

        [Required]
        [StringLength(25)]
        public string Password { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(50)]
        [Required]
        public string Email { get; set; }
       
        public DateTime? Joined { get; set; }

        public bool Active { get; set; }

       

        public virtual List<Tweet> Tweets { get; set; }

        public virtual List<Person> Following { get; set; }

        public virtual List<Person> Followers { get; set; }
    }

    public partial class PersonViewLoginModel
    {
        [StringLength(25)]
        [Required]
        [Display(Name = "User Id")]
        public string user_id { get; set; }

        [Required]
        [StringLength(25)]
        public string Password { get; set; }

    }

    public partial class PersonViewRegisterModel
    {
        [StringLength(25)]
        [Required]
        [Display(Name = "User Id")]
        public string user_id { get; set; }

        [Required]
        [StringLength(25)]
        public string Password { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(50)]
        [Required]
        public string Email { get; set; }
    }
}
