using DelegateTestApp.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DelegateTestApp.Desktop.RequestCreators
{
    public abstract class BaseRequestCreator
    {

        public BaseRequestCreator()
        {
            makeRequestDelageMethod = MakeGETRequest;
        }

        //delegate nedir? : 
        /*
        
        içerisine fonksiyon veya metotların referansını verebileceğimiz değişken tipleridir.
        int sayi = 10; -> sayi değişkeni için memory'de int tipi kaç byte ise o kadar byte'lık yer ayırılır.
        10 ise int değişkenine uygun bir değer atamasıdır.
          
        delegateler ise bir değerin değil, bir metodun içerisine referans olarak verilebileceği tiptir.
        tanımlama metot tanımlaması gibidir, erişim belirleyici -> dönüş tipi -> metot adı -> varsa parametre

        Örneğin : 
        Metot tanımlama patterni : 
        [Access Modifier] + [Return Type] + [Method Name]([parameters]) -> private void TestMethod(int num); gibi.

        Örnek delegate kullanimi : 
            delegate void TestMetotDelegate(int parameter);   //delegate tanımlandı  
            TestMetotDelegate testMetotDelegate = new ( //delegate newlendi. aynı bir classtan instance oluşturur gibi.
                {
                    //buraya metot ataması yapılmalı
                    //ve atanacak olan metot delegate'e uygun patternde olmalıdır. 
                    //Örneğimizde int parametre alan ve dönüş tipi void olan bir metot ataması gereklidir.
                });
        
        delegate içerisine metot ataması yaptık fakat şimdi ne olacak? delegateler aldığı metot parametresini kullanabilir.
        delagetelerin invoke metodu vardır; TestMetotDelegate.Invoke(); şeklinde kullanılabilirler. 
        Bu Invoke metodu, delegate'in aldığı metodun dönüş tipinde değer döndürür. 


        Bu class içerisinde şu kod aratın : 
           var baseAddress = getBaseAddressDelegateMethod.Invoke();
         
         */

        protected delegate string GetBaseAddressDelegate(); //delegate tanımlandı
        GetBaseAddressDelegate getBaseAddressDelegateMethod; //BaseRequestCreator'dan instance alan herhangi bir metot, setbaseadressmethodu çalıştırarak bu delegate'e değer ataması yapar.
        private delegate string MakeRequestDelage(); 
        MakeRequestDelage makeRequestDelageMethod;

        public delegate void RequesStartedDelegate();
        RequesStartedDelegate requesStartedMethod;

        //Func'ta  bir delegate'tir.
        Func<int> requestCountFunc; //işlemin kaç kez yapılacağını belirlemek için kullanıyoruz, SetRequestCount metodunda değer ataması yapılır.

        private HttpMethod httpMethod; 


        protected void SetRequestCount(int count)  
        {
            requestCountFunc = () => count;
        }

        protected void SetBaseAddressMethod(GetBaseAddressDelegate paramDelegateMethod)
        {
            getBaseAddressDelegateMethod = paramDelegateMethod;
        }

        public void SetRequestStartedMethod(RequesStartedDelegate requesStartedMethod)
        {
            this.requesStartedMethod = requesStartedMethod;
        }

        private string MakeGETRequest()
        {
            HttpClient client = new HttpClient();

            var baseAddress = getBaseAddressDelegateMethod.Invoke();

            var msg = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(baseAddress + GetUrlPath())
            };

            var httpRes = client.Send(msg);

            httpRes.EnsureSuccessStatusCode();

            var resultContent = httpRes.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return resultContent;
        }
        
        private string MakePOSRequest()
        {
            HttpClient client = new HttpClient();

            var baseAddress = getBaseAddressDelegateMethod.Invoke();

            var msg = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(baseAddress + GetUrlPath())
            };

            var bodyContent = GetBodyObject();
            if (bodyContent != null)
                msg.Content = new StringContent(JsonSerializer.Serialize(bodyContent));

            var httpRes = client.Send(msg);

            httpRes.EnsureSuccessStatusCode();

            var resultContent = httpRes.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return resultContent;
        }


        //MakeRequest metodu protected,
        //bu sayede sadece BaseRequestCreator'dan inherit edilen classlar bu metodu kullanabilir.
        protected string MakeRequest()
        {
            var requestCount = Math.Max(requestCountFunc.Invoke(), 1); //en az 1 kez çalışacak.

            while ((requestCount--) > 0)
            {
                requesStartedMethod.Invoke();
            }

            return makeRequestDelageMethod.Invoke(); //makeRequestDelageMethod, string döndürecek -> post ya da get isteği
        }

        //virtual keyword, bu classtan inherit edilen classların,
        //bu metodu override edebilmesi için kullanılır.
        protected virtual string GetUrlPath()
        {
            return "";
        }

        //BaseRequestCreator'dan inherit edilen Get,Post vs. sınıflar kendi httpmetodlarına göre
        //bu metodu constructorlarında set ederler.
        protected HttpMethod SetHttpMethod(HttpMethod method)
        {
            if (method == HttpMethod.Post)
                makeRequestDelageMethod = MakePOSRequest; //constructor'da default değer GETRequest olduğundan dolayı burada sadece POST requesti set ediyorum.

            return httpMethod = method;
        }

        protected virtual object GetBodyObject()
        {
            return default;
        }

    }
}
