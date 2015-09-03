git add -Av .
set MSG=%1
if "%MSG%"=="" set MSG=Message
git commit -m %MSG%
git push origin HEAD
