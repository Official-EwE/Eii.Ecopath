echo Updating build version numbers
set buildNumber=6.1.0.%date:~4,2%%date:~7,2%
echo Publishing Build
del "\\142.103.43.3\Public\Ecopath\webfiles\EwE6\DailyBuilds\EwE%buildNumber%_setup.exe" /F
copy "\\142.103.43.3\Public\Ecopath\webfiles\EwE6\DailyBuilds\EwE.exe" "\\142.103.43.3\Public\Ecopath\webfiles\EwE6\DailyBuilds\EwE%buildNumber%_setup.exe"
del "\\142.103.43.3\Public\Ecopath\webfiles\EwE6\DailyBuilds\EwE.exe" /F