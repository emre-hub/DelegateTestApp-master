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
    public class GetPostRequestCreator : BaseRequestCreator
    {

        //Event'ler sadece tanımlandıkları class içeriside kullanılabilir,
        //Oluşturduğum eventi buton click eventinde kullanacağımız için
        //Bu nedenle BaseRequestCreator class içerisinde tanımlamadım.
        //Button Click'te GetPostRequestCreator kullanıldığında Base classtan event kullanamam. 
        public event EventHandler<int> OnRequestFinished;

        public GetPostRequestCreator()
        {
            base.SetBaseAddressMethod(() =>
            {
                return "https://jsonplaceholder.typicode.com/";
            });

            SetHttpMethod(HttpMethod.Get);

            SetRequestCount(2); //request sayısı 2
        }

        public List<PostModel> GetPosts()
        {
            //Bir http isteği oluştururken ihtiyacım olan şeyler : 
            //Base Url
            //Path
            //HttpMethod -> Get, Post etc.
            //Body Object

            var responseContent = MakeRequest();

            OnRequestFinished?.Invoke(this, responseContent.Length); //butona tıklandığında çalışan bir event

            return JsonSerializer.Deserialize<List<PostModel>>(responseContent);
        }



        protected override string GetUrlPath()
        {
            return "posts";
        }
    }
}
