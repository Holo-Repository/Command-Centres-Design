using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowManager
{
    internal class SettingsData
    {
        public Tv Tv { get; set; }
        public Grid Grid { get; set; }
    }
    public class Tv
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public double X_Position { get; set; }
        public double Y_Position { get; set; }
    }

    public class Grid
    {
        public double[] RowHeights { get; set; }
        public double[] ColumnWidths { get; set;}
    }

}
