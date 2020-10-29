namespace BikeApp.data
{
    /*
     * Data class that stores a position and direction.
     */
    public class RouteData
    {
        public double[] dir;
        public int[] pos;

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