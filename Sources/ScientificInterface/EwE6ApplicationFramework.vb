' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Reflection
Imports EwECore
Imports EwELicense
Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Serilog
Imports Debug = System.Diagnostics.Debug
Imports SharedResources = ScientificInterfaceShared.My.Resources


#End Region ' Imports

Module EwE6ApplicationFramework

#Region " Private vars "

    Private m_splash As frmSplash = Nothing
    Private m_main As frmEwE6 = Nothing
    Private m_root As String = ""

    Private m_pluginfolders() As String = New String() {"", ".\plugins"}
    Private m_lsa As New Dictionary(Of String, Assembly)

    Private m_bExpirationChecked As Boolean = False
    Private m_logger As Microsoft.Extensions.Logging.ILogger = Nothing

#End Region ' Private vars 

    Public Sub Main()

        Dim logPath = Path.GetDirectoryName(LoggingContext.LogFile)

        ' Configure Serilog
        Log.Logger = New LoggerConfiguration() _
            .MinimumLevel.Debug() _
            .Enrich.WithThreadId() _
            .Enrich.WithThreadName() _
            .WriteTo.File($"{logPath}\log-.txt", outputTemplate:="[{Timestamp:HH:mm:ss} {Level:u3} Tid:{ThreadId}]({ThreadName}) {Message:lj}{NewLine}{Exception}", rollingInterval:=RollingInterval.Day) _
            .CreateLogger()

        ' Initialize LoggerFactory
        LoggingContext.LoggerFactory = LoggerFactory.Create(Sub(builder)
                                                                builder.AddSerilog()
                                                            End Sub)

        ' Initialize logger after LoggerFactory is created
        m_logger = LoggingContext.LoggerFactory.CreateLogger("EwE6ApplicationFramework")

        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf OnResolveAssembly

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        m_root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

        ' Launch Splash window in separate thread
        ' https://stackoverflow.com/questions/32418695/show-splash-screen-during-loading-the-main-form
        If (My.Settings.ShowSplash) Then
            m_splash = New frmSplash()
            Dim threadSplashStart As New Threading.ThreadStart(AddressOf Splash)
            Dim threadSplash As New Threading.Thread(threadSplashStart)
            threadSplash.SetApartmentState(Threading.ApartmentState.STA)
            threadSplash.Start()
        End If

        ' Define new main UI
        m_main = New frmEwE6()

        AddHandler m_main.OnLoadCompleted, AddressOf OnLoadCompleted
        Try
            Application.Run(m_main)
        Catch ex As Exception
            m_logger.LogError(ex, "EwE6ApplicationFramework.Main()")
        End Try
        RemoveHandler m_main.OnLoadCompleted, AddressOf OnLoadCompleted
        RemoveHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf OnResolveAssembly

        cFileUtils.PurgeTempFiles()

        ' Termninate well
        My.Settings.Save()

    End Sub

#Region " Release mode "

    Public ReadOnly Property ReleaseMode As eReleaseMode
        Get
#If BETA = 1 Then
            Return eReleaseMode.Beta
#End If
#If DEBUG Then
            Return eReleaseMode.Dev
#Else
            Return eReleaseMode.Release
#End If
        End Get
    End Property

#End Region ' Release mode

#Region " Localization "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Find resource satellite assemblies in the known locations (not the GAK) 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function OnResolveAssembly(sender As Object, args As ResolveEventArgs) As Assembly

        Dim an As New AssemblyName(args.Name)
        Dim ass As Assembly = Nothing

        If (Not an.Name.EndsWith(".resources")) Then Return ass

        ' Not sure if sat ass is loaded more than once. I presume not, but does not hurt to check
        Dim key As String = an.Name & "_" & an.CultureInfo.Name
        If m_lsa.ContainsKey(key) Then
            Return m_lsa(key)
        End If

        ' Find in possible locations
        Dim fn As String = LocateResourceAssembly(m_root, an)
        Dim i As Integer = 0
        While (i < m_pluginfolders.Count - 1) And String.IsNullOrEmpty(fn)
            fn = LocateResourceAssembly(Path.Combine(m_root, m_pluginfolders(i)), an)
            i += 1
        End While

        If Not String.IsNullOrWhiteSpace(fn) Then
            Try
                ass = Assembly.LoadFile(fn)
            Catch ex As Exception
                m_logger.LogError(ex, "OnResolveAssemlby(" & key & ")")
            End Try
        End If
        m_lsa(key) = ass

        Return ass

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="folder"></param>
    ''' <param name="an"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LocateResourceAssembly(folder As String, an As AssemblyName) As String
        Dim fn As String = Path.Combine(folder, an.CultureInfo.Name, an.Name + ".dll")
        If (File.Exists(fn)) Then Return fn
        Return ""
    End Function

#End Region ' Localization

#Region " Splash screen "

    Private Sub Splash()
        Application.Run(m_splash)
    End Sub

    Private Sub OnLoadCompleted(sender As Object, args As EventArgs)
        CloseSplash()
        m_main.Activate()
    End Sub

    Public Sub CloseSplash()
        If (m_splash Is Nothing) Then Return
        If (m_splash.Disposing) Or (m_splash.IsDisposed) Then Return
        m_splash.Invoke(New MethodInvoker(AddressOf m_splash.Close))
        m_splash.Dispose()
        m_splash = Nothing
    End Sub

    ''' <summary>
    ''' Send a status message to the splash window
    ''' </summary>
    ''' <param name="message">The message.</param>
    Public Sub SplashStatus(message As String)
        Throw New NotImplementedException("So sorry")
    End Sub

#End Region ' Splash screen

#Region " Version formatting "

    Public Function EwEVersion(bIncludeCompileDate As Boolean, bIncludeBitness As Boolean, bIncludeRelease As Boolean) As String

        Dim strCaption As String = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.GENERIC_CAPTION, Version(bIncludeCompileDate, bIncludeBitness))
        If bIncludeRelease Then
            Dim strRelease As String = EwERelease()
            If (Not String.IsNullOrEmpty(strRelease)) Then
                strCaption = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, strCaption, strRelease)
            End If
        End If
        Return strCaption

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the EwE6.exe assembly version, formatted as a string.
    ''' </summary>
    ''' <param name="bIncludeBitness">Inlcude 32 or 64 bitness in version string.</param>
    ''' <param name="bIncludeCompilationDate">Inlcude compilation date in version string.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Version(Optional bIncludeCompilationDate As Boolean = False, Optional bIncludeBitness As Boolean = False) As String
        Get
            Try
                Dim ass As Assembly = Assembly.GetAssembly(GetType(EwE6ApplicationFramework))
                Dim an As AssemblyName = ass.GetName()
                Dim strVersion As String = cAssemblyUtils.GetVersion(an).ToString

                ' Remove the last period and everything after it, to get the Semantic version (3 digits; major, minor, patch)
                Dim lastDotIndex As Integer = strVersion.LastIndexOf("."c)
                If lastDotIndex > 0 Then
                    strVersion = strVersion.Substring(0, lastDotIndex)
                End If

                If bIncludeCompilationDate Then
                    strVersion = cStringUtils.Localize(SharedResources.VERSION_EXT_COMPILED, strVersion, cAssemblyUtils.GetCompileDate(ass).ToShortDateString)
                End If

                If bIncludeBitness Then
                    strVersion = cStringUtils.Localize("{0} - {1}", strVersion, If(cSystemUtils.Is64BitProcess, SharedResources.BITNESS_64, SharedResources.BITNESS_32))
                End If

                Return strVersion
            Catch ex As Exception
                m_logger.LogError(ex, "Version")
                Return ""
            End Try
        End Get
    End Property

    ''' <summary>
    ''' Returns the release mode of EwE: beta, debug, official release
    ''' </summary>
    ''' <returns></returns>
    Public Function EwERelease() As String

        Dim strRelease As String = ""
        Select Case EwE6ApplicationFramework.ReleaseMode
            Case eReleaseMode.Beta
                strRelease = My.Resources.VERSION_BETA
            Case eReleaseMode.Dev
                strRelease = My.Resources.VERSION_DEVELOPMENT
            Case eReleaseMode.Release
                ' NOP
        End Select

#If DEBUG Then
        If Not String.IsNullOrWhiteSpace(strRelease) Then
            strRelease &= ", "
        End If
        strRelease += "DEBUG " & AppContext.BaseDirectory
#End If
        Return strRelease

    End Function

    ''' <summary>
    ''' Returns the registration status of EwE
    ''' </summary>
    ''' <returns></returns>
    Public Function EwERegistration(lic As cLicense) As String

        Try
            If (lic IsNot Nothing) Then
                If (lic.IsRegistered) Then
                    'If (lic.IsLicensed) Then
                    Return cStringUtils.Localize(My.Resources.REGISTRATION_ACTIVE, lic.Owner, cDateUtils.ToShortDateStringFullYear(lic.Expiry))
                    'Else
                    '    Return cStringUtils.Localize(My.Resources.REGISTRATION_EXPIRED, lic.Owner)
                    'End If
                End If
            End If
        Catch ex2 As ObjectDisposedException
            ' Can happen during app shutdown. Ignore
        Catch ex As Exception

        End Try
        Return My.Resources.REGISTRATION_NONE

    End Function

    ''' <summary>
    ''' Returns the license of EwE: Pro [teams|individual], free version
    ''' </summary>
    Public Function EwELicense(lic As cLicense) As String
        Try
            If (lic IsNot Nothing) Then
                If (lic.IsRegistered) Then
                    Select Case lic.LicenseType
                        Case cLicense.eLicenseType.NotSet
                            Return My.Resources.LICENSE_PRO
                        Case cLicense.eLicenseType.Team
                            Return My.Resources.LICENSE_PRO_TEAM
                        Case cLicense.eLicenseType.Individual
                            Return My.Resources.LICENSE_PRO_INDIVIDUAL
                    End Select
                End If
            End If
        Catch ex2 As ObjectDisposedException
            ' Can happen during app shutdown. Ignore
        Catch ex As Exception

        End Try
        Return My.Resources.LICENSE_FREE
    End Function

#End Region ' Version formatting

End Module
