using System;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV.ML;
using Emgu.CV.ML.Structure;

public class Class1
{
	public Class1()
	{
	}
    public void ANN()
    {
        int trainSampleCount = 100;

        #region Generate the traning data and classes
        Matrix<float> trainData = new Matrix<float>(trainSampleCount, 2);
        Matrix<float> trainClasses = new Matrix<float>(trainSampleCount, 1);

        Image<Bgr, Byte> img = new Image<Bgr, byte>(500, 500);

        Matrix<float> sample = new Matrix<float>(1, 2);
        Matrix<float> prediction = new Matrix<float>(1, 1);

        Matrix<float> trainData1 = trainData.GetRows(0, trainSampleCount >> 1, 1);
        trainData1.SetRandNormal(new MCvScalar(200), new MCvScalar(50));
        Matrix<float> trainData2 = trainData.GetRows(trainSampleCount >> 1, trainSampleCount, 1);
        trainData2.SetRandNormal(new MCvScalar(300), new MCvScalar(50));

        Matrix<float> trainClasses1 = trainClasses.GetRows(0, trainSampleCount >> 1, 1);
        trainClasses1.SetValue(1);
        Matrix<float> trainClasses2 = trainClasses.GetRows(trainSampleCount >> 1, trainSampleCount, 1);
        trainClasses2.SetValue(2);
        #endregion

        Matrix<int> layerSize = new Matrix<int>(new int[] { 2, 5, 1 });

        MCvANN_MLP_TrainParams parameters = new MCvANN_MLP_TrainParams();
        parameters.term_crit = new MCvTermCriteria(10, 1.0e-8);
        parameters.train_method = Emgu.CV.ML.MlEnum.ANN_MLP_TRAIN_METHOD.BACKPROP;
        parameters.bp_dw_scale = 0.1;
        parameters.bp_moment_scale = 0.1;

        using (ANN_MLP network = new ANN_MLP(layerSize, Emgu.CV.ML.MlEnum.ANN_MLP_ACTIVATION_FUNCTION.SIGMOID_SYM, 1.0, 1.0))
        {
            network.Train(trainData, trainClasses, null, null, parameters, Emgu.CV.ML.MlEnum.ANN_MLP_TRAINING_FLAG.DEFAULT);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    sample.Data[0, 0] = j;
                    sample.Data[0, 1] = i;
                    network.Predict(sample, prediction);

                    // estimates the response and get the neighbors' labels
                    float response = prediction.Data[0, 0];

                    // highlight the pixel depending on the accuracy (or confidence)
                    img[i, j] = response < 1.5 ? new Bgr(90, 0, 0) : new Bgr(0, 90, 0);
                }
            }
        }

        // display the original training samples
        for (int i = 0; i < (trainSampleCount >> 1); i++)
        {
            PointF p1 = new PointF(trainData1[i, 0], trainData1[i, 1]);
            img.Draw(new CircleF(p1, 2), new Bgr(255, 100, 100), -1);
            PointF p2 = new PointF((int)trainData2[i, 0], (int)trainData2[i, 1]);
            img.Draw(new CircleF(p2, 2), new Bgr(100, 255, 100), -1);
        }
        Emgu.CV.UI.ImageViewer.Show(img);
    }
    
    
}
