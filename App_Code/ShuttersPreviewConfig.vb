Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class ShuttersPreviewConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

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
        Dim result As String = String.Empty
        Try
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
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Double = 0
        Try
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
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Sub BindContent(Id As String, FilePath As String)
        Dim pdfFilePath As String = FilePath
        Using fs As New FileStream(pdfFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
            Dim headerData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + Id + "'")
            Dim customerId As String = headerData.Tables(0).Rows(0).Item("CustomerId").ToString()

            Dim orderId As String = headerData.Tables(0).Rows(0).Item("OrderId").ToString()
            Dim orderDate As DateTime = headerData.Tables(0).Rows(0).Item("CreatedDate")
            Dim submitDate As DateTime = If(IsDBNull(headerData.Tables(0).Rows(0).Item("SubmittedDate")), Nothing, Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("SubmittedDate")))
            Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id= '" + customerId + "'")
            Dim orderNumber As String = headerData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            Dim orderName As String = headerData.Tables(0).Rows(0).Item("OrderName").ToString()
            Dim orderNote As String = headerData.Tables(0).Rows(0).Item("OrderNote").ToString()

            Dim addressData As DataSet = GetListData("SELECT * FROM CustomerAddress WHERE CustomerId = '" + customerId + "' AND [Primary]=1")

            Dim orderAddress As String = String.Empty
            Dim orderPort As String = String.Empty

            If Not addressData.Tables(0).Rows.Count = 0 Then
                Dim unitNumber As String = addressData.Tables(0).Rows(0).Item("UnitNumber").ToString()
                Dim street As String = addressData.Tables(0).Rows(0).Item("Street").ToString()
                Dim suburb As String = addressData.Tables(0).Rows(0).Item("Suburb").ToString()
                Dim states As String = addressData.Tables(0).Rows(0).Item("States").ToString()
                Dim postCode As String = addressData.Tables(0).Rows(0).Item("PostCode").ToString()

                orderPort = addressData.Tables(0).Rows(0).Item("Port").ToString()

                orderAddress = street & ", " & suburb & " " & states & " " & postCode
                If Not unitNumber = "" Then
                    orderAddress = unitNumber & " " & street & ", " & suburb & " " & states & " " & postCode
                End If
            End If

            Dim totalItems As Integer = GetItemData("SELECT SUM(TotalBlinds) FROM OrderDetails WHERE HeaderId = '" + Id + "' AND Active = 1")

            Dim doc As New Document(PageSize.A4, 20, 20, 130, 50)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

            Dim pageEvent As New ShuttersPreviewEvents() With {
                .PageOrderId = orderId,
                .PageOrderDate = orderDate,
                .PageSubmitDate = submitDate,
                .PageCustomerName = customerName,
                .PageOrderNumber = orderNumber,
                .PageOrderName = orderName,
                .PageNote = orderNote,
                .PageAddress = orderAddress,
                .PagePort = orderPort,
                .PageTotalItem = totalItems
            }
            writer.PageEvent = pageEvent

            doc.Open()

            ' START PANORAMA PVC PARTS
            Try
                Dim panoramaPartsData As DataSet = GetListData("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Panorama PVC Parts' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")
                If Not panoramaPartsData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "PANORAMA PVC PARTS"
                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = panoramaPartsData.Tables(0)
                    Dim items(7, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim length As String = String.Empty
                        If dt.Rows(i)("PartLength") > 0 Then
                            length = dt.Rows(i)("PartLength").ToString()
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Qty").ToString()
                        items(2, i) = dt.Rows(i)("PartCategory").ToString()
                        items(3, i) = dt.Rows(i)("PartComponent").ToString()
                        items(4, i) = dt.Rows(i)("PartColour").ToString()
                        items(5, i) = length
                        items(6, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 9)

                        Dim headers As String() = {"", "Qty", "Category", "Component", "Colour", "Length", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 3, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 3
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 22
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END PANORAMA PVC PARTS

            ' START PANORAMA PVC SHUTTERS
            Try
                Dim panoramaData As DataSet = GetListData("SELECT OrderDetails.*, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Panorama PVC Shutters' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not panoramaData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Panorama PVC"
                    pageEvent.PageTitle2 = "Shutters"
                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = panoramaData.Tables(0)
                    Dim items(41, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim layoutCode As String = If(dt.Rows(i)("Layout").ToString() = "Other", dt.Rows(i)("LayoutSpecial").ToString(), dt.Rows(i)("Layout").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = dt.Rows(i)("MidrailHeight1")
                        Dim midrailHeight2 As Integer = dt.Rows(i)("MidrailHeight2")

                        If midrailHeight1 > 0 Then midrailList.Add("1 : " & midrailHeight1)
                        If midrailHeight2 > 0 Then midrailList.Add("2 : " & midrailHeight2)

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim gap1 As Integer = dt.Rows(i)("LocationTPost1")
                        Dim gap2 As Integer = dt.Rows(i)("LocationTPost2")
                        Dim gap3 As Integer = dt.Rows(i)("LocationTPost3")
                        Dim gap4 As Integer = dt.Rows(i)("LocationTPost4")
                        Dim gap5 As Integer = dt.Rows(i)("LocationTPost5")

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = dt.Rows(i)("SplitHeight1")
                        Dim split2 As Integer = dt.Rows(i)("SplitHeight2")
                        Dim splitHeigth As String = "Height 1 : " & split1 & ", " & "Height 2 : " & split2

                        Dim horizontalHeight As String = String.Empty
                        If dt.Rows(i)("HorizontalTPostHeight") > 0 Then
                            horizontalHeight = dt.Rows(i)("HorizontalTPostHeight").ToString()
                        End If

                        Dim bottomTrack As String = dt.Rows(i)("BottomTrackType").ToString()
                        If dt.Rows(i)("BottomTrackRecess").ToString() = "Yes" Then
                            bottomTrack = dt.Rows(i)("BottomTrackType").ToString() & " | Recess: Yes"
                        End If
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Width").ToString()
                        items(3, i) = dt.Rows(i)("Drop").ToString()
                        items(4, i) = dt.Rows(i)("Mounting").ToString()
                        items(5, i) = dt.Rows(i)("ColourType").ToString()
                        items(6, i) = dt.Rows(i)("LouvreSize").ToString()
                        items(7, i) = dt.Rows(i)("LouvrePosition").ToString()
                        items(8, i) = midrailHeight
                        items(9, i) = dt.Rows(i)("MidrailCritical").ToString()
                        items(10, i) = dt.Rows(i)("HingeColour").ToString()
                        items(11, i) = dt.Rows(i)("BlindName").ToString()
                        items(12, i) = dt.Rows(i)("SemiInsideMount").ToString()
                        items(13, i) = dt.Rows(i)("PanelQty").ToString()
                        items(14, i) = dt.Rows(i)("CustomHeaderLength").ToString()
                        items(15, i) = dt.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = dt.Rows(i)("FrameType").ToString()
                        items(18, i) = dt.Rows(i)("FrameLeft").ToString()
                        items(19, i) = dt.Rows(i)("FrameRight").ToString()
                        items(20, i) = dt.Rows(i)("FrameTop").ToString()
                        items(21, i) = dt.Rows(i)("FrameBottom").ToString()
                        items(22, i) = bottomTrack
                        items(23, i) = dt.Rows(i)("Buildout").ToString()
                        items(24, i) = dt.Rows(i)("BuildoutPosition").ToString()
                        items(25, i) = dt.Rows(i)("PanelSize").ToString()
                        items(26, i) = gapPosition
                        items(27, i) = horizontalHeight
                        items(28, i) = dt.Rows(i)("HorizontalTPost").ToString()
                        items(29, i) = dt.Rows(i)("TiltrodType").ToString()
                        items(30, i) = dt.Rows(i)("TiltrodSplit").ToString()
                        items(31, i) = splitHeigth
                        items(32, i) = dt.Rows(i)("ReverseHinged").ToString()
                        items(33, i) = dt.Rows(i)("PelmetFlat").ToString()
                        items(34, i) = dt.Rows(i)("ExtraFascia").ToString()
                        items(35, i) = dt.Rows(i)("HingesLoose").ToString()
                        items(36, i) = dt.Rows(i)("DoorCutOut").ToString()
                        items(37, i) = dt.Rows(i)("SpecialShape").ToString()
                        items(38, i) = dt.Rows(i)("TemplateProvided").ToString()
                        items(39, i) = FormatNumber(dt.Rows(i)("SquareMetre"), 2)
                        items(40, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 7, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 7)

                        Dim headers As String() = {"", "Room / Location", "Width (mm)", "Height (mm)", "Mounting", "Colour", "Louvre Size", "Sliding Louvre Position", "Midrail Height (mm)", "Critical Midrail", "Hinge Colour", "Installation Method", "Semi Inside Mount", "Panel Qty", "Custom Header Length (mm)", "Co-joined Panels", "Layout Code", "Frame Type", "Left Frame", "Right Frame", "Top Frame", "Bottom Frame", "Bottom Track", "Buildout", "Buildout Position", "Same Size Panel", "Gap / T-Post (mm)", "Hor T-Post Height (mm)", "Hor T-Post Required", "Tiltrod Type", "Split Tiltrod Rotation", "Split Height (mm)", "Reverse Hinged", "Pelmet Flat Packed", "Extra Fascia", "Hinges Loose", "French Door Cut-Out", "Special Shape", "Template Provided", "M2", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_CENTER
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 16
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 3, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_CENTER
                                cellContent.MinimumHeight = 16
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 3
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_CENTER
                                emptyCell.MinimumHeight = 16
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END PANORAMA PVC SHUTTERS

            ' START EVOLVE SHUTTERS
            Try
                Dim evolveData As DataSet = GetListData("SELECT OrderDetails.*, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Evolve Shutters' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not evolveData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Evolve"
                    pageEvent.PageTitle2 = "Shutters"
                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = evolveData.Tables(0)
                    Dim items(36, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim layoutCode As String = If(dt.Rows(i)("Layout").ToString() = "Other", dt.Rows(i)("LayoutSpecial").ToString(), dt.Rows(i)("Layout").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = dt.Rows(i)("MidrailHeight1")
                        Dim midrailHeight2 As Integer = dt.Rows(i)("MidrailHeight2")

                        If midrailHeight1 > 0 Then midrailList.Add("1 : " & midrailHeight1)
                        If midrailHeight2 > 0 Then midrailList.Add("2 : " & midrailHeight2)

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim gap1 As Integer = dt.Rows(i)("LocationTPost1")
                        Dim gap2 As Integer = dt.Rows(i)("LocationTPost2")
                        Dim gap3 As Integer = dt.Rows(i)("LocationTPost3")
                        Dim gap4 As Integer = dt.Rows(i)("LocationTPost4")
                        Dim gap5 As Integer = dt.Rows(i)("LocationTPost5")

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = dt.Rows(i)("SplitHeight1")
                        Dim split2 As Integer = dt.Rows(i)("SplitHeight2")
                        Dim splitHeigth As String = "Height 1 : " & split1 & ", " & "Height 2 : " & split2

                        Dim customHeaderLength As String = dt.Rows(i)("CustomHeaderLength").ToString()
                        If customHeaderLength = "" Or customHeaderLength = "0" Then customHeaderLength = String.Empty

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Width").ToString()
                        items(3, i) = dt.Rows(i)("Drop").ToString()
                        items(4, i) = dt.Rows(i)("Mounting").ToString()
                        items(5, i) = dt.Rows(i)("SemiInsideMount").ToString()
                        items(6, i) = dt.Rows(i)("ColourType").ToString()
                        items(7, i) = dt.Rows(i)("LouvreSize").ToString()
                        items(8, i) = dt.Rows(i)("LouvrePosition").ToString()
                        items(9, i) = midrailHeight
                        items(10, i) = dt.Rows(i)("MidrailCritical").ToString()
                        items(11, i) = dt.Rows(i)("HingeColour").ToString()
                        items(12, i) = dt.Rows(i)("BlindName").ToString()
                        items(13, i) = dt.Rows(i)("PanelQty").ToString()
                        items(14, i) = customHeaderLength
                        items(15, i) = dt.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = dt.Rows(i)("FrameType").ToString()
                        items(18, i) = dt.Rows(i)("FrameLeft").ToString()
                        items(19, i) = dt.Rows(i)("FrameRight").ToString()
                        items(20, i) = dt.Rows(i)("FrameTop").ToString()
                        items(21, i) = dt.Rows(i)("FrameBottom").ToString()
                        items(22, i) = dt.Rows(i)("Buildout").ToString()
                        items(23, i) = dt.Rows(i)("PanelSize").ToString()
                        items(24, i) = gapPosition
                        items(25, i) = dt.Rows(i)("HorizontalTPostHeight").ToString()
                        items(26, i) = dt.Rows(i)("HorizontalTPost").ToString()
                        items(27, i) = dt.Rows(i)("TiltrodType").ToString()
                        items(28, i) = dt.Rows(i)("TiltrodSplit").ToString()
                        items(29, i) = splitHeigth
                        items(30, i) = dt.Rows(i)("ReverseHinged").ToString()
                        items(31, i) = dt.Rows(i)("PelmetFlat").ToString()
                        items(32, i) = dt.Rows(i)("ExtraFascia").ToString()
                        items(33, i) = dt.Rows(i)("HingesLoose").ToString()
                        items(34, i) = FormatNumber(dt.Rows(i)("SquareMetre"), 2)
                        items(35, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Width (mm)", "Height (mm)", "Mounting", "Semi Inside Mount", "Colour", "Louvre Size", "Sliding Louvre Position", "Midrail Height (mm)", "Critical Midrail", "Hinge Colour", "Installation Method", "Panel Qty", "Custom Header Length (mm)", "Co-joined Panels", "Layout Code", "Frame Type", "Left Frame", "Right Frame", "Top Frame", "Bottom Frame", "Buildout", "Same Size Panel", "Gap / T-Post (mm)", "Hor T-Post Height (mm)", "Hor T-Post Required", "Tiltrod Type", "Split Tiltrod", "Split Height (mm)", "Reverse Hinged", "Pelmet Flat Packed", "Extra Fascia", "Hinges Loose", "M2", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_CENTER
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 16
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 3, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_CENTER
                                cellContent.MinimumHeight = 16
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 3
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_CENTER
                                emptyCell.MinimumHeight = 16
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END EVOLVE SHUTTERS
            doc.Close()
            writer.Close()
        End Using
    End Sub
End Class

Public Class ShuttersPreviewEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String
    Public Property PageTitle2 As String
    Public Property PageOrderId As String
    Public Property PageOrderDate As DateTime
    Public Property PageSubmitDate As DateTime
    Public Property PageCustomerName As String
    Public Property PageOrderNumber As String
    Public Property PageOrderName As String
    Public Property PageNote As String
    Public Property PageAddress As String
    Public Property PagePort As String
    Public Property PageTotalItem As Integer
    Public Property PageTotalDoc As Integer

    Private baseFont As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
    Private template As PdfTemplate

    Public Overrides Sub OnOpenDocument(writer As PdfWriter, document As Document)
        template = writer.DirectContent.CreateTemplate(50, 50)
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        If template Is Nothing Then
            template = cb.CreateTemplate(50, 50)
        End If

        Dim headerTable As New PdfPTable(3)
        headerTable.TotalWidth = document.PageSize.Width - 40
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.3F, 0.55F, 0.15F})

        Dim nestedTable As New PdfPTable(3)
        nestedTable.TotalWidth = headerTable.TotalWidth * 0.5F
        nestedTable.SetWidths(New Single() {0.25F, 0.05F, 0.7F})

        Dim phraseFirst As New Phrase()
        phraseFirst.Add(New Chunk(PageTitle, New Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD)))
        If Not String.IsNullOrEmpty(PageTitle2) Then
            phraseFirst.Add(New Chunk(Environment.NewLine))
            phraseFirst.Add(New Chunk(PageTitle2, New Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD)))
        End If

        Dim firstHeaderCell As New PdfPCell(phraseFirst)
        firstHeaderCell.Border = 0
        firstHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        firstHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(firstHeaderCell)

        Dim labels As String() = {"Order No", "Created Date", "Submitted Date", "Retailer", "Retailer Order No", "Customer Name", "Note", "Total Item Order"}
        Dim values As String() = {PageOrderId, PageOrderDate.ToString("dd MMM yyyy"), PageSubmitDate.ToString("dd MMM yyyy"), PageCustomerName, PageOrderNumber, PageOrderName, PageNote, PageTotalItem}

        For i As Integer = 0 To labels.Length - 1
            nestedTable.AddCell(New PdfPCell(New Phrase(labels(i), New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {
                .Border = 0,
                .HorizontalAlignment = Element.ALIGN_LEFT
            })
            nestedTable.AddCell(New PdfPCell(New Phrase(":", New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {
                .Border = 0,
                .HorizontalAlignment = Element.ALIGN_CENTER
            })
            nestedTable.AddCell(New PdfPCell(New Phrase(values(i), New Font(Font.FontFamily.TIMES_ROMAN, 8))) With {
                .Border = 0,
                .HorizontalAlignment = Element.ALIGN_LEFT
            })
        Next

        Dim secondHeaderCell As New PdfPCell(nestedTable) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_LEFT
        }
        headerTable.AddCell(secondHeaderCell)

        Dim phraseThird As New Phrase()
        phraseThird.Add(New Chunk("Delivery Address: ", New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)))
        phraseThird.Add(New Chunk(Environment.NewLine))
        phraseThird.Add(New Chunk(PageAddress, New Font(Font.FontFamily.TIMES_ROMAN, 8)))
        phraseThird.Add(New Chunk(Environment.NewLine & Environment.NewLine))
        phraseThird.Add(New Chunk("Nearest Port: ", New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)))
        phraseThird.Add(New Chunk(Environment.NewLine))
        phraseThird.Add(New Chunk(PagePort, New Font(Font.FontFamily.TIMES_ROMAN, 8)))

        Dim thirdHeaderCell As New PdfPCell(phraseThird)
        thirdHeaderCell.Border = 0
        thirdHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        thirdHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(thirdHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 20, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True
        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("Print Date: " & Now.ToString("dd MMM yyyy"), New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_LEFT
        }
        footerTable.AddCell(leftFooterCell)

        Dim pageText As String = "Page " & writer.PageNumber.ToString() & " of "
        Dim pageFont As New Font(Font.FontFamily.TIMES_ROMAN, 8)
        Dim rightFooterCell As New PdfPCell(New Phrase(pageText, pageFont)) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_RIGHT
        }
        footerTable.AddCell(rightFooterCell)

        Dim footerY As Single = document.PageSize.GetBottom(30)
        footerTable.WriteSelectedRows(0, -1, 36, footerY, cb)

        Dim baseFont As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
        Dim textWidth As Single = baseFont.GetWidthPoint(pageText, 8)
        Dim xPos As Single = document.PageSize.Width - textWidth - 1
        Dim yPos As Single = footerY - 10.0F

        cb.AddTemplate(template, xPos, yPos)
    End Sub

    Public Overrides Sub OnCloseDocument(writer As PdfWriter, document As Document)
        template.BeginText()
        template.SetFontAndSize(baseFont, 8)
        template.SetTextMatrix(0, 0)
        template.ShowText((writer.PageNumber - 1).ToString())
        template.EndText()

        PageTotalDoc = writer.PageNumber

        MyBase.OnCloseDocument(writer, document)
    End Sub
End Class