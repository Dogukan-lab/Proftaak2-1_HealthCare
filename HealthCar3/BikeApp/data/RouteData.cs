namespace BikeApp.data
{
    /*
     * Data class that stores a position and direction.
     */
    internal class RouteData
    {
        public int[] pos;
        public int[] dir;

        /*
         * Initializes the data.
         */
        public RouteData(int[] position, int[] direction)
        {
            pos = position;
            dir = direction;
        }
    }
}
