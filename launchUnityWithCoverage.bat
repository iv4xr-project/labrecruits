@rem Change directory to Unity (You need v2019.3 or higher to enable code coverage feature)
@rem Because AIGym project was developed for Unity 2019.2.6f1, you need to deny the "Asset Database Version Upgrade"
cd C:\Program Files\Unity 2019.4.32f1\Editor
@rem launch Unity with the desired project (-projectPath), 
@rem the customized method from "Assets/Editor/LaunchLabRecruits.cs" (-executeMethod), 
@rem and force the usage of code coverage feature (-enableCodeCoverage). Remember to install code coverage plugin
Unity.exe -projectPath "C:\Users\Fernando\Desktop\iv4xr\labrecruits\Unity\AIGym" -executeMethod LaunchLabRecruits.load -enableCodeCoverage