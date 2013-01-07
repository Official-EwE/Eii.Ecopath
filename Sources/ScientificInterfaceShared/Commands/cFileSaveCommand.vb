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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Commands
Imports System.IO

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Generic command to launch an interface to select a 'save file' location.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cFileSaveCommand
        Inherits cCommand

#Region " Privates "

        ''' <summary>Dialog caption.</summary>
        Private m_strTitle As String = ""
        ''' <summary>Name of the file to save.</summary>
        Private m_strFileName As String = ""
        ''' <summary>Directory to initialize the dialog with.</summary>
        Private m_strDirectory As String = ""
        ''' <summary>File filter to use.</summary>
        Private m_strFileFilters As String = ""
        ''' <summary>Default file filter.</summary>
        Private m_iFilter As Integer = 0
        ''' <summary>The dialog result.</summary>
        Private m_iResult As DialogResult = DialogResult.OK

#End Region ' Privates

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~savefile"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        Public Overrides Sub Invoke()
            Me.m_iResult = DialogResult.Cancel
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <param name="strFileName"></param>
        ''' <param name="strFileFilter"></param>
        ''' <param name="iFilter"></param>
        ''' <param name="strTitle">
        ''' Optional dialog title. If left empty, the .NET default is used.
        ''' </param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal strFileName As String, _
                                    ByVal strFileFilter As String, _
                                    Optional ByVal iFilter As Integer = 0, _
                                    Optional ByVal strTitle As String = "")

            Dim strPath As String = ""

            Me.m_strTitle = strTitle
            Me.m_strFileName = strFileName
            Me.m_strFileFilters = strFileFilter
            Me.m_iFilter = iFilter

            Try
                ' Only update directory if a directory has been specified
                strPath = Path.GetDirectoryName(strFileName)
                If Not String.IsNullOrEmpty(strPath) Then Me.m_strDirectory = strPath
            Catch ex As Exception
            End Try

            Me.Invoke()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <param name="strFileFilter"></param>
        ''' <param name="iFilter"></param>
        ''' <param name="strTitle">
        ''' Optional dialog title. If left empty, the Visual Studio default is used.
        ''' </param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal strFileFilter As String, _
                                    Optional ByVal iFilter As Integer = 0, _
                                    Optional ByVal strTitle As String = "")

            Me.Invoke("", strFileFilter, iFilter, strTitle)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the title to display in the dialog. If left emtpy, the .NET
        ''' framework will use the default file open title.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Title() As String
            Get
                Return Me.m_strTitle
            End Get
            Set(ByVal strTitle As String)
                Me.m_strTitle = strTitle
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set the file name to show in the dialog. Once invoked and closed
        ''' with the result <see cref="DialogResult.OK">OK</see>, this
        ''' property will contain the full path to the file selected in the 
        ''' dialog.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FileName() As String
            Get
                Return Me.m_strFileName
            End Get
            Set(ByVal strFileName As String)
                Me.m_strFileName = strFileName
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the directory that the command was invoked with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Directory() As String
            Get
                Return Me.m_strDirectory
            End Get
            Set(ByVal strDirectory As String)
                Me.m_strDirectory = strDirectory
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The result that the dialog closed with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Result() As DialogResult
            Get
                Return Me.m_iResult
            End Get
            Set(ByVal value As DialogResult)
                Me.m_iResult = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the file filters that the dialog was invoked with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Filters() As String
            Get
                Return Me.m_strFileFilters
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the file filter index that the dialog was invoked with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FilterIndex() As Integer
            Get
                Return Me.m_iFilter
            End Get
            Set(ByVal value As Integer)
                Me.m_iFilter = value
            End Set
        End Property


    End Class

End Namespace
