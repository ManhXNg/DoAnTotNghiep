using System.Linq;
using WebsiteSellingSport.Models;

namespace WebsiteSellingSport.Repostitory
{
    public class LoginRepository
    {
        QLBHQAContext _context;
        public LoginRepository(QLBHQAContext qLBHQA)
        {
            _context = qLBHQA;  
        }
        public int CreateCustomer(Customer customer)
        {
            try
            {
                string password = string.IsNullOrEmpty(customer.Password) ? "" : Common.MD5.EncryptPassword(customer.Password);
                var temp = _context.Customers.Where(c => c.Email == customer.Email).ToList();
                if (temp.Count < 1)
                {
                    customer.Password = password;
                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                    return 1;//Đăng ký thành công
                }
                else
                {
                    return 2; //Tài khoản đã tồn tại
                }
            }
            catch 
            {

                return 0;
            }

        }
        public Customer Login(Customer customer)
        {
            Customer temp = new Customer();
            try
            {
                string password = string.IsNullOrEmpty(customer.Password) ? "" : Common.MD5.EncryptPassword(customer.Password);
                temp = _context.Customers.Where(c => c.Email == customer.Email && c.Password == password).FirstOrDefault();
                if (temp != null)
                {
                    
                    return temp;//Đăng nhập thành công
                }
                else
                {
                    return temp; //Email hoặc mật khẩu không chính xác
                }
            }
            catch
            {

                return temp;
            }

        }

    }
}
