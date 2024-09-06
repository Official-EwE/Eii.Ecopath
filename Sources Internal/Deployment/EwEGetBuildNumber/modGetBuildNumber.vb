Imports System.IO
Imports System.Xml

Module modGetBuildNumber

    Sub Main()

        Dim strPath As String = ""
        Dim strBuild As String = "0.0.x.0"
        Dim strRev As String = "0"
        Dim strSlikSVN As String = ""
        Dim xdoc As New XmlDocument()
        Dim xn As XmlNode = Nothing
        Dim log As New StreamWriter(Path.Combine(My.Application.Info.DirectoryPath, "log.txt"))

        log.WriteLine("Received " & System.Environment.CommandLine)

        If File.Exists("C:\Program Files\SlikSvn\bin\svn.exe") Then
            strSlikSVN = "C:\Program Files\SlikSvn\bin\svn.exe"
        Else
            strSlikSVN = "C:\Program Files (x86)\SlikSvn\bin\svn.exe"
        End If
        Try
            If (System.Environment.GetCommandLineArgs.Length >= 2) Then
                Dim strParm As String = System.Environment.GetCommandLineArgs()(1)
                Dim astrBits() As String = strParm.Split(","c)

                strBuild = astrBits(0).ToLower()
                If (astrBits.Length > 1) Then
                    strPath = astrBits(1).Replace("/", "\").Replace("\\", "\")
                End If

                ' Start the child process.
                Dim p As New Process()
                ' Redirect the output stream of the child process
                p.StartInfo.UseShellExecute = False
                p.StartInfo.RedirectStandardOutput = True
                p.StartInfo.FileName = strSlikSVN
                p.StartInfo.Arguments = "info --xml " & strPath
                p.Start()
                ' Do not wait for the child process to exit before reading to the end of its redirected stream.
                ' p.WaitForExit()
                ' Read the output stream first and then wait.
                xdoc.LoadXml(p.StandardOutput.ReadToEnd())
                p.WaitForExit()
                p.Dispose()
                p = Nothing
            Else
                ' xdoc.Load("test.xml")
                log.WriteLine("Cannot launch svn process")
                log.WriteLine("")
                log.WriteLine("Usage: EwEGetBuildNumber #.#.x.# {path to sources}")
                log.WriteLine("       sources path is optional")
                Console.Write("0.0.0.0")
                Return
            End If
        Catch ex As Exception
            log.WriteLine(ex.Message)
        End Try

        Try
            xn = xdoc.GetElementsByTagName("commit")(0)
            strRev = xn.Attributes("revision").InnerText
        Catch ex As Exception
            log.WriteLine(ex.Message)
        End Try

        If strBuild.IndexOf("x") > -1 Then
            strBuild = strBuild.Replace("x", strRev)
        Else
            strBuild = strBuild & "." & strRev
        End If
        Console.Write(strBuild)

        log.WriteLine("Build number " & strBuild)
        log.Close()

    End Sub

End Module
