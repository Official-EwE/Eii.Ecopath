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
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwEUtils.SystemUtilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore
Imports EwELicense

#End Region ' Imports

Module EwE6ApplicationFramework

#Region " Private vars "

    Private m_splash As frmSplash = Nothing
    Private m_main As frmEwE6 = Nothing
    Private m_root As String = ""

    Private m_pluginfolders() As String = New String() {"", ".\plugins"}
    Private m_lsa As New Dictionary(Of String, Assembly)

    Private m_bExpirationChecked As Boolean = False

#End Region ' Private vars 

    Public Sub Main()

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
            cLog.Write(ex)
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
                cLog.Write(ex, "OnResolveAssemlby(" & key & ")")
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
        CloseSplash()
    End Sub

    Private Sub OnLoadCompleted(sender As Object, args As EventArgs)
        CloseSplash()
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

        Dim strCaption As String = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.GENERIC_CAPTION, cCore.Version(bIncludeCompileDate, bIncludeBitness))
        If bIncludeRelease Then
            Dim strRelease As String = EwERelease()
            If (Not String.IsNullOrEmpty(strRelease)) Then
                strCaption = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, strCaption, strRelease)
            End If
        End If
        Return strCaption

    End Function

    ''' <summary>
    ''' Returns the release mode of EwE: beta, debug, official release
    ''' </summary>
    ''' <returns></returns>
    Public Function EwERelease() As String

        Select Case EwE6ApplicationFramework.ReleaseMode
            Case eReleaseMode.Beta
                Return My.Resources.VERSION_BETA
            Case eReleaseMode.Dev
                Return My.Resources.VERSION_DEVELOPMENT
            Case eReleaseMode.Release
                ' NOP
        End Select
        Return ""

    End Function

    ''' <summary>
    ''' Returns the registration status of EwE
    ''' </summary>
    ''' <returns></returns>
    Public Function EwERegistration(lic As cLicense) As String

        Try
            If (lic IsNot Nothing) Then
                If (lic.IsRegistered) Then
                    If (lic.IsLicensed) Then
                        Return cStringUtils.Localize(My.Resources.REGISTRATION_ACTIVE, lic.Owner, lic.Expiry.ToShortDateString())
                    Else
                        Return cStringUtils.Localize(My.Resources.REGISTRATION_EXPIRED, lic.Owner)
                    End If
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
