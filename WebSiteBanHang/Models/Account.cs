using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSiteBanHang.Models
{
    public class Account
    {
        [Required(ErrorMessage = "Chưa Nhập Tài Khoản ")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "Chưa Nhập Mật Khẩu ")]
        public string MatKhau { get; set; }
    }
}