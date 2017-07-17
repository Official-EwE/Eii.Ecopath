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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
Option Strict On
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.IO
Imports System.Reflection

''' <summary>
''' Base code of a plug-in that demonstrates the integration of a plug-in with the EwE auto-save system.
''' </summary>
''' <remarks>
''' <para>In order to run and test this plug-in it must be integrated within the EwE6 scientific interface. 
''' To achieve this, add this project to the EwE6 solution, and reference this project from within the 
''' ScientificInterface. This ensures that your plug-in will be built with EwE6, and will be loaded by the 
''' EwE6 plug-in manager when you run EwE6.</para>
''' </remarks>
Public Class cAutoSavePlugin
    Implements IAutoSavePlugin
    Implements IEcosimRunCompletedPlugin

    Private m_core As cCore = Nothing

    Public Sub New()
        ' By default turn auto-saving on. EwE will set this flag to its state of the last EwE session.
        Me.AutoSave = True
    End Sub

    Public Function AutoSaveName() As String Implements IAutoSavePlugin.AutoSaveName
        ' This name will be shown to the user in the EwE File Management options panel, 
        ' Menu > Tools > options > File management
        Return "AutoSave plugin example"
    End Function

    Public Function AutoSaveType() As eAutosaveTypes Implements IAutoSavePlugin.AutoSaveType
        ' The auto-save UI will nest this plug-in under the main Ecosim category
        Return eAutosaveTypes.Ecosim
    End Function

    Public Function AutoSaveOutputPath() As String Implements IAutoSavePlugin.AutoSaveOutputPath
        ' The auto-save UI will display the following directory as the destination where this plug-in will save
        Return Path.Combine(Me.m_core.DefaultOutputPath(Me.AutoSaveType), "AutoSavePluginSample")
    End Function

    ' This flag states if the plug-in should autosave. The user can change this flag through the EwE user interface
    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave

    ' Finally, here the actual auto-save logic is implemented
    Public Sub EcosimRunCompleted(EcosimDatastructures As Object) Implements IEcosimRunCompletedPlugin.EcosimRunCompleted

        ' Abort if not configured for auto-saving
        If (Not Me.AutoSave) Then Return

        ' Abort if output folder does not exist, and cannot be created
        If Not cFileUtils.IsDirectoryAvailable(Me.AutoSaveOutputPath, True) Then Return

        ' Create output file name
        Dim FileName As String = Path.Combine(Me.AutoSaveOutputPath, "test_output.txt")

        Try
            ' Save the file (this may explode)
            Using sw As New StreamWriter(FileName)
                sw.WriteLine("Auto-save plug-in example wrote this file on " & Date.Now.ToShortDateString & ", " & Date.Now.ToShortTimeString)
                sw.Flush()
            End Using

            ' Notify user via a status panel message that data has been saved OK
            Dim msg As New cMessage("Auto-save plug-in data was written to " & FileName, eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            ' Provide a hyperlink for the message, which the EwE user interface will show as a clickable link
            msg.Hyperlink = Me.AutoSaveOutputPath
            ' Send the message
            Me.m_core.Messages.SendMessage(msg)

        Catch ex As Exception

            ' Notify user via a status panel message that data failed to save
            Dim msg As New cMessage("Auto-save plug-in data failed to save. " & ex.Message, eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
            ' Send the message
            Me.m_core.Messages.SendMessage(msg)

            ' Write exception to the EwE log file, explaining where it occurred
            cLog.Write(ex, "cAutoSavePlugin.EcosimRunCompleted")

        End Try
    End Sub

#Region " Generic plug-in functionality "

    Public Sub Initialize(ByVal core As Object) _
        Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public ReadOnly Property Author() As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "your name"
        End Get
    End Property

    Public ReadOnly Property Contact() As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "your email"
        End Get
    End Property

    Public ReadOnly Property Description() As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Dim assembly As Assembly = Assembly.GetAssembly(GetType(cAutoSavePlugin))
            Dim descr As AssemblyDescriptionAttribute = CType(assembly.GetCustomAttribute(GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
            Return descr.Description
        End Get
    End Property

    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            ' Internal unique name identifying the plug-in
            Return "AutoSave plug-in example"
        End Get
    End Property

#End Region ' Generic plug-in functionality

End Class
