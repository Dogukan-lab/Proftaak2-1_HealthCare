namespace BikeApp.data
{
    /*
     * Data class that stores a position and direction.
     */
    internal class RouteData
    {
        public int[] pos;
        public double[] dir;

        /*
         * Initializes the data.
         */
        public RouteData(int[] position, double[] direction)
        {
            pos = position;
            dir = direction;
        }
    }
}
