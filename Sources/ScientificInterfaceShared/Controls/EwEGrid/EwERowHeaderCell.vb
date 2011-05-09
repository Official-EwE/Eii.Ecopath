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
    ''' EwERowHeaderCell implements a EwERowHeaderCell to implement row headers. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwERowHeaderCell
        : Inherits EwEHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cEwEGridRowHeaderVisualizer()

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell with an optional static value.
        ''' </summary>
        ''' <param name="strValue">The value to set.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(Optional ByVal strValue As String = "")
            MyBase.New(strValue)
            ' Set visualizer
            Me.VisualModel = g_visualizer
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell displaying a single unit.
        ''' </summary>
        ''' <param name="strUnitMask">The mask should contain ONE {0} placeholder where
        ''' the <paramref name="unitType">unit</paramref> will be displayed.</param>
        ''' <param name="unitType">The <see cref="cStyleGuide.eUnitType">unit</see>
        ''' to dynamically substitute in the cell display text.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal strUnitMask As String, ByVal unitType As cStyleGuide.eUnitType)
            Me.New(strUnitMask, New cStyleGuide.eUnitType() {unitType})
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell displaying a series of units.
        ''' </summary>
        ''' <param name="strUnitMask">The mask should contain a string format 
        ''' placeholder for each <paramref name="aunitTypes">unit</paramref>.</param>
        ''' <param name="aUnitTypes">The <see cref="cStyleGuide.eUnitType">units</see>
        ''' to dynamically substitute in the cell display text.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New()
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

#End Region ' Construction 

    End Class

End Namespace
