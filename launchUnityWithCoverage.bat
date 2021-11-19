@rem Change directory to Unity 2019.4.32f1 directory
cd C:\Program Files\Unity 2019.4.32f1\Editor
@rem launch Unity with the desired project (-projectPath), 
@rem the customized method from "Assets/Editor/LaunchLabRecruits.cs" (-executeMethod), 
@rem and force the usage of code coverage feature (-enableCodeCoverage).
Unity.exe -projectPath "C:\Users\Fernando\Desktop\iv4xr\labrecruits\Unity\AIGym" -executeMethod LaunchLabRecruits.load -enableCodeCoverage