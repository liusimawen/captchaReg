using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.ML.Structure;
using Emgu.CV.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pictureanalysis
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class PicUtils
    {
        public static char[] Standard = new char[]
        {
            '0','1','2','3','4','5',
            '6','7','8','9','A','B',
            'C','D','E','F','G','H',
            'I','J','K','L','M','N',
            'O','P','Q','R','S','T',
            'U','V','W','X','Y','Z',
        };
        public static char[] StandardNo0O1IZ = new char[]
        {
            '2','3','4','5',
            '6','7','8','9','A','B',
            'C','D','E','F','G','H',
            'J','K','L','M','N',
            'P','Q','R','S','T',
            'U','V','W','X','Y',
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="testBmp"></param>
        /// <param name="sampleFeature">保存的xml文件的名称</param>
        /// <returns></returns>
        public static char regANN(Bitmap testBmp)
        {
            Matrix<float> sample = PicUtils.calPixlPercent(testBmp, 5, 10);
            Matrix<float> prediction = new Matrix<float>(1, 31);
            //表示创建3层神经网络
            Matrix<int> layerSize = new Matrix<int>(new int[] { 50,100, 100, 31 });
            //
            using (ANN_MLP network = new ANN_MLP(layerSize, Emgu.CV.ML.MlEnum.ANN_MLP_ACTIVATION_FUNCTION.SIGMOID_SYM, 1.0, 1.0))
            {
                //判断是否存在该路径
                //加载训练完成后保存的权值矩阵xml文件
                //FileInfo file = new FileInfo(Application.StartupPath/*表示程序的Debug目录*/+ @"/" + samplexml);
                network.Load(Application.StartupPath/*表示程序的Debug目录*/+ @"/mlp.xml");
                network.Predict(sample, prediction);
            }
            int index = getMax(prediction);
            return StandardNo0O1IZ[index];

        }
        /// <summary>
        /// FIXME
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int getMax(Matrix<float> array)
        {
            float temp = array.Data[0, 0];
            int index = 0;

            for (int i = 0; i < array.Cols; i++)
            {
                if (temp < array[0, i])    //如果用<= 则找到的是最大值(多个中的最后一个) <则是多个中的第一个
                {
                    temp = array[0, i];
                    index = i;
                }
            }
            return index;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainData"></param>
        /// <param name="trainClasses"></param>
        /// <param name="samplexml">保存的权值矩阵xml文件的名称</param>
        public static void trainANN(Matrix<float> trainData, Matrix<float> trainClasses)
        {
            


            //表示创建3层神经网络
            Matrix<int> layerSize = new Matrix<int>(new int[] { 50,100,  100, 31 });

            //初始化，设置神经网络参数
            MCvANN_MLP_TrainParams parameters = new MCvANN_MLP_TrainParams();
            //终止条件：迭代次数、误差最小值
            //50个样本，迭代次数50；105个样本，迭代次数20
            parameters.term_crit = new MCvTermCriteria(40, 0.01);//*********（,1.0e-8）****************为什么设置迭代次数大了之后所有数字都一样了
            //训练方法
            parameters.train_method = Emgu.CV.ML.MlEnum.ANN_MLP_TRAIN_METHOD.BACKPROP;
            //权值更新率
            parameters.bp_dw_scale = 0.01;
            //权值更新冲量
            parameters.bp_moment_scale = 0.05;

            //创建网络
            /*
             * layerSizes:一个整型的数组，这里面用Mat存储。
             * 它是一个1*N的Mat，N代表神经网络的层数，第i列的值表示第i层的结点数。
             * 这里需要注意的是，在创建这个Mat时，一定要是整型的，uchar和float型都会报错。
             * 
             * 第二个参数表示激活函数，三四参数表示sigmoid函数中的参数α、β
             */
            using (ANN_MLP network = new ANN_MLP(layerSize, Emgu.CV.ML.MlEnum.ANN_MLP_ACTIVATION_FUNCTION.SIGMOID_SYM, 1.0, 1.0))
            {
                //**********可以即时训练也可以加载之前训练好的xml文件（权值矩阵）
                network.Train(trainData, trainClasses, null, null, parameters,
                    Emgu.CV.ML.MlEnum.ANN_MLP_TRAINING_FLAG.DEFAULT);
                network.Save("mlp.xml");//保存在Debug目录下--------覆盖
                //network.Load(Application.StartupPath/*表示程序的Debug目录*/+ @"/mlp.xml");
                //network.Predict(sample, prediction);
            }

        }
        /// <summary>
        /// 计算图片的黑色像素比例为特征向量
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="xNum"></param>
        /// <param name="yNum"></param>
        /// <returns></returns>
        public static Matrix<float> calPixlPercent(Bitmap sourceImage, int xNum, int yNum)//传进来一幅图片，切割、计算每份子图的像素比例，最后输出字符串形式的特征向量
        {
            float xWidth = sourceImage.Width / xNum;
            float yWidth = sourceImage.Height / yNum;
            Matrix<float> matresult = new Matrix<float>(1, xNum * yNum);
            for (int countY = 0; countY < yNum; countY++)
            {
                for (int countX = 0; countX < xNum; countX++)
                {
                    RectangleF cloneRect = new RectangleF(countX * xWidth, countY * yWidth, xWidth, yWidth);
                    Bitmap kidbmp = sourceImage.Clone(cloneRect, PixelFormat.Format24bppRgb);
                    double kidpercent = PixlPercent(kidbmp);
                    matresult.Data[0, countY * xNum + countX] = (float)kidpercent;
                }
            }
            return matresult;

        }
        /// <summary>  
        /// 计算黑色像素比列  
        /// </summary>  
        /// <param name="tempimg"></param>  
        /// <returns></returns>  
        public static double PixlPercent(Bitmap tempimg)
        {
            int temp = 0;
            int w_h = tempimg.Width * tempimg.Height;
            for (int x = 0; x < tempimg.Width; x++)
            {
                for (int y = 0; y < tempimg.Height; y++)
                {
                    Color __c = tempimg.GetPixel(x, y);
                    if (__c.R == 0)
                    {
                        temp++;
                    }
                }
            }
            //tempimg.Dispose();
            double result = temp * 1.0 / w_h;
            result = result.ToString().Length > 3 ? Convert.ToDouble(result.ToString().Substring(0, 4)) : result;//0.021025641025641025,substring(0,3)为0.0，小数点算一位
            return result;
        }


        /// <summary>
        /// 灰度化处理图像
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }
        /// <summary>
        /// Get the vertical projection of the image 返回垂直投影的二维数组（也可以返回二维数组对应的图像）
        /// </summary>
        /// <param name="imageSrc">the path of src image</param>
        public static Byte[,] VerticalProjection(Bitmap bmp/*, string imageDestPath*/)
        {


            Int32 imgWidth = bmp.Width;

            Int32 imgHeight = bmp.Height;

            //用于存储当前横坐标垂直方向上的有效像素点数量(组成字符的像素点)
            Int32[] verticalPoints = new Int32[imgWidth];
            Array.Clear(verticalPoints, 0, imgWidth);



            //用于存储竖直投影后的二值化数组
            Byte[,] verticalProArray = new Byte[imgHeight, imgWidth];
            //先将该二值化数组初始化为白色
            for (Int32 x = 0; x < imgWidth; x++)
            {
                for (Int32 y = 0; y < imgHeight; y++)
                {
                    verticalProArray[y, x] = 255;
                }
            }

            //统计源图像的二值化数组中在每一个横坐标的垂直方向所包含的像素点数
            for (Int32 x = 0; x < imgWidth; x++)
            {
                for (Int32 y = 0; y < imgHeight; y++)
                {
                    if (0 == bmp.GetPixel(x, y).B)
                    {
                        verticalPoints[x]++;
                    }
                }
            }

            //将源图像中横坐标垂直方向上所包含的像素点按垂直方向依次从imgWidth开始叠放在竖直投影二值化数组中
            for (Int32 x = 0; x < imgWidth; x++)
            {
                for (Int32 y = (imgHeight - 1); y > (imgHeight - verticalPoints[x] - 1); y--)
                {
                    verticalProArray[y, x] = 0;
                }
            }

            //将竖直投影的二值化数组转换为二值化图像并保存
            Bitmap verBmp = BinaryArrayToBinaryBitmap(verticalProArray);
            return verticalProArray;
            //verBmp.Save(imageDestPath, System.Drawing.Imaging.ImageFormat.Jpeg);

        }
        /// <summary>
        /// 将二值化数组转换为二值化图像
        /// </summary>
        /// <param name="binaryArray">二值化数组</param>
        /// <returns>二值化图像</returns>
        public static Bitmap BinaryArrayToBinaryBitmap(Byte[,] binaryArray)
        {   // 将二值化数组转换为二值化数据
            Int32 PixelHeight = binaryArray.GetLength(0);
            Int32 PixelWidth = binaryArray.GetLength(1);
            Int32 Stride = ((PixelWidth + 31) >> 5) << 2;
            Byte[] Pixels = new Byte[PixelHeight * Stride];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                Int32 Base = i * Stride;
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    if (binaryArray[i, j] != 0)
                    {
                        Pixels[Base + (j >> 3)] |= Convert.ToByte(0x80 >> (j & 0x7));
                    }
                }
            }

            // 创建黑白图像
            Bitmap BinaryBmp = new Bitmap(PixelWidth, PixelHeight, PixelFormat.Format1bppIndexed);

            // 设置调色表
            ColorPalette cp = BinaryBmp.Palette;
            cp.Entries[0] = Color.Black;    // 黑色
            cp.Entries[1] = Color.White;    // 白色
            BinaryBmp.Palette = cp;

            // 设置位图图像特性
            BitmapData BinaryBmpData = BinaryBmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            Marshal.Copy(Pixels, 0, BinaryBmpData.Scan0, Pixels.Length);
            BinaryBmp.UnlockBits(BinaryBmpData);

            return BinaryBmp;
        }
        /// <summary>
        /// 将二值化图像转为二维数组
        /// </summary>
        /// <param name="bmp">二值化后的图像</param>
        /// <returns></returns>
        public static Byte[,] ToBinaryArray(Bitmap bmp) 
        {
            Byte[,] BinaryArray = new Byte[bmp.Height, bmp.Width];
            for (Int32 i = 0; i < bmp.Height; i++)
            {
                for (Int32 j = 0; j < bmp.Width; j++)
                {
                    BinaryArray[i, j] = bmp.GetPixel(j, i).B;
                }
            }

            return BinaryArray;
        }

        /// <summary>
        /// 实现Sauvola算法实现图像二值化，最好的二值化算法之一,比我之前的阈值直接取平均数的算法好太多了---lFeng
        /// </summary>
        /// <param name="bin_image">用于存储二值化完成的图像</param>
        /// <param name="gray_image">用于存储等待二值化完成的灰度图像</param>
        public static Bitmap Sauvola(Bitmap grayBmp)
        {
            Byte[,] gray_image = PicUtils.ToBinaryArray(grayBmp);
            Byte[,] bin_image=new Byte[gray_image.GetLength(0),gray_image.GetLength(1)];
            int w = 40;
            double k = 0.3;
            int whalf = w >> 1;
            int MAXVAL = 256;

            int image_width = gray_image.GetLength(0);
            int image_height = gray_image.GetLength(1);


            int[,] integral_image = new int[image_width, image_height];
            int[,] integral_sqimg = new int[image_width, image_height];
            int[,] rowsum_image = new int[image_width, image_height];
            int[,] rowsum_sqimg = new int[image_width, image_height];


            int xmin, ymin, xmax, ymax;
            double diagsum, idiagsum, diff, sqdiagsum, sqidiagsum, sqdiff, area;
            double mean, std, threshold;

            for (int j = 0; j < image_height; j++)
            {
                rowsum_image[0, j] = gray_image[0, j];
                rowsum_sqimg[0, j] = gray_image[0, j] * gray_image[0, j];
            }
            for (int i = 1; i < image_width; i++)
            {
                for (int j = 0; j < image_height; j++)
                {
                    rowsum_image[i, j] = rowsum_image[i - 1, j] + gray_image[i, j];
                    rowsum_sqimg[i, j] = rowsum_sqimg[i - 1, j] + gray_image[i, j] * gray_image[i, j];
                }
            }

            for (int i = 0; i < image_width; i++)
            {
                integral_image[i, 0] = rowsum_image[i, 0];
                integral_sqimg[i, 0] = rowsum_sqimg[i, 0];
            }
            for (int i = 0; i < image_width; i++)
            {
                for (int j = 1; j < image_height; j++)
                {
                    integral_image[i, j] = integral_image[i, j - 1] + rowsum_image[i, j];
                    integral_sqimg[i, j] = integral_sqimg[i, j - 1] + rowsum_sqimg[i, j];
                }
            }

            //Calculate the mean and standard deviation using the integral image

            for (int i = 0; i < image_width; i++)
            {
                for (int j = 0; j < image_height; j++)
                {
                    xmin = Math.Max(0, i - whalf);
                    ymin = Math.Max(0, j - whalf);
                    xmax = Math.Min(image_width - 1, i + whalf);
                    ymax = Math.Min(image_height - 1, j + whalf);
                    area = (xmax - xmin + 1) * (ymax - ymin + 1);
                    // area can't be 0 here
                    // proof (assuming whalf >= 0):
                    // we'll prove that (xmax-xmin+1) > 0,
                    // (ymax-ymin+1) is analogous
                    // It's the same as to prove: xmax >= xmin
                    // image_width - 1 >= 0         since image_width > i >= 0
                    // i + whalf >= 0               since i >= 0, whalf >= 0
                    // i + whalf >= i - whalf       since whalf >= 0
                    // image_width - 1 >= i - whalf since image_width > i
                    // --IM
                    if (area <= 0)
                        throw new Exception("Binarize: area can't be 0 here");
                    if (xmin == 0 && ymin == 0)
                    { // Point at origin
                        diff = integral_image[xmax, ymax];
                        sqdiff = integral_sqimg[xmax, ymax];
                    }
                    else if (xmin == 0 && ymin > 0)
                    { // first column
                        diff = integral_image[xmax, ymax] - integral_image[xmax, ymin - 1];
                        sqdiff = integral_sqimg[xmax, ymax] - integral_sqimg[xmax, ymin - 1];
                    }
                    else if (xmin > 0 && ymin == 0)
                    { // first row
                        diff = integral_image[xmax, ymax] - integral_image[xmin - 1, ymax];
                        sqdiff = integral_sqimg[xmax, ymax] - integral_sqimg[xmin - 1, ymax];
                    }
                    else
                    { // rest of the image
                        diagsum = integral_image[xmax, ymax] + integral_image[xmin - 1, ymin - 1];
                        idiagsum = integral_image[xmax, ymin - 1] + integral_image[xmin - 1, ymax];
                        diff = diagsum - idiagsum;
                        sqdiagsum = integral_sqimg[xmax, ymax] + integral_sqimg[xmin - 1, ymin - 1];
                        sqidiagsum = integral_sqimg[xmax, ymin - 1] + integral_sqimg[xmin - 1, ymax];
                        sqdiff = sqdiagsum - sqidiagsum;
                    }

                    mean = diff / area;
                    std = Math.Sqrt((sqdiff - diff * diff / area) / (area - 1));
                    threshold = mean * (1 + k * ((std / 128) - 1));
                    if (gray_image[i, j] < threshold)
                        bin_image[i, j] = 0;
                    else
                        bin_image[i, j] = (byte)(MAXVAL - 1);
                }
            }
            Bitmap savolaBmp = PicUtils.BinaryArrayToBinaryBitmap(bin_image);
            return savolaBmp;
        }
        /// <summary>
        /// 根据起始滴落点完成滴水算法，实现粘粘图片的分割
        /// </summary>
        /// <param name="imageSrc">the path of src image</param>
        public static Bitmap DropFall(Bitmap bmp)
        {



            Byte[,] grayArray = ToBinaryArray(bmp);

            int imageHeight = grayArray.GetLength(0);

            int imageWidth = grayArray.GetLength(1);

            //用于存储分割后的图像的二值化数组
            Byte[,] dividedArray = new Byte[imageHeight, imageWidth];

            for (Int32 x = 0; x < imageHeight; x++)
            {
                for (Int32 y = 0; y < imageWidth; y++)
                {
                    dividedArray[x, y] = 255;
                }
            }

            //记录图像出现的上下极限位置
            int xIndex1 = 0, xIndex2 = 0;

            //记录图像出现的左右极限位置
            int yIndex1 = 0, yIndex2 = 0;

            byte loopFlg = 0;//当该变量置为1时将跳出双重循环

            for (int x = 0; (x < imageHeight) && (0 == loopFlg); x++)
            {
                for (int y = 0; (y < imageWidth) && (0 == loopFlg); y++)
                {
                    if (0 == grayArray[x, y])//从边界开始遇到第一个黑色像素点时，此处为图像出现的在纵轴上的第一个点
                    {
                        xIndex1 = x;
                        loopFlg = 1;
                    }
                }
            }

            loopFlg = 0;//当该变量置为零时将跳出双重循环

            for (int x = (imageHeight - 1); (x > 0) && (0 == loopFlg); x--)
            {
                for (int y = 0; (y < imageWidth) && (0 == loopFlg); y++)
                {
                    if (0 == grayArray[x, y])//从边界开始遇到第一个黑色像素点时，此处为图像出现的在纵轴上的第一个点
                    {
                        xIndex2 = x;
                        loopFlg = 1;
                    }
                }
            }

            loopFlg = 0;//当该变量置为零时将跳出双重循环

            for (int y = 0; (y < imageWidth) && (0 == loopFlg); y++)
            {
                for (int x = 0; (x < imageHeight) && (0 == loopFlg); x++)
                {
                    if (0 == grayArray[x, y])//从边界开始遇到第一个黑色像素点时，此处为图像出现的在纵轴上的第一个点
                    {
                        yIndex1 = y;
                        loopFlg = 1;
                    }
                }
            }

            loopFlg = 0;//当该变量置为零时将跳出双重循环

            for (int y = (imageWidth - 1); (y > 0) && (0 == loopFlg); y--)
            {
                for (int x = 0; (x < imageHeight) && (0 == loopFlg); x++)
                {
                    if (0 == grayArray[x, y])//从边界开始遇到第一个黑色像素点时，此处为图像出现的在纵轴上的第一个点
                    {
                        yIndex2 = y;
                        loopFlg = 1;
                    }
                }
            }
            //有字符区域的宽度
            int validWidth = yIndex2 - yIndex1;
            //有字符区域的高度
            int validHeight = xIndex2 - xIndex1;

            int addVal = validWidth >> 2;
            //临时起始滴落点，该滴落点确保能将示例中的第一个字符分割开来  
            int iniPointY = yIndex1 + addVal;

            int iniPointX = xIndex1;

            byte leftRightFlg = 0;//该标志位置位时，表示情况5与情况6可能会循环出现

            while ((iniPointX < xIndex2) && (iniPointY < yIndex2))
            {
                Byte pointLeft = grayArray[iniPointX, (iniPointY - 1)];
                Byte pointRight = grayArray[iniPointX, (iniPointY + 1)];
                Byte pointDown = grayArray[(iniPointX + 1), iniPointY];
                Byte pointDownLeft = grayArray[(iniPointX + 1), (iniPointY - 1)];
                Byte pointDownRight = grayArray[(iniPointX + 1), (iniPointY + 1)];

                for (int y = yIndex1; y < iniPointY; y++)
                {
                    dividedArray[iniPointX, (y - yIndex1)] = grayArray[iniPointX, y];
                }

                if (((0 == pointLeft) && (0 == pointRight) && (0 == pointDown) && (0 == pointDownLeft) && ((0 == pointDownRight)))// all black
                || ((255 == pointLeft) && (255 == pointRight) && (255 == pointDown) && (255 == pointDownLeft) && ((255 == pointDownRight))//all white
                || ((0 == pointDownLeft) && (255 == pointDown))))//情况1与情况3
                {//down
                    iniPointX = iniPointX + 1;
                    leftRightFlg = 0;
                }
                else if ((0 == pointLeft) && (0 == pointRight) && (0 == pointDown) && (255 == pointDownLeft) && (0 == pointDownRight))
                {//left down //情况2
                    iniPointX = iniPointX + 1;
                    iniPointY = iniPointY - 1;

                    leftRightFlg = 0;
                }
                else if ((0 == pointDown) && (0 == pointDownLeft) && (255 == pointDownRight))
                {//情况4
                    iniPointX = iniPointX + 1;
                    iniPointY = iniPointY + 1;

                    leftRightFlg = 0;
                }
                else if ((255 == pointRight) && (0 == pointDown) && (0 == pointDownLeft) && (0 == pointDownRight))
                {//情况5
                    if (0 == leftRightFlg)
                    {
                        iniPointY = iniPointY + 1;

                        leftRightFlg = 1;

                    }
                    else//标志位为1时说明上一个点出现在情况6，而本次循环的点出现在情况5，此种情况将垂直渗透
                    {
                        iniPointX = iniPointX + 1;

                        leftRightFlg = 0;
                    }
                }
                else if ((255 == pointLeft) && (0 == pointDown) && (0 == pointDownLeft) && (0 == pointDownRight))
                {//情况6
                    if (0 == leftRightFlg)
                    {
                        iniPointY = iniPointY - 1;

                        leftRightFlg = 1;
                    }
                    else//标志位为1时说明上一个点出现在情况5，而本次循环的点出现在情况6，此种情况将垂直渗透
                    {
                        iniPointX = iniPointX + 1;

                        leftRightFlg = 0;
                    }
                }
                else
                {
                    iniPointX = iniPointX + 1;
                }
            }
            Bitmap bmpDst = BinaryArrayToBinaryBitmap(dividedArray);
            return bmpDst;
            //bmpDst.Save(imageDestPath, System.Drawing.Imaging.ImageFormat.Jpeg);

        }

        public static List<Bitmap> SimpleCutTo4(Bitmap Binarybmp) 
        {
            //存放分割后的字符的列表，作为返回值
            List<Bitmap> littleBmpList = new List<Bitmap>();
            float xWidth = Binarybmp.Width / 4;
            int imgHeight = Binarybmp.Height;
            for (int countX = 0; countX < 4; countX++)
            {
                RectangleF cloneRect = new RectangleF(countX * xWidth, 0, xWidth, imgHeight);
                Bitmap kidbmp = Binarybmp.Clone(cloneRect, PixelFormat.Format24bppRgb);
                littleBmpList.Add(kidbmp);
            }

            return littleBmpList;
        }
    

    }



    public class MyUtils 
    {
        /// <summary>
        /// 比较二个字符串，查找出相同字数和差异字符
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static int strCompare(string s1, string s2)
        {
            int count = 0;/*相同字符个数*/
            int n = s1.Length > s2.Length ? s2.Length : s1.Length;/*获得较短的字符串的长度*/
            for (int i = 0; i < n; i++)
            {
                if (s1.Substring(i, 1) == s2.Substring(i, 1))
                /*同位置字符是否相同*/
                {
                    count++;
                }
                else
                {
                    //MessageBox.Show("s1:" + s1.Substring(i, 1) + "| s2:" + s2.Substring(i, 1));
                }
            }
            return count;
        }
        public static Bitmap MyGussianBlur(Bitmap binaryBmp) 
        {
            Image<Hsv, Byte> Iimage = new Image<Hsv, byte>(binaryBmp);
            //Image<Hsv, Byte> HsvImage = Iimage.Resize(32, 32);
            //littleBmpArray[lba] = HsvImage.ToBitmap();
            //参数必须为奇数
            return Iimage.SmoothMedian(5).ToBitmap();
        }
        /// <summary>
        /// 灰度化处理图像
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }
        /// <summary>
        ///  中值滤波法去除噪点   去掉杂点（适合杂点/杂线粗为1）
        /// </summary>
        /// <param name="dgGrayValue">背前景灰色界限</param>
        /// <returns></returns>
        public static  void ClearNoise(Bitmap bmpobj, int dgGrayValue, int MaxNearPoints)
        {
            Color piexl;
            int nearDots = 0;
            //逐点判断
            for (int i = 0; i < bmpobj.Width; i++)
                for (int j = 0; j < bmpobj.Height; j++)
                {
                    piexl = bmpobj.GetPixel(i, j);
                    if (piexl.R < dgGrayValue)
                    {
                        nearDots = 0;
                        //判断周围8个点是否全为空
                        if (i == 0 || i == bmpobj.Width - 1 || j == 0 || j == bmpobj.Height - 1)  //边框全去掉
                        {
                            bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        }
                        else
                        {
                            if (bmpobj.GetPixel(i - 1, j - 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i, j - 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i + 1, j - 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i - 1, j).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i + 1, j).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i - 1, j + 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i, j + 1).R < dgGrayValue) nearDots++;
                            if (bmpobj.GetPixel(i + 1, j + 1).R < dgGrayValue) nearDots++;
                        }

                        if (nearDots < MaxNearPoints)
                            bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));   //去掉单点 && 粗细小3邻边点
                    }
                    else  //背景
                        bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
        }


        /// <summary>
        /// 得到灰度图像前景背景的临界值，用于图像去躁算法
        /// </summary>
        /// <param name="bmpobj"></param>
        /// <returns></returns>
        public static int GetDgGrayValue(Bitmap bmpobj)
        {
            int[] pixelNum = new int[256];
            int n, n1, n2;
            int total;
            double m1, m2, sum, csum, fmax, sb;
            int k, t, q;
            int threshValue = 1;
            //int step=1;
            //生成直方图
            for (int i = 0; i < bmpobj.Width; i++)
            {
                for (int j = 0; j < bmpobj.Height; j++)
                {
                    pixelNum[bmpobj.GetPixel(i, j).R]++;
                }
            }
            for (k = 0; k <= 255; k++)
            {
                total = 0;
                for (t = -2; t <= 2; t++)
                {
                    q = k + t;
                    if (q < 0)
                        q = 0;
                    if (q > 255)
                        q = 255;
                    total = total + pixelNum[q];
                }
                pixelNum[k] = (int)((float)total / 5.0 + 0.5);
            }
            sum = csum = 0.0;
            n = 0;
            for (k = 0; k < 255; k++)
            {
                sum += (double)k * (double)pixelNum[k];
                n += pixelNum[k];
            }
            fmax = -1.0;
            n1 = 0;
            for (k = 0; k < 256; k++)
            {
                n1 += pixelNum[k];
                if (n1 == 0) { continue; }
                n2 = n - n1;
                if (n2 == 0) { break; }
                csum += (double)k * pixelNum[k];
                m1 = csum / n1;
                m2 = (sum - csum) / n2;
                sb = (double)n1 * (double)n2 * (m1 - m2) * (m1 - m2);
                if (sb > fmax)
                {
                    fmax = sb;
                    threshValue = k;
                }
            }
            return threshValue;
        }
    }






    /// <summary>
    /// 通常我们关注的字符并不会填满整张图片，找到我们所关注字符的边界并将其记录。
    /// </summary>
    public class ImgBoundary
    {
        private int _heightMax = 0;

        public int heightMax
        {
            get { return _heightMax; }
            set { _heightMax = value; }
        }

        private int _heightMin = 0;
        public int heightMin
        {
            get { return _heightMin; }
            set { _heightMin = value; }
        }

        private int _widthMax = 0;
        public int widthMax
        {
            get { return _widthMax; }
            set { _widthMax = value; }
        }

        private int _widthMin = 0;
        public int widthMin
        {
            get { return _widthMin; }
            set { _widthMin = value; }
        }
    }

    /// <summary>
    /// 记录图片中顶部的谷点集合和底部的谷点集合
    /// </summary>
    public class ImgValley
    {
        private ArrayList _upValley;
        public ArrayList upValley
        {
            get { return _upValley; }
            set { _upValley = value; }
        }


        private ArrayList _downValley;
        public ArrayList downValley
        {
            get { return _downValley; }
            set { _downValley = value; }
        }
    }


    public class PixPos
    {
        private int _widthPos = 0;
        public int widthPos
        {
            get { return _widthPos; }
            set { _widthPos = value; }
        }

        private int _heightPos = 0;
        public int heightPos
        {
            get { return _heightPos; }
            set { _heightPos = value; }
        }
    }



    public class SegmentFunction
    {
        /// <summary>
        /// 将输入的含粘粘字符的图片转换为分割后的字符图片
        /// </summary>
        /// <param name="imageSrcPath">粘粘字符图片的路径</param>
        /// <param name="imageSrcPath">分割后图片的存储路径</param>
        /// <returns>字符分割后的图片为位图</returns>
        public static ArrayList ImageSegment(Bitmap bmp/*, string imageDestPath*/)
        {
            //int Threshold = 0;
            //二值化图像对应的垂直投影二维数组
            Byte[,] BinaryArray = PicUtils.ToBinaryArray(bmp);

            ImgBoundary Boundary = getImgBoundary(BinaryArray);

            ImgValley iniValley = getImgValley(BinaryArray);

            ImgValley filterValley = filterImgValley(iniValley, Boundary);

            ArrayList downValley = filterValley.downValley;



            //ArrayList segPathList = getSegmentPath(filterValley, Boundary, BinaryArray);

            //Bitmap segmentBmp = getDivideImg(BinaryArray, segPathList);

            //segmentBmp.Save(imageDestPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            return downValley;
        }

        /// <summary>
        /// 获取含粘粘字符图片的谷点
        /// </summary>
        /// <param name="BinaryArray">原始图片的二值化数组</param>
        /// <returns>谷点列表</returns>
        public static ImgValley getImgValley(Byte[,] BinaryArray)
        {

            int imageHeight = BinaryArray.GetLength(0);

            int imageWidth = BinaryArray.GetLength(1);

            ArrayList upValley = new ArrayList();

            ArrayList downValley = new ArrayList();

            ImgValley Valley = new ImgValley();

            Byte[,] m_DesImage = Thining.ThinPicture(BinaryArray);

            byte flg = 1;

            byte P0 = 0;
            byte P1 = 0;
            byte P2 = 0;
            byte P3 = 0;
            byte P4 = 0;
            byte P5 = 0;
            byte P6 = 0;
            byte P7 = 0;
            byte P8 = 0;
            byte P9 = 0;
            byte P10 = 0;
            byte P11 = 0;
            byte P12 = 0;
            byte P13 = 0;
            byte P14 = 0;

            #region
            //get the up valley point
            for (int j = 2; j < imageWidth - 2; j++)//我将j < imageWidth改为j < imageWidth - 2，下面同理--ml2018/01/26
            {
                flg = 1;
                for (int i = 2; ((i < imageHeight-2) && (1 == flg)); i++)
                {
                    if (0 == m_DesImage[i, j])
                    {
                        flg = 0;

                        P0 = m_DesImage[i - 2, j - 2];
                        P1 = m_DesImage[i - 2, j - 1];
                        P2 = m_DesImage[i - 2, j];
                        P3 = m_DesImage[i - 2, j + 1];
                        P4 = m_DesImage[i - 2, j + 2];
                        P5 = m_DesImage[i - 1, j - 2];
                        P6 = m_DesImage[i - 1, j - 1];
                        P7 = m_DesImage[i - 1, j];
                        P8 = m_DesImage[i - 1, j + 1];
                        P9 = m_DesImage[i - 1, j + 2];
                        P10 = m_DesImage[i, j - 2];
                        P11 = m_DesImage[i, j - 1];
                        P12 = m_DesImage[i, j];
                        P13 = m_DesImage[i, j + 1];
                        P14 = m_DesImage[i, j + 2];

                        //共同特征:2/3/6/7/8/12/13
                        if ((255 == P2) && (255 == P3) && (255 == P7) && (255 == P8) && (0 == P12) && (0 == P13) && (0 == P6) && (
                                ((0 == P0) && (255 == P1)) || ((255 == P4) && (255 == P9) && (0 == P14))))
                        {
                            PixPos valleyPos = new PixPos();
                            valleyPos.widthPos = j;
                            valleyPos.heightPos = i;
                            upValley.Add(valleyPos);
                        }
                        //共同特征:1/2/6/7/8/12
                        if ((255 == P1) && (255 == P2) && (255 == P7) && (0 == P8) && (0 == P12) && (0 == P6) && (
                                (0 == P0) || ((255 == P3) && (0 == P4))))
                        {
                            PixPos valleyPos = new PixPos();
                            valleyPos.widthPos = j;
                            valleyPos.heightPos = i;
                            upValley.Add(valleyPos);
                        }

                        //共同特征:1/2/6/7/8/11/12
                        if ((255 == P1) && (255 == P2) && (255 == P7) && (0 == P8) && (255 == P6) && (0 == P11) && (0 == P12) && (
                                ((255 == P0) && (255 == P5) && (0 == P10)) || ((255 == P3) && (0 == P4))))
                        {
                            PixPos valleyPos = new PixPos();
                            valleyPos.widthPos = j;
                            valleyPos.heightPos = i;
                            upValley.Add(valleyPos);
                        }
                        break;
                    }
                }
            }
            #endregion

            #region
            //get the down valley point
            for (int j = imageWidth - 3; j > 2; j--)
            {
                flg = 1;
                for (int i = imageHeight - 3; ((i > 0) && (1 == flg)); i--)
                {
                    P0 = m_DesImage[i, j - 2];
                    P1 = m_DesImage[i, j - 1];
                    P2 = m_DesImage[i, j];
                    P3 = m_DesImage[i, j + 1];
                    P4 = m_DesImage[i, j + 2];

                    P5 = m_DesImage[i + 1, j - 2];
                    P6 = m_DesImage[i + 1, j - 1];
                    P7 = m_DesImage[i + 1, j];
                    P8 = m_DesImage[i + 1, j + 1];
                    P9 = m_DesImage[i + 1, j + 2];

                    P10 = m_DesImage[i + 2, j - 2];
                    P11 = m_DesImage[i + 2, j - 1];
                    P12 = m_DesImage[i + 2, j];
                    P13 = m_DesImage[i + 2, j + 1];
                    P14 = m_DesImage[i + 2, j + 2];

                    if (0 == m_DesImage[i, j])
                    {
                        flg = 0;

                        //共同特征:2/3/7/8/11/12/13
                        if ((0 == P2) && (0 == P3) && (255 == P7) && (255 == P8) && (255 == P11) && (255 == P12) && (255 == P13) && (
                                ((0 == P6) && (0 == P10)) || ((0 == P1) && (0 == P5) && (255 == P6) && (255 == P10))))
                        {
                            PixPos valleyPos = new PixPos();
                            valleyPos.widthPos = j;
                            valleyPos.heightPos = i;
                            downValley.Add(valleyPos);
                        }
                        //共同特征:2/6/7/8/11/12/13
                        if ((0 == P2) && (0 == P6) && (255 == P7) && (0 == P8) && (255 == P11) && (255 == P12) && (255 == P13) && (
                                (0 == P14) || (0 == P10)))
                        {
                            PixPos valleyPos = new PixPos();
                            valleyPos.widthPos = j;
                            valleyPos.heightPos = i;
                            downValley.Add(valleyPos);
                        }

                        //共同特征:1/2/6/7/8/11/12/13
                        if ((0 == P1) && (0 == P2) && (255 == P7) && (0 == P8) && (255 == P6) && (255 == P11) && (255 == P12) && (255 == P13) && (
                                ((0 == P0) && (255 == P5) && (255 == P10)) || (0 == P14)))
                        {
                            PixPos valleyPos = new PixPos();
                            valleyPos.widthPos = j;
                            valleyPos.heightPos = i;
                            downValley.Add(valleyPos);
                        }
                        break;
                    }
                }
            }
            #endregion

            Valley.upValley = upValley;
            Valley.downValley = downValley;

            return Valley;
        }

        /// <summary>
        ///在四字符的前提下对谷点进行过滤，过滤原则:
        ///1.与左右边界间隔距离小于1/8字符宽度的谷点淘汰；
        ///2.若两个谷点之间的宽度差距小于3个像素点，则靠左侧的像素点被淘汰
        /// </summary>
        /// <param name="valleyCollection">谷点列表集合</param>
        /// <param name="Boundary">图片中字符所在区域的边界</param>
        /// <returns>过滤后的谷点列表</returns>

        public static ImgValley filterImgValley(ImgValley valleyCollection, ImgBoundary Boundary)
        {
            ArrayList filterUpValley = valleyCollection.upValley;
            ArrayList filterDownValley = valleyCollection.downValley;

            int upCount = filterUpValley.Count;
            int downCount = filterDownValley.Count;

            int leftWidth = Boundary.widthMin;
            int rightWidth = Boundary.widthMax;
            int filterWidth = (rightWidth - leftWidth) >> 3;//右移3位相当于除以4，因为有四个字符--ml--×

            int tmpWidth0 = 0;
            int tmpWidth1 = 0;

            // ArrayList removeList = new ArrayList();

            PixPos pos0 = new PixPos();
            PixPos pos1 = new PixPos();

            for (int i = 0; i < upCount; i++)
            {
                pos0 = (PixPos)filterUpValley[i];

                if (i < (upCount - 1))
                {
                    pos1 = (PixPos)filterUpValley[i + 1];
                }

                tmpWidth0 = pos0.widthPos;
                tmpWidth1 = pos1.widthPos;

                if ((System.Math.Abs(tmpWidth1 - tmpWidth0) < 3) || ((tmpWidth0 - leftWidth) < filterWidth)
                    || ((rightWidth - tmpWidth0) < filterWidth))
                {
                    filterUpValley.RemoveAt(i);
                    upCount--;
                }

            }

            
            for (int i = 0; i < downCount; i++)
            {
                pos0 = (PixPos)filterDownValley[i];

                if (i < (downCount - 1))
                {
                    pos1 = (PixPos)filterDownValley[i + 1];
                }
 
                tmpWidth0 = pos0.widthPos;
                tmpWidth1 = pos1.widthPos;
                //这里的15是自己定的--------ml 
                if ((System.Math.Abs(tmpWidth1 - tmpWidth0) < 15) || ((tmpWidth0 - leftWidth) < filterWidth)
                    || ((rightWidth - tmpWidth0) < filterWidth))
                {
                   // removeList.Add(i);
                    filterDownValley.RemoveAt(i);
                    downCount--;
                }
            }
            
            filterDownValley.TrimToSize();

            ImgValley filterValley = new ImgValley();
            filterValley.upValley = filterUpValley;
            filterValley.downValley = filterDownValley;

            return filterValley;
        }

        /// <summary>
        ///根据谷点获取图片的分割路径
        /// </summary>
        /// <param name="valleyCollection">谷点列表</param>
        /// <param name="Boundary">图片中字符所在区域的边界</param>
        /// <param name="BinaryArray">二值化图片数组</param>
        /// <returns>返回粘粘图片的分割路径</returns>
        public static ArrayList getSegmentPath(ImgValley valleyCollection, ImgBoundary Boundary, Byte[,] BinaryArray)
        {
            ArrayList segPathList = new ArrayList();

            int yIndex2 = Boundary.widthMax;

            int yIndex1 = Boundary.widthMin;

            int xIndex2 = Boundary.heightMax;

            int xIndex1 = Boundary.heightMin;

            int valleyNum = valleyCollection.upValley.Count;

            //将图像最左侧的点所在直线作为第一条分割路径
            // int validHeight = xIndex2 - xIndex1;

            ArrayList pathList0 = new ArrayList();

            for (int a = xIndex1; a < xIndex2; a++)
            {
                PixPos newPos = new PixPos();

                newPos.widthPos = yIndex1;
                newPos.heightPos = a;

                pathList0.Add(newPos);

            }

            segPathList.Add(pathList0);
            
            #region
            for (int i = 0; i < valleyNum; i++)
            {
                PixPos pos = (PixPos)valleyCollection.upValley[i];

                ArrayList pathList = new ArrayList();

                int iniPointY = pos.widthPos;

                int iniPointX = xIndex1;//任一分割路径的高度起始点均为有效字符出现的第一个点

                byte leftRightFlg = 0;//该标志位置位时，表示情况5与情况6可能会循环出现

                while ((iniPointX < xIndex2) && (iniPointY < yIndex2))
                {
                    Byte pointLeft = BinaryArray[iniPointX, (iniPointY - 1)];
                    Byte pointRight = BinaryArray[iniPointX, (iniPointY + 1)];
                    Byte pointDown = BinaryArray[(iniPointX + 1), iniPointY];
                    Byte pointDownLeft = BinaryArray[(iniPointX + 1), (iniPointY - 1)];
                    Byte pointDownRight = BinaryArray[(iniPointX + 1), (iniPointY + 1)];

                    PixPos newPos = new PixPos();

                    newPos.widthPos = iniPointY;
                    newPos.heightPos = iniPointX;

                    pathList.Add(newPos);

                    if (((0 == pointLeft) && (0 == pointRight) && (0 == pointDown) && (0 == pointDownLeft) && ((0 == pointDownRight)))// all black
                    || ((255 == pointLeft) && (255 == pointRight) && (255 == pointDown) && (255 == pointDownLeft) && ((255 == pointDownRight))//all white
                    || ((0 == pointDownLeft) && (255 == pointDown))))//情况1与情况3
                    {//down
                        iniPointX = iniPointX + 1;
                        leftRightFlg = 0;
                    }
                    else if ((0 == pointLeft) && (0 == pointRight) && (0 == pointDown) && (255 == pointDownLeft) && (0 == pointDownRight))
                    {//left down //情况2
                        iniPointX = iniPointX + 1;
                        iniPointY = iniPointY - 1;

                        leftRightFlg = 0;
                    }
                    else if ((0 == pointDown) && (0 == pointDownLeft) && (255 == pointDownRight))
                    {//情况4
                        iniPointX = iniPointX + 1;
                        iniPointY = iniPointY + 1;

                        leftRightFlg = 0;
                    }
                    else if ((255 == pointRight) && (0 == pointDown) && (0 == pointDownLeft) && (0 == pointDownRight))
                    {//情况5
                        if (0 == leftRightFlg)
                        {
                            iniPointY = iniPointY + 1;

                            leftRightFlg = 1;

                        }
                        else//标志位为1时说明上一个点出现在情况6，而本次循环的点出现在情况5，此种情况将垂直渗透
                        {
                            iniPointX = iniPointX + 1;

                            leftRightFlg = 0;
                        }
                    }
                    else if ((255 == pointLeft) && (0 == pointDown) && (0 == pointDownLeft) && (0 == pointDownRight))
                    {//情况6
                        if (0 == leftRightFlg)
                        {
                            iniPointY = iniPointY - 1;

                            leftRightFlg = 1;
                        }
                        else//标志位为1时说明上一个点出现在情况5，而本次循环的点出现在情况6，此种情况将垂直渗透
                        {
                            iniPointX = iniPointX + 1;

                            leftRightFlg = 0;
                        }
                    }
                    else
                    {
                        iniPointX = iniPointX + 1;
                    }
                }

                segPathList.Add(pathList);
            }
            #endregion

            //将图像最右侧的点所在直线作为最后一条分割路径
            ArrayList pathListLast = new ArrayList();

            for (int a = xIndex1; a < xIndex2; a++)
            {
                PixPos newPos = new PixPos();

                newPos.widthPos = yIndex2;
                newPos.heightPos = a;

                pathListLast.Add(newPos);

            }

            segPathList.Add(pathListLast);

            return segPathList;
        }

        /// <summary>
        ///根据分割路径分割粘粘字符并另存为位图
        /// </summary>
        /// <param name="valleyCollection">谷点列表</param>
        /// <param name="Boundary">图片中字符所在区域的边界</param>
        /// <param name="BinaryArray">二值化图片数组</param>
        /// <returns>返回粘粘图片的分割路径</returns>
        public static Bitmap getDivideImg(Byte[,] BinaryArray, ArrayList SegPathList)
        {
            int imageHeight = BinaryArray.GetLength(0);

            int imageWidth = BinaryArray.GetLength(1);

            int segCount = SegPathList.Count;

            int locationVal = imageWidth / (segCount - 1);//问题出现在这里，imageWidth为200太大了，导致locationVal太大，。。。。

            int locationPos = 0;

            Byte[,] divideArray = new Byte[imageHeight, imageWidth];

            for (Int32 x = 0; x < imageHeight; x++)
            {
                for (Int32 y = 0; y < imageWidth; y++)
                {
                    divideArray[x, y] = 255;
                }
            }

            PixPos divPosRight = new PixPos();

            PixPos divPosLeft = new PixPos();

            ArrayList pathListLeft = new ArrayList();
            ArrayList pathListRight = new ArrayList();

            int indexWidthRight = 0;
            int indexWidthLeft = 0;

            int indexHeightRight = 0;
            int indexHeightLeft = 0;

            int pointIndexLeft = 0;
            int pointIndexRight = 0;

            int posCountLeft = 0;
            int posCountRight = 0;

            for (int i = 0; i < segCount - 1; i++)
            {
                //每条分割路径的起始点与终点均相同，且不会出现同一个高度值对应两个宽度值的状况，应此每条路径的点数均相同
                pathListLeft = (ArrayList)SegPathList[i];
                pathListRight = (ArrayList)SegPathList[i + 1];

                posCountLeft = pathListLeft.Count;
                posCountRight = pathListRight.Count;

                /*
                int posCount = 0;
 
                if (posCountLeft < posCountRight)
                {
                    posCount = posCountLeft;
                }
                else
                {
                    posCount = posCountRight;
                }
                */

                locationPos = 5 + locationVal * i;

                pointIndexLeft = 0;
                pointIndexRight = 0;

                //目前所用的滴水算法下，同一高度值最多同时对应两个宽度值
                while ((pointIndexLeft < posCountLeft) && (pointIndexRight < posCountRight))
                {
                    divPosRight = (PixPos)pathListRight[pointIndexRight];
                    divPosLeft = (PixPos)pathListLeft[pointIndexLeft];

                    pointIndexLeft++;
                    pointIndexRight++;

                    indexWidthLeft = divPosLeft.widthPos;
                    indexWidthRight = divPosRight.widthPos;

                    indexHeightRight = divPosLeft.heightPos;
                    indexHeightLeft = divPosRight.heightPos;
                    //indexHeightRight = divPosRight.heightPos;
                    //indexHeightLeft = divPosLeft.heightPos;

                    if (pointIndexLeft < posCountLeft)
                    {
                        divPosLeft = (PixPos)pathListLeft[pointIndexLeft];

                        if (indexHeightLeft == divPosLeft.heightPos)//若下一点高度值相同，索引值加1，宽度值更新
                        {
                            pointIndexLeft++;
                            indexWidthLeft = divPosLeft.widthPos;
                        }
                    }

                    if (pointIndexRight < posCountRight)
                    {
                        divPosRight = (PixPos)pathListRight[pointIndexRight];
                        if (indexHeightRight == divPosRight.heightPos)//若下一点高度值相同，索引值加1，宽度值更新
                        {
                            pointIndexRight++;
                            indexWidthRight = divPosRight.widthPos;
                        }
                    }



                    for (Int32 y = indexWidthLeft; y < indexWidthRight; y++)
                    {
                        //索引超出了数组界限。
                        divideArray[indexHeightRight, (y - indexWidthLeft + locationPos)] = BinaryArray[indexHeightRight, y];
                    }
                }

            }

            Bitmap GrayBmp = PicUtils.BinaryArrayToBinaryBitmap(divideArray);

            return GrayBmp;
        }


        /// <summary>
        ///获取字符所在区域的边界
        /// </summary>
        /// <param name="BinaryArray">二值化图片数组</param>
        /// <returns>返回字符所在区域的边界</returns>
        public static ImgBoundary getImgBoundary(Byte[,] BinaryArray)
        {
            ImgBoundary boundary = new ImgBoundary();

            int imageHeight = BinaryArray.GetLength(0);

            int imageWidth = BinaryArray.GetLength(1);

            //记录图像出现的左右极限位置
            int widthLeft = 0, widthRight = 0;

            //记录图像出现的上下极限位置
            int heightUp = 0, heightDown = 0;

            byte loopFlg = 0;//当该变量置为1时将跳出双重循环

            //外层循环从原点开始沿宽度方向值图像右侧
            for (int x = 0; (x < imageWidth) && (0 == loopFlg); x++)
            {
                //内层循环从原点开始沿高度方向值图像下方
                for (int y = 0; (y < imageHeight) && (0 == loopFlg); y++)
                {
                    //遇到的第一个点为最接近原点宽度方向上的第一个点，左侧极限点
                    if (0 == BinaryArray[y, x])
                    {
                        widthLeft = x;
                        loopFlg = 1;
                    }
                }
            }

            loopFlg = 0;//当该变量置为零时将跳出双重循环

            //外层循环从图像右侧沿宽度方向到原点
            for (int x = (imageWidth - 1); (x > 0) && (0 == loopFlg); x--)
            {
                //内层循环从原点开始沿高度方向到图像下方
                for (int y = 0; (y < imageHeight) && (0 == loopFlg); y++)
                {
                    //遇到的第一个点离原点最原的右侧极限点
                    if (0 == BinaryArray[y, x])
                    {
                        widthRight = x;
                        loopFlg = 1;
                    }
                }
            }

            loopFlg = 0;//当该变量置为零时将跳出双重循环

            //外层循环从图像下方开始向上接近原点方向，高度方向
            for (int x = (imageHeight - 1); (x > 0) && (0 == loopFlg); x--)
            {
                //内层循环从原点开始，沿宽度方向
                for (int y = 0; (y < imageWidth) && (0 == loopFlg); y++)
                {
                    //遇到的第一个点为高度方向离原点最远的点，下方极限点
                    if (0 == BinaryArray[x, y])
                    {
                        heightDown = x;
                        loopFlg = 1;
                    }
                }
            }

            loopFlg = 0;//当该变量置为零时将跳出双重循环

            //外层循环从原点开始沿高度方向
            for (int x = 0; (x < imageHeight) && (0 == loopFlg); x++)
            {
                //内层循环从原点开始沿高度方向
                for (int y = 0; (y < imageWidth) && (0 == loopFlg); y++)
                {
                    if (0 == BinaryArray[x, y])//从边界开始遇到第一个黑色像素点时，此处为图像出现的在纵轴上的第一个点
                    {
                        heightUp = x;
                        loopFlg = 1;
                    }
                }
            }



            boundary.widthMin = widthLeft;
            boundary.widthMax = widthRight;

            boundary.heightMin = heightUp;
            boundary.heightMax = heightDown;

            return boundary;
        }
    }
    public static class Thining
    {
        //调用此函数即可实现提取图像骨架
        //public static void getThinPicture(string imageSrcPath, string imageDestPath)
        //{
        //    Bitmap bmp = new Bitmap(imageSrcPath);

        //    int Threshold = 0;

        //    Byte[,] m_SourceImage = ToBinaryArray(bmp, out Threshold);

        //    Byte[,] m_DesImage = Thining.ThinPicture(m_SourceImage);

        //    Bitmap bmpThin = PicUtils.BinaryArrayToBinaryBitmap(m_DesImage);

        //    bmpThin.Save(imageDestPath, System.Drawing.Imaging.ImageFormat.Jpeg);
        //}

        public static int B(Byte[,] picture, int x, int y)
        {
            return picture[x, y - 1] + picture[x + 1, y - 1] + picture[x + 1, y] + picture[x + 1, y + 1] +
                   picture[x, y + 1] + picture[x - 1, y + 1] + picture[x - 1, y] + picture[x - 1, y - 1];
        }

        public static int A(Byte[,] picture, int x, int y)
        {
            int counter = 0;
            if ((picture[x, y - 1] == 0) && (picture[x + 1, y - 1] == 1))
            {
                counter++;
            }
            if ((picture[x + 1, y - 1] == 0) && (picture[x + 1, y] == 1))
            {
                counter++;
            }
            if ((picture[x + 1, y] == 0) && (picture[x + 1, y + 1] == 1))
            {
                counter++;
            }
            if ((picture[x + 1, y + 1] == 0) && (picture[x, y + 1] == 1))
            {
                counter++;
            }
            if ((picture[x, y + 1] == 0) && (picture[x - 1, y + 1] == 1))
            {
                counter++;
            }
            if ((picture[x - 1, y + 1] == 0) && (picture[x - 1, y] == 1))
            {
                counter++;
            }
            if ((picture[x - 1, y] == 0) && (picture[x - 1, y - 1] == 1))
            {
                counter++;
            }
            if ((picture[x - 1, y - 1] == 0) && (picture[x, y - 1] == 1))
            {
                counter++;
            }
            return counter;
        }



        public static Byte[,] ThinPicture(Byte[,] newPicture)
        {

            Byte[,] picture = new Byte[newPicture.GetLength(0) + 2, newPicture.GetLength(1) + 2];
            Byte[,] pictureToRemove = new Byte[newPicture.GetLength(0) + 2, newPicture.GetLength(1) + 2];
            bool hasChanged;
            for (int i = 0; i < picture.GetLength(1); i++)
            {
                for (int j = 0; j < picture.GetLength(0); j++)
                {
                    picture[j, i] = 255;
                    pictureToRemove[j, i] = 0;
                }
            }

            for (int i = 0; i < newPicture.GetLength(1); i++)
            {
                for (int j = 0; j < newPicture.GetLength(0); j++)
                {
                    picture[j + 1, i + 1] = newPicture[j, i];
                }
            }

            for (int i = 0; i < picture.GetLength(1); i++)
            {
                for (int j = 0; j < picture.GetLength(0); j++)
                {
                    picture[j, i] = picture[j, i] == 0 ? picture[j, i] = 1 : picture[j, i] = 0;
                }
            }
            do
            {
                hasChanged = false;
                for (int i = 0; i < newPicture.GetLength(1); i++)
                {
                    for (int j = 0; j < newPicture.GetLength(0); j++)
                    {
                        if ((picture[j, i] == 1) && (2 <= B(picture, j, i)) && (B(picture, j, i) <= 6) && (A(picture, j, i) == 1) &&
                            (picture[j, i - 1] * picture[j + 1, i] * picture[j, i + 1] == 0) &&
                            (picture[j + 1, i] * picture[j, i + 1] * picture[j - 1, i] == 0))
                        {
                            pictureToRemove[j, i] = 1;
                            hasChanged = true;
                        }
                    }
                }
                for (int i = 0; i < newPicture.GetLength(1); i++)
                {
                    for (int j = 0; j < newPicture.GetLength(0); j++)
                    {
                        if (pictureToRemove[j, i] == 1)
                        {
                            picture[j, i] = 0;
                            pictureToRemove[j, i] = 0;
                        }
                    }
                }
                for (int i = 0; i < newPicture.GetLength(1); i++)
                {
                    for (int j = 0; j < newPicture.GetLength(0); j++)
                    {
                        if ((picture[j, i] == 1) && (2 <= B(picture, j, i)) && (B(picture, j, i) <= 6) &&
                            (A(picture, j, i) == 1) &&
                            (picture[j, i - 1] * picture[j + 1, i] * picture[j - 1, i] == 0) &&
                            (picture[j, i - 1] * picture[j, i + 1] * picture[j - 1, i] == 0))
                        {
                            pictureToRemove[j, i] = 1;
                            hasChanged = true;
                        }
                    }
                }

                for (int i = 0; i < newPicture.GetLength(1); i++)
                {
                    for (int j = 0; j < newPicture.GetLength(0); j++)
                    {
                        if (pictureToRemove[j, i] == 1)
                        {
                            picture[j, i] = 0;
                            pictureToRemove[j, i] = 0;
                        }
                    }
                }
            } while (hasChanged);

            for (int i = 0; i < newPicture.GetLength(1); i++)
            {
                for (int j = 0; j < newPicture.GetLength(0); j++)
                {
                    if ((picture[j, i] == 1) &&
                        (((picture[j, i - 1] * picture[j + 1, i] == 1) && (picture[j - 1, i + 1] != 1)) || ((picture[j + 1, i] * picture[j, i + 1] == 1) && (picture[j - 1, i - 1] != 1)) ||      //Небольшая модификцаия алгоритма для ещё большего утоньшения
                        ((picture[j, i + 1] * picture[j - 1, i] == 1) && (picture[j + 1, i - 1] != 1)) || ((picture[j, i - 1] * picture[j - 1, i] == 1) && (picture[j + 1, i + 1] != 1))))
                    {
                        picture[j, i] = 0;
                    }
                }
            }

            for (int i = 0; i < picture.GetLength(1); i++)
            {
                for (int j = 0; j < picture.GetLength(0); j++)
                {
                    // picture[j, i] = picture[j, i] == 0 ? 255 : 0;      
                    if (0 == picture[j, i])
                    {
                        picture[j, i] = 255;
                    }
                    else
                    {
                        picture[j, i] = 0;
                    }
                }
            }

            Byte[,] outPicture = new Byte[newPicture.GetLength(0), newPicture.GetLength(1)];

            for (int i = 0; i < newPicture.GetLength(1); i++)
            {
                for (int j = 0; j < newPicture.GetLength(0); j++)
                {
                    outPicture[j, i] = picture[j + 1, i + 1];
                }
            }
            return outPicture;
        }

    }
}

