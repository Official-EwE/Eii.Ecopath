#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D

Imports EwECore
Imports ScientificInterface.Other

#End Region ' Imports

Namespace Ecosim

    Public Class dlgApplyShape

        Public Enum eEditMode As Integer
            ''' <summary>Dialog opened for a single pred/prey combination.</summary>
            PredPrey = 0
            ''' <summary>Dialog opened for all diets involving this unfortunate prey.</summary>
            Prey
            ''' <summary>Dialog opened for all diets of a predator.</summary>
            Predator
            ''' <summary>Dialog opened for all diets.</summary>
            All
        End Enum

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_PPIManager As cPPIManager
        Private m_lPPI As New List(Of cPredPreyInteraction)
        Private m_lFFs As New List(Of cForcingFunction)

        Private m_iSelPrey As Integer = -1
        Private m_strSelPreyName As String = ""
        Private m_iSelPredIndex As Integer = -1
        Private m_strSelPredName As String = ""

        ''' <summary>Image list used for displaying large thumbnails.</summary>
        Private m_ilLarge As New ImageList()
        ''' <summary>Image list used for displaying small thumbnails.</summary>
        Private m_ilSmall As New ImageList()

        Private m_editMode As eEditMode = eEditMode.PredPrey
        Private m_nGroups As Integer = 0

        Private m_shapeMode As eApplyShapeTypes = eApplyShapeTypes.NotSet
        Private m_targetType As eApplyTargetTypes = eApplyTargetTypes.NotSet

#End Region ' Private vars

        Public Sub New(ByVal uic As cUIContext, _
                       ByVal iPrey As Integer, ByVal iPred As Integer, _
                       ByVal shapeType As eApplyShapeTypes, ByVal targetType As eApplyTargetTypes)

            Try

                Me.Init(uic, eEditMode.PredPrey, shapeType, targetType)

                ' the index for selected prey and predator index
                Me.m_iSelPrey = iPrey
                Me.m_iSelPredIndex = iPred

                'Set the Prey and Predator name from index here. They are not editable
                m_strSelPreyName = Me.m_uic.Core.EcoPathGroupInputs(m_iSelPrey).Name
                m_strSelPredName = Me.m_uic.Core.EcoPathGroupInputs(m_iSelPredIndex).Name

                m_lPPI.Add(m_PPIManager.Interaction(m_iSelPredIndex, m_iSelPrey))

            Catch ex As Exception
                ' NOP
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Created the dialog for a single predator or prey.
        ''' </summary>
        ''' <param name="iGroup">Group this dialog was opened for.</param>
        ''' <param name="editMode">Flag stating how this group should be interpreted.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal iGroup As Integer, ByVal editMode As eEditMode, _
                       ByVal shapeType As eApplyShapeTypes, ByVal targetType As eApplyTargetTypes)

            Init(uic, editMode, shapeType, targetType)

            Select Case editMode

                Case eEditMode.Prey
                    m_iSelPrey = iGroup
                    m_strSelPreyName = Me.m_uic.Core.EcoPathGroupInputs(m_iSelPrey).Name

                    For i As Integer = 1 To m_nGroups
                        If m_PPIManager.isPredPrey(i, m_iSelPrey) Then
                            m_lPPI.Add(m_PPIManager.Interaction(i, m_iSelPrey))
                        End If
                    Next

                Case eEditMode.Predator
                    m_iSelPredIndex = iGroup
                    m_strSelPredName = Me.m_uic.Core.EcoPathGroupInputs(m_iSelPredIndex).Name

                    For i As Integer = 1 To m_nGroups
                        If m_PPIManager.isPredPrey(m_iSelPredIndex, i) Then
                            m_lPPI.Add(m_PPIManager.Interaction(m_iSelPredIndex, i))
                        End If
                    Next

                Case Else
                    Debug.Assert(False, String.Format("Invalid editmode {0} provided, expected Pred or Prey", editMode.ToString))

            End Select

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create the dialog for all diets
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal shapeType As eApplyShapeTypes, _
                       ByVal targetType As eApplyTargetTypes)

            Init(uic, eEditMode.All, shapeType, targetType)

            For iPred As Integer = 1 To Me.m_uic.Core.nLivingGroups
                ' For each row (rowIndex - Prey)
                For iPrey As Integer = 1 To Me.m_uic.Core.nGroups
                    ' Can assign FF at this spot in the matrix?
                    If m_PPIManager.isPredPrey(iPred, iPrey) Then
                        m_lPPI.Add(m_PPIManager.Interaction(iPred, iPrey))
                    End If
                Next
            Next
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Debug.Assert(Me.m_uic IsNot Nothing)

            LoadAvailableShapes()
            LoadMultiplierOption()
            LoadAppliedShapes()

            ' Load Prey and predator pair name
            Select Case m_editMode
                Case eEditMode.PredPrey
                    txbPreyName.Text = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, m_iSelPrey, m_strSelPreyName)
                    txbPredName.Text = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, m_iSelPredIndex, m_strSelPredName)
                Case eEditMode.Prey
                    lblTitle.Text = String.Format(My.Resources.ECOSIM_PROMPT_APPLY_SHAPES_PREY, m_iSelPrey, m_strSelPreyName)
                Case eEditMode.Predator
                    lblTitle.Text = String.Format(My.Resources.ECOSIM_PROMPT_APPLY_SHAPES_PRED, m_iSelPredIndex, m_strSelPredName)
                Case eEditMode.All
                    lblTitle.Text = String.Format(My.Resources.ECOSIM_PROMPT_APPLY_SHAPES_ALL)
            End Select

            UpdateControls()

        End Sub

        Private Sub Init(ByVal uic As cUIContext, _
                         ByVal editMode As eEditMode, _
                         ByVal shapeType As eApplyShapeTypes, _
                         ByVal targetType As eApplyTargetTypes)

            Me.InitializeComponent()

            Me.m_uic = uic

            ' Get the Prey - Pred interaction manager
            Me.m_PPIManager = Me.m_uic.Core.PPInteractionManager

            Me.m_editMode = editMode
            Me.m_shapeMode = shapeType
            Me.m_targetType = targetType

            ' Set title
            Select Case Me.m_shapeMode
                Case eApplyShapeTypes.Forcing
                    Me.Text = My.Resources.ECOSIM_CAPTION_APPLYFF
                Case eApplyShapeTypes.Mediation
                    Me.Text = My.Resources.ECOSIM_CAPTION_APPLYMED
                Case Else
                    Debug.Assert(False, String.Format("Mode {0} not supported by dialog", Me.m_shapeMode.ToString()))
            End Select

            ' Get the available forcing and mediation shapes which can be applied to this Prey-Pred pair.
            For i As Integer = 1 To m_PPIManager.NShapes
                m_lFFs.Add(m_PPIManager.Shapes(i))
            Next

            Me.GenerateImages()

            Me.m_nGroups = Me.m_uic.Core.nGroups
            Me.m_lPPI.Clear()

        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

            Dim iNumApplied As Integer = lvAppliedShapes.Items.Count
            Dim lvItem As ListViewItem = Nothing
            Dim shape As cForcingFunction = Nothing
            Dim iApplication As Integer = 0
            Dim ppi As cPredPreyInteraction = Nothing
            Dim ffappl As eForcingFunctionApplication = Nothing

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)

            Me.m_uic.Core.SetBatchLock(cCore.eBatchLockType.Update)

            ' Update Applied Shape info for this Pred Prey Pair
            Try
                ' FG: Jan 17, 2007 
                ' After discussing with Joe about one bug in UI, how to clear the list when no shape is being applied
                ' The solution is we will always loop through the MaxNumShapes 
                ' If one shape is applied, we will set it to the core
                ' If not, we will set it to nothing
                For iPPI As Integer = 0 To m_lPPI.Count - 1
                    ' Get PPI
                    ppi = m_lPPI(iPPI)

                    ' JS 10sep07: optimized by minimizing the amount of unnecessary updates to the core
                    ppi.LockUpdates = True

                    For iApplicationSlot As Integer = 1 To ppi.MaxNumShapes
                        If iApplicationSlot <= iNumApplied Then ' The shape is being applied
                            lvItem = lvAppliedShapes.Items(iApplicationSlot - 1)
                            shape = DirectCast(lvItem.Tag, cForcingFunction)

                            ffappl = GetTypeFromMultiplier(lvItem.SubItems(1).Text)
                            ppi.setShape(iApplicationSlot, shape, ffappl)
                        Else
                            ppi.setShape(iApplicationSlot, Nothing)
                        End If
                    Next
                    ppi.LockUpdates = False
                Next
            Catch ex As Exception

            End Try

            Me.m_uic.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, True)
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnAdd.Click
            Me.AddShape()
        End Sub

        Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnRemove.Click
            Me.RemoveShape()
        End Sub

        Private Sub SetMultiplier(ByVal sender As System.Object, ByVal e As System.EventArgs) _
         '   Handles rbVulArea.CheckedChanged, rbVul.CheckedChanged, rbSearchRate.CheckedChanged, rbArea.CheckedChanged

            Dim colSelected As ListView.SelectedIndexCollection = lvAppliedShapes.SelectedIndices

            If colSelected.Count > 0 Then
                Dim item As ListViewItem = lvAppliedShapes.Items(colSelected(0))
                item.SubItems(1).Text = GetMultiplier()
            End If

        End Sub

        Private Sub lvAppliedShapes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles lvAppliedShapes.SelectedIndexChanged
            Me.UpdateControls()
        End Sub

        Private Sub lvAllShapes_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles lvAllShapes.DoubleClick
            Me.AddShape()
        End Sub

        Private Sub lvAppliedShapes_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles lvAppliedShapes.DoubleClick
            Me.RemoveShape()
        End Sub

#Region "Private methods"

        Private ReadOnly Property LargeIconSize() As Integer
            Get
                Debug.Assert(Me.m_uic.StyleGuide IsNot Nothing)
                Return Me.m_uic.StyleGuide.ThumbnailSize
            End Get
        End Property

        Private ReadOnly Property SmallIconSize() As Integer
            Get
                Debug.Assert(Me.m_uic.StyleGuide IsNot Nothing)
                Return CInt(Math.Ceiling(Me.m_uic.StyleGuide.ThumbnailSize / 3))
            End Get
        End Property

        Private Property Shape(ByVal lvi As ListViewItem) As cForcingFunction
            Get
                Return DirectCast(lvi.Tag, cForcingFunction)
            End Get
            Set(ByVal value As cForcingFunction)
                lvi.Tag = value
            End Set
        End Property

        Private Sub AddShape()

            Dim colSelected As ListView.SelectedIndexCollection = lvAllShapes.SelectedIndices
            Dim shapeSelected As cForcingFunction = Nothing
            Dim shapeTest As cForcingFunction = Nothing
            Dim item As ListViewItem = Nothing

            If colSelected.Count > 0 Then

                'Get the shape data
                item = Me.lvAllShapes.Items(colSelected(0))
                shapeSelected = Shape(item)

                ' Sanity check
                Debug.Assert(shapeSelected IsNot Nothing, "Unable to locate applied forcing function")

                ' Find duplicate
                For Each itemTest As ListViewItem In lvAppliedShapes.Items
                    shapeTest = Shape(itemTest)
                    If Object.ReferenceEquals(shapeSelected, shapeTest) Then Return
                Next

                item = New ListViewItem(shapeSelected.Name)
                item.ImageIndex = FindImageIndex(shapeSelected)
                item.SubItems.Add(GetMultiplier())
                item.SubItems.Add(CStr(shapeSelected.Index))
                item.Tag = shapeSelected

                lvAppliedShapes.Items.Add(item)
            End If

            UpdateControls()
        End Sub

        Public Sub RemoveShape()
            Dim colSelected As ListView.SelectedIndexCollection = lvAppliedShapes.SelectedIndices
            If colSelected.Count > 0 Then
                lvAppliedShapes.Items.RemoveAt(colSelected(0))
            End If

            If lvAppliedShapes.Items.Count > 0 Then
                lvAppliedShapes.Items(lvAppliedShapes.Items.Count - 1).Selected = True
            End If

            UpdateControls()
        End Sub

        Private Sub UpdateControls()

            Dim colSelected As ListView.SelectedIndexCollection = lvAppliedShapes.SelectedIndices
            Dim lvi As ListViewItem = Nothing
            Dim shape As cShapeData = Nothing
            Dim bAppliedSelected As Boolean = False
            Dim bOnlyAllowedAppliedSelected As Boolean = True
            Dim nApplied As Integer = lvAppliedShapes.Items.Count

            ' Determine if Applied shape selection consists of ONLY allowed shapes
            For Each iIndex As Integer In colSelected
                lvi = lvAppliedShapes.Items(iIndex)
                shape = DirectCast(lvi.Tag, cShapeData)
                bAppliedSelected = True
                bOnlyAllowedAppliedSelected = bOnlyAllowedAppliedSelected And (Me.IsAllowedShape(shape))
            Next

            'Add button is disabled when the count of AppliedShape for the current Pred-Prey pair is >=5
            ' ToDo_JS: obtain '5' from a constant in cPPIManager?
            btnAdd.Enabled = (nApplied < 5)
            'Remove button is disabled when no appliedShape is selected for the current Pred-Prey pair
            btnRemove.Enabled = bAppliedSelected And bOnlyAllowedAppliedSelected

            If (Me.m_editMode = eEditMode.PredPrey) Then
                lblPrey.Visible = True
                txbPreyName.Visible = True
                lblPred.Visible = True
                txbPredName.Visible = True
                lblTitle.Visible = False
            Else
                lblTitle.Location = lblPrey.Location
                lblPrey.Visible = False
                txbPreyName.Visible = False
                lblPred.Visible = False
                txbPredName.Visible = False
                lblTitle.Visible = True
            End If

        End Sub

        Private Function GetMultiplier() As String

            If rbSearchRate.Checked Then
                Return My.Resources.SHAPE_MULTIPLIER_1
            ElseIf rbVul.Checked Then
                Return My.Resources.SHAPE_MULTIPLIER_2
            ElseIf rbArea.Checked Then
                Return My.Resources.SHAPE_MULTIPLIER_3
            ElseIf rbVulArea.Checked Then
                Return My.Resources.SHAPE_MULTIPLIER_4
            ElseIf rbProdRate.Checked Then
                Return My.Resources.SHAPE_MULTIPLIER_5
            End If

            Return String.Empty

        End Function

        Private Function GetTypeFromMultiplier(ByVal s As String) As eForcingFunctionApplication

            Select Case s
                Case My.Resources.SHAPE_MULTIPLIER_1
                    Return eForcingFunctionApplication.SearchRate
                Case My.Resources.SHAPE_MULTIPLIER_2
                    Return eForcingFunctionApplication.Vulnerability
                Case My.Resources.SHAPE_MULTIPLIER_3
                    Return eForcingFunctionApplication.ArenaArea
                Case My.Resources.SHAPE_MULTIPLIER_4
                    Return eForcingFunctionApplication.VulAndArea
                Case My.Resources.SHAPE_MULTIPLIER_5
                    Return eForcingFunctionApplication.ProductionRate
            End Select

        End Function

        Private Function GetMultiplierFromType(ByVal type As eForcingFunctionApplication) As String

            Select Case type
                Case eForcingFunctionApplication.SearchRate, _
                     eForcingFunctionApplication.ProductionRate
                    Select Case Me.m_targetType
                        Case eApplyTargetTypes.Consumer
                            Return My.Resources.SHAPE_MULTIPLIER_1
                        Case eApplyTargetTypes.PrimaryProducer
                            Return My.Resources.SHAPE_MULTIPLIER_5
                    End Select
                Case eForcingFunctionApplication.Vulnerability
                    Return My.Resources.SHAPE_MULTIPLIER_2
                Case eForcingFunctionApplication.ArenaArea
                    Return My.Resources.SHAPE_MULTIPLIER_3
                Case eForcingFunctionApplication.VulAndArea
                    Return My.Resources.SHAPE_MULTIPLIER_4
                Case Else
                    Debug.Assert(False)
            End Select
            Return String.Empty

        End Function

        Private Sub GenerateImages()

            Dim i As Integer = 0
            Dim bmp As Bitmap = Nothing

            'Set up the thumbnail image size
            m_ilLarge.ImageSize = New Size(LargeIconSize, LargeIconSize)
            m_ilSmall.ImageSize = New Size(SmallIconSize, SmallIconSize)

            If m_lFFs.Count > 0 Then

                For Each shapeFunc As cForcingFunction In m_lFFs

                    bmp = New Bitmap(LargeIconSize, LargeIconSize)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        ShapeImage.DrawShape(Me.m_uic, shapeFunc, New Rectangle(0, 0, bmp.Width, bmp.Height), g, Color.Red, eSketchDrawModeTypes.Line)
                        m_ilLarge.Images.Add(bmp)
                    End Using

                    bmp = New Bitmap(SmallIconSize, SmallIconSize)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        ShapeImage.DrawShape(Me.m_uic, shapeFunc, New Rectangle(0, 0, bmp.Width, bmp.Height), g, Color.Red, eSketchDrawModeTypes.Line)
                        m_ilSmall.Images.Add(bmp)
                    End Using

                Next
            End If

        End Sub

        Private Sub LoadAvailableShapes()

            Dim item As ListViewItem = Nothing
            Dim i As Integer = 0

            lvAllShapes.Items.Clear()

            If m_lFFs.Count > 0 Then

                For Each ff As cForcingFunction In m_lFFs

                    If Me.IsAllowedShape(ff) Then
                        ' JS 30nov09: add ff index to label
                        item = New ListViewItem(String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, ff.Index, ff.Name))
                        item.ImageIndex = FindImageIndex(ff)
                        item.Tag = ff
                        lvAllShapes.Items.Add(item)

                        ' Next
                        i += 1
                    End If
                Next

                lvAllShapes.View = View.SmallIcon
                lvAllShapes.Items(0).Selected = True
                lvAllShapes.LargeImageList = Me.m_ilLarge
                lvAllShapes.SmallImageList = Me.m_ilSmall

            End If

        End Sub

        Private Sub LoadAppliedShapes()

            If (m_editMode = eEditMode.PredPrey) Then

                Dim ppi As cPredPreyInteraction = m_lPPI(0)
                Dim item As ListViewItem = Nothing
                Dim shape As cForcingFunction = Nothing
                Dim ffappl As eForcingFunctionApplication

                If ppi Is Nothing Then Return
                'Create listviewItem from Pred -Prey Interaction
                For i As Integer = 1 To ppi.NAppliedShapes

                    ppi.getShape(i, shape, ffappl)
                    ' JS 30nov09: add index to label
                    item = New ListViewItem(String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, shape.Index, shape.Name))
                    item.ImageIndex = FindImageIndex(shape)
                    item.SubItems.Add(GetMultiplierFromType(ffappl))
                    item.SubItems.Add(FindAppliedShapeIndex(shape).ToString)
                    item.Tag = shape

                    If Me.IsAllowedShape(shape) Then
                        item.ForeColor = SystemColors.ControlText
                    Else
                        item.ForeColor = SystemColors.GrayText
                    End If

                    lvAppliedShapes.Items.Add(item)

                Next
            End If

            lvAppliedShapes.View = View.Details
            lvAppliedShapes.LargeImageList = m_ilLarge
            lvAppliedShapes.SmallImageList = m_ilSmall

        End Sub

        Private Function IsAllowedShape(ByVal shape As cShapeData) As Boolean
            If (TypeOf shape Is cMediationFunction) Then
                Return (Me.m_shapeMode = eApplyShapeTypes.Mediation)
            Else
                Return (Me.m_shapeMode = eApplyShapeTypes.Forcing)
            End If
        End Function

        Private Sub LoadMultiplierOption()

            Select Case Me.m_targetType
                Case eApplyTargetTypes.Consumer
                    rbProdRate.Visible = False : rbProdRate.Enabled = False : rbProdRate.Checked = False
                    rbSearchRate.Visible = True : rbSearchRate.Enabled = True : rbSearchRate.Checked = True
                    rbVul.Visible = True : rbVul.Enabled = True
                    rbVulArea.Visible = True : rbVulArea.Enabled = True
                    rbArea.Visible = True : rbArea.Enabled = True

                Case eApplyTargetTypes.PrimaryProducer
                    rbProdRate.Visible = True : rbProdRate.Enabled = True : rbProdRate.Checked = True
                    rbSearchRate.Visible = False : rbSearchRate.Enabled = False
                    rbVul.Visible = False : rbVul.Enabled = False
                    rbVulArea.Visible = False : rbVulArea.Enabled = False
                    rbArea.Visible = False : rbArea.Enabled = False

                Case eApplyTargetTypes.NotSet
                    Debug.Assert(False)

            End Select

        End Sub

        Private Function FindAppliedShapeIndex(ByRef ff As cForcingFunction) As Integer

            For i As Integer = 0 To m_lFFs.Count - 1
                Dim shape As cForcingFunction = m_lFFs(i)

                If (TypeOf shape Is cMediationFunction) And (TypeOf ff Is cMediationFunction) Then
                    If ff.Index = shape.Index Then Return shape.Index
                End If

                ' JS 23may08 (happy b-day Ethan)
                '          MedF is inherited of FF. The original test would always return true, even when testing FF vs MedF
                '          Better to test if shape IS a FF, not is or is inherited from FF
                'If (TypeOf shape Is cForcingFunction) And (TypeOf ff Is cForcingFunction) Then
                If (Not TypeOf shape Is cMediationFunction) And (Not TypeOf ff Is cMediationFunction) Then
                    If ff.Index = shape.Index Then Return shape.Index
                End If
            Next

            Return -1

        End Function

        Private Function FindImageIndex(ByRef ff As cForcingFunction) As Integer

            For i As Integer = 0 To m_lFFs.Count - 1
                Dim shape As cForcingFunction = m_lFFs(i)

                If (TypeOf shape Is cMediationFunction) And (TypeOf ff Is cMediationFunction) Then
                    If ff.Index = shape.Index Then Return i
                End If

                If (Not TypeOf shape Is cMediationFunction) And (Not TypeOf ff Is cMediationFunction) Then
                    If ff.Index = shape.Index Then Return i
                End If
            Next

            Return -1

        End Function

#End Region

    End Class

End Namespace

