using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebApplication2;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class GioHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // Lây danh sách giỏ hàng
        public List<itemGioHang> LayGioHang()
        {
            List<itemGioHang> lstGioHang = Session["GioHang"] as List<itemGioHang>;
            if(lstGioHang == null)
            {
                //Nếu session bằng  null thì khởi tạo gio hàng
                lstGioHang = new List<itemGioHang>();
                // Gán lại giỏ hàng cho session
                Session["GioHang"] = lstGioHang;
            }
            // nếu giỏ hàng khác null ( đã có sản phẩm trong giỏ ) thì trả về  list
            return lstGioHang;
        }
        //  Thêm sản phẩm bằng cách thông thường ( Load lại trang bằng URL)
        public ActionResult ThemGioHang(int MaSP,string strURL)
        {
            // Kiểm tra trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trả về trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            // nếu != null thì Lấy giỏ hàng
            List<itemGioHang> lstGioHang = LayGioHang();
            // Xét trường hợp sản phẩm được chọn đã có trong giỏ hàng -> tăng số lượng và cập nhật thành tiền
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(spCheck != null)
            {
                // Kiểm tra số lượng tồn kho
                if(spCheck.SoLuong > sp.SoLuongTon)
                {
                    // trả về thông báo hết hàng
                    return View("ThongBao");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                // trả về trang URL hiện tại
                return Redirect(strURL);
            }
            // nếu sp không có trong giỏ hàng -> tạo sp theo MaSP mới rồi add vào giỏ hàng hiện tại
            itemGioHang itemGH = new itemGioHang(MaSP);
            // Kiểm tra số lượng tồn kho
            if (itemGH.SoLuong > sp.SoLuongTon)
            {
                // trả về thông báo hết hàng
                return View("ThongBao");
            }
            lstGioHang.Add(itemGH);
            return Redirect(strURL);

        }
        // Tính tổng số lượng
        public double TinhTongSoLuong()
        {
            // Lấy giỏ hàng từ Session 
            List<itemGioHang> lstGioHang = Session["GioHang"] as List<itemGioHang>;
            if(lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.SoLuong);
        }

        // Tính tổng tiền
        public decimal TinhTongTien()
        {
            List<itemGioHang> lstGioHang = Session["GioHang"] as List<itemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.ThanhTien);
        }

        public ActionResult GioHangPartial()
        {
            //Kiểm tra nếu tổng số lượng = 0 thì trả về View 0
            if (TinhTongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            // Gán trả về ViewBag
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();

            return PartialView();
        }


        // GET: GioHang
        public ActionResult XemGioHang()
        {
            List<itemGioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }


        //Chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang(int maSP)
        {
            // Kiểm tra giỏ hàng tồn tại hay chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Kiểm tra sp có trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Lấy list giỏ hàng từ Session
            List<itemGioHang> lstGioHang = LayGioHang();
            // Kiểm tra sp sửa có tồn tại trong list hay không
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == maSP);
            if(spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Gán lstGioHang qua ViewBag để tạo giao diện chỉnh sửa
            ViewBag.GioHang = lstGioHang;


            //Nếu tồn tại rồi
            return View(spCheck);
        }

        // Chức năng xử lý cập nhật giỏ hàng CapNhatGioHang
        // nhận 1 biến itemGioHang
        [HttpPost]
        public ActionResult CapNhatGioHang(itemGioHang itemGH)
        {
            // Kiểm tra tồn kho
            SanPham spCheck = db.SanPhams.Single(n => n.MaSP == itemGH.MaSP);
            if(spCheck.SoLuongTon< itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            // Cập nhật số lượng trong session giỏ hàng
            List<itemGioHang> lstGioHang = LayGioHang();
            // tìm itemGH trong lstGioHang
            itemGioHang itemGHUpdate = lstGioHang.Find(n => n.MaSP == itemGH.MaSP);
            itemGHUpdate.SoLuong = itemGH.SoLuong;
            // Cập nhật số lượng --> cập nhật thành tiền
            itemGHUpdate.ThanhTien = itemGHUpdate.DonGia * itemGHUpdate.SoLuong;


            //return RedirectToAction("SuaGioHang",new { @maSP = itemGHUpdate.MaSP});
            return RedirectToAction("XemGioHang");
        }

        public ActionResult XoaGioHang(int maSP)
        {
            // Kiểm tra giỏ hàng tồn tại hay chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Kiểm tra sp có trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Lấy list giỏ hàng từ Session
            List<itemGioHang> lstGioHang = LayGioHang();
            // Kiểm tra sp sửa có tồn tại trong list hay không
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == maSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xóa item trong giỏ hàng
            lstGioHang.Remove(spCheck);

            return RedirectToAction("XemGioHang");

        }

        //Xây dựng chức năng đặt hàng
        public ActionResult DatHang(KhachHang kh)
        {
            // Kiểm tra giỏ hàng tồn tại hay chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KhachHang khang = new KhachHang();
            if(Session["TaiKhoan"] == null)
            {
                //Thêm kh vào bảng KhachHang ...khi chưa đăng nhập
                khang = kh;
                db.KhachHangs.Add(khang);
                db.SaveChanges();
            }
            else
            {
                // Thêm kh bằng session Taikhoan
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                khang.TenKH = tv.HoTen;
                khang.DiaChi = tv.DiaChi;
                khang.Email = tv.Email;
                khang.SoDienThoai = tv.SoDienThoai;
                khang.MaThanhVien = tv.MaThanhVien;
                db.KhachHangs.Add(khang);
                db.SaveChanges();
            }
            //Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = khang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            ddh.DaHuy = false;
            ddh.DaXoa = false;
            ddh.MaDDHString = DateTime.Now.Ticks;
            db.DonDatHangs.Add(ddh);
            db.SaveChanges();
            // Thêm chi tiết đơn hàng
            List<itemGioHang> lstGioHang = LayGioHang();
           // double Amoun = 0;
            foreach(var item in lstGioHang)
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.TenSP = item.TenSP;
                ctdh.MaSP = item.MaSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia;
               // Amoun = Amoun + (double) ctdh.DonGia * ctdh.SoLuong;
                db.ChiTietDonDatHangs.Add(ctdh);
            }
            db.SaveChanges();
            //#region "Thanh toán online thẻ nội địa"
            //string SECURE_SECRET = "A3EFDFABA8653DF2342E8DAC29B51AF0";
            //// Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan
            //VPCRequest conn = new VPCRequest("https://mtf.onepay.vn/onecomm-pay/vpc.op");
            //conn.SetSecureSecret(SECURE_SECRET);
            //// Add the Digital Order Fields for the functionality you wish to use
            //// Core Transaction Fields
            //conn.AddDigitalOrderField("Title", "onepay paygate");
            //conn.AddDigitalOrderField("vpc_Locale", "vn");//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
            //conn.AddDigitalOrderField("vpc_Version", "2");
            //conn.AddDigitalOrderField("vpc_Command", "pay");
            //conn.AddDigitalOrderField("vpc_Merchant", "ONEPAY");
            //conn.AddDigitalOrderField("vpc_AccessCode", "D67342C2");
            //conn.AddDigitalOrderField("vpc_MerchTxnRef", ddh.MaDDHString.ToString());
            //conn.AddDigitalOrderField("vpc_OrderInfo", "9704250000000001  NGUYEN VAN A");
            //conn.AddDigitalOrderField("vpc_Amount", (TinhTongTien()*100).ToString());
            //conn.AddDigitalOrderField("vpc_Currency", "VND");
            //conn.AddDigitalOrderField("vpc_ReturnURL", "http://localhost:53174/KetQuaThanhToan/IndexVpc_dr");
            //// Thong tin them ve khach hang. De trong neu khong co thong tin
            //conn.AddDigitalOrderField("vpc_SHIP_Street01", "194 Tran Quang Khai");
            //conn.AddDigitalOrderField("vpc_SHIP_Provice", "Hanoi");
            //conn.AddDigitalOrderField("vpc_SHIP_City", "Hanoi");
            //conn.AddDigitalOrderField("vpc_SHIP_Country", "Vietnam");
            //conn.AddDigitalOrderField("vpc_Customer_Phone", "043966668");
            //conn.AddDigitalOrderField("vpc_Customer_Email", "support@onepay.vn");
            //conn.AddDigitalOrderField("vpc_Customer_Id", "onepay_paygate");
            //// Dia chi IP cua khach hang
            //conn.AddDigitalOrderField("vpc_TicketNo", DateTime.Now.Ticks.ToString());
            //// Chuyen huong trinh duyet sang cong thanh toan
            //String url = conn.Create3PartyQueryString();
            //return Redirect(url);
            //#endregion

            #region "Thanh toán online quốc tế"
            string SECURE_SECRET = "6D0870CDE5F24F34F3915FB0045120DB";
            // Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan
            VPCRequest conn = new VPCRequest("https://mtf.onepay.vn/vpcpay/vpcpay.op");
            conn.SetSecureSecret(SECURE_SECRET);
            // Add the Digital Order Fields for the functionality you wish to use
            // Core Transaction Fields
            conn.AddDigitalOrderField("AgainLink", "http://onepay.vn");
            conn.AddDigitalOrderField("Title", "onepay.vn");
            conn.AddDigitalOrderField("vpc_Locale", "vn");//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
            conn.AddDigitalOrderField("vpc_Version", "2");
            conn.AddDigitalOrderField("vpc_Command", "pay");
            conn.AddDigitalOrderField("vpc_Merchant", "TESTONEPAY");
            conn.AddDigitalOrderField("vpc_AccessCode", "6BEB2546");
            conn.AddDigitalOrderField("vpc_MerchTxnRef", ddh.MaDDHString.ToString());
            conn.AddDigitalOrderField("vpc_OrderInfo", "4000000000000002 | 05/21 | 123 | Tran Quang Khai");
            conn.AddDigitalOrderField("vpc_Amount", (TinhTongTien() * 100).ToString());
            conn.AddDigitalOrderField("vpc_ReturnURL", "http://localhost:53174/KetQuaThanhToan/IndexVpc_drr"); 
            // Thong tin them ve khach hang. De trong neu khong co thong tin
            conn.AddDigitalOrderField("vpc_SHIP_Street01", "194 Tran Quang Khai");
            conn.AddDigitalOrderField("vpc_SHIP_Provice", "Hanoi");
            conn.AddDigitalOrderField("vpc_SHIP_City", "Hanoi");
            conn.AddDigitalOrderField("vpc_SHIP_Country", "Vietnam");
            conn.AddDigitalOrderField("vpc_Customer_Phone", "043966668");
            conn.AddDigitalOrderField("vpc_Customer_Email", "support@onepay.vn");
            conn.AddDigitalOrderField("vpc_Customer_Id", "onepay_paygate");
            // Dia chi IP cua khach hang
            conn.AddDigitalOrderField("vpc_TicketNo", DateTime.Now.Ticks.ToString());
            // Chuyen huong trinh duyet sang cong thanh toan
            String url = conn.Create3PartyQueryString();
           return Redirect(url);
            #endregion
        }


        // Thêm giỏ hàng Ajax
        public ActionResult ThemGioHangAjax(int MaSP, string strURL)
        {
            // Kiểm tra trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trả về trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            // nếu != null thì Lấy giỏ hàng
            List<itemGioHang> lstGioHang = LayGioHang();
            // Xét trường hợp sản phẩm được chọn đã có trong giỏ hàng -> tăng số lượng và cập nhật thành tiền
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                // Kiểm tra số lượng tồn kho
                if (spCheck.SoLuong > sp.SoLuongTon)
                {
                    // trả về thông báo hết hàng
                    return Content("<script>alert(\"Sản phẩm đã hết hàng\")</script>");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                // trả về trang URL hiện tại
                //return Redirect(strURL);
                // Trả về PartialView GioHangPartial --> cập nhật lại ViewBag
                ViewBag.TongTien = TinhTongTien();
                ViewBag.TongSoLuong = TinhTongSoLuong();
                return PartialView("GioHangPartial");
            } 
            // nếu sp không có trong giỏ hàng -> tạo sp theo MaSP mới rồi add vào giỏ hàng hiện tại
            itemGioHang itemGH = new itemGioHang(MaSP);
            // Kiểm tra số lượng tồn kho
            if (itemGH.SoLuong > sp.SoLuongTon)
            {
                // trả về thông báo hết hàng
                return Content("<script>alert(\"Sản phẩm đã hết hàng\")</script>");
            }
            lstGioHang.Add(itemGH);
            ViewBag.TongTien = TinhTongTien();
            ViewBag.TongSoLuong = TinhTongSoLuong();
            return PartialView("GioHangPartial");

        }
    }
}