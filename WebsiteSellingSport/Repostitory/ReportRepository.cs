using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebsiteSellingSport.Areas.Admin.ModelView;
using WebsiteSellingSport.Models;

namespace WebsiteSellingSport.Repostitory
{
    public class ReportRepository
    {
       private readonly  QLBHQAContext _context;
        public ReportRepository(QLBHQAContext qLBHQAContext)
        {
            
                _context = qLBHQAContext;
            
        }
        public List<ReportView> GetFilterReportView(DateTime dateStart, DateTime dateEnd)
        {
            var reports = new List<ReportView>();

            // Tạo danh sách các ngày cần thống kê
            var dates = Enumerable.Range(0, (dateEnd - dateStart).Days + 1)
                                  .Select(i => dateStart.AddDays(i))
                                  .ToList();

            // Duyệt qua từng ngày và tính toán doanh thu, tổng đơn hàng
            foreach (var date in dates)
            {
                var revenue = _context.OrderDetails
                            .Where(od => od.Order.OrderDate.Date == date.Date && od.Order.Status == 3)
                            .Sum(od => od.Quantity * od.Price);

                var totalOrders = _context.Orders
                                    .Where(o => o.OrderDate.Date == date.Date && o.Status == 3)
                                    .Count();

                ReportView reportView = new ReportView
                {
                    DateValue = date,
                    Order = totalOrders,
                    TotalMoney = revenue
                };

                reports.Add(reportView);
            }

            return reports;
        }



    }
}
