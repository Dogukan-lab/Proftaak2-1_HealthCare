using System;
using System.Collections.Generic;
using System.Text;

namespace BikeApp.data
{

    /** 
     * This data structure holds the data of a skybox object command. 
     */

    class SkyBoxData : VpnData
    {
        private double time;
        private string type;
        private SkyBoxDataFiles files;

        public double GetTime()
        {
            return time;
        }

        public string GetSkyBoxType()
        {
            return type;
        }

        public void SetTime(double time)
        {
            this.time = time;
        }

        public void SetSkyBoxType(string type)
        {
            this.type = type;
        }

    }

    class SkyBoxDataFiles : SkyBoxData
    {
        private string xpos;
        private string xneg;
        private string ypos;
        private string yneg;
        private string zpos;
        private string zneg;

        public string GetXPos()
        {
            return xpos;
        }

        public string GetXNeg()
        {
            return xpos;
        }

        public string GetYPos()
        {
            return xpos;
        }

        public string GetYNeg()
        {
            return xpos;
        }

        public string GetZPos()
        {
            return xpos;
        }

        public string GetZNeg()
        {
            return xpos;
        }

        public void SetXPos(string xpos)
        {
            this.xpos = xpos;
        }

        public void SetXNeg(string xneg)
        {
            this.xneg = xneg;
        }

        public void SetYPos(string ypos)
        {
            this.ypos = ypos;
        }

        public void SetYNeg(string yneg)
        {
            this.yneg = yneg;
        }

        public void SetZPos(string zpos)
        {
            this.zpos = zpos;
        }

        public void SetZNeg(string zneg)
        {
            this.zneg = zneg;
        }
    }
}
