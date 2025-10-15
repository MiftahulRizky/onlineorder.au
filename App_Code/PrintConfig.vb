Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic

Public Class PrintConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim tableStart As String = "<table style='width:100%;border:1px solid black;border-collapse:collapse;margin-bottom:15px;'>"
    Dim tableEnd As String = "</table>"

    Dim trStart As String = "<tr>"
    Dim trEnd As String = "</tr>"

    Dim thStart As String = "<th style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Dim thStartColSpan2 As String = "<th colspan='2' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Dim thStartColSpan3 As String = "<th colspan='3' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Dim thStartColSpan4 As String = "<th colspan='4' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Dim thStartRowSpan2 As String = "<th rowspan='2' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Dim thEnd As String = "</th>"

    Dim tdStart As String = "<td style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Dim tdStartRowSpan2 As String = "<td rowspan='2' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Dim tdStartColSpan2 As String = "<td colspan='2' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Dim tdStartColSpan3 As String = "<td colspan='3' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Dim tdStartColSpan4 As String = "<td colspan='4' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Dim tdEnd As String = "</td>"

    Dim bNotesStart As String = "<b style='margin-left:100px;color:red;'>Notes: "
    Dim bNotesEnd As String = "</b>"

    Dim spanStart As String = "<span style='font-size:12px;font-weight:bold;'>"
    Dim spanEnd As String = "</span>"

    Dim queryQtyBlind As String = "SELECT COUNT(*) FROM view_details WHERE Active = 1 {0} {1}"

    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Public Function GetListData(thisString As String) As DataSet
        Dim thisCmd As New SqlCommand(thisString)
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                thisCmd.Connection = thisConn
                thisAdapter.SelectCommand = thisCmd
                Using thisDataSet As New DataSet()
                    thisAdapter.Fill(thisDataSet)
                    Return thisDataSet
                End Using
            End Using
        End Using
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

    Public Sub CreatePDFOrder(HeaderId As String, Directories As String, FileName As String)
        Dim result As String = String.Empty
        result = Print_HeaderTemplate(HeaderId)

        'ALUMINIUM
        result += Print_AluminiumBlinds(HeaderId)

        'VENETIAN
        result += Print_VenetianBlinds(HeaderId)

        'VERI SHADES
        result += Print_Verishades(HeaderId)
        result += Print_Verishades_Track(HeaderId)
        result += Print_Verishades_Slat(HeaderId)

        'VERTICAL
        result += Print_Vertical_Complete(HeaderId)
        result += Print_Vertical_Track(HeaderId)
        result += Print_Vertical_Slat(HeaderId)

        'ROLLER
        result += Print_Roller_SkinOnly(HeaderId)
        result += Print_RollerBlind(HeaderId)
        result += Print_Roller_Motorised(HeaderId)
        result += Print_Cassette(HeaderId)
        'result += Print_CassetteMotorised(HeaderId)

        Using stream As FileStream = New FileStream(Directories + "/" + FileName, FileMode.Create)
            Dim pdfDoc As Document = New Document(PageSize.A4.Rotate)
            Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, stream)
            pdfDoc.Open()
            Dim sr As StringReader = New StringReader(result)
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr)
            pdfDoc.NewPage()
            pdfDoc.Close()
            stream.Close()
        End Using
    End Sub

    Protected Function Print_HeaderTemplate(HeaderId As String) As String
        Dim result As String = String.Empty

        Dim headerData As DataSet = GetListData("SELECT * FROM view_headers WHERE Id = '" + HeaderId + "'")

        Dim orderNo As String = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
        Dim orderCust As String = headerData.Tables(0).Rows(0).Item("OrderCust").ToString()
        Dim delivery As String = headerData.Tables(0).Rows(0).Item("Delivery").ToString()
        Dim createdBy As String = headerData.Tables(0).Rows(0).Item("UserName").ToString()
        Dim note As String = headerData.Tables(0).Rows(0).Item("Note").ToString()
        Dim createdDate As String = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("dd MMM yyyy")
        Dim status As String = headerData.Tables(0).Rows(0).Item("Status").ToString()
        Dim storeId As String = headerData.Tables(0).Rows(0).Item("StoreId").ToString()
        Dim storeName As String = headerData.Tables(0).Rows(0).Item("StoreName").ToString()

        Dim request As HttpRequest = HttpContext.Current.Request
        Dim baseUrl As String = request.Url.Scheme & "://" & request.Url.Authority & request.ApplicationPath.TrimEnd("/"c)


        result += "<table style='width:100%;margin-bottom:10px;margin-top:25px;font-size:smaller;'>"
        result += trStart
        result += "<td style='vertical-align:top;width:40%;font-size:small;'>"
        result += "<img width='100%' src='" & baseUrl & "/Content/static/new-icon.png' alt='Your Logo'/>"
        result += "<br />"
        result += "<p style='font-size:small;'>"
        result += "<b>Sunlight Products Pty Ltd</b>"
        result += "<br />"
        result += "ABN 72 953 837 890"
        result += "<br /><br />"
        result += "Phone: 02 9688 1555"
        result += "<br />"
        result += "Fax: 02 9631 7555"
        result += "</p>"

        result += tdEnd

        result += "<td style='vertical-align:top;width:60%;font-size:small;'>"
        result += "<table style='width:100%;font-size:smaller;'>"
        result += trStart
        result += "<td style='vertical-align:top;font-size:small;'>"
        result += "<table style='width:100%;font-size:small;'>"

        result += trStart
        result += "<td style='width:170px;font-size:small;'>Store Name</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & storeName & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:large;'>Order Number</td>"
        result += "<td style='width:10px;font-size:large;'>:</td>"
        result += "<td style='font-size:large;'><b>" & orderNo & "</b>" & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:large;'>Reference</td>"
        result += "<td style='width:10px;font-size:large;'>:</td>"
        result += "<td style='font-size:large;'><b>" & orderCust & "</b>" & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:small;'>Delivery / Pick Up</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & delivery & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:small;'>Created</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & createdBy & " on " & createdDate & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;'>Status</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & status & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;'>Total Quantity Order</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND Active=1") & " Piece" & tdEnd
        result += trEnd
        result += tableEnd
        result += tdEnd
        result += trEnd

        'START NOTE
        If Not note = "" Then
            result += trStart
            result += "<td style='vertical-align:top;font-size:smaller;'>"
            result += "<table style='width:100%;font-size:smaller;'>"
            result += trStart
            result += "<td>Note :</td>"
            result += trEnd

            result += trStart
            result += tdStart & note & tdEnd
            result += trEnd

            result += tableEnd
            result += tdEnd
            result += trEnd
        End If
        'END NOTE

        'START DESCRIPTION
        result += trStart
        result += "<td style='vertical-align:top;font-size:smaller;'>"
        result += "<table style='width:100%;font-size:smaller;'>"
        result += trStart
        result += "<td>Description Quantity :</td>"
        result += trEnd

        result += trStart
        result += tdStart & BindDescOrderItem(HeaderId) & tdEnd
        result += trEnd

        result += tableEnd
        result += tdEnd
        result += trEnd
        'END DESCRIPTION

        result += tableEnd
        result += tdEnd
        result += trEnd
        result += tableEnd

        Return result
    End Function

    Protected Function Print_AluminiumBlinds(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Aluminium Blinds' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "ALUMINIUM BLINDS" & spanEnd
                result += tableStart
                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Bracket" & thEnd
                result += thStartRowSpan2 & "Bottom" & thEnd
                result += thStartColSpan2 & "Control" & thEnd
                result += thStartColSpan2 & "Top LHS" & thEnd
                result += thStartColSpan2 & "Top RHS" & thEnd
                result += thStartColSpan2 & "Bottom LHS" & thEnd
                result += thStartColSpan2 & "Bottom RHS" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Position" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim LHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Top").ToString()
                    Dim LHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Top").ToString()
                    Dim RHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Top").ToString()
                    Dim RHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Top").ToString()
                    Dim LHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Bottom").ToString()
                    Dim LHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Bottom").ToString()
                    Dim RHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Bottom").ToString()
                    Dim RHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Bottom").ToString()

                    If LHSWidth_Top = "0" Then : LHSWidth_Top = "" : End If
                    If LHSHeight_Top = "0" Then : LHSHeight_Top = "" : End If
                    If RHSWidth_Top = "0" Then : RHSWidth_Top = "" : End If
                    If RHSHeight_Top = "0" Then : RHSHeight_Top = "" : End If
                    If LHSWidth_Bottom = "0" Then : LHSWidth_Bottom = "" : End If
                    If LHSHeight_Bottom = "0" Then : LHSHeight_Bottom = "" : End If
                    If RHSWidth_Bottom = "0" Then : RHSWidth_Bottom = "" : End If
                    If RHSHeight_Bottom = "0" Then : RHSHeight_Bottom = "" : End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & tdEnd
                    result += tdStart & LHSWidth_Top & tdEnd
                    result += tdStart & LHSHeight_Top & tdEnd
                    result += tdStart & RHSWidth_Top & tdEnd
                    result += tdStart & RHSHeight_Top & tdEnd
                    result += tdStart & LHSWidth_Bottom & tdEnd
                    result += tdStart & LHSHeight_Bottom & tdEnd
                    result += tdStart & RHSWidth_Bottom & tdEnd
                    result += tdStart & RHSHeight_Bottom & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ALUMINIUM BLINDS ERROR CREATE PDF"
        End Try
        Return result
    End Function

    Protected Function Print_VenetianBlinds(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName ='Venetian Blinds' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='24' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "VENETIAN BLINDS" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Bottom" & thEnd
                result += thStartColSpan2 & "Control" & thEnd
                result += thStartColSpan3 & "Pelmet" & thEnd
                result += thStartColSpan2 & "Hand Return" & thEnd
                result += thStartColSpan2 & "Top LHS" & thEnd
                result += thStartColSpan2 & "Top RHS" & thEnd
                result += thStartColSpan2 & "Bottom LHS" & thEnd
                result += thStartColSpan2 & "Bottom RHS" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Position" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Type" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Size" & thEnd
                result += thStart & "Left" & thEnd
                result += thStart & "Right" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim LHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Top").ToString()
                    Dim LHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Top").ToString()
                    Dim RHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Top").ToString()
                    Dim RHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Top").ToString()
                    Dim LHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Bottom").ToString()
                    Dim LHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Bottom").ToString()
                    Dim RHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Bottom").ToString()
                    Dim RHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Bottom").ToString()

                    If LHSWidth_Top = "0" Then : LHSWidth_Top = "" : End If
                    If LHSHeight_Top = "0" Then : LHSHeight_Top = "" : End If
                    If RHSWidth_Top = "0" Then : RHSWidth_Top = "" : End If
                    If RHSHeight_Top = "0" Then : RHSHeight_Top = "" : End If
                    If LHSWidth_Bottom = "0" Then : LHSWidth_Bottom = "" : End If
                    If LHSHeight_Bottom = "0" Then : LHSHeight_Bottom = "" : End If
                    If RHSWidth_Bottom = "0" Then : RHSWidth_Bottom = "" : End If
                    If RHSHeight_Bottom = "0" Then : RHSHeight_Bottom = "" : End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetWidth").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetReturnSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetReturnSize2").ToString() & tdEnd
                    result += tdStart & LHSWidth_Top & tdEnd
                    result += tdStart & LHSHeight_Top & tdEnd
                    result += tdStart & RHSWidth_Top & tdEnd
                    result += tdStart & RHSHeight_Top & tdEnd
                    result += tdStart & LHSWidth_Bottom & tdEnd
                    result += tdStart & LHSHeight_Bottom & tdEnd
                    result += tdStart & RHSWidth_Bottom & tdEnd
                    result += tdStart & RHSHeight_Bottom & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF VENETIAN BLINDS"
        End Try
        Return result
    End Function

    Protected Function Print_Verishades(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Veri Shades' AND BlindName='Single' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='14' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "SINGLE VERI SHADES" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Fabric" & thEnd
                result += thStartRowSpan2 & "Stack" & thEnd
                result += thStartColSpan2 & "Track" & thEnd
                result += thStartColSpan2 & "Wand" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Type" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Size" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & "mm" & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF SINGLE VERI SHADES"
        End Try
        Return result
    End Function

    Protected Function Print_Verishades_Track(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Veri Shades' AND BlindName='Track Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='11' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "VERI SHADES TRACK ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Stack" & thEnd
                result += thStartColSpan2 & "Track" & thEnd
                result += thStartColSpan2 & "Wand" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Type" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Size" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ID").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF TRACK ONLY VERI SHADES"
        End Try
        Return result
    End Function

    Protected Function Print_Verishades_Slat(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Veri Shades' AND BlindName='Slat Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='8' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;'>"
                result += spanStart & "VERI SHADES SLAT ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Blind Size" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim blindSize As String = "No"
                    If thisData.Tables(0).Rows(i).Item("BlindSize").ToString() = "1" Then
                        blindSize = "Yes"
                    End If
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ID").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & blindSize & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF SKIN ONLY VERI SHADES"
        End Try
        Return result
    End Function

    Protected Function Print_Vertical_Complete(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Vertical Blinds' AND BlindName='Complete' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='21' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "COMPLETE VERTICAL BLIND" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Fabric/Slat Size" & thEnd
                result += thStart & "Stack" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain/Wand Colour" & thEnd
                result += thStart & "Control Length" & thEnd
                result += thStart & "Track" & thEnd
                result += thStart & "Brackets" & thEnd
                result += thStart & "Bracket Colour" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Insert In Track" & thEnd
                result += thStart & "Sloper" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim controlType As String = thisData.Tables(0).Rows(i).Item("ControlType").ToString()

                    Dim chainwandColour As String = thisData.Tables(0).Rows(i).Item("WandColour").ToString()
                    Dim chainwandLength As String = thisData.Tables(0).Rows(i).Item("WandLength").ToString()
                    If controlType = "Chain" Then
                        chainwandColour = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                        chainwandLength = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    End If

                    Dim insertInTrack As String = "No"
                    Dim sloper As String = "No"
                    If Not thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "" Then
                        insertInTrack = "Yes"
                    End If
                    If Not thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "" Then
                        sloper = "Yes"
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SlatSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & chainwandColour & tdEnd
                    result += tdStart & chainwandLength & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & insertInTrack & tdEnd
                    result += tdStart & sloper & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF VERTICAL COMPLETE"
        End Try
        Return result
    End Function

    Protected Function Print_Vertical_Track(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Vertical Blinds' AND BlindName='Track Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='18' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "VERTICAL TRACK ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Slat" & thEnd
                result += thStart & "Stack" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain/Wand" & thEnd
                result += thStart & "Length/Size" & thEnd
                result += thStart & "Track" & thEnd
                result += thStart & "Brackets" & thEnd
                result += thStart & "Bracket Colour" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Insert In Track" & thEnd
                result += thStart & "Sloper" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim controlType As String = thisData.Tables(0).Rows(i).Item("ControlType").ToString()

                    Dim chainwandColour As String = thisData.Tables(0).Rows(i).Item("WandColour").ToString()
                    Dim chainwandLength As String = thisData.Tables(0).Rows(i).Item("WandLength").ToString()
                    If controlType = "Chain" Then
                        chainwandColour = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                        chainwandLength = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    End If

                    Dim insertInTrack As String = "No"
                    Dim sloper As String = "No"
                    If Not thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "" Then
                        insertInTrack = "Yes"
                    End If
                    If Not thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "" Then
                        sloper = "Yes"
                    End If

                    Dim slat As String = thisData.Tables(0).Rows(i).Item("SlatSize").ToString()
                    slat += " - "
                    slat += thisData.Tables(0).Rows(i).Item("SlatQty").ToString()

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & slat & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & chainwandColour & tdEnd
                    result += tdStart & chainwandLength & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & insertInTrack & tdEnd
                    result += tdStart & sloper & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF TRACK ONLY VERTICAL"
        End Try
        Return result
    End Function

    Protected Function Print_Vertical_Slat(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Vertical Blinds' AND BlindName='Slat Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='10' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "VERTICAL SLAT ONLY" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Slat Size" & thEnd
                result += thStart & "Slat Qty" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricWidth").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SlatQty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF SKIN ONLY VERTICAL"
        End Try
        Return result
    End Function

    Protected Function Print_Roller_SkinOnly(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Skin Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='8' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "ROLLER SKIN ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Trim" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROLLER SKIN ONLY"
        End Try
        Return result
    End Function

    Protected Function Print_RollerBlind(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Roller Blind' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "ROLLER BLIND" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Roll" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain Colour" & thEnd
                result += thStart & "Chain Length" & thEnd
                result += thStart & "Trim" & thEnd
                result += thStart & "Bottom Rail" & thEnd
                result += thStart & "Tube" & thEnd
                result += thStart & "Childsafe" & thEnd
                result += thStart & "Accessory" & thEnd
                result += thStart & "Bracket Covers" & thEnd
                result += thStart & "Bracket Ext" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim bracketType As String = thisData.Tables(0).Rows(i).Item("BracketType").ToString()
                    Dim kitName As String = thisData.Tables(0).Rows(i).Item("KitName").ToString()

                    If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If
                    End If


                    If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 3" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & kitName & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TubeSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChildSafe").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketExtension").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result += "THERE IS AN ERROR IN THE ROLLER BLIND. PLEASE CONTACT <b>support@onlineorder.au</b>"
        End Try
        Return result
    End Function

    Protected Function Print_Roller_Motorised(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Motorised' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse: collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "ROLLER MOTORISED" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Roll" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Motor" & thEnd
                result += thStart & "Remote" & thEnd
                result += thStart & "Charger" & thEnd
                result += thStart & "Flush Connect" & thEnd
                result += thStart & "Trim" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Tube" & thEnd
                result += thStart & "Accessory" & thEnd
                result += thStart & "Bracket Covers" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim bracketType As String = thisData.Tables(0).Rows(i).Item("BracketType").ToString()
                    Dim kitName As String = thisData.Tables(0).Rows(i).Item("KitName").ToString()

                    If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If
                    End If


                    If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 3" Then
                            Dim getConnectedId As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & kitName & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorStyle").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorRemote").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorCharger").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Connector").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TubeSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                    result += trEnd
                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROLLER MOTORIZED"
        End Try
        Return result
    End Function

    Protected Function Print_Cassette(HeaderId As String) As String
        Dim result As String = String.Empty
        Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Cassette' AND ControlType='JAI Geared' AND Active=1 ORDER BY Id, BlindNo ASC")
        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim tdNotes As String = "<td colspan='18' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
            result += spanStart & "ROLLER CASSETTE - JAI SYSTEM" & spanEnd
            result += tableStart

            result += trStart
            result += thStart & "No" & thEnd
            result += thStart & "ID" & thEnd
            result += thStart & "Qty" & thEnd
            result += thStart & "Product" & thEnd
            result += thStart & "Location" & thEnd
            result += thStart & "Mounting" & thEnd
            result += thStart & "Width" & thEnd
            result += thStart & "Drop" & thEnd
            result += thStart & "Roll" & thEnd
            result += thStart & "Fabric" & thEnd
            result += thStart & "Control" & thEnd
            result += thStart & "Chain Colour" & thEnd
            result += thStart & "Chain Length" & thEnd
            result += thStart & "Trim" & thEnd
            result += thStart & "Bottom" & thEnd
            result += thStart & "ChildSafe" & thEnd
            result += thStart & "Accessory" & thEnd
            result += thStart & "Bracket Covers" & thEnd
            result += trEnd

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += trStart
                result += tdStart & i + 1 & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ChildSafe").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                result += trEnd

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += trStart
                    result += tdNotes
                    result += bNotesStart
                    result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                    result += bNotesEnd
                    result += tdEnd
                    result += trEnd
                End If
            Next
            result += tableEnd
        End If
        Return result
    End Function

    Protected Function Print_CassetteMotorised(HeaderId As String) As String
        Dim result As String = String.Empty
        Dim thisData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Cassette' AND ControlType='Motorised' AND Active=1 ORDER BY Id, BlindNo ASC")
        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim tdNotes As String = "<td colspan='19' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
            result += spanStart & "ROLLER CASSETTE - MOTORISED" & spanEnd
            result += tableStart

            result += trStart
            result += thStart & "No" & thEnd
            result += thStart & "ID" & thEnd
            result += thStart & "Qty" & thEnd
            result += thStart & "Product" & thEnd
            result += thStart & "Location" & thEnd
            result += thStart & "Mounting" & thEnd
            result += thStart & "Control" & thEnd
            result += thStart & "Fabric" & thEnd
            result += thStart & "Width" & thEnd
            result += thStart & "Drop" & thEnd
            result += thStart & "Motor" & thEnd
            result += thStart & "Remote" & thEnd
            result += thStart & "Charger" & thEnd
            result += thStart & "Flush Connect" & thEnd
            result += thStart & "Cable Exit" & thEnd
            result += thStart & "Trim" & thEnd
            result += thStart & "Bottom" & thEnd
            result += thStart & "Accessory" & thEnd
            result += thStart & "Bracket Covers" & thEnd
            result += trEnd

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += trStart
                result += tdStart & i + 1 & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ID").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("MotorStyle").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("MotorRemote").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("MotorCharger").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Connector").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("CableExitPoint").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                result += trEnd

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += trStart
                    result += tdNotes
                    result += bNotesStart
                    result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                    result += bNotesEnd
                    result += tdEnd
                    result += trEnd
                End If
            Next
            result += tableEnd
        End If
        Return result
    End Function

    Protected Function BindDescOrderItem(HeaderId As String) As String
        Dim result As String = ""

        Dim separted As String = " | "
        Dim totalAluminium As String = GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Aluminium Blinds' AND Active=1")
        Dim totalVenetian As String = GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Venetian Blinds' AND Active=1")
        Dim totalRoller As String = GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Roller Blinds' AND Active=1")
        Dim totalVerishades As String = GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Veri Shades' AND Active=1")
        Dim totalVertical As String = GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Vertical Blinds' AND Active=1")

        If totalAluminium = "" Then : totalAluminium = "-" : End If
        If totalVenetian = "" Then : totalVenetian = "-" : End If
        If totalRoller = "" Then : totalRoller = "-" : End If
        If totalVerishades = "" Then : totalVerishades = "-" : End If
        If totalVertical = "" Then : totalVertical = "-" : End If

        Dim aluminiumblinds As String = "<b>Aluminium Blinds: " & totalAluminium & "</b>"
        Dim venetianblinds As String = "<b>Venetian Blinds:  " & totalVenetian & "</b>"
        Dim rollerblinds As String = "<b>Roller Blinds: " & totalRoller & "</b>"
        Dim verishades As String = "<b>Veri Shades: " & totalVerishades & "</b>"
        Dim verticalblinds As String = "<b>Vertical Blinds: " & totalVertical & "</b>"

        result = aluminiumblinds & separted & venetianblinds & separted & rollerblinds & separted & verishades & separted & verticalblinds
        Return result
    End Function

    Public Sub CreatePDFQuote(Id As String, User As String, Dir As String, Name As String)
        Dim build As String = String.Empty

        Dim thisData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + Id + "'")

        Dim storeId As String = thisData.Tables(0).Rows(0).Item("StoreId").ToString

        build += BuildLogoQuote(storeId)
        build += "<br /><br /><br />"
        build += BuildHeaderQuote(Id, User)
        build += BuildDescQuote(Id)
        build += BuildTotalQuote(Id)

        build += "<hr />"

        build += BuildFooterQuote(storeId)

        Dim html As String = build

        Using stream As FileStream = New FileStream(Dir + "/" + Name, FileMode.Create)
            Dim pdfDoc As Document = New Document(PageSize.A4)
            Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, stream)
            pdfDoc.Open()
            Dim sr As StringReader = New StringReader(html)
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr)
            pdfDoc.Close()
            stream.Close()
        End Using
    End Sub

    Public Function BuildLogoQuote(StoreId As String) As String
        Dim result As String = String.Empty

        ' Dim fileDirectory As String = "https://images.onlineorder.au/img/stores/"
        Dim fileDirectory As String = "http://10.0.209.168:8888/Content/static/stores/"
        Dim fileName As String = GetItemData("SELECT [Image] FROM Stores WHERE Id = '" + StoreId + "'")

        Dim src As String = fileDirectory & fileName

        result += "<table style='width:100%;margin-bottom:25px;'>"
        result += trStart
        result += "<td valign='top' style='width:50%;'>"
        result += "<img width='200px' src='" & src & "' />"
        result += tdEnd
        result += "<td valign='bottom' style='width:50%;text-align:right;font-weight:bold;margin-right:20px;'>QUOTE ORDER</td>"
        result += trEnd
        result += tableEnd
        Return result
    End Function

    Public Function BuildHeaderQuote(HeaderId As String, UserName As String) As String
        Dim result As String = String.Empty

        Dim thisData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")

        Dim orderNo As String = thisData.Tables(0).Rows(0).Item("OrderNo").ToString
        Dim orderCust As String = thisData.Tables(0).Rows(0).Item("OrderCust").ToString
        Dim address As String = thisData.Tables(0).Rows(0).Item("Address").ToString
        Dim suburb As String = thisData.Tables(0).Rows(0).Item("Suburb").ToString
        Dim states As String = thisData.Tables(0).Rows(0).Item("States").ToString
        Dim postCode As String = thisData.Tables(0).Rows(0).Item("PostCode").ToString
        Dim phone As String = thisData.Tables(0).Rows(0).Item("Phone").ToString
        Dim email As String = thisData.Tables(0).Rows(0).Item("Email").ToString

        Dim fullAddress As String = address & " " & suburb
        fullAddress += "<br />" & states & " " & postCode

        tableStart = "<table style='width:100%;'>"
        Dim tdStartTitle1 As String = "<td valign='top' style='height:auto;width:25%;font-size:12px;padding-top:5px;padding-bottom:5px;'>"
        Dim tdStartTitle2 As String = "<td valign='top' style='height:auto;width:auto;font-size:12px;padding-top:5px;padding-bottom:5px;'>"
        Dim tdStartContent As String = "<td valign='top' style='height:auto;text-align:left;width:auto;font-size:12px;padding-top:5px;padding-bottom:5px;'>"

        result += tableStart
        result += trStart

        ' START COLUMN 1
        result += "<td valign='top' style='width:65%;'>"

        result += tableStart

        result += trStart
        result += tdStartTitle1 & "Customer Name:" & tdEnd
        result += tdStartContent & "<b>" & orderCust & "</b>" & tdEnd
        result += trEnd

        result += trStart
        result += tdStartTitle1 & "Address:" & tdEnd
        result += tdStartContent & fullAddress & tdEnd
        result += trEnd

        result += trStart
        result += tdStartTitle1 & "Phone:" & tdEnd
        result += tdStartContent & phone & tdEnd
        result += trEnd

        result += trStart
        result += tdStartTitle1 & "Email:" & tdEnd
        result += tdStartContent & email & tdEnd
        result += trEnd

        result += tableEnd

        result += tdEnd
        ' END COLUMN 1

        ' START COLUMN 2
        result += "<td valign='top' style='width:35%;'>"

        result += tableStart

        result += trStart
        result += tdStartTitle2 & "Quote No:" & tdEnd
        result += tdStartContent & "<b>" & orderNo & "</b>" & tdEnd
        result += trEnd

        result += trStart
        result += tdStartTitle2 & "Date:" & tdEnd
        result += tdStartContent & "<b>" & DateTime.Now.ToString("MMM dd, yyyy") & "</b>" & tdEnd
        result += trEnd

        result += trStart
        result += tdStartTitle2 & "Operator:" & tdEnd
        result += tdStartContent & "<b>" & UserName & "</b>" & tdEnd
        result += trEnd

        result += tableEnd

        result += tdEnd
        ' END COLUMN 2

        result += trEnd
        result += tableEnd
        Return result
    End Function

    Public Function BuildDescQuote(HeaderId As String) As String
        Dim result As String = String.Empty

        Dim detailData As DataSet = GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND Active=1 ORDER BY Id ASC")

        thStart = "<th style='color:white;background-color:#007ACC;width:{0};text-align:center;border:1px solid;height:26px;font-size:12px;'>"
        tdStart = "<td style='height:auto;word-wrap:break-word;border:1px solid;font-size:12px;border-collapse:collapse;padding-top:10px;padding-bottom:10px;padding-left:10px;'>"

        result += "<table style='margin-top:20px;width:100%;border-collapse: collapse;'>"

        result += trStart
        result += String.Format(thStart, "5%") & "Item" & thEnd
        result += String.Format(thStart, "19%") & "Room" & thEnd
        result += String.Format(thStart, "60%") & "Description" & thEnd
        result += String.Format(thStart, "16%") & "Unit Price" & thEnd
        result += trEnd

        For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
            Dim item As String = i + 1

            Dim designName As String = detailData.Tables(0).Rows(i).Item("DesignName").ToString()
            Dim blindName As String = detailData.Tables(0).Rows(i).Item("BlindName").ToString()

            Dim kitName As String = detailData.Tables(0).Rows(i).Item("KitName").ToString()
            Dim room As String = detailData.Tables(0).Rows(i).Item("Location").ToString()
            Dim fabricType As String = detailData.Tables(0).Rows(i).Item("FabricType").ToString()

            Dim cost As Decimal = detailData.Tables(0).Rows(i).Item("TotalMatrix") + detailData.Tables(0).Rows(i).Item("TotalCharge")
            Dim markUp As Decimal = detailData.Tables(0).Rows(i).Item("MarkUp")
            Dim unitPrice As Decimal = Math.Round(cost + (cost * markUp / 100), 2)

            Dim description As String = kitName

            If designName = "Roller Blind" Then
                description = kitName & " #" & fabricType
            End If

            If designName = "Veri Shades" Or designName = "Vertical Blind" Then
                description = kitName & " #" & fabricType
                If blindName = "Track Only" Then
                    description = kitName
                End If
            End If

            result += trStart
            result += tdStart & item & tdEnd
            result += tdStart & room & tdEnd
            result += tdStart & description & tdEnd

            result += tdStart
            result += "$" & unitPrice.ToString("N2", enUS)
            result += tdEnd

            result += trEnd
        Next
        result += tableEnd
        Return result
    End Function

    Public Function BuildTotalQuote(HeaderId As String) As String
        Dim result As String = String.Empty

        Dim sumPrice As Decimal = GetItemData_Decimal("SELECT SUM(TotalMatrix + TotalCharge + ((TotalMatrix + TotalCharge) * MarkUp / 100)) AS Cost FROM OrderDetails WHERE HeaderId = '" + HeaderId + "' AND Active=1")

        Dim dataQuote As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")
        Dim discount As Decimal = dataQuote.Tables(0).Rows(0).Item("QuoteDisc")
        Dim install As Decimal = If(dataQuote.Tables(0).Rows(0).Item("QuoteInstall").ToString() = "", 0, dataQuote.Tables(0).Rows(0).Item("QuoteInstall"))
        Dim measure As Decimal = dataQuote.Tables(0).Rows(0).Item("QuoteMeasure")
        Dim dataGst As String = dataQuote.Tables(0).Rows(0).Item("QuoteGST").ToString()

        Dim minCharge As Decimal = 0.00
        Dim gst As Decimal = 0.00
        Dim total As Decimal = 0.00
        Dim finalTotal As Decimal = 0.00

        total = sumPrice + gst + minCharge

        finalTotal = Math.Round(total - discount + measure + install, 2)

        tdStart = "<td style='text-align:right;height:30px;width:80%;font-size:12px;'>"

        Dim tdStartTitle As String = "<td style='text-align:right;height:30px;width:80%;font-size:12px;'>"
        Dim tdStartContent As String = "<td style='text-align:center;height:30px;width:20%;font-size: 12px;'>"

        result += "<table style='margin-top:40px;width:100%;'>"

        result += trStart
        result += tdStartTitle & "Sub Total : " & tdEnd
        result += tdStartContent
        result += "<b>$" & sumPrice.ToString("N2", enUS) & "</b>"
        result += tdEnd
        result += trEnd
        If dataGst = "No" Or dataGst = "" Then
            result += trStart
            result += tdStartTitle & "<b>TOTAL EXCLUDE GST : </b>" & tdEnd
            result += tdStartContent
            result += "<b>$" & total.ToString("N2", enUS) & "</b>"
            result += tdEnd
            result += trEnd
        End If

        If dataGst = "Yes" Then
            result += trStart
            result += tdStartTitle & "GST (10%) : " & tdEnd
            result += tdStartContent
            result += "<b>$" & gst.ToString("N2", enUS) & "</b>"
            result += tdEnd
            result += trEnd

            result += trStart
            result += tdStartTitle & "<b>TOTAL INCLUDE GST : </b>" & tdEnd
            result += tdStartContent
            result += "<b>$" & total.ToString("N2", enUS) & "</b>"
            result += tdEnd
            result += trEnd
        End If

        If discount > 0 Then
            result += trStart
            result += tdStartTitle & "Discount : " & tdEnd
            result += tdStartContent
            result += "<b>$" & discount.ToString("N2", enUS) & "</b>"
            result += tdEnd
            result += trEnd
        End If

        If install > 0 Then
            result += trStart
            result += tdStartTitle & "Installation : " & tdEnd
            result += tdStartContent
            result += "<b>$" & install.ToString("N2", enUS) & "</b>"
            result += tdEnd
            result += trEnd
        End If

        If measure > 0 Then
            result += trStart
            result += tdStartTitle & "Check Measure : " & tdEnd
            result += tdStartContent
            result += "<b>$" & measure.ToString("N2", enUS) & "</b>"
            result += tdEnd
            result += trEnd
        End If

        result += trStart
        result += tdStartTitle & "<b>GRAND TOTAL : </b>" & tdEnd
        result += tdStartContent
        result += "<b>$" & finalTotal.ToString("N2", enUS) & "</b>"
        result += tdEnd
        result += trEnd

        result += tableEnd
        Return result
    End Function

    Public Function BuildFooterQuote(StoreId As String) As String
        Dim result As String = String.Empty

        Dim resultStore As DataSet = GetListData("SELECT * FROM Stores WHERE Id = '" + StoreId + "'")

        Dim storeName As String = resultStore.Tables(0).Rows(0).Item("Name").ToString()
        Dim storeAddress As String = resultStore.Tables(0).Rows(0).Item("Address").ToString()
        Dim storePhone As String = resultStore.Tables(0).Rows(0).Item("Phone").ToString()
        Dim storeABN As String = resultStore.Tables(0).Rows(0).Item("ABN").ToString()
        Dim storeFax As String = resultStore.Tables(0).Rows(0).Item("Fax").ToString()
        Dim storeTerms As String = resultStore.Tables(0).Rows(0).Item("Terms").ToString()

        If Not storeAddress = "" Then
            result += "<p style='font-size:13px;'>Address : <b>" & storeAddress & "</b></p>"
        End If

        If storePhone = "" Then : storePhone = "-" : End If
        If storeFax = "" Then : storeFax = "-" : End If
        If storeABN = "" Then : storeABN = "-" : End If

        result += "<p style='font-size:13px;'>"
        result += "Phone : <b>" & storePhone & "</b>, "
        result += "Fax : <b>" & storeFax & "</b>, "
        result += "ABN : <b>" & storeABN & "</b>"
        result += "</p>"
        result += "<p style='font-size:13px;'><b>Terms & Conditions : </b></p>"
        result += "<p style='font-size:13px;'>" & storeTerms & "</p>"

        result += "<p style='margin-top:65px;font-size:13px;'><b>Best Regards</b></p>"
        result += "<p style='margin-top:5px;font-size:13px;'><b>" & storeName & "</b></p>"

        Return result
    End Function

    Protected Function BuildKitName(kitName As String, UniqueId As String, BlindNo As String) As String
        Dim result As String = kitName

        Return result
    End Function
End Class
