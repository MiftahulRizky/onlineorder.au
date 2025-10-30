Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic

Public Class HalperJobSheetRenderer

    Shared publicCfg As New PublicConfig()

    '#Print Header
    Public Shared Function PrintHeader(currentData As DataRow) As String
        Dim result As String = String.Empty
        Dim ReportType As String = String.Empty
        Dim ReportIcon As String = String.Empty
        Dim GoWith As String = String.Empty
        Dim goWithList As New List(Of String)()
        Dim JobId As String = currentData("JobId").ToString()

        ' Dim BlindNameList As DataSet = Nothing
        '#-------------------------------|| Find Go With ||-------------------------------#
        '#All
        Dim all As DataSet = publicCfg.GetListData("SELECT DesignName, BlindName FROM Jobsheets WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To all.Tables(0).Rows.Count - 1
            Dim designName As String = all.Tables(0).Rows(i).Item("DesignName").ToString()
            Select Case designName
            Case "Cellora Blinds"
                goWithList.Add("Cel")
            Case "Roman Blinds"
                goWithList.Add("Rom")
            Case "Panel Glides"
                goWithList.Add("PG")
            End Select
        Next

        '#Roller Blinds
        Dim rollerList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_RollerBlinds WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To rollerList.Tables(0).Rows.Count - 1
            Dim blindName As String = rollerList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case blindName
                Case "Roller Blind"
                        goWithList.Add("H")
                Case "Motorised"
                    goWithList.Add("Motorised")
                Case "Cassette"
                        goWithList.Add("Hc")
                Case "Skin Only"
                    goWithList.Add("Hs")
            End Select
        Next

        '#Aluminium Blinds
        Dim alumList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Aluminium WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To alumList.Tables(0).Rows.Count - 1
            goWithList.Add("Alu")
        Next

        '#Venetian Blinds
        Dim venList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Venetian WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To venList.Tables(0).Rows.Count - 1
            Dim BlindName As String = venList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case BlindName
                Case "Mockwood Venetian"
                    goWithList.Add("MV")
                Case "Timber Venetian"
                    goWithList.Add("TV")
                Case "Wooden Venetian"
                    goWithList.Add("WV")
            End Select
        Next

        '#Vertical Blinds
        Dim verList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Verticals WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To verList.Tables(0).Rows.Count - 1
            Dim blindName As String = verList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case blindName
                Case "Complete"
                    goWithList.Add("VD")
                Case "Slat Only"
                    goWithList.Add("VDs")
                Case "Track Only"
                    goWithList.Add("VDt")
            End Select
        Next

        '#Vertical Blinds
        Dim veriList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Verishades WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To veriList.Tables(0).Rows.Count - 1
            Dim blindName As String = veriList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case blindName
                Case "Single"
                    goWithList.Add("VR")
                Case "Slat Only"
                    goWithList.Add("VRs")
                Case "Track Only"
                    goWithList.Add("VRt")
            End Select
        Next


        '#-------------------------------|| ReportIcon & ReportType ||-------------------------------#
        Select Case currentData("DesignName").ToString()
            '#--------Roller Blinds-----------
            Case "Roller Blinds"
                ReportIcon = "H"
                ReportType = "Holland"

                Select Case  currentData("BlindName").ToString()
                    Case "Roller Blind"
                        ReportType = "Holland Blinds"
                        ReportIcon = "H"

                    Case "Motorised"
                        ReportType = "Holland Motorised"
                        ReportIcon = "HM"

                    Case "Cassette"
                        ReportType = "Holland Cassette"
                        ReportIcon = "Hc"

                    Case "Skin Only"
                        ReportType = "Holland Skin"
                        ReportIcon = "Hs"
                End Select

            '#--------Aluminium Blinds-----------
            Case "Aluminium Blinds"
                ReportIcon = "V-alu"
                ReportType = "Venetian Aluminium"

            '#--------Venetian Blinds-----------
            Case "Venetian Blinds"

                ReportIcon = "V"
                ReportType = "Venetian"

                Select Case  currentData("BlindName").ToString()
                    Case "Mockwood Venetian"
                        ReportType = "Mockwood Venetian"
                        ReportIcon = "MV"

                    Case "Timber Venetian"
                        ReportType = "Timber Venetian"
                        ReportIcon = "TV"

                    Case "Wooden Venetian"
                        ReportType = "Wooden Venetian"
                        ReportIcon = "WV"
                End Select

            '#--------Vertical Blinds-----------
            Case "Vertical Blinds"
                ReportIcon = "V"
                ReportType = "Vertical"

                Select Case  currentData("BlindName").ToString()
                    Case "Complete"
                        ReportType = "Vertical"
                        ReportIcon = "VD"

                    Case "Slat Only"
                        ReportType = "Vertical Slat"
                        ReportIcon = "VDs"

                    Case "Track Only"
                        ReportType = "Vertical Track"
                        ReportIcon = "VDt"
                End Select


            ' '#--------Veri Shades-----------
            Case "Veri Shades"

                ReportIcon = "VR"
                ReportType = "Verishade"
                
                Select Case  currentData("BlindName").ToString()
                    Case "Single"
                        ReportIcon = "VR"
                        ReportType = "Verishade"
                    Case "Slat Only"
                        ReportIcon = "VRs"
                        ReportType = "Verishade Slat"
                    Case "Track Only"
                        ReportIcon = "VRt"
                        ReportType = "Verishade Track"
                End Select
            Case "Cellora Blinds"
                ReportIcon = "CL"
                ReportType = "Cellora"
            Case "Roman Blinds"
                ReportIcon = "R"
                ReportType = "Roman"
            Case "Panel Glides"
                ReportIcon = "PG"
                ReportType = "Panel Glide"
            Case Else
                ReportIcon = "NO"
                ReportType = "Nulable"
        End Select
        
        If goWithList.Count > 0 Then
            GoWith = String.Join(" / ", goWithList)
        End If


        Dim OrderCreated As String = Convert.ToDateTime(currentData("OrderCreated")).ToString("dd/MM/yyyy")
        Dim JobCreated As String = Convert.ToDateTime(currentData("JobCreated")).ToString("dd/MM/yyyy")
        Dim ShipDate As String = Convert.ToDateTime(currentData("ShipDate")).ToString("dd MMM yy").ToUpper()
        Dim Notes1 As String = currentData("Notes1").ToString()
        Dim Notes2 As String = currentData("Notes2").ToString()
        Dim Notes3 As String = currentData("Notes3").ToString()
        Dim Notes4 As String = currentData("Notes4").ToString()
        Dim Notes5 As String = currentData("Notes5").ToString()
        Dim Notes6 As String = currentData("Notes6").ToString()
        Dim hightColumnNotes As String = "height: 30px;"
        If Not (String.IsNullOrEmpty(Notes1) Or String.IsNullOrEmpty(Notes2) Or String.IsNullOrEmpty(Notes3) Or String.IsNullOrEmpty(Notes4) Or String.IsNullOrEmpty(Notes5) Or String.IsNullOrEmpty(Notes6)) Then hightColumnNotes = ""
        '#header
        result+= "<table style='width: 100%; border-collapse: collapse;'>"
            '#Go With, Icon, & Job No
            result+= "<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 15px; padding-bottom: 5px;'>Go With</th>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 15px; padding-bottom: 5px;'>: "& GoWith &" /</th>"

                '#Heading Center This Only
                result+= "<th style='font-family: Impact, sans-serif; text-align: center; font-size: 35px; width: auto;' rowspan='6'>" & ReportIcon & "</th>"

                '#heading Right
                ' result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Job No</th>"
                ' result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("JoNumber").ToString() &"</th>"
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'></th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'></th>"
            result+= "</tr>"

            '#Order/Job Date & Reff
            result+="<tr>"   
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px; font-size: 12px; padding-bottom: 5px;'>Order/Job Date</td>"
                result+= "<td style=' text-align: left; width: 350px; font-size: 12px; padding-bottom: 5px;'>: "& OrderCreated &" / "& JobCreated &"</td>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 13px; padding-bottom: 5px;'>Reff</th>"
                result+= "<th style=' text-align: left; font-size: 13px; padding-bottom: 5px;'>: "& currentData("OrderCust").ToString() &"</th>"
            result+="</tr>"

            '#ID Unique & Design Type
            result+="<tr>"
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px;font-size: 13px; padding-bottom: 5px;'>ID Unique</td>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 13px; padding-bottom: 5px;'>: "& currentData("HeaderId").ToString() &"</th>"

                '#Heading Right
                result+= "<td style=' text-align: left; width: 80px; font-size: 12px; padding-bottom: 5px;'>Design Type</td>"
                result+= "<td style=' text-align: left; font-size: 12px; padding-bottom: 5px;'>: " & ReportType & "</td>"
            result+="</tr>"

            '#Store & Due Date
            result+="<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 15px; padding-bottom: 5px;'>Store</th>"
                result+= "<th style=' text-align: left; width: 350px;font-size: 15px; padding-bottom: 5px;'>: "& currentData("StoreName").ToString() &"</th>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Due date</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "&  ShipDate &"</th>"
            result+="</tr>"

            '#Order No & Zone
            result+="<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 12px; padding-bottom: 5px;'>Order No</th>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 12px; padding-bottom: 5px;'>: "& currentData("OrderNo").ToString() &"</th>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Zone</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("ZoneId").ToString() &"</th>"
            result+="</tr>"

            '#Total Order Blind & Entered By
            result+="<tr>"
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px; font-size: 12px;'>Total Order Blind</td>"
                result+= "<td style=' text-align: left; width: 350px;  font-size: 12px;'>: "& currentData("AmountBlind").ToString() &"</td>"

                '#Heading Right
                result+= "<td style=' text-align: left; width: 80px; font-size: 12px; padding-bottom: 5px;'>Entered By</td>"
                result+= "<td style=' text-align: left; font-size: 12px; padding-bottom: 5px;'>: "& currentData("UserName").ToString() &"</td>"
            result+="</tr>"
            
            '#Information Notes
            result+="<tr>"
                result+= "<td colspan='5' style='font-size: 14px; border-top:1px solid black; "& hightColumnNotes &" vertical-align: top; padding: 0;'>"
                    result+= Notes1 & Notes2 & Notes3 & Notes4 & Notes5 & Notes6
                result+="</td>"
            result+="</tr>"
        result+= "</table>"
        Return result
    End Function

    '#JobSheet Roller Blind

    
End Class
