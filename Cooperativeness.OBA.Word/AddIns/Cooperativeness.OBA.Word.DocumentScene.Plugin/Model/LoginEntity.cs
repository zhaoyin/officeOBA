
namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Model
{
    public class LoginEntity
    {
        /// <summary>
        /// 通过Xml构造Login对象
        /// </summary>
        /// <param name="loginXml"></param>
        public LoginEntity(string loginXml)
        {
            
        }

        private IDbServer _dbServer = null;

        public IDbServer DbServer
        {
            get { return _dbServer; }
        }

        private string _auth;
        public string AuthInfo
        {
            get { return _auth; }
        }

        private bool _isAdmin;

        public bool IsAdmin
        {
            get { return _isAdmin; }
        }

        private string _languageId;
        public string LanguageId
        {
            get { return _languageId; }
        }

        private IUser _user;
        public IUser LoginObject { 
            get
            {
                return _user;
            }
        }

        private string _userToken;
        public string UserToken
        {
            get { return _userToken; }
        }

        private IFileServer _fileServer;
        public IFileServer FileServer
        {
            get { return _fileServer; }
        }
    }
}
