/* --------------------------------------------------------
 * 作者：liufeng
 * 
 * 博客：
 * 
 * 开发环境：
 *      Visual Studio V2012
 *      .NET Framework 4.5
 *      
 * 版本历史：
 *      V1.0    2018年0*月**日
 *              验证码识别winform程序           
--------------------------------------------------------- */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;//Bitmap类
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;//BitmapData类
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Data.OleDb;
using LikeDegree;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.ML;
using Emgu.CV.ML.Structure;
//using Emgu.CV.Features2D;
using System.Globalization;

namespace Pictureanalysis
{
    public partial class Analysis : Form
    {
        public static string Myconnstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=新建文件夹/Picture.accdb";
        OleDbConnection mycnn = new OleDbConnection();
        OleDbCommand cmd = new OleDbCommand();
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        DataSet dt = new DataSet();

        //Bitmap bmp = new Bitmap(Application.StartupPath/*表示程序的Debug目录*/+ @"/image/49---52.bmp");//F://water.jpg
        public static Bitmap cutbmp = new Bitmap(Application.StartupPath/*表示程序的Debug目录*/+ @"/image/WaitForCut.bmp");//F://water.jpg
        public List<int> XList = new List<int>();//用于分割字符的泛型,在CutImg方法中使用
        public  List<int> YList = new List<int>();
        public static int count = 0;//字符入标准库需要用到的索引
        StringBuilder regnizeresult = new StringBuilder();//存放识别字符的结果，在字符分割按钮中使用
        //GetLikeHood实例
        public GetLikeHood GetLikeHood = new GetLikeHood();
        public Analysis()
        {
            
            InitializeComponent();
            //ANN();
            //sda();
        }
        
        
        public void sda()
        {

            //Matrix<float> layerSize = new Matrix<float>(1,4);
            //layerSize.Data[0,0]=-0.78F;
            //layerSize.Data[0,1]= -0.87F;
            //layerSize.Data[0,2]= 36;
            //layerSize.Data[0,3]=0.1F;
            //PicUtils.getMax(layerSize);
            




            //IntPtr image = CvInvoke.cvCreateImage(new System.Drawing.Size(400, 300),IPL_DEPTH.IPL_DEPTH_8U, 1);
            //CvInvoke.cvResize(image, image, INTER.CV_INTER_LINEAR);
            
            //Image<Hsv, Byte> Iimage = new Image<Hsv, byte>(bmp);
            //Image<Hsv, Byte> HsvImage = Iimage.Resize(32,32);
            //pictureBox1.Image = HsvImage.ToBitmap();

        }
        /// <summary>
        /// FIXME
        /// </summary>
        //public void Rotate() 
        //{
        //    //浮点数表示的坐标
        //    PointF center = new PointF(3, 4);
        //    Matrix<float> trainData = new Matrix<float>(100, 2);
        //    //变换需要的矩阵
        //    //计算旋转矩阵center：正值表示逆时针；坐标原点在左上角
        //    IntPtr ip = CvInvoke.cv2DRotationMatrix(center, 20, 1, trainData);
        //    //Matrix<float> transforMatrix = CvInvoke.cv2DRotationMatrix(center, 20, 1, trainData);
        //    //利用变换矩阵旋转一定角度
        //    //CvInvoke.cvWarpAffine();
        //}
        private void Analysis_Load(object sender, EventArgs e)
        {

            //Bitmap newbmp = LikeDegree.GetLikeHood.Rotate(bmp, 360);
            //pictureBox1.Image = newbmp;
            
        }
        
        
        
        /// <summary>
        /// FIXME
        /// </summary>
        /// <param name="waitRotateBmp"></param>
        /// <returns></returns>
        //public Bitmap degreeCorrect(ref Bitmap waitRotateBmp)
        //{
        //    int bw = LikeDegree.GetLikeHood.GetBlackWidth(waitRotateBmp);
        //    //1.5度
        //    waitRotateBmp = LikeDegree.GetLikeHood.Rotate(waitRotateBmp, 5);
        //    int bw2 = LikeDegree.GetLikeHood.GetBlackWidth(waitRotateBmp);
        //    waitRotateBmp = LikeDegree.GetLikeHood.Rotate(waitRotateBmp, -10);
        //    //减小了
        //    int bw1 = LikeDegree.GetLikeHood.GetBlackWidth(waitRotateBmp);
        //    if (bw >= LikeDegree.GetLikeHood.GetBlackWidth(waitRotateBmp))
        //    {
        //        while (bw >= LikeDegree.GetLikeHood.GetBlackWidth(waitRotateBmp))
        //        {
        //            waitRotateBmp = LikeDegree.GetLikeHood.Rotate(waitRotateBmp, 5);
        //        }
        //        return LikeDegree.GetLikeHood.Rotate(waitRotateBmp, -5);
        //    }
        //    //增大了
        //    else 
        //    {
        //        waitRotateBmp = LikeDegree.GetLikeHood.Rotate(waitRotateBmp, -10);
        //        while (bw >= LikeDegree.GetLikeHood.GetBlackWidth(waitRotateBmp))
        //        {
        //            waitRotateBmp = LikeDegree.GetLikeHood.Rotate(waitRotateBmp, -5);
        //        }
        //        return LikeDegree.GetLikeHood.Rotate(waitRotateBmp, 5);
        //    }

        //}
        #region Old方法
        #region 分割图片
        /// <summary>  
        /// 分割只有单个字符的图片，去除多余的空白部分，在字符分割之后调用,
        /// </summary>  
        /// <returns>处理后的验证码</returns>  
        public  Bitmap SingleWordCutImg(Bitmap cutbmp)
        {

            //Y轴分割  
            CutY(cutbmp);//将黑色像素的X轴坐标放入XList
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
                    while (XList.Contains(__w) && __idx < XList.Count)
                    {
                        //区域宽度自加  
                        __w++;
                        //x索引自加  
                        __idx++;
                    }
                    //区域X轴坐标  
                    int x = __start;
                    //区域Y轴坐标  
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
                        int y1 = YList[0] == 0 ? 1 : YList[0];
                        //y=0的原因是因为YList.Count = cutbmp.Height，所以if没进来，y还是初始值0
                        //int y1 = YList[0];
                        int y2 = YList[YList.Count - 1];
                        if (y1 != 1)
                        {
                            y = y1 - 1;
                        }
                        height = y2 - y1 + 1;
                    }
                    //GDI+绘图对象
                    Graphics g = Graphics.FromImage(cutbmp);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
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

                    cutbmp = childbmp;
                    
                    
                    g.Dispose();
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
            return cutbmp;
        }
        #endregion
        
        
        
        
        
        
        
        
        
        /// <summary>
        /// 保存生成的图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveBmp(Bitmap bmp)
        {
            if (bmp == null)
            {
                return;
            }
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Title = "保存为";
            saveDlg.OverwritePrompt = true;
            saveDlg.Filter =
                "BMP文件 (*.bmp) | *.bmp|" +
                "Gif文件 (*.gif) | *.gif|" +
                "JPEG文件 (*.jpg) | *.jpg|" +
                "PNG文件 (*.png) | *.png";
            saveDlg.ShowHelp = true;
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveDlg.FileName;
                string strFilExtn = fileName.Remove(0, fileName.Length - 3);
                switch (strFilExtn)
                {
                    case "bmp":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "jpg":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "gif":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "tif":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case "png":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
                MessageBox.Show("保存成功");
            }
            

        }
        
        /// <summary>
        /// 求联结数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int DetectConnectivity(int []list) 
        {
            Int32 count = list[6] - list[6] * list[7] * list[0];
            count += list[0] - list[0] * list[1] * list[2];
            count += list[2] - list[2] * list[3] * list[4];
            count += list[4] - list[4] * list[5] * list[6];
            return count; 

        }
        
        
        
        

        #region 分割图片
        /// <summary>  
        /// 分割图片  
        /// </summary>  
        /// <returns>处理后的验证码</returns>  
        public Bitmap CutImg(Bitmap cutbmp)
        {
            
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
                        if (y1 != 1)
                        {
                            y = y1 - 1;
                        }
                        height = y2 - y1 + 1;
                    }
                    //GDI+绘图对象
                    Graphics g = Graphics.FromImage(cutbmp);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
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
                    
                    #region 对childbmp进行倾斜矫正
                    //横坐标不动，纵坐标遍历
                    


                    #endregion
                    #region//是否画切割出来的图片
                    /*
                    switch (count)
                    {
                        case 1:
                            pictureBox2.Image = childbmp;
                            break;
                        case 2:
                            pictureBox3.Image = childbmp;
                            break;
                        case 3:
                            pictureBox4.Image = childbmp;
                            break;
                        case 4:
                            pictureBox5.Image = childbmp;
                            break;
                        default:
                            break;
                    }
                    count++;
                    */
                    #endregion
                    
                    //SaveCutBmpToAccess(childbmp);
                    //childbmp.Save(x + "---" + y + ".bmp");
                    string wordstring = CutNM(childbmp, 5, 5);
                    string regnize = CharRegnize(wordstring);
                    regnizeresult.Append(regnize);
                    //SaveMaterial(s);//保存到原料库,当需要向原料库增加标准字符时，才取消注释
                    g.Dispose();
                    //起始值指向下一组
                    if (__idx < XList.Count)
                    {
                        __start = XList[__idx];
                    }
                    else
                    {                        
                        __start = __end;
                        //return childbmp;
                    }

                }
            }
            return cutbmp;
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
                    Color  __c = img.GetPixel(x, y);
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
                    Color  __c = tempImg.GetPixel(y, x);
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
        #region 黑色像素比列
        
        #endregion

        #region 暂时没用的方法
        public void CutImgToDB(Bitmap cutbmp, string truewords)//与CutImg方法相对应，该方法用于将字符存入数据库
        {

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
                    //区域Y轴坐标  
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
                        if (y1 != 1)
                        {
                            y = y1 - 1;
                        }
                        height = y2 - y1 + 1;
                    }
                    //GDI+绘图对象  
                    Graphics g = Graphics.FromImage(cutbmp);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //画出验证码区域  
                    g.DrawRectangle(new Pen(Brushes.Green), new Rectangle(x, y, width, height));
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
                    string wordstring = CutNM(childbmp, 5, 5);
                    SaveMaterial(wordstring, truewords);//保存到原料库,当需要向原料库增加标准字符时，才取消注释
                    g.Dispose();
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
        }
        /// <summary>
        /// 将一幅图片以数组的形式存入数据库
        /// </summary>
        /// <param name="cutbmp"></param>
        public void SaveCutBmpToAccess(Bitmap cutbmp) //mark是图片的标记
        {
            try
            {
                double pp = PicUtils.PixlPercent(cutbmp);
                MemoryStream ms = new MemoryStream();
                cutbmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Flush();
                //将二进制数据存到byte数字中
                byte[] bmpBytes = ms.ToArray();
                pictureBox1.Image = Image.FromStream(new MemoryStream(bmpBytes));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bmpBytes.Length; i++)
                {
                    if (i != bmpBytes.Length - 1)
                    {
                        sb.Append(bmpBytes[i] + ",");
                    }
                    else
                    {
                        sb.Append(bmpBytes[i]);
                    }

                }
                if(sb.Length>65536)
                {
                    sb.Clear();
                    sb.Append("该图片太大了，字节数超过65536");                    
                }                
                mycnn.ConnectionString = Myconnstr;                
                cmd.Connection = mycnn;//出错,因为cmd为空
                mycnn.Open();
                cmd.CommandText = "delete from AfterCut";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "insert into AfterCut(content_,pixel_percent) values('" + sb.ToString() + "','" + pp.ToString() + "')";//access数据库中的备注类型最大长度为65536，所以图片不能太大
                cmd.ExecuteNonQuery();                
            }
            catch (Exception w)
            {
                MessageBox.Show(w.ToString());
            }
            finally 
            {
                mycnn.Close();                
            }
            
        }
        /// <summary>
        /// 保存样本字符到原料库的方法
        /// </summary>
        /// <param name="s"></param>
        public void SaveMaterial(string s,string truewords)
        {
            try
            {               
                    char[] CharArray = truewords.ToArray();                    
                    mycnn.ConnectionString = Myconnstr;
                    cmd.Connection = mycnn;//出错,因为cmd为空
                    mycnn.Open();
                    cmd.CommandText = "select max(ID) from StandardWord";
                    int maxid = Convert.ToInt16( cmd.ExecuteScalar());
                    maxid++;
                    cmd.CommandText = "insert into StandardWord(ID,VSM,word_) values('"+maxid+"','" + s + "','" + CharArray[count] + "')";//access数据库中的备注类型最大长度为65536，所以图片不能太大
                    cmd.ExecuteNonQuery();
                    count++;                        
                                 
            }            
            catch (Exception w)
            {
                MessageBox.Show(w.ToString());
            }
            finally 
            {
                mycnn.Close();                
            } 
        }
        public string CharRegnize(string waitreg)//传进来待识别字符的字符串表示
        {
            string[] CharArray = waitreg.Split(',');
            try
            {
                mycnn.ConnectionString = Myconnstr;
                cmd.Connection = mycnn;//出错,因为cmd为空
                mycnn.Open();
                cmd.CommandText = "select * from StandardWord";//access数据库中的备注类型最大长度为65536，所以图片不能太大
                //cmd.ExecuteNonQuery();//？？
                adapter.SelectCommand = cmd;
                adapter.Fill(dt, "原料库");
                double []result=new double[dt.Tables["原料库"].Rows.Count];
                for (int rec = 0; rec < dt.Tables["原料库"].Rows.Count; rec++)
                {
                    string vsm = dt.Tables["原料库"].Rows[rec]["VSM"].ToString();
                    string[] aim = vsm.Split(',');
                    result[rec] = LikeDegree.GetLikeHood.EuclideanDistanceDouble(CharArray, aim);//输入字符串数组
                }
                double compare = result[0];
                int pos = 0;
                for (int i = 0; i < result.Length;i++ )
                {
                    if(compare>=result[i])
                    {
                        compare = result[i];
                        pos = i;
                    }
                }    
                cmd.CommandText = "select word_ from StandardWord where word_='" + dt.Tables["原料库"].Rows[pos]["word_"].ToString() + "'";//access数据库中的备注类型最大长度为65536，所以图片不能太大
                string back = cmd.ExecuteScalar().ToString();
                cmd.ExecuteNonQuery();//？？
                dt.Clear();
                return back;
                
            }
            catch (Exception w)
            {
                mycnn.Close();
                MessageBox.Show(w.ToString());
                return null;
            }
            finally
            {
                mycnn.Close();                
            } 
        }
        /// <summary>
        /// 用于N*M切割图片，以生成特征向量
        /// </summary>
        /// <param name="NMbmp"></param>
        /// <param name="N"></param>
        /// <param name="M"></param>
        /// <returns>返回生成向量的字符串表示</returns>
        public string CutNM(Bitmap sourceImage, int xNum, int yNum)//传进来一幅图片，切割、计算每份子图的像素比例，最后输出字符串形式的特征向量
        {
            float xWidth = sourceImage.Width / xNum;
            float yWidth = sourceImage.Height / yNum;
            StringBuilder sbresult = new StringBuilder();
            for (int countY = 0; countY < yNum; countY++)
            {
                for (int countX = 0; countX < xNum; countX++)
                {
                    RectangleF cloneRect = new RectangleF(countX * xWidth, countY * yWidth, xWidth, yWidth);
                    Bitmap kidbmp = sourceImage.Clone(cloneRect, PixelFormat.Format24bppRgb);
                    double kidpercent = PicUtils.PixlPercent(kidbmp);
                    //判断是为了以后用split切割时不会出现最后一个数组元素为空的情况
                    if (countY == yNum - 1 && countX == xNum - 1)
                    {
                        sbresult.Append(kidpercent.ToString());
                    }
                    else
                    {
                        sbresult.Append(kidpercent.ToString() + ",");
                    }
                }
            }
            return sbresult.ToString();

        }
        #endregion

#endregion
        #region //************************************菜单按钮↓
        #region 文件下拉列表
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap savebmp = (Bitmap)pictureBox1.Image;
            SaveBmp(savebmp);
        }
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "图像文件(*.bmp;*.jpg;*gif;*png;*.tif;*.wmf)|" + "*.bmp;*jpg;*gif;*png;*.tif;*.wmf";

            if (open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap openbmp = (Bitmap)Image.FromFile(open.FileName);
                    pictureBox1.Image = openbmp;

                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }
                
            }
        }
        #endregion
        private void 生成训练样本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1.首先要实现批量读取图片F:\毕业设计2018届刘锋\myCharSamples\train
            //获取文件夹下的所有图片路径名："F:\\毕业设计2018届刘锋\\OOP.PictureAnalyse\\testImages\\0000.jpg"
            string[] files = Directory.GetFiles(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\train",
                "*.jpg", SearchOption.AllDirectories);
            //所有的待测图像
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    Bitmap bmpTogether = (Bitmap)Image.FromFile(files[i]);
                    //灰度化
                    bmpTogether =MyUtils.ToGray(bmpTogether);
                    //二值化
                    bmpTogether = PicUtils.Sauvola(bmpTogether);
                    //图像去躁
                    bmpTogether = bmpTogether.Clone(new Rectangle(0, 0, bmpTogether.Width, bmpTogether.Height), PixelFormat.Format24bppRgb);

                    bmpTogether = MyUtils.MyGussianBlur(bmpTogether);
                    MyUtils.ClearNoise(bmpTogether, MyUtils.GetDgGrayValue(bmpTogether), 4);//按照需要进行设置

                    Byte[,] BinaryArray = PicUtils.ToBinaryArray(bmpTogether);
                    ImgBoundary Boundary = SegmentFunction.getImgBoundary(BinaryArray);
                    //jpg格式转换为bmp，方式SetPixel方法报错
                    bmpTogether = bmpTogether.Clone(new Rectangle(Boundary.widthMin, Boundary.heightMin,
                        Boundary.widthMax - Boundary.widthMin, Boundary.heightMax - Boundary.heightMin), PixelFormat.Format24bppRgb);

                    //图像分割，应该返回4个子图像
                    List<Bitmap> littleBmpArray = PicUtils.SimpleCutTo4(bmpTogether);//CutImg(cutbmp);
                    //保存路径
                    DirectoryInfo TheFolder = new DirectoryInfo(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\sample");
                    int nameIndex = files[i].LastIndexOf(@"\");//取出最后一个"\"的索引
                    string fileName = files[i].Substring(nameIndex + 1, 4);//取出图像的名称
                    char[] arrayName = fileName.ToArray();//转换为字符数组
                    //再对子图像去除多余的空白部分
                    //一张图像里的所有字符
                    for (int lba = 0; lba < littleBmpArray.Count; lba++)
                    {
                        //1.去除多余部分，不过好像childbmp本来就没有多余的空白部分
                        //littleBmpArray[lba]=SingleWordCutImg(littleBmpArray[lba]);

                        //2.保存图像
                        if (littleBmpArray[lba] == null)
                        {
                            continue;
                        }

                        bool isExist = false;//标记是否已存在该名称的文件夹
                        //先遍历图像名称，因为文件夹名称刚开始是没有的

                        //-如果已存在DirectoryInfo[] GetDirectories()
                        if (TheFolder.GetDirectories() != null)
                        {
                            string pathName = "";//最终保存图像的完整路径名
                            foreach (DirectoryInfo nextdirectory in TheFolder.GetDirectories())
                            {

                                if (nextdirectory.Name == arrayName[lba].ToString())
                                {
                                    isExist = true;
                                    string pre = nextdirectory.FullName + @"\";//前缀
                                    string mid = arrayName[lba].ToString();//中间动态名称
                                    mid += "_";
                                    mid += DateTime.Now.ToString("mm-ss-fffffff", DateTimeFormatInfo.InvariantInfo);
                                    string suf = ".bmp";//后缀

                                    pathName = pre + mid + suf;//"F:\\毕业设计2018届刘锋\\OOP.PictureAnalyse\\myCharSamples\\train\\00-01773.bmp"
                                    //存到已存在的文件夹中           第一个参数："F:\\毕业设计2018届刘锋\\asdad.bmp"
                                    littleBmpArray[lba].Save(pathName, System.Drawing.Imaging.ImageFormat.Bmp);
                                    break;//跳出foreach
                                }

                            }
                            //如果不存在对应的文件夹
                            if (!isExist)
                            {
                                string newDir = TheFolder.FullName + @"\" + arrayName[lba].ToString();
                                if (!Directory.Exists(newDir))
                                {
                                    Directory.CreateDirectory(newDir);
                                    string pre = newDir + @"\";//前缀
                                    string mid = arrayName[lba].ToString();//中间动态名称
                                    mid += "_";
                                    mid += DateTime.Now.ToString("mm-ss-fffffff", DateTimeFormatInfo.InvariantInfo);
                                    string suf = ".bmp";//后缀
                                    pathName = pre + mid + suf;
                                    littleBmpArray[lba].Save(pathName, System.Drawing.Imaging.ImageFormat.Bmp);
                                    continue;
                                }
                            }
                        }



                    }
                }
                catch (Exception ex)
                {
                    //遇到异常就跳过，处理下一张图片
                    continue;
                }

            }
        }



        /// <summary>
        /// author:liusimawen
        /// date:2018-04-19
        /// 识别--计算识别率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 计算识别率ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt;)|" + "*.txt;";

            if (open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    
                    int nChars = 9;
                    char[] charArray = new char[nChars];

                    string str = File.ReadAllText(open.FileName);
                    int totalC = 0, rightC4 = 0, rightC3 = 0, rightC2 = 0, rightC1 = 0, rightC0 = 0;//计数499,196
                    //跨度为10，因为包含了转行符“\n”
                    for (int i = 0; i < str.Length - 11; i += 11)//利用正则表达式动态设置-11还是-10；---ml 未解决
                    {
                        totalC++;//记事本中所有的记录数
                        try
                        {
                            charArray = str.Substring(i, 9).ToCharArray();

                            string rightStr = charArray[0].ToString() + charArray[1].ToString() + charArray[2].ToString() + charArray[3].ToString();
                            string testStr = charArray[5].ToString() + charArray[6].ToString() + charArray[7].ToString() + charArray[8].ToString();
                            
                            int count = MyUtils.strCompare(rightStr, testStr);
                            switch(count)
                            {
                                case 0:
                                    rightC0++;
                                    break;
                                case 1:
                                    rightC1++;
                                    break;
                                case 2:
                                    rightC2++;
                                    break;
                                case 3:
                                    rightC3++;
                                    break;
                                case 4:
                                    rightC4++;
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                    }
                    float rightRate4 = (float)rightC4 / totalC*100;
                    float rightRate3 = (float)rightC3 / totalC * 100;
                    float rightRate2 = (float)rightC2 / totalC * 100;
                    float rightRate1 = (float)rightC1 / totalC * 100;
                    float rightRate0 = (float)rightC0 / totalC * 100;
                    string message = "总记录："+totalC+"\n"
                        + "对4个：" + rightC4 +","+ "识别率：" + rightRate4 + "\n"
                        + "对3个：" + rightC3 + "," + "识别率：" + rightRate3 + "\n"
                        + "对2个：" + rightC2 + "," + "识别率：" + rightRate2 + "\n"
                        + "对1个：" + rightC1 + "," + "识别率：" + rightRate1 + "\n"
                        + "对0个：" + rightC0 + "," + "识别率：" + rightRate0;
                    MessageBox.Show(message);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }

            }
        }
        #endregion

        
        

        

       

        

        

        
        /// <summary>
        /// author:liusimawen
        /// date:2018-03-22
        /// 传入bitmap，返回旋转纠正，并连通域最大的bitmap
        /// </summary>
        /// <param name="mp"></param>
        /// <returns></returns>
        public static Bitmap DegreeCorrect(Bitmap mp,int degree=15)
        {
            Bitmap benchBmp = mp;

            int bw = LikeDegree.GetLikeHood.GetBlackWidth(mp);
            //resullt_box.Text = bw + "";
            mp = LikeDegree.GetLikeHood.Rotate(mp, -degree);//度数不能太小
            //int bw1 = LikeDegree.GetLikeHood.GetBlackWidth(mp);
            if (bw >= LikeDegree.GetLikeHood.GetBlackWidth(mp))
            {
                while (bw >= LikeDegree.GetLikeHood.GetBlackWidth(mp))
                {
                    bw = LikeDegree.GetLikeHood.GetBlackWidth(mp);
                    mp = LikeDegree.GetLikeHood.Rotate(mp, -degree);
                    
                }
            }
            else 
            {
                
                mp = LikeDegree.GetLikeHood.Rotate(benchBmp, degree);
                int newbw = LikeDegree.GetLikeHood.GetBlackWidth(mp);
                //反方向转动之后宽度还是变大，说明原来就是正的
                //if (bw < LikeDegree.GetLikeHood.GetBlackWidth(mp))
                //{
                //    List<Bitmap> bmpThinList1 = new GetLikeHood().CutImg(benchBmp);
                //    mp = bmpThinList1.Where(btl => btl.Width == bmpThinList1.Max(bt => bt.Width)).FirstOrDefault();
                //    return mp;
                //}
                //else
                //{
                //    while (bw >= LikeDegree.GetLikeHood.GetBlackWidth(mp))
                //    {
                //        bw = LikeDegree.GetLikeHood.GetBlackWidth(mp);
                //        mp = LikeDegree.GetLikeHood.Rotate(mp, degree);

                //    }
                //}


                while (bw >= LikeDegree.GetLikeHood.GetBlackWidth(mp))
                {
                    bw = LikeDegree.GetLikeHood.GetBlackWidth(mp);
                    mp = LikeDegree.GetLikeHood.Rotate(mp, degree);

                }
                
            }
            //mp = RotateCutImg(mp);
            //mp =new Analysis().SingleWordCutImg(mp);
            //分割
            List<Bitmap> bmpThinList = new GetLikeHood().CutImg(mp);
            mp = bmpThinList.Where(btl => btl.Width == bmpThinList.Max(bt => bt.Width)).FirstOrDefault();
            return mp;
        }
        /// <summary>
        /// author:liusimawen
        /// date:2018-04-19
        /// 训练样本--旋转训练集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 旋转训练集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Now;
                //总样本数
                Matrix<float> trainData = new Matrix<float>(6200, 50);
                //生成--输出矩阵
                Matrix<float> trainClasses = new Matrix<float>(6200, 31);
                Matrix<float> sample = new Matrix<float>(1, 50);
                Matrix<float> prediction = new Matrix<float>(1, 31);
                DirectoryInfo theFolder = new DirectoryInfo(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\sample");
                DirectoryInfo[] dirInfo = theFolder.GetDirectories();
                //遍历文件夹
                for (int dc = 0; dc < dirInfo.Length; dc++)//max--36
                {
                    DirectoryInfo NextFolder = dirInfo[dc];
                    string[] files = Directory.GetFiles(NextFolder.FullName, "*.bmp", SearchOption.AllDirectories);
                    for (int fc = 0; fc < files.Length; fc++)//max--50
                    {
                        try
                        {
                            Bitmap bmp50 = (Bitmap)Image.FromFile(files[fc]);
                            //旋转纠正
                            bmp50 = DegreeCorrect(bmp50);

                            Matrix<float> rowPP = PicUtils.calPixlPercent(bmp50, 5, 10);
                            //将特征向量赋值给输入矩阵
                            for (int rppc = 0; rppc < rowPP.Cols; rppc++)
                            {
                                trainData.Data[dc * files.Length + fc, rppc] = rowPP[0, rppc];
                            }
                            //初始化trainClasses,对应位置的值设置为1
                            trainClasses.Data[dc * files.Length + fc, dc] = 1;
                        }
                        catch (Exception sex)
                        {
                            continue;
                        }
                    }


                }

                PicUtils.trainANN(trainData, trainClasses);
                DateTime dtEnd = DateTime.Now;
                TimeSpan ts = dtEnd - dtStart;
                MessageBox.Show("样本训练完成，已保存xml文件！用时" + ts.TotalMinutes);
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// author:liusimawen
        /// date:2018-04-19
        /// 识别--批量识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 批量识别ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否继续识别", "确认框", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                DateTime dtStart = DateTime.Now;
                //先清空记事本中的记录
                using (System.IO.StreamWriter file = new System.IO.StreamWriter
                            (@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\testresult1.txt", false))//true表示追加在末尾，false为覆盖
                {
                    //file.Write(line);//直接追加文件末尾，不换行
                    file.Write("");// 直接追加文件末尾，换行   
                }
                //1.首先要实现批量读取图片F:\毕业设计2018届刘锋\myCharSamples\train
                //获取文件夹下的所有图片路径名："F:\\毕业设计2018届刘锋\\OOP.PictureAnalyse\\testImages\\0000.jpg"
                string[] files = Directory.GetFiles(@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\test500", "*.jpg", SearchOption.AllDirectories);
                //所有的待测图像
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {



                        Bitmap bmpTest = (Bitmap)Image.FromFile(files[i]);
                        //灰度化
                        bmpTest = MyUtils.ToGray(bmpTest);
                        //二值化
                        bmpTest = PicUtils.Sauvola(bmpTest);
                        //图像去躁
                        bmpTest = bmpTest.Clone(new Rectangle(0, 0, bmpTest.Width, bmpTest.Height), PixelFormat.Format24bppRgb);
                        
                        bmpTest = MyUtils.MyGussianBlur(bmpTest);
                        MyUtils.ClearNoise(bmpTest, MyUtils.GetDgGrayValue(bmpTest), 4);//按照需要进行设置
                        
                        Byte[,] BinaryArray = PicUtils.ToBinaryArray(bmpTest);
                        ImgBoundary Boundary = SegmentFunction.getImgBoundary(BinaryArray);
                        //jpg格式转换为bmp，方式SetPixel方法报错
                        bmpTest = bmpTest.Clone(new Rectangle(Boundary.widthMin, Boundary.heightMin,
                            Boundary.widthMax - Boundary.widthMin, Boundary.heightMax - Boundary.heightMin), PixelFormat.Format24bppRgb);

                        //图像分割，应该返回4个子图像
                        List<Bitmap> littleBmpArray = PicUtils.SimpleCutTo4(bmpTest);//CutImg(cutbmp);
                        //如果子图像不为4个，则跳过本图，继续下一张图片
                        if (littleBmpArray.Count != 4)
                        {
                            continue;
                        }
                        int nameIndex = files[i].LastIndexOf(@"\");//取出最后一个"\"的索引
                        string fileName = files[i].Substring(nameIndex + 1, 4);//取出图像的名称
                        char[] arrayName = fileName.ToArray();//转换为字符数组
                        //再对子图像去除多余的空白部分
                        //一张图像里的所有字符
                        StringBuilder resultSb = new StringBuilder();
                        for (int lba = 0; lba < littleBmpArray.Count; lba++)
                        {

                            if (littleBmpArray[lba] == null)
                            {
                                continue;
                            }

                            //旋转纠正
                            littleBmpArray[lba] = DegreeCorrect(littleBmpArray[lba]);
                            char resultChar = PicUtils.regANN(littleBmpArray[lba]);
                            resultSb.Append(resultChar);
                            

                            




                        }
                        //testresult.txt存储识别结果：正确字符 识别结果
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter
                            (@"F:\毕业设计2018届刘锋\OOP.PictureAnalyse\myCharSamples\A16-浪潮-验证码样本数据\验证码样本数据\data-3\testresult1.txt", true))//true表示追加在末尾，false为覆盖
                        {
                            //file.Write(line);//直接追加文件末尾，不换行
                            file.WriteLine(fileName + " " + resultSb);// 直接追加文件末尾，换行   
                        }
                    }
                    catch (Exception ex)
                    {
                        //遇到异常就跳过，处理下一张图片
                        continue;//对应最外面的for循环
                    }

                }
                DateTime dtEnd = DateTime.Now;
                TimeSpan ts = dtEnd - dtStart;
                MessageBox.Show("识别完成，已保存入testresult1.txt！用时" + ts.Minutes);
            }

        }

        #region 图像处理菜单栏
        private void 灰度化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap mp = (Bitmap)pictureBox1.Image;
                mp = MyUtils.ToGray(mp);
                pictureBox1.Image = mp;
            }
            else
            {
                MessageBox.Show("先导入图片！");
            }
            //bmp = ToGray(bmp);
            //pictureBox1.Image = bmp;

            //Bitmap mp=(Bitmap)pictureBox1.Image;
            ////使用using关键字，限制图像范围，方便自动垃圾回收
            //using (Image<Bgr, Byte> oldImage = new Image<Bgr, byte>(mp)) 
            //{
            //    Image<Bgr, Byte> resizeImage = oldImage.Resize(32, 32);
            //    //获取2行1列的像素点
            //    //Bgr  color=resizeImage[2,1];
            //    //对像素点赋值
            //    //resizeImage[2, 1] = color;
            //    pictureBox1.Image = resizeImage.ToBitmap();
            //}
            
        }

        private void 二值化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap twobmp = (Bitmap)pictureBox1.Image;

            pictureBox1.Image = PicUtils.Sauvola(twobmp);
        }

        private void 去躁ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap ClearNoisejpg = (Bitmap)pictureBox1.Image;
            Bitmap ClearNoisebmp = ClearNoisejpg.Clone(new Rectangle(0, 0, ClearNoisejpg.Width, ClearNoisejpg.Height), PixelFormat.Format24bppRgb);
            //for (int i = 0; i < 1; i++)
            //{
            //    MyUtils.ClearNoise(ClearNoisebmp, MyUtils.GetDgGrayValue(ClearNoisebmp), 4);//按照需要进行设置
            //}
            
            //ClearNoisebmp = MyUtils.MyGussianBlur(ClearNoisebmp);
            MyUtils.ClearNoise(ClearNoisebmp, MyUtils.GetDgGrayValue(ClearNoisebmp), 4);//按照需要进行设置
            pictureBox1.Image = ClearNoisebmp;
        }

        private void 倾斜纠正ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap mp = (Bitmap)pictureBox1.Image;

            mp = DegreeCorrect(mp);
            //mp = DegreeCorrect(mp);
            pictureBox1.Image = mp;
            //int bw = LikeDegree.GetLikeHood.GetBlackWidth(mp);
            //resullt_box.Text = bw + "";
        }

        private void 细化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap twobmp = (Bitmap)pictureBox1.Image;
            Byte[,] binaryBmp = PicUtils.ToBinaryArray(twobmp);
            binaryBmp = Thining.ThinPicture(binaryBmp);
            pictureBox1.Image = PicUtils.BinaryArrayToBinaryBitmap(binaryBmp);
        }
        #endregion
        /// <summary>
        /// author:liusimawen
        /// date:2018-04-19
        /// 单个字符识别按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            Bitmap bmpTest = (Bitmap)pictureBox1.Image;
            //灰度化
            bmpTest = MyUtils.ToGray(bmpTest);
            //二值化
            bmpTest = PicUtils.Sauvola(bmpTest);
            //图像去躁
            bmpTest = bmpTest.Clone(new Rectangle(0, 0, bmpTest.Width, bmpTest.Height), PixelFormat.Format24bppRgb);
            bmpTest = MyUtils.MyGussianBlur(bmpTest);
            MyUtils.ClearNoise(bmpTest, MyUtils.GetDgGrayValue(bmpTest), 4);//按照需要进行设置

            //图像分割，应该返回4个子图像
            List<Bitmap> littleBmpArray = PicUtils.SimpleCutTo4(bmpTest);//CutImg(cutbmp);
            //如果子图像不为4个，则跳过本图，继续下一张图片
            if (littleBmpArray.Count != 4)
            {
                return;
            }
            
            //再对子图像去除多余的空白部分
            //一张图像里的所有字符
            StringBuilder resultSb = new StringBuilder();
            for (int lba = 0; lba < littleBmpArray.Count; lba++)
            {

                if (littleBmpArray[lba] == null)
                {
                    continue;
                }
                
                //旋转纠正
                littleBmpArray[lba] = DegreeCorrect(littleBmpArray[lba]);
                //System.Threading.Thread.Sleep(2000);
                char resultChar = PicUtils.regANN(littleBmpArray[lba]);
                resultSb.Append(resultChar);
            }
            resullt_box.Text = resultSb.ToString();
        }

        

        

        

        
        
    }
}
