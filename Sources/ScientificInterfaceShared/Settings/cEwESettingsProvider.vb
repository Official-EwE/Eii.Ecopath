'==============================================================================
'
' $Log: cEwESettingsProvider.vb,v $
' Revision 1.4  2009/04/08 13:11:19  jeroens
' Try! Catch! Aargh!
' Hopefully this class is robust enough now
'
' Revision 1.3  2009/03/26 17:40:44  jeroens
' Added null checks, just in case
'
' Revision 1.2  2008/12/15 15:37:54  jeroens
' no message
'
' Revision 1.1  2008/08/14 01:49:54  jeroens
' Moved
'
' Revision 1.2  2008/07/31 16:17:33  jeroens
' Added Name
'
' Revision 1.1  2008/07/29 18:55:05  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports System.Configuration
Imports System.Configuration.Provider
Imports System.IO
Imports System.Windows.Forms
Imports System.Collections.Specialized
Imports Microsoft.Win32
Imports System.Xml

#End Region ' Imports

Public Class cEwESettingsProvider
    Inherits SettingsProvider

#Region " Private parts "

    Private Const cSETTINGSROOT As String = "Settings" 'XML Root Node
    Private m_xmldocSettings As Xml.XmlDocument = Nothing

#End Region ' Private parts

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialization. Overridden to stop .NET from trying to be too smart.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="col"></param>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Initialize(ByVal strName As String, ByVal col As NameValueCollection)
        MyBase.Initialize(Me.ApplicationName, col)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' I have no idea who uses this, but hey, I'll override anything you'll
    ''' tell me to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property Name() As String
        Get
            Return "EwEProgramSettingsProvider"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Who are you?
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property ApplicationName() As String
        Get
            Return Path.GetFileNameWithoutExtension(Application.ExecutablePath)
        End Get
        Set(ByVal value As String)
            'Do nothing
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store the values of all properties.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="propvals"></param>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub SetPropertyValues(ByVal context As SettingsContext, ByVal propvals As SettingsPropertyValueCollection)

        ' Sanity check
        If propvals Is Nothing Then Return

        Try
            'Iterate through the settings to be stored
            'Only dirty settings are included in propvals, and only ones relevant to this provider
            For Each propval As SettingsPropertyValue In propvals
                StoreValue(propval)
            Next

            ' Save the document
            SettingsDoc.Save(IO.Path.Combine(GetAppSettingsPath, GetAppSettingsFilename))

        Catch ex As Exception
            'Ignore if can't save
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the values of all properties.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="props"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function GetPropertyValues(ByVal context As SettingsContext, ByVal props As SettingsPropertyCollection) As SettingsPropertyValueCollection

        Dim values As SettingsPropertyValueCollection = New SettingsPropertyValueCollection()
        Dim value As SettingsPropertyValue = Nothing

        If props IsNot Nothing Then
            'Iterate through the settings to be retrieved
            For Each setting As SettingsProperty In props
                Try
                    value = New SettingsPropertyValue(setting)
                    value.IsDirty = False
                    value.SerializedValue = GetValue(setting)
                    values.Add(value)
                Catch ex As Exception
                    ' Yohoho
                End Try
            Next
        End If

        Return values

    End Function

#Region " Internal overridables "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get location where to store settings file.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' .NET uses the ApplicationData structure for this. EwE6 instead stores this value 
    ''' in the application directory.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Function GetAppSettingsPath() As String
        Dim fi As New System.IO.FileInfo(Application.ExecutablePath)
        Return fi.DirectoryName
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get name of settings file.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' .NET commonly tries to do very fancy things here, pertaining to merging
    ''' different versions of settings, and managing local and roaming settings.
    ''' EwE6 does not need any of that stuff; let's keep it simple.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Function GetAppSettingsFilename() As String
        Return Me.ApplicationName & ".settings"
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the XML document to operate on.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable ReadOnly Property SettingsDoc() As Xml.XmlDocument
        Get

            Dim strSettingsDocPath As String = IO.Path.Combine(GetAppSettingsPath, GetAppSettingsFilename)
            Dim bFileRead As Boolean = False
            Dim decl As XmlDeclaration = Nothing
            Dim node As XmlNode = Nothing

            ' Is XML doc present?
            If (Me.m_xmldocSettings Is Nothing) Then
                ' #No: make one
                Me.m_xmldocSettings = New Xml.XmlDocument
                ' Does file exist?
                If File.Exists(strSettingsDocPath) Then
                    ' #Yes: try to read it
                    Try
                        ' Load file
                        Me.m_xmldocSettings.Load(strSettingsDocPath)
                        ' All good
                        bFileRead = True
                    Catch ex As Exception
                        ' Kaboom
                        bFileRead = False
                    End Try
                End If

                ' File not read yet?
                If (Not bFileRead) Then
                    ' #Yes: create new document
                    decl = Me.m_xmldocSettings.CreateXmlDeclaration("1.0", "utf-8", String.Empty)
                    Me.m_xmldocSettings.AppendChild(decl)

                    node = Me.m_xmldocSettings.CreateNode(XmlNodeType.Element, cSETTINGSROOT, "")
                    Me.m_xmldocSettings.AppendChild(node)
                End If
            End If

            ' Return prepared (and hopefully read) document
            Return Me.m_xmldocSettings

        End Get
    End Property

#End Region ' Internal overridables

#Region " Internal bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return a single value from the settings.
    ''' </summary>
    ''' <param name="sp"></param>
    ''' <returns>
    ''' A value in the form of a string, or an emtpy string if an error occurred.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function GetValue(ByVal sp As SettingsProperty) As String

        Dim strValue As String = ""

        If (sp IsNot Nothing) Then

            Try
                strValue = SettingsDoc.SelectSingleNode(cSETTINGSROOT & "/" & sp.Name).InnerText
            Catch ex As Exception
                If (sp.DefaultValue IsNot Nothing) Then
                    strValue = sp.DefaultValue.ToString
                Else
                    strValue = ""
                End If
            End Try
        End If

        Return strValue

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store a single value into the settings.
    ''' </summary>
    ''' <param name="propVal"></param>
    ''' -----------------------------------------------------------------------
    Private Sub StoreValue(ByVal propVal As SettingsPropertyValue)

        Dim elem As Xml.XmlElement

        If (propVal Is Nothing) Then Return

        'Determine if the setting is roaming.
        'If roaming then the value is stored as an element under the root
        'Otherwise it is stored under a machine name node 
        Try
            elem = DirectCast(SettingsDoc.SelectSingleNode(cSETTINGSROOT & "/" & propVal.Name), XmlElement)
        Catch ex As Exception
            elem = Nothing
        End Try

        Try

            'Check to see if the node exists, if so then set its new value
            If (elem IsNot Nothing) Then
                elem.InnerText = propVal.SerializedValue.ToString
            Else
                'Store the value as an element of the Settings Root Node
                elem = SettingsDoc.CreateElement(propVal.Name)
                elem.InnerText = propVal.SerializedValue.ToString
                SettingsDoc.SelectSingleNode(cSETTINGSROOT).AppendChild(elem)
            End If
        Catch ex As Exception
            ' Value not set
        End Try

    End Sub

#End Region ' Internal bits

End Class

