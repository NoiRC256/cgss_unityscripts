CGSS Unity Scripts
========



JoinModel.cs
------------
CGSS人物模型默认头身分离，这个脚本可以使Head和Body在播放动画时同步姿态


LiveLookAt.cs
------------
视线追踪脚本，用于 Head hierarchy，需要 LookAtHandler.cs

原理是让 'Eye_L'，'Eye_R' 根据相机位置左右移动

需要配合 LookAtHandler.cs 使用

#### 食用方法： ####

* 使 'Eye_locator_L'，'Eye_locator_R' 作为 'Eye_L'，'Eye_R' 的父级，用来规定 'Eye_L'，'Eye_R' 的移动轨道

* 在 Head heiarchy 下新建 'CamLookAt'，变换为零，绑定到 LookAtHandler.cs，用于反应相机相对于 Head 的本地变换

* 绑定 'CamLookAt'，'Eye_L'，'Eye_R'

* 变量的默认数值适用于十倍大小的模型，可以根据需要自行调整数值

（74行，87行）：'Max Out'=视线向外偏移限制，'Max In'=视线向内偏移限制

另外两个 'eRelativeModDivisor'（72, 85行）和 'QuaternionDifferenceWeight'（73, 86行）是敏感度系数

LookAtHandler.cs
------------
视线追踪辅助脚本，用于 Head hierarchy，配合 LiveLookAt.cs

将相机的世界变换转换为 Head 本地变换

把相机和新建的 'CamLookAt' 绑上去就ok


LiveLookAt_v006.cs
------------
LiveLookAt.cs 重构版 目前无效
