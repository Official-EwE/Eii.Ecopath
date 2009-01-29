'==============================================================================
'
' $Log: EwEGridCell.vb,v $
' Revision 1.6  2009/01/29 23:38:40  jeroens
' Added UnitCell
'
' Revision 1.5  2009/01/23 03:11:47  jeroens
' Prft
'
' Revision 1.4  2008/11/12 23:42:16  jeroens
' Bravo, Sherm
'
' Revision 1.3  2008/11/12 22:47:16  sherman
' Set resize to only widths
'
' Revision 1.2  2008/10/06 21:12:16  jeroens
' Status NULL cells are shown as blank
'
' Revision 1.1  2008/09/26 07:31:15  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

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

#Region " Class EwECellBase "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Base class for EwE SourceGrid2 Grid cells, implementing EwE GUI 
    ''' feedback.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class EwECellBase
        Inherits Cell

        ' JS 26Jan08: experimental new behaviour for EwECells: trap enter key
        Class CatchEnterPressBehaviour
            Inherits BehaviorModels.Common

            Public Overrides Sub OnKeyUp(ByVal e As SourceGrid2.PositionKeyEventArgs)
                If (e.KeyEventArgs.KeyCode = Keys.Enter) Then
                    Dim p As New Position((e.Position.Row + 1) Mod e.Grid.RowsCount, e.Position.Column)
                    e.Grid.SetFocusCell(p)
                End If
            End Sub

        End Class

#Region " Construction "

        ''' <summary>Default visualizer for EwECells</summary>
        Private Shared g_visualizer As New EwECellVisualizer()

        Public Sub New(ByVal objVal As Object, ByRef t As Type)
            MyBase.New(Nothing, t)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            ' Configure data model
            Me.DataModel.AllowNull = True
            ' Catch ENTER presses
            Me.Behaviors.Add(New CatchEnterPressBehaviour())
            ' Only resize width, not height of cells
            Me.Behaviors.Add(New SourceGrid2.BehaviorModels.Resize(CellResizeMode.Width))
        End Sub

#End Region ' Construction

#Region " Data (value, style, remarks) "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Custom cell style
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_style As StyleGuide.eStyleFlags = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Allows to set a custom <see cref="StyleGuide.eStyleFlags">style</see>,
        ''' triggering EwE colour feedback and EwE cell edit behaviour.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Style() As StyleGuide.eStyleFlags

            Get
                Return Me.m_style
            End Get

            Set(ByVal s As StyleGuide.eStyleFlags)
                Me.m_style = s
                If ((s And StyleGuide.eStyleFlags.NotEditable) = 0) Then
                    Me.DataModel.EnableEdit = True
                    Me.DataModel.EditableMode = SourceGrid2.EditableMode.Default
                Else
                    Me.DataModel.EnableEdit = False
                    Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
                End If
            End Set

        End Property

        Public Overrides Sub OnEditStarting(ByVal e As SourceGrid2.PositionCancelEventArgs)
            ' JS, 26aug08: Bug fix 502
            ' Safety catch, this method should be obsolete but *apparently* a double-click on
            ' disabled cells (EndableEdit and EditableMode locked down) still
            ' results into EditStarting!
            If ((Me.Style And StyleGuide.eStyleFlags.NotEditable) = StyleGuide.eStyleFlags.NotEditable) Then
                e.Cancel = True
            End If
            MyBase.OnEditStarting(e)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>If true, the cell will not show numerical '0' values.</summary>
        ''' -------------------------------------------------------------------
        Private m_bSuppressZero As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Zero value to suppress
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_sZeroValue As Single = 0.0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' When set to True, cells will not show numerical '0' values
        ''' </summary>
        ''' <param name="sZeroValue">A custom zero value, if applicable.</param>
        ''' -------------------------------------------------------------------
        Public Property SuppressZero(Optional ByVal sZeroValue As Single = 0.0) As Boolean
            Get
                Return Me.m_bSuppressZero
            End Get
            Set(ByVal bSuppress As Boolean)
                If (bSuppress <> Me.m_bSuppressZero) Then
                    Me.m_bSuppressZero = bSuppress
                    Me.m_sZeroValue = sZeroValue
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Number of decimal digits to display
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_iNumDigits As Integer = -1

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the number of decimal digits to display when formatting
        ''' numeric values. Set this value to a negative number to use the 
        ''' system-wide <see cref="StyleGuide.NumDigits">NumDigits</see> setting.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property NumDigits() As Integer
            Get
                Return Me.m_iNumDigits
            End Get
            Set(ByVal iNumDigits As Integer)
                If (iNumDigits <> Me.m_iNumDigits) Then
                    Me.m_iNumDigits = iNumDigits
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the text to display in the cell.
        ''' </summary>
        ''' <returns>The formatted value of the cell.</returns>
        ''' <remarks>
        ''' Real values will be formatted according
        ''' to the <see cref="StyleGuide.NumDigits">number of digits</see>
        ''' setting specified in the EwE <see cref="StyleGuide">StyleGuide</see>,
        ''' or via the local <see cref="NumDigits">NumDigits</see> override
        ''' if provided.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayText() As String
            Get

                Dim objValue As Object = Me.Value
                Dim tValue As Type = Me.DataModel.ValueType

                If ((Me.Style And StyleGuide.eStyleFlags.Null) > 0) Then
                    Return ""
                End If

                ' Is this a single?
                If (tValue Is GetType(Single)) Then
                    Dim sValue As Single = 0
                    Try
                        ' #Yes: apply format
                        sValue = CSng(Val(objValue))
                    Catch ex As Exception

                    End Try
                    ' Must suppress true zero?
                    If (Me.SuppressZero And (sValue = Me.m_sZeroValue)) Then
                        ' #Yes: return empty cell
                        Return ""
                    End If
                    Return Me.m_sg.FormatNumber(sValue, Me.Style, Me.m_iNumDigits)
                End If

                ' Is this a double?
                If (tValue Is GetType(Double)) Then
                    ' #Yes: apply format
                    Dim dValue As Double = 0
                    Try
                        dValue = CDbl(Val(objValue))
                    Catch ex As Exception

                    End Try
                    ' Must suppress true zero?
                    If (Me.SuppressZero And (dValue = CDbl(Me.m_sZeroValue))) Then
                        ' #Yes: return empty cell
                        Return ""
                    End If
                    Return Me.m_sg.FormatNumber(dValue, Me.Style, Me.m_iNumDigits)
                End If

                ' Is this an integer?
                If (tValue Is GetType(Integer)) Then
                    ' #Yes: apply format
                    Dim iValue As Integer = 0
                    Try
                        iValue = CInt(Val(objValue))
                    Catch ex As Exception

                    End Try
                    ' Must suppress true zero?
                    If (Me.SuppressZero And (iValue = CInt(Me.m_sZeroValue))) Then
                        ' #Yes: return empty cell
                        Return ""
                    End If
                    Return CStr(iValue)
                End If

                ' Return value as-is
                Return CStr(objValue)

            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure a cell by examining <see cref="cVariableMetaData">variable meta data</see>.
        ''' </summary>
        ''' <param name="md">The <see cref="cVariableMetaData">variable meta data</see> to examine.</param>
        ''' -------------------------------------------------------------------
        Public Sub ConfigureCell(ByVal md As cVariableMetaData)

            ' Sanity check
            If md Is Nothing Then Return
            ' Set default val
            Me.DataModel.DefaultValue = md.NullValue

            ' Do not set min and max; the default may be out of range and this will massively confuse the grid engine
            'Me.DataModel.MinimumValue = md.Min
            'Me.DataModel.MaximumValue = md.Max

        End Sub

#End Region ' Data (value, style, remarks)

#Region " Updates (StyleGuide)"

        ''' <summary>StyleGuide instance for subscribing to events</summary>
        Protected WithEvents m_sg As StyleGuide = StyleGuide.GetInstance()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' StyleGuide change event handler; makes sure cells are redrawn
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overridable Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType) Handles m_sg.StyleGuideChanged
            Me.Invalidate()
        End Sub

#End Region ' Updated (StyleGuide)

    End Class

#End Region ' Class EwECellBase

#Region " Static cells "

#Region " Class EwECell "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A standard EwE grid cell for static values.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class EwECell
        : Inherits EwECellBase

#Region " Construction "

        Public Sub New(ByVal objVal As Object, ByRef t As Type)
            MyBase.New(objVal, t)
            ' Set value
            If objVal IsNot Nothing Then Me.Value = objVal
        End Sub

#End Region ' Construction 

#Region " Data "


        ''' -------------------------------------------------------------------
        ''' <summary>Locally maintained value.</summary>
        ''' -------------------------------------------------------------------
        Private m_objValue As Object = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commonly called in response to end edit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub SetValue(ByVal p_Position As SourceGrid2.Position, ByVal p_Value As Object)
            Me.Value = p_Value
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the locally maintained value.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Value() As Object
            Get
                Return Me.m_objValue
            End Get
            Set(ByVal objValue As Object)
                Me.m_objValue = objValue
            End Set
        End Property

#End Region ' Data

    End Class

#End Region ' Class EwECell

#Region " Class EwEHeaderCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwERowHeaderCell implements a Common cell rendered as an EwE name field.
    ''' EwERowHeaderCells are used in EwE to replace Row headers which values are statically
    ''' set.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class EwEHeaderCell
        : Inherits EwECell

#Region " Construction "

        Public Sub New(Optional ByVal objValue As Object = Nothing)
            MyBase.New(objValue, GetType(String))
            ' Disable edit
            Me.DataModel.EnableEdit = False
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

#Region " Data (style) "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that header cells use names colour feedback
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return (StyleGuide.eStyleFlags.Names Or StyleGuide.eStyleFlags.NotEditable Or MyBase.Style)
            End Get
            Set(ByVal styleNew As StyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or StyleGuide.eStyleFlags.Names Or StyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Data (style) 

#Region " Unit header text "

        Protected m_aUnitTypes() As StyleGuide.eUnitType
        Protected m_strUnitMask As String = ""

        Public Sub SetUnitHeader(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            ' Sanity checks
            Debug.Assert(aUnitTypes.Length = 1 Or aUnitTypes.Length = 2)

            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""

                If m_aUnitTypes Is Nothing Then
                    strDisplayText = MyBase.DisplayText
                Else
                    Select Case m_aUnitTypes.Length
                        Case 0
                            strDisplayText = MyBase.DisplayText
                        Case 1
                            strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)))
                        Case 2
                            strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)), GetUnitString(m_aUnitTypes(1)))
                        Case Else
                            Debug.Assert(False)
                    End Select
                End If
                Return strDisplayText
            End Get
        End Property

        Private Function GetUnitString(ByVal unitType As StyleGuide.eUnitType) As String
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim strUnitString As String = ""
            Select Case unitType
                Case StyleGuide.eUnitType.Currency
                    strUnitString = sg.CurrencyUnitText(sg.CurrencyUnit)
                Case StyleGuide.eUnitType.Time
                    strUnitString = sg.TimeUnitText(sg.TimeUnit)
                Case StyleGuide.eUnitType.Monetary
                    strUnitString = sg.MonetaryUnitText(sg.MonetaryUnit)
                Case Else
                    Debug.Assert(False)
            End Select
            Return strUnitString
        End Function

#End Region ' Unit header text

    End Class

#End Region ' Class EwEHeaderCell

#Region " Class EwERowHeaderCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwERowHeaderCell implements a EwERowHeaderCell to implement row headers. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EwERowHeaderCell
        : Inherits EwEHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cVisualizerEwERowHeader()

#Region " Construction "

        Public Sub New(Optional ByVal objValue As Object = Nothing)
            MyBase.New(objValue)
            ' Set visualizer
            Me.VisualModel = g_visualizer
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New()
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New()
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

    End Class

#End Region ' Class EwERowHeaderCell 

#Region " Class EwEColumnHeaderCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwEColumnHeader implements a column header with EwE style
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EwEColumnHeaderCell
        : Inherits EwEHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New EwEColumnHeaderVisualizer()

#Region " Construction "

        Public Sub New(Optional ByVal objValue As Object = Nothing)
            MyBase.New(objValue)
            Me.VisualModel = g_visualizer
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

    End Class

#End Region ' Class EwEColumnHeaderCell 

#End Region ' Static cells

#Region " Dynamic cells "

#Region " Class PropertyCell "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A standard EwE grid cell for <see cref="cProperty">cProperty</see>-driven values.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class PropertyCell
        : Inherits EwECellBase

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or Nothing when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec))
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
                Me.onPropertyChanged(prop, cProperty.eChangeFlags.All)
            End If
        End Sub

#End Region ' Construction 

#Region " Data (property)"

        ''' <summary>Connected property.</summary>
        Private WithEvents m_property As cProperty = Nothing

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
            If (Me.Style And StyleGuide.eStyleFlags.NotEditable) = StyleGuide.eStyleFlags.NotEditable Then Return
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
        ''' Allows to set a custom cell <see cref="StyleGuide.eStyleFlags">style</see>,
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
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Dim s As StyleGuide.eStyleFlags = MyBase.Style
                If s = 0 Then Return Me.m_property.GetStyle()
                Return s
            End Get
            Set(ByVal s As StyleGuide.eStyleFlags)
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
        Private Sub onPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags) Handles m_property.PropertyChanged

            ' Sanity checks
            Debug.Assert(prop IsNot Nothing, "Invalid event received")
            Debug.Assert(Object.Equals(prop, Me.m_property), "Event received for invalid property")

            ' Check style flag changes
            If (changeFlags And cProperty.eChangeFlags.CoreStatus) = cProperty.eChangeFlags.CoreStatus Then
                ' Update read-only state
                Me.DataModel.EnableEdit = ((prop.GetStyle() And StyleGuide.eStyleFlags.NotEditable) = 0)
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

#End Region ' Class PropertyCell 

#Region " Class PropertyHeaderCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyHeaderCell implements a PropertyCell based class for creating 
    ''' header cells.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class PropertyHeaderCell
        : Inherits PropertyCell

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop"><see cref="cProperty">Property</see> to attach to the cell.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(prop)
            ' Always
            Me.DataModel.EnableEdit = False
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, instructing the cell to use a unit mask.
        ''' </summary>
        ''' <param name="prop"><see cref="cProperty">Property</see> to attach to the cell.</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a
        ''' <see cref="StyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="StyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, instructing the cell to use a unit mask.
        ''' </summary>
        ''' <param name="prop"><see cref="cProperty">Property</see> to attach to the cell.</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="StyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="StyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a
        ''' <see cref="StyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="StyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="StyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="StyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

#Region " Data (style) "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that header cells use names colour feedback
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return (MyBase.Style Or StyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(ByVal styleNew As StyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or StyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Data (style) 

#Region " Unit header text "

        Protected m_aUnitTypes() As StyleGuide.eUnitType
        Protected m_strUnitMask As String = ""

        Public Sub SetUnitHeader(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            ' Sanity checks
            Debug.Assert(aUnitTypes.Length = 1 Or aUnitTypes.Length = 2)

            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""

                If (m_aUnitTypes Is Nothing) Or (String.IsNullOrEmpty(Me.m_strUnitMask)) Then
                    strDisplayText = MyBase.DisplayText
                Else
                    Select Case m_aUnitTypes.Length
                        Case 0
                            strDisplayText = String.Format(MyBase.DisplayText, Me.Value)
                        Case 1
                            strDisplayText = String.Format(Me.m_strUnitMask, Me.Value, GetUnitString(m_aUnitTypes(0)))
                        Case 2
                            strDisplayText = String.Format(Me.m_strUnitMask, Me.Value, GetUnitString(m_aUnitTypes(0)), GetUnitString(m_aUnitTypes(1)))
                        Case Else
                            Debug.Assert(False)
                    End Select
                End If
                Return strDisplayText
            End Get
        End Property

        Private Function GetUnitString(ByVal unitType As StyleGuide.eUnitType) As String
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim strUnitString As String = ""
            Select Case unitType
                Case StyleGuide.eUnitType.Currency
                    strUnitString = sg.CurrencyUnitText(sg.CurrencyUnit)
                Case StyleGuide.eUnitType.Time
                    strUnitString = sg.TimeUnitText(sg.TimeUnit)
                Case StyleGuide.eUnitType.Monetary
                    strUnitString = sg.MonetaryUnitText(sg.MonetaryUnit)
                Case Else
                    Debug.Assert(False)
            End Select
            Return strUnitString
        End Function

#End Region ' Unit header text

    End Class

#End Region ' Class PropertyHeaderCell 

#Region " Class PropertyRowHeaderCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyRowHeaderCell implements a PropertyCellBase to implement a row
    ''' header that dynamically derives its value from the core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class PropertyRowHeaderCell
        : Inherits PropertyHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As EwECellVisualizerBase

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(prop)
            Me.VisualModel = New cVisualizerEwERowHeader
        End Sub

        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a
        ''' <see cref="StyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="StyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="StyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="StyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

    End Class

#End Region ' Class PropertyRowHeaderCell 

#Region " Class PropertyColumnHeaderCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyColumnHeaderCell implements a PropertyCellBase rendered as clickable,
    ''' sortable column header.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class PropertyColumnHeaderCell
        : Inherits PropertyHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New EwEColumnHeaderVisualizer()

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            MyBase.New(prop)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            If prop.VarName <> eVarNameFlags.Name Then Me.ToolTipText = prop.Source().Name
        End Sub

        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal prop As cProperty, ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(prop)
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or Nothing when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec))
        End Sub
        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a
        ''' <see cref="StyleGuide.eUnitType">unit of measurement</see> into
        ''' the cell value.</param>
        ''' <param name="unitType">The <see cref="StyleGuide.eUnitType">unit of measurement</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal unitType As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">Secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' <param name="strUnitMask">Mask that specifies how to substitute a series
        ''' of <see cref="StyleGuide.eUnitType">unit of measurements</see> into
        ''' the cell value.</param>
        ''' <param name="aUnitTypes">The <see cref="StyleGuide.eUnitType">unit of measurements</see>
        ''' to substitute into the header cell text.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                ByVal SourceSec As cCoreInputOutputBase, _
                ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec), strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

    End Class

#End Region ' Class PropertyColumnHeaderCell 

#Region " UnitCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' UnitCell implements a cell that shows a dynamic unit string.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class UnitCell
        : Inherits EwECell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As EwECellVisualizerBase
        Protected m_aUnitTypes() As StyleGuide.eUnitType
        Protected m_strUnitMask As String = ""


#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal unitType As StyleGuide.eUnitType)
            Me.New("{0}", New StyleGuide.eUnitType() {unitType})
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As StyleGuide.eUnitType)
            MyBase.New(Nothing, GetType(String))

            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""

                If Me.m_aUnitTypes IsNot Nothing Then

                    Select Case m_aUnitTypes.Length
                        Case 0
                            ' NOP
                        Case 1
                            strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)))
                        Case 2
                            strDisplayText = String.Format(Me.m_strUnitMask, GetUnitString(m_aUnitTypes(0)), GetUnitString(m_aUnitTypes(1)))
                        Case Else
                            Debug.Assert(False)
                    End Select

                End If

                Return strDisplayText
            End Get
        End Property

        Private Function GetUnitString(ByVal unitType As StyleGuide.eUnitType) As String
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim strUnitString As String = ""
            Select Case unitType
                Case StyleGuide.eUnitType.Currency
                    strUnitString = sg.CurrencyUnitText(sg.CurrencyUnit)
                Case StyleGuide.eUnitType.Time
                    strUnitString = sg.TimeUnitText(sg.TimeUnit)
                Case StyleGuide.eUnitType.Monetary
                    strUnitString = sg.MonetaryUnitText(sg.MonetaryUnit)
                Case Else
                    Debug.Assert(False)
            End Select
            Return strUnitString
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that this cell cannot be edited.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return (MyBase.Style Or StyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(ByVal styleNew As StyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or StyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Construction 

    End Class

#End Region ' UnitCell

#End Region ' Dynamic cells

End Namespace
