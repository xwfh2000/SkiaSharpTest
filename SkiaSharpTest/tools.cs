using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SkiaSharp;
namespace DrawTools
{
    class Tools
    {
        /// <summary>
        /// 将int数组转变为点阵，纯粹为了调试方便。
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        internal static List<Point> ints2Pts(int[] ints)
        {
            List<Point> ret = new List<Point>();
            for (int i = 0; i < ints.Length / 2; i++)
                ret.Add(new Point(ints[i * 2], ints[i * 2 + 1]));
            return ret;
        }


        

        

        /// <summary>
        /// 输入一个点，输出周围的四个点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static Point[] GetSurroundPts(Point p)
        {
            return new Point[] { new Point(p.X, p.Y), new Point(p.X + 1, p.Y), new Point(p.X, p.Y + 1), new Point(p.X + 1, p.Y + 1) };
        }
        /// <summary>
        /// 输入一个点的数组，输出包围输入所有点的点
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        internal static List<Point> GetSurroumdPTS(Point[] pts)
        {
            List<Point> ret = new List<Point>();
            foreach (Point p in pts)
                ret.AddRange(GetSurroundPts(p));
            return ret;
        }
        /// <summary>
        /// 将点进行去重，奇留偶去，得到拐角点
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        internal static List<Point> GetCornerPts(Point[] pts)
        {
            List<Point> ret = new List<Point>();
            foreach (Point p in pts)
            {
                if (ret.Contains(p))
                    ret.Remove(p);
                else
                    ret.Add(p);
            }
            return ret;
        }
        /// <summary>
        /// 寻找该方向上的点数及最近的点
        /// </summary>
        /// <param name="p">已经挑出的激活点</param>
        /// <param name="pts">剩余的点</param>
        /// <param name="Direction">0123分别代表上下左右</param>
        /// <param name="nearestPt"></param>
        /// <returns></returns>
        internal static int FindPts(Point ap, List<Point> pts, int Direction, ref Point nearestPt)
        {
            int ret = 0;
            int min = int.MaxValue;
            foreach (Point p in pts)
            {
                switch (Direction)
                {
                    case 0:
                        if (p.X == ap.X && p.Y < ap.Y)
                        {
                            ret++;
                            if (min > ap.Y - p.Y)
                            {
                                min = ap.Y - p.Y;
                                nearestPt = new Point(p.X, p.Y);
                            }
                        }
                        break;
                    case 1:
                        if (p.X == ap.X && p.Y > ap.Y)
                        {
                            ret++;
                            if (min > p.Y - ap.Y)
                            {
                                min = p.Y - ap.Y;
                                nearestPt = new Point(p.X, p.Y);
                            }
                        }
                        break;
                    case 2:
                        if (p.Y == ap.Y && p.X < ap.X)
                        {
                            ret++;
                            if (min > ap.X - p.X)
                            {
                                min = ap.X - p.X;
                                nearestPt = new Point(p.X, p.Y);
                            }
                        }
                        break;
                    case 3:
                        if (p.Y == ap.Y && p.X > ap.X)
                        {
                            ret++;
                            if (min > p.X - ap.X)
                            {
                                min = p.X - ap.X;
                                nearestPt = new Point(p.X, p.Y);
                            }
                        }
                        break;
                }
            }
            return ret;
        }
        /// <summary>
        /// 返回值代表是否移动了一个点
        /// </summary>
        /// <param name="subPath"></param>
        /// <param name="cornerPts"></param>
        /// <returns></returns>
        internal static bool MoveAPoint(ref List<Point> subPath, ref List<Point> cornerPts)
        {
            if (subPath.Count == 0 && cornerPts.Count > 0)
            {
                subPath.Add(new Point(cornerPts[0].X, cornerPts[0].Y));
                cornerPts.Remove(cornerPts[0]);
                return true;
            }
            Point nearestP = new Point(0, 0);
            if (FindPts(subPath[subPath.Count - 1], cornerPts, 0, ref nearestP) % 2 == 1 ||
                FindPts(subPath[subPath.Count - 1], cornerPts, 1, ref nearestP) % 2 == 1 ||
                FindPts(subPath[subPath.Count - 1], cornerPts, 2, ref nearestP) % 2 == 1 ||
                FindPts(subPath[subPath.Count - 1], cornerPts, 3, ref nearestP) % 2 == 1)
            {
                subPath.Add(new Point(nearestP.X, nearestP.Y));
                cornerPts.Remove(nearestP);
                return true;
            }
            else
                return false;
        }
        internal static void GetSubPath(ref List<Point> subPath, ref List<Point> cornerPts)
        {
            while (MoveAPoint(ref subPath, ref cornerPts)) ;
        }
        /// <summary>
        /// 最终应用，输入点，输出所有包络的子路径
        /// </summary>
        /// <param name="pts">输入一组点的列表</param>
        /// <returns>输出洞数+1组点的列表</returns>
        internal static List<List<Point>> GetPaths(ref List<Point> pts)
        {
            List<List<Point>> ret = new List<List<Point>>();
            pts = GetSurroumdPTS(pts.ToArray());
            pts = GetCornerPts(pts.ToArray());
            while (pts.Count > 0)
            {
                List<Point> subPath = new List<Point>();
                GetSubPath(ref subPath, ref pts);
                ret.Add(subPath);
            }
            return ret;
        }


        /// <summary>
        /// 判断是否正好有2个2，3个3，以此类推。groupnum为组数。
        /// </summary>
        /// <param name="group"></param>
        /// <param name="groupnum"></param>
        /// <returns></returns>
        internal static bool CanGrouped(List<int> group, ref int groupnum)
        {
            groupnum = 0;
            List<int> nums = new List<int>();//不同的数字的列表
            List<int> counts = new List<int>();//数字出现的次数
            for (int i = 0; i < group.Count; i++)
            {
                if (!nums.Contains(group[i]))
                {
                    nums.Add(group[i]);
                    counts.Add(1);
                }
                else
                {
                    int index = nums.IndexOf(group[i]);
                    counts[index]++;
                }
            }

            for (int i = 0; i < nums.Count; i++)
            {
                if (counts[i] % nums[i] != 0)
                    return false;
                else
                    groupnum += counts[i] / nums[i];
            }
            return true;

        }

        /// <summary>
        /// 对列表进行去重和排序
        /// </summary>
        /// <param name="toBeClean"></param>
        /// <returns></returns>
        internal static List<int> DisNSortList(List<int> toBeClean)
        {
            toBeClean = toBeClean.Distinct().ToList();
            toBeClean.Sort();
            return toBeClean;
        }
        /// <summary>
        /// 对列表中每一个列表进行去重和排序
        /// </summary>
        /// <param name="toBeClean"></param>
        /// <returns></returns>
        internal static List<List<int>> DisNSortListList(List<List<int>> lists)
        {
            List<List<int>> ret = new List<List<int>>();
            for (int i = 0; i < lists.Count; i++)
            {
                lists[i] = DisNSortList(lists[i]);
                ret.Add(lists[i]);
            }
            return ret;
        }
        /// <summary>
        /// 看看是否有足够的同组数字，否则就删掉。出现空列表就返回false;
        /// </summary>
        /// <param name="boxes"></param>
        /// <returns></returns>
        internal static bool DeleteExtra(ref List<List<int>> boxes)
        {
            List<int> nums = new List<int>();//不同的数字的列表
            List<int> counts = new List<int>();//数字出现的次数
            for (int i = 0; i < boxes.Count; i++)
            {
                for (int j = 0; j < boxes[i].Count; j++)
                {
                    if (!nums.Contains(boxes[i][j]))
                    {
                        nums.Add(boxes[i][j]);
                        counts.Add(1);
                    }
                    else
                    {
                        int index = nums.IndexOf(boxes[i][j]);
                        counts[index]++;
                    }
                }
            }
            for (int i = 0; i < nums.Count; i++)
            {
                if (counts[i] < nums[i])
                {
                    foreach (List<int> list in boxes)
                    {
                        list.Remove(nums[i]);
                        if (list.Count == 0)
                            return false;
                    }
                }

            }
            return true;
        }

        internal static bool IsGroupInRange(int group, int shortestLenth, int longestLength)
        {
            return group >= shortestLenth && group <= longestLength;
        }
        /// <summary>
        /// 判断人的组们是否有一个在范围内。
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="shortestLenth"></param>
        /// <param name="longestLength"></param>
        /// <returns></returns>
        internal static bool IsGroupsInRange(List<int> groups, int shortestLenth, int longestLength)
        {
            foreach (int group in groups)
            {
                if (IsGroupInRange(group, shortestLenth, longestLength))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 预判箱子受力串的长度范围
        /// </summary>
        /// <param name="incompleteBoxGroup"></param>
        /// <param name="boxGroupFullLength"></param>
        /// <param name="shortestLenth"></param>
        /// <param name="longestLength"></param>
        /// <returns></returns>
        internal static bool JudgeLengthRange(List<int> incompleteBoxGroup, int boxGroupFullLength, ref int shortestLenth, ref int longestLength)
        {
            shortestLenth = 0;
            longestLength = 0;
            List<int> nums = new List<int>();
            List<int> counts = new List<int>();
            GetCountsOfNums(incompleteBoxGroup, ref nums, ref counts);
            int needToAdd = 0;
            int mustExistGroupNum = 0;
            for (int i = 0; i < nums.Count; i++)
            {
                if (nums[i] > 1)
                {
                    mustExistGroupNum += counts[i] / nums[i];
                    if (counts[i] % nums[i] != 0)
                    {
                        needToAdd += nums[i] - counts[i] % nums[i];
                        mustExistGroupNum++;
                    }
                }
                else//数字为1的情况，自成N组。
                {
                    mustExistGroupNum +=counts[i];
                }
            }
            int extraSpace = boxGroupFullLength - (incompleteBoxGroup.Count + needToAdd);
            if (extraSpace < 0)
                return false;
            shortestLenth = mustExistGroupNum + (extraSpace == 0 ? 0 : 1);
            longestLength = mustExistGroupNum + extraSpace;
            return true;
        }

        /// <summary>
        /// 统计各个数字出现的频率
        /// </summary>
        /// <param name="group"></param>
        /// <param name="nums"></param>
        /// <param name="counts"></param>
        internal static void GetCountsOfNums(List<int> group, ref List<int> nums, ref List<int> counts)
        {
            nums = new List<int>();//不同的数字的列表
            counts = new List<int>();//数字出现的次数
            for (int i = 0; i < group.Count; i++)
            {
                if (!nums.Contains(group[i]))
                {
                    nums.Add(group[i]);
                    counts.Add(1);
                }
                else
                {
                    int index = nums.IndexOf(group[i]);
                    counts[index]++;
                }
            }
        }

        /// <summary>
        /// 用于判断受力串上的箱子的们的确定的组别和人的组们是否匹配。
        /// </summary>
        /// <param name="incompleteBoxGroup"></param>
        /// <param name="boxGroupFullLength"></param>
        /// <param name="manGroups"></param>
        /// <returns></returns>
        internal static bool IsBoxgroupMatchManGroups(List<int> incompleteBoxGroup, int boxGroupFullLength, List<int> manGroups)
        {
            int shortestLenth = 0;
            int longestLength = 0;
            bool ret = JudgeLengthRange(incompleteBoxGroup, boxGroupFullLength, ref shortestLenth, ref longestLength);
            if (!ret)
                return false;
            ret = IsGroupsInRange(manGroups, shortestLenth, longestLength);
            return ret;
        }

        /// <summary>
        /// 实现全排列的递归函数
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="BoxGroups"></param>
        /// <param name="templist"></param>
        /// <param name="Index"></param>
        internal static void MakeCombination(ref List<List<int>> ret, List<List<int>> BoxGroups, List<int> templist, int Index)
        {
            if (templist.Count >= BoxGroups.Count)
            {
                ret.Add(templist);
                return;
            }
            else
                for (int j = 0; j < BoxGroups[Index].Count; j++)
                {
                    List<int> tt = new List<int>();
                    foreach (int i in templist)
                        tt.Add(i);
                    tt.Add(BoxGroups[Index][j]);
                    MakeCombination(ref ret, BoxGroups, tt, Index + 1);
                }
        }

        /// <summary>
        /// sumoban的判断主函数。
        /// </summary>
        /// <param name="ret">返回正确的组合 </param>
        /// <param name="ManGroup">人的组们</param>
        /// <param name="BoxGroups">受力串上的箱子们的组们</param>
        /// <param name="templist">已经组合好的前半部分，用于参加递归</param>
        /// <param name="Index">递归的层级</param>
        /// <returns></returns>
        internal static bool CanManGroupMoveBoxGroups(ref List<List<int>> ret, List<int> ManGroup, List<List<int>> BoxGroups, List<int> templist, int Index)
        {
            if (templist.Count >= BoxGroups.Count)
            {
                ret.Add(templist);
                //*************如果返回false，别的递归还会继续。如果返回true，会一层层向上返回，最终返回true************
                return IsBoxgroupMatchManGroups(templist, BoxGroups.Count, ManGroup);
            }
            else
            {
                for (int j = 0; j < BoxGroups[Index].Count; j++)
                {
                    List<int> tt = new List<int>();
                    foreach (int i in templist)
                        tt.Add(i);
                    tt.Add(BoxGroups[Index][j]);
                    if (!IsBoxgroupMatchManGroups(tt, BoxGroups.Count, ManGroup))
                    {
                        //这个分支废了，跳过这个分支，继续平行的其他分支
                        continue;
                    }
                    else
                    {
                        if (CanManGroupMoveBoxGroups(ref ret, ManGroup, BoxGroups, tt, Index + 1))
                            return true;
                    }
                }
                return false;
            }
        }

        internal static SKPoint PointF2skPoint(PointF p,float gridsize)
        {
            return new SKPoint(p.X*gridsize, p.Y*gridsize);
        }

        internal static SKPath Points2Path(ref List<Point> pts,float gridsize)
        {
            SKPath ret = new SKPath();
            var LLPointpaths = GetPaths(ref pts);
            foreach (List<Point> path in LLPointpaths)
            {
                SKPath skPath = new SKPath();
                skPath.MoveTo(PointF2skPoint(path[0],gridsize));
                for (int i = 1; i < path.Count; i++)
                {
                    skPath.LineTo(PointF2skPoint(path[i],gridsize));
                }
                skPath.Close();
                ret.AddPath(skPath);
            }
            return ret;
        }


    }
}
