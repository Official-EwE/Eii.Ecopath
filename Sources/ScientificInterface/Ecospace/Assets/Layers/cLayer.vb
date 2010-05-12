#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPData.Mapping
Imports SAUPUtil.Misc.Colours
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' <summary>
    ''' Class that wraps a single <see cref="cEcospaceLayer">Ecospace data layer</see> for 
    ''' manipulation in a User Interface.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Ok, the UI layer thing has gotten so complex that a bit of an explanation would not hurt.
    ''' </para>
    ''' <para>
    ''' The entire basemap chain consists of the following collaborating classes:
    ''' <list>
    ''' <item>
    ''' <description>One <see cref="cEcospaceBasemap">basemap</see> which defines the size and other aspects
    ''' of the map currently active in Ecospace. This class also provides access to individual data
    ''' <see cref="cEcospaceLayer">layers</see>.
    ''' </description></item>
    ''' <item><description>
    ''' <para>
    ''' Several <see cref="cEcospaceLayer">data layers</see> which each expose 
    ''' spatial array(s) of data. Basemap layers are two dimensional, allowing access to the array
    ''' via cell(row, col) interaction. Poking around in a basemap layer in fact modifies Ecospace
    ''' spatial cells of spatial data array that the the layer is connected to.
    ''' </para>
    ''' <para>
    ''' To obtain a layer from the basemap, call <see cref="cEcospaceBasemap.Layers">cEcospaceBasemap.Layers</see>
    ''' or one of the Layer-exposing properties of that class.
    ''' </para>
    ''' </description></item>
    ''' <item><description>
    ''' <para>
    ''' GUI <see cref="cLayer">Layers</see> combine one or more <see cref="cEcospaceLayer">core
    ''' basemap layers</see> as a single unit for display and interaction in the user interface. The GUI
    ''' Layer uses a <see cref="cLayerRenderer">layer renderer</see> to decide how this assembly
    ''' of core data is reflected.
    ''' </para>
    ''' <para>
    ''' To standardize the use of layers, the GUI provides a <see cref="cLayerFactory">factory</see>
    ''' class that delivers layers in a standardized way. To obtain one or more layers, call
    ''' <see cref="cLayerFactory.GetLayers">GetLayers</see>.
    ''' </para>
    ''' <para>
    ''' You can also create your own layers if need be, but try to use the factory whenever possible.
    ''' </para>
    ''' </description></item>
    ''' </list>
    ''' </para>
    ''' </remarks>
    Public Class cLayer
        Implements IDisposable

        ' ToDo_JS: build different name strategies, similar to property row header cells
        '          1. Via fixed string
        '          2. Via a property
        '          3. Add support for units

#Region " Private helper classes "

        ''' ===================================================================
        ''' <summary>
        ''' Default editor class for layers without an editor.
        ''' </summary>
        ''' ===================================================================
        Private Class cEditorLocked
            Inherits cLayerEditor

            Public Sub New()
                MyBase.New(Nothing)
            End Sub

            Public Overrides Property IsReadOnly() As Boolean
                Get
                    Return True
                End Get
                Set(ByVal value As Boolean)
                End Set
            End Property

        End Class

#End Region ' Private helper classes

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_mh As cMessageHandler = Nothing
        Private m_bDisposed As Boolean = False

        Private m_strName As String = ""
        Private m_source As cCoreInputOutputBase = Nothing
        Private m_varName As eVarNameFlags = eVarNameFlags.NotSet
        Private m_data As cEcospaceLayer = Nothing
        Private m_valueType As Type = GetType(Single)
        Private m_valueSet As Single = cCore.NULL_VALUE
        Private m_valueClear As Single = cCore.NULL_VALUE
        Private m_renderer As cLayerRenderer = Nothing
        Private m_editor As cLayerEditor = Nothing

        Private m_bAllowValidation As Boolean = True

        Private m_bSelected As Boolean = False
        Private m_bModified As Boolean = False
        Private m_bInUpdate As Boolean = False

        ''' <summary>
        ''' The <see cref="cProperty">property</see> that provides the name of this layer.
        ''' As well, this property is used to simulate live map changes to sync different
        ''' layer instances linked to the same data.
        ''' </summary>
        ''' <remarks>
        ''' This is a hack solution. <see cref="cEcospaceLayer">Basemap layers</see> are not exposed as true
        ''' <see cref="EwECore.ValueWrapper.cValue">core value objects</see>. To provide layers with common GUI issues
        ''' such as remark feedback and broadcasted updates, as well as the ability to attach 
        ''' <see cref="cVisualStyle">Visual Styles</see> to layers, a hidden property is used.
        ''' </remarks>
        Private m_propName As cProperty = Nothing

        Private m_aUnitTypes() As cStyleGuide.eUnitType = Nothing
        Private m_strUnitMask As String = ""

        ' --- shared defaults ---

        Private Shared s_editorLocked As New cEditorLocked()

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal data As cEcospaceLayer, _
                       ByVal renderer As cLayerRenderer, _
                       ByVal editor As cLayerEditor)
            Me.New(uic, data, renderer, editor, cCore.NULL_VALUE, cCore.NULL_VALUE, Nothing, Nothing)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' <param name="source">
        ''' The core object that serves two purposes:
        ''' <list type="number">
        ''' <item><description>Provide the dynamic name for a layer</description></item>
        ''' <item><description>Provide the definition for distributing data changes</description></item>
        ''' </list>
        ''' </param>
        ''' <param name="varName">The name of the variable to associate data changes with</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal data As cEcospaceLayer, _
                       ByVal renderer As cLayerRenderer, _
                       ByVal editor As cLayerEditor, _
                       ByVal source As cCoreInputOutputBase, _
                       Optional ByVal varName As eVarNameFlags = eVarNameFlags.Name)
            Me.New(uic, data, renderer, editor, cCore.NULL_VALUE, cCore.NULL_VALUE, source, varName)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' <param name="objValueSet"></param>
        ''' <param name="objValueClear"></param>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal data As cEcospaceLayer, _
                       ByVal renderer As cLayerRenderer, _
                       ByVal editor As cLayerEditor, _
                       ByVal objValueSet As Single, _
                       ByVal objValueClear As Single)
            Me.New(uic, data, renderer, editor, objValueSet, objValueClear, Nothing, Nothing)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="renderer"></param>
        ''' <param name="source">
        ''' The core object that serves two purposes:
        ''' <list type="number">
        ''' <item><description>Provide the dynamic name for a layer</description></item>
        ''' <item><description>Provide the definition for distributing data changes</description></item>
        ''' </list>
        ''' </param>
        ''' <param name="varName">The name of the variable to associate data changes with</param>
        ''' <param name="objValueSet"></param>
        ''' <param name="objValueClear"></param>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal data As cEcospaceLayer, _
                       ByVal renderer As cLayerRenderer, _
                       ByVal editor As cLayerEditor, _
                       ByVal objValueSet As Single, _
                       ByVal objValueClear As Single, _
                       ByVal source As cCoreInputOutputBase, _
                       Optional ByVal varName As eVarNameFlags = eVarNameFlags.Name)

            Debug.Assert(uic IsNot Nothing)

            Me.m_uic = uic
            Me.m_mh = New cMessageHandler(AddressOf EcospaceMessageHandler, eCoreComponentType.EcoSpace, eMessageType.DataModified, Me.m_uic.SyncObject)
            Me.m_uic.Core.Messages.AddMessageHandler(Me.m_mh)

            ' Sanity checks
            Debug.Assert(Not Object.ReferenceEquals(data, Nothing))

            If (editor Is Nothing) Then editor = cLayer.s_editorLocked

            Me.m_strName = ""
            Me.m_source = source
            Me.m_varName = varName
            Me.m_data = data
            Me.m_renderer = renderer
            Me.m_editor = editor
            Me.m_valueSet = objValueSet
            Me.m_valueClear = objValueClear
            Me.m_valueType = data.ValueType
            Me.m_propName = Me.m_uic.PropertyManager.GetProperty(source, varName)

            If (m_propName IsNot Nothing) Then
                AddHandler Me.m_propName.PropertyChanged, AddressOf OnPropertyChanged
            End If

            ' Update editor
            Me.m_editor.Initialize(uic, Me)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Copy constructor.
        ''' </summary>
        ''' <param name="layer">The layer to copy.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, ByVal layer As cLayer)

            Me.New(uic, layer.Data, layer.Renderer.Clone(), layer.Editor.Clone(), _
                   layer.ValueSet, layer.ValueClear, layer.Source, layer.VarName)

            Me.Name = layer.Name
            Me.IsSelected = layer.IsSelected
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="bDisposing"></param>
        ''' -----------------------------------------------------------------------
        Protected Overridable Sub Dispose(ByVal bDisposing As Boolean)
            If Not Me.m_bDisposed Then
                If bDisposing Then
                    If Me.m_uic IsNot Nothing Then
                        Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mh)
                    End If
                    If Me.m_propName IsNot Nothing Then
                        RemoveHandler Me.m_propName.PropertyChanged, AddressOf OnPropertyChanged
                        Me.m_propName = Nothing
                    End If
                End If
            End If
            Me.m_bDisposed = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region ' Construction / destruction

#Region " Public definitions "

        ''' <summary>
        ''' Enumerated type to indicate layer changes.
        ''' </summary>
        Public Enum eChangeFlags As Integer
            ''' <summary>Value to indicate that a layers' map data has changed.</summary>
            Map = 1
            ''' <summary>Value to indicate that a layers' visual representation style has changed.</summary>
            VisualStyle = 2
            ''' <summary>Value to indicate that a layers' visible state has changed.</summary>
            Visibility = 4
            ''' <summary>Value to indicate that a layers' selected state has changed.</summary>
            Selected = 8
            ''' <summary>Value to indicate that one ore more of a layers' name, description (?) 
            ''' and other descriptive values have changed.</summary>
            Descriptive = 16
            ''' <summary>Value to indicate that a layers' editable state has changed.</summary>
            Editable = 32
            ''' <summary>All possible flags.</summary>
            All = &HFFFF

        End Enum

        ''' <summary>
        ''' Layer change event
        ''' </summary>
        ''' <param name="layer"></param>
        ''' <param name="updateType"></param>
        Public Event LayerChanged(ByVal layer As cLayer, ByVal updateType As eChangeFlags)

#End Region ' Public definitions

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Instructs the layer to incorporate units the layer name display.
        ''' </summary>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field to place the layer name value, and a '{1}' field
        ''' to place the unit value.</param>
        ''' <param name="unitType">Definition of the unit to place in the layer
        ''' display text.</param>
        ''' -----------------------------------------------------------------------
        Public Sub SetUnitMask(ByVal strUnitMask As String, ByVal unitType As cStyleGuide.eUnitType)
            Me.SetUnitHeader(strUnitMask, New cStyleGuide.eUnitType() {unitType})
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Instructs the layer to incorporate units the layer name display.
        ''' </summary>
        ''' <param name="strUnitMask">The format mask to apply. This mask must
        ''' contain a '{0}' field to place the layer name value, and placeholder
        ''' fields for the units. The unit fields must be numbered '{1}', '{2}'
        ''' etc. Units will be placed in the placeholder fields in the order that
        ''' they are defined in <paramref name="aUnitTypes">aUnitTypes</paramref>.</param>
        ''' <param name="aUnitTypes">Definitions of units to place in the layer
        ''' display text.</param>
        ''' -----------------------------------------------------------------------
        Public Sub SetUnitMask(ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Call this whenever properties and visual aspects of the layer have changed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub Update(ByVal updateType As eChangeFlags)

            ' Prevent looped updates
            If Me.m_bInUpdate = True Then Return

            Me.m_bInUpdate = True

            ' Assess changes
            Try
                ' Map has changed via user drawing
                If ((updateType And eChangeFlags.Map) = eChangeFlags.Map) Then
                    ' Update visuals
                    Me.m_data.Invalidate()

                    ' Is a core layer?
                    If Me.m_data.DataType <> eDataTypes.NotSet Then
                        ' #Yes: inform the core
                        If (Me.m_uic IsNot Nothing) And (Me.AllowValidation) Then
                            Me.m_uic.Core.onChanged(Me.m_data)
                        End If
                    Else
                        ' #No: Fire off property change to make other copies of non-core layers respond
                        If (Me.m_propName IsNot Nothing) Then
                            Me.m_propName.FireChangeNotification(cProperty.eChangeFlags.Custom)
                        End If
                    End If

                End If

                If ((updateType And eChangeFlags.VisualStyle) = eChangeFlags.VisualStyle) Then
                    Me.m_renderer.Update()
                    If (Me.AllowValidation) Then
                        Me.m_renderer.VisualStyle.Update()
                    End If
                End If

                ' Inform the world last
                RaiseEvent LayerChanged(Me, updateType)

            Catch ex As Exception

            End Try

            Me.m_bInUpdate = False

        End Sub

#End Region ' Public access

#Region " Public properties "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of this layer.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Name() As String
            Get
                ' If no hard-wired name provided, obtain the name from the attached property
                If String.IsNullOrEmpty(Me.m_strName) And (Me.m_propName IsNot Nothing) Then
                    Return CStr(Me.m_propName.GetValue())
                End If
                Return Me.m_strName
            End Get
            Set(ByVal value As String)
                Me.m_strName = value
                Me.Update(eChangeFlags.Descriptive)
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the source of this layer.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Source() As cCoreInputOutputBase
            Get
                Return Me.m_source
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the variable of the source this layer applies to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_varName
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the underlying core-exposed layer data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Data() As cEcospaceLayer
            Get
                Return Me.m_data
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the value that this layer interprets as relevant values.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property ValueSet() As Single
            Get
                Return Me.m_valueSet
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the value that this layer interprets as clear values.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property ValueClear() As Single
            Get
                Return Me.m_valueClear
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get whether a given cell position has a value.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <param name="iCol"></param>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property HasValue(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
            Get
                If Object.ReferenceEquals(Me.m_valueSet, Nothing) Then Return False

                ' ToDo_JS: Build smartness to detect dimension of layer type
                Dim cellValue As Single = CSng(Me.Value(iRow, iCol))

                If Me.m_valueSet.Equals(cCore.NULL_VALUE) Then
                    If Me.m_valueClear.Equals(cCore.NULL_VALUE) Then
                        Return (cellValue <> cCore.NULL_VALUE)
                    Else
                        Return (cellValue <> 0)
                    End If
                End If
                Return Me.m_valueSet.Equals(cellValue)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value in the underlying data layer.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <param name="iCol"></param>
        ''' -----------------------------------------------------------------------
        Public Property Value(ByVal iRow As Integer, ByVal iCol As Integer) As Object
            Get
                Return Me.m_data.Cell(iRow, iCol)
            End Get
            Set(ByVal value As Object)
                Me.m_data.Cell(iRow, iCol) = CSng(value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the data type of values in the underlying data layer.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property ValueType() As Type
            Get
                Return Me.m_valueType
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the layer <see cref="cLayerRenderer">renderer</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Renderer() As cLayerRenderer
            Get
                Return Me.m_renderer
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the layer <see cref="cLayerEditor">editor</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Editor() As cLayerEditor
            Get
                Return Me.m_editor
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer is selected.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property IsSelected() As Boolean
            Get
                Return Me.m_bSelected
            End Get
            Set(ByVal value As Boolean)
                Me.m_bSelected = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer has been modified.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property IsModified() As Boolean
            Get
                Return Me.m_bModified
            End Get
            Set(ByVal value As Boolean)
                Me.m_bModified = value
                Me.m_data.Invalidate()
            End Set
        End Property

        Public Property AllowValidation() As Boolean
            Get
                Return Me.m_bAllowValidation
            End Get
            Set(ByVal value As Boolean)
                Me.m_bAllowValidation = value
            End Set
        End Property

#End Region ' Public properties

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' For core layers, the regular core DataModified messages relay layer 
        ''' updates.
        ''' </summary>
        ''' <param name="msg"></param>
        ''' -------------------------------------------------------------------
        Private Sub EcospaceMessageHandler(ByRef msg As cMessage)

            If msg.DataType = Me.m_data.DataType Then

                ' Prevent looped updates
                If Me.m_bInUpdate Then Return
                ' Trigger update
                Me.Update(eChangeFlags.Map)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' For non-core layers, a property change is used to trigger layer 
        ''' updates among independent copies of layers.
        ''' </summary>
        ''' <param name="prop"></param>
        ''' <param name="changeFlags"></param>
        ''' -------------------------------------------------------------------
        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

            ' Prevent looped updates
            If Me.m_bInUpdate Then Return

            ' Translate property change flags into layer change flags
            Dim flag As cLayer.eChangeFlags = 0

            ' Has the name or remark changed?
            If (changeFlags And (cProperty.eChangeFlags.Value Or cProperty.eChangeFlags.Remarks)) > 0 Then
                ' Send out layer name change event
                flag = flag Or eChangeFlags.Descriptive
            End If

            ' Using the property hacK?
            If (changeFlags And cProperty.eChangeFlags.Custom) > 0 Then
                ' Not so sure!
                flag = flag Or (eChangeFlags.All And (Not eChangeFlags.Map))
            End If

            If (flag <> 0) Then
                ' Trigger update
                Me.Update(flag)
            End If

        End Sub

#End Region ' Events

#Region " Internals "

        Protected Sub SetUnitHeader(ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

        Public Overridable ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""

                If (m_aUnitTypes Is Nothing) Or (String.IsNullOrEmpty(Me.m_strUnitMask)) Then
                    strDisplayText = Me.Name
                Else
                    Try
                        Dim sg As cStyleGuide = Me.m_uic.StyleGuide

                        Select Case m_aUnitTypes.Length
                            Case 0
                                strDisplayText = String.Format(Me.m_strUnitMask, Me.Name)
                            Case 1
                                strDisplayText = String.Format(Me.m_strUnitMask, Me.Name, _
                                                               sg.GetUnitString(m_aUnitTypes(0)))
                            Case 2
                                strDisplayText = String.Format(Me.m_strUnitMask, Me.Name, _
                                                               sg.GetUnitString(m_aUnitTypes(0)), _
                                                               sg.GetUnitString(m_aUnitTypes(1)))
                            Case Else
                                Debug.Assert(False)
                        End Select
                    Catch ex As Exception
                        Debug.Assert(False, "Failed to apply format mask, please check")
                        strDisplayText = Me.Name
                    End Try
                End If
                Return strDisplayText
            End Get
        End Property


#End Region ' Internals

    End Class ' Layer

End Namespace
