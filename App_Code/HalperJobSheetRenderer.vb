Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic

Public Class HalperJobSheetRenderer

    '#Print Header
    Public Function PrintHeader(currentData As DataRow) As String
        Dim result As String = String.Empty
        Dim ReportType As String = String.Empty
        Dim ReportIcon As String = String.Empty
        Dim GoWith As String = String.Empty
        '#Validate 1
        Select Case  currentData("DesignName").ToString()
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

            '#--------Veri Shades-----------
            Case "Veri Shades"
                ReportIcon = "VR"
                GoWith = "VR"
                ReportType = "Verishade"
        End Select
        '#Validate 2
        Select Case  currentData("BlindName").ToString()
            '#--------Roller | Roller Blinds-----------
            Case "Roller Blind"
                ReportType = "Holland Blinds"
                ReportIcon = "H"

            '#--------Roller | Roller Motorised-----------
            Case "Motorised"
                ReportType = "Holland Motorised"
                ReportIcon = "HM"
                GoWith = "Motorised"

            '#--------Roller | Roller Cassette-----------
            Case "Cassette"
                ReportType = "Holland Cassette"
                ReportIcon = "HC"
                GoWith = "Cassette"

            '#--------Roller | Roller Skin-----------
            Case "Skin Only"
                ReportType = "Holland Skin"
                ReportIcon = "HSO"
                GoWith = "Skin"

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
        Dim Notes1 As String = currentData("Notes1").ToString()
        Dim Notes2 As String = currentData("Notes2").ToString()
        Dim Notes3 As String = currentData("Notes3").ToString()
        Dim Notes4 As String = currentData("Notes4").ToString()
        Dim Notes5 As String = currentData("Notes5").ToString()
        Dim Notes6 As String = currentData("Notes6").ToString()
        Dim hightColumnNotes As String = "height: 50px;"
        If Not String.IsNullOrEmpty(Notes1) Or Not String.IsNullOrEmpty(Notes2) Or Not String.IsNullOrEmpty(Notes3) Or Not String.IsNullOrEmpty(Notes4) Or Not String.IsNullOrEmpty(Notes5) Or Not String.IsNullOrEmpty(Notes6) Then hightColumnNotes = ""
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
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Job No</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("JoNumber").ToString() &"</th>"
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
