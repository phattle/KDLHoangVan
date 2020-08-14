using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebSiteBanHang.Models
{
    [MetadataTypeAttribute(typeof(ThanhVienMetaData))]
    public  partial class ThanhVien
    {
        internal sealed class ThanhVienMetaData
        {

            public int MaThanhVien { get; set; }

            [DisplayName("Tài khoản")]
            [Required(ErrorMessage = "{0} không được bỏ trống" )]
            public string TaiKhoan { get; set; }

            [Required]
            public string MatKhau { get; set; }
            [Required]
            public string HoTen { get; set; }
            [Required]
            public string DiaChi { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string SoDienThoai { get; set; }
            [Required]
            public string CauHoi { get; set; }
            [Required]
            public string CauTraLoi { get; set; }
            [Required]
            public Nullable<int> MaLoaiTV { get; set; }
        }
    }
}