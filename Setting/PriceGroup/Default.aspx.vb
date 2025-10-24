Partial Class Setting_PriceGroup_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            Call BindDesign()

            ddlDesign.SelectedValue = Session("priceGroupDesign")
            txtSearch.Text = Session("priceGroupSearch")

            Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("priceGroupDesign") = ddlDesign.SelectedValue
        Session("priceGroupSearch") = txtSearch.Text
        Response.Redirect("~/setting/pricegroup/add", False)
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("priceGroupSearch") = txtSearch.Text
            Session("priceGroupDesign") = ddlDesign.SelectedValue
            Session("priceGroupDetail") = UCase(row.Cells(1).Text)
            Response.Redirect("~/setting/pricegroup/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            sdsPage.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE PRICE GROUP. ID : " & lblId.Text)

            Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(5).Text

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = UCase(row.Cells(1).Text)
            lblActive.Text = newActive

            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE PRICE GROUP. ID : " & lblId.Text)

            Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(DesignText As String, SearchText As String)
        Session("priceGroupDetail") = "" : Session("priceGroupSearch") = "" : Session("priceGroupDesign") = ""
        Try
            Dim stringSearch As String = String.Empty
            Dim designSearch As String = " WHERE DesignId = '" + DesignText + "'"
            If DesignText = "" Then
                designSearch = " WHERE DesignId IS NOT NULL"
            End If

            If Not SearchText = "" Then
                stringSearch = " AND (PricesGroup.Name LIKE '%" + SearchText + "%' OR Designs.Name LIKE '%" + SearchText + "%')"
            End If
            Dim myQuery As String = String.Format("SELECT PricesGroup.*, Designs.Name AS DesignName FROM PricesGroup LEFT JOIN Designs ON PricesGroup.DesignId = Designs.Id {0} {1} ORDER BY Designs.Name, PricesGroup.Name ASC", designSearch, stringSearch)

            gvList.DataSource = publicCfg.GetListData(myQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub
    Private Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()
            If ddlDesign.Items.Count > 1 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
    Protected Function IconActiveOnNames(Active As String, Name As String) As String
       Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-green fa-circle-check'></i> " + Name : End If
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
