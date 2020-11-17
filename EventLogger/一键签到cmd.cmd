@echo off 
rem 关闭自动输出
:begin

rem 接收输入
set username=chenhao-a
set pwd=6112538asd

set key=
set /p key=请输入key:

rem 输出得到的输入信息

echo 您输入的key是：%key%

F:\Projects\EventLogger\EventLogger\bin\Debug\EventLogger.exe -Mouse="F:\Projects\EventLogger\EventLogger\bin\Debug\一键签到_MouseDB.db" -Key="F:\Projects\EventLogger\EventLogger\bin\Debug\一键签到_KeyDB.db" -username=%username% -pwd=%pwd% -key=%key%
