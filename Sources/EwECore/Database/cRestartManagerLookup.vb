' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
'
' The single-file lock-detection technique below (QueryFilePids via
' NtQueryInformationFile, and QueryRmSession via the Restart Manager API)
' is a stripped-down port of the detection core from WHYBBE/WpfFileUnlocker
' (https://github.com/WHYBBE/WpfFileUnlocker), used under its MIT license.
' Folder scanning, process-kill functionality, and UI/localization code
' from that project are not included - this keeps only single-file
' friendly-name lookup, since that is all PublishLocalCopyToSource's error
' message needs.

Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Identifies which running process, if any, currently holds a given
    ''' file open, and resolves a friendly display name for it (e.g.
    ''' "Microsoft OneDrive" rather than "OneDrive.exe"). Used purely to
    ''' improve the wording of the "file is locked" message shown after
    ''' cEwEDatabase.PublishLocalCopyToSource exhausts its retry attempts -
    ''' see cCore.Save(). Never throws and never blocks correctness: any
    ''' failure here just means falling back to generic phrasing ("another
    ''' application"), never a reason to abort the save/retry flow itself.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Friend NotInheritable Class cRestartManagerLookup

        Private Sub New()
            ' Static-only class - not instantiable.
        End Sub

#Region " Win32 interop "

        Private Const ERROR_MORE_DATA As Integer = 234
        Private Shared ReadOnly INVALID_HANDLE_VALUE As IntPtr = New IntPtr(-1)

        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
        Private Structure RM_UNIQUE_PROCESS
            Public dwProcessId As Integer
            Public ProcessStartTime As System.Runtime.InteropServices.ComTypes.FILETIME
        End Structure

        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
        Private Structure RM_PROCESS_INFO
            Public Process As RM_UNIQUE_PROCESS
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)>
            Public strAppName As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public strServiceShortName As String
            Public ApplicationType As Integer
            Public AppStatus As UInteger
            Public TSSessionId As UInteger
            <MarshalAs(UnmanagedType.Bool)>
            Public bRestartable As Boolean
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Private Structure IO_STATUS_BLOCK
            Public Status As IntPtr
            Public Information As IntPtr
        End Structure

        <DllImport("rstrtmgr.dll", CharSet:=CharSet.Unicode)>
        Private Shared Function RmStartSession(ByRef pSessionHandle As Integer, dwSessionFlags As Integer, strSessionKey As String) As Integer
        End Function

        <DllImport("rstrtmgr.dll")>
        Private Shared Function RmEndSession(pSessionHandle As Integer) As Integer
        End Function

        <DllImport("rstrtmgr.dll", CharSet:=CharSet.Unicode)>
        Private Shared Function RmRegisterResources(pSessionHandle As Integer, nFiles As UInteger, rgsFileNames As String(), nApplications As UInteger, rgApplications As IntPtr, nServices As UInteger, rgsServiceNames As IntPtr) As Integer
        End Function

        <DllImport("rstrtmgr.dll")>
        Private Shared Function RmGetList(dwSessionHandle As Integer, ByRef pnProcInfoNeeded As UInteger, ByRef pnProcInfo As UInteger, <[In], Out> rgAffectedApps As RM_PROCESS_INFO(), ByRef lpdwRebootReasons As UInteger) As Integer
        End Function

        <DllImport("ntdll.dll")>
        Private Shared Function NtQueryInformationFile(FileHandle As IntPtr, ByRef IoStatusBlock As IO_STATUS_BLOCK, FileInformation As IntPtr, Length As UInteger, FileInformationClass As Integer) As Integer
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Private Shared Function CreateFileW(lpFileName As String, dwDesiredAccess As UInteger, dwShareMode As UInteger, lpSecurityAttributes As IntPtr, dwCreationDisposition As UInteger, dwFlagsAndAttributes As UInteger, hTemplateFile As IntPtr) As IntPtr
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Shared Function CloseHandle(hObject As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Private Shared Function QueryFullProcessImageName(hProcess As IntPtr, dwFlags As Integer, lpExeName As StringBuilder, ByRef lpdwSize As UInteger) As Boolean
        End Function

        Private Const FileProcessIdsUsingFileInformation As Integer = 47
        Private Const FILE_READ_ATTRIBUTES As UInteger = &H80UI
        Private Const FILE_SHARE_READ As UInteger = &H1UI
        Private Const FILE_SHARE_WRITE As UInteger = &H2UI
        Private Const FILE_SHARE_DELETE As UInteger = &H4UI
        Private Const OPEN_EXISTING As UInteger = 3UI
        Private Const FILE_FLAG_BACKUP_SEMANTICS As UInteger = &H2000000UI

#End Region

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a friendly display name (e.g. "Microsoft OneDrive") for
        ''' the first process found holding strPath open, or an empty
        ''' string if none was found, or if detection itself failed for any
        ''' reason. Combines two independent detection mechanisms - a
        ''' direct NTFS query (QueryFilePids) and the Restart Manager API
        ''' (QueryRmSession) - since each can catch cases the other misses;
        ''' this mirrors WpfFileUnlocker's own dual-engine approach.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Function GetLockingAppName(strPath As String) As String
            Try
                Dim pidNames As New Dictionary(Of Integer, String)

                For Each iPid As Integer In QueryFilePids(strPath)
                    If Not pidNames.ContainsKey(iPid) Then pidNames(iPid) = ""
                Next

                For Each kv As KeyValuePair(Of Integer, String) In QueryRmSession(strPath)
                    If Not pidNames.ContainsKey(kv.Key) Then pidNames(kv.Key) = kv.Value
                Next

                Dim iSelfPid As Integer = Process.GetCurrentProcess().Id

                For Each iPid As Integer In pidNames.Keys
                    If iPid = iSelfPid Then Continue For ' this process holding its own working copy briefly is not useful to report
                    Dim strName As String = ResolveFriendlyName(iPid, pidNames(iPid))
                    If Not String.IsNullOrEmpty(strName) Then Return strName
                Next

                Return ""

            Catch
                ' Detection is purely cosmetic - any failure here (missing
                ' privilege, unexpected OS behavior, etc.) just falls back
                ' to generic phrasing at the caller, never an error of its
                ' own.
                Return ""
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Resolves a display name for a PID: prefers the locking
        ''' executable's own FileDescription (e.g. "Microsoft OneDrive"),
        ''' falls back to the Restart Manager-reported app name, then to
        ''' the bare process name (e.g. "OneDrive").
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Function ResolveFriendlyName(iPid As Integer, strRmAppName As String) As String
            Try
                Using proc As Process = Process.GetProcessById(iPid)
                    Dim strExePath As String = TryGetProcessPath(proc)
                    Dim strDescription As String = GetFileDescription(strExePath)
                    If Not String.IsNullOrWhiteSpace(strDescription) Then Return strDescription
                    If Not String.IsNullOrWhiteSpace(strRmAppName) Then Return strRmAppName
                    Return proc.ProcessName
                End Using
            Catch
                Return If(String.IsNullOrWhiteSpace(strRmAppName), "", strRmAppName)
            End Try
        End Function

        Private Shared Function GetFileDescription(strExePath As String) As String
            If String.IsNullOrEmpty(strExePath) OrElse Not File.Exists(strExePath) Then Return ""
            Try
                Return FileVersionInfo.GetVersionInfo(strExePath).FileDescription
            Catch
                Return ""
            End Try
        End Function

        Private Shared Function TryGetProcessPath(proc As Process) As String
            Try
                Return If(proc.MainModule?.FileName, "")
            Catch
                Try
                    Dim sb As New StringBuilder(1024)
                    Dim uSize As UInteger = CUInt(sb.Capacity)
                    Return If(QueryFullProcessImageName(proc.Handle, 0, sb, uSize), sb.ToString(), "")
                Catch
                    Return ""
                End Try
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Queries the Restart Manager API for processes with strPath open.
        ''' Returns a PID -> app-name map (app name as RM itself reports it,
        ''' which ResolveFriendlyName only falls back to if a better
        ''' FileDescription isn't available).
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Function QueryRmSession(strPath As String) As Dictionary(Of Integer, String)
            Dim result As New Dictionary(Of Integer, String)

            Dim iSessionHandle As Integer = 0
            Dim iRes As Integer = RmStartSession(iSessionHandle, 0, Guid.NewGuid().ToString())
            If iRes <> 0 Then Return result

            Try
                iRes = RmRegisterResources(iSessionHandle, 1, {strPath}, 0, IntPtr.Zero, 0, IntPtr.Zero)
                If iRes <> 0 Then Return result

                Dim pnProcInfo As UInteger = 0
                Dim lpdwRebootReasons As UInteger = 0
                Dim pnProcInfoNeeded As UInteger = 0

                iRes = RmGetList(iSessionHandle, pnProcInfoNeeded, pnProcInfo, Nothing, lpdwRebootReasons)

                If iRes = ERROR_MORE_DATA AndAlso pnProcInfoNeeded > 0 Then
                    Dim processInfo(CInt(pnProcInfoNeeded) - 1) As RM_PROCESS_INFO
                    pnProcInfo = pnProcInfoNeeded
                    iRes = RmGetList(iSessionHandle, pnProcInfoNeeded, pnProcInfo, processInfo, lpdwRebootReasons)

                    If iRes = 0 Then
                        For i As Integer = 0 To CInt(pnProcInfo) - 1
                            result(processInfo(i).Process.dwProcessId) = processInfo(i).strAppName
                        Next
                    End If
                End If

            Finally
                Try
                    RmEndSession(iSessionHandle)
                Catch
                End Try
            End Try

            Return result
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Queries NTFS directly (via the undocumented but long-stable
        ''' NtQueryInformationFile / FileProcessIdsUsingFileInformation
        ''' info class - the same technique Process Explorer and
        ''' SysInternals' handle.exe use) for every PID currently holding a
        ''' handle to strPath.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Function QueryFilePids(strPath As String) As List(Of Integer)
            Dim result As New List(Of Integer)
            Dim dwFlags As UInteger = If(Directory.Exists(strPath), FILE_FLAG_BACKUP_SEMANTICS, 0UI)

            Dim hFile As IntPtr = CreateFileW(strPath, FILE_READ_ATTRIBUTES,
                FILE_SHARE_READ Or FILE_SHARE_WRITE Or FILE_SHARE_DELETE,
                IntPtr.Zero, OPEN_EXISTING, dwFlags, IntPtr.Zero)

            If hFile = INVALID_HANDLE_VALUE Then Return result

            Try
                Dim bufferSize As UInteger = 4096
                Dim buffer As IntPtr = Marshal.AllocHGlobal(CInt(bufferSize))
                Try
                    Dim ioStatusBlock As IO_STATUS_BLOCK
                    Dim iStatus As Integer = NtQueryInformationFile(hFile, ioStatusBlock, buffer, bufferSize, FileProcessIdsUsingFileInformation)
                    If iStatus <> 0 Then Return result

                    Dim iCount As Integer = Marshal.ReadInt32(buffer)
                    Dim iListOffset As Integer = IntPtr.Size

                    For i As Integer = 0 To iCount - 1
                        Dim lPid As Long
                        If IntPtr.Size = 8 Then
                            lPid = Marshal.ReadInt64(buffer, iListOffset + i * 8)
                        Else
                            lPid = Marshal.ReadInt32(buffer, iListOffset + i * 4)
                        End If

                        If lPid > 0 AndAlso lPid <= Integer.MaxValue Then
                            result.Add(CInt(lPid))
                        End If
                    Next
                Finally
                    Marshal.FreeHGlobal(buffer)
                End Try
            Finally
                CloseHandle(hFile)
            End Try

            Return result
        End Function

    End Class

End Namespace
