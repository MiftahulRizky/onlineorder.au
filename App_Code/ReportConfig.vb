Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports OfficeOpenXml
Imports OfficeOpenXml.Style

Public Class ReportConfig

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
        Dim result As Integer = 0
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

    Public Sub BlindsDaily(Files As String, Status As String, Tanggal As DateTime, Period As String)
        Try
            Dim startDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-01")
            Dim endDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-dd")

            Dim StatusAdditional As String = "AND OrderHeaders.StatusAdditional = 'Waiting For Deposit'"
            If Status = "Production" Then
                StatusAdditional = "AND OrderHeaders.StatusAdditional IS NULL"
            End IF
            Dim thisQuery As String = String.Format("SELECT OrderHeaders.StoreId AS CustomerId, Customers.Name AS CustomerName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.StoreId = Customers.Id LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN HardwareKits ON OrderDetails.KitId = HardwareKits.Id WHERE OrderHeaders.Active = 1 AND CONVERT ( DATE, OrderHeaders.SubmittedDate ) >= '{0}' AND CONVERT ( DATE, OrderHeaders.SubmittedDate ) <= '{1}' {2} GROUP BY OrderHeaders.StoreId, Customers.Name ORDER BY Customers.Name ASC", startDate, endDate, StatusAdditional)
            ' Throw New Exception(thisQuery)
            Dim thisData As DataSet = GetListData(thisQuery)

            Dim Products As DataSet = GetListData("SELECT Id, Name FROM Designs WHERE Type ='Blinds' AND Description LIKE '%Production%' AND Active = 1 ORDER BY Name ASC")

            Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
            Dim pdfFilePath As String = Files

            Using fs As New FileStream(pdfFilePath, FileMode.Create)
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

                Dim pageEvent As New ReportEvents() With {
                    .PageTitle = String.Format("Blinds Daily Report - {0}", Status),
                    .PagePeriod = Period
                }

                writer.PageEvent = pageEvent
                doc.Open()

                Dim data As String() = {}

                Dim productCount As Integer = Products.Tables(0).Rows.Count
                Dim totalColumns As Integer = 1 + (productCount * 2)
                Dim table As New PdfPTable(totalColumns)
                table.WidthPercentage = 100

                Dim widths(totalColumns - 1) As Single
                widths(0) = 0.4F
                For i As Integer = 1 To totalColumns - 1
                    widths(i) = 0.15F
                Next
                table.SetWidths(widths)

                table.AddCell(HeaderCell("CUSTOMER NAME"))

                ' PRODUCT
                For a As Integer = 0 To productCount - 1
                    Dim ProductName As String =Products.Tables(0).Rows(a)("Name").ToString()
                    table.AddCell(HeaderCellProduct(ProductName))
                Next

                ' TODAY / MTD
                For b As Integer = 0 To productCount - 1
                    table.AddCell(HeaderCellTanggal("Today"))
                    table.AddCell(HeaderCellTanggal("MTD"))
                Next


                If thisData.Tables(0).Rows.Count > 0 Then

                    For c As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim customerId As String = thisData.Tables(0).Rows(c)("CustomerId").ToString()
                        Dim customerName As String = thisData.Tables(0).Rows(c)("CustomerName").ToString()

                        table.AddCell(ContentCell(customerName))

                        For d As Integer = 0 To productCount - 1
                            Dim ProductId As String =Products.Tables(0).Rows(d)("Id").ToString()

                            ' TODAY
                            data = {customerId, ProductId, endDate, endDate,""}
                            table.AddCell(ContentCell(GetPerCustomerBlinds2(data)))

                            ' MTD
                            data = {customerId, ProductId, startDate, endDate, ""}
                            table.AddCell(ContentCell(GetPerCustomerBlinds2(data)))

                        Next

                    Next

                    table.AddCell(FooterCell("TOTAL"))
                    For e As Integer = 0 To productCount - 1
                        Dim ProductId As String = Products.Tables(0).Rows(e)("Id").ToString()

                        ' TODAY TOTAL
                        data = { ProductId, endDate, endDate, ""}
                        table.AddCell(ContentCell(GetPerDesignBlinds2(data)))

                        ' MTD TOTAL
                        data = { ProductId, startDate, endDate, ""}
                        table.AddCell(ContentCell(GetPerDesignBlinds2(data)))

                    Next

                Else

                    Dim noDataCell As PdfPCell = ContentCell("No data Found")

                    noDataCell.Colspan = totalColumns
                    noDataCell.HorizontalAlignment = Element.ALIGN_CENTER

                    noDataCell.VerticalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(noDataCell)

                End If


                doc.Add(table)

                doc.Close()

            End Using
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Sub BlindsDailyExcel(Files As String, Status As String, Tanggal As DateTime, Period As String)
        Try
            Dim startDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-01")
            Dim endDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-dd")

            Dim StatusAdditional As String = "AND OrderHeaders.StatusAdditional = 'Waiting For Deposit'"
            If Status = "Production" Then
                StatusAdditional = "AND OrderHeaders.StatusAdditional IS NULL"
            End IF
            Dim thisQuery As String = String.Format("SELECT OrderHeaders.StoreId AS CustomerId, Customers.Name AS CustomerName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.StoreId = Customers.Id LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN HardwareKits ON OrderDetails.KitId = HardwareKits.Id WHERE OrderHeaders.Active = 1 AND CONVERT ( DATE, OrderHeaders.SubmittedDate ) >= '{0}' AND CONVERT ( DATE, OrderHeaders.SubmittedDate ) <= '{1}' {2} GROUP BY OrderHeaders.StoreId, Customers.Name ORDER BY Customers.Name ASC", startDate, endDate, StatusAdditional)

            Dim thisData As DataSet = GetListData(thisQuery)

            Dim Products As DataSet = GetListData("SELECT Id, Name FROM Designs WHERE Type ='Blinds' AND Description LIKE '%Production%' AND Active = 1 ORDER BY Name ASC")


            If Products Is Nothing OrElse Products.Tables.Count = 0 OrElse Products.Tables(0).Rows.Count = 0 Then
                Return
            End If


            Using package As New OfficeOpenXml.ExcelPackage()

                Dim worksheet As OfficeOpenXml.ExcelWorksheet = package.Workbook.Worksheets.Add("Blinds Daily")
                Dim productCount As Integer = Products.Tables(0).Rows.Count

                Dim totalColumns As Integer = 1 + (productCount * 2)

                worksheet.Cells(1, 1).Value = "CUSTOMER NAME"

                ' Merge customer 
                worksheet.Cells(1, 1, 2, 1).Merge = True


                Dim currentColumn As Integer = 2
                For a As Integer = 0 To productCount - 1
                    Dim ProductName As String = Products.Tables(0).Rows(a)("Name").ToString()

                    ' Each product occupies 2 columns:
                    ' Today + MTD
                    worksheet.Cells(1, currentColumn).Value = ProductName
                    worksheet.Cells(1, currentColumn, 1, currentColumn + 1).Merge = True

                    currentColumn += 2
                Next

                currentColumn = 2
                For b As Integer = 0 To productCount - 1
                    worksheet.Cells(2, currentColumn).Value = "Today"
                    worksheet.Cells(2, currentColumn + 1).Value = "MTD"
                    currentColumn += 2
                Next


                Using headerRange As OfficeOpenXml.ExcelRange = worksheet.Cells(1, 1, 2, totalColumns)

                    headerRange.Style.Font.Bold = True
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                    headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center
                    headerRange.Style.WrapText = True
                    headerRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin

                End Using


                Dim rowIndex As Integer = 3
                If thisData IsNot Nothing AndAlso thisData.Tables.Count > 0 AndAlso thisData.Tables(0).Rows.Count > 0 Then
                    For c As Integer = 0 To thisData.Tables(0).Rows.Count - 1

                        Dim customerId As String = thisData.Tables(0).Rows(c)("CustomerId").ToString()
                        Dim customerName As String = thisData.Tables(0).Rows(c)("CustomerName").ToString()

                        ' Customer name
                        worksheet.Cells(rowIndex, 1).Value = customerName

                        currentColumn = 2
                        For d As Integer = 0 To productCount - 1

                            Dim ProductId As String = Products.Tables(0).Rows(d)("Id").ToString()

                            Dim data As String() = {customerId, ProductId, endDate, endDate, ""}
                            Dim todayValue As String = GetPerCustomerBlinds2(data)
                            worksheet.Cells(rowIndex, currentColumn).Value = todayValue

                            data = {customerId, ProductId, startDate, endDate, ""}
                            Dim mtdValue As String =GetPerCustomerBlinds2(data)
                            worksheet.Cells(rowIndex,currentColumn + 1).Value = mtdValue
                            currentColumn += 2

                        Next

                        rowIndex += 1

                    Next

                    worksheet.Cells(rowIndex, 1).Value = "TOTAL"
                    currentColumn = 2
    
                    For e As Integer = 0 To productCount - 1
                        Dim ProductId As String = Products.Tables(0).Rows(e)("Id").ToString()
    
                        Dim data As String() = {ProductId, endDate, endDate, ""}
                        Dim todayTotal As String = GetPerDesignBlinds2(data)
    
                        worksheet.Cells(rowIndex,currentColumn).Value = todayTotal
    
                        data = {ProductId, startDate, endDate,""}
                        Dim mtdTotal As String = GetPerDesignBlinds2(data)
    
                        worksheet.Cells(rowIndex, currentColumn + 1).Value = mtdTotal
    
                        currentColumn += 2
    
                    Next
    
    
                    Using totalRange As OfficeOpenXml.ExcelRange = worksheet.Cells(rowIndex, 1, rowIndex, totalColumns)
    
                        totalRange.Style.Font.Bold = True
                        totalRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                        totalRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                        totalRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                        totalRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
    
                    End Using

                Else
                    worksheet.Cells(rowIndex, 1).Value = "No data Found"
                    worksheet.Cells(rowIndex, 1, rowIndex, totalColumns).Merge = True
                    worksheet.Cells(rowIndex, 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                    worksheet.Cells(rowIndex, 1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center
                    worksheet.Cells(rowIndex, 1).Style.Font.Bold = True
                    worksheet.Cells(rowIndex, 1).Style.WrapText = True
                End If

    
                Using dataRange As OfficeOpenXml.ExcelRange = worksheet.Cells(1, 1, rowIndex, totalColumns)

                    dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center

                End Using

                worksheet.Column(1).Width = 30

                For i As Integer = 2 To totalColumns
                    worksheet.Column(i).Width = 12
                Next

                worksheet.Cells(1, 1, rowIndex, totalColumns).Style.WrapText = True

                worksheet.View.FreezePanes(3, 2)



                If rowIndex > 3 Then
                    worksheet.Cells(2, 1, rowIndex, totalColumns).AutoFilter = True
                End If


                Dim excelFilePath As String = Files
                Dim fileInfo As New System.IO.FileInfo(excelFilePath)
                package.SaveAs(fileInfo)

            End Using

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Sub BlindsOrders(Files As String, Status As String, StartDate As DateTime, EndDate As DateTime)
        Try
            Dim startDateStr As String = StartDate.ToString("yyyy-MM-dd")
            Dim endDateStr As String = EndDate.ToString("yyyy-MM-dd")

            Dim StatusAdditional As String = "AND StatusAdditional = 'Waiting For Deposit'"
            If Status = "Production" Then
                StatusAdditional = "AND StatusAdditional IS NULL"
            End IF
            Dim thisQuery As String = String.Format("SELECT Id, HeaderId, Qty, Width, [Drop], SubmittedDate, JobDate, CustomerName, OrderNumber, OrderName, DesignName, KitName, FabricName, FabricType, FabricColour, Notes FROM view_details WHERE Active = 1 AND HeaderActive = 1 AND CONVERT ( DATE, SubmittedDate ) >= '{0}' AND CONVERT ( DATE, SubmittedDate ) <= '{1}' AND Status IN ( 'New Order', 'In Production', 'Completed' ) {2} ORDER BY HeaderId ASC", startDateStr, endDateStr, StatusAdditional)

            Dim thisData As DataSet = GetListData(thisQuery)

           
            Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
            Dim pdfFilePath As String = Files
            Using fs As New FileStream(pdfFilePath, FileMode.Create)
                Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                Dim pageEvent As New ReportEvents() With {
                    .PageTitle = String.Format("Blinds Orders Report - {0}", Status),
                    .PagePeriod = period
                }
                writer.PageEvent = pageEvent
                doc.Open()

                Dim data As String() = {}

                Dim table As New PdfPTable(9)
                table.WidthPercentage = 100
                table.SetWidths(New Single() {0.05F, 0.1F, 0.1F, 0.2F, 0.15F, 0.1F, 0.1F, 0.1F, 0.1F})

                ' table.AddCell(HeaderCellOriginal("No"))
                table.AddCell(HeaderCellOriginal("ID"))
                table.AddCell(HeaderCellOriginal("Received"))
                table.AddCell(HeaderCellOriginal("Ship Date"))
                table.AddCell(HeaderCellOriginal("Store Name"))
                table.AddCell(HeaderCellOriginal("Store Order No"))
                table.AddCell(HeaderCellOriginal("Store Customer"))
                table.AddCell(HeaderCellOriginal("Description"))
                table.AddCell(HeaderCellOriginal("Fabric"))
                table.AddCell(HeaderCellOriginal("Reason for hold"))

                If thisData IsNot Nothing AndAlso thisData.Tables.Count > 0 AndAlso thisData.Tables(0).Rows.Count > 0 Then

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim Id As String = thisData.Tables(0).Rows(i).Item("Id").ToString()
                        Dim HeaderId As String = thisData.Tables(0).Rows(i).Item("HeaderId").ToString()
                        Dim Qty As String = thisData.Tables(0).Rows(i).Item("Qty").ToString()
                        Dim SubmittedDate As String = thisData.Tables(0).Rows(i).Item("SubmittedDate").ToString()
                        Dim JobDate As String = thisData.Tables(0).Rows(i).Item("JobDate").ToString()
                        Dim CustomerName As String = thisData.Tables(0).Rows(i).Item("CustomerName").ToString()
                        Dim OrderNumber As String = thisData.Tables(0).Rows(i).Item("OrderNumber").ToString()
                        Dim OrderName As String = thisData.Tables(0).Rows(i).Item("OrderName").ToString()
                        Dim Width As String = thisData.Tables(0).Rows(i).Item("Width").ToString()
                        Dim Drop As String = thisData.Tables(0).Rows(i).Item("Drop").ToString()
                        Dim FabricType As String = thisData.Tables(0).Rows(i).Item("FabricType").ToString()
                        Dim FabricColour As String = thisData.Tables(0).Rows(i).Item("FabricColour").ToString()
                        Dim DesignName As String = thisData.Tables(0).Rows(i).Item("DesignName").ToString()
                        Dim KitName As String = thisData.Tables(0).Rows(i).Item("KitName").ToString()
                        Dim FabricName As String = thisData.Tables(0).Rows(i).Item("FabricName").ToString()
                        Dim Notes As String = thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        ' table.AddCell(ContentCell(i + 1))
                        table.AddCell(ContentCell(HeaderId))
                        table.AddCell(ContentCell(SubmittedDate))
                        table.AddCell(ContentCell(JobDate))
                        table.AddCell(ContentCell(CustomerName))
                        table.AddCell(ContentCell(OrderNumber))
                        table.AddCell(ContentCell(OrderName))
                        table.AddCell(ContentCell(FindDescription(Id)))
                        table.AddCell(ContentCell(FabricName))
                        table.AddCell(ContentCell(Notes))
                    Next

                    ' Dim totalItemCount As Integer = GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")
                    ' Dim totalPanelQty As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")
                    ' Dim totalSquareMetre As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.SquareMetre) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")

                    ' table.AddCell(FooterCellCol4("TOTAL"))
                    ' table.AddCell(FooterCell(totalItemCount))
                    ' table.AddCell(FooterCell(totalPanelQty))
                    ' table.AddCell(FooterCell(totalSquareMetre))
                Else
                    Dim noDataCell As PdfPCell =ContentCell("No data Found")
                    noDataCell.Colspan = 9
                    noDataCell.HorizontalAlignment = Element.ALIGN_CENTER
                    noDataCell.VerticalAlignment = Element.ALIGN_MIDDLE

                    table.AddCell(noDataCell)
                End If

                doc.Add(table)

                doc.Close()
            End Using
           
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Sub BlindsOrdersExcel(Files As String, Status As String, StartDate As DateTime, EndDate As DateTime)
        Try
            Dim startDateStr As String = StartDate.ToString("yyyy-MM-dd")
            Dim endDateStr As String = EndDate.ToString("yyyy-MM-dd")

            Dim StatusAdditional As String = "AND StatusAdditional = 'Waiting For Deposit'"

            If Status = "Production" Then
                StatusAdditional = "AND StatusAdditional IS NULL"
            End If
            Dim thisQuery As String = String.Format("SELECT Id, HeaderId, Qty, Width, [Drop], SubmittedDate, JobDate, CustomerName, OrderNumber, OrderName, DesignName, KitName, FabricName, FabricType, FabricColour, Notes FROM view_details WHERE Active = 1 AND HeaderActive = 1 AND CONVERT ( DATE, SubmittedDate ) >= '{0}' AND CONVERT ( DATE, SubmittedDate ) <= '{1}' AND Status IN ( 'New Order', 'In Production', 'Completed' ) {2} ORDER BY HeaderId ASC", startDateStr, endDateStr, StatusAdditional)
            Dim thisData As DataSet =GetListData(thisQuery)


            Using package As New OfficeOpenXml.ExcelPackage()
                Dim worksheet As OfficeOpenXml.ExcelWorksheet = package.Workbook.Worksheets.Add("Blinds Orders")

                worksheet.Cells(1, 1).Value = "ID"
                worksheet.Cells(1, 2).Value = "Received"
                worksheet.Cells(1, 3).Value = "Ship Date"
                worksheet.Cells(1, 4).Value = "Store Name"
                worksheet.Cells(1, 5).Value = "Store Order No"
                worksheet.Cells(1, 6).Value = "Store Customer"
                worksheet.Cells(1, 7).Value = "Description"
                worksheet.Cells(1, 8).Value = "Fabric"
                worksheet.Cells(1, 9).Value = "Reason for hold"


                Using headerRange As OfficeOpenXml.ExcelRange = worksheet.Cells(1, 1, 1, 9)

                    headerRange.Style.Font.Bold = True
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                    headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center
                    headerRange.Style.WrapText = True
                    headerRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin

                End Using


                Dim rowIndex As Integer = 2
                If thisData IsNot Nothing AndAlso thisData.Tables.Count > 0 AndAlso thisData.Tables(0).Rows.Count > 0 Then

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim Id As String =thisData.Tables(0).Rows(i).Item("Id").ToString()
                        Dim HeaderId As String = thisData.Tables(0).Rows(i).Item("HeaderId").ToString()
                        Dim SubmittedDate As String = thisData.Tables(0).Rows(i).Item("SubmittedDate").ToString()
                        Dim JobDate As String = thisData.Tables(0).Rows(i).Item("JobDate").ToString()
                        Dim CustomerName As String = thisData.Tables(0).Rows(i).Item("CustomerName").ToString()
                        Dim OrderNumber As String = thisData.Tables(0).Rows(i).Item("OrderNumber").ToString()
                        Dim OrderName As String = thisData.Tables(0).Rows(i).Item("OrderName").ToString()
                        Dim FabricName As String = thisData.Tables(0).Rows(i).Item("FabricName").ToString()
                        Dim Notes As String = thisData.Tables(0).Rows(i).Item("Notes").ToString()


                        worksheet.Cells(rowIndex, 1 ).Value = HeaderId
                        worksheet.Cells(rowIndex, 2).Value = SubmittedDate
                        worksheet.Cells(rowIndex, 3).Value = JobDate
                        worksheet.Cells(rowIndex, 4).Value = CustomerName
                        worksheet.Cells(rowIndex, 5).Value = OrderNumber
                        worksheet.Cells(rowIndex, 6).Value = OrderName
                        worksheet.Cells(rowIndex, 7).Value = FindDescription(Id)
                        worksheet.Cells(rowIndex, 8).Value = FabricName
                        worksheet.Cells(rowIndex, 9).Value = Notes

                        rowIndex += 1

                    Next

                Else

                    worksheet.Cells(rowIndex, 1).Value = "No data Found"
                    worksheet.Cells(rowIndex, 1, rowIndex, 9).Merge = True
                    worksheet.Cells(rowIndex, 1).Style.Font.Bold = True
                    worksheet.Cells(rowIndex, 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                    worksheet.Cells(rowIndex, 1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center
                    worksheet.Cells(rowIndex, 1).Style.WrapText = True

                End If


                Using dataRange As OfficeOpenXml.ExcelRange = worksheet.Cells(1, 1, rowIndex, 9)

                    dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    dataRange.Style.VerticalAlignment =OfficeOpenXml.Style.ExcelVerticalAlignment.Top

                End Using


                worksheet.Column(1).Width = 10
                worksheet.Column(2).Width = 18
                worksheet.Column(3).Width = 18
                worksheet.Column(4).Width = 50
                worksheet.Column(5).Width = 20
                worksheet.Column(6).Width = 25
                worksheet.Column(7).Width = 100
                worksheet.Column(8).Width = 25
                worksheet.Column(9).Width = 35


                worksheet.Cells(1, 1, rowIndex, 9).Style.WrapText = True
                worksheet.View.FreezePanes(2, 1)

                If thisData IsNot Nothing AndAlso thisData.Tables.Count > 0 AndAlso thisData.Tables(0).Rows.Count > 0 Then
                    worksheet.Cells( 1, 1, rowIndex - 1, 9).AutoFilter = True
                End If

                Dim excelFilePath As String = Files

                Dim fileInfo As New System.IO.FileInfo(excelFilePath)
                package.SaveAs(fileInfo)


            End Using


        Catch ex As Exception

            Throw

        End Try

    End Sub

    Public Sub BlindsOrdersExcels(Files As String, Status As String, StartDate As DateTime, EndDate As DateTime)
        Try
            Dim startDateStr As String = StartDate.ToString("yyyy-MM-dd")
            Dim endDateStr As String = EndDate.ToString("yyyy-MM-dd")

            Dim StatusAdditional As String = "AND StatusAdditional = 'Waiting For Deposit'"
            If Status = "Production" Then
                StatusAdditional = "AND StatusAdditional IS NULL"
            End IF
            Dim thisQuery As String = String.Format("SELECT Id, HeaderId, Qty, Width, [Drop], SubmittedDate, JobDate, CustomerName, OrderNumber, OrderName, DesignName, KitName, FabricName, FabricType, FabricColour, Notes FROM view_details WHERE Active = 1 AND HeaderActive = 1 AND CONVERT ( DATE, SubmittedDate ) >= '{0}' AND CONVERT ( DATE, SubmittedDate ) <= '{1}' AND Status IN ( 'New Order', 'In Production', 'Completed' ) {2} ORDER BY HeaderId ASC", startDateStr, endDateStr, StatusAdditional)

            Dim thisData As DataSet = GetListData(thisQuery)

            ' If thisData Is Nothing OrElse thisData.Tables.Count = 0 OrElse thisData.Tables(0).Rows.Count = 0 Then
            '     Return
            ' End If

            Using package As New OfficeOpenXml.ExcelPackage()
                Dim worksheet As OfficeOpenXml.ExcelWorksheet = package.Workbook.Worksheets.Add("Blinds Orders")

                worksheet.Cells(1, 1).Value = "ID"
                worksheet.Cells(1, 2).Value = "Received"
                worksheet.Cells(1, 3).Value = "Ship Date"
                worksheet.Cells(1, 4).Value = "Store Name"
                worksheet.Cells(1, 5).Value = "Store Order No"
                worksheet.Cells(1, 6).Value = "Store Customer"
                worksheet.Cells(1, 7).Value = "Description"
                worksheet.Cells(1, 8).Value = "Fabric"
                worksheet.Cells(1, 9).Value = "Reason for hold"


                Using headerRange As OfficeOpenXml.ExcelRange = worksheet.Cells(1, 1, 1, 9)

                    headerRange.Style.Font.Bold = True
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                    headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center
                    headerRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                    headerRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin

                End Using

                Dim rowIndex As Integer = 2
                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1

                    Dim Id As String = thisData.Tables(0).Rows(i).Item("Id").ToString()
                    Dim HeaderId As String = thisData.Tables(0).Rows(i).Item("HeaderId").ToString()
                    Dim SubmittedDate As String = thisData.Tables(0).Rows(i).Item("SubmittedDate").ToString()
                    Dim JobDate As String = thisData.Tables(0).Rows(i).Item("JobDate").ToString()
                    Dim CustomerName As String = thisData.Tables(0).Rows(i).Item("CustomerName").ToString()
                    Dim OrderNumber As String = thisData.Tables(0).Rows(i).Item("OrderNumber").ToString()
                    Dim OrderName As String = thisData.Tables(0).Rows(i).Item("OrderName").ToString()
                    Dim FabricName As String = thisData.Tables(0).Rows(i).Item("FabricName").ToString()
                    Dim Notes As String = thisData.Tables(0).Rows(i).Item("Notes").ToString()

                    worksheet.Cells(rowIndex, 1).Value = HeaderId
                    worksheet.Cells(rowIndex, 2).Value = SubmittedDate
                    worksheet.Cells(rowIndex, 3).Value = JobDate
                    worksheet.Cells(rowIndex, 4).Value = CustomerName
                    worksheet.Cells(rowIndex, 5).Value = OrderNumber
                    worksheet.Cells(rowIndex, 6).Value = OrderName
                    worksheet.Cells(rowIndex, 7).Value = FindDescription(Id)
                    worksheet.Cells(rowIndex, 8).Value = FabricName
                    worksheet.Cells(rowIndex, 9).Value = Notes

                    rowIndex += 1

                Next

                If rowIndex > 2 Then

                    Using dataRange As OfficeOpenXml.ExcelRange = worksheet.Cells(1, 1, rowIndex - 1, 9)

                        dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                        dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                        dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                        dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin
                        dataRange.Style.VerticalAlignment =OfficeOpenXml.Style.ExcelVerticalAlignment.Top

                    End Using

                End If

                worksheet.Column(1).Width = 10
                worksheet.Column(2).Width = 18
                worksheet.Column(3).Width = 18
                worksheet.Column(4).Width = 25
                worksheet.Column(5).Width = 20
                worksheet.Column(6).Width = 25
                worksheet.Column(7).Width = 50
                worksheet.Column(8).Width = 25
                worksheet.Column(9).Width = 35

                worksheet.Cells(1, 1, rowIndex - 1, 9).Style.WrapText = True

                worksheet.View.FreezePanes(2, 1)


                If rowIndex > 2 Then
                    worksheet.Cells(1, 1, rowIndex - 1, 9).AutoFilter = True
                End If


                Dim excelFilePath As String = Files

                Dim fileInfo As New System.IO.FileInfo(excelFilePath)

                package.SaveAs(fileInfo)

            End Using

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Function FindDescription(id As String) As string
        Try
            Dim result As String = ""
            Dim Data As Dataset = GetListData(String.Format("SELECT Width, [Drop], SlatSize, DesignId, KitName, BracketType, TubeType, DesignName, BlindName, FabricName, FabricType, FabricColour, CustomerId FROM view_details WHERE Id ='{0}' AND Active=1", id))
            Dim DataCount As Integer = Data.Tables(0).Rows.Count
            Dim DataRow As DataRow = Data.Tables(0).Rows(0)
            IF DataCount > 0 Then

                Dim Width As String = DataRow.Item("Width").ToString()
                Dim Drop As String = DataRow.Item("Drop").ToString()
                Dim SlatSize As String = DataRow.Item("SlatSize").ToString()
                Dim FabricName As String = DataRow.Item("FabricName").ToString()
                Dim FabricType As String = DataRow.Item("FabricType").ToString()
                Dim FabricColour As String = DataRow.Item("FabricColour").ToString()
                Dim DesignId As String = DataRow.Item("DesignId").ToString()
                Dim KitName As String = DataRow.Item("KitName").ToString()
                Dim BracketType As String = DataRow.Item("BracketType").ToString()
                Dim TubeType As String = DataRow.Item("TubeType").ToString()
                Dim DesignName As String = DataRow.Item("DesignName").ToString()
                Dim BlindName As String = DataRow.Item("BlindName").ToString()
                Dim CustomerId As String = DataRow.Item("CustomerId").ToString()

                Dim CustDiscValue As Double = 0
                Dim CustDisc As String = GetItemData(String.Format("SELECT Discount FROM CustomerDiscounts WHERE CustomerData='{0}' AND DesignId='{1}'", CustomerId, DesignId))

                If Not String.IsNullOrWhiteSpace(CustDisc) Then
                    Double.TryParse(CustDisc, CustDiscValue)
                End If

                Dim Disc As String = String.Format("(Inc {0}% Disc. Plus Additional 0% Disc.)", CustDiscValue.ToString("0.##"))
                Dim Size As String = String.Format("{0}x{1}", Width, Drop)

                result = String.Format("{0} {1} {2}", KitName, Size, Disc)

                If DesignName = "Roller Blinds" Then
                    result = String.Format("{0} - {1} #{2} {3} {4}", BlindName, KitName, FabricName, Size, Disc)
                End If

                If DesignName = "Venetian Blinds" Or DesignName = "Veri Shades" Then
                    result = String.Format("{0} {1} {2}", KitName, Size, Disc)
                End If

                If DesignName = "Vertical Blinds" Then
                    SlatSize = SlatSize.Replace("mm", "")

                    result = String.Format("{0}mm {1} #{2} {3} {4}", SlatSize, KitName, FabricName, Size, Disc)
                    If BlindName = "Slat Only" Then
                        result = String.Format("{0}mm {1} #{2} (Drop : {3}mm) {4}", SlatSize, KitName, FabricName, Drop, Disc)
                    End If
                    If BlindName = "Track Only" Then
                        result = String.Format("{0}mm {1} (Width : {2}mm) {3}", SlatSize, KitName, Width, Disc)
                    End If

                End If
            End If

            Return result
        Catch ex As Exception
            Return "Oops... something went wrong"
        End Try
    End Function

    Public Sub ShuttersDaily(Files As String, Tanggal As DateTime, Period As String)
        Try
            Dim startDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-01")
            Dim endDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-dd")

            Dim thisQuery As String = "SELECT OrderHeaders_Shutters.CustomerId AS CustomerId, Customers.Name AS CustomerName FROM OrderHeaders_Shutters INNER JOIN Customers ON OrderHeaders_Shutters.CustomerId = Customers.Id LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.Active = 1 AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE' OR Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12') GROUP BY OrderHeaders_Shutters.CustomerId, Customers.Name ORDER BY Customers.Name ASC"
            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Shutters Daily Report",
                        .PagePeriod = Period
                    }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim data As String() = {}

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.4F, 0.15F, 0.15F, 0.15F, 0.15F})

                    table.AddCell(HeaderCell("CUSTOMER NAME"))
                    table.AddCell(HeaderCellProduct("PANORAMA PVC SHUTTERS"))
                    table.AddCell(HeaderCellProduct("EVOLVE SHUTTERS"))

                    table.AddCell(HeaderCellTanggal("Today"))
                    table.AddCell(HeaderCellTanggal("MTD"))
                    table.AddCell(HeaderCellTanggal("Today"))
                    table.AddCell(HeaderCellTanggal("MTD"))

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim customerId As String = thisData.Tables(0).Rows(i)("CustomerId").ToString()
                        Dim customerName As String = thisData.Tables(0).Rows(i)("CustomerName").ToString()

                        table.AddCell(ContentCell(customerName))

                        ' PANORAMA
                        data = {customerId, "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", endDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", startDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' EVOLVE
                        data = {customerId, "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", endDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", startDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                    Next

                    table.AddCell(FooterCell("TOTAL"))
                    ' PANORAMA
                    data = {"0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", endDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))
                    data = {"0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", startDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))

                    ' EVOLVE
                    data = {"F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", endDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))
                    data = {"F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", startDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))

                    doc.Add(table)
                    doc.Close()
                End Using
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub ShuttersMTM(Files As String, StartDate As DateTime, EndDate As DateTime)
        Try
            ' Jalankan untuk DesignId pertama
            Dim query1 As String = "EXEC report_MTM @DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8', @StartMonth = '" & StartDate.ToString("yyyy-MM-dd") & "', @EndMonth = '" & EndDate.ToString("yyyy-MM-dd") & "';"
            Dim ds1 As DataSet = GetListData(query1)
            Dim dt1 As DataTable = ds1.Tables(0)

            ' Jalankan untuk DesignId kedua
            Dim query2 As String = "EXEC report_MTM @DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7', @StartMonth = '" & StartDate.ToString("yyyy-MM-dd") & "', @EndMonth = '" & EndDate.ToString("yyyy-MM-dd") & "';"
            Dim ds2 As DataSet = GetListData(query2)
            Dim dt2 As DataTable = ds2.Tables(0)

            ' Gabungkan dt1 dan dt2 ke satu DataTable
            Dim dt As DataTable = dt1.Clone()

            ' Salin semua dari dt1
            For Each row As DataRow In dt1.Rows
                dt.ImportRow(row)
            Next

            ' Gabungkan dari dt2 (jumlahkan jika CustomerName sama)
            For Each row As DataRow In dt2.Rows
                Dim custName As String = row("CustomerName").ToString()
                Dim existingRow As DataRow = dt.Select("CustomerName = '" & custName.Replace("'", "''") & "'").FirstOrDefault()

                If existingRow IsNot Nothing Then
                    For Each col As DataColumn In dt.Columns
                        If col.ColumnName <> "CustomerName" Then
                            Dim val1 As Decimal = 0
                            Dim val2 As Decimal = 0
                            Decimal.TryParse(existingRow(col.ColumnName).ToString(), val1)
                            Decimal.TryParse(row(col.ColumnName).ToString(), val2)
                            existingRow(col.ColumnName) = val1 + val2
                        End If
                    Next
                Else
                    dt.ImportRow(row)
                End If
            Next

            ' Jika data ada, generate PDF
            If dt.Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Shutters Month to Month",
                        .PagePeriod = period
                    }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim columnCount As Integer = dt.Columns.Count
                    Dim table As New PdfPTable(columnCount)
                    table.WidthPercentage = 100

                    table.AddCell(HeaderCellOriginal("Retailer Name"))
                    For Each col As DataColumn In dt.Columns
                        If col.ColumnName <> "CustomerName" AndAlso col.ColumnName <> "Total" Then
                            table.AddCell(HeaderCellOriginal(col.ColumnName))
                        End If
                    Next
                    table.AddCell(HeaderCellOriginal("Total"))

                    ' Data rows
                    For Each row As DataRow In dt.Rows
                        For Each col As DataColumn In dt.Columns
                            table.AddCell(ContentCell(row(col.ColumnName).ToString()))
                        Next
                    Next

                    ' Hitung total per kolom
                    Dim footerRow(columnCount - 1) As Decimal
                    For Each row As DataRow In dt.Rows
                        For i As Integer = 0 To dt.Columns.Count - 1
                            Dim colName As String = dt.Columns(i).ColumnName
                            If colName <> "CustomerName" Then
                                Dim value As Decimal
                                If Decimal.TryParse(row(colName).ToString(), value) Then
                                    footerRow(i) += value
                                End If
                            End If
                        Next
                    Next

                    ' Footer row (total keseluruhan)
                    For i As Integer = 0 To dt.Columns.Count - 1
                        Dim colName As String = dt.Columns(i).ColumnName
                        If colName = "CustomerName" Then
                            table.AddCell(FooterCell("Total"))
                        Else
                            table.AddCell(FooterCell(footerRow(i).ToString("N0")))
                        End If
                    Next

                    doc.Add(table)
                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try

    End Sub

    'Public Sub ShuttersMTM(Files As String, StartDate As DateTime, EndDate As DateTime)
    '    Try
    '        Dim thisQuery As String = "EXEC report_MTM @DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8', @StartMonth = '" + StartDate.ToString("yyyy-MM-dd") + "', @EndMonth = '" + EndDate.ToString("yyyy-MM-dd") + "';"
    '        Dim thisData As DataSet = GetListData(thisQuery)

    '        If thisData.Tables(0).Rows.Count > 0 Then
    '            Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
    '            Dim pdfFilePath As String = Files
    '            Using fs As New FileStream(pdfFilePath, FileMode.Create)
    '                Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

    '                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
    '                Dim pageEvent As New ReportEvents() With {
    '                    .PageTitle = "Shutters Month to Month",
    '                    .PagePeriod = period
    '                }
    '                writer.PageEvent = pageEvent
    '                doc.Open()

    '                Dim dt As DataTable = thisData.Tables(0)
    '                Dim columnCount As Integer = thisData.Tables(0).Columns.Count

    '                Dim table As New PdfPTable(columnCount)
    '                table.WidthPercentage = 100

    '                table.AddCell(HeaderCellOriginal("Retailer Name"))
    '                For Each col As DataColumn In dt.Columns
    '                    If col.ColumnName <> "CustomerName" AndAlso col.ColumnName <> "Total" Then
    '                        table.AddCell(HeaderCellOriginal(col.ColumnName))
    '                    End If
    '                Next
    '                table.AddCell(HeaderCellOriginal("Total"))

    '                For Each row As DataRow In dt.Rows
    '                    For Each col As DataColumn In dt.Columns
    '                        table.AddCell(ContentCell(row(col.ColumnName).ToString()))
    '                    Next
    '                Next

    '                Dim footerRow(columnCount - 1) As Decimal
    '                For Each row As DataRow In dt.Rows
    '                    For i As Integer = 0 To dt.Columns.Count - 1
    '                        Dim colName As String = dt.Columns(i).ColumnName
    '                        If colName <> "CustomerName" Then
    '                            Dim value As Decimal
    '                            If Decimal.TryParse(row(colName).ToString(), value) Then
    '                                footerRow(i) += value
    '                            End If
    '                        End If
    '                    Next
    '                Next

    '                For i As Integer = 0 To dt.Columns.Count - 1
    '                    Dim colName As String = dt.Columns(i).ColumnName
    '                    If colName = "CustomerName" Then
    '                        table.AddCell(FooterCell("Total"))
    '                    Else
    '                        table.AddCell(FooterCell(footerRow(i).ToString("N0")))
    '                    End If
    '                Next

    '                doc.Add(table)

    '                doc.Close()
    '            End Using
    '        End If
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Public Sub ShuttersOrders(Files As String, StartDate As DateTime, EndDate As DateTime)
        Try
            Dim startDateStr As String = StartDate.ToString("yyyy-MM-dd")
            Dim endDateStr As String = EndDate.ToString("yyyy-MM-dd")

            Dim thisQuery As String = "SELECT DISTINCT OrderHeaders_Shutters.*, Customers.Name AS CustomerName FROM OrderHeaders_Shutters INNER JOIN Customers ON OrderHeaders_Shutters.CustomerId = Customers.Id INNER JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId INNER JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.Active = 1 AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND (OrderHeaders_Shutters.Status = 'In Production' OR OrderHeaders_Shutters.Status = 'Completed') ORDER BY Customers.Name ASC"

            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Shutters Orders Report",
                        .PagePeriod = period
                    }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim data As String() = {}

                    Dim table As New PdfPTable(9)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.05F, 0.1F, 0.1F, 0.2F, 0.15F, 0.1F, 0.1F, 0.1F, 0.1F})

                    table.AddCell(HeaderCellOriginal("No"))
                    table.AddCell(HeaderCellOriginal("Job Number"))
                    table.AddCell(HeaderCellOriginal("Job Date"))
                    table.AddCell(HeaderCellOriginal("Retailer Name"))
                    table.AddCell(HeaderCellOriginal("Retailer Order Number"))
                    table.AddCell(HeaderCellOriginal("Customer Name"))
                    table.AddCell(HeaderCellOriginal("Item Count"))
                    table.AddCell(HeaderCellOriginal("Panel Qty"))
                    table.AddCell(HeaderCellOriginal("M2"))

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim headerId As String = thisData.Tables(0).Rows(i).Item("Id").ToString()

                        Dim itemCount As Integer = GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.HeaderId = '" + headerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails_Shutters.Active=1")

                        Dim panelQty As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.HeaderId = '" + headerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails_Shutters.Active=1")

                        Dim squareMetre As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.SquareMetre) FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.HeaderId = '" + headerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails_Shutters.Active=1")

                        Dim jobDate As String = String.Empty
                        If Not thisData.Tables(0).Rows(0).Item("JobDate").ToString() = "" Then
                            jobDate = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("jobDate")).ToString("dd MMM yyyy")
                        End If

                        table.AddCell(ContentCell(i + 1))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("JobId").ToString()))
                        table.AddCell(ContentCell(jobDate))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("CustomerName").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("OrderNumber").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("OrderName").ToString()))
                        table.AddCell(ContentCell(itemCount.ToString()))
                        table.AddCell(ContentCell(panelQty.ToString()))
                        table.AddCell(ContentCell(squareMetre.ToString()))
                    Next

                    Dim totalItemCount As Integer = GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")
                    Dim totalPanelQty As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")
                    Dim totalSquareMetre As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.SquareMetre) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")

                    table.AddCell(FooterCellCol4("TOTAL"))
                    table.AddCell(FooterCell(totalItemCount))
                    table.AddCell(FooterCell(totalPanelQty))
                    table.AddCell(FooterCell(totalSquareMetre))

                    doc.Add(table)

                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ShutterEvolveOrders(Files As String, StartDate As DateTime, EndDate As DateTime)
        Try
            Dim startDateStr As String = StartDate.ToString("yyyy-MM-dd")
            Dim endDateStr As String = EndDate.ToString("yyyy-MM-dd")

            Dim thisQuery As String = "SELECT DISTINCT OrderHeaders_Shutters.*, Customers.Name AS CustomerName FROM OrderHeaders_Shutters INNER JOIN Customers ON OrderHeaders_Shutters.CustomerId = Customers.Id INNER JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId INNER JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.Active = 1 AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND (OrderHeaders_Shutters.Status = 'In Production' OR OrderHeaders_Shutters.Status = 'Completed') ORDER BY Customers.Name ASC"

            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Evolve Orders Report",
                        .PagePeriod = period
                    }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim data As String() = {}

                    Dim table As New PdfPTable(9)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.05F, 0.1F, 0.1F, 0.2F, 0.15F, 0.1F, 0.1F, 0.1F, 0.1F})

                    table.AddCell(HeaderCellOriginal("No"))
                    table.AddCell(HeaderCellOriginal("Job Number"))
                    table.AddCell(HeaderCellOriginal("Job Date"))
                    table.AddCell(HeaderCellOriginal("Retailer Name"))
                    table.AddCell(HeaderCellOriginal("Retailer Order Number"))
                    table.AddCell(HeaderCellOriginal("Customer Name"))
                    table.AddCell(HeaderCellOriginal("Item Count"))
                    table.AddCell(HeaderCellOriginal("Panel Qty"))
                    table.AddCell(HeaderCellOriginal("M2"))

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim headerId As String = thisData.Tables(0).Rows(i).Item("Id").ToString()

                        Dim itemCount As Integer = GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.HeaderId = '" + headerId + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderDetails_Shutters.Active=1")

                        Dim panelQty As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.HeaderId = '" + headerId + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderDetails_Shutters.Active=1")

                        Dim squareMetre As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.SquareMetre) FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.HeaderId = '" + headerId + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderDetails_Shutters.Active=1")

                        Dim jobDate As String = String.Empty
                        If Not thisData.Tables(0).Rows(0).Item("JobDate").ToString() = "" Then
                            jobDate = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("jobDate")).ToString("dd MMM yyyy")
                        End If

                        table.AddCell(ContentCell(i + 1))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("JobId").ToString()))
                        table.AddCell(ContentCell(jobDate))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("CustomerName").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("OrderNumber").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("OrderName").ToString()))
                        table.AddCell(ContentCell(itemCount.ToString()))
                        table.AddCell(ContentCell(panelQty.ToString()))
                        table.AddCell(ContentCell(squareMetre.ToString()))
                    Next

                    Dim totalItemCount As Integer = GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")
                    Dim totalPanelQty As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")
                    Dim totalSquareMetre As Integer = GetItemData_Integer("SELECT SUM(OrderDetails_Shutters.SquareMetre) FROM OrderDetails_Shutters INNER JOIN OrderHeaders_Shutters ON OrderDetails_Shutters.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderHeaders_Shutters.Active = 1 AND OrderDetails_Shutters.Active=1")

                    table.AddCell(FooterCellCol4("TOTAL"))
                    table.AddCell(FooterCell(totalItemCount))
                    table.AddCell(FooterCell(totalPanelQty))
                    table.AddCell(FooterCell(totalSquareMetre))

                    doc.Add(table)

                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Customers(Files As String, CashSale As String)
        Try
            Dim thisQuery As String = "SELECT Customers.*, CustomerGroups.Name AS GroupName, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CashSaleCustomer, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS OnStopCustomer FROM Customers LEFT JOIN CustomerGroups ON Customers.[Group] = CustomerGroups.Id ORDER BY Customers.Name ASC"
            If CashSale = "Yes" Then
                thisQuery = "SELECT Customers.*, CustomerGroups.Name AS GroupName, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CashSaleCustomer, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS OnStopCustomer FROM Customers LEFT JOIN CustomerGroups ON Customers.[Group] = CustomerGroups.Id WHERE CashSale = 1 ORDER BY Customers.Name ASC"
            End If

            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4, 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportCustomerEvents() With {
                        .PageTitle = "Customer List"
                    }
                    writer.PageEvent = pageEvent

                    doc.Open()

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.38F, 0.2F, 0.2F, 0.12F, 0.1F})

                    table.AddCell(HeaderCellOriginal("CUSTOMER NAME"))
                    table.AddCell(HeaderCellOriginal("TYPE"))
                    table.AddCell(HeaderCellOriginal("GROUP"))
                    table.AddCell(HeaderCellOriginal("CASH SALE"))
                    table.AddCell(HeaderCellOriginal("ON STOP"))

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("Name").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("Type").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("GroupName").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("CashSaleCustomer").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("OnStopCustomer").ToString()))
                    Next

                    doc.Add(table)
                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Fabrics(Files As String, Logo As String)
        Try
            Dim ds As DataSet = GetListData("SELECT * FROM Fabrics WHERE (DesignId LIKE '%E7959750-5CF8-48E5-9171-CD71B53CDC2F%' OR DesignId LIKE '%50CE8EDF-E106-414C-BDE3-D7AA8F8046D2%' OR DesignId LIKE '%B594C6C7-452D-4CB6-8A20-6914BD97B830%' OR DesignId LIKE '%B556E35C-CEAC-40F8-A6CF-156601BD57DA%') AND Active = 1")

            Using package As New ExcelPackage()
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add("Worksheet")

                worksheet.View.ShowGridLines = False
                worksheet.Cells("A1:N1").Merge = True
                worksheet.Cells("A2:N2").Merge = True
                worksheet.Cells("A3:N3").Merge = True
                worksheet.Row(1).Height = 40
                worksheet.Row(2).Height = 25
                worksheet.Row(4).Height = 60

                Dim logoPath As String = Logo
                If File.Exists(logoPath) Then
                    Using logoStream As New FileStream(logoPath, FileMode.Open, FileAccess.Read)
                        Dim picture = worksheet.Drawings.AddPicture("Logo", logoStream)
                        picture.SetPosition(0, 0, 0, 0)
                        picture.SetSize(330, 80)
                    End Using
                Else
                    worksheet.Cells("A1").Value = "Logo Not Found"
                End If

                worksheet.Cells("A1").Value = "Lifestyle Emerge Fabric Collection"
                worksheet.Cells("A1").Style.Font.Bold = True
                worksheet.Cells("A1").Style.Font.Size = 28
                worksheet.Cells("A1").Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
                worksheet.Cells("A1").Style.VerticalAlignment = ExcelVerticalAlignment.Center

                worksheet.Cells("A2").Value = "Effective: " & Now.ToString("dd MMM yyyy")
                worksheet.Cells("A2").Style.Font.Size = 14
                worksheet.Cells("A2").Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
                worksheet.Cells("A2").Style.VerticalAlignment = ExcelVerticalAlignment.Center

                Dim headers As String() = {"Price Group", "Fabric Name", "Fabric Type", "Colours Available", "Composition", "Flame Retardant", "PVC Free = P Lead Free = L", "Greenguard Gold", "Fabric Weight (gsm)", "Vertical", "Roller", "Roman", "Panel Glide", "Usable Width (mm)"}
                Dim columnWidths As Double() = {7, 25, 11, 60, 25, 10, 12, 12, 10, 10, 10, 10, 10, 9}

                For colIndex As Integer = 0 To headers.Length - 1
                    Dim cell = worksheet.Cells(4, colIndex + 1)
                    cell.Value = headers(colIndex)
                    cell.Style.WrapText = True
                    cell.Style.Font.Bold = True
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.White)
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black)
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center
                    worksheet.Column(colIndex + 1).Width = columnWidths(colIndex)
                Next

                Dim rowIndex As Integer = 4
                For Each row As DataRow In ds.Tables(0).Rows
                    rowIndex += 1

                    Dim fabricId As String = row("Id").ToString()
                    Dim designId As String = row("DesignId").ToString()
                    Dim dataColour As DataSet = GetListData("SELECT Colour FROM FabricColours WHERE FabricId = '" & fabricId & "' AND Active = 1")
                    Dim fabricColour As String = String.Join(", ", dataColour.Tables(0).AsEnumerable().Select(Function(r) r("Colour").ToString()))

                    worksheet.Cells(rowIndex, 1).Value = row("Group").ToString().Replace("Group ", "")
                    worksheet.Cells(rowIndex, 2).Value = row("Name")
                    worksheet.Cells(rowIndex, 3).Value = row("Type")
                    worksheet.Cells(rowIndex, 4).Value = fabricColour
                    worksheet.Cells(rowIndex, 5).Value = row("Composition")
                    worksheet.Cells(rowIndex, 6).Value = If(Convert.ToBoolean(row("FlameReterdant")), "Yes", "No")
                    worksheet.Cells(rowIndex, 7).Value = row("PvcLead")
                    worksheet.Cells(rowIndex, 8).Value = If(Convert.ToBoolean(row("GreenguardGold")), "Yes", "No")
                    worksheet.Cells(rowIndex, 9).Value = row("Weight")

                    worksheet.Cells(rowIndex, 10).Value = If(designId.Contains("B556E35C-CEAC-40F8-A6CF-156601BD57DA"), "Yes", "No")
                    worksheet.Cells(rowIndex, 11).Value = If(designId.Contains("50CE8EDF-E106-414C-BDE3-D7AA8F8046D2"), "Yes", "No")
                    worksheet.Cells(rowIndex, 12).Value = If(designId.Contains("B594C6C7-452D-4CB6-8A20-6914BD97B830"), "Yes", "No")
                    worksheet.Cells(rowIndex, 13).Value = If(designId.Contains("E7959750-5CF8-48E5-9171-CD71B53CDC2F"), "Yes", "No")

                    worksheet.Cells(rowIndex, 14).Value = GetItemData_Integer("SELECT TOP 1 Width FROM FabricColours WHERE FabricId = '" + fabricId + "'")

                    worksheet.Row(rowIndex).Height = 30
                    worksheet.Row(rowIndex).Style.WrapText = True
                Next
                File.WriteAllBytes(Files, package.GetAsByteArray())
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Function HeaderCell(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.Rowspan = 2
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        Return thisCell
    End Function

    Private Function HeaderCellOriginal(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 20
        Return thisCell
    End Function

    Private Function HeaderCellProduct(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.Colspan = 2
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function HeaderCellTanggal(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function ContentCell(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 8)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function FooterCell(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function FooterCellCol4(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_RIGHT
        thisCell.MinimumHeight = 15
        thisCell.Colspan = 6
        Return thisCell
    End Function

    Private Function GetPerCustomerBlinds(Data As String()) As Integer
        Dim result As Integer = 500
        Try
            Dim customerId As String = Data(0)
            Dim designId As String = Data(1)
            Dim startDate As String = Data(2)
            Dim endDate As String = Data(3)
            Dim factory As String = Data(4)

            Dim thisQuery As String = "SELECT SUM(OrderDetails_Shutters.TotalBlinds) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.CustomerId = '" + customerId + "' AND Products.DesignId = '" + UCase(designId).ToString() + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"

            If designId = "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.CustomerId = '" + customerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            If designId = "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.CustomerId = '" + customerId + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            If designId = "83C7039F-4A2E-4D6A-9389-761664FD9449" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.TotalBlinds) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.CustomerId = '" + customerId + "' AND (Products.DesignId = 'B72EEA9A-5FA8-48EC-9307-C66AAAB1AA8F' OR Products.DesignId = 'C105C02C-E587-40FB-8AAD-0F79DD8B63AE' OR Products.DesignId = '83C7039F-4A2E-4D6A-9389-761664FD9449') AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            If designId = "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.TotalBlinds) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderHeaders_Shutters.CustomerId = '" + customerId + "' AND (Products.DesignId = '50CE8EDF-E106-414C-BDE3-D7AA8F8046D2' OR Products.DesignId = '9BD1C03C-F15F-4323-B7A0-CC988B0E231B') AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            result = GetItemData_Integer(thisQuery)
        Catch ex As Exception
            result = 500
        End Try
        Return result
    End Function

    Private Function GetPerCustomerBlinds2(Data As String()) As Integer
        Dim result As Integer = 500
        Try
            Dim customerId As String = Data(0)
            Dim designId As String = Data(1)
            Dim startDate As String = Data(2)
            Dim endDate As String = Data(3)
            Dim factory As String = Data(4)

            Dim thisQuery As String = String.Format("SELECT SUM(Qty) FROM view_details WHERE CustomerId = '{0}' AND DesignId = '{1}' AND CONVERT ( DATE, SubmittedDate ) >= '{2}' AND CONVERT ( DATE, SubmittedDate ) <= '{3}' AND Active = 1", customerId, designId, startDate, endDate)

            result = GetItemData_Integer(thisQuery)
        Catch ex As Exception
            result = 500
        End Try
        Return result
    End Function

    Private Function GetPerDesignBlinds(Data As String()) As Integer
        Dim result As Integer = 0
        Try
            Dim designId As String = Data(0)
            Dim startDate As String = Data(1)
            Dim endDate As String = Data(2)
            Dim factory As String = Data(3)

            Dim thisQuery As String = "SELECT SUM(OrderDetails_Shutters.TotalBlinds) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND Products.DesignId = '" + designId + "' AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"

            If designId = "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            If designId = "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.PanelQty) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            ' ROLLER BLIND
            If designId = "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.TotalBlinds) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND (Products.DesignId = '9BD1C03C-F15F-4323-B7A0-CC988B0E231B' OR Products.DesignId = '50CE8EDF-E106-414C-BDE3-D7AA8F8046D2') AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            ' VENETIAN
            If designId = "83C7039F-4A2E-4D6A-9389-761664FD9449" Then
                thisQuery = "SELECT SUM(OrderDetails_Shutters.TotalBlinds) FROM OrderHeaders_Shutters LEFT JOIN OrderDetails_Shutters ON OrderHeaders_Shutters.Id = OrderDetails_Shutters.HeaderId LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders_Shutters.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders_Shutters.JobDate) <= '" + endDate + "' AND (Products.DesignId = 'B72EEA9A-5FA8-48EC-9307-C66AAAB1AA8F' OR Products.DesignId = '83C7039F-4A2E-4D6A-9389-761664FD9449' OR Products.DesignId = 'C105C02C-E587-40FB-8AAD-0F79DD8B63AE') AND OrderDetails_Shutters.Active = 1 AND OrderDetails_Shutters.Production = '" + factory + "'"
            End If

            result = GetItemData_Integer(thisQuery)
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Private Function GetPerDesignBlinds2(Data As String()) As Integer
        Dim result As Integer = 500
        Try
            Dim designId As String = Data(0)
            Dim startDate As String = Data(1)
            Dim endDate As String = Data(2)
            Dim factory As String = Data(3)

            Dim thisQuery As String = String.Format("SELECT SUM(Qty) FROM view_details WHERE DesignId = '{0}' AND CONVERT ( DATE, SubmittedDate ) >= '{1}' AND CONVERT ( DATE, SubmittedDate ) <= '{2}' AND Active = 1", designId, startDate, endDate)

            result = GetItemData_Integer(thisQuery)
        Catch ex As Exception
            result = 00
        End Try
        Return result
    End Function
End Class

Public Class ReportEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String
    Public Property PagePeriod As String

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()
        Dim chunk1 As New Chunk(UCase(PageTitle).ToString(), New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD))
        phrase.Add(chunk1)
        Dim chunk2 As New Chunk(vbCrLf & vbCrLf)
        phrase.Add(chunk2)
        Dim chunk3 As New Chunk("Period : " & PagePeriod & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD))
        phrase.Add(chunk3)

        Dim leftHeaderCell As New PdfPCell(phrase)
        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(leftHeaderCell)

        Dim rightHeaderCell As New PdfPCell(New Phrase("Date : " & DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_BOTTOM
        headerTable.AddCell(rightHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 36, document.PageSize.Height - 20, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("All information within this report is private and confidential.", New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.TIMES_ROMAN, 10)))
        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(36), cb)
    End Sub
End Class

Public Class ReportCustomerEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()
        Dim chunk1 As New Chunk(UCase(PageTitle).ToString(), New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD))
        phrase.Add(chunk1)
        Dim chunk2 As New Chunk(vbCrLf & vbCrLf)
        phrase.Add(chunk2)

        Dim leftHeaderCell As New PdfPCell(phrase)
        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(leftHeaderCell)

        Dim rightHeaderCell As New PdfPCell(New Phrase("Date : " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)))
        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_BOTTOM
        headerTable.AddCell(rightHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 36, document.PageSize.Height - 36, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("All information within this report is private and confidential.", New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)))
        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.TIMES_ROMAN, 8)))
        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(36), cb)
    End Sub
End Class