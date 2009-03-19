' =============================================================================
'
' $Log: cPropertyFormatProvider.vb,v $
' Revision 1.1  2009/03/19 16:00:33  jeroens
' Split files
'
' =============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Properties

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cEwEFormatProvider">cEwEFormatProvider</see> that is driven
    ''' by a <see cref="cProperty">cProperty</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPropertyFormatProvider
        Inherits cEwEFormatProvider

        ''' <summary>Property that serves as data and style source.</summary>
        Private m_prop As cProperty = Nothing
        ''' <summary>The wrapped control</summary>
        Private m_ctrl As Control = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctrl"></param>
        ''' <param name="source"></param>
        ''' <param name="varName"></param>
        ''' <param name="sourceSec"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal ctrl As Control, _
                       ByVal source As cCoreInputOutputBase, _
                       ByVal varName As eVarNameFlags, Optional ByVal sourceSec As cCoreInputOutputBase = Nothing, _
                       Optional ByVal aItems As Object() = Nothing)

            ' Get underlying cProperty for these values
            Me.New(ctrl, cPropertyManager.GetInstance().GetProperty(source, varName, sourceSec), aItems)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctrl"></param>
        ''' <param name="prop"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal ctrl As Control, _
                       ByVal prop As cProperty, _
                       Optional ByVal aItems As Object() = Nothing)

            MyBase.New(ctrl, prop.GetValueType(), aItems, prop.GetVariableMetadata())

            ' Store relevant bits
            Me.m_prop = prop
            AddHandler Me.m_prop.PropertyChanged, AddressOf OnPropertyChanged

            Me.m_ctrl = ctrl
            If (TypeOf (Me.m_ctrl) Is Control) Then
                AddHandler DirectCast(Me.m_ctrl, Control).Enter, AddressOf OnGotFocus
            End If

            ' Fire change event manually to immediately show the property value
            Me.OnPropertyChanged(Me.m_prop, cProperty.eChangeFlags.All)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Release the format provider from the wrapped control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Release()

            If Me.m_prop IsNot Nothing Then
                RemoveHandler Me.m_prop.PropertyChanged, AddressOf OnPropertyChanged
                Me.m_prop = Nothing
            End If

            If Me.m_ctrl IsNot Nothing Then
                If (TypeOf (Me.m_ctrl) Is Control) Then
                    RemoveHandler DirectCast(Me.m_ctrl, Control).Enter, AddressOf OnGotFocus
                End If
                Me.m_ctrl = Nothing
            End If

            MyBase.Release()

        End Sub

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the style to reflect in the TextBox, overriding the style
        ''' dictated by the underlying cProperty.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Dim eStyle As StyleGuide.eStyleFlags = MyBase.Style()
                If (eStyle <> StyleGuide.eStyleFlags.OK) Then Return eStyle
                If Me.m_prop Is Nothing Then Return Nothing
                Return Me.m_prop.GetStyle()
            End Get
            Set(ByVal eStyle As StyleGuide.eStyleFlags)
                MyBase.Style = eStyle
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value maintained in the underlying Property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Value() As Object
            Get
                If Me.m_prop Is Nothing Then Return Nothing
                Return Me.m_prop.GetValue()
            End Get
            Set(ByVal value As Object)
                If Me.m_prop Is Nothing Then Return
                Me.m_prop.SetValue(value, TriState.UseDefault)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Refresh()
            Me.m_prop.Refresh()
        End Sub

#End Region ' Data

#Region " Local events "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, invoked when the source Property <see cref="cProperty.PropertyChanged">changes</see>.
        ''' </summary>
        ''' <param name="prop">The property that changed.</param>
        ''' <param name="changeFlags">The type of change.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

            ' Sanity check
            Debug.Assert(Object.ReferenceEquals(prop, Me.m_prop))

            ' Update control
            If (changeFlags And (cProperty.eChangeFlags.CoreStatus Or cProperty.eChangeFlags.Value)) > 0 Then
                ' Get new content
                Me.UpdateContent()
            End If

            ' Update tooltip
            If ((changeFlags And cProperty.eChangeFlags.Remarks) > 0) Then
                If (TypeOf (Me.m_ctrl) Is Control) Then
                    cToolTipShared.GetInstance().SetToolTip(DirectCast(Me.m_ctrl, Control), prop.GetRemark())
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the wrapped control receives focus. Handled to fire 
        ''' an application-wide <see cref="PropertySelectionCommand">PropertySelectionCommand</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnGotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim dsc As PropertySelectionCommand = DirectCast(cmdh.GetCommand(PropertySelectionCommand.COMMAND_NAME), PropertySelectionCommand)

            If Object.ReferenceEquals(dsc, Nothing) Then Return

            dsc.Invoke(Me.m_prop)
        End Sub

#End Region ' Local events 

    End Class ' cPropertyFormatProvider

End Namespace ' Controls
