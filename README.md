# captchaReg
识别浪潮集团在中国大学生服务外包大赛上提供的第三类验证码，有60%正确率
开发工具 VS2015
语言 C#
# 
采用winform的形式实现了验证码识别系统，是本人的本科毕业设计。
## 系统设计流程图
![](./images/systemProcess.jpg)<br>
### 1）图像预处理
原图
![](./images/before.jpg)<br>
灰度化
![](./images/aftergray.jpg)<br>
二值化
![](./images/aftertwo.jpg)<br>
去噪
![](./images/afterquzao.jpg)<br>
### 2）字符分割
倾斜
![](./images/beforeDegree.jpg)<br>
倾斜纠正
![](./images/Degree.jpg)<br>
