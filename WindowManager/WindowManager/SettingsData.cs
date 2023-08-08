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
        public Panels Panels { get; set; }
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
        public double[] ColumnWidths { get; set; }
    }

    public class Panels
    {
        public Panel Panel1 { get; set; }
        public Panel Panel2 { get; set; }
        public Panel Panel3 { get; set; }
        public Panel Panel4 { get; set; }
        public Panel Panel5 { get; set; }
        public Panel Panel6 { get; set; }
        public Panel Panel7 { get; set; }
        public Panel Panel8 { get; set; }
        public Panel Panel9 { get; set; }

        public Panel[] GetPanelsArray()
        {
            return new[] { Panel1, Panel2, Panel3, Panel4, Panel5, Panel6, Panel7, Panel8, Panel9 };

        }

    }
    public class Panel
    {
        public int PanelNum { get; set; }
        public string Uri { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }
    }

}
