Partial Class Order_Default
    Inherits Page

    Dim publicCfg As New PublicConfig
    Dim DownloadCSV As New DownloadCSV
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            Call BindListStatus()
            ddlStatus.SelectedValue = Session("orderStatus")
            txtSearch.Text = Session("orderSearch")
            ddlActive.SelectedValue = Session("orderActive")
            ddlStoreType.SelectedValue = Session("orderStoreType")

            Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
            Call PermissionAction()
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("headerAction") = "AddHeader"
        Response.Redirect("~/order/header", False)
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "gvList_PageIndexChanging", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub linkChange_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim headerId As String = row.Cells(1).Text

            Session("orderStatus") = ddlStatus.SelectedValue
            Session("orderSearch") = txtSearch.Text
            Session("orderActive") = ddlActive.SelectedValue
            Session("orderStoreType") = ddlStoreType.SelectedValue
            Session("changeFrom") = "Header"
            Session("headerId") = headerId

            Response.Redirect("~/order/change", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "linkChange_Click", ex.ToString())
            End If
        End Try
    End Sub


    ' Protected Sub linkDownloadCSV_Click(sender As Object, e As EventArgs)
    '     Try
    '         Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
    '         Dim row As GridViewRow = gvList.Rows(rowIndex)
    '         Dim headerId As String = row.Cells(1).Text

    '         DownloadCSV.ExportOrderToCSV(Response, headerId)
    '     Catch ex As Exception
    '         Call MessageError(True, ex.ToString())
    '         If Not Session("RoleName") = "Administrator" Then
    '             Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
    '             publicCfg.MailError(Session("UserId"), Page.Title, "linkDownloadCSV_Click", ex.ToString())
    '         End If
    '     End Try
    ' End Sub

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim headerId As String = row.Cells(1).Text
            Session("orderSearch") = txtSearch.Text
            Session("headerId") = headerId
            Session("orderActive") = ddlActive.SelectedValue
            Session("orderStoreType") = ddlStoreType.SelectedValue
            Session("orderStatus") = ddlStatus.SelectedValue

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "linkView_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblHeaderId.Text = txtIdDelete.Text
            sdsDelete.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE ORDER : " & lblHeaderId.Text)

            lblHeaderId.Text = ""

            Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnDelete_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnRestore_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblHeaderId.Text = txtIdRestore.Text
            sdsRestore.Update()
            publicCfg.InsertActivity(Session("UserId"), Page.Title, "RESTORE ORDER : " & lblHeaderId.Text)
            Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnRestore_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlStoreType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlStoreType.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Private Sub BindListStatus()
        ddlStatus.Items.Clear()
        Try
            ddlStatus.Items.Add(New ListItem("ALL", ""))
            ddlStatus.Items.Add(New ListItem("DRAFT", "Draft"))
            ddlStatus.Items.Add(New ListItem("NEW ORDER", "New Order"))
            ddlStatus.Items.Add(New ListItem("IN PRODUCTION", "In Production"))
            ddlStatus.Items.Add(New ListItem("ON HOLD", "On Hold"))
            ddlStatus.Items.Add(New ListItem("COMPLETED", "Completed"))
            ddlStatus.Items.Add(New ListItem("CANCELED", "Canceled"))

            If Session("RoleName") = "PPIC & DE" Then
                ddlStatus.Items.Clear()

                ddlStatus.Items.Add(New ListItem("ALL", ""))
                ddlStatus.Items.Add(New ListItem("NEW ORDER", "New Order"))
                ddlStatus.Items.Add(New ListItem("IN PRODUCTION", "In Production"))
                ddlStatus.Items.Add(New ListItem("ON HOLD", "On Hold"))
                ddlStatus.Items.Add(New ListItem("COMPLETED", "Completed"))
                ddlStatus.Items.Add(New ListItem("CANCELED", "Canceled"))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindListStatus", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataOrder(Search As String, Status As String, Type As String, Active As String)
        Session("designId") = "" : Session("headerAction") = "" : Session("headerId") = ""
        Session("itemId") = "" : Session("itemAction") = ""
        Session("orderSearch") = "" : Session("orderStatus") = ""
        Session("changeFrom") = ""
        Session("orderActive") = ""
        Session("orderStoreType") = ""
        Try
            Dim byStatus As String = ""
            Dim bySearch As String = ""
            Dim byOrderBy As String = " ORDER BY CreatedDate DESC"
            Dim byActive As String = " WHERE Active = 1"
            Dim byRole As String = ""
            Dim byType As String = ""

            If Active = "0" Then : byActive = " WHERE Active = 0" : End If

            If Not Status = "" Then
                byStatus = " And Status = '" + Status + "'"
                If Status = "New Order" Or Status = "In Production" Or Status = "On Hold" Then
                    byOrderBy = " ORDER BY SubmittedDate DESC"
                End If
                If Status = "Canceled" Then
                    byOrderBy = " ORDER BY CanceledDate DESC"
                End If
                If Status = "Completed" Then
                    byOrderBy = " ORDER BY CompletedDate DESC"
                End If
            End If

            If Not Search = "" Then
                bySearch = " AND (Id LIKE '%" + Search + "%' OR UserId LIKE '%" + Search + "%' OR UserName LIKE '%" + Search + "%' OR StoreId LIKE '%" + Search + "%' OR OrderNo LIKE '%" + Search + "%' OR OrderCust LIKE '%" + Search + "%' OR StoreName LIKE '%" + Search + "%' OR JoNumber LIKE '%" + Search + "%')"
            End If

            If Session("RoleName") = "PPIC & DE" Then
                byRole = " AND StoreCompany = '" + Session("StoreCompany").ToString() + "'"
            End If

            If Session("RoleName") = "Customer" Then
                byRole = " AND StoreId = '" + UCase(Session("StoreId")).ToString() + "'"
                If Session("LevelName") = "Member" Then
                    byRole = " AND UserId = '" + UCase(Session("UserId")).ToString() + "'"
                End If
            End If

            If Not Type = "" Then
                byType = " AND StoreType = '" + Type + "'"
            End If

            Dim thisQuery As String = String.Format("SELECT * FROM view_headers " & " {0} {1} {2} {3} {4} {5}", byActive, byRole, byType, byStatus, bySearch, byOrderBy)

            gvList.DataSource = publicCfg.GetListData(thisQuery)
            gvList.DataBind()

            tblInfo.Visible = False
            If gvList.Rows.Count > 0 Then : tblInfo.Visible = True : End If

            divActive.Visible = False : divStoreType.Visible = False
            If Session("RoleName") = "Administrator" Then
                divActive.Visible = True : divStoreType.Visible = True
            End If

            gvList.Columns(2).Visible = True
            If Session("RoleName") = "Customer" Then
                gvList.Columns(2).Visible = False
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataOrder", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Protected Function VisibleChange(Status As String, Active As String) As Boolean
        Dim result As Boolean = False
        If Session("RoleName") = "PPIC & DE" Then
            result = True
            If Status = "Draft" Then : result = False : End If
        End If
        If Session("RoleName") = "Administrator" Then : result = True : End If
        If Status = "Completed" Or Status = "Canceled" Then : result = False : End If
        If Active = False Then : result = False : End If
        Return result
    End Function

    ' Protected Function VisibleDownloadCSV(Status As String, Active As String) As Boolean
    '     Dim result As Boolean = False
    '     '#
    '     If Session("RoleName") = "PPIC & DE" Then
    '         result = False
    '         If Status = "Draft" Then : result = False : End If
    '     End If

    '     If Session("RoleName") = "Administrator" Then 
    '       result = True 
    '       If Status = "Draft" Then : result = False : End If
    '     End If

    '     If Status = "Canceled" Then : result = False : End If
    '     If Active = False Then : result = False : End If
    '     Return result
    ' End Function

    Protected Function VisibleDelete(Status As String, UserId As String) As Boolean
        Dim result As Boolean = False
        If Status = "Draft" Then
            result = True
            If Session("RoleName") = "PPIC & DE" And Not UCase(Session("UserId")) = UCase(UserId).ToString() Then
                result = False
            End If
        End If
        If Session("RoleName") = "Administrator" And Not Status = "Canceled" Then
            result = True
        End If
        If Session("RoleName") = "Manager" Or Session("RoleName") = "Account" Then
            result = False
        End If
        Return result
    End Function

    Protected Function VisibleRestore(Active As Boolean) As Boolean
        Dim result As Boolean = False
        If Session("RoleName") = "Administrator" And Active = False Then
            result = True
        End If
        Return result
    End Function

    Protected Function TrigerJoNumber(JoNumber As String) As String
        Dim result As String = "<span class='badge badge-outline text-red'>" + JoNumber +"</span>"
        If JoNumber = "" Then
            result = ""
        End If
        Return result
    End Function

    Protected Function TrigerDelivery(Delivery As String) As String
        Dim result As String = "<span class='badge bg-pink-lt'><i class='bi bi-truck-front'></i> " + Delivery +"</span>"
        If Delivery = "Pick Up" Then
            result = "<span class='badge bg-cyan-lt'><i class='bi bi-box-seam'></i> " + Delivery +"</span>"
        End If
        Return result
    End Function

    Protected Function PermissionAction()
        If Session("RoleName") = "Manager" Or Session("RoleName") = "Account" Then
            btnAdd.Visible = False
        End If
    End Function
End Class






