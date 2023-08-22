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
        private static List<int[]> OrderOnArea(List<int[]> rectangles, double[] columns, double[] rows)
        {
            double[] areas = new double[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    areas[j + i * 3] = columns[j] * rows[i];
                }
            }

            // a and b are each possible combination of rectangles
            // iterate over every single panel in a and b and find their areas
            // sum those and then find which is larger and place forward
            //rectangles.Sort((a, b) => b.Sum(x => areas[x-1]) - a.Sum(x => areas[x-1]));
            rectangles.Sort((a, b) => b.Sum(x => areas[x - 1]).CompareTo(a.Sum(x => areas[x - 1]))); //doubles comparison

            return rectangles;
        }

        //screen = screen frame; calculate from co-ordinates if needed. columnWidths, rowHeights = grid dimensions
        public static List<int[]> IntermediateRectangles(int screen, double[] columnWidths, double[] rowHeights, double minHeight, double minWidth)
        {
            List<int[]> intermediates = new List<int[]>();

            //panel size viability
            double[] widths = new double[9];
            double[] heights = new double[9];

            for (int i = 0; i < 3; i++)
            {
                //singles
                intermediates.Add(new int[] { 1 + i * 3 });
                intermediates.Add(new int[] { 2 + i * 3 });
                intermediates.Add(new int[] { 3 + i * 3 });

                //horizontals
                intermediates.Add(new int[] { 1 + i * 3, 2 + i * 3 });
                intermediates.Add(new int[] { 2 + i * 3, 3 + i * 3 });
                intermediates.Add(new int[] { 1 + i * 3, 2 + i * 3, 3 + i * 3 });

                //verticals
                intermediates.Add(new int[] { 1 + i, 4 + i });
                intermediates.Add(new int[] { 4 + i, 7 + i });
                intermediates.Add(new int[] { 1 + i, 4 + i, 7 + i });

                //calculate height, width pairs
                for (int j = 0; j < 3; j++)
                {
                    widths[j + i * 3] = columnWidths[j];
                    heights[j + i * 3] = rowHeights[i];
                }
            }

            //skip if screen central
            if (screen != 5)
            {
                //squares
                intermediates.Add(new int[] { 1, 2, 4, 5 });
                intermediates.Add(new int[] { 2, 3, 5, 6 });
                intermediates.Add(new int[] { 4, 5, 7, 8 });
                intermediates.Add(new int[] { 5, 6, 8, 9 });

                //2 x 3s
                intermediates.Add(new int[] { 1, 2, 3, 4, 5, 6 });
                intermediates.Add(new int[] { 4, 5, 6, 7, 8, 9 });
                intermediates.Add(new int[] { 1, 2, 4, 5, 7, 8 });
                intermediates.Add(new int[] { 2, 3, 5, 6, 8, 9 });
            }

            //remove screen overlapping and unviable dimensions rectangles
            List<int[]> filteredIntermediates = intermediates.Where(rectangle => !rectangle.Contains(screen) &&
                rectangle.Sum(x => heights[x-1]) >= minHeight && rectangle.Sum(x => widths[x-1]) >= minWidth).ToList();

            //calculate on configuration, save as global
            return OrderOnArea(filteredIntermediates, columnWidths, rowHeights);
        }

        //orders components on size
        // frames is array of panels
        public static List<Uri> UriPriority(Uri deltaUri, List<int[]> intermediates, Panel[] frames, bool isAdd)
        {
            List<int[]> rectangles = new List<int[]>();
            List<Uri> uris = new List<Uri>();

            foreach (Panel f in frames)
            {
                if (f == null) continue; //more robust type check
                
                int c = f.ColumnSpan;
                int r = f.RowSpan;
                int[] rect = new int[r * c];

                int a = 0;
                // loop through columns
                for (int i = 0; i < c; i++)
                {
                    // loop through rows
                    for (int j = 0; j < r; j++)
                    {
                        rect[a] = f.PanelNum + i + j * 3;
                        a++;
                    }
                }

                rectangles.Add(rect);
                uris.Add(new Uri(f.Uri));
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
        public static dynamic PackedFrames(List<Uri> uris, List<List<int[]>> optimalFrames)
        {
            //configuration for number of panels
            List<int[]> instanceOptimalFrames = optimalFrames[uris.Count - 1];

            //dynamic packed = new System.Dynamic.ExpandoObject();
            var packed = new Dictionary<string, Dictionary<string, dynamic>>();
            List<(int[], Uri)> combinedList = instanceOptimalFrames.Zip(uris, (rect, uri) => (rect, uri)).ToList();

            foreach ((int[], Uri) a in combinedList)
            {
                var attributes = new Dictionary<string, dynamic>();
                attributes["uri"] = a.Item2;

                if (a.Item1.Length == 1) //single
                {
                    attributes["ColumnSpan"] = 1;
                    attributes["RowSpan"] = 1;

                }
                else if (a.Item1.Length == 4) //square
                {
                    attributes["ColumnSpan"] = 2;
                    attributes["RowSpan"] = 2;

                }
                else if (a.Item1.Length == 6) //2 x 3
                {
                    attributes["ColumnSpan"] = 2 + (a.Item1[2] - a.Item1[1] == 1 ? 1 : 0);
                    attributes["RowSpan"] = 3 - (a.Item1[2] - a.Item1[1] == 1 ? 1 : 0);

                }
                else if (a.Item1[1] - a.Item1[0] == 1) //horizontal
                {
                    attributes["ColumnSpan"] = a.Item1.Length;
                    attributes["RowSpan"] = 1;

                }
                else
                {
                    attributes["ColumnSpan"] = 1;
                    attributes["RowSpan"] = a.Item1.Length;
                }

                packed[$"Panel{a.Item1[0]}"] = attributes;
            }

            return packed;
        }

        private static void GenerateCombinations(List<int[]> remainingRectangles, List<int[]> currentCombination, int num, List<List<int[]>> store)
        {
            if (num == 0)
            {
                store.Add(currentCombination);
                return;
            }

            foreach (int[] array in remainingRectangles)
            {

                //optimisation - past rectangles checked, no need to repeat; just check array
                List<int[]> newRemainingRectangles = remainingRectangles.Where(rect => !rect.Intersect(array).Any() && rect != array).ToList();

                if (newRemainingRectangles.Count == 0 && num != 1) continue; //skip if no rectangles left and not last step

                //add array to combination
                List<int[]> newCurrentCombination = new List<int[]>(currentCombination) { array };

                GenerateCombinations(newRemainingRectangles, newCurrentCombination, num - 1, store);
            }
        }


        public static List<List<int[]>> OptimalFrames(List<int[]> intermediates)
        {
            //be sure to make it so only x number of panels can be added - where x is the total number of optimal combinations this returns

            List<List<int[]>> optimalFrames = new List<List<int[]>>();

            int i = 0;
            while (true)
            {
                i++;
                //if 1 frame, select largest
                if (i == 1)
                {
                    optimalFrames.Add(new List<int[]>() { intermediates[0] });
                    continue;
                }
                //if 8 frames, select singles
                if (i == 8)
                {
                    List<int[]> sizeCheckStore = new List<int[]>(intermediates.Where(array => array.Length == 1).ToList());
                    if (sizeCheckStore.Count == 8) optimalFrames.Add(sizeCheckStore); //skip if not 8 panels
                    break;
                }

                List<List<int[]>> candidates = new List<List<int[]>>();
                GenerateCombinations(intermediates, new List<int[]>(), i, candidates); //check for frame quantity in recursion format
                //if n panels possible, then n-1 always possible, so no risk of discontinuity

                //sort candidates by average index of their intermediates
                List<List<int[]>> sortedCandidates = candidates.OrderBy(candidateList =>
                {
                    double averageIndex = candidateList.Average(candidate => intermediates.IndexOf(candidate));
                    return averageIndex;
                }).ToList();
                optimalFrames.Add(sortedCandidates[0]); //take highest ranked
            }

            return optimalFrames;
        }
    }
}