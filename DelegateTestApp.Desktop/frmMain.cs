using DelegateTestApp.Desktop.RequestCreators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DelegateTestApp.Desktop
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            var req = new GetPostRequestCreator();

            //ne zaman req.OnRequestFinished Invoke edilirse
            //Req_OnRequestFinished metodunu çağırır : 
            req.OnRequestFinished += Req_OnRequestFinished; //req.OnRequestFinished += (s, e) => {} şeklinde inline olarak da yazılabilirdi.

            req.SetRequestStartedMethod(() =>
            {
                MessageBox.Show("Request started"); //inline method, EVENT gibi kullanılıyor.
            });

            var posts = req.GetPosts();

            MessageBox.Show(posts.FirstOrDefault().title);
        }

        private void Req_OnRequestFinished(object sender, int e)
        {
            MessageBox.Show($"request finished with size of {e}"); //dışarıdan gelen datanın karakter sayısını ekrana bastırıyoruz messagebox ile
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var req = new CreatePostRequestCreator();

            req.SetRequestStartedMethod(() =>
            {
                MessageBox.Show("Request started");
            });

            var createPost = req.CreatePost(new Models.PostModel()
            {
                title = "foo",
                body = "bar",
                userId = 1
            });

            MessageBox.Show($"result id: {createPost.id}");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
