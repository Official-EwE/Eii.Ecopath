#Region " Imports "

Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Xml
Imports System.Globalization
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Module modUtility
    Public Const DEFAULT_COL_WIDTH As Integer = 70
    Public Const ID_COL_WIDTH As Integer = 25
    Public Const GRP_NAME_COL_WIDTH As Integer = 110
    Public Const FIRST_ROW_HEIGHT As Integer = 45

    Public Sub SetGridColumnPropertyDefault(ByVal DataGrid As Windows.Forms.DataGridView)
        DataGrid.ColumnHeadersVisible = False
        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            'DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = DEFAULT_COL_WIDTH '110
            DataGrid.Columns(intColIndex).Frozen = False
            DataGrid.Columns(intColIndex).SortMode = Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Next
    End Sub

    Public Sub AddCurve(ByVal strName As String, ByVal CurveVar() As Single, ByVal Pane As GraphPane, _
      ByVal MyColor As Color, Optional ByVal Symbol As SymbolType = SymbolType.None)
        Dim List As PointPairList
        Dim iNumPoints As Integer = CurveVar.GetUpperBound(0)

        List = New PointPairList()
        For iTime As Integer = 1 To iNumPoints
            List.Add(iTime, CurveVar(iTime))
        Next
        Pane.AddCurve(strName, List, MyColor, Symbol)

        Pane.XAxis.Scale.Max = iNumPoints
    End Sub

    Public Enum ePyramidTypes As Byte
        [Catch] = 0
        Flow = 1
        Biomass = 2
    End Enum

    Public Function WritePyramidFile(ByVal strModel As String, ByVal pyramidtype As ePyramidTypes, _
                                     ByVal strUnits As String, ByVal iNumTL As Integer, _
                                     ByVal sTotalB As Single, ByVal asBiomass() As Single, ByVal asValue() As Single) As String

        Dim doc As XmlDocument = New XmlDocument()
        Dim nodePyramid As XmlNode = Nothing
        Dim attrib As XmlAttribute = Nothing
        Dim nodeTL As XmlNode = Nothing
        Dim ciEnUSLocale As New CultureInfo("en-US")
        Dim strOutputFile As String = SystemUtilities.MakeTempFile("NA-pyramid-biomass.xml")

        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", ""))
        nodePyramid = doc.CreateElement("pyramid")
        doc.AppendChild(nodePyramid)

        attrib = doc.CreateAttribute("model")
        attrib.Value = strModel
        nodePyramid.Attributes.Append(attrib)

        attrib = doc.CreateAttribute("type")
        attrib.Value = pyramidtype.ToString()
        nodePyramid.Attributes.Append(attrib)

        attrib = doc.CreateAttribute("unit")
        attrib.Value = strUnits
        nodePyramid.Attributes.Append(attrib)

        attrib = doc.CreateAttribute("total-biomass")
        attrib.Value = sTotalB.ToString(ciEnUSLocale)
        nodePyramid.Attributes.Append(attrib)

        attrib = doc.CreateAttribute("num-tl")
        attrib.Value = iNumTL.ToString(ciEnUSLocale)
        nodePyramid.Attributes.Append(attrib)

        For iTL As Integer = 1 To iNumTL
            nodeTL = doc.CreateElement("trophic-level")

            attrib = doc.CreateAttribute("level")
            attrib.Value = iTL.ToString(ciEnUSLocale)
            nodeTL.Attributes.Append(attrib)

            attrib = doc.CreateAttribute("biomass")
            attrib.Value = asBiomass(iTL).ToString(ciEnUSLocale)
            nodeTL.Attributes.Append(attrib)

            attrib = doc.CreateAttribute("value")
            attrib.Value = asValue(iTL).ToString(ciEnUSLocale)
            nodeTL.Attributes.Append(attrib)

            nodePyramid.AppendChild(nodeTL)
        Next iTL

        doc.Save(strOutputFile)

        Return strOutputFile
    End Function

End Module
