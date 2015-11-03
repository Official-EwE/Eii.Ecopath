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

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Core


Public Class cEcotroph_Eco_BasePlugIn
    Implements EwEPlugin.IGUIPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin
    Implements EwEPlugin.IHelpPlugin
    Implements EwEPlugin.IUIContextPlugin

    Public Sub New()

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public ReadOnly Property HelpTopic As String Implements EwEPlugin.IHelpPlugin.HelpTopic
        Get
            Return "http://sirs.agrocampus-ouest.fr//EcoTroph/index.php?action=examples"
        End Get
    End Property

    Public ReadOnly Property HelpURL As String Implements EwEPlugin.IHelpPlugin.HelpURL
        Get
            Return Me.HelpTopic
        End Get
    End Property

    Structure ETinputtot
        Dim groupname() As String
        Dim groupseq() As Single
        Dim TL() As Single
        Dim B() As Single
        Dim PROD() As Single
        Dim istanza() As Single
        Dim pp_input() As Single
        Dim accessibility() As Single
        Dim OI() As Single
        Dim catches()() As Single
        Dim numfleet As Single
        Dim fleetname() As String
        Dim fleetseq() As Single
        Dim ModelName As String
        Dim Modeldescription As String
        Dim comments As String
        Dim habitat_area() As Single
        Dim biomass_habitat_area() As Single
        Dim ee() As Single
        Dim biom_acc() As Single
        Dim diets() As Single

        Dim biom_acc_rate() As Single
        Dim flow_to_det() As Single
        Dim net_efficiency() As Single
        Dim fish_mort_rate() As Single
        Dim pred_mort_rate() As Single
        Dim net_migration_rate() As Single
        Dim other_mort_rate() As Single
        ' new import for Ecopath/EcoBase interoperability
        Dim PROD_INPUT() As Single
        Dim habitat_area_input() As Single
        Dim ee_input() As Single
        Dim qb() As Single
        Dim qb_input() As Single
        Dim biom_acc_input() As Single

    End Structure

    Public Shared ETinputdata As ETinputtot
    Public Shared ETinputdatafromEP As ETinputtot
    ' Public Shared ETinputdataFLEET As ETinputFLEET
    ' Public Shared ETinputdataFLEETfromEP As ETinputFLEET
    Public Shared etCore As cCore
    Public Shared pack_version As String

    Private m_uic As cUIContext

    Private frmET As frmEcoTroph_Eco_Base

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized

    End Sub

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jerome Guitton, Didier Gascuel"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "jerome.guitton@agrocampus-ouest.fr"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "EcoTroph (ET) is a modelling approach articulated around the idea that an ecosystem can be represented by its biomass distribution across trophic levels. Such an approach, wherein species as such disappear, may be regarded as the ultimate stage in the use of the trophic level metric for ecosystem modelling. By concentrating on biomass flow as a quasi-physical process, it allows aspects of ecosystem functioning to be explored which are complementary to EwE. It provides users with simple tools to quantify the impacts of fishing at an ecosystem scale and a new way of looking at ecosystems. It thus appears a useful complement to Ecopath."
        End Get
    End Property

    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            etCore = DirectCast(core, cCore)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ET_Eco_Base plug-in"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "EcoTroph_EcoBase"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "EcoTroph_Eco_Base"
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.Idle
        End Get
    End Property


    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        ' Test if form still exists
        If Not Me.HasInterface(Me.frmET) Then
            frmET = New frmEcoTroph_Eco_Base
            frmET.UIContext = Me.m_uic
        End If

        ' Pass form reference back to calling app
        frmPlugin = frmET
    End Sub


    Private Function HasInterface(ByVal theForm As System.Windows.Forms.Form) As Boolean
        If theForm Is Nothing Then Return False
        If theForm.IsDisposed Then Return False
        Return True
    End Function

    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property
    Public ReadOnly Property Core() As cCore
        Get
            Return Me.m_uic.Core
        End Get
    End Property
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted
        Dim epdata As EwECore.cEcopathDataStructures

        Dim compteur As Integer

        Dim AInitMortCoef(9, Core.nGroups) As Object

        epdata = DirectCast(EcopathDataStructures, cEcopathDataStructures)

        Dim default_accessibility As Single = 0.8
        
        
        ReDim ETinputdatafromEP.B(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.groupname(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.PROD(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.PROD_INPUT(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.TL(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.accessibility(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.OI(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.fleetname(epdata.NumFleet)
        ReDim ETinputdatafromEP.biom_acc_rate(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.habitat_area(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.biomass_habitat_area(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.ee(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.flow_to_det(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.net_efficiency(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.fish_mort_rate(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.pred_mort_rate(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.net_migration_rate(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.other_mort_rate(epdata.B.Length - 1)
        'new values for Ecopath --> Ecopath
        ReDim ETinputdatafromEP.pp_input(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.PROD_INPUT(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.habitat_area_input(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.ee_input(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.qb(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.qb_input(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.biom_acc_input(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.groupseq(epdata.B.Length - 1)
        ReDim ETinputdatafromEP.fleetseq(epdata.NumFleet)
        ReDim ETinputdatafromEP.istanza(epdata.B.Length - 1)

        ReDim ETinputdata.B(epdata.B.Length - 1)
        ReDim ETinputdata.groupname(epdata.B.Length - 1)
        ReDim ETinputdata.PROD(epdata.B.Length - 1)
        ReDim ETinputdata.PROD_INPUT(epdata.B.Length - 1)
        ReDim ETinputdata.TL(epdata.B.Length - 1)
        ReDim ETinputdata.accessibility(epdata.B.Length - 1)
        ReDim ETinputdata.OI(epdata.B.Length - 1)
        ReDim ETinputdata.fleetname(epdata.NumFleet)
        ReDim ETinputdata.biom_acc_rate(epdata.B.Length - 1)
        ReDim ETinputdata.habitat_area(epdata.B.Length - 1)
        ReDim ETinputdata.biomass_habitat_area(epdata.B.Length - 1)
        ReDim ETinputdata.ee(epdata.B.Length - 1)
        ReDim ETinputdata.flow_to_det(epdata.B.Length - 1)
        ReDim ETinputdata.net_efficiency(epdata.B.Length - 1)
        ReDim ETinputdata.fish_mort_rate(epdata.B.Length - 1)
        ReDim ETinputdata.pred_mort_rate(epdata.B.Length - 1)
        ReDim ETinputdata.net_migration_rate(epdata.B.Length - 1)
        ReDim ETinputdata.other_mort_rate(epdata.B.Length - 1)
        'new values for Ecopath --> Ecopath
        ReDim ETinputdata.pp_input(epdata.B.Length - 1)
        ReDim ETinputdata.PROD_INPUT(epdata.B.Length - 1)
        ReDim ETinputdata.habitat_area_input(epdata.B.Length - 1)
        ReDim ETinputdata.ee_input(epdata.B.Length - 1)
        ReDim ETinputdata.qb(epdata.B.Length - 1)
        ReDim ETinputdata.qb_input(epdata.B.Length - 1)
        ReDim ETinputdata.biom_acc_input(epdata.B.Length - 1)
        ReDim ETinputdata.groupseq(epdata.B.Length - 1)
        ReDim ETinputdata.fleetseq(epdata.NumFleet)
        ReDim ETinputdata.istanza(epdata.B.Length - 1)

        For Row = 1 To Core.nGroups
            AInitMortCoef(0, Row) = Core.EcoPathGroupOutputs(Row).Index
            ETinputdatafromEP.groupname(Row) = Core.EcoPathGroupOutputs(Row).Name
            ETinputdatafromEP.PROD(Row) = Core.EcoPathGroupOutputs(Row).PBOutput
            ETinputdatafromEP.PROD_INPUT(Row) = Core.EcoPathGroupInputs(Row).PBInput
            ETinputdatafromEP.TL(Row) = Core.EcoPathGroupOutputs(Row).TTLX
            ETinputdatafromEP.OI(Row) = Core.EcoPathGroupOutputs(Row).OmnivoryIndex

            ETinputdatafromEP.habitat_area(Row) = Core.EcoPathGroupOutputs(Row).Area
            ETinputdatafromEP.B(Row) = Core.EcoPathGroupOutputs(Row).Biomass
            ETinputdatafromEP.biomass_habitat_area(Row) = Core.EcoPathGroupOutputs(Row).BiomassArea
            ETinputdatafromEP.ee(Row) = Core.EcoPathGroupOutputs(Row).EEOutput
            ETinputdatafromEP.biom_acc_rate(Row) = Core.EcoPathGroupOutputs(Row).BioAccumRatePerYear
            ETinputdatafromEP.flow_to_det(Row) = Core.EcoPathGroupOutputs(Row).FlowToDet
            ETinputdatafromEP.net_efficiency(Row) = Core.EcoPathGroupOutputs(Row).NetEfficiency
            ETinputdatafromEP.fish_mort_rate(Row) = Core.EcoPathGroupOutputs(Row).MortCoFishRate
            ETinputdatafromEP.pred_mort_rate(Row) = Core.EcoPathGroupOutputs(Row).MortCoPredMort
            ETinputdatafromEP.net_migration_rate(Row) = Core.EcoPathGroupOutputs(Row).MortCoNetMig
            ETinputdatafromEP.other_mort_rate(Row) = Core.EcoPathGroupOutputs(Row).MortCoOtherMort

            ETinputdatafromEP.pp_input(Row) = Core.EcoPathGroupInputs(Row).PP
            ETinputdatafromEP.PROD_INPUT(Row) = Core.EcoPathGroupInputs(Row).PBInput
            ETinputdatafromEP.habitat_area_input(Row) = Core.EcoPathGroupInputs(Row).BiomassAreaInput
            ETinputdatafromEP.ee_input(Row) = Core.EcoPathGroupInputs(Row).EEInput
            ETinputdatafromEP.qb(Row) = Core.EcoPathGroupOutputs(Row).QBOutput
            ETinputdatafromEP.qb_input(Row) = Core.EcoPathGroupInputs(Row).QBInput
            ETinputdatafromEP.biom_acc_input(Row) = Core.EcoPathGroupInputs(Row).BioAccum
            ETinputdatafromEP.groupseq(Row) = Core.EcoPathGroupInputs(Row).Index
            ETinputdatafromEP.istanza(Row) = Core.EcoPathGroupInputs(Row).iStanza
        Next


        'System.Array.Copy(epdata.B, ETinputdatafromEP.B, epdata.B.Length)
        'System.Array.Copy(epdata.GroupName, ETinputdatafromEP.groupname, epdata.GroupName.Length)
        'System.Array.Copy(epdata.PB, ETinputdatafromEP.PROD, epdata.PB.Length)
        ' Rajout du search and replace pour les production, pour mettre à 0 les valeurs ecopath à -9999
        For compteur = 0 To UBound(ETinputdatafromEP.PROD)
            If ETinputdatafromEP.PROD(compteur) = -9999 Then ETinputdatafromEP.PROD(compteur) = 0
        Next

        'System.Array.Copy(epdata.TTLX, ETinputdatafromEP.TL, epdata.TTLX.Length)
        System.Array.Copy(epdata.FleetName, ETinputdatafromEP.fleetname, epdata.NumFleet + 1)

        'Récupération de l'index d'Omnivory
        'System.Array.Copy(epdata.BQB, ETinputdatafromEP.OI, epdata.BQB.Length)

        'System.Array.Copy(epdata.Area, ETinputdatafromEP.habitat_area, epdata.Area.Length)
        'System.Array.Copy(epdata.EE, ETinputdatafromEP.ee, epdata.EE.Length)

        'System.Array.Copy(epdata.BA, ETinputdatafromEP.biom_acc_rate, epdata.BA.Length)

        'System.Array.Copy(epdata.FlowToDet, ETinputdatafromEP.flow_to_det, epdata.FlowToDet.Length)

        'System.Array.Copy(epdata.GEff, ETinputdatafromEP.net_efficiency, epdata.GEff.Length)
        'C'est pas GEFF qui est par modele et pas par groupes

        'System.Array.Copy(epdata.pr, ETinputdatafromEP.net_migration_rate, epdata.OtherMortinput.Length)

        'System.Array.Copy(epdata.OtherMortinput, ETinputdatafromEP.other_mort_rate, epdata.OtherMortinput.Length)





        ETinputdatafromEP.numfleet = epdata.NumFleet
        ETinputdatafromEP.catches = New Single(epdata.NumFleet)() {}
        ETinputdata.catches = New Single(epdata.NumFleet)() {}
        ETinputdata.ModelName = epdata.ModelName
        ETinputdata.Modeldescription = epdata.ModelDescription


        For ifleet As Integer = 0 To epdata.NumFleet - 1
            ETinputdata.fleetname(ifleet) = Core.FleetInputs(ifleet + 1).Name
            ETinputdata.fleetseq(ifleet) = Core.FleetInputs(ifleet + 1).Index

            ETinputdatafromEP.catches(ifleet) = New Single(epdata.GroupName.Length) {}
            ETinputdata.catches(ifleet) = New Single(epdata.GroupName.Length) {}
            For j As Integer = 1 To epdata.B.Length - 1
                If (ETinputdatafromEP.accessibility(j) = 0 And (epdata.Landing(ifleet, j) > 0 Or epdata.Discard(ifleet, j) > 0)) Then ETinputdatafromEP.accessibility(j) = default_accessibility
                ETinputdatafromEP.catches(ifleet)(j) = epdata.Landing(ifleet + 1, j) + epdata.Discard(ifleet + 1, j)


            Next
        Next




    End Sub

    Private Function match(ByVal epdata As cEcopathDataStructures, ByVal p2 As String) As Array
        Throw New NotImplementedException
    End Function

    Public Sub UIContext(ByVal uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

End Class
