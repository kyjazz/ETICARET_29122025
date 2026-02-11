using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Order
    {
        public int Id { get; set; } //siparişin benzersiz kimliği
        public string OrderNumber { get; set; } //sipariş numarası
        public DateTime OrderDate { get; set; } //sipariş tarihi
        public string UserId { get; set; } //siparişi veren kullanıcı kimliği
        public string FirstName { get; set; } //kullanıcı adı
        public string LastName { get; set; } //kullanıcı soyadı
        public string Address { get; set; } //teslimat adresi
        public string Phone { get; set; } //telefon numarası
        public string City { get; set; }
        public string Email { get; set; } //e-posta adresi
        public string OrderNote { get; set; } //sipariş notu
        public string PaymentId { get; set; } //ödeme kimliği
        public string PaymentToken { get; set; } //ödeme tokeni
        public string ConversionId { get; set; } //dönüşüm kimliği
        public EnumOrderState OrderState { get; set; } //sipariş durumu
        public EnumPaymentTypes PaymentTypes { get; set; } //ödeme türü
        public List<OrderItem> OrderItems { get; set; } //sipariş içindeki ürünlerin listesi
        public Order()
        {
            OrderItems = new List<OrderItem>(); //boş liste ile başlatma
        }

    }

    public enum EnumOrderState
    {
        waiting = 0, //sipariş beklemede
        unpaid = 1, //ödeme yapılmamış
        completed = 2, //sipariş tamamlanmış
    }
    public enum EnumPaymentTypes
    {
        CreditCard = 0, // kredi kartı
        Eft = 1,  // banka havalesi

    }
}
