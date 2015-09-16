echo Committing assembly changes
set buildNumber=6.1.0.%date:~4,2%%date:~7,2%
cd C:\Ecopath\Releases\Ecopath6
svn commit -m "Build %buildNumber%" --username automatedbuild --password lenfest --quiet C:\Ecopath\Releases\Ecopath6\Sources
svn commit -m "Build %buildNumber%" --username automatedbuild --password lenfest --quiet C:\Ecopath\Releases\Ecopath6_Internal\Sources
