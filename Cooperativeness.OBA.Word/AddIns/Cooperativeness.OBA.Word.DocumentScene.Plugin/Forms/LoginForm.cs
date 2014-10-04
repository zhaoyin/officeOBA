using F=System.Windows.Forms;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Forms
{
    public partial class LoginForm : F.Form
    {
        #region 属性
        private bool _isLogin = false;
        public bool IsLogin
        {
            get { return _isLogin; }
        }
        #endregion

        public LoginForm()
        {
            InitializeComponent();
            lblUserName.Text = SceneContext.Instance.GetString("SceneDesigner.LoginForm.lblUserName");
            lblPassword.Text = SceneContext.Instance.GetString("SceneDesigner.LoginForm.lblPassword");
            lblVerify.Text = SceneContext.Instance.GetString("SceneDesigner.LoginForm.lblVerify");
            btnLogin.Text = SceneContext.Instance.GetString("SceneDesigner.LoginForm.btnLogin");
            btnCancel.Text = SceneContext.Instance.GetString("SceneDesigner.LoginForm.btnCancel");
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            if (txtUserName.Text.Trim().Equals("admin") && txtPassword.Text.Trim().Equals("admin"))
            {
                _isLogin = true;
                this.Close();
                return;
            }
            F.MessageBox.Show("输入不正确，请重试!");
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
