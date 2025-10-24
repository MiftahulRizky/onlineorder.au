Partial Class Setting_Fabric_Default
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
            ddlDesign.SelectedValue = Session("fabricDesign")
            txtSearch.Text = Session("fabricSearch")
            Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)

        Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)

        Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("fabricDesign") = ddlDesign.SelectedValue
        Session("fabricSearch") = txtSearch.Text
        Response.Redirect("~/setting/fabric/add", False)
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

            Session("fabricDesign") = ddlDesign.SelectedValue
            Session("fabricSearch") = txtSearch.Text
            Session("fabricDetail") = row.Cells(0).Text
            Response.Redirect("~/setting/fabric/detail", False)
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
            publicCfg.InsertActivity(userId, Page.Title, "DELETE FABRIC. ID : " & lblId.Text)

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

            lblId.Text = row.Cells(0).Text
            lblActive.Text = newActive

            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE FABRIC. ID : " & lblId.Text)

            Call BindData(ddlDesign.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(DesignId As String, SearchText As String)
        Session("fabricDetail") = "" : Session("fabricSearch") = ""
        Try
            Dim strLike As String = " (Fabrics.Id LIKE '%" + SearchText + "%' OR Fabrics.Name LIKE '%" + SearchText + "%' OR Designs.Name LIKE '%" + SearchText + "%')"
            Dim strFromat As String = String.Empty

            If Not DesignId = "" Then
                strFromat = " WHERE Fabrics.DesignId='" + UCase(DesignId).ToString() + "'"
                If Not SearchText = "" Then
                    strFromat += " AND" & strLike
                End If
            End If

            If DesignId = "" And Not SearchText = "" Then
                strFromat = " WHERE" & strLike
            End If

            Dim myQuery As String = String.Format("SELECT Fabrics.*, Designs.Name AS DesignName FROM Fabrics LEFT JOIN Designs ON Fabrics.DesignId = Designs.Id {0} ORDER BY Fabrics.Name ASC", strFromat)

            gvList.DataSource = publicCfg.GetListData(myQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Company = 'SP' AND Active = 1 ORDER BY Name ASC")
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

    Protected Function IconActiveOnNames(Active As String, Name As String) As String
        Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-green fa-circle-check'></i> " + Name : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat me-2 opacity-50 fs-3'></i>Switch Activated"
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

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
