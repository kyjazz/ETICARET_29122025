using System.Net;
using System.Net.Mail;

namespace ETICARET.WebUI.EmailService
{
    public class MailHelper //eposta gönderme işlemlerini yapacak yardımcı sınıf
    {
        //Tek bir alıcıya eposta gönderen method
        //body: eposta içeriği
        //to: eposta gönderilecek adres
        //subject: eposta konusu
        //isHtml: eposta içeriğinin html formatında olup olmadığı
        public static bool SendEmail(string body, string to,string subject, bool isHtml = true)
        {
            return SendEmail(body, new List<string> { to }, subject, isHtml);
        }

        //birden fazla alıcıya eposta gönderen method
        private static bool SendEmail(string body, List<string> to, string subject, bool isHtml)
        {
            bool result = false; //eposta gönderme işleminin başarılı olup olmadığını tutacak değişken
            try
            {
                var message = new MailMessage(); //eposta mesajını oluşturmak için MailMessage sınıfından bir nesne oluşturulur
                message.From = new MailAddress("furkanucanuby@gmail.com");

                //alıcı listesindeki her e-posta adresini mesaja ekleyelim
                to.ForEach(x =>
                {
                    message.To.Add(new MailAddress(x));
                });
                message.Subject = subject; //eposta konusu
                message.Body = body; //eposta içeriği
                message.IsBodyHtml = isHtml; //eposta içeriğinin html formatında olup olmadığı

                //SmtpClient sınıfı, e-posta gönderme işlemini gerçekleştirmek için kullanılır(gmail SMTP sunucusu kullanıyor)
                using (var smtp = new SmtpClient("smtp.gmail.com",587))
                {
                    smtp.EnableSsl = true;//SSL kullanarak güvenli bağlantı sağlanır
                    smtp.Credentials = new NetworkCredential(
                        "furkanucanuby@gmail.com",
                        "jemk bvqj erpv hymh" //gmail hesabının şifresi veya uygulama şifresi
                    );

                    smtp.Send(message); //eposta gönderme işlemi gerçekleştirilir
                    result = true; //eposta gönderme işlemi başarılı olduysa result değişkeni true olarak güncellenir
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); //hata durumunda hata mesajı konsola yazdırılır
                result = false; //eposta gönderme işlemi başarısız olduysa result değişkeni false olarak güncellenir
            }
            return result; //eposta gönderme işleminin sonucunu döndürür
        }
    }
}
