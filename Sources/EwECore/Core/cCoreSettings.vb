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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Storage class for system-wide, model independent EwE core settings.
''' </summary>
''' ---------------------------------------------------------------------------
Friend Class cCoreSettings

#Region " Private vars "

    ''' <summary>Autosave flags</summary>
    Private m_bAutosave() As Boolean
    ''' <summary>Flag, stating if autosave info should carry standard EwE header information</summary>
    Private m_bAutosaveHeaders As Boolean
    ''' <summary>Autosave formats</summary>
    Private m_strAutosaveFormat() As String
    ''' <summary>Path for EwE core processes to write output information to.</summary>
    Private m_strOutputPath As String = ""
    ''' <summary>Path for the core to write backup files to.</summary>
    Private m_strBackupFileMask As String = ""

    ''' <summary>Default author name.</summary>
    Private m_strAuthor As String = ""
    ''' <summary>Default author contact information.</summary>
    Private m_strContact As String = ""

#End Region ' Private vars

#Region " Constructor "

    Public Sub New()
        ReDim m_bAutosave([Enum].GetValues(GetType(eAutosaveTypes)).Length)
        ReDim m_strAutosaveFormat([Enum].GetValues(GetType(eAutosaveTypes)).Length)
    End Sub

#End Region ' Constructor

#Region " Accessors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a component is allowed to auto-save.
    ''' </summary>
    ''' <param name="t">The <see cref="eAutosaveTypes">auto-save enabled component</see>
    ''' to enable or disable.</param>
    ''' -----------------------------------------------------------------------
    Public Property Autosave(t As eAutosaveTypes) As Boolean
        Get
            Return Me.m_bAutosave(t)
        End Get
        Set(value As Boolean)
            Me.m_bAutosave(t) = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a component is allowed to auto-save.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property AutosaveHeaders() As Boolean
        Get
            Return Me.m_bAutosaveHeaders
        End Get
        Set(value As Boolean)
            Me.m_bAutosaveHeaders = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the file format for autosaving a given component.
    ''' </summary>
    ''' <param name="t">The <see cref="eAutosaveTypes">auto-save enabled component</see>
    ''' to access the format for.</param>
    ''' -----------------------------------------------------------------------
    Public Property AutosaveFormat(t As eAutosaveTypes) As String
        Get
            Dim str As String = Me.m_strAutosaveFormat(t)
            If String.IsNullOrWhiteSpace(str) Then str = ".csv"
            Return str
        End Get
        Set(value As String)
            Me.m_strAutosaveFormat(t) = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the core output path.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property OutputPath As String
        Get
            Return Me.m_strOutputPath
        End Get
        Set(value As String)
            Me.m_strOutputPath = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the model backup path mask.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BackupFileMask As String
        Get
            If (String.IsNullOrWhiteSpace(Me.m_strBackupFileMask)) Then
                Return "{ModelFile}_{Date}-{Time}.{ModelExt}"
            End If
            Return Me.m_strBackupFileMask
        End Get
        Set(value As String)
            Me.m_strBackupFileMask = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default EwE author.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Author As String
        Get
            If (String.IsNullOrWhiteSpace(Me.m_strAuthor)) Then Return cSystemUtils.GetUserName()
            Return Me.m_strAuthor
        End Get
        Set(value As String)
            Me.m_strAuthor = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default EwE author contact.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Contact As String
        Get
            Return Me.m_strContact
        End Get
        Set(value As String)
            Me.m_strContact = value
        End Set
    End Property

#End Region ' Accessors

End Class
