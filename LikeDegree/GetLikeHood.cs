using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Data.OleDb;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV.ML;
using Emgu.CV.ML.Structure;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace LikeDegree
{
    public class GetLikeHood
    {
        public List<int> XList = new List<int>();//用于分割字符的泛型,在CutImg方法中使用
        public List<int> YList = new List<int>();

        public static double LikeHood(string[] a, string[] b)     //a为索引文档，b为查询项                     string[] onlykeywords = keywords.Split(ca);
        {                                                                                               //字符串转化为字符数组
            int[] inta = Array.ConvertAll<string, int>(a, delegate(string s) { return int.Parse(s); });
            int[] intb = Array.ConvertAll<string, int>(b, delegate(string s) { return int.Parse(s); });
            double ab = 0; double aa = 0; double bb = 0;
            for (int i = 0; i < inta.Length; i++)
            {
                ab += inta[i] * intb[i];
                aa += Math.Pow(inta[i], 2);
                bb += Math.Pow(intb[i], 2);
            }
            double sumab = (double)aa * bb;
            double squre = Math.Sqrt(sumab);
            double result = ab / squre;
            return result;

        }
        public static double Jaccard(string[] a, string[] b)     //a为索引文档，b为查询项                     string[] onlykeywords = keywords.Split(ca);
        {                                                                                               //字符串转化为字符数组
            int[] inta = Array.ConvertAll<string, int>(a, delegate(string s) { return int.Parse(s); });
            int[] intb = Array.ConvertAll<string, int>(b, delegate(string s) { return int.Parse(s); });
            double ab = 0; double aa = 0; double bb = 0;
            for (int i = 0; i < inta.Length; i++)
            {
                ab += inta[i] * intb[i];
                aa += Math.Pow(inta[i], 2);
                bb += Math.Pow(intb[i], 2);
            }
            double sumab = (double)aa + bb;
            //double squre = Math.Sqrt(sumab);
            double result = ab / (sumab - ab);
            return result;

        }
        public static double Dice(string[] a, string[] b)     //a为索引文档，b为查询项                     string[] onlykeywords = keywords.Split(ca);
        {                                                                                               //字符串转化为字符数组
            int[] inta = Array.ConvertAll<string, int>(a, delegate(string s) { return int.Parse(s); });
            int[] intb = Array.ConvertAll<string, int>(b, delegate(string s) { return int.Parse(s); });
            double ab = 0; double aa = 0; double bb = 0;
            for (int i = 0; i < inta.Length; i++)
            {
                ab += inta[i] * intb[i];
                aa += Math.Pow(inta[i], 2);
                bb += Math.Pow(intb[i], 2);
            }
            double sumab = (double)aa + bb;
            double result = (2 * ab) / sumab;
            return result;

        }
        public static double EuclideanDistance(string[] a, string[] b)
        {
            int[] inta = Array.ConvertAll<string, int>(a, delegate(string s) { return int.Parse(s); });
            int[] intb = Array.ConvertAll<string, int>(b, delegate(string s) { return int.Parse(s); });
            double ab = 0;
            for (int i = 0; i < inta.Length; i++)
            {
                ab += Math.Pow(inta[i] - intb[i], 2);
            }
            double sumab = Math.Sqrt(ab);
            return sumab;
        }
        public static double EuclideanDistanceDouble(string[] a, string[] b)
        {
            double []doublea=new double[a.Length];
            double[] doubleb = new double[b.Length];
            for (int i = 0; i < a.Length;i++ ) 
            {
                doublea[i] = Convert.ToDouble(a[i]);
                doubleb[i] = Convert.ToDouble(b[i]);
            }            
            double ab = 0;
            for (int i = 0; i < doublea.Length; i++)
            {
                ab += Math.Pow(doublea[i] - doubleb[i], 2);
            }
            double sumab = Math.Sqrt(ab);
            return sumab;
        }

        /// <summary>
        /// 传进来待旋转的图片,需要解决每旋转一次，图片尺寸变大的问题
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Bitmap Rotate(Bitmap bmp, double degree)
        {
            
            
            //1.将bitmap转为MIplImage
            MIplImage img_srcmi = BitmapToMIplImage(bmp);//相当于IplImage的托管结构
            
            
            int size = Marshal.SizeOf(img_srcmi);
            IntPtr intptr_src = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(img_srcmi, intptr_src, true);
            
            //******************************************************
            
            //2.由角度制degree转为弧度制angle；并算出sine，cosine值，用于计算存放旋转后图片需要的大小
            double angle = degree * Math.PI/ 180;
            double a=Math.Sin(angle),b=Math.Cos(angle);
            
            //3.计算存放旋转后图片的载体的尺寸
            int width_src = img_srcmi.width;
            int height_src = img_srcmi.height;

            int width_dst=(int)(height_src*Math.Abs(a)+width_src*Math.Abs(b));
            int height_dst = (int)(height_src * Math.Abs(b) + width_src * Math.Abs(a));

            //4.
            //浮点数表示的坐标:旋转的中心
            PointF center = new PointF(width_src / 2, height_src / 2);
            //变换需要的矩阵:为什么要2行3列
            Matrix<float> matOld = new Matrix<float>(2, 3);
            
            //计算旋转矩阵center：正值表示逆时针；坐标原点在左上角，角度制

            CvInvoke.cv2DRotationMatrix(center, degree, 1, matOld);
            //修改旋转矩阵，保证旋转后的图片仍在框中
            matOld[0, 2] += (width_dst - width_src) / 2;
            matOld[1, 2] += (height_dst - height_src) / 2;
            
            //利用变换矩阵旋转一定角度
            
            //INTER.CV_INTER_LINEAR=1
            //最后一个参数改为1，旋转的方法就会报错
            IntPtr intptr_dst = CvInvoke.cvCreateImage(new System.Drawing.Size(width_dst, height_dst), IPL_DEPTH.IPL_DEPTH_8U, 3);
            
            //将背景色由默认的0改为255            
            MIplImage intptr_temMI = IntPtrToMIplImage(intptr_dst);
            for (int w = 0; w < intptr_temMI.width; w++)
            {
                for (int h = 0; h < intptr_temMI.height; h++)
                {
                    //坐标顺序是（高度，宽度）！写反了报内存的错
                    CvInvoke.cvSet2D(intptr_dst, h, w, new MCvScalar(255,255,255));
                }
            }

            CvInvoke.cvWarpAffine(intptr_src, intptr_dst, matOld, 1, new MCvScalar(255, 255, 255));
            MIplImage intptr_dstMI = IntPtrToMIplImage(intptr_dst);
            //如何将MIplImage转为bitmap---Image<Bgr, byte>可以.toBitmap()转为bitmap
            //是否可以将MIplImage转为Image<Bgr, byte>
            Bitmap finalbmp = MIplImageToBitmap(intptr_dstMI);
            
            return finalbmp;

                           
        }
        /// <summary>
        /// 获取在y轴方向上，第一个黑色像素点与最后一个黑色像素点的距离
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static int GetBlackWidth(Bitmap img)
        {
            List<int> List = new List<int>();
            int black_width = 0;
            for (int x = 0; x < img.Width; x++)
            {
                bool isWhilteLine = false;
                for (int y = 0; y < img.Height; y++)
                {
                    Color __c = img.GetPixel(x, y);
                    if (__c.R == 255)
                    {
                        isWhilteLine = true;
                    }
                    else
                    {
                        isWhilteLine = false;
                        break;
                    }
                }
                if (!isWhilteLine)
                {
                    List.Add(x);
                    black_width++;
                }
            }
            return black_width;
        }
        
        #region 转换方法
        //下面这些方法都不对啊，必须要for循环取出所有的像素点赋值才可以？
        //bitmap转IntPtr: bmp.GetHbitmap()
        /// <summary>
        /// Bitmap转MIplImage
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static MIplImage BitmapToMIplImage(Bitmap bmp)
        {            
            Emgu.CV.Image<Bgr, Byte> img = new Image<Bgr, byte>(bmp);
            MIplImage mi = img.MIplImage;
            return mi;
        }
        /// <summary>
        /// IntPtr转MIplImage
        /// </summary>
        /// <param name="intptr"></param>
        /// <returns></returns>
        public static MIplImage IntPtrToMIplImage(IntPtr intptr) 
        {
            MIplImage mi = new MIplImage();
            mi = (MIplImage)Marshal.PtrToStructure(intptr, typeof(MIplImage));
            return mi;
        }
        /// <summary>
        /// IntPtr转Image<Bgr, byte>
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Image<Hsv, byte> IntPtrToImage(IntPtr ip,Bitmap bmp)
        {
            Image<Hsv, byte> img_now = new Image<Hsv, byte>(bmp.Width,bmp.Height);
            Marshal.PtrToStructure(ip, img_now);
            //Image<Hsv, byte> dst = new Image<Hsv, byte>(CvInvoke.cvGetSize(ip));
            return img_now;
        }
        public static Bitmap MIplImageToBitmap(MIplImage mi)
        {
            PixelFormat pixelFormat;    //像素格式
            string unsupportedDepth = "不支持的像素位深度IPL_DEPTH";
            string unsupportedChannels = "不支持的通道数（仅支持1，2，4通道）";
            switch (mi.nChannels)
            {
                case 1:
                    switch (mi.depth)
                    {
                        case IPL_DEPTH.IPL_DEPTH_8U:
                            pixelFormat = PixelFormat.Format8bppIndexed;
                            break;
                        case IPL_DEPTH.IPL_DEPTH_16U:
                            pixelFormat = PixelFormat.Format16bppGrayScale;
                            break;
                        default:
                            throw new NotImplementedException(unsupportedDepth);
                    }
                    break;
                case 3:
                    switch (mi.depth)
                    {
                        case IPL_DEPTH.IPL_DEPTH_8U:
                            pixelFormat = PixelFormat.Format24bppRgb;
                            break;
                        case IPL_DEPTH.IPL_DEPTH_16U:
                            pixelFormat = PixelFormat.Format48bppRgb;
                            break;
                        default:
                            throw new NotImplementedException(unsupportedDepth);
                    }
                    break;
                case 4:
                    switch (mi.depth)
                    {
                        case IPL_DEPTH.IPL_DEPTH_8U:
                            pixelFormat = PixelFormat.Format32bppArgb;
                            break;
                        case IPL_DEPTH.IPL_DEPTH_16U:
                            pixelFormat = PixelFormat.Format64bppArgb;
                            break;
                        default:
                            throw new NotImplementedException(unsupportedDepth);
                    }
                    break;
                default:
                    throw new NotImplementedException(unsupportedChannels);

            }
            Bitmap bitmap = new Bitmap(mi.width, mi.height, mi.widthStep, pixelFormat, mi.imageData);
            //对于灰度图像，还要修改调色板
            //if (pixelFormat == PixelFormat.Format8bppIndexed)
            //    SetColorPaletteOfGrayscaleBitmap(bitmap);
            return bitmap;
        }
        #endregion

        #region 分割图片
        /// <summary>  
        /// 分割图片  
        /// </summary>  
        /// <returns>处理后的验证码</returns>  
        public List<Bitmap> CutImg(Bitmap cutbmp)
        {
            //存放分割后的字符的列表，作为返回值
            List<Bitmap> littleBmpList = new List<Bitmap>();
            //Y轴分割  
            CutY(cutbmp);
            //区域个数  
            int __count = 0;
            if (XList.Count > 1)
            {
                //x起始值  
                int __start = XList[0];
                //x结束值  
                int __end = XList[XList.Count - 1];
                //x索引  
                int __idx = 0;
                while (__start != __end)
                {
                    //区域宽度  
                    int __w = __start;
                    //区域个数自加  
                    __count++;
                    while (XList.Contains(__w) && __idx < XList.Count)
                    {
                        //区域宽度自加  
                        __w++;
                        //x索引自加  
                        __idx++;
                    }
                    //区域X轴坐标  
                    int x = __start;
                    //区域Y轴坐标  ，纵坐标的起点
                    int y = 0;
                    //区域宽度
                    int width = __w - __start;
                    //区域高度
                    int height = cutbmp.Height;
                    /* 
                     * X轴分割当前区域
                     */
                    CutX(cutbmp.Clone(new Rectangle(x, y, width, height), cutbmp.PixelFormat));
                    if (YList.Count > 1 && YList.Count != cutbmp.Height)
                    {
                        int y1 = YList[0];
                        int y2 = YList[YList.Count - 1];
                        if (y1 != 0)
                        {
                            y = y1 - 1;
                        }
                        height = y2 - y1 + 1;
                    }
                    //GDI+绘图对象
                    //Graphics g = Graphics.FromImage(cutbmp);
                    //g.SmoothingMode = SmoothingMode.HighQuality;
                    //g.CompositingMode = CompositingMode.SourceOver;
                    //g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //画出字符区域  
                    //g.DrawRectangle(new Pen(Brushes.Green), new Rectangle(x, y, width, height));

                    
                    //每画出一个矩形，生成一张同大小的bitmap
                    Bitmap childbmp = new Bitmap(width, height);
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color c = cutbmp.GetPixel(x + i, y + j);
                            childbmp.SetPixel(i, j, c);
                        }
                    }
                    littleBmpList.Add(childbmp);
                    #region 对childbmp进行倾斜矫正
                    //横坐标不动，纵坐标遍历



                    #endregion
                    
                    
                    //g.Dispose();
                    //起始值指向下一组
                    if (__idx < XList.Count)
                    {
                        __start = XList[__idx];
                    }
                    else
                    {
                        __start = __end;
                        
                    }

                }
            }
            return littleBmpList;
        }
        #endregion
        #region Y轴字符分割图片
        /// <summary>  
        /// 得到Y轴分割点  
        /// 判断每一竖行是否有黑色  
        /// 有则添加  
        /// </summary>  
        /// <param name="img">要验证的图片</param>  
        private void CutY(Bitmap img)
        {
            XList.Clear();
            for (int x = 0; x < img.Width; x++)
            {
                bool isWhilteLine = false;
                for (int y = 0; y < img.Height; y++)
                {
                    Color __c = img.GetPixel(x, y);
                    if (__c.R == 255)
                    {
                        isWhilteLine = true;
                    }
                    else
                    {
                        isWhilteLine = false;
                        break;
                    }
                }
                if (!isWhilteLine)
                {
                    XList.Add(x);
                }
            }
        }
        #endregion

        #region X轴字符分割图片
        /// <summary>  
        /// 得到X轴分割点  
        /// 判断每一横行是否有黑色  
        /// 有则添加  
        /// </summary>  
        /// <param name="tempImg">临时区域</param>  
        private void CutX(Bitmap tempImg)
        {
            YList.Clear();
            for (int x = 0; x < tempImg.Height; x++)
            {
                bool isWhilteLine = false;
                for (int y = 0; y < tempImg.Width; y++)
                {
                    Color __c = tempImg.GetPixel(y, x);
                    if (__c.R == 255)
                    {
                        isWhilteLine = true;
                    }
                    else
                    {
                        isWhilteLine = false;
                        break;
                    }
                }
                if (!isWhilteLine)
                {
                    YList.Add(x);
                }
            }
            tempImg.Dispose();
        }
        #endregion

        public static void getConnectedDomain(Bitmap bmp)//boundingbox为最终结果，存放各个连通域的包围盒  
        {
            //Mat& src, vector<Rect>& boundingbox
            //int img_row = src.rows;
            //int img_col = src.cols; 

            int img_row = bmp.Width;
            int img_col = bmp.Height;
            //Mat flag = Matrix.zeros(Size(img_col, img_row), IPL_DEPTH.IPL_DEPTH_8U);//标志矩阵，为0则当前像素点未访问过  
            int [,]flag=new int[img_col,img_row];
            for (int i = 0; i < img_row; i++)  
            {  
                for (int j = 0; j < img_col; j++)
                {  
                    if (bmp.GetPixel(i,j).B==0&&flag[i,j]==0 )  //src.ptr<uchar>(i)[j] == 0 && flag.ptr<uchar>(i)[j] == 0黑色像素且未标记03-20
                    {  
                        Stack<Point> cd=new Stack<Point>();  
                        cd.Push(new Point(j, i));  
                        flag[i,j] = 1;  
                        int minRow = i, minCol = j;//包围盒左、上边界  
                        int maxRow = i, maxCol = j;//包围盒右、下边界  
                        while (cd.Count>0 )  //!cd.Empty()不为空--ml03-20
                        {  
                            Point tmp = cd.Peek();  
                            if (minRow > tmp.Y)//更新包围盒  
                                minRow = tmp.Y;  
                            if (minCol > tmp.X)  
                                minCol = tmp.X;  
                            if (maxRow < tmp.Y)  
                                maxRow = tmp.Y;  
                            if (maxCol < tmp.X)  
                                maxCol = tmp.X;  
                            cd.Pop();  
                            Point []p=new Point[4];//邻域像素点，这里用的四邻域  
                            p[0] =new Point(tmp.X - 1 > 0 ? tmp.X - 1 : 0, tmp.Y);  
                            p[1] =new Point(tmp.X + 1 < img_col - 1 ? tmp.X + 1 : img_row - 1, tmp.Y);  
                            p[2] =new Point(tmp.X, tmp.Y - 1 > 0 ? tmp.Y - 1 : 0);  
                            p[3] =new Point(tmp.X, tmp.Y + 1 < img_row - 1 ? tmp.Y + 1 : img_row - 1);  
                            for (int m = 0; m < 4; m++)  
                            {  
                                int x = p[m].Y;  
                                int y = p[m].X;  
                                if (bmp.GetPixel(x,y).B==0 && flag[x,y] == 0)//如果未访问，则入栈，并标记访问过该点  
                                {  
                                    cd.Push(p[m]);  
                                    flag[x,y] = 1;  
                                }  
                            }  
                        }  
                        Rectangle rect=new Rectangle(minCol, minRow,maxCol + 1, maxRow + 1);  
                        //boundingbox.push_back(rect);  
                    }  
                }  
            }  
        }

        //废弃2018-04-19
        public void rename() 
        {
            //StreamReader sr = new StreamReader(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\train\mappings.txt");
            // 读100个字符 
            int nChars = 9;
            char[] charArray = new char[nChars];
            //int nCharsRead =0;
            //string nextLine;
            //while ((nextLine = sr.ReadLine()) != null)
            //{
            //nCharsRead = sr.Read(charArray, i, nChars);
            //}
            //sr.Close();

            string str = File.ReadAllText(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\train\0mappings.txt");
            //跨度为10，因为包含了转行符“\n”
            for (int i = 0; i < str.Length - 10; i += 10)
            {
                try
                {
                    charArray = str.Substring(i, 9).ToCharArray();

                    string fileNo = charArray[0].ToString() + charArray[1].ToString() + charArray[2].ToString() + charArray[3].ToString();
                    string fileName = charArray[5].ToString() + charArray[6].ToString() + charArray[7].ToString() + charArray[8].ToString();
                    FileInfo file = new FileInfo(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\train\" + fileNo + ".jpg");
                    file.MoveTo(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\train\" + fileName + ".jpg");
                }
                catch (Exception ex)
                {
                    charArray = str.Substring(i, 9).ToCharArray();

                    string fileNo = charArray[0].ToString() + charArray[1].ToString() + charArray[2].ToString() + charArray[3].ToString();
                    string fileName = charArray[5].ToString() + charArray[6].ToString() + charArray[7].ToString() + charArray[8].ToString();
                    FileInfo file = new FileInfo(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\train\" + fileNo + ".jpg");
                    file.Delete();
                    continue;
                }

            }
        
        }
    }
}
