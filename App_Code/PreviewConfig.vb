Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Public Class PreviewConfig
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

            Dim pageEvent As New PreviewEvents() With {
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

            ' START ALUMINIUM BLIND
            Try
                Dim aluminiumData As DataSet = GetListData("SELECT OrderDetails.*, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Aluminium Blind' AND OrderDetails.Active=1 ORDER BY OrderDetails.Number ASC")
                If Not aluminiumData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Venetian Blind"
                    pageEvent.PageTitle2 = "Aluminium"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = aluminiumData.Tables(0)
                    Dim items(13, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("ColourType").ToString()
                        items(7, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(8, i) = dt.Rows(i)("TilterPosition").ToString()
                        items(9, i) = dt.Rows(i)("ControlLength").ToString()
                        items(10, i) = dt.Rows(i)("WandLength").ToString()
                        items(11, i) = dt.Rows(i)("Supply").ToString()
                        items(12, i) = dt.Rows(i)("Notes").ToString()
                    Next
                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Venetian Type", "Colour", "Control Position", "Tilter Position", "Cord Length (mm)", "Wand Length (mm)", "Hold Down Clip", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
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
            ' END ALUMINIUM BLIND

            ' START PRIVACY VENETIAN
            Try
                Dim privacyData As DataSet = GetListData("SELECT OrderDetails.*, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'LS Venetian' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")
                If Not privacyData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Venetian Blind"
                    pageEvent.PageTitle2 = "Privacy"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = privacyData.Tables(0)
                    Dim items(9, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("ColourType").ToString()
                        items(7, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(8, i) = dt.Rows(i)("Notes").ToString()
                    Next
                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Privacy Type", "Colour", "Control Position", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
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
            ' END PRIVACY VENETIAN

            ' START VENETIAN BLIND
            Try
                Dim venetianData As DataSet = GetListData("SELECT OrderDetails.*, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Venetian Blind' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")
                If Not venetianData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Venetian Blind"
                    pageEvent.PageTitle2 = "Basswood & Econo"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = venetianData.Tables(0)
                    Dim items(17, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim returnLength As String = String.Empty
                        If dt.Rows(i)("ValanceReturnLength") > 0 Then
                            returnLength = dt.Rows(i)("ValanceReturnLength")
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("ColourType").ToString()
                        items(7, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(8, i) = dt.Rows(i)("TilterPosition").ToString()
                        items(9, i) = dt.Rows(i)("ControlLength").ToString()
                        items(10, i) = dt.Rows(i)("Tassel").ToString()
                        items(11, i) = dt.Rows(i)("ValanceType").ToString()
                        items(12, i) = dt.Rows(i)("ValanceSize").ToString()
                        items(13, i) = dt.Rows(i)("ValanceReturnPosition").ToString()
                        items(14, i) = returnLength
                        items(15, i) = dt.Rows(i)("Supply").ToString()
                        items(16, i) = dt.Rows(i)("Notes").ToString()
                    Next
                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If
                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Venetian Type", "Colour", "Control Position", "Tilter Position", "Cord Length (mm)", "Tassel Option", "Valance Type", "Valance Size (mm)", "Valance Return Position", "Return Length (mm)", "Hold Down Clip", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
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
            ' END VENETIAN BLIND

            ' START CELLULAR SHADES
            Try
                Dim cellularData As DataSet = GetListData("SELECT OrderDetails.*, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Cellular Shades' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not cellularData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Cellular Shades"
                    pageEvent.PageTitle2 = "Standard"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = cellularData.Tables(0)
                    Dim items(12, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")
                        Dim number As Integer = i + 1

                        Dim controlType As String = dt.Rows(i)("ControlType").ToString()
                        Dim controlPosition As String = String.Empty
                        Dim controlLength As String = String.Empty

                        If controlType = "Corded" Then
                            controlPosition = dt.Rows(i)("ControlPosition").ToString()
                            controlLength = dt.Rows(i)("ControlLength").ToString()
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = fabricName
                        items(6, i) = fabricColour
                        items(7, i) = controlType
                        items(8, i) = controlPosition
                        items(9, i) = controlLength
                        items(10, i) = dt.Rows(i)("Supply").ToString()
                        items(11, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Fabric Type", "Fabric Colour", "Control Type", "Control Position", "Control Length (mm)", "Hold Down Clip", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
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
            ' END CELLULAR SHADES

            ' START CURTAIN
            Try
                Dim curtainData As DataSet = GetListData("SELECT OrderDetails.*, Products.ControlType AS ControlType, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Curtain' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not curtainData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Curtain"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = curtainData.Tables(0)
                    Dim items(15, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim number As Integer = i + 1

                        Dim returnSize As String = String.Empty
                        If dt.Rows(i)("ValanceReturnLength") > 0 Then
                            returnSize = dt.Rows(i)("ValanceReturnLength").ToString()
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("ControlType").ToString()
                        items(7, i) = dt.Rows(i)("ColourType").ToString()
                        items(8, i) = fabricName
                        items(9, i) = fabricColour
                        items(10, i) = dt.Rows(i)("Lining").ToString()
                        items(11, i) = dt.Rows(i)("StackOption").ToString()
                        items(12, i) = dt.Rows(i)("Supply").ToString()
                        items(13, i) = returnSize
                        items(14, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Fitting", "Width (mm)", "Drop (mm)", "Curtain Heading", "Track Type", "Track Colour", "Fabric Type", "Fabric Colour", "Lining", "Drawing", "HEM", "Return Size", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END CURTAIN

            ' START PANEL GLIDE
            Try
                Dim pgData As DataSet = GetListData("SELECT OrderDetails.*, Products.TubeType AS TubeType, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Panel Glide' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not pgData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Panel Glide"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = pgData.Tables(0)
                    Dim items(18, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim layoutCode As String = dt.Rows(i)("Layout").ToString()
                        If dt.Rows(i)("Layout").ToString() = "S" Then
                            layoutCode = dt.Rows(i)("LayoutSpecial").ToString()
                        End If

                        Dim blindName As String = dt.Rows(i)("BlindName").ToString()
                        Dim drop As String = dt.Rows(i)("Drop").ToString()
                        Dim trackLength As String = String.Empty

                        If blindName = "Track and Panel" Then
                            If dt.Rows(i)("TrackLength") > 0 Then
                                trackLength = dt.Rows(i)("TrackLength").ToString()
                            End If
                        End If
                        If blindName = "Track Only" Then
                            drop = String.Empty
                            If dt.Rows(i)("TrackLength") > 0 Then
                                trackLength = dt.Rows(i)("TrackLength").ToString()
                            End If
                        End If

                        Dim wandLength As String = If(IsDBNull(dt.Rows(i)("WandLength")), Nothing, dt.Rows(i)("WandLength").ToString())

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = drop
                        items(5, i) = blindName
                        items(6, i) = dt.Rows(i)("TubeType").ToString()
                        items(7, i) = dt.Rows(i)("ColourType").ToString()
                        items(8, i) = fabricName
                        items(9, i) = fabricColour
                        items(10, i) = dt.Rows(i)("TrackType").ToString()
                        items(11, i) = trackLength
                        items(12, i) = dt.Rows(i)("Supply").ToString()
                        items(13, i) = wandLength
                        items(14, i) = layoutCode
                        items(15, i) = dt.Rows(i)("Batten").ToString()
                        items(16, i) = dt.Rows(i)("BattenB").ToString()
                        items(17, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Panel System", "Panel Style", "Track Colour", "Fabric Type", "Fabric Colour", "Track Type", "Custom Track Length (mm)", "Track in One Piece", "Wand Length", "Layout Code", "Front Batten Colour", "Rear Batten Colour", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END PANEL GLIDE

            ' START PELMET
            Try
                Dim pelmetData As DataSet = GetListData("SELECT OrderDetails.*, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Pelmet' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not pelmetData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Pelmet"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = pelmetData.Tables(0)
                    Dim items(12, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("BlindName").ToString()
                        items(5, i) = dt.Rows(i)("ColourType").ToString()
                        items(6, i) = fabricName
                        items(7, i) = fabricColour
                        items(8, i) = dt.Rows(i)("PelmetFor").ToString()
                        items(9, i) = dt.Rows(i)("ValanceReturnLength").ToString()
                        items(10, i) = dt.Rows(i)("BracketSize").ToString()
                        items(11, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Pelmet Type", "Colour", "Fabric Type", "Fabric Colour", "Pelmet For", "Return Size (mm)", "Bracket Size", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END PELMET

            ' START ROLLER BLIND
            Try
                Dim rollerData As DataSet = GetListData("SELECT OrderDetails.Id, OrderDetails.Number, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.Width AS Width, OrderDetails.[Drop] AS Height, Blinds.Name AS BlindName, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricId AS Fabric, OrderDetails.FabricColourId AS FabricColour, OrderDetails.Roll, OrderDetails.ControlPosition, OrderDetails.ChainId AS Chain, OrderDetails.ControlLength AS ControlLength, OrderDetails.RemoteChannel, OrderDetails.BatteryCharger, OrderDetails.ExtensionCable, OrderDetails.BottomRailId AS BottomRail, OrderDetails.WandExtendable AS BottomExtendable, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.Adjusting, OrderDetails.Notes, 1 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Roller Blind' AND OrderDetails.Active = 1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Number, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthB AS Width, OrderDetails.DropB AS Height, Blinds.Name AS BlindName, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdB AS Fabric, OrderDetails.FabricColourIdB AS FabricColour, OrderDetails.RollB, OrderDetails.ControlPositionB, OrderDetails.ChainIdB AS Chain, OrderDetails.ControlLengthB AS ControlLength, OrderDetails.RemoteChannel, OrderDetails.BatteryCharger, OrderDetails.ExtensionCable, OrderDetails.BottomRailIdB AS BottomRail, OrderDetails.WandExtendableB AS BottomExtendable, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.Adjusting, OrderDetails.Notes, 2 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Roller Blind' AND (Blinds.Name IN ('Double Bracket', 'Link 2 Blinds Dep', 'Link 2 Blinds Head to Tail', 'Link 2 Blinds Ind', 'Link 3 Blinds Dep', 'Link 3 Blinds Head to Tail with Ind', 'Link 3 Blinds Ind with Dep', 'Link 4 Blinds Ind with Dep', 'DB Link 2 Blinds Dep', 'DB Link 2 Blinds Ind', 'DB Link 3 Blinds Dep', 'DB Link 3 Blinds Ind with Dep')) AND OrderDetails.Active = 1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Number, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthC AS Width, OrderDetails.DropC AS Height, Blinds.Name AS BlindName, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdC AS Fabric, OrderDetails.FabricColourIdC AS FabricColour, OrderDetails.RollC, OrderDetails.ControlPositionC, OrderDetails.ChainIdC AS Chain, OrderDetails.ControlLengthC AS ControlLength, OrderDetails.RemoteChannel, OrderDetails.BatteryCharger, OrderDetails.ExtensionCable, OrderDetails.BottomRailIdC AS BottomRail, OrderDetails.WandExtendableC AS BottomExtendable, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.Adjusting, OrderDetails.Notes, 3 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Roller Blind' AND (Blinds.Name IN ('Link 3 Blinds Dep', 'Link 3 Blinds Head to Tail with Ind', 'Link 3 Blinds Ind with Dep', 'Link 4 Blinds Ind with Dep', 'DB Link 2 Blinds Dep', 'DB Link 2 Blinds Ind', 'DB Link 3 Blinds Dep', 'DB Link 3 Blinds Ind with Dep')) AND OrderDetails.Active = 1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Number, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthC AS Width, OrderDetails.DropC AS Height, Blinds.Name AS BlindName, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdC AS Fabric, OrderDetails.FabricColourIdC AS FabricColour, OrderDetails.RollC, OrderDetails.ControlPositionC, OrderDetails.ChainIdC AS Chain, OrderDetails.ControlLengthC AS ControlLength, OrderDetails.RemoteChannel, OrderDetails.BatteryCharger, OrderDetails.ExtensionCable, OrderDetails.BottomRailIdC AS BottomRail, OrderDetails.WandExtendableC AS BottomExtendable, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.Adjusting, OrderDetails.Notes, 4 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Roller Blind' AND (Blinds.Name IN ('Link 4 Blinds Ind with Dep', 'DB Link 2 Blinds Dep', 'DB Link 2 Blinds Ind', 'DB Link 3 Blinds Dep', 'DB Link 3 Blinds Ind with Dep')) AND OrderDetails.Active = 1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Number, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthC AS Width, OrderDetails.DropC AS Height, Blinds.Name AS BlindName, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdC AS Fabric, OrderDetails.FabricColourIdC AS FabricColour, OrderDetails.RollC, OrderDetails.ControlPositionC, OrderDetails.ChainIdC AS Chain, OrderDetails.ControlLengthC AS ControlLength, OrderDetails.RemoteChannel, OrderDetails.BatteryCharger, OrderDetails.ExtensionCable, OrderDetails.BottomRailIdC AS BottomRail, OrderDetails.WandExtendableC AS BottomExtendable, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.Adjusting, OrderDetails.Notes, 5 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Roller Blind' AND (Blinds.Name IN ('DB Link 3 Blinds Dep', 'DB Link 3 Blinds Ind with Dep')) AND OrderDetails.Active = 1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Number, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthC AS Width, OrderDetails.DropC AS Height, Blinds.Name AS BlindName, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdC AS Fabric, OrderDetails.FabricColourIdC AS FabricColour, OrderDetails.RollC, OrderDetails.ControlPositionC, OrderDetails.ChainIdC AS Chain, OrderDetails.ControlLengthC AS ControlLength, OrderDetails.RemoteChannel, OrderDetails.BatteryCharger, OrderDetails.ExtensionCable, OrderDetails.BottomRailIdC AS BottomRail, OrderDetails.WandExtendableC AS BottomExtendable, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.Adjusting, OrderDetails.Notes, 6 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Roller Blind' AND (Blinds.Name IN ('DB Link 3 Blinds Dep', 'DB Link 3 Blinds Ind with Dep')) AND OrderDetails.Active = 1 ORDER BY OrderDetails.Id, OrderDetails.Number, Item ASC")
                If Not rollerData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Roller Blind"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = rollerData.Tables(0)
                    Dim items(26, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim controlType As String = dt.Rows(i)("ControlType").ToString()

                        Dim fabricId As String = dt.Rows(i)("Fabric").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColour").ToString()
                        Dim bottomId As String = dt.Rows(i)("BottomRail").ToString()

                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim bottomType As String = GetItemData("SELECT Type FROM Bottoms WHERE Id = '" + bottomId + "'")
                        Dim bottomColour As String = GetItemData("SELECT Colour FROM Bottoms WHERE Id = '" + bottomId + "'")

                        Dim chainColour As String = String.Empty
                        Dim remoteType As String = String.Empty

                        Dim chainId As String = dt.Rows(i)("Chain").ToString()
                        Dim chainName As String = GetItemData("SELECT Name FROM Chains WHERE Id = '" + chainId + "'")

                        If controlType = "Chain" Then
                            chainColour = chainName
                            remoteType = ""
                        Else
                            chainColour = ""
                            remoteType = chainName
                        End If

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Height").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("TubeType").ToString()
                        items(7, i) = dt.Rows(i)("ControlType").ToString()
                        items(8, i) = dt.Rows(i)("ColourType").ToString()
                        items(9, i) = fabricName
                        items(10, i) = fabricColour
                        items(11, i) = dt.Rows(i)("Roll").ToString()
                        items(12, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(13, i) = chainColour
                        items(14, i) = dt.Rows(i)("ControlLength").ToString()
                        items(15, i) = remoteType
                        items(16, i) = dt.Rows(i)("RemoteChannel").ToString()
                        items(17, i) = dt.Rows(i)("BatteryCharger").ToString()
                        items(18, i) = dt.Rows(i)("ExtensionCable").ToString()
                        items(19, i) = bottomType
                        items(20, i) = bottomColour
                        items(21, i) = dt.Rows(i)("BottomExtendable").ToString()
                        items(22, i) = dt.Rows(i)("SpringAssist").ToString()
                        items(23, i) = dt.Rows(i)("BracketSize").ToString()
                        items(24, i) = dt.Rows(i)("Adjusting").ToString()
                        items(25, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Roller Type", "System", "Operation", "Bracket Colour", "Fabric Type", "Fabric Colour", "Roll Direction", "Control Position", "Chain Colour", "Chain Length (mm)", "Remote Type", "Remote Channel", "Battery Charger", "Extension Cable", "Bottom Rail", "Bottom Rail Colour", "Extendable Wand", "Assisted Lift", "Bracket Size", "Link Adjusting Spanner", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END ROLLER BLIND

            ' START ROMAN BLIND
            Try
                Dim romanData As DataSet = GetListData("SELECT OrderDetails.*, Products.ControlType AS ControlType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Roman Blind' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not romanData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Roman Blind"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = romanData.Tables(0)
                    Dim items(17, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim bottomId As String = dt.Rows(i)("BottomRailId").ToString()
                        Dim bottomType As String = GetItemData("SELECT Type FROM Bottoms WHERE Id = '" + bottomId + "'")
                        Dim bottomColour As String = GetItemData("SELECT Colour FROM Bottoms WHERE Id = '" + bottomId + "'")

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = fabricName
                        items(7, i) = fabricColour
                        items(8, i) = dt.Rows(i)("ControlType").ToString()
                        items(9, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(10, i) = dt.Rows(i)("ControlColour").ToString()
                        items(11, i) = dt.Rows(i)("ControlLength").ToString()
                        items(12, i) = bottomType
                        items(13, i) = bottomColour
                        items(14, i) = dt.Rows(i)("Batten").ToString()
                        items(15, i) = dt.Rows(i)("BattenB").ToString()
                        items(16, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Roman Style", "Fabric Type", "Fabric Colour", "Control Type", "Control Position", "Control Colour", "Control Length (mm)", "Bottom Type", "Bottom Colour", "Front Batten Colour", "Rear Batten Colour", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END ROMAN BLIND

            ' START SKIN ONLY
            Try
                Dim skinData As DataSet = GetListData("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Skin Only' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not skinData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Skin Only"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = skinData.Tables(0)
                    Dim items(9, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim bottomId As String = dt.Rows(i)("BottomRailId").ToString()
                        Dim bottomType As String = GetItemData("SELECT Type FROM Bottoms WHERE Id = '" + bottomId + "'")
                        Dim bottomColour As String = GetItemData("SELECT Colour FROM Bottoms WHERE Id = '" + bottomId + "'")

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Width").ToString()
                        items(3, i) = dt.Rows(i)("Drop").ToString()
                        items(4, i) = fabricName
                        items(5, i) = fabricColour
                        items(6, i) = bottomType
                        items(7, i) = bottomColour
                        items(8, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Width (mm)", "Drop (mm)", "Fabric Type", "Fabric Colour", "Bottom Type", "Bottom Colour", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END SKIN ONLY

            ' START VERI SHADES
            Try
                Dim verishadesData As DataSet = GetListData("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Veri Shades' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not verishadesData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Veri Shades"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = verishadesData.Tables(0)
                    Dim items(15, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = fabricName
                        items(6, i) = fabricColour
                        items(7, i) = dt.Rows(i)("TrackType").ToString()
                        items(8, i) = dt.Rows(i)("TrackColour").ToString()
                        items(9, i) = dt.Rows(i)("StackOption").ToString()
                        items(10, i) = dt.Rows(i)("WandColour").ToString()
                        items(11, i) = dt.Rows(i)("WandLength").ToString()
                        items(12, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(13, i) = dt.Rows(i)("BracketExtension").ToString()
                        items(14, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Fabric Type", "Fabric Colour", "Track Type", "Track Colour", "Drawing", "Wand Colour", "Wand Size", "Wand Position", "Extension Bracket", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END VERI SHADES

            ' START VERTICAL
            Try
                Dim verticalData As DataSet = GetListData("SELECT OrderDetails.*, Products.ControlType AS ControlType, Products.TubeType AS TubeType, Products.ColourType AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Vertical' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")
                If Not verticalData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Vertical"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = verticalData.Tables(0)
                    Dim items(22, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim number As Integer = i + 1

                        Dim width As String = String.Empty
                        If dt.Rows(i)("Width") > 0 Then width = dt.Rows(i)("Width").ToString()

                        Dim bladeQty As String = String.Empty
                        If dt.Rows(i)("QtyBlade") > 0 Then bladeQty = dt.Rows(i)("QtyBlade").ToString()

                        Dim controllength As String = String.Empty
                        If dt.Rows(i)("ControlLength") > 0 Then
                            controllength = dt.Rows(i)("ControlLength").ToString()
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = width
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("TubeType").ToString()
                        items(7, i) = bladeQty
                        items(8, i) = fabricName
                        items(9, i) = fabricColour
                        items(10, i) = dt.Rows(i)("ColourType").ToString()
                        items(11, i) = dt.Rows(i)("StackOption").ToString()
                        items(12, i) = dt.Rows(i)("ControlType").ToString()
                        items(13, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(14, i) = dt.Rows(i)("ControlColour").ToString()
                        items(15, i) = controllength
                        items(16, i) = dt.Rows(i)("BottomWeight").ToString()
                        items(17, i) = dt.Rows(i)("BottomWeightColour").ToString()
                        items(18, i) = dt.Rows(i)("BracketExtension").ToString()
                        items(19, i) = dt.Rows(i)("FabricInsert").ToString()
                        items(20, i) = dt.Rows(i)("Sloping").ToString()
                        items(21, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Vertical System", "Blade Size", "Blade Qty", "Fabric Type", "Fabric Colour", "Track Colour", "Stack Position", "Control Type", "Control Position", "Chain/Cord Colour", "Control Length (mm)", "Bottom Weight", "Bottom Weight Colour", "Extension Bracket", "Fabric Insert", "Sloping", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END VERTICAL

            ' START ZEBRA BLIND
            Try
                Dim zebraData As DataSet = GetListData("SELECT OrderDetails.*, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId = Designs.Id WHERE OrderDetails.HeaderId = '" + Id + "' AND Designs.Name = 'Zebra Blind' AND OrderDetails.Active = 1 ORDER BY OrderDetails.Number ASC")

                If Not zebraData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Zebra Blind"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = zebraData.Tables(0)
                    Dim items(14, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricName As String = GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" + fabricColourId + "'")

                        Dim bottomId As String = dt.Rows(i)("BottomRailId").ToString()
                        Dim bottomColour As String = GetItemData("SELECT Colour FROM Bottoms WHERE Id = '" + bottomId + "'")

                        Dim number As Integer = i + 1

                        Dim controlLength As String = "Standard"
                        If dt.Rows(i)("ControlLength") > 0 Then
                            controlLength = dt.Rows(i)("ControlLength").ToString()
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = fabricName
                        items(6, i) = fabricColour
                        items(7, i) = dt.Rows(i)("ControlType").ToString()
                        items(8, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(9, i) = dt.Rows(i)("ControlColour").ToString()
                        items(10, i) = controlLength
                        items(11, i) = bottomColour
                        items(12, i) = dt.Rows(i)("SideBySide").ToString()
                        items(13, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Fabric Type", "Fabric Colour", "Control Type", "Control Position", "Control Colour", "Control Length (mm)", "Rail Colour", "Side by Side", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
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
            ' END ZEBRA BLIND
            doc.Close()
            writer.Close()
        End Using
    End Sub
End Class

Public Class PreviewEvents
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

        ' ================= HEADER =================
        Dim headerTable As New PdfPTable(3)
        headerTable.TotalWidth = document.PageSize.Width - 40
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.3F, 0.55F, 0.15F})

        Dim nestedTable As New PdfPTable(3)
        nestedTable.TotalWidth = headerTable.TotalWidth * 0.5F
        nestedTable.SetWidths(New Single() {0.25F, 0.05F, 0.7F})

        Dim phraseFirst As New Phrase()
        phraseFirst.Add(New Chunk("Lifestyle Blinds & Shutters", New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        phraseFirst.Add(New Chunk(Environment.NewLine))
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

        'Dim labels As String() = {"Order No", "Created Date", "Submitted Date", "Retailer", "Retailer Order No", "Customer Name", "Note", "Total Item Order", "Total Page"}
        'Dim values As String() = {PageOrderId, PageOrderDate.ToString("dd MMM yyyy"), PageSubmitDate.ToString("dd MMM yyyy"), PageCustomerName, PageOrderNumber, PageOrderName, If(PageNote.Length > 20, PageNote.Substring(0, 20) & " ....", PageNote), PageTotalItem, PageTotalDoc.ToString()}
        Dim labels As String() = {"Order No", "Created Date", "Submitted Date", "Retailer", "Retailer Order No", "Customer Name", "Note", "Total Item Order"}
        Dim values As String() = {PageOrderId, PageOrderDate.ToString("dd MMM yyyy"), PageSubmitDate.ToString("dd MMM yyyy"), PageCustomerName, PageOrderNumber, PageOrderName, If(PageNote.Length > 20, PageNote.Substring(0, 20) & " ....", PageNote), PageTotalItem}

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

        ' Footer Section (rapi)
        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True
        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("Print Date: " & Now.ToString("dd MMM yyyy"), New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_LEFT
        }
        footerTable.AddCell(leftFooterCell)

        ' Tulis "Page x of " di sebelah kanan
        Dim pageText As String = "Page " & writer.PageNumber.ToString() & " of "
        Dim pageFont As New Font(Font.FontFamily.TIMES_ROMAN, 8)
        Dim rightFooterCell As New PdfPCell(New Phrase(pageText, pageFont)) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_RIGHT
        }
        footerTable.AddCell(rightFooterCell)

        ' Gambar table footer dulu
        Dim footerY As Single = document.PageSize.GetBottom(30)
        footerTable.WriteSelectedRows(0, -1, 36, footerY, cb)

        ' Hitung panjang teks "Page x of" supaya bisa tempatkan template setelahnya
        Dim baseFont As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
        Dim textWidth As Single = baseFont.GetWidthPoint(pageText, 8)
        Dim xPos As Single = document.PageSize.Width - textWidth - 1
        Dim yPos As Single = footerY - 10.0F ' sedikit turun biar sejajar teks

        ' Gambar angka total halaman dari template
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
