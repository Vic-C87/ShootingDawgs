@echo off
git status
git pull origin main
git add *
set /p message=Write commit message:
git commit -m "%message%"
git push origin main
pause