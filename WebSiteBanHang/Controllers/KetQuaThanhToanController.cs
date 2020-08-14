using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebApplication2;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class KetQuaThanhToanController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Kết quả thanh toán";
            int result = int.Parse( TempData["result"].ToString());
            long  MaDonHang= long.Parse(TempData["MaDonHang"].ToString());
            string status;
            if (result == 0)
            {
                status = "Giao dịch thành công. Vui lòng kiểm tra email của bạn.";
                Session["GioHang"] = null;
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                DonDatHang ddh = db.DonDatHangs.Where((p) => p.MaDDHString == MaDonHang).FirstOrDefault();
            
                if (ddh != null)
                {
                    string MaTraCuu = string.Format("{0}-{1}", ddh.MaDDHString, ddh.MaDDH);
                    Core.MyQRCode qr = new Core.MyQRCode(5, 4, 3);
                    // Tao image
                    Image img = qr.getImageFromString(MaTraCuu, false);

                    // Convert byte[] to Base64 String
                    ImageConverter imageConverter = new ImageConverter();
                    byte[] imageByte = (byte[])imageConverter.ConvertTo(img, typeof(byte[]));
                    // string base64String = Convert.ToBase64String(imageByte);
                    MemoryStream logo = new MemoryStream(imageByte);
                    // Send email
                    // string body = string.Format("<p>Chào bạn,<br/>ĐƠN HÀNG CỦA BẠN ĐÃ ĐẶT THÀNH CÔNG.<br/>Vui lòng đưa mã QR code này tới quầy vé để nhận vé.</p><br/><img src=\"data:image/png;base64, {0}\">", base64String);
                    string body = "<p>Chào bạn,<br/>ĐƠN HÀNG CỦA BẠN ĐÃ ĐẶT THÀNH CÔNG.<br/>Vui lòng đưa mã QR code này tới quầy vé để nhận vé.</p><br/><br/><img src=cid:imagepath>";
                    AlternateView imgView = AlternateView.CreateAlternateViewFromString(body,null,"text/html");
                    LinkedResource lr = new LinkedResource(logo);
                    lr.ContentId = "imagepath";
                    imgView.LinkedResources.Add(lr);

                    new QuanLyDonHangController().GuiEmail("Xác nhận đơn hàng", lr.ContentId, ddh.KhachHang.Email, imgView);
                }
            }
            else if (result == 1)
            {
                Session["GioHang"] = null;
                status = "Giao dịch đang đợi.";
            }
            else
            {
                status = "Giao dịch không thành công.";
                return RedirectToAction("XemGioHang");
            }
            ViewBag.status = status;
            return View();
        }
        // GET: KetQuaThanhToan
        //public ActionResult IndexVpc_dr()
        //{
        //    string SECURE_SECRET = "A3EFDFABA8653DF2342E8DAC29B51AF0";
        //    string hashvalidateResult = "";
        //    // Khoi tao lop thu vien
        //    VPCRequest conn = new VPCRequest("http://onepay.vn");
        //    conn.SetSecureSecret(SECURE_SECRET);
        //    // Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
        //    hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);

        //    // Lay gia tri tham so tra ve tu cong thanh toan
        //    String vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode", "Unknown");
        //    string amount = conn.GetResultField("vpc_Amount", "Unknown");
        //    string localed = conn.GetResultField("vpc_Locale", "Unknown");
        //    string command = conn.GetResultField("vpc_Command", "Unknown");
        //    string version = conn.GetResultField("vpc_Version", "Unknown");
        //    string cardBin = conn.GetResultField("vpc_Card", "Unknown");
        //    string orderInfo = conn.GetResultField("vpc_OrderInfo", "Unknown");
        //    string merchantID = conn.GetResultField("vpc_Merchant", "Unknown");
        //    string authorizeID = conn.GetResultField("vpc_AuthorizeId", "Unknown");
        //    string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "Unknown");
        //    string transactionNo = conn.GetResultField("vpc_TransactionNo", "Unknown");
        //    string txnResponseCode = vpc_TxnResponseCode;
        //    string message = conn.GetResultField("vpc_Message", "Unknown");
        //    //int loop1;
        //    int result = -1;
        //    long maDDHString = 0;
        //    // Sua lai ham check chuoi ma hoa du lieu
        //    if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
        //    {
        //        maDDHString = long.Parse(merchTxnRef);
        //        result = 0;
        //    }
        //    else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
        //    {
        //        result = 1;
        //    }
        //    else
        //    {
        //        result = 2;
        //    }
        //    TempData["MaDonHang"] = maDDHString;
        //    TempData["result"] = result;
        //    return RedirectToAction("Index", "KetQuaThanhToan");
        //}
        public ActionResult IndexVpc_drr()
        {
            string SECURE_SECRET = "6D0870CDE5F24F34F3915FB0045120DB";
            string hashvalidateResult = "";
            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest("http://onepay.vn");
            conn.SetSecureSecret(SECURE_SECRET);
            // Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);
            // Lay gia tri tham so tra ve tu cong thanh toan
            String vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode", "Unknown");
            string amount = conn.GetResultField("vpc_Amount", "Unknown");
            string localed = conn.GetResultField("vpc_Locale", "Unknown");
            string command = conn.GetResultField("vpc_Command", "Unknown");
            string version = conn.GetResultField("vpc_Version", "Unknown");
            string cardType = conn.GetResultField("vpc_Card", "Unknown");
            string orderInfo = conn.GetResultField("vpc_OrderInfo", "Unknown");
            string merchantID = conn.GetResultField("vpc_Merchant", "Unknown");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId", "Unknown");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "Unknown");
            string transactionNo = conn.GetResultField("vpc_TransactionNo", "Unknown");
            string acqResponseCode = conn.GetResultField("vpc_AcqResponseCode", "Unknown");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message", "Unknown");
            //int loop1;
            int result = -1;
            long maDDHString = 0;
            // Sua lai ham check chuoi ma hoa du lieu
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                maDDHString = long.Parse(merchTxnRef);
                result = 0;
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                result = 1;
            }
            else
            {
                result = 2;
            }
            TempData["MaDonHang"] = maDDHString;
            TempData["result"] = result;
            return RedirectToAction("Index", "KetQuaThanhToan");
        }
    }
}