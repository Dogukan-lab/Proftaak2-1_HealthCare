using System.Windows.Forms;

namespace SimulatorGui
{
    public partial class SimForm : Form
    {
        public SimForm()
        {
            InitializeComponent();
            // Set some base values
            speedBox.Text = "7,5";
            speedSwayCheck.Checked = true;
            speedSwayBox.Text = "0,5";
            heartRateBox.Text = "85";
            heartRateSwayCheck.Checked = true;
            heartRateSway.Text = "2";
            resistanceBox.Text = "0";
        }

        // Speed data
        public float GetSpeed()
        {
            return speedBox.Text.Length > 0? float.Parse(speedBox.Text) : 0;
        }
        public bool SpeedSwayEnabled()
        {
            return speedSwayCheck.Checked;
        }
        public float GetSpeedSway()
        {
            return speedSwayBox.Text.Length > 0? float.Parse(speedSwayBox.Text) : 0;
        }

        // Heart rate data
        public int GetHeartRate()
        {
            return heartRateBox.Text.Length > 0? int.Parse(heartRateBox.Text) : 0;
        }
        public bool HeartRateSwayEnabled()
        {
            return heartRateSwayCheck.Checked;
        }
        public int GetHeartRateSway()
        {
            return heartRateSway.Text.Length > 0? int.Parse(heartRateSway.Text) : 0;
        }

        // Setting resistance thread safe
        private delegate void SetResistanceCallBack(float resistance);
        
        private void SetResistanceCB(float resistance)
        {
            resistanceBox.Text = resistance.ToString();
        }

        public void SetResistance(float resistance)
        {
            // if accessed from a different thread, invoke
            if (resistanceBox.InvokeRequired)
            {
                SetResistanceCallBack res = new SetResistanceCallBack(SetResistanceCB);
                this.Invoke(res, new object[] { resistance });
            }
            // if it is one the same thread no need to invoke
            else
            {
                resistanceBox.Text = resistance.ToString();
            }
        }
    }
}
