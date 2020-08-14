using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
namespace WebSiteBanHang.Controllers
{
    public class QuanLyDonHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: QuanLyDonHang
        public ActionResult ChuaThanhToan()
        {
            // Ds đơn hàng chưa duyệt
            var lst = db.DonDatHangs.Where(n => n.DaThanhToan == false).OrderBy(n => n.NgayDat);
            return View(lst);
        }

        public ActionResult ChuaGiao()
        {
            var lst = db.DonDatHangs.Where(n => n.DaThanhToan == true && n.TinhTrangGiaoHang == false).OrderByDescending(n => n.NgayDat);
            return View(lst);
        }

        public ActionResult DaGiaoDaThanhToan()
        {
            var lst = db.DonDatHangs.Where(n => n.DaThanhToan == true && n.TinhTrangGiaoHang == true).OrderByDescending(n => n.NgayDat);
            return View(lst);
        }

        [HttpGet]
        public ActionResult DuyetDonHang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonDatHang model = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);
            if(model == null)
            {
                return HttpNotFound();
            }
            // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(model);
        }

        [HttpPost]
        public ActionResult DuyetDonHang(DonDatHang ddh)
        {
            // Lấy dữ liệu của đơn hàng đó
            DonDatHang ddhUpdate = db.DonDatHangs.Single(n => n.MaDDH == ddh.MaDDH);
            ddhUpdate.DaThanhToan = ddh.DaThanhToan;
            ddhUpdate.TinhTrangGiaoHang = ddh.TinhTrangGiaoHang;
            db.SaveChanges();

            // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddh.MaDDH);
            ViewBag.ListChiTietDH = lstChiTietDH;
            GuiEmail("Xác nhận đơn hàng", "<p>Chào bạn,<br/>ĐƠN HÀNG CỦA BẠN ĐÃ ĐẶT THÀNH CÔNG.</p>", ddhUpdate.KhachHang.Email,null);
            return View(ddhUpdate);
    
        }
        [HttpPost]
        public  JsonResult GuiEmail(string Title, string _description, string mailKH, AlternateView av)
        {
            string senderID = "phatle@hoangvanco.com.vn";
            string senderPassword = "Phatle1144";
            string result = "Email Sent Successfully";

            string body = Title;
            body += _description;
            try
            {
                MailMessage mail = new MailMessage(senderID, mailKH);
                mail.Subject = "XÁC NHẬN ĐƠN HÀNG";
                mail.Body = body;
                mail.IsBodyHtml = true;
                if (av != null)
                {
                    mail.AlternateViews.Add(av);
                }
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                result = "problem occurred";
                Response.Write("Exception in sendEmail:" + ex.Message);
            }
            return Json(result);
        }
        //public void GuiEmail( string ToEmail, string FromEmail, string Content)
        //{
        //    // goi email
        //    MailMessage mail = new MailMessage();
        //    mail.To.Add("phatle@hoangvanco.com.vn"); // Địa chỉ nhận
        //    mail.From = new MailAddress("phatle@hoangvanco.com.vn"); // Địa chửi gửi
        //    mail.Subject = "XÁC NHẬN ĐƠN HÀNG";   // tiêu đề gửi
        //    mail.Body = Content;                 // Nội dung
        //    mail.IsBodyHtml = true;
        //    SmtpClient smtp = new SmtpClient();
        //    smtp.Host = "smtp.gmail.com"; // host gửi của Gmail
        //    smtp.Port = 587;               //port của Gmail
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = new System.Net.NetworkCredential
        //        ("phatle@hoangvanco.com.vn", "Phatle1144");
        //    /*("phatle@hoangvanco.com.vn", "Phatle1144");*///Tài khoản password người gửi
        //    smtp.EnableSsl = true;   //kích hoạt giao tiếp an toàn SSL
        //    smtp.Send(mail);   //Gửi mail đi


        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}