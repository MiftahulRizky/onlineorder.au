Partial Class Setting_Discount_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Session("RoleName") <> "Administrator" AndAlso Session("RoleName") <> "Account" Then
        ' If Session("RoleName") <> "Administrator" Then
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Account" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            txtSearch.Text = Session("discountSearch")
            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("discountSearch") = txtSearch.Text
        Response.Redirect("~/setting/discount/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("discountSearch") = txtSearch.Text
            Session("discountDetail") = UCase(row.Cells(1).Text)
            Response.Redirect("~/setting/discount/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            sdsApp.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE DISCOUNT. ID : " & lblId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(6).Text

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = UCase(row.Cells(1).Text)
            lblActive.Text = newActive

            sdsApp.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE / DEACTIVATE DISCOUNT. ID : " & lblId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Session("discountDetail") = "" : Session("discountSearch") = ""
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " WHERE Stores.Id LIKE '%" + SearchText + "%' OR Stores.Name LIKE '%" + SearchText + "%' OR Designs.Name LIKE '%" + SearchText + "%' OR PricesGroup.Name LIKE '%" + SearchText + "%'"
            End If
            Dim myQuery As String = String.Format("SELECT Discounts.*, Stores.Name AS StoreName, Designs.Name + ', ' + PricesGroup.Name AS PriceGroupName FROM Discounts INNER JOIN Stores ON Discounts.StoreId = Stores.Id INNER JOIN PricesGroup ON Discounts.PriceGroupId = PricesGroup.Id INNER JOIN Designs ON PricesGroup.DesignId = Designs.Id {0} ORDER BY Discounts.StoreId, PricesGroup.Name ASC", search)

            gvList.DataSource = publicCfg.GetListData(myQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Protected Function IconActiveOnNames(Active As String, StoreName As String) As String
        Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + StoreName
        If Active = "True" Then : Return "<i class='fa-regular text-green fa-circle-check'></i> " + StoreName : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat fs-3 me-2 opacity-50'></i> Switch Activated"
        ' Dim result As String = "<i class='fa-solid fa-arrows-rotate'></i>"
        ' If Active = "True" Then : Return "<i class='fa-solid fa-arrows-rotate'></i>" : End If
        Return result
    End Function

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-green"
        ' If Active = False Then : result = "btn btn-secondary" : End If
        Return result
    End Function
End Class
