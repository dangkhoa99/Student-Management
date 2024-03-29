﻿using _51800882_51800187_QLSinhVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using _51800882_51800187_QLSinhVien.Res;

namespace _51800882_51800187_QLSinhVien.Controllers
{
    public class SinhVienController : Controller
    {
        // GET: SinhVien
        SinhVienDAO dao = new SinhVienDAO(new QLSVContext());
        string apiUrl = "https://localhost:44328/api/";

        // Danh sách sinh viên
        [Authorize(Roles = "admin, user")]
        public ActionResult Index()
        {
            ViewBag.Loai = 0;
            var db = new QLSVContext();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            IList<SinhVien> sv = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                //HTTP GET
                var responseTask = client.GetAsync("SinhVienAPI");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SinhVien>>();
                    readTask.Wait();

                    sv = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact admin.");
                }
            }
            return View(sv);
        }

        // Danh sách sinh viên lọc theo Khoa
        [Authorize(Roles = "admin, user")]
        public ActionResult IndexByMaKhoa(string makhoa)
        {
            var db = new QLSVContext();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", makhoa);
            KhoaDAO khoadao = new KhoaDAO(new QLSVContext());
            ViewBag.Loai = 1;
            ViewBag.TenKhoa = khoadao.GetTenKhoaByMaKhoa(makhoa);
            IList<SinhVien> sv = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                //HTTP GET
                var responseTask = client.GetAsync("SinhVienAPI?makhoa=" + makhoa);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SinhVien>>();
                    readTask.Wait();

                    sv = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact admin.");
                }
            }
            return View("Index", sv);
        }

        // Tạo form Create SV
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            var db = new QLSVContext();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");

            List<SelectListItem> GioiTinh = new List<SelectListItem>() {
                new SelectListItem(){Text="Nam", Value="Nam"},
                new SelectListItem(){Text="Nữ", Value="Nữ"}
            };

            ViewBag.GioiTinh = new SelectList(GioiTinh, "Value", "Text");

            return View();
        }

        // Create SV
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create(SinhVien sv)
        {
            if (ModelState.IsValid)
            {
                var isExists = dao.GetByID(sv.MaSV);

                if (isExists == null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);

                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<SinhVien>("SinhVienAPI", sv);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, LangResource.messExistsStudentID);
                }
            }
            var db = new QLSVContext();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", sv.MaKhoa);
            List<SelectListItem> GioiTinh = new List<SelectListItem>() {
                new SelectListItem(){Text="Nam", Value="Nam"},
                new SelectListItem(){Text="Nữ", Value="Nữ"}
            };

            ViewBag.GioiTinh = new SelectList(GioiTinh, "Value", "Text", sv.GioiTinh);

            return View(sv);
        }

        // Tạo form Edit SV
        [Authorize(Roles = "admin")]
        public ActionResult Edit(string id)
        {
            SinhVien sv = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                //HTTP GET
                var responseTask = client.GetAsync("SinhVienAPI?id=" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<SinhVien>();
                    readTask.Wait();

                    sv = readTask.Result;
                }
            }

            var db = new QLSVContext();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", sv.MaKhoa);
            List<SelectListItem> GioiTinh = new List<SelectListItem>() {
                new SelectListItem(){Text="Nam", Value="Nam"},
                new SelectListItem(){Text="Nữ", Value="Nữ"}
            };

            ViewBag.GioiTinh = new SelectList(GioiTinh, "Value", "Text", sv.GioiTinh);



            return View(sv);
        }

        // Edit SV
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(SinhVien sv)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    //HTTP POST
                    var putTask = client.PutAsJsonAsync<SinhVien>("SinhVienAPI", sv);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            var db = new QLSVContext();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", sv.MaKhoa);
            List<SelectListItem> GioiTinh = new List<SelectListItem>() {
                new SelectListItem(){Text="Nam", Value="Nam"},
                new SelectListItem(){Text="Nữ", Value="Nữ"}
            };

            ViewBag.GioiTinh = new SelectList(GioiTinh, "Value", "Text", sv.GioiTinh);
            return View(sv);
        }
    }
}