using F=System.Windows.Forms;
using Host = System.Windows.Forms.Integration.ElementHost;


namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Control
{
    public partial class PaneDecorator : F.UserControl
    {
        private CustomPane _designPane;
        public CustomPane DesignPane
        {
            get { return _designPane; }
        }

        public PaneDecorator(CustomPane pane)
        {
            InitializeComponent();
            _designPane = pane;

            var host = new Host();
            host.Dock = F.DockStyle.Fill;
            host.Child = pane;
            this.Controls.Add(host);
        }
    }
}
