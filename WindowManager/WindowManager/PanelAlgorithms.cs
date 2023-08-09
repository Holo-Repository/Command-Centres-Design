using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowManager
{
    class PanelAlgorithms
    {
        private static List<int[]> OrderOnArea(List<int[]> rectangles, int[] columns, int[] rows)
        {
            dynamic areas = new System.Dynamic.ExpandoObject();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    areas[j + i * 3] = columns[j] * rows[i];
                }
            }

            rectangles.Sort((a, b) => b.Sum(x => areas[x]) - a.Sum(x => areas[x]));

            return rectangles;
        }

        //screen = screen frame; calculate from co-ordinates if needed. columnWidths, rowHeights = grid dimensions
        public static List<int[]> IntermediateRectangles(int screen, int[] columnWidths, int[] rowHeights)
        {
            List<int[]> intermediates = new List<int[]>();

            for (int i = 0; i < 3; i++)
            {
                //singles
                intermediates.Add(new int[] { i * 3 });
                intermediates.Add(new int[] { 1 + i * 3 });
                intermediates.Add(new int[] { 2 + i * 3 });

                //horizontals
                intermediates.Add(new int[] { i * 3, 1 + i * 3 });
                intermediates.Add(new int[] { 1 + i * 3, 2 + i * 3 });
                intermediates.Add(new int[] { i * 3, 1 + i * 3, 2 + i * 3 });

                //verticals
                intermediates.Add(new int[] { i, i + 3 });
                intermediates.Add(new int[] { i + 3, i + 6 });
                intermediates.Add(new int[] { i, i + 3, i + 6 });

            }

            //skip if screen central
            if (screen != 4)
            {
                //squares
                intermediates.Add(new int[] { 0, 1, 3, 4 });
                intermediates.Add(new int[] { 1, 2, 4, 5 });
                intermediates.Add(new int[] { 3, 4, 6, 7 });
                intermediates.Add(new int[] { 4, 5, 7, 8 });

                //2 x 3s
                intermediates.Add(new int[] { 0, 1, 2, 3, 4, 5 });
                intermediates.Add(new int[] { 3, 4, 5, 6, 7, 8 });
                intermediates.Add(new int[] { 0, 1, 3, 4, 6, 7 });
                intermediates.Add(new int[] { 1, 2, 4, 5, 7, 8 });
            }

            foreach (var rectangle in intermediates)
            {
                if (rectangle.Contains(screen)) intermediates.Remove(rectangle);
            }

            //calculate on configuration, save as global
            return OrderOnArea(intermediates, columnWidths, rowHeights);
        }

        //orders components on size
        public static List<Uri> UriPriority(Uri deltaUri, List<int[]> intermediates, dynamic frames, bool isAdd)
        {
            List<int[]> rectangles = new List<int[]>();
            List<Uri> uris = new List<Uri>();

            foreach (KeyValuePair<int, dynamic> f in frames)
            {
                int c = f.Value.columnSpan;
                int r = f.Value.rowSpan;
                int[] rect = new int[r * c];

                int a = 0;
                for (int i = 0; i < c; i++)
                {
                    for (int j = 0; j < r; j++)
                    {
                        rect[a] = f.Key + i + j * 3;
                        a++;
                    }
                }

                rectangles.Add(rect);
                uris.Add(new Uri(f.Value.uri));
            }

            //sort uris by priority (frame area); append new
            List<(int[], Uri)> combinedList = rectangles.Zip(uris, (rect, uri) => (rect, uri)).ToList();
            combinedList.Sort((a, b) => intermediates.IndexOf(a.Item1).CompareTo(intermediates.IndexOf(b.Item1)));
            uris = combinedList.Select(tuple => tuple.Item2).ToList();

            if (isAdd) uris.Add(deltaUri);
            else uris.Remove(deltaUri);

            return uris;
        }

        //returns object for json
        public static dynamic PackedFrames(List<Uri> uris, List<int[]> optimalFrames)
        {
            dynamic packed = new System.Dynamic.ExpandoObject();
            List<(int[], Uri)> combinedList = optimalFrames.Zip(uris, (rect, uri) => (rect, uri)).ToList();

            foreach ((int[], Uri) a in combinedList)
            {
                //single frame
                if (a.Item1.Length == 1)
                {
                    dynamic singleFrame = new System.Dynamic.ExpandoObject();
                    singleFrame.uri = a.Item2;
                    singleFrame.columnSpan = 1;
                    singleFrame.rowSpan = 1;
                    packed[a.Item1[0]] = singleFrame;
                    continue;
                }
                //square
                if (a.Item1.Length == 4)
                {
                    dynamic squareFrame = new System.Dynamic.ExpandoObject();
                    squareFrame.uri = a.Item2;
                    squareFrame.columnSpan = 2;
                    squareFrame.rowSpan = 2;
                    packed[a.Item1[0]] = squareFrame;
                    continue;
                }
                //2 X 3
                if (a.Item1.Length == 6)
                {
                    dynamic twobythree = new System.Dynamic.ExpandoObject();
                    twobythree.uri = a.Item2;
                    twobythree.columnSpan = 2 + (a.Item1[2] - a.Item1[1] == 1 ? 1 : 0);
                    twobythree.rowSpan = 3 - (a.Item1[2] - a.Item1[1] == 1 ? 1 : 0);
                    packed[a.Item1[0]] = twobythree;
                    continue;
                }
                //horizontal
                if (a.Item1[1] - a.Item1[0] == 1)
                {
                    dynamic horizontalFrame = new System.Dynamic.ExpandoObject();
                    horizontalFrame.uri = a.Item2;
                    horizontalFrame.columnSpan = a.Item1.Length;
                    horizontalFrame.rowSpan = 1;
                    packed[a.Item1[0]] = horizontalFrame;
                    continue;
                }
                //vertical
                dynamic verticalFrame = new System.Dynamic.ExpandoObject();
                verticalFrame.uri = a.Item2;
                verticalFrame.columnSpan = 1;
                verticalFrame.rowSpan = a.Item1.Length;
                packed[a.Item1[0]] = verticalFrame;
            }

            return packed;
        }

        public static dynamic OptimalFrames(List<int[]> intermediates, int[] columnWidths, int[] rowHeights)
        {
            dynamic areas = new System.Dynamic.ExpandoObject();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    areas[j + i * 3] = columnWidths[j] * rowHeights[i];
                }
            }

            //find areas of frames, find combinations of panels (x many), find optimal

            return areas; //remove - just to get rid of error

        }
    }
}
