' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.Core
Imports EwECore.Plugins.Ecopath
Imports EwECore.Plugins.UI
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports ScientificInterfaceShared.Controls

Public Class cEcotrophPlugin
    Implements IGUIPlugin
    Implements IMenuItemPlugin
    Implements ICorePlugin
    Implements IEcopathRunCompletedPlugin
    Implements IHelpPlugin
    Implements IUIContextPlugin

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcotrophPlugin)()

    Public Sub New()

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public ReadOnly Property HelpTopic As String Implements IHelpPlugin.HelpTopic
        Get
            Return "http://sirs.agrocampus-ouest.fr//EcoTroph/index.php?action=examples"
        End Get
    End Property

    Public ReadOnly Property HelpURL As String Implements IHelpPlugin.HelpURL
        Get
            Return Me.HelpTopic
        End Get
    End Property

    Public Shared ETinputdata As ETinputtot
    Public Shared ETinputdatafromEP As ETinputtot
    ' Public Shared ETinputdataFLEET As ETinputFLEET
    ' Public Shared ETinputdataFLEETfromEP As ETinputFLEET
    Public Shared etCore As cCore
    Public Shared pack_version As String

    Private m_uic As cUIContext

    Private frmET As frmEcotroph

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements ICorePlugin.CoreInitialized
        ETinputdata = New ETinputtot()
        ETinputdatafromEP = New ETinputtot()
    End Sub

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jerome Guitton, Didier Gascuel"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "jerome.guitton@agrocampus-ouest.fr"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "EcoTroph (ET) is a modelling approach articulated around the idea that an ecosystem can be represented by its biomass distribution across trophic levels. Such an approach, wherein species as such disappear, may be regarded as the ultimate stage in the use of the trophic level metric for ecosystem modelling. By concentrating on biomass flow as a quasi-physical process, it allows aspects of ecosystem functioning to be explored which are complementary to EwE. It provides users with simple tools to quantify the impacts of fishing at an ecosystem scale and a new way of looking at ecosystems. It thus appears a useful complement to Ecopath."
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Try
            etCore = DirectCast(core, cCore)
        Catch ex As Exception
            m_logger.LogError(ex, "cEcotrophPlugin.Initialize")
        End Try
    End Sub

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "EwEEcotrophPlugin"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "EcoTroph"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return "EcoTroph"
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.Idle
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick

        Try
            If Not Me.HasInterface(Me.frmET) Then
                Me.frmET = New frmEcotroph()
                Me.frmET.UIContext = Me.m_uic
            End If

            ' Pass form reference back to calling app
            frmPlugin = Me.frmET

        Catch ex As Exception
            m_logger.LogError(ex, "cEcotrophPlugin.OnControlClick")
        End Try
    End Sub

    Private Function HasInterface(theForm As System.Windows.Forms.Form) As Boolean
        If theForm Is Nothing Then Return False
        If theForm.IsDisposed Then Return False
        Return True
    End Function

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) Implements IEcopathRunCompletedPlugin.EcopathRunCompleted

        Try
            Dim epdata As EwECore.cEcopathDataStructures
            Dim compteur As Integer
            epdata = DirectCast(EcopathDataStructures, cEcopathDataStructures)

            Dim default_accessibility As Single = 0.8

            ReDim ETinputdatafromEP.B(epdata.B.Length - 1)
            ReDim ETinputdatafromEP.GroupName(epdata.B.Length - 1)
            ReDim ETinputdatafromEP.PROD(epdata.B.Length - 1)
            ReDim ETinputdatafromEP.TL(epdata.B.Length - 1)
            ReDim ETinputdatafromEP.accessibility(epdata.B.Length - 1)
            ReDim ETinputdatafromEP.OI(epdata.B.Length - 1)
            ReDim ETinputdatafromEP.FleetName(epdata.NumFleet)

            ReDim ETinputdata.B(epdata.B.Length - 1)
            ReDim ETinputdata.GroupName(epdata.B.Length - 1)
            ReDim ETinputdata.PROD(epdata.B.Length - 1)
            ReDim ETinputdata.TL(epdata.B.Length - 1)
            ReDim ETinputdata.accessibility(epdata.B.Length - 1)
            ReDim ETinputdata.OI(epdata.B.Length - 1)
            ReDim ETinputdata.FleetName(epdata.NumFleet)

            System.Array.Copy(epdata.B, ETinputdatafromEP.B, epdata.B.Length)
            System.Array.Copy(epdata.GroupName, ETinputdatafromEP.GroupName, epdata.GroupName.Length)
            System.Array.Copy(epdata.PB, ETinputdatafromEP.PROD, epdata.PB.Length)
            ' Rajout du search and replace pour les production, pour mettre à 0 les valeurs ecopath à -9999
            For compteur = 0 To UBound(ETinputdatafromEP.PROD)
                If ETinputdatafromEP.PROD(compteur) = -9999 Then ETinputdatafromEP.PROD(compteur) = 0
            Next

            System.Array.Copy(epdata.TTLX, ETinputdatafromEP.TL, epdata.TTLX.Length)
            System.Array.Copy(epdata.FleetName, ETinputdatafromEP.FleetName, epdata.NumFleet + 1)

            'Récupération de l'index d'Omnivory
            System.Array.Copy(epdata.BQB, ETinputdatafromEP.OI, epdata.BQB.Length)
            ETinputdatafromEP.NumFleet = epdata.NumFleet
            ETinputdatafromEP.Catches = New Single(epdata.NumFleet)() {}
            ETinputdata.Catches = New Single(epdata.NumFleet)() {}
            'ETinputdata.comments = 

            ETinputdata.ModelName = epdata.ModelName
            ETinputdata.ModelDescription = epdata.ModelDescription

            For ifleet As Integer = 0 To epdata.NumFleet - 1
                ETinputdata.FleetName(ifleet) = epdata.FleetName(ifleet + 1)
                ETinputdatafromEP.Catches(ifleet) = New Single(epdata.GroupName.Length) {}
                ETinputdata.Catches(ifleet) = New Single(epdata.GroupName.Length) {}
                For j As Integer = 1 To epdata.B.Length - 1
                    If (ETinputdatafromEP.accessibility(j) = 0 And (epdata.Landing(ifleet, j) > 0 Or epdata.Discard(ifleet, j) > 0)) Then ETinputdatafromEP.accessibility(j) = default_accessibility
                    ETinputdatafromEP.Catches(ifleet)(j) = epdata.Landing(ifleet + 1, j) + epdata.Discard(ifleet + 1, j)

                Next
            Next

        Catch ex As Exception
            m_logger.LogError(ex, "cEcotrophPlugin.EcopathRunCompleted")
        End Try

    End Sub

    'Private Function match(epdata As cEcopathDataStructures, p2 As String) As Array
    '    Throw New NotImplementedException
    'End Function

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception
            m_logger.LogError(ex, "cEcotrophPlugin.UIContext")
        End Try
    End Sub

End Class
