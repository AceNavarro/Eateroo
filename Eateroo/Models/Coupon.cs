using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Eateroo.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Coupon Type")]
        public string CouponType { get; set; }

        public enum ECouponType
        {
            Dollar,
            Percent
        }

        [Required]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Minimum Amount")]
        public double MinimumAmount { get; set; }

        public byte[] Image { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
    }
}
