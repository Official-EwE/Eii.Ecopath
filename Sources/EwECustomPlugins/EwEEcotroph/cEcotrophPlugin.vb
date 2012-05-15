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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin

''' <summary>
''' Plug-in point for the Ecotroph plug-in.
''' </summary>
Public Class cEcotrophPlugin
    Implements EwEPlugin.IGUIPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin

#Region " Internals "

    Structure ETinputtot
        Dim groupname() As String
        Dim TL() As Single
        Dim B() As Single
        Dim PROD() As Single
        Dim accessibility() As Single
        Dim OI() As Single
        Dim catches()() As Single
        Dim numfleet As Integer
        Dim fleetname() As String
        Dim ModelName As String
        Dim Modeldescription As String
        Dim comments As String
    End Structure

    Public Shared ETinputdata As ETinputtot
    Public Shared ETinputdatafromEP As ETinputtot
    ' Public Shared ETinputdataFLEET As ETinputFLEET
    ' Public Shared ETinputdataFLEETfromEP As ETinputFLEET
    Public Shared etCore As cCore = Nothing

    Private frmET As frmEcotroph = Nothing

#End Region ' Internals

#Region " Construction / destruction "

    Public Sub New()
        ' NOP
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#End Region ' Construction / destruction

#Region " Plugin points implementation "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.CoreInitialized"/>
    ''' -----------------------------------------------------------------------
    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return My.Resources.PLUGIN_AUTHOR
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Contact"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "http://sirs.agrocampus-ouest.fr/EcoTroph/index.php"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return My.Resources.PLUGIN_DESCRIPTION
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        etCore = DirectCast(core, cCore)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndEcotroph"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IGUIPlugin.ControlImage"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IGUIPlugin.ControlText"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.PLUGIN_CONTROLTEXT
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IGUIPlugin.ControlTooltipText"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return Me.ControlText
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IGUIPlugin.EnabledState"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.Idle
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IGUIPlugin.OnControlClick"/>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        ' Test if form still exists
        If Not Me.HasInterface(DirectCast(Me.frmET, System.Windows.Forms.Form)) Then
            frmET = New frmEcotroph
        End If

        ' Pass form reference back to calling app
        frmPlugin = frmET
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IMenuItemPlugin.MenuItemLocation"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted"/>
    ''' -----------------------------------------------------------------------
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted

        Dim epdata As EwECore.cEcopathDataStructures = DirectCast(EcopathDataStructures, cEcopathDataStructures)
        Dim compteur As Integer
        Dim default_accessibility As Single = 1

        ReDim ETinputdatafromEP.B(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.groupname(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.PROD(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.TL(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.accessibility(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.OI(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.fleetname(epdata.NumFleet)

        ReDim ETinputdata.B(epdata.B.Length - 1)
        ReDim ETinputdata.groupname(epdata.B.Length - 1)
        ReDim ETinputdata.PROD(epdata.B.Length - 1)
        ReDim ETinputdata.TL(epdata.B.Length - 1)
        ReDim ETinputdata.accessibility(epdata.B.Length - 1)
        ReDim ETinputdata.OI(epdata.B.Length - 1)
        ReDim ETinputdata.fleetname(epdata.NumFleet)

        System.Array.Copy(epdata.B, ETinputdatafromEP.B, epdata.B.Length)
        System.Array.Copy(epdata.GroupName, ETinputdatafromEP.groupname, epdata.GroupName.Length)
        System.Array.Copy(epdata.PB, ETinputdatafromEP.PROD, epdata.PB.Length)
        ' Rajout du search and replace pour les production, pour mettre à 0 les valeurs ecopath à -9999
        For compteur = 0 To epdata.B.Length - 1
            If ETinputdatafromEP.PROD(compteur) = -9999 Then ETinputdatafromEP.PROD(compteur) = 0
        Next

        System.Array.Copy(epdata.TTLX, ETinputdatafromEP.TL, epdata.TTLX.Length)
        System.Array.Copy(epdata.FleetName, ETinputdatafromEP.fleetname, epdata.NumFleet + 1)

        'Récupération de l'index d'Omnivory
        System.Array.Copy(epdata.BQB, ETinputdatafromEP.OI, epdata.BQB.Length)
        ETinputdatafromEP.numfleet = epdata.NumFleet
        ETinputdatafromEP.catches = New Single(epdata.NumFleet)() {}
        ETinputdata.catches = New Single(epdata.NumFleet)() {}
        ETinputdata.ModelName = epdata.ModelName
        ETinputdata.Modeldescription = epdata.ModelDescription

        For ifleet As Integer = 0 To epdata.NumFleet - 1
            ETinputdata.fleetname(ifleet) = epdata.FleetName(ifleet + 1)
            ETinputdatafromEP.catches(ifleet) = New Single(epdata.GroupName.Length) {}
            ETinputdata.catches(ifleet) = New Single(epdata.GroupName.Length) {}
            For j As Integer = 1 To epdata.B.Length - 1
                If (ETinputdatafromEP.accessibility(j) = 0 And epdata.Landing(ifleet, j) > 0) Then ETinputdatafromEP.accessibility(j) = default_accessibility
                ETinputdatafromEP.catches(ifleet)(j) = epdata.Landing(ifleet, j)
            Next
        Next

    End Sub

#End Region ' Plugin points implementation

#Region " Internals "

    Private Function HasInterface(ByVal theForm As System.Windows.Forms.Form) As Boolean
        If theForm Is Nothing Then Return False
        If theForm.IsDisposed Then Return False
        Return True
    End Function

    Private Function match(ByVal epdata As cEcopathDataStructures, ByVal p2 As String) As Array
        Throw New NotImplementedException
    End Function

#End Region ' Internals

End Class
