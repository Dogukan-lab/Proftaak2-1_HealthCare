using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.data
{

    /** 
     * This data structure holds the data of a road object command. 
     */

    class RoadData : VpnData
    {
        private new string id;
        private string route;
        private string diffuse;
        private string normal;
        private string specular;
        private double heightoffset;

        public string GetID()
        {
            return id;
        }

        public string GetRoute()
        {
            return route;
        }

        public string GetDiffuse()
        {
            return diffuse;
        }

        public string GetNormal()
        {
            return normal;
        }

        public string GetSpecular()
        {
            return specular;
        }

        private double GetHeighoffset()
        {
            return heightoffset;
        }

        public void SetID(string id)
        {
            this.id = id;
        }
        public void SetRoute(string route)
        {
            this.route = route;
        }

        public void SetDiffuse(string diffuse)
        {
            this.diffuse = diffuse;
        }

        public void SetNormal(string normal)
        {
            this.normal = normal;
        }

        public void SetSpecular(string specular)
        {
            this.specular = specular;
        }

        public void SetHeightoffset(double heightoffset)
        {
            this.heightoffset = heightoffset;
        }

    }
}
//respose to add and update
/*{
    "id" : "route/road/add",
    "data" :
    {
    "uuid" : nodeid
    }
}*/

