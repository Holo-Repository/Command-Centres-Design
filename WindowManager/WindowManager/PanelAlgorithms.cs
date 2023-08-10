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

            List<int[]> filteredIntermediates = new List<int[]>(intermediates);

            foreach (var rectangle in intermediates)
            {
                if (rectangle.Contains(screen)) filteredIntermediates.Remove(rectangle);
            }

            //calculate on configuration, save as global
            return OrderOnArea(filteredIntermediates, columnWidths, rowHeights);
        }

        //orders components on size
        public static List<Uri> UriPriority(Uri deltaUri, List<int[]> intermediates, dynamic frames, bool isAdd)
        {
            List<int[]> rectangles = new List<int[]>();
            List<Uri> uris = new List<Uri>();

            foreach (KeyValuePair<int, dynamic> f in frames)
            {
                int c = f.Value.ColumnSpan;
                int r = f.Value.RowSpan;
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
        public static dynamic PackedFrames(List<Uri> uris, List<List<int[]>> optimalFrames)
        {
            //configuration for number of panels
            List<int[]> instanceOptimalFrames = optimalFrames[uris.Count - 1];

            dynamic packed = new System.Dynamic.ExpandoObject();
            List<(int[], Uri)> combinedList = instanceOptimalFrames.Zip(uris, (rect, uri) => (rect, uri)).ToList();

            foreach ((int[], Uri) a in combinedList)
            {
                //single frame
                if (a.Item1.Length == 1)
                {
                    dynamic singleFrame = new System.Dynamic.ExpandoObject();
                    singleFrame.uri = a.Item2;
                    singleFrame.ColumnSpan = 1;
                    singleFrame.RowSpan = 1;
                    packed[$"Panel{a.Item1[0]}"] = singleFrame;
                    continue;
                }
                //square
                if (a.Item1.Length == 4)
                {
                    dynamic squareFrame = new System.Dynamic.ExpandoObject();
                    squareFrame.uri = a.Item2;
                    squareFrame.ColumnSpan = 2;
                    squareFrame.RowSpan = 2;
                    packed[$"Panel{a.Item1[0]}"] = squareFrame;
                    continue;
                }
                //2 X 3
                if (a.Item1.Length == 6)
                {
                    dynamic twobythree = new System.Dynamic.ExpandoObject();
                    twobythree.uri = a.Item2;
                    twobythree.ColumnSpan = 2 + (a.Item1[2] - a.Item1[1] == 1 ? 1 : 0);
                    twobythree.RowSpan = 3 - (a.Item1[2] - a.Item1[1] == 1 ? 1 : 0);
                    packed[$"Panel{a.Item1[0]}"] = twobythree;
                    continue;
                }
                //horizontal
                if (a.Item1[1] - a.Item1[0] == 1)
                {
                    dynamic horizontalFrame = new System.Dynamic.ExpandoObject();
                    horizontalFrame.uri = a.Item2;
                    horizontalFrame.ColumnSpan = a.Item1.Length;
                    horizontalFrame.RowSpan = 1;
                    packed[$"Panel{a.Item1[0]}"] = horizontalFrame;
                    continue;
                }
                //vertical
                dynamic verticalFrame = new System.Dynamic.ExpandoObject();
                verticalFrame.uri = a.Item2;
                verticalFrame.ColumnSpan = 1;
                verticalFrame.RowSpan = a.Item1.Length;
                packed[a.Item1[0]] = verticalFrame;
            }

            return packed;
        }

        private static bool NoOverlap(int[] array, List<int[]> arrayList)
        {
            return !arrayList.Any(a => array.Intersect(a).Any());
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
                //add array to combination, remove overlaps from remaining
                List<int[]> newCurrentCombination = new List<int[]>(currentCombination) { array };
                List<int[]> newRemainingRectangles = remainingRectangles.Where(rect => NoOverlap(rect, newCurrentCombination)).ToList();

                if (newRemainingRectangles.Count == 0 && num != 1) continue; //skip if no rectangles left and not last step

                GenerateCombinations(newRemainingRectangles, newCurrentCombination, num - 1, store);
            }
        }


        public static List<List<int[]>> OptimalFrames(List<int[]> intermediates)
        {

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
                    optimalFrames.Add(new List<int[]>(intermediates.Where(array => array.Length == 1).ToList()));
                    break;
                }

                List<List<int[]>> candidates = new List<List<int[]>>();
                GenerateCombinations(intermediates, new List<int[]>(), i, candidates);

                //sort candidates by average index of their intermediates
                List<List<int[]>> sortedCandidates = candidates.OrderBy(candidateList =>
                {
                    double averageIndex = candidateList.Average(candidate => intermediates.IndexOf(candidate));
                    return averageIndex;
                }).ToList();
                optimalFrames.Add(sortedCandidates[0]);
            }

            return optimalFrames;
        }
    }
}