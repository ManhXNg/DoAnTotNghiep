using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public long CustomerId { get; set; }
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "Password không được để trống")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(\d{10}|\d{11})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime DateBirth { get; set; }


        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? Address { get; set; }

       [Required(ErrorMessage = "Giới tính không được để trống")]
        public bool Sex { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
