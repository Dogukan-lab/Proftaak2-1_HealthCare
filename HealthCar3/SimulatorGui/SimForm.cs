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

        // Resistance data
        public float GetResistance()
        {
            return resistanceBox.Text.Length > 0? float.Parse(resistanceBox.Text) : 0;
        }
    }
}
