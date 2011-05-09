#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwEColumnHeader implements a column header with EwE style
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwEColumnHeaderCell
        : Inherits EwEHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cEwEGridColumnHeaderVisualizer()
        Private m_vizDefault As IVisualModel = Nothing

#Region " Construction / destruction "

        Public Sub New(Optional ByVal strValue As String = "")
            MyBase.New(strValue)
            Me.m_vizDefault = Me.VisualModel
            Me.VisualModel = g_visualizer
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As cStyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, New cStyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags)
            Me.New(New cVarnameTypeFormatter().GetDescriptor(varname, eDescriptorTypes.Name))
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, ByVal strUnitMask As String, ByVal unitType As cStyleGuide.eUnitType)
            Me.New(String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                 New cVarnameTypeFormatter().GetDescriptor(varname, eDescriptorTypes.Name), _
                                 strUnitMask), _
                   New cStyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New(String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                 New cVarnameTypeFormatter().GetDescriptor(varname, eDescriptorTypes.Name), _
                                 strUnitMask), _
                   aUnitTypes)
        End Sub

        Public Overrides Sub Dispose()
            Me.VisualModel = Me.m_vizDefault
            MyBase.Dispose()
        End Sub

#End Region ' Construction / destruction

    End Class

End Namespace
