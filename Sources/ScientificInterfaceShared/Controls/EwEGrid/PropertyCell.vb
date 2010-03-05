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

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A standard EwE grid cell for <see cref="cProperty">cProperty</see>-driven values.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class PropertyCell
        : Inherits EwECellBase

        ''' <summary>Connected property.</summary>
        Private m_property As cProperty = Nothing

#Region " Construction and destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="pm"><see cref="cPropertyManager">Property manager</see> to extract data from.</param>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or Nothing when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal source As cCoreInputOutputBase, _
                       ByVal varname As eVarNameFlags, _
                       Optional ByVal sourceSec As cCoreInputOutputBase = Nothing)
            Me.New(pm.GetProperty(source, varname, sourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="prop">The property to assign to the cell.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(Nothing, prop.GetValueType())
            ' Store the property
            ' Set the property
            Me.m_property = prop
            ' Valid assignment?
            If (prop IsNot Nothing) Then
                ' Configure the cell
                Me.ConfigureCell(prop.GetVariableMetadata())
                ' Fire a change notification
                Me.OnPropertyChanged(prop, cProperty.eChangeFlags.All)
                ' Register property
                AddHandler Me.m_property.PropertyChanged, AddressOf Me.OnPropertyChanged
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clean up!
        ''' </summary>
        ''' <param name="bDisposing">Yeah, we're really doing this.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)

            ' Unregister property
            If (Me.m_property IsNot Nothing) Then
                RemoveHandler Me.m_property.PropertyChanged, AddressOf Me.OnPropertyChanged
                Me.m_property = Nothing
            End If

            MyBase.Dispose(bDisposing)

        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

#End Region ' Construction 

#Region " Data (property)"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the property in the cell
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetProperty() As cProperty
            Return Me.m_property
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commonly called in response to end edit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub SetValue(ByVal p_Position As SourceGrid2.Position, ByVal p_Value As Object)
            ' Sanity check
            If (Me.Style And cStyleGuide.eStyleFlags.NotEditable) = cStyleGuide.eStyleFlags.NotEditable Then Return
            ' Apply edited value
            Me.Value = p_Value
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to access the value maintained by the property
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Value() As Object
            Get

                ' Does property exist?
                If (m_property IsNot Nothing) Then
                    ' #Yes: return value
                    Return m_property.GetValue()
                End If
                ' #No: return default
                Return Nothing

            End Get
            Set(ByVal value As Object)

                Dim bChanged As Boolean = True

                ' Does property exist?
                If (m_property IsNot Nothing) Then
                    ' #Yes: update the property. The property will take care of dispatching any changes
                    bChanged = m_property.SetValue(value, TriState.UseDefault)
                End If

                ' Anything changed?
                If (bChanged) Then
                    ' #Yes: redraw the cell
                    Me.Invalidate()
                End If

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Allows to set a custom cell <see cref="cStyleGuide.eStyleFlags">style</see>,
        ''' overriding any style in the attached property.
        ''' </summary>
        ''' <remarks>
        ''' <para>Note that this style will not affect the cProperty. Unlike values, which
        ''' can be modified from both core and GUI, Styles are interpreted core status 
        ''' calculations.</para>
        ''' <para>To use a custom Style on a per-cell basis, use <see cref="EwECell.Style">EwECell.Style</see></para>
        ''' <para>To use a custom Style on a system-wide basis for a particular cProperty,
        ''' modify the <see cref="cProperty.SetStyle">Style</see> in the instance of the cProperty.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Property Style() As cStyleGuide.eStyleFlags
            Get
                Dim s As cStyleGuide.eStyleFlags = MyBase.Style
                If s = 0 Then Return Me.m_property.GetStyle()
                Return s
            End Get
            Set(ByVal s As cStyleGuide.eStyleFlags)
                MyBase.Style = s
            End Set
        End Property

#End Region ' Data (property)

#Region " Updates (property) "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Property change event handler. Invoked when the property attached 
        ''' to this cell has changed.
        ''' </summary>
        ''' <param name="prop">The <see cref="cProperty">property</see> that changed.</param>
        ''' <param name="changeFlags">Bitwise flag that states what <see cref="cProperty.eChangeFlags">aspect</see>
        ''' of the property has changed.</param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

            ' Sanity checks
            Debug.Assert(prop IsNot Nothing, "Invalid event received")

            ' Ignore events for other properties (could be that a derived class is frolicking with strangers, eek)
            If Not Object.Equals(prop, Me.m_property) Then Return

            ' Check style flag changes
            If (changeFlags And cProperty.eChangeFlags.CoreStatus) = cProperty.eChangeFlags.CoreStatus Then
                ' Update read-only state
                Me.DataModel.EnableEdit = ((prop.GetStyle() And cStyleGuide.eStyleFlags.NotEditable) = 0)
            End If

            ' Check for remark changes
            If (changeFlags And cProperty.eChangeFlags.Remarks) = cProperty.eChangeFlags.Remarks Then
                Me.ToolTipText = prop.GetRemark()
            End If

            ' Redraw the cell
            Me.Invalidate()

        End Sub

#End Region ' Updates (property)

    End Class

End Namespace
