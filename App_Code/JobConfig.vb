Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic


Public Class JobConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetListData(HeaderId As Integer) As DataSet
        Dim thisDataSet As New DataSet()
        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand("CreateJobSheets", thisConn)
                ' Tentukan bahwa ini adalah stored procedure
                thisCmd.CommandType = CommandType.StoredProcedure
                ' Tambahkan parameter HeaderId
                thisCmd.Parameters.Add(New SqlParameter("@HeaderId", SqlDbType.Int) With {.Value = HeaderId})
                Using thisAdapter As New SqlDataAdapter(thisCmd)
                    Try
                        thisConn.Open()
                        thisAdapter.Fill(thisDataSet)
                    Catch ex As Exception
                        ' Tangani error sesuai kebutuhan
                        Throw New Exception("Terjadi kesalahan saat mengambil data: " & ex.Message)
                    End Try
                End Using
            End Using
        End Using
        Return thisDataSet
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = ""
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(thisString, thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Double = 0.00
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(thisString, thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0)
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function





    '#========================|| Create Job Sheet ||===================#
    public Sub CreateJob(HeaderId As String, fileDirectory As String, fileName As String)
        Dim result As String = String.Empty
        Dim MyDataJobSheet As DataSet = GetListData(HeaderId)
        Using stream As FileStream = New FileStream(fileDirectory + "/" + fileName, FileMode.Create)
            Dim pdfDoc As Document = New Document(PageSize.A4, 20, 20, 20, 20)
            Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, stream)
            pdfDoc.Open()
            IF MyDataJobSheet.Tables(0).Rows.Count > 0 Then
                '=========================Page Counter=========================
                Dim rollerCount As New Dictionary(Of String, Integer)
                For i As Integer = 0 To MyDataJobSheet.Tables(0).Rows.Count - 1
                    Dim currentData As DataRow = MyDataJobSheet.Tables(0).Rows(i)
                    Dim blindType As String = currentData("BlindType").ToString()
                    If rollerCount.ContainsKey(blindType) Then
                        rollerCount(blindType) += 1
                    Else
                        rollerCount.Add(blindType, 1)
                    End If
                Next
                '=========================/Page Counter=========================
                '#loop data
                For i As Integer = 0 To MyDataJobSheet.Tables(0).Rows.Count - 1
                    ' Ambil data dari record saat ini
                    Dim currentData As DataRow = MyDataJobSheet.Tables(0).Rows(i)
                    Dim count As Integer = rollerCount(currentData("BlindType").ToString())
                    '=========================Data Looper=========================
                        result = PrintHeader(currentData)
                        '#.................|| Roller Blinds ||.................#
                        If currentData("DesignType").ToString() = "Roller Blinds" Then
                            Select Case currentData("BlindType").ToString()
                                Case "Roller Blind"
                                    result += PrintRollerBlinds(currentData, count)
                                Case "Motorised"
                                    result += PrintRollerMotorised(currentData, count)
                            End Select
                        End If

                        '#.................|| Aluminium Blinds ||.................#
                        If currentData("DesignType").ToString() = "Aluminium Blinds" Then
                            result += PrintAluminium(currentData, count)
                        End If

                        '#.................|| Vertical Blinds ||.................#
                        If currentData("DesignType").ToString() = "Vertical Blinds" Then
                            Select Case currentData("BlindType").ToString()
                                Case "Complete"
                                    result += PrintVerticalComplete(currentData, count)
                            End Select
                        End If
                        
                    '=========================/Data Looper========================
                    
                    Dim sr As StringReader = New StringReader(result)
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr)
                    ' Tambahkan halaman baru kecuali pada record terakhir
                    If i < MyDataJobSheet.Tables(0).Rows.Count - 1 Then
                        pdfDoc.NewPage()
                    End If
                Next
            End If
            pdfDoc.Close()
            stream.Close()
        End Using
    End Sub


    '#========================|| Print Header ||=======================#
    Protected Function PrintHeader(currentData)
        Dim result As String = String.Empty
        Dim ReportType As String = ""
        Dim ReportIcon As String = ""
        Dim GoWith As String = ""
        '#Validate 1
        Select Case  currentData("DesignType").ToString()
            '#--------Roller Blinds-----------
            Case "Roller Blinds"
                ReportIcon = "H"
                GoWith = "H"
                ReportType = "Holland"

            '#--------Aluminium Blinds-----------
            Case "Aluminium Blinds"
                ReportIcon = "V-alu"
                GoWith = "Alu"
                ReportType = "Aluminium Venetian"

            '#--------Vertical Blinds-----------
            Case "Vertical Blinds"
                ReportIcon = "V"
                GoWith = "V"
                ReportType = "Vertical"
        End Select
        '#Validate 2
        Select Case  currentData("BlindType").ToString()
            '#--------Roller | Roller Blinds-----------
            Case "Roller Blind"
                ReportType = "Holland Blinds"
                ReportIcon = "H"

            '#--------Roller | Roller Motorised-----------
            Case "Motorised"
                ReportType = "Holland Motorised"
                ReportIcon = "HM"
                GoWith = "Motorised"

            '#--------Vertical | Vertical Complete-----------
            Case "Complete"
                ReportType = "V-Complete"
                ReportIcon = "VD"
                GoWith = "VD"

            '#--------Vertical | Vertical Slat Only-----------
            Case "Slat Only"
                ReportType = "V-Slat Only"
                ReportIcon = "VDs"
                GoWith = "VDs"

            '#--------Vertical | Vertical Track Only-----------
            Case "Track Only"
                ReportType = "V-Track Only"
                ReportIcon = "VDt"
                GoWith = "VDt"
        End Select
        Dim OrderCreated As String = Convert.ToDateTime(currentData("OrderCreated")).ToString("dd/MM/yyyy")
        Dim JobCreated As String = Convert.ToDateTime(currentData("JobCreated")).ToString("dd/MM/yyyy")
        Dim ShipDate As String = Convert.ToDateTime(currentData("ShipDate")).ToString("dd/MM/yyyy")
        '#header
        result+= "<table style='width: 100%; border-collapse: collapse;'>"
            '#Line 1
            result+= "<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 15px; padding-bottom: 5px;'>Go With</th>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 15px; padding-bottom: 5px;'>: "& GoWith &" /</th>"

                '#Heading Center This Only
                result+= "<th style='font-family: Impact, sans-serif; text-align: center; font-size: 35px; width: auto;' rowspan='6'>" & ReportIcon & "</th>"

                '#heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Job No</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("JoNumber").ToString() &"</th>"
            result+= "</tr>"

            '#Line 2
            result+="<tr>"   
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px; font-size: 12px; padding-bottom: 5px;'>Order/Job Date</td>"
                result+= "<td style=' text-align: left; width: 350px; font-size: 12px; padding-bottom: 5px;'>: "& OrderCreated &" / "& JobCreated &"</td>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 13px; padding-bottom: 5px;'>Reff</th>"
                result+= "<th style=' text-align: left; font-size: 13px; padding-bottom: 5px;'>: "& currentData("OrderCust").ToString() &"</th>"
            result+="</tr>"

            '#Line 3
            result+="<tr>"
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px;font-size: 13px; padding-bottom: 5px;'>ID Unique</td>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 13px; padding-bottom: 5px;'>: "& currentData("HeaderId").ToString() &"</th>"

                '#Heading Right
                result+= "<td style=' text-align: left; width: 80px; font-size: 12px; padding-bottom: 5px;'>Design Type</td>"
                result+= "<td style=' text-align: left; font-size: 12px; padding-bottom: 5px;'>: " & ReportType & "</td>"
            result+="</tr>"

            '#Line 4
            result+="<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 15px; padding-bottom: 5px;'>Store</th>"
                result+= "<th style=' text-align: left; width: 350px;font-size: 15px; padding-bottom: 5px;'>: "& currentData("StoreName").ToString() &"</th>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Due date</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "&  ShipDate &"</th>"
            result+="</tr>"

            '#Line 5
            result+="<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 12px; padding-bottom: 5px;'>Order No</th>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 12px; padding-bottom: 5px;'>: "& currentData("OrderNo").ToString() &"</th>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Zone</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("ZoneId").ToString() &"</th>"
            result+="</tr>"

            '#Line 6
            result+="<tr>"
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px; font-size: 12px;'>Total Order Blind</td>"
                result+= "<td style=' text-align: left; width: 350px;  font-size: 12px;'>: "& currentData("AmountBlinds").ToString() &"</td>"

                '#Heading Right
                result+= "<td style=' text-align: left; width: 80px; font-size: 12px; padding-bottom: 5px;'>Entered By</td>"
                result+= "<td style=' text-align: left; font-size: 12px; padding-bottom: 5px;'>: "& currentData("UserName").ToString() &"</td>"
            result+="</tr>"
            '#Line Option
            result+="<tr>"
                result+= "<td colspan='5' style='font-size: 13px; border-top:1px solid black; height: 50px;  vertical-align: top; padding: 0;'>"
                    result+=""
                result+="</td>"
            result+="</tr>"
        result+= "</table>"
        Return result
    End Function

    '#........................|| Templates ||........................#
    Dim tableDetStart AS String = "<table style='width: 100%; border-collapse: collapse; font-size:13px;'>"
    Dim boldStart As String = "<b>"
    Dim boldEnd As String = "</b>"
    Dim trDetStart As String = "<tr style='text-align:left;'>"
    Dim tdTitleStart As String ="<td style='width:100px; border-top:1px solid black; border-right:1px solid black;'>"
    Dim tdDetStart  As String ="<td style='width:100px; padding:5px 2px; border-top:1px solid black; border-right:1px solid black;'>"
    Dim tdDetFooterStart  As String ="<td style='width:100px; padding:5px 2px; text-align: center;'>"
    Dim tdDetTransStart  As String ="<td style='width:100px; padding:5px 2px; border-top:1px solid black; border-right:1px solid black; color:white;'>"
    Dim tdDetRight  As String ="<td style='width:100px padding:5px 2px; border-top:1px solid black;'>"
    Dim tdDetEnd As String = "</td>"
    Dim trDetEnd As String = "</tr>"
    Dim fs12Start As String = "<span style='font-size:12px;'>"
    Dim fs11Start As String = "<span style='font-size:11px;'>"
    Dim fs10Start As String = "<span style='font-size:10px;'>"
    Dim fsEnd As String = "</span>"
    Dim tableDetEnd As String = "</table>"



    '#========================|| Holland Blinds ||=======================#
    '#........................|| Roller ||........................#
    Protected Function PrintRollerBlinds(currentData, count)
        Dim result As String = String.Empty
        '#validate bracket type
        Dim bracketTypes As String() = {
            currentData("BracketType1").ToString(),
            currentData("BracketType2").ToString(),
            currentData("BracketType3").ToString(),
            currentData("BracketType4").ToString(),
            currentData("BracketType5").ToString(),
            currentData("BracketType6").ToString()
        }

        For i As Integer = 0 To bracketTypes.Length - 1
            If bracketTypes(i) = "Double and Link System Dep" Then
                bracketTypes(i) = "DB Link (Dep)"
            ElseIf bracketTypes(i) = "Double and Link System Ind" Then
                bracketTypes(i) = "DB Link (Ind)"
            End If
        Next

        ' Jika Anda masih perlu mengassign kembali ke variabel individual
        Dim BracketType1 As String = bracketTypes(0)
        Dim BracketType2 As String = bracketTypes(1)
        Dim BracketType3 As String = bracketTypes(2)
        Dim BracketType4 As String = bracketTypes(3)
        Dim BracketType5 As String = bracketTypes(4)
        Dim BracketType6 As String = bracketTypes(5)

        '#Substitute Fabric
        result+= SubstituteFabric(currentData)

        '#Line Options
        result+= lineOptions(currentData)

        '#Table Data
        result+= tableDetStart  
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#FabricType Or Fabric
            result+= trDetStart
                result+= tdTitleStart & "Fabric" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#FabricColour Or Colour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricColour6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TubeSkinSize Or Tube Width
            result+= trDetStart
                result+= tdTitleStart & "Tube Width" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize1").ToString()), "0", currentData("TubeSkinSize1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize2").ToString()), "0", currentData("TubeSkinSize2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize3").ToString()), "0", currentData("TubeSkinSize3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize4").ToString()), "0", currentData("TubeSkinSize4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize5").ToString()), "0", currentData("TubeSkinSize5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("TubeSkinSize6").ToString()), "0", currentData("TubeSkinSize6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#TubeSkinSize Or Skin Width
            result+= trDetStart
                result+= tdTitleStart & "Skin Width" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize1").ToString()), "0", currentData("TubeSkinSize1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize2").ToString()), "0", currentData("TubeSkinSize2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize3").ToString()), "0", currentData("TubeSkinSize3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize4").ToString()), "0", currentData("TubeSkinSize4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize5").ToString()), "0", currentData("TubeSkinSize5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("TubeSkinSize6").ToString()), "0", currentData("TubeSkinSize6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#NumBoldNuts Or Skin Drop
            result+= trDetStart
                result+= tdTitleStart & "Skin Drop" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts1").ToString()), "0", currentData("NumBoldNuts1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts2").ToString()), "0", currentData("NumBoldNuts2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts3").ToString()), "0", currentData("NumBoldNuts3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts4").ToString()), "0", currentData("NumBoldNuts4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts5").ToString()), "0", currentData("NumBoldNuts5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("NumBoldNuts6").ToString()), "0", currentData("NumBoldNuts6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Trim Or Trims
            result+= trDetStart
                result+= tdTitleStart & "Trim" & tdDetEnd
                result+= tdDetStart & currentData("Trims1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Trims6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RollDirection
            result+= trDetStart
                result+= tdTitleStart & "Roll Direction" & tdDetEnd
                result+= tdDetStart & currentData("RollDirection1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RollDirection6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControllType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Control Type" & boldEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("HardwareType1").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("HardwareType2").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("HardwareType3").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("HardwareType4").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("HardwareType5").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs12Start & boldStart & currentData("HardwareType6").ToString() & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

            '#ControllColour
            result+= trDetStart
                result+= tdTitleStart & "Control Colour" & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("HardwareColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControllPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition1").ToString()), "-", currentData("ControlPosition1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition2").ToString()), "-", currentData("ControlPosition2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition3").ToString()), "-", currentData("ControlPosition3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition4").ToString()), "-", currentData("ControlPosition4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition5").ToString()), "-", currentData("ControlPosition5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ControlPosition6").ToString()), "-", currentData("ControlPosition6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#ChainColour
            result+= trDetStart
                result+= tdTitleStart & "Chain Colour" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour1").ToString()), "-", currentData("ChainColour1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour2").ToString()), "-", currentData("ChainColour2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour3").ToString()), "-", currentData("ChainColour3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour4").ToString()), "-", currentData("ChainColour4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour5").ToString()), "-", currentData("ChainColour5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ChainColour6").ToString()), "-", currentData("ChainColour6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#ChildSafe
            result+= trDetStart
                result+= tdTitleStart & "Delux Child Safe" & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe1").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe2").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe3").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe4").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe5").ToString() & fsEnd & tdDetEnd
                result+= tdDetRight & fs11Start & currentData("ChildSafe6").ToString() & fsEnd & tdDetEnd
            result+= trDetEnd

            '#ControlLength
            result+= trDetStart
                result+= tdTitleStart & "Control Length" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength1").ToString()), "-", currentData("ChainLength1").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength2").ToString()), "-", currentData("ChainLength2").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength3").ToString()), "-", currentData("ChainLength3").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength4").ToString()), "-", currentData("ChainLength4").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength5").ToString()), "-", currentData("ChainLength5").ToString() & " + joiner") & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ChainLength6").ToString()), "-", currentData("ChainLength6").ToString() & " + joiner") & tdDetEnd
            result+= trDetEnd

            '#TubeSize
            result+= trDetStart
                result+= tdTitleStart & "Tube Size" & tdDetEnd
                result+= tdDetStart & currentData("TubeSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TubeSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomRailShape
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Shape" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType1").ToString()), "-", currentData("BrailType1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType2").ToString()), "-", currentData("BrailType2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType3").ToString()), "-", currentData("BrailType3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType4").ToString()), "-", currentData("BrailType4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType5").ToString()), "-", currentData("BrailType5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("BrailType6").ToString()), "-", currentData("BrailType6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd

            '#BottomRailColour
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Colour" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour1").ToString()), "-", currentData("BrailColour1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour2").ToString()), "-", currentData("BrailColour2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour3").ToString()), "-", currentData("BrailColour3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour4").ToString()), "-", currentData("BrailColour4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour5").ToString()), "-", currentData("BrailColour5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour6").ToString()), "-", currentData("BrailColour6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Accessories
            result+= trDetStart
                result+= tdTitleStart & "Accessories" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories1").ToString()), "-", currentData("Accessories1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories2").ToString()), "-", currentData("Accessories2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories3").ToString()), "-", currentData("Accessories3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories4").ToString()), "-", currentData("Accessories4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories5").ToString()), "-", currentData("Accessories5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Accessories6").ToString()), "-", currentData("Accessories6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#SpringAssist
            result+= trDetStart
                result+= tdTitleStart & "Spring Assist" & tdDetEnd
                result+= tdDetStart & currentData("SpringAssist1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringAssist2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringAssist3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringAssist4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringAssist5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("SpringAssist6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Bracket
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & BracketType1 & tdDetEnd
                result+= tdDetStart & BracketType2 & tdDetEnd
                result+= tdDetStart & BracketType3 & tdDetEnd
                result+= tdDetStart & BracketType4 & tdDetEnd
                result+= tdDetStart & BracketType5 & tdDetEnd
                result+= tdDetRight & BracketType6 & tdDetEnd
            result+= trDetEnd

            '#LinkBlinds
            result+= trDetStart
                result+= tdTitleStart & "Link Blinds" & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LinkBlinds6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BracketCoverColour
            result+= trDetStart
                result+= tdTitleStart & "Bkt Cover Colour" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour1").ToString()), "-", currentData("BracketColour1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour2").ToString()), "-", currentData("BracketColour2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour3").ToString()), "-", currentData("BracketColour3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour4").ToString()), "-", currentData("BracketColour4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour5").ToString()), "-", currentData("BracketColour5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("BracketColour6").ToString()), "-", currentData("BracketColour6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops1").ToString()), "0", currentData("Drops1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops2").ToString()), "0", currentData("Drops2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops3").ToString()), "0", currentData("Drops3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops4").ToString()), "0", currentData("Drops4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops5").ToString()), "0", currentData("Drops5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drops6").ToString()), "0", currentData("Drops6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#SpringType
            result+= trDetStart
                result+= tdTitleStart & "Spring Type" & tdDetEnd
                result+= tdDetStart & currentData("SpringType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpringType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("SpringType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Fixing
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room1").ToString()), "-", currentData("Room1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room2").ToString()), "-", currentData("Room2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room3").ToString()), "-", currentData("Room3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room4").ToString()), "-", currentData("Room4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room5").ToString()), "-", currentData("Room5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Room6").ToString()), "-", currentData("Room6").ToString()) & tdDetEnd
            result+= trDetEnd
            '#Empty
            result+= trDetStart
                result+= tdDetTransStart & "Empty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd
            '#Empty
            result+= trDetStart
                result+= tdDetTransStart & "Empty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
            result+= trDetEnd
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Rollers: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & currentData("AmountOfPage").ToString() & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Tube" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Fabric" & tdDetEnd
                result+= tdDetFooterStart &  "Sewing" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling, Packing" & tdDetEnd
                result+= tdDetFooterStart &  "QC" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("pageOf").ToString() &" OF "& count & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd
        Return result
    End Function

    '#........................|| Roller Motorised ||........................#
    Protected Function PrintRollerMotorised(currentData, count)
        Dim result As String = String.Empty
        '#Substitute Fabric
        result+= SubstituteFabric(currentData)

        '#Line Options
        result+= lineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#FabricType Or Fabric
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Fabric" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#FabricColour Or Colour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricColour6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TubeSkinSize
            result+= trDetStart
                result+= tdTitleStart & fs11Start & boldStart & "Tube & Skin Width" & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize1").ToString()), "0", currentData("TubeSkinSize1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize2").ToString()), "0", currentData("TubeSkinSize2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize3").ToString()), "0", currentData("TubeSkinSize3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize4").ToString()), "0", currentData("TubeSkinSize4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize5").ToString()), "0", currentData("TubeSkinSize5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize6").ToString()), "0", currentData("TubeSkinSize6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd

            '#NumBoldNuts Or Skin Drop
            result+= trDetStart
                result+= tdTitleStart & "Skin Drop" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts1").ToString()), "0", currentData("NumBoldNuts1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts2").ToString()), "0", currentData("NumBoldNuts2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts3").ToString()), "0", currentData("NumBoldNuts3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts4").ToString()), "0", currentData("NumBoldNuts4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts5").ToString()), "0", currentData("NumBoldNuts5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("NumBoldNuts6").ToString()), "0", currentData("NumBoldNuts6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#TubeSize
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Tube"  & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("TubeSize6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ControllType
            result+= trDetStart
                result+= tdTitleStart & "Control Type" & tdDetEnd
                result+= tdDetStart &  currentData("HardwareType1").ToString() & tdDetEnd
                result+= tdDetStart &  currentData("HardwareType2").ToString() & tdDetEnd
                result+= tdDetStart &  currentData("HardwareType3").ToString() & tdDetEnd
                result+= tdDetStart &  currentData("HardwareType4").ToString() & tdDetEnd
                result+= tdDetStart &  currentData("HardwareType5").ToString() & tdDetEnd
                result+= tdDetRight &  currentData("HardwareType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#MotorStyle
            result+= trDetStart
                result+= tdTitleStart & boldStart & "MotorStyle" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorStyle1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorStyle2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorStyle3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorStyle4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorStyle5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("MotorStyle6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ControllColour
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Control Colour" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareColour1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareColour2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareColour3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareColour4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareColour5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("HardwareColour6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#RollDirection
            result+= trDetStart
                result+= tdTitleStart & "Roll Direction" & tdDetEnd
                result+= tdDetStart & currentData("RollDirection1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RollDirection6").ToString() & tdDetEnd
            result+= trDetEnd

            '#MotorRemote or Remote/Switch
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Remote/Switch" & boldEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote1").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote2").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote3").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote4").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote5").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs10Start & boldStart & currentData("MotorRemote6").ToString() & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

            '#MotorCharger or Charger
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Charger" & boldEnd & tdDetEnd
                result+= tdDetStart & fs11Start & If(String.IsNullOrEmpty(currentData("MotorCharger1").ToString()), "-", currentData("MotorCharger1").ToString()) & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & If(String.IsNullOrEmpty(currentData("MotorCharger2").ToString()), "-", currentData("MotorCharger2").ToString()) & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & If(String.IsNullOrEmpty(currentData("MotorCharger3").ToString()), "-", currentData("MotorCharger3").ToString()) & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & If(String.IsNullOrEmpty(currentData("MotorCharger4").ToString()), "-", currentData("MotorCharger4").ToString()) & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & If(String.IsNullOrEmpty(currentData("MotorCharger5").ToString()), "-", currentData("MotorCharger5").ToString()) & fsEnd & tdDetEnd
                result+= tdDetRight & fs11Start & If(String.IsNullOrEmpty(currentData("MotorCharger6").ToString()), "-", currentData("MotorCharger6").ToString()) & fsEnd & tdDetEnd
            result+= trDetEnd

            '#Cleat or Flush Connect
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Flush Connect" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("ChildSafe1").ToString()), "No", "Yes") & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("ChildSafe2").ToString()), "No", "Yes") & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("ChildSafe3").ToString()), "No", "Yes") & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("ChildSafe4").ToString()), "No", "Yes") & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("ChildSafe5").ToString()), "No", "Yes") & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("ChildSafe6").ToString()), "No", "Yes") & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ControllPosition Or Motor Side
            result+= trDetStart
                result+= tdTitleStart & "Motor Side" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition1").ToString()), "-", currentData("ControlPosition1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition2").ToString()), "-", currentData("ControlPosition2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition3").ToString()), "-", currentData("ControlPosition3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition4").ToString()), "-", currentData("ControlPosition4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition5").ToString()), "-", currentData("ControlPosition5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ControlPosition6").ToString()), "-", currentData("ControlPosition6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#BracketType or Bracket
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketType1").ToString()), "-", currentData("BracketType1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketType2").ToString()), "-", currentData("BracketType2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketType3").ToString()), "-", currentData("BracketType3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketType4").ToString()), "-", currentData("BracketType4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketType5").ToString()), "-", currentData("BracketType5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("BracketType6").ToString()), "-", currentData("BracketType6").ToString()) & tdDetEnd
            result+= trDetEnd

             '#LinkBlinds
            result+= trDetStart
                result+= tdTitleStart & "Link Blinds" & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlinds5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LinkBlinds6").ToString() & tdDetEnd
            result+= trDetEnd

            '#? or Bkt Cover Color
            result+= trDetStart
                result+= tdTitleStart & "Bkt Cover Color" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour1").ToString()), "-", currentData("BracketColour1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour2").ToString()), "-", currentData("BracketColour2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour3").ToString()), "-", currentData("BracketColour3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour4").ToString()), "-", currentData("BracketColour4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketColour5").ToString()), "-", currentData("BracketColour5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("BracketColour6").ToString()), "-", currentData("BracketColour6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Trim Or Trims
            result+= trDetStart
                result+= tdTitleStart & "Trim" & tdDetEnd
                result+= tdDetStart & currentData("Trims1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trims5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Trims6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomRailShape
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Shape" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType1").ToString()), "-", currentData("BrailType1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType2").ToString()), "-", currentData("BrailType2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType3").ToString()), "-", currentData("BrailType3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType4").ToString()), "-", currentData("BrailType4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailType5").ToString()), "-", currentData("BrailType5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("BrailType6").ToString()), "-", currentData("BrailType6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd

            '#BottomRailColour
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Colour" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour1").ToString()), "-", currentData("BrailColour1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour2").ToString()), "-", currentData("BrailColour2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour3").ToString()), "-", currentData("BrailColour3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour4").ToString()), "-", currentData("BrailColour4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour5").ToString()), "-", currentData("BrailColour5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("BrailColour6").ToString()), "-", currentData("BrailColour6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd
            '#Accessories
            result+= trDetStart
                result+= tdTitleStart & "Accessories" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories1").ToString()), "-", currentData("Accessories1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories2").ToString()), "-", currentData("Accessories2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories3").ToString()), "-", currentData("Accessories3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories4").ToString()), "-", currentData("Accessories4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Accessories5").ToString()), "-", currentData("Accessories5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Accessories6").ToString()), "-", currentData("Accessories6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Blind Width
            result+= trDetStart
                result+= tdTitleStart & "Blind Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Blind Drop
            result+= trDetStart
                result+= tdTitleStart & "Blind Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops1").ToString()), "0", currentData("Drops1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops2").ToString()), "0", currentData("Drops2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops3").ToString()), "0", currentData("Drops3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops4").ToString()), "0", currentData("Drops4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops5").ToString()), "0", currentData("Drops5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drops6").ToString()), "0", currentData("Drops6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Fixing
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room1").ToString()), "-", currentData("Room1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room2").ToString()), "-", currentData("Room2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room3").ToString()), "-", currentData("Room3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room4").ToString()), "-", currentData("Room4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room5").ToString()), "-", currentData("Room5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Room6").ToString()), "-", currentData("Room6").ToString()) & tdDetEnd
            result+= trDetEnd
            '#? or Motorised
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Motorised" & boldEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("ControlType1").ToString() & " " & currentData("TubeSize1").ToString() & " " & currentData("HardwareColour1").ToString()  &boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("ControlType2").ToString() & " " & currentData("TubeSize2").ToString() & " " & currentData("HardwareColour2").ToString()  &boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("ControlType3").ToString() & " " & currentData("TubeSize3").ToString() & " " & currentData("HardwareColour3").ToString()  &boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("ControlType4").ToString() & " " & currentData("TubeSize4").ToString() & " " & currentData("HardwareColour4").ToString()  &boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("ControlType5").ToString() & " " & currentData("TubeSize5").ToString() & " " & currentData("HardwareColour5").ToString()  &boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs10Start & boldStart & currentData("ControlType6").ToString() & " " & currentData("TubeSize6").ToString() & " " & currentData("HardwareColour6").ToString()  &boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd
             '#Empty
            result+= trDetStart
                result+= tdDetTransStart & "Empty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd
            '#Empty
            result+= trDetStart
                result+= tdDetTransStart & "Empty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd
        result+=tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
            result+= trDetEnd
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 2px;'>" & "<span>Total Rollers: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & currentData("AmountOfPage").ToString() & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Tube" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Fabric" & tdDetEnd
                result+= tdDetFooterStart &  "Sewing" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling, Packing" & tdDetEnd
                result+= tdDetFooterStart &  "QC" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style=' padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("pageOf").ToString() &" OF "& count & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd
        Return result
    End Function
 


    '#========================|| Aluminium ||=======================#
    Protected Function PrintAluminium(currentData, count)
        Dim result As String = String.Empty
        Dim SlatType1 As String = "-"
        Dim SlatType2 As String = "-"
        Dim SlatType3 As String = "-"
        Dim SlatType4 As String = "-"
        Dim SlatType5 As String = "-"
        Dim SlatType6 As String = "-"
        IF currentData("VenType1").ToString() <> "" THEN : SlatType1 = "Standard" : END IF
        IF currentData("VenType2").ToString() <> "" THEN : SlatType2 = "Standard" : END IF
        IF currentData("VenType3").ToString() <> "" THEN : SlatType3 = "Standard" : END IF
        IF currentData("VenType4").ToString() <> "" THEN : SlatType4 = "Standard" : END IF
        IF currentData("VenType5").ToString() <> "" THEN : SlatType5 = "Standard" : END IF
        IF currentData("VenType6").ToString() <> "" THEN : SlatType6 = "Standard" : END IF

        '#Line Options
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Ven Type
            result+= trDetStart
                result+= tdTitleStart & "Ven Type" & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("VenType1").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("VenType2").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("VenType3").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("VenType4").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("VenType5").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs11Start & boldStart & currentData("VenType6").ToString() & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

            '#Slat Type
            result+= trDetStart
                result+= tdTitleStart & "Slat Type" & tdDetEnd
                result+= tdDetStart & SlatType1 & tdDetEnd
                result+= tdDetStart & SlatType2 & tdDetEnd
                result+= tdDetStart & SlatType3 & tdDetEnd
                result+= tdDetStart & SlatType4 & tdDetEnd
                result+= tdDetStart & SlatType5 & tdDetEnd
                result+= tdDetRight & SlatType6 & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops1").ToString()), "0", currentData("Drops1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops2").ToString()), "0", currentData("Drops2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops3").ToString()), "0", currentData("Drops3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops4").ToString()), "0", currentData("Drops4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops5").ToString()), "0", currentData("Drops5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drops6").ToString()), "0", currentData("Drops6").ToString()) & tdDetEnd
            result+= trDetEnd
            
            '#Colour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HardwareColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("HardwareColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControllPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition1").ToString()), "-", currentData("ControlPosition1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition2").ToString()), "-", currentData("ControlPosition2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition3").ToString()), "-", currentData("ControlPosition3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition4").ToString()), "-", currentData("ControlPosition4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ControlPosition5").ToString()), "-", currentData("ControlPosition5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ControlPosition6").ToString()), "-", currentData("ControlPosition6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#WandLength
            result+= trDetStart
                result+= tdTitleStart & "Wand (Length)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("WandLength1").ToString()), "-", currentData("WandLength1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("WandLength2").ToString()), "-", currentData("WandLength2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("WandLength3").ToString()), "-", currentData("WandLength3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("WandLength4").ToString()), "-", currentData("WandLength4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("WandLength5").ToString()), "-", currentData("WandLength5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("WandLength6").ToString()), "-", currentData("WandLength6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#BracketOption
            result+= trDetStart
                result+= tdTitleStart & "Bracket Type" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketOption1").ToString()), "-", currentData("BracketOption1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketOption2").ToString()), "-", currentData("BracketOption2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketOption3").ToString()), "-", currentData("BracketOption3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketOption4").ToString()), "-", currentData("BracketOption4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("BracketOption5").ToString()), "-", currentData("BracketOption5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("BracketOption6").ToString()), "-", currentData("BracketOption6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Cut Outs
            result+= trDetStart
                result+= "<th colspan='7' style='border-top: 1px solid black; border-bottom:1px solid black; text-align: left; padding:5px 0'>Cut Outs :</th>"
            result+= trDetEnd

        result+= tableDetEnd   
        Return result
    End Function




    '#========================|| Vertical ||=======================#
    '#........................|| Complete ||........................#
    Protected Function PrintVerticalComplete(currentData, count)
        Dim result As String = String.Empty
        '#Line Options
        result+= LineOptions(currentData)
        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Ven Type
            result+= trDetStart
                result+= tdTitleStart & "Vertical Type" & tdDetEnd
                result+= tdDetStart & currentData("VenType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("VenType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("VenType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("VenType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("VenType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("VenType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Track Type
            result+= trDetStart
                result+= tdTitleStart & "Track Type" & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("HardwareType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("HardwareType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Track Colour
            result+= trDetStart
                result+= tdTitleStart & "Track Colour" & tdDetEnd
                result+= tdDetStart & currentData("TrackColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Slat Size
            result+= trDetStart
                result+= tdTitleStart & "Slat Size (mm)" & tdDetEnd
                result+= tdDetStart & currentData("SlatSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("SlatSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Carries
            result+= trDetStart
                result+= tdTitleStart & "Carries Qty" & tdDetEnd
                result+= tdDetStart & currentData("CarriesQty1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarriesQty2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarriesQty3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarriesQty4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarriesQty5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CarriesQty6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Spacer
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Spacer Size (mm)" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("SpacerSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpacerSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpacerSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpacerSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SpacerSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("SpacerSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops1").ToString()), "0", currentData("Drops1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops2").ToString()), "0", currentData("Drops2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops3").ToString()), "0", currentData("Drops3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops4").ToString()), "0", currentData("Drops4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drops5").ToString()), "0", currentData("Drops5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drops6").ToString()), "0", currentData("Drops6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Tack Option
            result+= trDetStart
                result+= tdTitleStart & "Tack Option" & tdDetEnd
                result+= tdDetStart & currentData("TrackOption1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackOption2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackOption3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackOption4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackOption5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackOption6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Control Type | Chain/Wand
            result+= trDetStart
                result+= tdTitleStart & "Chain/Wand" & tdDetEnd
                result+= tdDetStart & currentData("ControlType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Chain Type
            result+= trDetStart
                result+= tdTitleStart & "Chain Type" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength1").ToString()), "-", currentData("ChainLength1").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength2").ToString()), "-", currentData("ChainLength2").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength3").ToString()), "-", currentData("ChainLength3").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength4").ToString()), "-", currentData("ChainLength4").ToString() & " + joiner") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength5").ToString()), "-", currentData("ChainLength5").ToString() & " + joiner") & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ChainLength6").ToString()), "-", currentData("ChainLength6").ToString() & " + joiner") & tdDetEnd
            result+= trDetEnd

            '#Control Colour | Chain/Wand Colour
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Chain/Wand Colour" & fsEnd & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour1").ToString()), "-", currentData("ChainColour1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour2").ToString()), "-", currentData("ChainColour2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour3").ToString()), "-", currentData("ChainColour3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour4").ToString()), "-", currentData("ChainColour4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainColour5").ToString()), "-", currentData("ChainColour5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ChainColour6").ToString()), "-", currentData("ChainColour6").ToString()) & tdDetEnd
            result+= trDetEnd


            '#Stacking
            result+= trDetStart
                result+= tdTitleStart & "Stacking" & tdDetEnd
                result+= tdDetStart & currentData("StackPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("StackPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Control Position
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Control Length
            result+= trDetStart
                result+= tdTitleStart & "Control Length" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength1").ToString()), "0", currentData("ChainLength1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength2").ToString()), "0", currentData("ChainLength2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength3").ToString()), "0", currentData("ChainLength3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength4").ToString()), "0", currentData("ChainLength4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("ChainLength5").ToString()), "0", currentData("ChainLength5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("ChainLength6").ToString()), "0", currentData("ChainLength6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Fabric Material
            result+= trDetStart
                result+= tdTitleStart & "Fabric Material" & tdDetEnd
                result+= tdDetStart & currentData("FabricType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Fabric Colour
            result+= trDetStart
                result+= tdTitleStart & "Fabric Colour" & tdDetEnd
                result+= tdDetStart & currentData("FabricColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Fabric QTY (M)
            result+= trDetStart
                result+= tdTitleStart & "Fabric QTY (M)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("FabricCutDrop1").ToString()), "0", currentData("FabricCutDrop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("FabricCutDrop2").ToString()), "0", currentData("FabricCutDrop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("FabricCutDrop3").ToString()), "0", currentData("FabricCutDrop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("FabricCutDrop4").ToString()), "0", currentData("FabricCutDrop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("FabricCutDrop5").ToString()), "0", currentData("FabricCutDrop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("FabricCutDrop6").ToString()), "0", currentData("FabricCutDrop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Hanger Type
            result+= trDetStart
                result+= tdTitleStart & "Hanger Type" & tdDetEnd
                result+= tdDetStart & currentData("HangerType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("HangerType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Bottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom" & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Bottom Colour
            result+= trDetStart
                result+= tdTitleStart & "Bottom Colour" & tdDetEnd
                result+= tdDetStart & currentData("BottomColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Bracket
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & currentData("BracketOption1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BracketOption6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Left Hand Return
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Left Hand Return" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LeftHandReturn1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LeftHandReturn2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LeftHandReturn3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LeftHandReturn4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LeftHandReturn5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LeftHandReturn6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Right Hand Return
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Right Hand Return" & fsEnd & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("RightHandReturn1").ToString()), "0", currentData("RightHandReturn1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("RightHandReturn2").ToString()), "0", currentData("RightHandReturn2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("RightHandReturn3").ToString()), "0", currentData("RightHandReturn3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("RightHandReturn4").ToString()), "0", currentData("RightHandReturn4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("RightHandReturn5").ToString()), "0", currentData("RightHandReturn5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("RightHandReturn6").ToString()), "0", currentData("RightHandReturn6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Insert In Track
            result+= trDetStart
                result+= tdTitleStart & "Insert In Track" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("InsertInTrack1").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("InsertInTrack2").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("InsertInTrack3").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("InsertInTrack4").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("InsertInTrack5").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("InsertInTrack6").ToString()), "No", "Yes") & tdDetEnd
            result+= trDetEnd

            '#Sloper
            result+= trDetStart
                result+= tdTitleStart & "Sloper" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Sloper1").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Sloper2").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Sloper3").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Sloper4").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Sloper5").ToString()), "No", "Yes") & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Sloper6").ToString()), "No", "Yes") & tdDetEnd
            result+= trDetEnd


            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room1").ToString()), "-", currentData("Room1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room2").ToString()), "-", currentData("Room2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room3").ToString()), "-", currentData("Room3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room4").ToString()), "-", currentData("Room4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Room5").ToString()), "-", currentData("Room5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Room6").ToString()), "-", currentData("Room6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Fixing
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Empty
            result+= trDetStart
                result+= tdDetTransStart & "Empty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

        result+= tableDetEnd


         '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 2px;'>" & "<span>Total Rollers: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & currentData("AmountOfPage").ToString() & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Tube" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Fabric" & tdDetEnd
                result+= tdDetFooterStart &  "Sewing" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling, Packing" & tdDetEnd
                result+= tdDetFooterStart &  "QC" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("pageOf").ToString() &" OF "& count & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd
       Return result
    End Function













    '#........................|| Substitute Fabric ||........................#
    Private Function SubstituteFabric(currentData)
        Dim result As String = String.Empty
        result+= "<table style='width: 100%; border-top:1px solid black; font-size:11px; border-collapse: collapse;'>"
            result+= "<tr>"
                result+= "<td style='width:100px; padding:5px 0;'>Substitute Fabric:</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
            result+= "</tr>"
        result+= "</table>"
        Return result
    End Function


    '#........................|| Line Options ||........................#
    Private Function LineOptions(currentData)
        Dim result As String = String.Empty
        Dim Line1 As String = "Line 0"
        Dim Line2 As String = "Line 0"
        Dim Line3 As String = "Line 0"
        Dim Line4 As String = "Line 0"
        Dim Line5 As String = "Line 0"
        Dim Line6 As String = "Line 0"
        If Not String.IsNullOrEmpty(currentData("Line1").ToString()) Then Line1 = currentData("Line1").ToString()
        If Not String.IsNullOrEmpty(currentData("Line2").ToString()) Then Line2 = currentData("Line2").ToString()
        If Not String.IsNullOrEmpty(currentData("Line3").ToString()) Then Line3 = currentData("Line3").ToString()
        If Not String.IsNullOrEmpty(currentData("Line4").ToString()) Then Line4 = currentData("Line4").ToString()
        If Not String.IsNullOrEmpty(currentData("Line5").ToString()) Then Line5 = currentData("Line5").ToString()
        If Not String.IsNullOrEmpty(currentData("Line6").ToString()) Then Line6 = currentData("Line6").ToString()
        '#Line Option
        result+= "<table style='width: 100%; border-top:1px solid black; font-size:10px; border-collapse: collapse;'>"
            '#Line Option
            result+= "<tr style='text-align: center;'>"
                result+= "<td style='width:100px; padding:5px 0; color:white;'>[Empty]</td>"
                result+= "<td style='width:100px'>"& Line1 &"</td>"
                result+= "<td width:100px>"& Line2 &"</td>"
                result+= "<td width:100px>"& Line3 &"</td>"
                result+= "<td width:100px>"& Line4 &"</td>"
                result+= "<td width:100px>"& Line5 &"</td>"
                result+= "<td width:100px>"& Line6 &"</td>"
            result+= "</tr>"
        result+= "</table>"
        Return result
    End Function
End Class  