echo start Copying
rmdir /s /q "\\142.103.47.27\Ecopath\Help"
xcopy "C:\Ecopath\Releases\Ecopath6\Sources\Documentation\Help" "\\142.103.47.27\Ecopath\Help\" /E
echo Finished Copying