@echo off 
rem �ر��Զ����
:begin

rem ��������
set username=chenhao-a
set pwd=6112538asd

set key=
set /p key=������key:

rem ����õ���������Ϣ

echo �������key�ǣ�%key%

F:\Projects\EventLogger\EventLogger\bin\Debug\EventLogger.exe -Mouse="F:\Projects\EventLogger\EventLogger\bin\Debug\һ��ǩ��_MouseDB.db" -Key="F:\Projects\EventLogger\EventLogger\bin\Debug\һ��ǩ��_KeyDB.db" -username=%username% -pwd=%pwd% -key=%key%
